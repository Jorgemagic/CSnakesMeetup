using CSnakes.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
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

            Dictionary<string, string> dictionary = default;
            AnsiConsole.Clear();
            AnsiConsole.Status()
                .Start("[greenyellow]Compare Models...[/]", ctx =>
                {

                    var LLMmodel = "llama3:latest";
                    var prompt = "Escribe una poesía sobre .NET";

                    ctx.Spinner(Spinner.Known.Star);
                    ctx.SpinnerStyle(Style.Parse("blue"));

                    // Simple prompt
                    string result = module.Prompt(LLMmodel, "¿Me puedes escribir una poesia sobre .NET?");

                    // Compare differents models
                    var models = new[] { "llama3.2:3b", "llama3:latest" };
                    var comparative = module.CompareModels(prompt, models);
                    var json = comparative.ToString();
                    dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                });

            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.BorderColor(Color.DodgerBlue1);
            table.ShowRowSeparators();
            table.AddColumn("[dodgerblue1]Model[/]");
            table.AddColumn("[dodgerblue1]Result[/]");
            int index = 0;
            foreach (var kvp in dictionary)
            {
                table.Columns[index++].Centered();
                table.AddRow($"[orangered1]{kvp.Key}[/]", $"[greenyellow]{kvp.Value}[/]");
            }
            AnsiConsole.Write(table);
        }
    }
}
