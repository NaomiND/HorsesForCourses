using QuickPulse;
using QuickPulse.Arteries;
using QuickPulse.Show;

namespace HorsesForCourses.WebApi;

public static class QuickPulseExtensions
{
    public static T Log<T>(this T item)
    {
        Signal.Tracing<string>().SetArtery(WriteData.ToFile("quickpulse.log")).Pulse(Introduce.This(item!));
        return item;
    }
}