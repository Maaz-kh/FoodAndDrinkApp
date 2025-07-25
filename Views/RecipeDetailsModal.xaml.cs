// RecipeDetailsModal.xaml.cs

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

    private void BindRecipeData()
    {
        TitleLabel.Text = _recipe.Title;
        EstimatedTimeLabel.Text = $"Estimated Time: {_recipe.EstimatedTime}";
        IngredientsLabel.Text = _recipe.Ingredients;
        InstructionsLabel.Text = _recipe.Instructions;
        RecipeImage.Source = _recipe.ImagePath; 
    }
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

    public async Task HideAsync()
    {
        await Task.WhenAll(
            this.FadeTo(0, 200),
            this.TranslateTo(0, -200, 200, Easing.CubicIn)
        );
        this.IsVisible = false;
    }

    private void OnCloseClicked(object sender, EventArgs e)
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OnEditClicked(object sender, EventArgs e)
    {
        EditRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OnDeleteClicked(object sender, EventArgs e)
    {
        DeleteRequested?.Invoke(this, EventArgs.Empty);
    }

    public Recipe GetRecipe()
    {
        return _recipe;
    }
}
