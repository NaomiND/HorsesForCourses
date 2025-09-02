namespace HorsesForCourses.Application.Paging
{
    public sealed record PageRequest(int PageNumber = 1, int PageSize = 20)
    {
        public int Page => PageNumber < 1 ? 1 : PageNumber;
        public int Size => PageSize is < 1 ? 1 : (PageSize > 20 ? 20 : PageSize); // Guardrails
    }
}