using CSnakes.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SkiaSharp;

namespace Demo4
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

            var module = environment.Demo4();

            ////string imagePath = "sa_10937.jpg";
            string imagePath = "sa_10922.jpg";
            var rawLabelMap = module.SegmentImage(imagePath);

            var labelMap = new List<List<int>>();

            // 3) Itera cada "fila" del PyList
            foreach (dynamic pyRow in rawLabelMap)
            {
                var row = new List<int>();
                // 4) Itera cada valor de la fila
                foreach (dynamic pyVal in pyRow)
                {
                    // Convierte pyVal a int
                    row.Add((int)pyVal);
                }
                labelMap.Add(row);
            }

            // Read mask and create overlay segmentation with skiaSharp

            // 1) Load original image
            using var original = SKBitmap.Decode(imagePath);
            int width = original.Width;
            int height = original.Height;

            // 2) Create a transparent bitmap for the overlay
            using var overlay = new SKBitmap(width, height);

            // 3) Choose random colors
            int maxLabel = labelMap.Max(row => row.Max());
            var rnd = new Random();
            var colors = Enumerable.Range(0, maxLabel + 1)
                .Select(i => i == 0
                    ? SKColors.Transparent          // etiqueta 0 = transparent
                    : new SKColor(                  // etiquetas 1..N = random
                        (byte)rnd.Next(256),
                        (byte)rnd.Next(256),
                        (byte)rnd.Next(256),
                        128))
                .ToArray();

            // 4) Fill pixel by pixel where mask[y][x] == 1:            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int label = labelMap[y][x];
                    overlay.SetPixel(x, y, colors[label]);
                }
            }

            // 5) Combine original image and the overlay:
            var info = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;

            canvas.Clear();                          // Clear background
            canvas.DrawBitmap(original, 0, 0);       // Draw original image
            canvas.DrawBitmap(overlay, 0, 0);        // Draw overlay

            // 6) Save segmented image
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);
            using var fileOut = File.OpenWrite("segmented_overlay.jpg");
            data.SaveTo(fileOut);

            Console.WriteLine("Segmented image created");
        }
    }
}
