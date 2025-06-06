using Microsoft.Maui.Controls;

namespace KitapTakipMaui
{
    public partial class App : Application
    {
        // Statik IServiceProvider özelliği
        public static IServiceProvider Services { get; private set; }

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            Services = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider), "Service provider cannot be null.");
            MainPage = new AppShell();
        }
    }
}