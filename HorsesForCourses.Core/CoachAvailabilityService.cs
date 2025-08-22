namespace HorsesForCourses.Core
{
    public class CoachAvailabilityService
    {
        public bool IsCoachAvailableForCourse(Coach coach, Course newCourse, IEnumerable<Course> allCourses)
        {
            var assignedCourses = allCourses
                .Where(c => c.AssignedCoach?.Id == coach.Id && c.Id != newCourse.Id);

            foreach (var newSlot in newCourse.ScheduledTimeSlots)
            {
                foreach (var existingCourse in assignedCourses)
                {
                    foreach (var existingSlot in existingCourse.ScheduledTimeSlots)
                    {
                        if (newSlot.OverlapsWith(existingSlot))
                        {
                            return false;                                               // Overlap gevonden
                        }
                    }
                }
            }

            return true;                                                                // Geen overlap
        }
    }
}