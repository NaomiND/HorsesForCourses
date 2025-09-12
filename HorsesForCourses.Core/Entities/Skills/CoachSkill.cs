namespace HorsesForCourses.Core;

public class CoachSkill
{
    public int CoachId { get; set; }
    public Coach Coach { get; set; }
    public int SkillId { get; set; }
    public Skill Skill { get; set; }
}
