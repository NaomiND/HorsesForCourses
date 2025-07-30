using HorsesForCourses.Core;

namespace HorsesForCourses.Repository;

public class InMemoryCourseRepository        //TODO
{
    private readonly Dictionary<int, Course> _courses = new();

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