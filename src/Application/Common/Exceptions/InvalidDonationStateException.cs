namespace Application.Common.Exceptions;

/// <summary>Thrown when a donation request is not in a state that allows the requested transition. Mapped to HTTP 409.</summary>
public class InvalidDonationStateException : Exception
{
    public InvalidDonationStateException(string message) : base(message) { }
}
