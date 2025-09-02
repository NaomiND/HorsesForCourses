namespace HorsesForCourses.MVC;

public class ModelPagedResult // dit mag geen dto zijn maar moet een modelview zijn (lijkt op dto maar is 2 richtingen, met een readmodel moet je dit oplossen)
//anders altijd zorgen dat je notracking hebt als je rechtstreeks implementeert. 
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; }
    public int NumberOfCoursesAssignedTo { get; set; }              //nodig om aantal courses/coach te tonen

}