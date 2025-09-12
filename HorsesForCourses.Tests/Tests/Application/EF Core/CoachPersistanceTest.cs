using HorsesForCourses.Core;
using HorsesForCourses.Application;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Tests;

public class CoachPersistanceTests : DbContextTestBase
{
    [Fact]
    public async Task Coach_CanBeSavedAndRetrieved_PropertiesAreCorrect()
    {
        var id = 0;
        var fullName = FullName.From("Jan De Tester");
        var email = EmailAddress.Create("jan.tester@example.com");
        var newCoach = new Coach(fullName, email);

        _context.Coaches.Add(newCoach);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear(); // Maak de context leeg om zeker te zijn dat we data uit de DB halen

        var savedCoach = await _context.Coaches.FindAsync(newCoach.Id);

        Assert.NotNull(savedCoach);
        Assert.Equal(newCoach.Id, savedCoach.Id);
        Assert.Equal("Jan De Tester", savedCoach.Name.DisplayName);
        Assert.Equal("jan.tester@example.com", savedCoach.Email.Value);
    }

    [Fact]
    public async Task Coach_CanBeSavedAndRetrieved_WithSkills()
    {
        // Maak eerst de Skill entiteiten aan en sla ze op.
        var skill1 = new Skill("c#");
        var skill2 = new Skill("testing");
        _context.Skills.AddRange(skill1, skill2);
        await _context.SaveChangesAsync();

        // Maak de Coach aan
        var newCoach = new Coach(FullName.From("Marie Curie"), EmailAddress.Create("marie@radium.com"));

        // Maak de koppelingen (CoachSkill)
        var coachSkill1 = new CoachSkill { Coach = newCoach, Skill = skill1 };
        var coachSkill2 = new CoachSkill { Coach = newCoach, Skill = skill2 };

        // Voeg de koppelingen toe aan de context. EF Core is slim genoeg om de relaties te begrijpen.
        _context.CoachSkills.AddRange(coachSkill1, coachSkill2);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var savedCoach = await _context.Coaches
            .Include(c => c.CoachSkills)
            .ThenInclude(cs => cs.Skill)
            .FirstOrDefaultAsync(c => c.Id == newCoach.Id);

        Assert.NotNull(savedCoach);
        Assert.Equal(2, savedCoach.CoachSkills.Count);

        var skillNames = savedCoach.CoachSkills.Select(cs => cs.Skill.Name).ToList();
        Assert.Contains("c#", skillNames);
        Assert.Contains("testing", skillNames);
    }

    [Fact]
    public async Task Coach_SavingDuplicateEmail_ThrowsDbUpdateException()
    {
        var email = EmailAddress.Create("duplicate@example.com");
        var coach1 = new Coach(FullName.From("Coach Een"), email);
        var coach2 = new Coach(FullName.From("Coach Twee"), email);

        _context.Coaches.Add(coach1);
        await _context.SaveChangesAsync();

        _context.Coaches.Add(coach2);

        await Assert.ThrowsAsync<DbUpdateException>(() => _context.SaveChangesAsync());
    }
}