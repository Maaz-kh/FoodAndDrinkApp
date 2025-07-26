namespace FoodAndDrinkApp.Views;
using System;
using Microsoft.Maui.Controls;
using FoodAndDrinkApp.Models;
using FoodAndDrinkApp.Services;

public partial class EditRecipeModal : ContentView
{
    private Recipe _editingRecipe;

    public event EventHandler RecipeEdited;
    public event EventHandler EditiCanceled;

    public EditRecipeModal()
	{
		InitializeComponent();
        this.IsVisible = false;
        this.Opacity = 0;
    }
    // Function to show the modal with a fade-in effect and slide down animation
    public async Task ShowAsync()
    {
        EditPreviewPlaceholderLabel.IsVisible = true;
        this.IsVisible = true;
        await this.FadeTo(1, 200);
        await EditformWrapper.TranslateTo(0, (this.Height / 2) - 250, 600, Easing.CubicOut);
    }

    // Function to hide the modal with a fade-out effect and slide up animation
    public async Task HideAsync()
    {
        await EditformWrapper.TranslateTo(0, -500, 300, Easing.CubicIn);
        await this.FadeTo(0, 200);
        this.IsVisible = false;
    }

    // Function to load a recipe into the edit form
    public void LoadRecipe(Recipe recipe)
    {
        _editingRecipe = recipe;

        EditTitleEntry.Text = recipe.Title;
        EditEstimatedTimeEntry.Text = recipe.EstimatedTime;
        EditIngredientsEditor.Text = recipe.Ingredients;
        EditInstructionsEditor.Text = recipe.Instructions;
        EditRecipeImage.Source = recipe.ImagePath;
        EditRecipeImage.Opacity = 1;
        EditPreviewPlaceholderLabel.IsVisible = false;
    }

    // Function to get the edited recipe from the form
    public Recipe GetEditedRecipe()
    {
        if (_editingRecipe == null)
            return null;

        return new Recipe
        {
            Id = _editingRecipe.Id, 
            Title = EditTitleEntry.Text,
            EstimatedTime = EditEstimatedTimeEntry.Text,
            Ingredients = EditIngredientsEditor.Text,
            Instructions = EditInstructionsEditor.Text,
            ImagePath = string.IsNullOrWhiteSpace(EditImagePathEntry.Text)
                    ? _editingRecipe.ImagePath
                    : EditImagePathEntry.Text
        };
    }

    // Event handlers for button clicks with animations
    private async void OnEditImagePickClicked(object sender, EventArgs e)
    {
        await EditImagePickBtn.ScaleTo(0.90, 60, Easing.CubicOut);
        await EditImagePickBtn.ScaleTo(1, 100, Easing.CubicIn);
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select a Recipe Image",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                EditImagePathEntry.Text = result.FullPath;
                EditRecipeImage.Source = ImageSource.FromFile(result.FullPath);
                EditRecipeImage.Opacity = 1;
                EditPreviewPlaceholderLabel.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            await MyCustomAlert.Show("Error", $"Image selection failed: {ex.Message}", 0);
        }
    }

    private async void OnCancelEditRecipeClicked(object sender, EventArgs e)
    {
        await CancelEditBtn.ScaleTo(0.90, 60, Easing.CubicOut);
        await CancelEditBtn.ScaleTo(1, 100, Easing.CubicIn);
        EditiCanceled?.Invoke(this, EventArgs.Empty);
    }

    // Event handler for saving the edited recipe
    private async void OnEditRecipeClicked(object sender, EventArgs e)
    {
        await SaveEditBtn.ScaleTo(0.90, 60, Easing.CubicOut);
        await SaveEditBtn.ScaleTo(1, 100, Easing.CubicIn);
        var updatedRecipe = GetEditedRecipe();

        if (updatedRecipe.Title == "" || updatedRecipe.Ingredients == "" || updatedRecipe.Instructions == "" || updatedRecipe.EstimatedTime == "")
        {
            ShowDimBackground();

            string missingFields = "";
            if (updatedRecipe.Title == "") missingFields += "Title, ";
            if (updatedRecipe.Ingredients == "") missingFields += "Ingredients, ";
            if (updatedRecipe.Instructions == "") missingFields += "Instructions, ";
            if (updatedRecipe.EstimatedTime == "") missingFields += "Estimated Time, ";

            // Remove trailing comma and space
            if (missingFields.EndsWith(", "))
                missingFields = missingFields.Substring(0, missingFields.Length - 2);

            await MyCustomAlert.Show("Error", $"{missingFields} cannot be empty.", 0);

            HideDimBackground();
            return;
        }

        if (updatedRecipe != null)
        {
            await DatabaseService.UpdateRecipeAsync(updatedRecipe);
            RecipeEdited?.Invoke(this, EventArgs.Empty);
            await HideAsync();
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