using Foundation;
using MediaManager;
using UIKit;

namespace SpellingTest.Maui
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            CrossMediaManager.Current.Init();
            return base.FinishedLaunching(application, launchOptions);
 
          
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}