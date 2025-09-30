using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Application.Abstractions.Results {
    public static class Errors {
        public static class Common {
            public static Error Validation(string description) => new("Common.Validation", description, ErrorKind.Validation);
            public static Error NotFound(string what) => new("Common.NotFound", $"{what} was not found.", ErrorKind.NotFound);
            public static Error AccessDenied(string? description = null) => new("Common.AccessDenied", description ?? "You do not have permission to perform this action.", ErrorKind.AccessDenied);
            public static Error Conflict(string description) => new("Common.Conflict", description, ErrorKind.Conflict);
            public static Error Failed(string description) => new("Common.Failed", description, ErrorKind.Failed);
        }

        public static class TodoList {
            public static Error NotFound() => new("TodoList.NotFound", "The specified todo list was not found.", ErrorKind.NotFound);
            public static Error CreateFailed() => new("TodoList.CreateFailed", "Could not create the list.", ErrorKind.Failed);
            public static Error UpdateFailed() => new("TodoList.UpdateFailed", "Could not update the list.", ErrorKind.Failed);
        }

        public static class TodoItem {
            public static Error NotFound() => new("TodoItem.NotFound", "The specified todo item was not found.", ErrorKind.NotFound);
            public static Error CreateFailed() => new("TodoItem.CreateFailed", "Could not create the item.", ErrorKind.Failed);
            public static Error UpdateFailed() => new("TodoItem.UpdateFailed", "Could not update the item.", ErrorKind.Failed);
        }

        public static class Identity {
            public static Error InvalidCredentials() => new("Identity.InvalidCredentials", "Invalid email or password.", ErrorKind.Validation);
            public static Error LockedOut() => new("Identity.LockedOut", "Your account is locked.", ErrorKind.RateLimited);
            public static Error RequiresTwoFactor() => new("Identity.RequiresTwoFactor", "Two-factor authentication is required.", ErrorKind.Unauthorized);
            public static Error RegistrationFailed() => new("Identity.RegistrationFailed", "Could not complete registration. Please try again.", ErrorKind.Failed);
            public static Error DuplicateEmail() => new("Identity.DuplicateEmail", "An account with this email already exists.", ErrorKind.Conflict);
            public static Error NotFound() => new("Identity.NotFound", "User not found.", ErrorKind.NotFound);
        }
    }
}
