﻿using CommunityToolkit.HighPerformance;
using CSnakes.Runtime;
using CSnakes.Runtime.Python;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Numerics.Tensors;
using System.Runtime.InteropServices;

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

            // Hello world
            var result = module.HelloWorld(".NET");
            Console.WriteLine(result);

            // Classes
            var person = module.CreatePerson("John", 41);
            string name = person.GetAttr("name").ToString();
            int.TryParse(person.GetAttr("age").ToString(), out int age);
            Console.WriteLine($"Person Name:{name} Age:{age}");

            // Async function
            Task.Run(async () =>
            {
                var number = await module.AsyncFunction();
                Console.WriteLine($"Async function return {number}");
            });

            // Crossing the bridge too frequently
            var names = module.MakeSquare2dArray(1000);

            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    module.SetRandom(names, i, j);
                }
            }
            Console.WriteLine($"Crossing the bridge too frequently");

            // Marshalling return values unnecessarily
            module.LogValue("Add this message to the logger");

            // Passing a large amount of data
            double[] data = new double[1000];
            var random = new Random();
            for (int i = 0;i < 1000;i++)
            {
                data[i] = random.NextDouble();
            }
            IReadOnlyList<double> dataAsList = data.ToList();
            module.GetData1(dataAsList);

            byte[] dataAsBytes = MemoryMarshal.AsBytes<double>(data.AsSpan()).ToArray();
            module.GetData2(dataAsBytes);

            // Buffer Protocol
            var array1d = module.ExampleArray();
            if (array1d.IsScalar)
            {
                ReadOnlySpan<bool> booleans = array1d.AsBoolReadOnlySpan();
            }

            var array2D = module.ExampleArray2d();
            ReadOnlySpan2D<int> matrix = array2D.AsInt32ReadOnlySpan2D();

            var arrayND = module.ExampleTensor();
            var tensor = arrayND.AsReadOnlyTensorSpan<int>();
            Console.WriteLine(tensor[0, 0, 0, 0]);
            Console.WriteLine(tensor[1, 2, 3, 4]);

            // Calling Python without the Source Generator
            var rawModule = Import.ImportModule("demo1");
            using (GIL.Acquire())
            {
                Console.WriteLine("Invoking Python function: test_int_float");
                long a = 10;
                double b = 2.5f;
                
                PyObject pythonFunc = rawModule.GetAttr("test_int_float");
                using PyObject a_pyObject = PyObject.From(a)!;
                using PyObject b_pyObject = PyObject.From(b)!;
                using PyObject resultObject = pythonFunc.Call(a_pyObject, b_pyObject);
                double r = resultObject.As<double>();
                Console.WriteLine($"Result: {r}");
            }
            rawModule.Dispose();

            Console.ReadLine();
        }        
    }
}
