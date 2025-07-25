using horses_for_courses.Core;

namespace horses_for_courses.Repository;

public class InMemoryCourseRepository        //TODO
{
    private readonly Dictionary<Guid, Course> _courses = new();

    public Course? GetById(Guid id)
    {
        return _courses.TryGetValue(id, out var course) ? course : null;
    }

    public void Add(Course course)
    {
        _courses[course.Id] = course;
    }
    public void Save(Course course)
    {
        if (_courses.ContainsKey(course.Id))
        {
            _courses[course.Id] = course;
        }
        else
        {
            _courses.Add(course.Id, course);
        }
    }
}