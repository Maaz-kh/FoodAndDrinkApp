using FoodAndDrinkApp.Models;
using FoodAndDrinkApp.Services;

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

        // Function to show the modal
        public async Task ShowAsync()
        {
            previewPlaceholderLabel.IsVisible = true;
            this.IsVisible = true;
            await this.FadeTo(1, 200);
            await AddRecipeForm.TranslateTo(0, (this.Height / 2) - 250, 300, Easing.CubicOut);
        }

        // Function to hide the modal
        public async Task HideAsync()
        {
            previewPlaceholderLabel.IsVisible = false;
            await AddRecipeForm.TranslateTo(0, -500, 300, Easing.CubicIn);
            await this.FadeTo(0, 200);
            this.IsVisible = false;
        }

        // Function to get the recipe data from the form
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

        // Function to reset the form fields
        public void Reset()
        {
            titleEntry.Text = estimatedTimeEntry.Text = "";
            ingredientsEditor.Text = instructionsEditor.Text = "";
            imagePathEntry.Text = "";
            recipeImage.Source = "";
            previewPlaceholderLabel.IsVisible = true;
        }

        // Event handlers for button clicks
        private async void OnCancelAddRecipeClicked(object sender, EventArgs e)
        {
            await CancelAddBtn.ScaleTo(0.90, 60, Easing.CubicOut);
            await CancelAddBtn.ScaleTo(1, 100, Easing.CubicIn);
            Reset();
            CancelRequested?.Invoke(this, EventArgs.Empty);
        }

        // Event handler for the save button click
        private async void OnSaveRecipeClicked(object sender, EventArgs e)
        {
            await SaveAddBtn.ScaleTo(0.90, 60, Easing.CubicOut);
            await SaveAddBtn.ScaleTo(1, 100, Easing.CubicIn);

            try
            {
                var newRecipe = GetRecipe();

                // Trim inputs before checking
                string title = newRecipe.Title?.Trim() ?? "";
                string ingredients = newRecipe.Ingredients?.Trim() ?? "";
                string instructions = newRecipe.Instructions?.Trim() ?? "";
                string estimatedTime = newRecipe.EstimatedTime?.Trim() ?? "";

                if (title == "" || ingredients == "" || instructions == "" || estimatedTime == "")
                {
                    ShowDimBackground();

                    string missingFields = "";
                    if (title == "") missingFields += "Title, ";
                    if (ingredients == "") missingFields += "Ingredients, ";
                    if (instructions == "") missingFields += "Instructions, ";
                    if (estimatedTime == "") missingFields += "Estimated Time, ";

                    // Remove trailing comma and space
                    if (missingFields.EndsWith(", "))
                        missingFields = missingFields[..^2]; // C# 8+ range operator

                    await MyCustomAlert.Show("Error", $"{missingFields} cannot be empty.", 0);

                    HideDimBackground();
                    return;
                }

                // Assign trimmed values back if needed
                newRecipe.Title = title;
                newRecipe.Ingredients = ingredients;
                newRecipe.Instructions = instructions;
                newRecipe.EstimatedTime = estimatedTime;

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

        // Event handlers for image selection and capture
        private async void OnImagePickClicked(object sender, EventArgs e)
        {
            await SelectImgBtn.ScaleTo(0.90, 60, Easing.CubicOut);
            await SelectImgBtn.ScaleTo(1, 100, Easing.CubicIn);
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
        // Event handler for capturing an image using the camera
        private async void OnCaptureImageClicked(object sender, EventArgs e)
        {
            await CaptureImgBtn.ScaleTo(0.90, 60, Easing.CubicOut);
            await CaptureImgBtn.ScaleTo(1, 100, Easing.CubicIn);

            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    var photo = await MediaPicker.Default.CapturePhotoAsync();

                    if (photo != null)
                    {
                        var stream = await photo.OpenReadAsync();

                        // Save to temp path or stream (you can store stream directly if needed)
                        var filePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                        using (var fileStream = File.OpenWrite(filePath))
                            await stream.CopyToAsync(fileStream);

                        imagePathEntry.Text = filePath;
                        recipeImage.Source = ImageSource.FromFile(filePath);
                        recipeImage.Opacity = 1;
                        previewPlaceholderLabel.IsVisible = false;
                    }
                }
                else
                {
                    await MyCustomAlert.Show("Error", "Camera not supported on this device.", 0);
                }
            }
            catch (Exception ex)
            {
                ShowDimBackground();
                await MyCustomAlert.Show("Error", $"Camera failed: {ex.Message}", 0);
                HideDimBackground();
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