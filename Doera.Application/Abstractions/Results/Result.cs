using System.Collections.Generic;

namespace Doera.Application.Abstractions.Results {
    public class Result {
        public bool Succeeded { get; }
        public IReadOnlyList<Error> Errors { get; }

        protected Result(bool succeeded, IReadOnlyList<Error> errors) {
            Succeeded = succeeded;
            Errors = errors;
        }

        public static Result Success() => new(true, []);
        public static Result Failure(params Error[] errors) => new(false, errors);
        public static Result Failure(IEnumerable<Error> errors) => new(false, [.. errors]);

        public static implicit operator Result(Error error) => Failure(error);
        public static implicit operator Result(List<Error> errors) => Failure(errors);
    }

    public sealed class Result<T> : Result {
        public T? Value { get; }

        private Result(bool succeeded, T? value, IReadOnlyList<Error> errors)
            : base(succeeded, errors) {
            Value = value;
        }

        public static Result<T> Success(T value) => new(true, value, []);
        public static new Result<T> Failure(params Error[] errors) => new(false, default, errors);
        public static new Result<T> Failure(IEnumerable<Error> errors) => new(false, default, [.. errors]);

        // Implicit conversion from T to Result<T>
        // This allows returning a value of type T directly where Result<T> is expected
        // e.g., return someValue; instead of return Result<T>.Success(someValue);
        public static implicit operator Result<T>(T value) => Success(value);
        public static implicit operator Result<T>(Error error) => Failure(error);
        public static implicit operator Result<T>(List<Error> errors) => Failure(errors);
    }
}