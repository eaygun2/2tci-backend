namespace ApplicationCore.Entities
{
    // Settings for all models, Modeltype enum in dto should choose appropriate setting
    public class ModelSettingsBase
    {
        public ModelSettings? ImageClassification { get; set; }
        public ModelSettings? CarObjectDetection { get; set; }

        public ModelSettings? LicensePlateObjectDetection { get; set; }
    }
}
