using Microsoft.UI.Xaml.Controls;
using NeuralBee.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection; // Required for GetRequiredService

namespace NeuralBee.UI.Views;

public sealed partial class LibraryView : Page
{
    public LibraryViewModel ViewModel { get; }

    public LibraryView()
    {
        this.InitializeComponent();
        ViewModel = App.Host.Services.GetRequiredService<LibraryViewModel>();
    }
}
