using ApplicationCore.DomainServices.Services.Int;
using ApplicationCore.Entities;
using ApplicationCore.Entities.DataTransferObjects;
using ApplicationCore.Entities.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.ML;
using Microsoft.ML.Transforms.Onnx;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text.RegularExpressions;


namespace Infrastructure.Implementations
{
    public class ModelService<TInput, TOutput> : IModelService<TInput, TOutput> where TInput : class, IModelInput where TOutput : class, IModelOutput, new()
    {
        private readonly MLContext _mlContext;
        private readonly ModelSettingsBase _modelSettingsBase;

        public ModelService(IOptions<ModelSettingsBase> modelSettingsBase)
        {
            _mlContext = new MLContext();
            _modelSettingsBase = modelSettingsBase.Value ?? throw new ArgumentNullException("Error when importing ModelBaseSettings");
        }

        async Task<TOutput> IModelService<TInput, TOutput>.Predict(ModelInputDto input)
        {
            // Get the settings of the model
            var modelSettings = GetModelSettings(input.ModelType) ?? throw new ArgumentNullException("Settings are not loaded correctly");

            // Convert from base64string to an float array, so that model can take it as an input
            float[] imageData = ConvertBase64StringToFloatArray(input.ImageBase64String!, modelSettings.ImageWidth, modelSettings.ImageHeight);

            // Get prediction pipeline
            var predictionPipeline = GetPredictionPipeline(modelSettings);

            // Create input data as IDataView
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<TInput, TOutput>(predictionPipeline);

            // Extract the prediction result
            var prediction = predictionEngine.Predict(CreateModelInpput(imageData));

            // Return the predicted single value
            return prediction;
        }

        private ModelSettings GetModelSettings(ModelType type)
        {
            // For new model, add 
            return type switch
            {
                ModelType.ImageClassification => _modelSettingsBase.ImageClassification!,
                ModelType.CarDamageObjectDetection => _modelSettingsBase.CarDamageObjectDetection!,
                ModelType.Undefined => throw new ArgumentNullException($"Unsupported input type: {type}"),
            };
        }

        private OnnxTransformer GetPredictionPipeline(ModelSettings settings)
        {
            try
            {
                // Initialize pipeline
                var pipeline = _mlContext.Transforms.ApplyOnnxModel(modelFile: settings.ModelPath,
                                                        inputColumnName: settings.InputColumnName,
                                                        outputColumnName: settings.OutputColumnName);

                // Fit empty data
                var emptyDv = _mlContext.Data.LoadFromEnumerable(Array.Empty<TInput>());

                return pipeline.Fit(emptyDv);
            } catch(Exception e)
            {
                throw new Exception("Failed to load the model", e);
            }
        }

        // 
        private static TInput CreateModelInpput(float[] imageData)
        {
            var modelInput = Activator.CreateInstance<TInput>();
            modelInput.ImageData = imageData;

            return modelInput;
        }

        /// <summary>
        /// Converts an base64string to a float array, where each pixel's RGB values are normalized to [0, 1].
        /// This is done because the input of the models takes an Vector shaped float.
        /// </summary>
        /// <param name="image">The input image of type Image<Rgb24> to be converted.</param>
        /// <returns>A flattened float array representing the normalized RGB values of the image pixels.</returns>
        private static float[] ConvertBase64StringToFloatArray(string base64Image, int modelWidth, int modelHeight)
        {
            // Valid input validation
            if (string.IsNullOrEmpty(base64Image)) throw new ArgumentNullException("Base64String is not given");

            if (!IsBase64String(base64Image)) throw new FormatException("Invalid imageData");

            if (modelHeight <= 0 || modelWidth <= 0) throw new ArgumentNullException("Invalid image width or height");

            // Decode Base64 string to byte array
            byte[] imageBytes = Convert.FromBase64String(base64Image);

            // Load the image using SixLabors.ImageSharp
            using var image = Image.Load<Rgb24>(imageBytes);

            // Resize the image to size of what model input needs
            image.Mutate(x => x.Resize(new Size(modelWidth, modelHeight)));

            // Calculate the width and height of the image
            int width = image.Width;
            int height = image.Height;

            // Create a float array to store the flattened image data (RGB values)
            float[] floatArray = new float[width * height * 3];

            // Initialize the index for accessing the float array
            int index = 0;

            // Loop through each pixel of the image
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Get the RGB values of the current pixel
                    Rgb24 pixel = image[x, y];

                    // Normalize the RGB values to the range [0, 1] and store them in the float array
                    floatArray[index++] = pixel.R / 255.0f; // Red channel (normalized)
                    floatArray[index++] = pixel.G / 255.0f; // Green channel (normalized)
                    floatArray[index++] = pixel.B / 255.0f; // Blue channel (normalized)
                }
            }

            // Return the flattened float array representing the image data
            return floatArray;
        }

        public static bool IsBase64String(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return false;

            // Remove white spaces
            base64String = base64String.Trim();

            var isvalid = (base64String.Length % 4 == 0) && Regex.IsMatch(base64String, "^[-A-Za-z0-9+/=]|[^=]|={3,}$", RegexOptions.None);

            try
            {
                Convert.FromBase64String(base64String);
                return isvalid;
            } catch (FormatException)
            {
                return false;
            }
        }
    }
}
