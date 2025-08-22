using HorsesForCourses.Core;

namespace HorsesForCourses.Tests;

public class CoachTests
{
    [Fact]
    public void Coach_Constructor_InitializesPropertiesCorrectly()
    {
        FullName name = new FullName("Ine", "De Wit");
        EmailAddress email = EmailAddress.Create("Ine.dewit@gmail.com");

        Coach coach = new Coach(name, email);

        Assert.Equal(name, coach.Name);
        Assert.Equal(email, coach.Email);
        Assert.Empty(coach.Skills);                        // Competenties moeten leeg zijn bij initialisatie
    }

    [Fact]
    public void CreateCoach_ReturnsNewCoachWithValidData()
    {
        string name = "Ine De Wit";
        string email = "Ine.dewit@gmail.com";

        Coach coach = Coach.Create(0, "Ine De Wit", "Ine.dewit@gmail.com");

        Assert.Equal(0, coach.Id);
        Assert.Equal(name, coach.Name.DisplayName);
        Assert.Equal(email, coach.Email.Value);
        Assert.Empty(coach.Skills);
    }

    [Theory]
    [InlineData(42, "", "test@example.com")]
    [InlineData(42, "   ", "test@example.com")]
    [InlineData(42, "Test Coach", null)]
    [InlineData(42, "Test Coach", "invalid-email")]
    public void CreateCoach_ThrowsArgumentExceptionForInvalidInput(int id, string name, string email)
    {
        Assert.Throws<ArgumentException>(() => Coach.Create(id, name, email));
    }

    [Fact]
    public void AddSkill_AddsNewSkillToList()
    {
        FullName name = new FullName("Test", "Coach");
        EmailAddress email = EmailAddress.Create("test@example.com");
        Coach coach = new Coach(name, email);
        string skill = "c# programming";

        coach.AddSkill(skill);

        Assert.Contains(skill, coach.Skills);
        Assert.Single(coach.Skills);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AddSkill_InvalidInput_ThrowsArgumentException(string invalidSkill)
    {
        FullName name = new FullName("Test", "Coach");
        EmailAddress email = EmailAddress.Create("test@example.com");
        Coach coach = new Coach(name, email);

        Assert.Throws<ArgumentException>(() => coach.AddSkill(invalidSkill));
    }

    [Fact]
    public void AddSkill_ThrowsInvalidOperationExceptionWhenSkillAlreadyExists()
    {
        FullName name = new FullName("Test", "Coach");
        EmailAddress email = EmailAddress.Create("test@example.com");
        Coach coach = new Coach(name, email);
        string skill = "Dutch";
        coach.AddSkill(skill);

        Assert.Throws<InvalidOperationException>(() => coach.AddSkill("dutch"));
    }

    [Fact]
    public void RemoveSkill_RemovesExistingSkill()
    {
        FullName name = new FullName("Test", "Coach");
        EmailAddress email = EmailAddress.Create("test@example.com");
        Coach coach = new Coach(name, email);
        coach.AddSkill("French");
        coach.AddSkill("C# Programming");

        coach.RemoveSkill("french");

        Assert.DoesNotContain("French", coach.Skills);
        Assert.Single(coach.Skills);
        Assert.Contains("c# programming", coach.Skills);
    }

    [Fact]
    public void RemoveSkills_ThrowsInvalidOperationExceptionWhenSkillNotFound()
    {
        FullName name = new FullName("Test", "Coach");
        EmailAddress email = EmailAddress.Create("test@example.com");
        Coach coach = new Coach(name, email);
        coach.AddSkill("C# Programming");

        Assert.Throws<InvalidOperationException>(() => coach.RemoveSkill("French"));
        Assert.Contains("c# programming", coach.Skills);
    }

    [Fact]
    public void ClearSkills_RemovesAllSkills()
    {
        FullName name = new FullName("Test", "Coach");
        EmailAddress email = EmailAddress.Create("test@example.com");
        Coach coach = new Coach(name, email);
        coach.AddSkill("C# Programming");
        coach.AddSkill("Dutch");

        coach.ClearSkills();

        Assert.Empty(coach.Skills);
    }

    [Fact]
    public void HasAllRequiredSkills_ReturnsTrueWhenOk()
    {
        FullName name = new FullName("Test", "Coach");
        EmailAddress email = EmailAddress.Create("test@example.com");
        Coach coach = new Coach(name, email);
        coach.AddSkill("C# Programming");
        coach.AddSkill("dutch");
        coach.AddSkill("French");

        List<string> requiredSkills = new List<string> { "C# Programming", "Dutch" };

        bool result = coach.HasAllRequiredSkills(requiredSkills);

        Assert.True(result);
    }

    [Fact]
    public void HasAllRequiredSkills_ReturnsFalseWhenOneOrMoreSkillsAreMissing()
    {
        FullName name = new FullName("Test", "Coach");
        EmailAddress email = EmailAddress.Create("test@example.com");
        Coach coach = new Coach(name, email);
        coach.AddSkill("C# Programming");
        coach.AddSkill("Dutch");

        List<string> requiredSkills = new List<string> { "C# Programming", "French" };

        bool result = coach.HasAllRequiredSkills(requiredSkills);

        Assert.False(result);
    }
}