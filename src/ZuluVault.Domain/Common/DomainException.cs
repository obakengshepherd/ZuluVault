namespace ZuluVault.Domain.Common;

/// <summary>
/// Base exception for domain business logic violations
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception for invalid wallet operations
/// </summary>
public class InvalidWalletOperationException : DomainException
{
    public InvalidWalletOperationException(string message) : base(message) { }
}

/// <summary>
/// Exception for insufficient funds
/// </summary>
public class InsufficientFundsException : DomainException
{
    public InsufficientFundsException(string message) : base(message) { }
}

/// <summary>
/// Exception for daily transfer limits exceeded
/// </summary>
public class DailyLimitExceededException : DomainException
{
    public DailyLimitExceededException(string message) : base(message) { }
}

/// <summary>
/// Exception for locked wallet
/// </summary>
public class WalletLockedException : DomainException
{
    public WalletLockedException(string message) : base(message) { }
}

/// <summary>
/// Exception for duplicate idempotency key
/// </summary>
public class DuplicateIdempotencyKeyException : DomainException
{
    public DuplicateIdempotencyKeyException(string message) : base(message) { }
}
