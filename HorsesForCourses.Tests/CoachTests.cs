using HorsesForCourses.Core;
using System;

namespace HorsesForCourses.Tests;

public class CoachTests
{
    [Fact]
    public void Coach_Constructor_InitializesPropertiesCorrectly()
    {
        Guid id = Guid.NewGuid();
        FullName name = new FullName("Ine", "De Wit");
        EmailAddress email = EmailAddress.From("Ine.dewit@gmail.com");

        Coach coach = new Coach(id, name, email);

        Assert.Equal(id, coach.Id);
        Assert.Equal(name, coach.Name);
        Assert.Equal(email, coach.Email);
        Assert.Empty(coach.Competences);                        // Competenties moeten leeg zijn bij initialisatie
    }

    [Fact]
    public void CreateCoach_ReturnsNewCoachWithValidData()
    {
        string name = "Ine De Wit";
        string email = "Ine.dewit@gmail.com";

        Coach coach = Coach.Create(name, email);

        Assert.NotEqual(Guid.Empty, coach.Id);                      // ID moet gegenereerd worden
        Assert.Equal(name, coach.Name.DisplayName);
        Assert.Equal(email, coach.Email.Value);
        Assert.Empty(coach.Competences);
    }

    [Theory]
    [InlineData("", "test@example.com")]
    [InlineData("   ", "test@example.com")]
    [InlineData("Test Coach", null)]
    [InlineData("Test Coach", "invalid-email")]
    public void CreateCoach_ThrowsArgumentExceptionForInvalidInput(string name, string email)
    {
        Assert.Throws<ArgumentException>(() => Coach.Create(name, email));
    }

    [Fact]
    public void AddCompetence_AddsNewCompetenceToList()
    {
        FullName name = new FullName("Test", "Coach");
        EmailAddress email = EmailAddress.From("test@example.com");
        Coach coach = new Coach(Guid.NewGuid(), name, email);
        string competence = "C# Programming";

        coach.AddCompetence(competence);

        Assert.Contains(competence, coach.Competences);
        Assert.Single(coach.Competences);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AddCompetence_InvalidInput_ThrowsArgumentException(string invalidCompetence)
    {
        FullName name = new FullName("Test", "Coach");
        EmailAddress email = EmailAddress.From("test@example.com");
        Coach coach = new Coach(Guid.NewGuid(), name, email);

        Assert.Throws<ArgumentException>(() => coach.AddCompetence(invalidCompetence));
    }

    [Fact]
    public void AddCompetence_ThrowsInvalidOperationExceptionWhenCompetenceAlreadyExists()
    {
        FullName name = new FullName("Test", "Coach");
        EmailAddress email = EmailAddress.From("test@example.com");
        Coach coach = new Coach(Guid.NewGuid(), name, email);
        string competence = "Dutch";
        coach.AddCompetence(competence);

        Assert.Throws<InvalidOperationException>(() => coach.AddCompetence("dutch"));
    }

    [Fact]
    public void RemoveCompetence_RemovesExistingCompetence()
    {
        FullName name = new FullName("Test", "Coach");
        EmailAddress email = EmailAddress.From("test@example.com");
        Coach coach = new Coach(Guid.NewGuid(), name, email);
        coach.AddCompetence("French");
        coach.AddCompetence("C# Programming");

        coach.RemoveCompetence("french");

        Assert.DoesNotContain("French", coach.Competences);
        Assert.Single(coach.Competences);
        Assert.Contains("C# Programming", coach.Competences);
    }

    [Fact]
    public void RemoveCompetence_ThrowsInvalidOperationExceptionWhenCompetenceNotFound()
    {
        FullName name = new FullName("Test", "Coach");
        EmailAddress email = EmailAddress.From("test@example.com");
        Coach coach = new Coach(Guid.NewGuid(), name, email);
        coach.AddCompetence("C# Programming");

        Assert.Throws<InvalidOperationException>(() => coach.RemoveCompetence("French"));
        Assert.Contains("C# Programming", coach.Competences);
    }

    [Fact]
    public void ClearCompetences_RemovesAllCompetences()
    {
        FullName name = new FullName("Test", "Coach");
        EmailAddress email = EmailAddress.From("test@example.com");
        Coach coach = new Coach(Guid.NewGuid(), name, email);
        coach.AddCompetence("C# Programming");
        coach.AddCompetence("Dutch");

        coach.ClearCompetences();

        Assert.Empty(coach.Competences);
    }

    [Fact]
    public void HasAllRequiredCompetences_ReturnsTrueWhenOk()
    {
        FullName name = new FullName("Test", "Coach");
        EmailAddress email = EmailAddress.From("test@example.com");
        Coach coach = new Coach(Guid.NewGuid(), name, email);
        coach.AddCompetence("C# Programming");
        coach.AddCompetence("dutch");
        coach.AddCompetence("French");

        List<string> requiredCompetences = new List<string> { "C# Programming", "Dutch" };

        bool result = coach.HasAllRequiredCompetences(requiredCompetences);

        Assert.True(result);
    }

    [Fact]
    public void HasAllRequiredCompetences_ReturnsFalseWhenOneOrMoreCompetencesAreMissing()
    {
        FullName name = new FullName("Test", "Coach");
        EmailAddress email = EmailAddress.From("test@example.com");
        Coach coach = new Coach(Guid.NewGuid(), name, email);
        coach.AddCompetence("C# Programming");
        coach.AddCompetence("Dutch");

        List<string> requiredCompetences = new List<string> { "C# Programming", "French" };

        bool result = coach.HasAllRequiredCompetences(requiredCompetences);

        Assert.False(result);
    }
}