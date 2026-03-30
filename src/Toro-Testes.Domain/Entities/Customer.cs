using Toro.Testes.BuildingBlocks.Abstractions;
using Toro.Testes.BuildingBlocks.Constants;

namespace Toro.Testes.Domain.Entities;

public sealed class Customer : AuditableEntity
{
    private Customer()
    {
    }

    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string DocumentNumber { get; private set; } = string.Empty;
    public string Role { get; private set; } = ApplicationConstants.Roles.Investor;

    public static Customer Create(string fullName, string email, string documentNumber, string role)
        => new()
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Email = email.ToLowerInvariant(),
            DocumentNumber = documentNumber,
            Role = role
        };
}
