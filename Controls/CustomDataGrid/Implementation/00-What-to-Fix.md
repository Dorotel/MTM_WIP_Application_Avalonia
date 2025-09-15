Fix CustomDataGrid

1) Columns must always be aligned, currently rows are now alligned with their column headers.
2) Remove the "Last Updated" column as this will be part of the "Details" sections
3) Fix the stored procedure for Saving the Notes.
    3a) Logs
    System.ArgumentException: Parameter 'p_ID' not found in the collection.
   at MySql.Data.MySqlClient.MySqlParameterCollection.GetParameterFlexible(String parameterName, Boolean throwOnNotFound)
   at MySql.Data.MySqlClient.StoredProcedure.GetAndFixParameter(String spName, MySqlSchemaRow param, Boolean realAsFloat, MySqlParameter returnParameter)
   at MySql.Data.MySqlClient.StoredProcedure.CheckParametersAsync(String spName, Boolean execAsync)
   at MySql.Data.MySqlClient.StoredProcedure.Resolve(Boolean preparing)
   at MySql.Data.MySqlClient.MySqlCommand.ExecuteReaderAsync(CommandBehavior behavior, Boolean execAsync, CancellationToken cancellationToken)
   at MySql.Data.MySqlClient.MySqlCommand.ExecuteNonQueryAsync(Boolean execAsync, CancellationToken cancellationToken)
   at MTM_WIP_Application_Avalonia.Services.Helper_Database_StoredProcedure.ExecuteWithStatus(String connectionString, String procedureName, Dictionary`2 parameters) in c:\Users\johnk\source\repos\MTM_WIP_Application_Avalonia\Services\Database.cs:line 1571
2025-09-14 17:50:09.892 warn: MTM_WIP_Application_Avalonia.ViewModels.Overlay.NoteEditorViewModel[0]
      Failed to update note for inventory item 1032: Error executing stored procedure: Parameter 'p_ID' not found in the collection.
MTM_WIP_Application_Avalonia.ViewModels.Overlay.NoteEditorViewModel: Warning: Failed to update note for inventory item 1032: Error executing stored procedure: Parameter 'p_ID' not found in the collection.

4) Remove the "Duplicate" button, it wont be used.
