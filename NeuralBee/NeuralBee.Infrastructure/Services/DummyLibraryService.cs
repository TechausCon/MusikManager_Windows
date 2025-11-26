using NeuralBee.Core.Interfaces;
using NeuralBee.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralBee.Infrastructure.Services;

// This is a placeholder implementation for development purposes.
public class DummyLibraryService : ILibraryService
{
    private readonly List<Album> _albums;

    public DummyLibraryService()
    {
        _albums = new List<Album>
        {
            new Album { Id = 1, Title = "Discovery", Artist = "Daft Punk", Year = 2001 },
            new Album { Id = 2, Title = "Random Access Memories", Artist = "Daft Punk", Year = 2013 },
            new Album { Id = 3, Title = "Homework", Artist = "Daft Punk", Year = 1997 },
            new Album { Id = 4, Title = "Alive 2007", Artist = "Daft Punk", Year = 2007 }
        };
    }

    public Task<IEnumerable<Album>> GetAllAlbumsAsync()
    {
        return Task.FromResult(_albums.AsEnumerable());
    }

    public Task<IEnumerable<Track>> GetTracksByAlbumAsync(object albumId)
    {
        // Return a dummy list of tracks for any album
        var tracks = new List<Track>
        {
            new Track { Title = "Track 1", Artist = "Artist", Album = "Album", Duration = TimeSpan.FromMinutes(3) },
            new Track { Title = "Track 2", Artist = "Artist", Album = "Album", Duration = TimeSpan.FromMinutes(4) },
        };
        return Task.FromResult(tracks.AsEnumerable());
    }

    public Task ScanDirectoryAsync(string directoryPath)
    {
        // Simulate a scan
        return Task.CompletedTask;
    }
}
