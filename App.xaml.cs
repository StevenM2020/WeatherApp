namespace WeatherApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            StoreApiKey();

            MainPage = new AppShell();

        }

        private async void StoreApiKey()
        {
            await SecureStorage.SetAsync("WeatherApiKey", "");
            await SecureStorage.SetAsync("OpenAIApiKey", "");
        }

        // this method is used to set the size of the window
        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);
            window.Height = 750;
            window.Width = 600;
            return window;
        }
    }
}
