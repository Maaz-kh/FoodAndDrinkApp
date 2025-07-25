// HomePage.xaml.cs
using FoodAndDrinkApp.Models;
using FoodAndDrinkApp.Services;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

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

        private void EditRecipeModal_EditiCanceled(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private async Task LoadRecipesAsync()
        {
            recipeList = await DatabaseService.GetRecipesAsync();
            PopulateRecipeGrid(recipeList);
        }

        private void PopulateRecipeGrid(List<Recipe> recipes)
        {
            RecipesGrid.Children.Clear();
            RecipesGrid.ColumnDefinitions.Clear();
            RecipesGrid.RowDefinitions.Clear();

            int columnCount = DeviceDisplay.MainDisplayInfo.Width > 720 ? 3 : 1;

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

        private View CreateRecipeCard(Recipe recipe)
        {
            var image = new Image
            {
                Source = recipe.ImagePath,
                Aspect = Aspect.AspectFill,
                HeightRequest = 230,
                WidthRequest = 300
            };

            var titleLabel = new Label
            {
                Text = recipe.Title,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.Black,
                LineBreakMode = LineBreakMode.TailTruncation
            };

            var timeLabel = new Label
            {
                Text = $"Estimated Time: {recipe.EstimatedTime}",
                FontSize = 16,
                TextColor = Colors.Gray,
                Margin = new Thickness(0, 0, 0, 10) 

            };

            var textLayout = new VerticalStackLayout
            {
                Padding = new Thickness(15, 15, 15, 15),
                Spacing = 5,
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
                WidthRequest = DeviceInfo.Platform == DevicePlatform.Android ? 250 : 300,
                HeightRequest = DeviceInfo.Platform == DevicePlatform.Android ? 250 : 310,
                Content = outerLayout,
                
                GestureRecognizers =
                {
                    new TapGestureRecognizer
                    {
                        Command = new Command(async () =>
                        {
                            DetailsModal.LoadRecipe(recipe);
                            ShowDimBackground();
                            await DetailsModal.ShowAsync();
                        })
                    }
                }
                    };

                    return frame;
                }


        // Show the Add Recipe modal when the button is clicked
        private async void OnAddRecipeClicked(object sender, EventArgs e)
        {
            ShowDimBackground();
            await AddModal.ShowAsync();
        }

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
