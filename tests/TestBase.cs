namespace MTM_WIP_Application_Avalonia.Tests;

public class TestBase
{
    protected static bool IsInitialized { get; private set; }

    static TestBase()
    {
        try
        {
            // Basic initialization for testing
            IsInitialized = true;
        }
        catch
        {
            IsInitialized = false;
        }
    }
}
