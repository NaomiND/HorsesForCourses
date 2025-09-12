using System.Reflection;
using HorsesForCourses.Core;

namespace HorsesForCourses.Tests;

public class CoachTests
{
    private readonly Coach coach;

    public CoachTests()
    {
        FullName name = new("Ine", "De Wit");
        EmailAddress email = EmailAddress.Create("Ine.dewit@example.com");
        coach = new Coach(name, email);
    }

    [Fact]
    public void Coach_Constructor_InitializesPropertiesCorrectly()
    {
        Assert.Equal("Ine De Wit", coach.Name.DisplayName);
        Assert.Equal("Ine.dewit@example.com", coach.Email.Value);
    }

    [Fact]
    public void CreateCoach_ReturnsNewCoach_WithValidData()
    {
        string name = "Ine De Wit";
        string email = "Ine.dewit@example.com";

        Coach coach = Coach.Create(name, email);

        Assert.Equal(0, coach.Id);
        Assert.Equal(name, coach.Name.DisplayName);
        Assert.Equal(email, coach.Email.Value);
    }

    [Theory]
    [InlineData(42, "", "test@example.com")]
    [InlineData(42, "   ", "test@example.com")]
    [InlineData(42, "Test Coach", null)]
    [InlineData(42, "Test Coach", "invalid-email")]
    public void CreateCoach_ThrowsArgumentExceptionForInvalidInput(int id, string name, string email)
    {
        Assert.Throws<ArgumentException>(() => Coach.Create(name, email));
    }

    [Fact]
    public void HasAllRequiredSkills_ReturnsTrueWhenValid()
    {
        var skills = new List<CoachSkill>
        {
            new() { Coach = coach, Skill = new Skill("c# programming") },
            new() { Coach = coach, Skill = new Skill("dutch") },
            new() { Coach = coach, Skill = new Skill("french") }
        };
        // Gebruik reflectie om de private readonly 'coachSkills' lijst te vullen.
        var coachSkillsField = typeof(Coach).GetField("coachSkills", BindingFlags.NonPublic | BindingFlags.Instance);
        (coachSkillsField.GetValue(coach) as List<CoachSkill>).AddRange(skills);

        var requiredSkills = new List<string> { "C# Programming", "Dutch" };

        bool result = coach.HasAllRequiredSkills(requiredSkills);

        Assert.True(result);
    }

    [Fact]
    public void HasAllRequiredSkills_ReturnsFalseWhenOneOrMoreSkillsAreMissing()
    {
        var skills = new List<CoachSkill>
        {
            new() { Coach = coach, Skill = new Skill("c# programming") },
            new() { Coach = coach, Skill = new Skill("dutch") }
        };

        var coachSkillsField = typeof(Coach).GetField("coachSkills", BindingFlags.NonPublic | BindingFlags.Instance);
        (coachSkillsField.GetValue(coach) as List<CoachSkill>).AddRange(skills);

        var requiredSkills = new List<string> { "C# Programming", "French" };

        bool result = coach.HasAllRequiredSkills(requiredSkills);

        Assert.False(result);
    }
}