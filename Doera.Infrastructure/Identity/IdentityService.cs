using Doera.Application.Abstractions.Results;
using Doera.Application.Interfaces.Identity;
using Doera.Core.Entities;
using Doera.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Doera.Infrastructure.Identity {
    internal class IdentityService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IUnitOfWork _uow
    ) : IIdentityService {

        public async Task<Result<Guid>> RegisterAsync(string email, string password) {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing is not null)
                return Errors.Identity.DuplicateEmail();

            var user = new User { UserName = email, Email = email };

            var pwdErrors = new List<Error>();
            foreach (var validator in userManager.PasswordValidators) {
                var res = await validator.ValidateAsync(userManager, user, password);
                if (!res.Succeeded)
                    pwdErrors.AddRange(res.Errors.Select(e => Errors.Common.Validation(e.Description)));
            }
            if (pwdErrors.Count > 0)
                return pwdErrors;

            var create = await userManager.CreateAsync(user, password);
            if (!create.Succeeded)
                return Errors.Identity.RegistrationFailed();

            await _uow.TodoLists.AddAsync(
                new TodoList {
                    Name = "My Tasks",
                    UserId = user.Id,
                    Order = 0
                }
            );
            await _uow.CompleteAsync();

            return user.Id;
        }

        public async Task<Result> PasswordSignInAsync(string email, string password, bool isPersistent, bool lockoutOnFailure) {
            var result = await signInManager.PasswordSignInAsync(email, password, isPersistent, lockoutOnFailure);
            if (result.Succeeded) return Result.Success();
            if (result.IsLockedOut) return Errors.Identity.LockedOut();
            if (result.RequiresTwoFactor) return Errors.Identity.RequiresTwoFactor();
            return Errors.Identity.InvalidCredentials();
        }

        public async Task<Result> SignOutAsync() {
            await signInManager.SignOutAsync();
            return Result.Success();
        }

        public async Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword) {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Errors.Identity.NotFound();
            var change = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return change.Succeeded
                ? Result.Success()
                : change.Errors.Select(e => Errors.Common.Validation(e.Description)).ToList();
        }

        public async Task<Result<User>> FindByEmailAsync(string email) {
            var user = await userManager.FindByEmailAsync(email);
            return user is null
                ? Errors.Identity.NotFound()
                : user;
        }

        public async Task<Result<User>> FindByIdAsync(Guid id) {
            var user = await userManager.FindByIdAsync(id.ToString());
            return user is null
                ? Errors.Identity.NotFound()
                : user;
        }
    }
}