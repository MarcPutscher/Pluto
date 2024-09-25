using Pluto.Pages;

namespace Pluto
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
        protected override Window CreateWindow(IActivationState activationState)
        {
            var windows = base.CreateWindow(activationState);

            double height = 900;
            double width = 600;

            windows.Height = height;
            windows.Width = width;

            windows.MaximumHeight = height;
            windows.MaximumWidth = width;

            windows.MinimumHeight = height;
            windows.MinimumWidth = width;

            // Get display size
            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;

            // Center the window
            windows.X = (displayInfo.Width / displayInfo.Density - windows.Width) / 2;
            windows.Y = (displayInfo.Height / displayInfo.Density - windows.Height) / 2;

            return windows;
        }
    }
}
