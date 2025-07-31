using HorsesForCourses.Core;
using QuickPulse.Show.TimeyWimey;

namespace HorsesForCourses.Repository;

public class InMemoryCourseRepository        //TODO
{
    private readonly Dictionary<int, Course> _courses = new Dictionary<int, Course>
    { {666,new Course("test", new PlanningPeriod(new DateOnly(2025, 12, 4), new DateOnly(2025, 12, 5)))}};

    public Course? GetById(int id)
    {
        return _courses.TryGetValue(id, out var course) ? course : null;
    }
    public IEnumerable<Course> GetAll()
    {
        return _courses.Values;
    }

    public void Save(Course course)
    {
        _courses[course.Id] = course;
    }
}