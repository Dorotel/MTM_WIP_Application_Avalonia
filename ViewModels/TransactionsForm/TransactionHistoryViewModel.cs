using System;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using Avalonia.ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class TransactionHistoryViewModel : BaseViewModel
{
    private readonly MTM_Shared_Logic.Services.ITransactionService _transactionService;
    private readonly IApplicationStateService _applicationState;

    public TransactionHistoryViewModel(
        MTM_Shared_Logic.Services.ITransactionService transactionService,
        IApplicationStateService applicationState,
        ILogger<TransactionHistoryViewModel> logger) : base(logger)
    {
        _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));

        Logger.LogInformation("TransactionHistoryViewModel initialized with dependency injection");

        // TODO: Initialize commands and properties
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        // TODO: Initialize ReactiveCommands with injected services
    }
}
