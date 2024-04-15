using ApplicationCore.Entities.Interfaces;
using Microsoft.ML.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Entities.DataTransferObjects
{
    public class ModelOutputDto : EntityBase, IModelOutput
    {
        // TODO: For now, change the variables yourself, but this NEEDS to stay. Later, change it so that appsettings changes this.
        [ColumnName("dense_5")]
        [VectorType(1)]
        [NotMapped]
        public float[]? Prediction { get; set; }

        public string? PredictedClass { get; set; }

        public string? ImageBase64String { get; set; }
    }
}
