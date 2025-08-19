namespace HorsesForCourses.Service
{
    public sealed record PageRequest(int PageNumber = 1, int PageSize = 20)
    {
        public int Page => PageNumber < 1 ? 1 : PageNumber;
        public int Size => PageSize is < 1 ? 1 : (PageSize > 500 ? 500 : PageSize); // Guardrails
    }

    public sealed record PagedResult<T>(
        IReadOnlyList<T> Items,
        int TotalCount,
        int PageNumber,
        int PageSize)
    {
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }
}