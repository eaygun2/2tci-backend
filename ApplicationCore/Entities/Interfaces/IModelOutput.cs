namespace ApplicationCore.Entities.Interfaces
{
    public interface IModelOutput
    {
        public float[]? ProbabilityScores { get; set; }

        public string? ImageBase64String { get; set; }
    }
}
