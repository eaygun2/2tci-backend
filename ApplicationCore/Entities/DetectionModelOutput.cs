using ApplicationCore.Entities.Interfaces;

namespace ApplicationCore.Entities
{
    public class DetectionModelOutput : EntityBase, IModelOutput
    {
        public string? ImageBase64String { get; set; }
        public float[]? Box { get; set; }
        public string? Class { get; set; }
        public float? Score { get; set; }
    }
}
