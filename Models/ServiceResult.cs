using System;

namespace MTM_WIP_Application_Avalonia.Models;

/// <summary>
/// Represents the result of a service operation.
/// </summary>
public class ServiceResult
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public Exception? Exception { get; }

    protected ServiceResult(bool isSuccess, string? errorMessage = null, Exception? exception = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Exception = exception;
    }

    public static ServiceResult Success() => new(true);
    public static ServiceResult Success(string message) => new(true, message);
    public static ServiceResult Failure(string errorMessage) => new(false, errorMessage);
    public static ServiceResult Failure(string errorMessage, Exception exception) => new(false, errorMessage, exception);
    public static ServiceResult Failure(Exception exception) => new(false, exception.Message, exception);
}

/// <summary>
/// Represents the result of a service operation with a return value.
/// </summary>
/// <typeparam name="T">The type of the return value.</typeparam>
public class ServiceResult<T> : ServiceResult
{
    public T? Value { get; }

    private ServiceResult(bool isSuccess, T? value = default, string? errorMessage = null, Exception? exception = null)
        : base(isSuccess, errorMessage, exception)
    {
        Value = value;
    }

    public static ServiceResult<T> Success(T value) => new(true, value);
    public static ServiceResult<T> Success(T value, string message) => new(true, value, message);
    public static new ServiceResult<T> Failure(string errorMessage) => new(false, default, errorMessage);
    public static new ServiceResult<T> Failure(string errorMessage, Exception exception) => new(false, default, errorMessage, exception);
    public static new ServiceResult<T> Failure(Exception exception) => new(false, default, exception.Message, exception);
}
