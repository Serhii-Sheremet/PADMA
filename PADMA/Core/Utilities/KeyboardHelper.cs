using Microsoft.Maui.Platform;
#if IOS
using UIKit;
#endif

namespace PADMA.Core.Utilities
{
    public static class KeyboardHelper
    {
        public static void HideKeyboard()
        {
#if ANDROID
            var context = Platform.CurrentActivity;
            var view = context?.CurrentFocus;
            if (view != null)
            {
                var imm = (Android.Views.InputMethods.InputMethodManager)
                    context.GetSystemService(Android.Content.Context.InputMethodService);
                imm?.HideSoftInputFromWindow(view.WindowToken, 0);
                view.ClearFocus();
            }

#elif IOS
            try
            {
                // Находим активное окно (iOS 15–17)
                var windowScene = UIApplication.SharedApplication.ConnectedScenes
                    .OfType<UIWindowScene>()
                    .FirstOrDefault();

                var window = windowScene?.Windows?.FirstOrDefault(w => w.IsKeyWindow);
                var rootView = window?.RootViewController?.View;

                if (rootView != null)
                {
                    // Скрываем клавиатуру — правильный вызов без UIViewExtensions
                    rootView.EndEditing(true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] HideKeyboard iOS failed: {ex.Message}");
            }
#endif
        }
    }
}
