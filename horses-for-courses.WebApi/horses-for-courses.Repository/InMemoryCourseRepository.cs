using horses_for_courses.Core;

namespace horses_for_courses.Repository;

public class InMemoryCourseRepository        //TODO
{
    private readonly Dictionary<Guid, Course> _courses = new();

    public void Add(Course course)
    {
        _courses[course.Id] = course;
    }

    public Course? GetById(Guid id)
    {
        return _courses.TryGetValue(id, out var course) ? course : null;
    }

}