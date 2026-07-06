#!/usr/bin/env bash
# Deterministic structural guardrail checks for the DonationService harness.
# No model required. Exit non-zero if any guardrail is violated. Safe to run in CI.
set -uo pipefail

cd "$(dirname "$0")/.." || exit 2
fail=0
pass() { echo "PASS: $1"; }
bad()  { echo "FAIL: $1"; fail=1; }

echo "== Guardrail checks =="

# 1. PII must never be logged. Flag logging calls that reference sensitive fields.
#    (We log the donation Id, never RequesterName/Location/RequesterId.)
pii_hits=$(grep -rniE 'log[a-z]*\.[a-z(]*.*(RequesterName|Location|RequesterId)' \
  --include='*.cs' src 2>/dev/null || true)
if [ -n "$pii_hits" ]; then
  bad "PII referenced in a logging call:"
  echo "$pii_hits"
else
  pass "no PII (RequesterName/Location/RequesterId) in logging calls"
fi

# 2. Controllers stay thin — no repository/DbContext access from the Api layer.
ctrl_data=$(grep -rniE '(IDonationRepository|DbContext|\.SaveChanges)' \
  --include='*.cs' src/Api 2>/dev/null || true)
if [ -n "$ctrl_data" ]; then
  bad "data access found in Api layer (belongs in Infrastructure):"
  echo "$ctrl_data"
else
  pass "no data access in the Api layer"
fi

# 3. Data-store access lives only in Infrastructure — Application must not touch a concrete store.
app_data=$(grep -rniE '(DbContext|\.SaveChanges|new SqlConnection)' \
  --include='*.cs' src/Application 2>/dev/null || true)
if [ -n "$app_data" ]; then
  bad "concrete data-store access found in Application layer:"
  echo "$app_data"
else
  pass "Application layer depends only on interfaces"
fi

# 4. Every handler has a matching test file (heuristic: a *Tests.cs referencing the handler name).
missing=""
while IFS= read -r h; do
  name=$(basename "$h" .cs)
  if ! grep -rql "$name" --include='*Tests.cs' tests 2>/dev/null; then
    missing="$missing $name"
  fi
done < <(grep -rls 'IRequestHandler' --include='*Handler.cs' src/Application 2>/dev/null)
if [ -n "$missing" ]; then
  bad "handlers without a referencing test:$missing"
else
  pass "every handler is referenced by a test"
fi

echo "======================"
[ "$fail" -eq 0 ] && echo "All guardrails passed." || echo "Guardrail violations found."
exit "$fail"
