using FoodAndDrinkApp.Models;
using FoodAndDrinkApp.Services;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace FoodAndDrinkApp.Views
{
   
    public partial class AddRecipeModal : ContentView
    {
        public event EventHandler CancelRequested;
        public event EventHandler SaveRequested;

        public AddRecipeModal()
        {
            InitializeComponent();
            this.IsVisible = false;
            this.Opacity = 0;
        }
        
        public async Task ShowAsync()
        {
            this.IsVisible = true;
            await this.FadeTo(1, 200);
            await AddRecipeForm.TranslateTo(0, (this.Height / 2) - 250, 300, Easing.CubicOut);
        }

        public async Task HideAsync()
        {
            await AddRecipeForm.TranslateTo(0, -500, 300, Easing.CubicIn);
            await this.FadeTo(0, 200);
            this.IsVisible = false;
        }

        public Recipe GetRecipe()
        {
            return new Recipe
            {
                Title = titleEntry.Text,
                EstimatedTime = estimatedTimeEntry.Text,
                Ingredients = ingredientsEditor.Text,
                Instructions = instructionsEditor.Text,
                ImagePath = string.IsNullOrWhiteSpace(imagePathEntry.Text)
                    ? "https://via.placeholder.com/300x150.png?text=No+Image"
                    : imagePathEntry.Text
            };
        }
        public void Reset()
        {
            titleEntry.Text = estimatedTimeEntry.Text = "";
            ingredientsEditor.Text = instructionsEditor.Text = "";
            imagePathEntry.Text = "";
        }


        private void OnCancelAddRecipeClicked(object sender, EventArgs e)
        {
            CancelRequested?.Invoke(this, EventArgs.Empty);
        }

        private async void OnSaveRecipeClicked(object sender, EventArgs e)
        {
            var newRecipe = GetRecipe();

            if (string.IsNullOrWhiteSpace(newRecipe.Title))
            {
                await Application.Current.MainPage.DisplayAlert("Validation", "Title is required.", "OK");
                return;
            }

            await DatabaseService.AddRecipeAsync(newRecipe);
            SaveRequested?.Invoke(this, EventArgs.Empty); // Parent will refresh list
            await HideAsync();
            Reset();
        }

        private async void OnImagePickClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a Recipe Image",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    imagePathEntry.Text = result.FullPath;
                    recipeImage.Source = ImageSource.FromFile(result.FullPath);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Image selection failed: {ex.Message}", "OK");
            }
        }

    }
}