using HorsesForCourses.Core;

namespace HorsesForCourses.Tests;

public class SkillsTests
{
    [Fact]
    public void Constructor_WithValidName_InitializesPropertiesCorrectly()
    {
        var skillName = "C# Programming";

        var skill = new Skill(skillName);

        Assert.Equal("c# programming", skill.Name);
        Assert.Equal(0, skill.Id); // Id = 0 want nog niet opgeslagen in db
    }

    [Fact]
    public void Constructor_NormalizesNameToLowercase()
    {
        var skillNameWithMixedCase = "JavaScript";

        var skill = new Skill(skillNameWithMixedCase);

        Assert.Equal("javascript", skill.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ThrowsArgumentException(string invalidName)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Skill(invalidName));

        Assert.Contains("Skill cannot be empty.", exception.Message);
        Assert.Equal("name", exception.ParamName);
    }

    //skilltests voor coach
    //     [Fact]
    //     public void AddSkill_AddsNewSkillToList()
    //     {
    //         FullName name = new FullName("Test", "Coach");
    //         EmailAddress email = EmailAddress.Create("test@example.com");
    //         Coach coach = new Coach(name, email);
    //         string skill = "c# programming";

    //         coach.AddSkill(skill);

    //         Assert.Contains(skill, coach.Skills);
    //         Assert.Single(coach.Skills);
    //     }

    //     [Theory]
    //     [InlineData("")]
    //     [InlineData("   ")]
    //     public void AddSkill_InvalidInput_ThrowsArgumentException(string invalidSkill)
    //     {
    //         FullName name = new FullName("Test", "Coach");
    //         EmailAddress email = EmailAddress.Create("test@example.com");
    //         Coach coach = new Coach(name, email);

    //         Assert.Throws<ArgumentException>(() => coach.AddSkill(invalidSkill));
    //     }

    //     [Fact]
    //     public void AddSkill_ThrowsInvalidOperationExceptionWhenSkillAlreadyExists()
    //     {
    //         FullName name = new FullName("Test", "Coach");
    //         EmailAddress email = EmailAddress.Create("test@example.com");
    //         Coach coach = new Coach(name, email);
    //         string skill = "Dutch";
    //         coach.AddSkill(skill);

    //         Assert.Throws<InvalidOperationException>(() => coach.AddSkill("dutch"));
    //     }

    //     [Fact]
    //     public void RemoveSkill_RemovesExistingSkill()
    //     {
    //         FullName name = new FullName("Test", "Coach");
    //         EmailAddress email = EmailAddress.Create("test@example.com");
    //         Coach coach = new Coach(name, email);
    //         coach.AddSkill("French");
    //         coach.AddSkill("C# Programming");

    //         coach.RemoveSkill("french");

    //         Assert.DoesNotContain("French", coach.Skills);
    //         Assert.Single(coach.Skills);
    //         Assert.Contains("c# programming", coach.Skills);
    //     }

    //     [Fact]
    //     public void RemoveSkills_ThrowsInvalidOperationExceptionWhenSkillNotFound()
    //     {
    //         FullName name = new FullName("Test", "Coach");
    //         EmailAddress email = EmailAddress.Create("test@example.com");
    //         Coach coach = new Coach(name, email);
    //         coach.AddSkill("C# Programming");

    //         Assert.Throws<InvalidOperationException>(() => coach.RemoveSkill("French"));
    //         Assert.Contains("c# programming", coach.Skills);
    //     }

    //     [Fact]
    //     public void ClearSkills_RemovesAllSkills()
    //     {
    //         FullName name = new FullName("Test", "Coach");
    //         EmailAddress email = EmailAddress.Create("test@example.com");
    //         Coach coach = new Coach(name, email);
    //         coach.AddSkill("C# Programming");
    //         coach.AddSkill("Dutch");

    //         coach.ClearSkills();

    //         Assert.Empty(coach.Skills);
    //     }

    //     [Fact]
    //     public void HasAllRequiredSkills_ReturnsTrueWhenOk()
    //     {
    //         FullName name = new FullName("Test", "Coach");
    //         EmailAddress email = EmailAddress.Create("test@example.com");
    //         Coach coach = new Coach(name, email);
    //         coach.AddSkill("C# Programming");
    //         coach.AddSkill("dutch");
    //         coach.AddSkill("French");

    //         List<string> requiredSkills = new List<string> { "C# Programming", "Dutch" };

    //         bool result = coach.HasAllRequiredSkills(requiredSkills);

    //         Assert.True(result);
    //     }

    //     [Fact]
    //     public void HasAllRequiredSkills_ReturnsFalseWhenOneOrMoreSkillsAreMissing()
    //     {
    //         FullName name = new FullName("Test", "Coach");
    //         EmailAddress email = EmailAddress.Create("test@example.com");
    //         Coach coach = new Coach(name, email);
    //         coach.AddSkill("C# Programming");
    //         coach.AddSkill("Dutch");

    //         List<string> requiredSkills = new List<string> { "C# Programming", "French" };

    //         bool result = coach.HasAllRequiredSkills(requiredSkills);

    //         Assert.False(result);
    //     }
    // }


    // skill tests voor course
    //   [Fact]
    //     public void AddRequiredSkill_AddsNewToList()
    //     {
    //         var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
    //         var course = Course.Create("C#", period);

    //         course.AddSkill("Testing");

    //         Assert.Contains("testing", course.Skills);
    //     }

    //     [Theory]
    //     [InlineData(null)]
    //     [InlineData("")]
    //     [InlineData("   ")]
    //     public void AddRequiredSkill_InvalidInput_ThrowsArgumentException(string invalidSkill)
    //     {
    //         var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
    //         var course = Course.Create("C#", period);

    //         Assert.Throws<ArgumentException>(() => course.AddSkill(invalidSkill));
    //     }

    //     [Fact]
    //     public void AddSkill_ThrowsInvalidOperationExceptionWhenSkillAlreadyExists()
    //     {
    //         var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
    //         var course = Course.Create("C#", period);
    //         string skill = "Dutch";
    //         course.AddSkill(skill);

    //         Assert.Throws<InvalidOperationException>(() => course.AddSkill("dutch"));
    //     }

    //     [Fact]
    //     public void RemoveRequiredSkill_RemovesExistingSkill()
    //     {
    //         var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
    //         var course = Course.Create("C#", period);
    //         course.AddSkill("Testing");

    //         course.RemoveSkill("testing");

    //         Assert.Empty(course.Skills);
}
