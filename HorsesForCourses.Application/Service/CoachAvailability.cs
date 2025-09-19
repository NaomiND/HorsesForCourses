using HorsesForCourses.Core;
using HorsesForCourses.Infrastructure;

namespace HorsesForCourses.Application
{
    public class CoachAvailability
    {
        private readonly ICourseRepository _courseRepository;

        public CoachAvailability(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }
        public virtual async Task<bool> IsCoachAvailableForCourse(Coach coach, Course newCourse)
        {
            var assignedCourses = await _courseRepository.GetCoursesByCoachIdAsync(coach.Id);

            foreach (var existingCourse in assignedCourses)
            {
                if (existingCourse.Id == newCourse.Id)
                    continue;

                bool periodOverlaps = existingCourse.Period.StartDate <= newCourse.Period.EndDate &&
                                      existingCourse.Period.EndDate >= newCourse.Period.StartDate;

                if (!periodOverlaps)
                    continue;

                foreach (var newSlot in newCourse.ScheduledTimeSlots)
                {
                    foreach (var existingSlot in existingCourse.ScheduledTimeSlots)
                    {
                        if (newSlot.OverlapsWith(existingSlot))
                            return false;
                    }
                }
            }
            return true;
        }
    }
}