using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
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
            previewPlaceholderLabel.IsVisible = true;
            this.IsVisible = true;
            await this.FadeTo(1, 200);
            await AddRecipeForm.TranslateTo(0, (this.Height / 2) - 250, 300, Easing.CubicOut);
        }

        public async Task HideAsync()
        {
            previewPlaceholderLabel.IsVisible = false;
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
                    ? "default_image.jpg"
                    : imagePathEntry.Text
            };
        }
        public void Reset()
        {
            titleEntry.Text = estimatedTimeEntry.Text = "";
            ingredientsEditor.Text = instructionsEditor.Text = "";
            imagePathEntry.Text = "";
            recipeImage.Source = "";
            previewPlaceholderLabel.IsVisible = true;
        }


        private void OnCancelAddRecipeClicked(object sender, EventArgs e)
        {
            Reset();
            CancelRequested?.Invoke(this, EventArgs.Empty);
        }

        private async void OnSaveRecipeClicked(object sender, EventArgs e)
        {
            try
            {
                var newRecipe = GetRecipe();

                if (string.IsNullOrWhiteSpace(newRecipe.Title))
                {
                    ShowDimBackground();
                    await MyCustomAlert.Show("Error", "Title is required.", 0);
                    HideDimBackground();
                    return;
                }

                await DatabaseService.AddRecipeAsync(newRecipe);
                SaveRequested?.Invoke(this, EventArgs.Empty); // Parent refreshes

                await HideAsync();
                Reset();
            }
            catch (Exception ex)
            {
                await MyCustomAlert.Show("Error", ex.Message, 0);
            }
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
                    recipeImage.Opacity = 1;
                    previewPlaceholderLabel.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                ShowDimBackground();
                await MyCustomAlert.Show("Error", $"Image selection failed: {ex.Message}", 0);
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