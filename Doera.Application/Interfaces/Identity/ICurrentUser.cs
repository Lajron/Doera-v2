using System;

namespace Doera.Application.Interfaces.Identity {
    public interface ICurrentUser {
        Guid? UserId { get; }
        string? Email { get; }
        Guid RequireUserId();
    }
}