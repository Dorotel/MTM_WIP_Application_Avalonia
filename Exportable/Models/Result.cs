using System;

namespace MTM.Core.Models
{
    /// <summary>
    /// Represents the result of an operation that can either succeed or fail.
    /// This is a framework-agnostic implementation of the Result pattern for error handling.
    /// </summary>
    /// <typeparam name="T">The type of the success value</typeparam>
    public class Result<T>
    {
        private Result(T value)
        {
            IsSuccess = true;
            Value = value;
            Error = string.Empty;
        }

        private Result(string error)
        {
            IsSuccess = false;
            Value = default!;
            Error = error;
        }

        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Gets the success value. Only valid when IsSuccess is true.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Gets the error message. Only valid when IsSuccess is false.
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// Creates a successful result with the specified value.
        /// </summary>
        /// <param name="value">The success value</param>
        /// <returns>A successful result</returns>
        public static Result<T> Success(T value)
        {
            return new Result<T>(value);
        }

        /// <summary>
        /// Creates a failed result with the specified error message.
        /// </summary>
        /// <param name="error">The error message</param>
        /// <returns>A failed result</returns>
        public static Result<T> Failure(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
                throw new ArgumentException("Error message cannot be null or empty", nameof(error));

            return new Result<T>(error);
        }

        /// <summary>
        /// Creates a failed result from an exception.
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <returns>A failed result</returns>
        public static Result<T> Failure(Exception exception)
        {
            return new Result<T>(exception.Message);
        }

        /// <summary>
        /// Implicitly converts a value to a successful result.
        /// </summary>
        /// <param name="value">The value</param>
        public static implicit operator Result<T>(T value)
        {
            return Success(value);
        }

        /// <summary>
        /// Implicitly converts an error string to a failed result.
        /// </summary>
        /// <param name="error">The error message</param>
        public static implicit operator Result<T>(string error)
        {
            return Failure(error);
        }

        /// <summary>
        /// Matches the result and executes the appropriate function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="onSuccess">Function to execute on success</param>
        /// <param name="onFailure">Function to execute on failure</param>
        /// <returns>The result of the executed function</returns>
        public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure)
        {
            return IsSuccess ? onSuccess(Value) : onFailure(Error);
        }

        /// <summary>
        /// Maps the success value to a new type.
        /// </summary>
        /// <typeparam name="TNew">The new type</typeparam>
        /// <param name="mapper">Function to map the value</param>
        /// <returns>A new result with the mapped value</returns>
        public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
        {
            return IsSuccess ? Result<TNew>.Success(mapper(Value)) : Result<TNew>.Failure(Error);
        }

        /// <summary>
        /// Binds the result to another operation that returns a result.
        /// </summary>
        /// <typeparam name="TNew">The new type</typeparam>
        /// <param name="binder">Function that returns a new result</param>
        /// <returns>The bound result</returns>
        public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> binder)
        {
            return IsSuccess ? binder(Value) : Result<TNew>.Failure(Error);
        }

        /// <summary>
        /// Returns the value if successful, otherwise returns the default value.
        /// </summary>
        /// <param name="defaultValue">The default value to return on failure</param>
        /// <returns>The value or default value</returns>
        public T GetValueOrDefault(T defaultValue = default!)
        {
            return IsSuccess ? Value : defaultValue;
        }

        /// <summary>
        /// Throws an exception if the result is a failure.
        /// </summary>
        /// <returns>The success value</returns>
        /// <exception cref="InvalidOperationException">Thrown if the result is a failure</exception>
        public T ThrowIfFailure()
        {
            if (IsFailure)
                throw new InvalidOperationException(Error);

            return Value;
        }

        /// <summary>
        /// Returns a string representation of the result.
        /// </summary>
        public override string ToString()
        {
            return IsSuccess ? $"Success: {Value}" : $"Failure: {Error}";
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current result.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is not Result<T> other)
                return false;

            if (IsSuccess != other.IsSuccess)
                return false;

            return IsSuccess
                ? Equals(Value, other.Value)
                : string.Equals(Error, other.Error, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns a hash code for the current result.
        /// </summary>
        public override int GetHashCode()
        {
            return IsSuccess
                ? (Value?.GetHashCode() ?? 0)
                : Error.GetHashCode(StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// Represents the result of an operation that can either succeed or fail without a return value.
    /// </summary>
    public class Result
    {
        private Result(bool isSuccess, string error = "")
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Gets the error message. Only valid when IsSuccess is false.
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        public static Result Success()
        {
            return new Result(true);
        }

        /// <summary>
        /// Creates a failed result with the specified error message.
        /// </summary>
        /// <param name="error">The error message</param>
        public static Result Failure(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
                throw new ArgumentException("Error message cannot be null or empty", nameof(error));

            return new Result(false, error);
        }

        /// <summary>
        /// Creates a failed result from an exception.
        /// </summary>
        /// <param name="exception">The exception</param>
        public static Result Failure(Exception exception)
        {
            return new Result(false, exception.Message);
        }

        /// <summary>
        /// Implicitly converts an error string to a failed result.
        /// </summary>
        public static implicit operator Result(string error)
        {
            return Failure(error);
        }

        /// <summary>
        /// Matches the result and executes the appropriate action.
        /// </summary>
        /// <param name="onSuccess">Action to execute on success</param>
        /// <param name="onFailure">Action to execute on failure</param>
        public void Match(Action onSuccess, Action<string> onFailure)
        {
            if (IsSuccess)
                onSuccess();
            else
                onFailure(Error);
        }

        /// <summary>
        /// Throws an exception if the result is a failure.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the result is a failure</exception>
        public void ThrowIfFailure()
        {
            if (IsFailure)
                throw new InvalidOperationException(Error);
        }

        /// <summary>
        /// Returns a string representation of the result.
        /// </summary>
        public override string ToString()
        {
            return IsSuccess ? "Success" : $"Failure: {Error}";
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current result.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is not Result other)
                return false;

            if (IsSuccess != other.IsSuccess)
                return false;

            return IsSuccess || string.Equals(Error, other.Error, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns a hash code for the current result.
        /// </summary>
        public override int GetHashCode()
        {
            return IsSuccess ? 1 : Error.GetHashCode(StringComparison.Ordinal);
        }
    }
}