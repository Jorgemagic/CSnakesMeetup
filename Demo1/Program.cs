using CSnakes.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demo1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, Python friend!");

            var builder = Host.CreateDefaultBuilder(args)
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
                        .WithPipInstaller();
                }
            });

            var app = builder.Build();
            var environment = app.Services.GetRequiredService<IPythonEnvironment>();

            var module = environment.Demo1();

            // Call Function 1
            var result = module.HelloWorld(".NET");
            Console.WriteLine(result);

            // Call Function 2
            var person = module.CreatePerson("John", 41);
            string name = person.GetAttr("name").ToString();
            int.TryParse(person.GetAttr("age").ToString(), out int age);
            Console.WriteLine($"Person Name:{name} Age:{age}");

        }
    }
}
