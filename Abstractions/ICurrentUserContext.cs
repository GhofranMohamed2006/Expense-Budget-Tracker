namespace Pennywise.Common.Abstractions;

public interface ICurrentUserContext
{
    string? UserId { get; }
}
