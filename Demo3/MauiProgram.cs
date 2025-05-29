using CSnakes.Runtime;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demo3
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Configuración de CSnakes + entorno Python
            var home = Path.Combine(Environment.CurrentDirectory, "Python");
            var venv = Path.Combine(home, ".venv-Python");

            builder.Services.AddSingleton<IPythonEnvironment>(sp =>
            {
                var hostBuilder = Host.CreateDefaultBuilder()
                    .ConfigureServices(services =>
                    {
                        if (Directory.Exists(venv))
                        {
                            services
                                .WithPython()
                                .WithHome(home)
                                .WithVirtualEnvironment(venv)
                                .FromNuGet("3.12.6");
                        }
                        else
                        {
                            services
                                .WithPython()
                                .WithHome(home)
                                .WithVirtualEnvironment(venv)
                                .FromNuGet("3.12.6")
                                .WithUvInstaller();
                        }
                    });

                var host = hostBuilder.Build();
                return host.Services.GetRequiredService<IPythonEnvironment>();
            });


            return builder.Build();
        }
    }
}
