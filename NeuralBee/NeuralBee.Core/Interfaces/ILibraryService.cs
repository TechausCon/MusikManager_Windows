using NeuralBee.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuralBee.Core.Interfaces;

/// <summary>
/// Defines the contract for a service that manages the music library.
/// </summary>
public interface ILibraryService
{
    /// <summary>
    /// Gets all albums from the library.
    /// </summary>
    /// <returns>A collection of all albums.</returns>
    Task<IEnumerable<Album>> GetAllAlbumsAsync();

    /// <summary>
    /// Gets all tracks for a specific album.
    /// </summary>
    /// <param name="albumId">The unique identifier for the album.</param>
    /// <returns>A collection of tracks for the specified album.</returns>
    Task<IEnumerable<Track>> GetTracksByAlbumAsync(object albumId);

    /// <summary>
    /// Scans a directory for music files and adds them to the library.
    /// </summary>
    /// <param name="directoryPath">The path to the directory to scan.</param>
    /// <returns>A task representing the asynchronous scanning operation.</returns>
    Task ScanDirectoryAsync(string directoryPath);
}
