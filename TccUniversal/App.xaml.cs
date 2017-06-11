using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace TccUniversal
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static Frame rootFrame;
        public static string email;
        public static string login;
        public static string senha;
        public static bool validador;

        public UserResponse usuarioLogado { get; set; }
        public WriteableBitmap imgTemp { get; set; }
        public PostsResponse postRef { get; set; }

        private TransitionCollection transitions;

        public event Action<IReadOnlyList<StorageFile>> FilesOpenPicked;
        public event Action<StorageFile> FileSavePicked;
        public event EventHandler<BackPressedEventArgs> BackPressed;
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
        protected override void OnActivated(IActivatedEventArgs args)
        {
            var fopArgs = args as FileOpenPickerContinuationEventArgs;
            if (fopArgs != null)
            {
                // Pass the picked files to the subscribed event handlers
                // In a real world app you could also use a Messenger, Listener or any other subscriber-based model
                if (fopArgs.Files.Any() && FilesOpenPicked != null)
                {
                    FilesOpenPicked(fopArgs.Files);
                }
            }

            var fspArgs = args as FileSavePickerContinuationEventArgs;
            if (fspArgs != null)
            {
                // Pass the picked file to the subscribed event handlers
                // In a real world app you could also use a Messenger, Listener or any other subscriber-based model
                if (fspArgs.File != null && FileSavePicked != null)
                {
                    FileSavePicked(fspArgs.File);
                }
            }
        }
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            var handler = this.BackPressed;
            if (handler != null)
            {
                handler(sender, e);
            }

            if (frame.CanGoBack && !e.Handled)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }
        public static async void addLoad(bool ativar, string msgm)
        {
            if (ativar)
            {
                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.ProgressIndicator.Text = msgm;
                    await statusBar.ProgressIndicator.ShowAsync();
                }
            }
            else
            {
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    var statusBar = StatusBar.GetForCurrentView();
                    if (statusBar != null)
                    {
                        await statusBar.ProgressIndicator.HideAsync();
                    }
                }
            }
        }
    }
}
