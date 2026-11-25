using Klyroo.Stack;

namespace KlyrooMauiApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddKlyrooStack();
        return builder.Build();
    }
}
