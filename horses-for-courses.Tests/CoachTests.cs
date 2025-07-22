using horses_for_courses.Core;
using System;

namespace horses_for_courses.Tests;

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
        // Assert.Empty(coach.Competences);                        // Competenties moeten leeg zijn bij initialisatie
    }
}

