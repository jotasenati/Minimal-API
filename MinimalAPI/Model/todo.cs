public record Todo()
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public bool Done { get; set; }
}