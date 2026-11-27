namespace Pennywise.BLL.Abstractions;

public interface ICurrentUserContext
{
    string? UserId { get; }
}
