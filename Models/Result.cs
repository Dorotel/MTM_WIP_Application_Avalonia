using System;
using System.Collections.Generic;

namespace MTM.Models
{
    /// <summary>
    /// Represents the result of an operation that can either succeed or fail.
    /// Framework-agnostic implementation compatible with any .NET application.
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; protected set; }
        public bool IsFailure => !IsSuccess;
        public string? ErrorMessage { get; protected set; }
        public Exception? Exception { get; protected set; }

        protected Result(bool isSuccess, string? errorMessage, Exception? exception)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Exception = exception;
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        public static Result Success() => new(true, null, null);

        /// <summary>
        /// Creates a failed result with an error message.
        /// </summary>
        public static Result Failure(string errorMessage) => new(false, errorMessage, null);

        /// <summary>
        /// Creates a failed result with an exception.
        /// </summary>
        public static Result Failure(Exception exception) => new(false, exception.Message, exception);

        /// <summary>
        /// Creates a failed result with an error message and exception.
        /// </summary>
        public static Result Failure(string errorMessage, Exception exception) => new(false, errorMessage, exception);
    }

    /// <summary>
    /// Represents the result of an operation that can either succeed with a value or fail.
    /// </summary>
    /// <typeparam name="T">The type of the value returned on success</typeparam>
    public class Result<T> : Result
    {
        public T? Value { get; private set; }

        private Result(bool isSuccess, T? value, string? errorMessage, Exception? exception) 
            : base(isSuccess, errorMessage, exception)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a successful result with a value.
        /// </summary>
        public static Result<T> Success(T value) => new(true, value, null, null);

        /// <summary>
        /// Creates a failed result with an error message.
        /// </summary>
        public static new Result<T> Failure(string errorMessage) => new(false, default, errorMessage, null);

        /// <summary>
        /// Creates a failed result with an exception.
        /// </summary>
        public static new Result<T> Failure(Exception exception) => new(false, default, exception.Message, exception);

        /// <summary>
        /// Creates a failed result with an error message and exception.
        /// </summary>
        public static new Result<T> Failure(string errorMessage, Exception exception) => new(false, default, errorMessage, exception);

        /// <summary>
        /// Implicitly converts a value to a successful Result<T>.
        /// </summary>
        public static implicit operator Result<T>(T value) => Success(value);
    }

    /// <summary>
    /// Represents a quick button item in the MTM system.
    /// </summary>
    public class QuickButtonItem
    {
        public int Position { get; set; }
        public string PartId { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty; // String number like "90", "100", "110"
        public int Quantity { get; set; }
        public string DisplayText { get; set; } = string.Empty;
        public DateTime LastUsed { get; set; }
        public int UsageCount { get; set; }
        public bool IsActive { get; set; } = true;
    }
}