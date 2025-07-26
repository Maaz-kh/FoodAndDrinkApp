using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodAndDrinkApp.Utilities
{
    public static class Helper
    {
        // Helper Function for vibration and haptic feedback
        public static void SaveVibrationAndHapticFeedback()
        {
            try
            {
                // Vibration (if supported)
                if (Vibration.Default.IsSupported)
                {
                    Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(100));
                }

                // Haptic feedback (if supported)
                if (HapticFeedback.Default.IsSupported)
                {
                    HapticFeedback.Default.Perform(HapticFeedbackType.Click);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HardwareHelper] Failed to vibrate or haptic feedback: {ex.Message}");
            }
        }
    }
}
