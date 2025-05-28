using CSnakes.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace Demo2
{
    internal class Program
    {
        static void Main(string[] args)
        {
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
                        .WithUvInstaller();
                }
            });

            var app = builder.Build();
            var environment = app.Services.GetRequiredService<IPythonEnvironment>();
            var module = environment.Demo2();

            var modelList = module.GetModels().ToArray();

            var LLMmodel = "llama3:latest";
            var prompt = "Escribe una poesía sobre .NET";

            // Simple prompt
            string result = module.Prompt(LLMmodel, "¿Me puedes escribir una poesia sobre .NET?");

            // Compare differents models
            var models = new[] { "llama3.2:3b", "llama3:latest" };
            var comparative = module.CompareModels(prompt, models);
            var json = comparative.ToString();
            var dictionary  = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            foreach (var kvp in dictionary)
            {
                Console.WriteLine($"--- {kvp.Key} ---");
                Console.WriteLine(kvp.Value);
            }
        }
    }
}
