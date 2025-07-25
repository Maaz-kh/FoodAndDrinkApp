using FoodAndDrinkApp.Services;

namespace FoodAndDrinkApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            Task.Run(async () => await DatabaseService.InitAsync());
        }
    }
}
