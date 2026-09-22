namespace BaseLibrary.DTOs
{
    public class CreateDomainDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DomainId { get; set; }
    }
}
