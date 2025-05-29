namespace Demo3
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            SegmentBtn.Text = "Segmented image";
            SemanticScreenReader.Announce(SegmentBtn.Text);
        }
    }

}
