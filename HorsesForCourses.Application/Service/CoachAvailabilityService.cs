using HorsesForCourses.Infrastructure;
using HorsesForCourses.Core;

namespace HorsesForCourses.Application
{
    public class CoachAvailabilityService
    {
        private readonly ICourseRepository _courseRepository;

        public CoachAvailabilityService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<bool> IsCoachAvailableForCourseAsync(Coach coach, Course newCourse)
        {
            var assignedCourses = await _courseRepository.GetCoursesByCoachIdAsync(coach.Id);

            foreach (var newSlot in newCourse.ScheduledTimeSlots)
            {
                foreach (var existingCourse in assignedCourses.Where(c => c.Id != newCourse.Id))
                {
                    foreach (var existingSlot in existingCourse.ScheduledTimeSlots)
                    {
                        if (newSlot.OverlapsWith(existingSlot))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
