namespace NeuralBee.Core.Models;

public class Track
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Album { get; set; }
    public int TrackNumber { get; set; }
    public TimeSpan Duration { get; set; }
    public string FilePath { get; set; }
}
