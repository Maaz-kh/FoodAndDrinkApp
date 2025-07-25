namespace FoodAndDrinkApp.Views;
using System;
using Microsoft.Maui.Controls;
using FoodAndDrinkApp.Models;
using FoodAndDrinkApp.Services;

public partial class EditRecipeModal : ContentView
{
    private Recipe _editingRecipe;

    public event EventHandler RecipeEdited;

    public EditRecipeModal()
	{
		InitializeComponent();
        this.IsVisible = false;
        this.Opacity = 0;
    }
    public async Task ShowAsync()
    {
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
            ImagePath = EditImagePathEntry.Text
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
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Image selection failed: {ex.Message}", "OK");
        }
    }
    private async void OnCancelEditRecipeClicked(object sender, EventArgs e)
    {
        await HideAsync();
    }

    private async void OnEditRecipeClicked(object sender, EventArgs e)
    {
        var updatedRecipe = GetEditedRecipe();

        if (updatedRecipe != null)
        {
            await DatabaseService.UpdateRecipeAsync(updatedRecipe);
            RecipeEdited?.Invoke(this, EventArgs.Empty);
            await HideAsync();
        }
    }
}