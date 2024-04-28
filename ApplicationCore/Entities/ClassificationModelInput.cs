using ApplicationCore.Entities.Interfaces;
using Microsoft.ML.Data;

namespace ApplicationCore.Entities
{
    public class ClassificationModelInput : IModelInput
    {
        // TODO: For now, change the variables yourself, but this NEEDS to stay. Later, change it so that appsettings changes this.
        [ColumnName("input_1")]
        [VectorType(224 * 224 * 3)]
        public float[]? ImageData { get; set; }
    }
}
