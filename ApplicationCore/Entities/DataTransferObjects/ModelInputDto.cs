using ApplicationCore.Entities.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities.DataTransferObjects
{
    // Each used model should have a own enum type
    public enum ModelType
    {
        Undefined = 0,
        CarObjectDetection = 1,
        LicensePlateObjectDetection = 2,
    }

    public class ModelInputDto : IModelInput
    {
        [Required]
        public string? ImageBase64String { get; set; }

        [Required]
        public ModelType ModelType { get; set; } = ModelType.Undefined;
    }
}
