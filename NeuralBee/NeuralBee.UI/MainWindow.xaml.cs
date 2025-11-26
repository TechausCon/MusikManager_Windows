using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using Windows.Graphics;
using NeuralBee.UI.Views; // Required for page views
using System;

namespace NeuralBee.UI;

public sealed partial class MainWindow : Window
{
    private AppWindow _appWindow;

    public MainWindow()
    {
        this.InitializeComponent();
        this.Loaded += MainWindow_Loaded;
        SetupWindow();
    }

    private void SetupWindow()
    {
        Title = "NeuralBee";

        var hwnd = WindowNative.GetWindowHandle(this);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
        _appWindow = AppWindow.GetFromWindowId(windowId);

        if (_appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.SetBorderAndTitleBar(true, false);
        }

        if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
        {
            var micaController = new Microsoft.UI.Composition.SystemBackdrops.MicaController();
            micaController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
        }
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (_appWindow != null)
        {
            var scale = this.AppWindow.RasterizationScale;
            _appWindow.TitleBar.SetDragRectangles(new RectInt32[]
            {
                new RectInt32(
                    (int)(AppTitleBar.Margin.Left * scale),
                    (int)(AppTitleBar.Margin.Top * scale),
                    (int)(AppTitleBar.ActualWidth * scale),
                    (int)(AppTitleBar.ActualHeight * scale)
                )
            });
        }
        // Set initial page
        ContentFrame.Navigate(typeof(HomePage));
    }

    private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.IsSettingsInvoked)
        {
            // Navigate to settings page
        }
        else
        {
            var item = args.InvokedItemContainer;
            if (item != null)
            {
                Type navPageType = Type.GetType($"NeuralBee.UI.Views.{item.Tag}");
                if (navPageType != null)
                {
                    ContentFrame.Navigate(navPageType);
                }
            }
        }
    }
}
