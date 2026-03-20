using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;

namespace PADMA;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = Android.Content.PM.LaunchMode.SingleTop,
    ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize
                         | Android.Content.PM.ConfigChanges.Orientation
                         | Android.Content.PM.ConfigChanges.UiMode
                         | Android.Content.PM.ConfigChanges.ScreenLayout
                         | Android.Content.PM.ConfigChanges.SmallestScreenSize
                         | Android.Content.PM.ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void AttachBaseContext(Context? @base)
    {
        if (@base == null)
        {
            base.AttachBaseContext(@base);
            return;
        }

        var configuration = new Configuration(@base.Resources?.Configuration);
        configuration.FontScale = 1.0f;

        var adjustedContext = @base.CreateConfigurationContext(configuration);
        base.AttachBaseContext(adjustedContext);
    }

    public override void OnConfigurationChanged(Configuration newConfig)
    {
        if (newConfig != null)
            newConfig.FontScale = 1.0f;

        base.OnConfigurationChanged(newConfig);
    }
}