namespace ApplicationCore.Entities.Interfaces
{
    public interface IModelOutput
    {
        public float[]? Prediction { get; set; }

        public string? ImageBase64String { get; set; }
    }
}
