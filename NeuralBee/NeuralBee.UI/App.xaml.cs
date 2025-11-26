using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using NeuralBee.Core.Interfaces;
using NeuralBee.Infrastructure.Audio;
using NeuralBee.Infrastructure.Services; // Namespace for DummyLibraryService
using NeuralBee.UI.ViewModels;
using System;

namespace NeuralBee.UI;

public partial class App : Application
{
    public static IHost Host { get; private set; }

    public App()
    {
        this.InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Register Services
                services.AddSingleton<IAudioService, WasapiAudioService>();
                services.AddSingleton<ILibraryService, DummyLibraryService>(); // Using the dummy service

                // Register ViewModels
                services.AddTransient<MainViewModel>();
                services.AddTransient<LibraryViewModel>();

                // Register Views (as services to be resolved)
                services.AddTransient<MainWindow>();
            })
            .Build();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        m_window = Host.Services.GetRequiredService<MainWindow>();
        m_window.Activate();
    }

    private Window m_window;
}
