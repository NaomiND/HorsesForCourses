using System.Reflection;

namespace HorsesForCourses.Tests;

public static class Hack
{
    public static void TheId(object target, int id)
    {
        target.GetType()
            .GetProperty("Id", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(target, id);
    }
}
