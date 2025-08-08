using HorsesForCourses.Core;

namespace foo;

public class Spike
{
    [Fact]
    public void DoSomething()
    {
        var s = WeekDays.Monday.ToString();
        Assert.Equal(WeekDays.Monday, Enum.Parse<WeekDays>(s));
    }
}