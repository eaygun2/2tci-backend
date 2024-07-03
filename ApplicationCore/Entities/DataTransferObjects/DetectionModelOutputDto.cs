using ApplicationCore.Entities.Interfaces;

namespace ApplicationCore.Entities.DataTransferObjects
{
    public class DetectionModelOutputDto : EntityBase, IModelOutput
    {
        public string? ImageBase64String { get; set; }
        public float[]? Box { get; set; }
        public string? Class { get; set; }
        public float? Score { get; set; }
    }
}
