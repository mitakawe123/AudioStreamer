namespace AudioStreamer.Models;

public record StreamJobDto(
    Guid JobId,
    string Language,
    string Url);