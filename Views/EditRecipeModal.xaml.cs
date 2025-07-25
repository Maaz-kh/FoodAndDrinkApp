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
    public async Task ShowAsync()
    {
        EditPreviewPlaceholderLabel.IsVisible = true;
        this.IsVisible = true;
        await this.FadeTo(1, 200);
        await EditformWrapper.TranslateTo(0, (this.Height / 2) - 250, 600, Easing.CubicOut);
    }
    public async Task HideAsync()
    {
        await EditformWrapper.TranslateTo(0, -500, 300, Easing.CubicIn);
        await this.FadeTo(0, 200);
        this.IsVisible = false;
    }
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

    private async void OnEditImagePickClicked(object sender, EventArgs e)
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
        EditiCanceled?.Invoke(this, EventArgs.Empty);
    }

    private async void OnEditRecipeClicked(object sender, EventArgs e)
    {
        var updatedRecipe = GetEditedRecipe();

        if (string.IsNullOrWhiteSpace(updatedRecipe.Title))
        {
            ShowDimBackground();
            await MyCustomAlert.Show("Error", "Title is required.", 0);
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