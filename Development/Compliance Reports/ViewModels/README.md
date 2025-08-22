# ViewModels Compliance Reports

This directory contains compliance reports for ReactiveUI ViewModel files.

## ViewModel Standards

ViewModel files must comply with:
- **ReactiveUI Patterns**: RaiseAndSetIfChanged, ReactiveCommand, WhenAnyValue
- **Property Implementation**: Observable properties with backing fields
- **Command Implementation**: Proper error handling with ThrownExceptions
- **Data Binding**: ObservableCollection for collections
- **Dependency Injection**: Constructor preparation for services
- **MVVM Separation**: No business logic, only UI binding logic

## Current ViewModel Reports

*Reports will be generated here as ViewModels are audited*

## Expected ViewModels

Based on MTM WIP Application UI structure:
- `MainViewModel.cs` - Main window coordination
- `InventoryTabViewModel.cs` - Inventory management tab
- `RemoveTabViewModel.cs` - Item removal tab  
- `QuickButtonsViewModel.cs` - Quick action buttons
- `TransactionHistoryViewModel.cs` - Transaction display

---

*Generated compliance reports will appear in this directory following the naming convention: {ViewModelName}-compliance-report-{YYYY-MM-DD}.md*