using Doera.Application.Interfaces.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace Doera.Infrastructure.Identity {
    internal class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser {
        public Guid? UserId { 
            get {
                var principal = accessor.HttpContext?.User;
                if (principal?.Identity?.IsAuthenticated != true) return null;
                var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(id, out var guid) ? guid : null;
            }
        }

        public string? Email => accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

        public Guid RequireUserId() => UserId ?? throw new UnauthorizedAccessException();
    }
}