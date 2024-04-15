
namespace ApplicationCore.Entities
{
    // Settings for one model
    public class ModelSettings
    {
        public string? ModelPath { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        public string? InputColumnName { get; set; }

        public string? OutputColumnName { get; set; }
    }
}
