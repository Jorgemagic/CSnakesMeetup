using CSnakes.Runtime;
using SkiaSharp;
using System.Threading.Tasks;

namespace Demo3
{
    public partial class MainPage : ContentPage
    {
        private readonly IPythonEnvironment environment;

        public MainPage(IPythonEnvironment environment)
        {
            InitializeComponent();
            this.environment = environment;
        }

        private async void OnButtonClicked(object sender, EventArgs e)
        {
            BusyIndicator.IsVisible = true;
            BusyIndicator.IsRunning = true;
            SegmentBtn.Text = "Processing ...";
            SemanticScreenReader.Announce(SegmentBtn.Text);

            try
            {
                await Task.Run(() =>
                {
                    var module = environment.Demo3();

                    string imagePath = "Python/sa_10922.jpg";
                    var rawLabelMap = module.SegmentImage(imagePath);

                    var labelMap = new List<List<int>>();

                    // Iterate each PyList row
                    foreach (dynamic pyRow in rawLabelMap)
                    {
                        var row = new List<int>();
                        foreach (dynamic pyVal in pyRow)
                        {
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

                    /*using var fileOut = File.OpenWrite("segmented_overlay.jpg");
                    data.SaveTo(fileOut);*/

                    var ms = new MemoryStream(data.ToArray());

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        DisplayedImage.Source = ImageSource.FromStream(() =>
                        {
                            ms.Seek(0, SeekOrigin.Begin);
                            return ms;
                        });
                    });
                });
            }
            finally
            {

                BusyIndicator.IsRunning = false;
                BusyIndicator.IsVisible = false;
                SegmentBtn.Text = "Segmented image";
                SemanticScreenReader.Announce(SegmentBtn.Text);
            }
        }
    }

}
