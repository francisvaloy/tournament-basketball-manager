using Domain.Common;
using Domain.Managers;
using Domain.Organizers;
using Domain.Organizers.Exceptions;
using Domain.Organizers.DomainEvents;

namespace Domain.UnitTests.Organizers;
public class OrganizerTests
{
    [Fact]
    public void IsOrganizingATournament_ShouldBeTrue_WhenOrganizerHaveATournamentAssociated()
    {
        var organizer = Organizer.Create(new("test", "test", "test@gamil.com", DateTime.Now, new Address("", "", "", "", "")));
        organizer.CreateTournament("tt");

        organizer.IsOrganizingATournament.Should().BeTrue();
    }

    [Fact]
    public void IsOrganizingATournament_ShoulBeFalse_WhenTheOrganizerDoesNotHaveATournamentAssociated()
    {
        var organizer = Organizer.Create(new("test", "test", "test@gamil.com", DateTime.Now, new Address("", "", "", "", "")));

        organizer.IsOrganizingATournament.Should().BeFalse();
    }

    [Fact]
    public void Create_ShouldThrowAOrganizerPersonalInfoNullExcpetion_WhenNullOrganizerPersonalInfoIsPassed()
    {
        Action action = () => Organizer.Create(null!);

        action.Should().Throw<OrganizerPersonalInfoNullException>();
    }

    [Fact]
    public void Create_ShouldReturnAnInstanceOfOrganizer_WhenValidOrganizerPersonalInfoIsPassed()
    {
        var validOrganizerPersonalInfo = new OrganizerPersonalInfo("test", "test", "test@gamil.com", DateTime.Now, new Address("", "", "", "", ""));

        var organizer = Organizer.Create(validOrganizerPersonalInfo);

        organizer.Should().NotBeNull();
        organizer.GetType().Should().Be(typeof(Organizer));
    }

    [Fact]
    public void Create_ShouldRaiseAOrganizerCreatedDomainEvent_WhenValidOrganizerPersonalInfoIsPassed()
    {
        var validOrganizerPersonalInfo = new OrganizerPersonalInfo("test", "test", "test@gamil.com", DateTime.Now, new Address("", "", "", "", ""));

        var organizer = Organizer.Create(validOrganizerPersonalInfo);

        var @event = organizer.DomainEvents.FirstOrDefault(de => de.GetType() == typeof(OrganizerCreatedDomainEvent));
        @event.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("       ")]
    public void CreateTournament_ShouldThrowAInvalidTournamentNameException_WhenInvalidTournamentNameIsPassed(string invalidName)
    {
        var organizer = Organizer.Create(new("test", "test", "test@gamil.com", DateTime.Now, new Address("", "", "", "", "")));

        Action action = () => organizer.CreateTournament(invalidName);

        action.Should().Throw<InvalidTournamentNameException>();
    }

    [Fact]
    public void CreateTournament_ShouldCreateATournamentAndAddedToTheOrganizer_WhenValidTournamentNameIsPassed()
    {
        const string validTeamName = "valid team name";
        var organizer = Organizer.Create(new("test", "test", "test@gamil.com", DateTime.Now, new Address("", "", "", "", "")));

        organizer.CreateTournament(validTeamName);

        organizer.Tournament!.Name.Should().Be(validTeamName);
    }

    [Fact]
    public void RegisterTeam_ShouldThrowAOrganizerDoesNotHaveTournamentException_WhenTheOrganizerDoesNotHaveATournamentAssociated()
    {
        var organizer = Organizer.Create(new("test", "test", "test@gamil.com", DateTime.Now, new Address("", "", "", "", "")));

        Action action = () => organizer.RegisterTeam(null!);

        action.Should().Throw<OrganizerDoesNotHaveTournament>();
    }

    [Fact]
    public void RegisterTeam_ShouldRegisterATeamInTheTournament_WhenTheOrganizerHaveATeamAndValidArgumentArePassed()
    {
        var validTeam = Team.Create("test", Manager.Create(new("test", "test", "test", DateTime.Now, new("test", "test", "test", "test", "test"))));
        var organizer = Organizer.Create(new("test", "test", "test@gamil.com", DateTime.Now, new Address("", "", "", "", "")));
        organizer.CreateTournament("tournament");

        organizer.RegisterTeam(validTeam);

        organizer.Tournament!.Teams.Should().Contain(validTeam);
        validTeam.Tournament.Should().Be(organizer.Tournament);
    }

    [Fact]
    public void DiscardTeam_ShouldThrowAOrganizerDoesNotHaveTournament_WhenTheOrganizerDoesNotHaveATournamentAssociated()
    {
        var organizer = Organizer.Create(new("test", "test", "test@gamil.com", DateTime.Now, new Address("", "", "", "", "")));

        Action action = () => organizer.DiscardTeam(Guid.Empty);

        action.Should().Throw<OrganizerDoesNotHaveTournament>();
    }

    [Fact]
    public void DiscardTeam_ShouldDiscardTheTeamFromTheTournament_WhenTheOrganizerHaveATournamentAndValidArgumentArePassed()
    {
        var validTeam = Team.Create("test", Manager.Create(new("test", "test", "test", DateTime.Now, new("test", "test", "test", "test", "test"))));
        var organizer = Organizer.Create(new("test", "test", "test@gamil.com", DateTime.Now, new Address("", "", "", "", "")));
        organizer.CreateTournament("tournament");
        organizer.RegisterTeam(validTeam);

        organizer.DiscardTeam(validTeam.Id);

        organizer.Tournament!.Teams.Should().NotContain(validTeam);
    }

    [Fact]
    public void GetTournamentMatches_ShouldThrowAOrganizerDoesNotHaveTournament_WhenTheOrganizerDoesNotHaveATournamentAssociated()
    {
        var organizer = Organizer.Create(new("test", "test", "test@gamil.com", DateTime.Now, new Address("", "", "", "", "")));

        Action action = () => organizer.GetTournamentMatches(new RandomTeamMatchMaker());

        action.Should().Throw<OrganizerDoesNotHaveTournament>();
    }

    [Fact]
    public void GetTournamentMatches_ShouldGetTheTournamentMatches_WhenTheOrganizerHaveATournamentAndValidArgumentArePassed()
    {
        var organizer = GetOrganizerWithTournamentAndTeams();

        var matches = organizer.GetTournamentMatches(new RandomTeamMatchMaker());

        matches.Should().NotBeNull();
        matches.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public void FinishTournament_ShouldThrowAOrganizerDoesNotHaveTournament_WhenTheOrganizerDoesNotHaveATournamentAssociated()
    {
        var organizer = Organizer.Create(new("test", "test", "test@gamil.com", DateTime.Now, new Address("", "", "", "", "")));

        Action action = () => organizer.FinishTournament();

        action.Should().Throw<OrganizerDoesNotHaveTournament>();
    }

    [Fact]
    public void FinishTournament_ShouldDiscardAllTeamsInTheTournamentAndDropTheTournament_WhenTheOrganizerIsAssociatedWithATournament()
    {
        var organizer = GetOrganizerWithTournamentAndTeams();
        var tournament = organizer.Tournament;
        var teams = organizer.Tournament!.Teams;

        organizer.FinishTournament();

        tournament!.Teams.Should().BeNullOrEmpty();
        foreach (var team in teams)
        {
            team.Tournament.Should().BeNull();
            team.TournamentId.Should().Be(Guid.Empty);
        }
    }

    [Fact]
    public void FinishTournament_ShouldRaiseATournamentFinishedDomainEvent_WhenFinishTournamentWasSuccess()
    {
        var organizer = GetOrganizerWithTournamentAndTeams();

        organizer.FinishTournament();

        var @event = organizer.DomainEvents.FirstOrDefault(de => de.GetType() == typeof(TournamentFinishedDomainEvent));
        @event.Should().NotBeNull();
    }

    private static Organizer GetOrganizerWithTournamentAndTeams()
    {
        var t1 = Team.Create("test", Manager.Create(new("test1", "test", "test", DateTime.Now, new("test", "test", "test", "test", "test"))));
        var t2 = Team.Create("test", Manager.Create(new("test2", "test", "test", DateTime.Now, new("test", "test", "test", "test", "test"))));
        var organizer = Organizer.Create(new("test", "test", "test@gamil.com", DateTime.Now, new Address("", "", "", "", "")));
        organizer.CreateTournament("test tournament");
        organizer.RegisterTeam(t1);
        organizer.RegisterTeam(t2);
        return organizer;
    }
}