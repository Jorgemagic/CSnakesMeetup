using CSnakes.Runtime;
using CSnakes.Runtime.Python;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demo4Library
{
    public class Helpers
    {
        public static readonly Helpers Instance = new Helpers();
        private PyObject module;

        private Helpers() 
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    var home = Path.Join(Environment.CurrentDirectory, "Python");
                    var venv = Path.Join(home, ".venv-Python");
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

            var app = builder.Build();
            var environment = app.Services.GetRequiredService<IPythonEnvironment>();

            this.module = Import.ImportModule("demo4");
        }

        public float MyFunction(float x, float y)
        {
            var func = this.module.GetAttr("my_python_function");
            var result = func.Call(PyObject.From(x), PyObject.From(y));
            return (float)result.As<double>();
        }
    }
}
