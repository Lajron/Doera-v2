using Doera.Application.Abstractions.Results;
using Doera.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Doera.Application.Interfaces.Identity {
    public interface IIdentityService {
        Task<Result<Guid>> RegisterAsync(string email, string password);
        Task<Result> PasswordSignInAsync(string email, string password, bool isPersistent, bool lockoutOnFailure);
        Task<Result> SignOutAsync();
        Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task<Result<User>> FindByEmailAsync(string email);
        Task<Result<User>> FindByIdAsync(Guid id);
    }
}