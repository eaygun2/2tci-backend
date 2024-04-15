using ApplicationCore.Entities.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities.DataTransferObjects
{
    // Each used model should have a own enum type
    public enum ModelType
    {
        Undefined = 0,
        ImageClassification = 1,
        CarDamageObjectDetection = 2,
        ObjectDetection = 3,
        OCR = 4
    }


    public class ModelInputDto
    {
        [Required]
        public string? ImageBase64String { get; set; }

        [Required]
        public ModelType ModelType { get; set; } = ModelType.Undefined;
    }
}
