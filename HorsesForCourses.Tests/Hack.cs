using System.Reflection;

namespace HorsesForCourses.Tests;

// public static class Hack
// {
//     public static void TheId(object target, int id)
//     {
//         target.GetType()
//             .GetProperty("Id", BindingFlags.Instance | BindingFlags.NonPublic)!
//             .SetValue(target, id);
//     }
// }

public static class Hack
{
    public static void TheId(object target, int id)
    {
        // FIX: Zoek naar zowel publieke als niet-publieke properties.
        var propertyInfo = target.GetType()
            .GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (propertyInfo == null)
        {
            throw new ArgumentException($"Property 'Id' not found on type {target.GetType().Name}.");
        }

        // Controleer of de property een setter heeft (ook al is die private)
        if (propertyInfo.CanWrite)
        {
            propertyInfo.SetValue(target, id);
        }
        else
        {
            // Fallback voor properties met een private backing field (zoals { get; private set; })
            var backingField = target.GetType().GetField($"<{propertyInfo.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (backingField != null)
            {
                backingField.SetValue(target, id);
            }
            else
            {
                throw new InvalidOperationException($"Cannot set value for property 'Id' on type {target.GetType().Name}. No accessible setter or backing field found.");
            }
        }
    }
}