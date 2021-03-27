namespace Orchestrate.Data.Models
{
    public record UserIdentifier(int UserId);

    public record RoleIdentifier(int RoleId);

    public record GroupIdentifier(int GroupId);

    public record ConcertIdentifier(int GroupId, int ConcertId);

    public record CompositionIdentifier(int GroupId, int CompositionId);

    public record SheetMusicIdentifier(int GroupId, int CompositionId, int RoleId);
}