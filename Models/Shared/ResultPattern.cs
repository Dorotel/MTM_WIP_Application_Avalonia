using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTM_WIP_Application_Avalonia.Models
{
    /// <summary>
    /// Generic result pattern for operations that return data.
    /// Provides structured success/failure handling with comprehensive error information.
    /// </summary>
    /// <typeparam name="T">The type of data returned on success</typeparam>
    public class Result<T>
    {
        private Result(bool isSuccess, T? data, string errorMessage, string? errorCode, Exception? exception)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            Exception = exception;
        }

        /// <summary>
        /// Indicates if the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Indicates if the operation failed.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// The data returned by the operation (only available on success).
        /// </summary>
        public T? Data { get; }

        /// <summary>
        /// User-friendly error message (available on failure).
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Specific error code for programmatic handling (optional).
        /// </summary>
        public string? ErrorCode { get; }

        /// <summary>
        /// The original exception that caused the failure (optional).
        /// </summary>
        public Exception? Exception { get; }

        /// <summary>
        /// Creates a successful result with data.
        /// </summary>
        public static Result<T> Success(T data) => new(true, data, string.Empty, null, null);

        /// <summary>
        /// Creates a failed result with an error message.
        /// </summary>
        public static Result<T> Failure(string errorMessage) => new(false, default, errorMessage, null, null);

        /// <summary>
        /// Creates a failed result with an error message and error code.
        /// </summary>
        public static Result<T> Failure(string errorMessage, string errorCode) => new(false, default, errorMessage, errorCode, null);

        /// <summary>
        /// Creates a failed result with an error message, error code, and exception.
        /// </summary>
        public static Result<T> Failure(string errorMessage, string? errorCode, Exception? exception) => new(false, default, errorMessage, errorCode, exception);

        /// <summary>
        /// Creates a warning result (successful but with warnings).
        /// </summary>
        public static Result<T> Warning(T data, string warningMessage) => new(true, data, warningMessage, "WARNING", null);

        /// <summary>
        /// Implicit conversion from T to Result<T> for convenience.
        /// </summary>
        public static implicit operator Result<T>(T data) => Success(data);

        /// <summary>
        /// Maps the data to a new type if the result is successful.
        /// </summary>
        public Result<TOut> Map<TOut>(Func<T, TOut> mapper)
        {
            return IsSuccess 
                ? Result<TOut>.Success(mapper(Data!))
                : Result<TOut>.Failure(ErrorMessage, ErrorCode, Exception);
        }

        /// <summary>
        /// Binds the result to another operation if successful.
        /// </summary>
        public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> binder)
        {
            return IsSuccess 
                ? binder(Data!)
                : Result<TOut>.Failure(ErrorMessage, ErrorCode, Exception);
        }

        /// <summary>
        /// Executes an action if the result is successful.
        /// </summary>
        public Result<T> OnSuccess(Action<T> action)
        {
            if (IsSuccess) action(Data!);
            return this;
        }

        /// <summary>
        /// Executes an action if the result is a failure.
        /// </summary>
        public Result<T> OnFailure(Action<string> action)
        {
            if (IsFailure) action(ErrorMessage);
            return this;
        }

        /// <summary>
        /// Gets the data or returns a default value if the result is a failure.
        /// </summary>
        public T GetValueOrDefault(T defaultValue = default!) => IsSuccess ? Data! : defaultValue;

        /// <summary>
        /// Throws an exception if the result is a failure.
        /// </summary>
        public T GetValueOrThrow()
        {
            if (IsFailure)
                throw Exception ?? new InvalidOperationException(ErrorMessage);
            return Data!;
        }
    }

    /// <summary>
    /// Non-generic result pattern for operations that don't return data.
    /// Provides structured success/failure handling for confirmation operations.
    /// </summary>
    public class Result
    {
        private Result(bool isSuccess, string errorMessage, string? errorCode, Exception? exception)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            Exception = exception;
        }

        /// <summary>
        /// Indicates if the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Indicates if the operation failed.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// User-friendly error message (available on failure).
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Specific error code for programmatic handling (optional).
        /// </summary>
        public string? ErrorCode { get; }

        /// <summary>
        /// The original exception that caused the failure (optional).
        /// </summary>
        public Exception? Exception { get; }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        public static Result Success() => new(true, string.Empty, null, null);

        /// <summary>
        /// Creates a failed result with an error message.
        /// </summary>
        public static Result Failure(string errorMessage) => new(false, errorMessage, null, null);

        /// <summary>
        /// Creates a failed result with an error message and error code.
        /// </summary>
        public static Result Failure(string errorMessage, string errorCode) => new(false, errorMessage, errorCode, null);

        /// <summary>
        /// Creates a failed result with an error message, error code, and exception.
        /// </summary>
        public static Result Failure(string errorMessage, string? errorCode, Exception? exception) => new(false, errorMessage, errorCode, exception);

        /// <summary>
        /// Creates a warning result (successful but with warnings).
        /// </summary>
        public static Result Warning(string warningMessage) => new(true, warningMessage, "WARNING", null);

        /// <summary>
        /// Binds the result to another operation if successful.
        /// </summary>
        public Result Bind(Func<Result> binder)
        {
            return IsSuccess ? binder() : this;
        }

        /// <summary>
        /// Executes an action if the result is successful.
        /// </summary>
        public Result OnSuccess(Action action)
        {
            if (IsSuccess) action();
            return this;
        }

        /// <summary>
        /// Executes an action if the result is a failure.
        /// </summary>
        public Result OnFailure(Action<string> action)
        {
            if (IsFailure) action(ErrorMessage);
            return this;
        }

        /// <summary>
        /// Throws an exception if the result is a failure.
        /// </summary>
        public void ThrowIfFailure()
        {
            if (IsFailure)
                throw Exception ?? new InvalidOperationException(ErrorMessage);
        }
    }

    /// <summary>
    /// Extension methods for Result pattern operations.
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Maps the result data to a new type asynchronously.
        /// </summary>
        public static async Task<Result<TOut>> MapAsync<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, TOut> mapper)
        {
            var result = await resultTask;
            return result.IsSuccess 
                ? Result<TOut>.Success(mapper(result.Data!))
                : Result<TOut>.Failure(result.ErrorMessage, result.ErrorCode, result.Exception);
        }

        /// <summary>
        /// Binds the result to another async operation if successful.
        /// </summary>
        public static async Task<Result<TOut>> BindAsync<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Task<Result<TOut>>> binder)
        {
            var result = await resultTask;
            return result.IsSuccess 
                ? await binder(result.Data!)
                : Result<TOut>.Failure(result.ErrorMessage, result.ErrorCode, result.Exception);
        }

        /// <summary>
        /// Executes an async action if the result is successful.
        /// </summary>
        public static async Task<Result<T>> OnSuccessAsync<T>(this Task<Result<T>> resultTask, Func<T, Task> action)
        {
            var result = await resultTask;
            if (result.IsSuccess) await action(result.Data!);
            return result;
        }

        /// <summary>
        /// Executes an async action if the result is a failure.
        /// </summary>
        public static async Task<Result<T>> OnFailureAsync<T>(this Task<Result<T>> resultTask, Func<string, Task> action)
        {
            var result = await resultTask;
            if (result.IsFailure) await action(result.ErrorMessage);
            return result;
        }

        /// <summary>
        /// Converts a Result<T> to a non-generic Result.
        /// </summary>
        public static Result ToResult<T>(this Result<T> result)
        {
            return result.IsSuccess 
                ? Result.Success()
                : Result.Failure(result.ErrorMessage, result.ErrorCode, result.Exception);
        }

        /// <summary>
        /// Combines multiple results into a single result.
        /// </summary>
        public static Result Combine(params Result[] results)
        {
            var errors = new List<string>();
            
            foreach (var result in results)
            {
                if (result.IsFailure)
                    errors.Add(result.ErrorMessage);
            }

            return errors.Count == 0 
                ? Result.Success()
                : Result.Failure(string.Join("; ", errors));
        }

        /// <summary>
        /// Combines multiple generic results into a single result with collected data.
        /// </summary>
        public static Result<IEnumerable<T>> Combine<T>(params Result<T>[] results)
        {
            var errors = new List<string>();
            var data = new List<T>();
            
            foreach (var result in results)
            {
                if (result.IsSuccess)
                    data.Add(result.Data!);
                else
                    errors.Add(result.ErrorMessage);
            }

            return errors.Count == 0 
                ? Result<IEnumerable<T>>.Success(data)
                : Result<IEnumerable<T>>.Failure(string.Join("; ", errors));
        }
    }

    /// <summary>
    /// MTM-specific result extensions for business operations.
    /// </summary>
    public static class MTMResultExtensions
    {
        /// <summary>
        /// Creates a result from MTM stored procedure output with status handling.
        /// </summary>
        public static Result<T> FromStoredProcedureResult<T>(
            object? status, 
            string? message, 
            T? data,
            Func<T>? defaultValueFactory = null) where T : class
        {
            var statusCode = Convert.ToInt32(status ?? -1);
            
            return statusCode switch
            {
                0 => data != null ? Result<T>.Success(data) : 
                     defaultValueFactory != null ? Result<T>.Success(defaultValueFactory()) :
                     Result<T>.Failure("No data returned from successful operation"),
                1 => Result<T>.Warning(data ?? defaultValueFactory?.Invoke() ?? throw new InvalidOperationException("Cannot create warning result without data"), message ?? "Operation completed with warnings"),
                _ => Result<T>.Failure(message ?? "Operation failed", statusCode.ToString())
            };
        }

        /// <summary>
        /// Creates a result from MTM stored procedure output for value types.
        /// </summary>
        public static Result<T> FromStoredProcedureResultValue<T>(
            object? status, 
            string? message, 
            T data) where T : struct
        {
            var statusCode = Convert.ToInt32(status ?? -1);
            
            return statusCode switch
            {
                0 => Result<T>.Success(data),
                1 => Result<T>.Warning(data, message ?? "Operation completed with warnings"),
                _ => Result<T>.Failure(message ?? "Operation failed", statusCode.ToString())
            };
        }

        /// <summary>
        /// Creates a non-generic result from MTM stored procedure output.
        /// </summary>
        public static Result FromStoredProcedureResult(object? status, string? message)
        {
            var statusCode = Convert.ToInt32(status ?? -1);
            
            return statusCode switch
            {
                0 => Result.Success(),
                1 => Result.Warning(message ?? "Operation completed with warnings"),
                _ => Result.Failure(message ?? "Operation failed", statusCode.ToString())
            };
        }

        /// <summary>
        /// Logs the result using MTM error handling patterns.
        /// </summary>
        public static async Task<Result<T>> LogResultAsync<T>(
            this Result<T> result,
            string operation,
            string userId,
            Dictionary<string, object>? context = null)
        {
            if (result.IsFailure && result.Exception != null)
            {
                await Services.Core.ErrorHandling.HandleErrorAsync(
                    result.Exception, 
                    operation, 
                    userId, 
                    context ?? new Dictionary<string, object>());
            }

            return result;
        }

        /// <summary>
        /// Logs business operation performance and results.
        /// </summary>
        public static async Task<Result<T>> LogBusinessOperationAsync<T>(
            this Task<Result<T>> resultTask,
            string operation,
            string userId,
            Dictionary<string, object>? parameters = null)
        {
            var startTime = DateTime.UtcNow;
            
            try
            {
                var result = await resultTask;
                var duration = DateTime.UtcNow - startTime;
                
                // TODO: Implement proper logging utility service
                // await Services.LoggingUtility.LogBusinessOperationAsync(
                //     operation,
                //     userId,
                //     parameters ?? new Dictionary<string, object>(),
                //     result.Exception,
                //     result.IsSuccess ? "Success" : "Failure");

                // await Services.LoggingUtility.LogPerformanceMetricsAsync(
                //     operation,
                //     duration,
                //     userId,
                //     new Dictionary<string, object>
                //     {
                //         ["Success"] = result.IsSuccess,
                //         ["HasWarnings"] = !string.IsNullOrEmpty(result.ErrorMessage) && result.IsSuccess
                //     });

                return result;
            }
            catch (Exception ex)
            {
                var duration = DateTime.UtcNow - startTime;
                
                // TODO: Implement proper logging utility service
                // await Services.LoggingUtility.LogBusinessOperationAsync(
                //     operation,
                //     userId,
                //     parameters ?? new Dictionary<string, object>(),
                //     ex,
                //     "Exception");

                return Result<T>.Failure("Operation failed due to unexpected error", "EXCEPTION", ex);
            }
        }
    }
}