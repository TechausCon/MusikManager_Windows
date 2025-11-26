using NeuralBee.Core.Interfaces;
using NeuralBee.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NeuralBee.UI.ViewModels;

public partial class LibraryViewModel
{
    private readonly ILibraryService _libraryService;

    public ObservableCollection<Album> Albums { get; } = new();

    public LibraryViewModel(ILibraryService libraryService)
    {
        _libraryService = libraryService;
        _ = LoadAlbumsAsync();
    }

    private async Task LoadAlbumsAsync()
    {
        try
        {
            var albums = await _libraryService.GetAllAlbumsAsync();
            foreach (var album in albums)
            {
                Albums.Add(album);
            }
        }
        catch (Exception ex)
        {
            // In a real app, this should be logged to a proper logging service.
            Debug.WriteLine($"Error loading albums: {ex.Message}");
            // Optionally, update the UI to show an error message.
        }
    }
}
