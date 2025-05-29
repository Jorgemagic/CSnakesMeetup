namespace Demo3
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

            const int newWidth = 1280;
            const int newHeight = 720;

            window.Width = newWidth;
            window.Height = newHeight;

            return window;
        }
    }
}