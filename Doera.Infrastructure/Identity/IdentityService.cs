using Doera.Application.Abstractions.Results;
using Doera.Application.Interfaces.Identity;
using Doera.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Doera.Infrastructure.Identity {
    internal class IdentityService(
        UserManager<User> userManager,
        SignInManager<User> signInManager
    ) : IIdentityService {

        public async Task<Result<Guid>> RegisterAsync(string email, string password) {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing is not null)
                return Result<Guid>.Failure(new Error("DuplicateEmail", "Email is already registered.", "Email"));

            var user = new User { UserName = email, Email = email };

            var pwdErrors = new List<Error>();
            foreach (var validator in userManager.PasswordValidators) {
                var res = await validator.ValidateAsync(userManager, user, password);
                if (!res.Succeeded)
                    pwdErrors.AddRange(res.Errors.Select(e => new Error(e.Code, e.Description, "Password")));
            }
            if (pwdErrors.Count > 0)
                return Result<Guid>.Failure(pwdErrors);

            var create = await userManager.CreateAsync(user, password);
            if (!create.Succeeded)
                return Result<Guid>.Failure(new Error("RegistrationFailed", "Could not complete registration. Please try again."));


            return Result<Guid>.Success(user.Id);
        }

        public async Task<Result> PasswordSignInAsync(string email, string password, bool isPersistent, bool lockoutOnFailure) {
            var result = await signInManager.PasswordSignInAsync(email, password, isPersistent, lockoutOnFailure);
            if (result.Succeeded) return Result.Success();
            if (result.IsLockedOut) return Result.Failure(new Error("LockedOut", "Your account is locked."));
            if (result.RequiresTwoFactor) return Result.Failure(new Error("RequiresTwoFactor", "Two-factor authentication is required."));
            return Result.Failure(new Error("InvalidCredentials", "Invalid email or password."));
        }

        public async Task<Result> SignOutAsync() {
            await signInManager.SignOutAsync();
            return Result.Success();
        }

        public async Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword) {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Result.Failure(new Error("UserNotFound", "User not found."));
            var change = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return change.Succeeded
                ? Result.Success()
                : Result.Failure(change.Errors.Select(e => new Error(e.Code, e.Description, "Password")));
        }

        public async Task<Result<User>> FindByEmailAsync(string email) {
            var user = await userManager.FindByEmailAsync(email);
            return user is null
                ? Result<User>.Failure(new Error("NotFound", "User not found.", "Email"))
                : Result<User>.Success(user);
        }

        public async Task<Result<User>> FindByIdAsync(Guid id) {
            var user = await userManager.FindByIdAsync(id.ToString());
            return user is null
                ? Result<User>.Failure(new Error("NotFound", "User not found."))
                : Result<User>.Success(user);
        }
    }
}