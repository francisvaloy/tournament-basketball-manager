namespace Domain.Organizers;
public interface IOrganizerRepository
{
    Task CreateAsync(Organizer organizer, CancellationToken cancellationToken = default);
    Task<Organizer> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Match>> GetTournamentMatches(Guid organizerId, CancellationToken cancellationToken = default);
}