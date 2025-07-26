// HomePage.xaml.cs
using FoodAndDrinkApp.Models;
using FoodAndDrinkApp.Services;
using FoodAndDrinkApp.Utilities;


namespace FoodAndDrinkApp.Views
{
    public partial class HomePage : ContentPage
    {
        private List<Recipe> recipeList = new();

        public HomePage()
        {
            InitializeComponent();      // Loads the XAML components
            Shell.SetNavBarIsVisible(this, false);
            InitializeEventHandlers();     // Set up event handlers for modals and buttons
            _ = LoadRecipesAsync();   // Load recipes asynchronously when the page is initialized
        }

        private void InitializeEventHandlers()
        {
            AddModal.CancelRequested += async (_, __) => {
                await AddModal.HideAsync();
                HideDimBackground();  // Hide dim on cancel
            };

            AddModal.SaveRequested += async (_, __) => {
                await LoadRecipesAsync();
                ShowDimBackground();
                await MyCustomAlert.Show("Success", "Recipe Added successfully", 0);
                HideDimBackground();  // Hide dim on Save
            };

            DetailsModal.CloseRequested += async (_, __) => {
                await DetailsModal.HideAsync();
                HideDimBackground();

            };
            DetailsModal.DeleteRequested += async (_, __) => {
                await LoadRecipesAsync();
                ShowDimBackground();
                await MyCustomAlert.Show("Success", "Recipe deleted successfully", 0);
                HideDimBackground();
            };

            DetailsModal.EditRequested += (_, __) => EditRecipeDetails();

            EditRecipeModal.RecipeEdited += async (_, __) => { 
                await LoadRecipesAsync();
                ShowDimBackground();
                await MyCustomAlert.Show("Success", "Recipe Edited successfully", 0);
                HideDimBackground();
            };

            EditRecipeModal.EditiCanceled += async(_, __) => {
                await EditRecipeModal.HideAsync();
                HideDimBackground();  // Hide dim on cancel
            };
        }

        // Load recipes from the database asynchronously
        private async Task LoadRecipesAsync()
        {
            recipeList = await DatabaseService.GetRecipesAsync();
            PopulateRecipeGrid(recipeList, this.Width);
        }

        // Populate the grid with recipe cards based on the loaded recipes
        private void PopulateRecipeGrid(List<Recipe> recipes, double width = 0)
        {
            RecipesGrid.Children.Clear();
            RecipesGrid.ColumnDefinitions.Clear();
            RecipesGrid.RowDefinitions.Clear();

            int columnCount = width > 900 ? 3 : width > 500 ? 2 : 1;


            for (int i = 0; i < columnCount; i++)
                RecipesGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

            int row = 0, column = 0;

            foreach (var recipe in recipes)
            {
                if (column >= columnCount)
                {
                    column = 0;
                    row++;
                }

                if (RecipesGrid.RowDefinitions.Count <= row)
                    RecipesGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

                var card = CreateRecipeCard(recipe);
                Grid.SetColumn(card, column);
                Grid.SetRow(card, row);

                RecipesGrid.Children.Add(card);
                column++;
            }
        }
        // Override OnSizeAllocated to adjust the grid layout when the page size changes
        protected override async void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width > 0 && recipeList.Count > 0)
            {
                PopulateRecipeGrid(recipeList, width);
            }
        }

        // Create a recipe card view for each recipe
        private View CreateRecipeCard(Recipe recipe)
        {
            var isAndroid = DeviceInfo.Platform == DevicePlatform.Android;

            var image = new Image
            {
                Source = recipe.ImagePath,
                Aspect = Aspect.AspectFill,
                WidthRequest = isAndroid ? 250 : 300,
                HeightRequest = isAndroid ? 200 : 240
            };

            var titleLabel = new Label
            {
                Text = recipe.Title,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.Black,
                LineBreakMode = LineBreakMode.TailTruncation,
                HorizontalTextAlignment = isAndroid ? TextAlignment.Center : TextAlignment.Start,
                Margin = isAndroid ? new Thickness(0, 0, 0, 0) : new Thickness(0)
            };

            var timeLabel = new Label
            {
                Text = $"Estimated Time: {recipe.EstimatedTime}",
                FontSize = 16,
                TextColor = Colors.Gray,
                Margin = isAndroid ? new Thickness(0, 0, 0, 5) : new Thickness(0, 0, 0, 10),
                HorizontalTextAlignment = isAndroid ? TextAlignment.Center : TextAlignment.Start

            };

            var textLayout = new VerticalStackLayout
            {
                Padding = new Thickness(15, 15, 15, 15),
                Spacing = isAndroid ? 3 : 5,
                Children = { titleLabel, timeLabel }
            };

            var outerLayout = new VerticalStackLayout
            {
                Spacing = 0,
                Children = { image, textLayout }
            };

            var frame = new Frame
            {
                CornerRadius = 10,
                Padding = 0,
                Margin = new Thickness(10, 5),
                BackgroundColor = Colors.White,
                HasShadow = true,
                WidthRequest = isAndroid ? 250 : 300,
                HeightRequest = isAndroid ? 280 : 320,
                Content = outerLayout,
            };

            frame.GestureRecognizers.Add(
                new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        // tap animation
                        await frame.ScaleTo(0.95, 50, Easing.CubicOut);
                        await frame.ScaleTo(1, 100, Easing.CubicIn);

                        // Show modal
                        DetailsModal.LoadRecipe(recipe);
                        ShowDimBackground();
                        await DetailsModal.ShowAsync();
                    })
                }
            );

            return frame;
        }


        // Show the Add Recipe modal when the button is clicked
        private async void OnAddRecipeClicked(object sender, EventArgs e)
        {
            await AddButton.ScaleTo(0.90, 60, Easing.CubicOut);
            await AddButton.ScaleTo(1, 100, Easing.CubicIn);
            Helper.SaveVibrationAndHapticFeedback();
            ShowDimBackground();
            await AddModal.ShowAsync();
        }

        // Edit recipe details when the edit button is clicked in the details modal
        private async void EditRecipeDetails()
        {
            var recipeToEdit = DetailsModal.GetRecipe();
            if (recipeToEdit == null)
                return;

            if (recipeToEdit != null)
            {
                EditRecipeModal.LoadRecipe(recipeToEdit);
                await DetailsModal.HideAsync();
                await EditRecipeModal.ShowAsync();
            }
        }

        public void ShowDimBackground()
        {
            DimBackground.IsVisible = true;
        }

        public void HideDimBackground()
        {
            DimBackground.IsVisible = false;
        }

    }
}
