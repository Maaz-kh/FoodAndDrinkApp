
namespace FoodAndDrinkApp.Views
{
    public partial class CustomAlert : ContentView
    {
        private  TaskCompletionSource<bool> _tcs;

        public CustomAlert()
        {
            InitializeComponent();
            this.IsVisible = false;
            this.Opacity = 0;
        }
        // Function to show the alert with title, message, and optional duration
        public async Task Show(string title, string message, int durationSeconds = 0)
        {
            if (title.Contains("Added") || title.Contains("Success"))
            {
                AlertTitle.TextColor = Color.FromArgb("#4CAF50"); // Green
            }
            else if (title.Contains("Error"))
            {
                AlertTitle.TextColor = Color.FromArgb("#dc3545"); // Red
            }
            else
            {
                AlertTitle.TextColor = Color.FromArgb("#000000"); // Black (default)
            }

            AlertTitle.Text = title;
            AlertMessage.Text = message;
            this.IsVisible = true;

            // Fade In Animation
            await this.FadeTo(1, 300, Easing.CubicIn);

            if (durationSeconds > 0)
            {
                // Auto-close alert after duration
                await Task.Delay(durationSeconds * 1000);
                await HideAsync();
            }
            else
            {
                // Wait for user to press OK
                _tcs = new TaskCompletionSource<bool>();
                await _tcs.Task; // Pause until OK is pressed
                await OkBtn.ScaleTo(0.90, 60, Easing.CubicOut);
                await OkBtn.ScaleTo(1, 100, Easing.CubicIn);
                await HideAsync();
            }
        }

        private async Task HideAsync()
        {
            await this.FadeTo(0, 300, Easing.CubicOut);
            this.IsVisible = false;
        }

        private void OnDismissClicked(object sender, EventArgs e)
        {
            _tcs?.TrySetResult(true); // Complete the task so Show() resumes
        }
    }
}
