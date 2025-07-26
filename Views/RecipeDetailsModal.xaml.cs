// RecipeDetailsModal.xaml.cs
using FoodAndDrinkApp.Services;
using FoodAndDrinkApp.Models;
namespace FoodAndDrinkApp.Views;

public partial class RecipeDetailsModal : ContentView
{
    private Recipe _recipe;

    public event EventHandler EditRequested;
    public event EventHandler DeleteRequested;
    public event EventHandler CloseRequested;

    public Action OnClose;
    public Action<Recipe> OnDelete;
    public Action<Recipe> OnEdit;

    public RecipeDetailsModal()
    {
        this.InitializeComponent();
        this.IsVisible = false;
    }
    public void LoadRecipe(Recipe recipe)
    {
        _recipe = recipe;
        BindRecipeData();
    }
    // Binds the recipe data to the UI elements
    private void BindRecipeData()
    {
        TitleLabel.Text = _recipe.Title;
        EstimatedTimeLabel.Text = $"Estimated Time: {_recipe.EstimatedTime}";
        IngredientsLabel.Text = _recipe.Ingredients;
        InstructionsLabel.Text = _recipe.Instructions;
        RecipeImage.Source = _recipe.ImagePath; 
    }
    // Shows the modal with an animation
    public async Task ShowAsync()
    {
        this.IsVisible = true;
        this.Opacity = 0;
        this.TranslationY = -200;

        await Task.WhenAll(
            this.FadeTo(1, 250),
            this.TranslateTo(0, 0, 250, Easing.CubicOut)
        );
    }
    // Hides the modal with an animation
    public async Task HideAsync()
    {
        await Task.WhenAll(
            this.FadeTo(0, 200),
            this.TranslateTo(0, -200, 200, Easing.CubicIn)
        );
        this.IsVisible = false;
    }

    // Event handlers for button clicks with animations
    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await CancelDetailsBtn.ScaleTo(0.90, 60, Easing.CubicOut);
        await CancelDetailsBtn.ScaleTo(1, 100, Easing.CubicIn);
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    // Event handlers for button clicks with animations
    private async void OnEditClicked(object sender, EventArgs e)
    {
        await EditBtn.ScaleTo(0.90, 60, Easing.CubicOut);
        await EditBtn.ScaleTo(1, 100, Easing.CubicIn);
        EditRequested?.Invoke(this, EventArgs.Empty);
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        await DeleteBtn.ScaleTo(0.90, 60, Easing.CubicOut);
        await DeleteBtn.ScaleTo(1, 100, Easing.CubicIn);
        await DeleteRecipeAsync();
    }
    // Deletes the recipe from the database and hides the modal
    private async Task DeleteRecipeAsync()
    {

        if (_recipe != null)
        {
            await DatabaseService.DeleteRecipeAsync(_recipe);    // Delete the recipe from the database
            await HideAsync();    // Hide the Modal after deletion
            DeleteRequested?.Invoke(this, EventArgs.Empty);  // Notify parent about the deletion
        }
    }

    public Recipe GetRecipe()
    {
        return _recipe;
    }
}
