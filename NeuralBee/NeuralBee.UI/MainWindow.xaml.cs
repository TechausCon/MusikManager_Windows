using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Windowing;
using WinRT;
using WinRT.Interop;
using Windows.Graphics;
using NeuralBee.UI.Views; // Required for page views
using NeuralBee.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace NeuralBee.UI;

public sealed partial class MainWindow : Window
{
    private AppWindow _appWindow;
    public PlayerViewModel ViewModel { get; }

    public MainWindow()
    {
        this.InitializeComponent();
        ViewModel = App.Host.Services.GetRequiredService<PlayerViewModel>();
        RootGrid.Loaded += MainWindow_Loaded;
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
        // Set DataContext for the bottom player bar
        // Note: In XAML we could bind ElementName or x:Bind to ViewModel if we exposed it.
        // For simplicity with the existing structure, we can set the specific Grid's DataContext
        // or just let it inherit if we set it at Root.
        // But since NavView handles the main content, we only want the bottom bar to use PlayerViewModel.
        // Actually, let's find the Border in the visual tree or give it a name.
        // Let's give the PlayerControlsBorder a name in XAML first to make this cleaner,
        // or just set RootGrid.DataContext = this (and expose ViewModel property).
        // Let's go with setting the DataContext of the specific part if possible,
        // but naming it is better. For now, I'll rely on the update I made to MainWindow.xaml.
        // Wait, I didn't name the Border in the previous step.
        // Let's retry that part or just set it on the specific child index if needed?
        // No, that's brittle.
        // Better: Set RootGrid.DataContext to ViewModel? No, that messes up the NavView content frames potentially?
        // Actually, NavView content frames usually have their own DataContexts from their Pages.
        // So setting RootGrid.DataContext to an object that holds the PlayerViewModel is fine,
        // OR we can just bind to the Window's property using x:Bind.
        // I used {Binding ...} in XAML which implies DataContext.

        // Let's set the DataContext of the Player Controls Border.
        // I need to update XAML to give it a name to be safe.
        // Or I can use x:Bind to ViewModel.Command since I exposed ViewModel on MainWindow.

        PlayerControlsBorder.DataContext = ViewModel;

        if (_appWindow != null)
        {
            var scale = RootGrid.XamlRoot.RasterizationScale;
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

    private void OnSeekSliderPointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        ViewModel.StartSeek();
    }

    private void OnSeekSliderPointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (sender is Slider slider)
        {
            ViewModel.EndSeek(slider.Value);
        }
    }
}
