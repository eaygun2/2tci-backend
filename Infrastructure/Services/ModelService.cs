using ApplicationCore.DomainServices.Services.Int;
using ApplicationCore.Entities;
using ApplicationCore.Entities.DataTransferObjects;
using ApplicationCore.Entities.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text.RegularExpressions;


namespace Infrastructure.Implementations
{
    public class ModelService<TInput, TOutput> : IModelService<TInput, TOutput> where TInput : class, IModelInput where TOutput : class, IModelOutput, new()
    {
        private readonly ModelSettingsBase _modelSettingsBase;
        private ModelSettings _settings;
        private InferenceSession _session;

        public ModelService(IOptions<ModelSettingsBase> modelSettingsBase)
        {
            _modelSettingsBase = modelSettingsBase.Value ?? throw new ArgumentNullException(nameof(ModelSettingsBase), "Error when importing ModelBaseSettings");
        }

        async Task<TOutput> IModelService<TInput, TOutput>.Predict(ModelInputDto input)
        {
            _settings = GetModelSettings(input.ModelType);

            _session = new InferenceSession(_settings.ModelPath);

            var dimensions = _session.InputMetadata.First().Value.Dimensions;

            var tensor = new DenseTensor<float>(new[] { dimensions[0], dimensions[1], dimensions[2] });

            // Convert from base64string to an float array, so that model can take it as an input
            var imageData = ConvertBase64StringToImage(input.ImageBase64String!, dimensions[1], dimensions[2]);

            imageData.ProcessPixelRows(processor =>
            {
                for (int y = 0; y < processor.Height; y++)
                {
                    var rowSpan = processor.GetRowSpan(y);
                    for (int x = 0; x < processor.Width; x++)
                    {
                        tensor[0, x, y] = rowSpan[x].R;
                        tensor[1, x, y] = rowSpan[x].G;
                        tensor[2, x, y] = rowSpan[x].B;
                    }
                }
            });

            var namedValue = NamedOnnxValue.CreateFromTensor(_session.InputMetadata.First().Key, tensor);

            using var outputs = _session.Run(new List<NamedOnnxValue> { namedValue });

            return ProcessOutputs<TOutput>(outputs);
        }

        private TOutput ProcessOutputs<TOutput>(IDisposableReadOnlyCollection<DisposableNamedOnnxValue> outputs) where TOutput : IModelOutput, new()
        {
            var result = new TOutput();

            if (typeof(TOutput) == typeof(DetectionModelOutput))
            {
                var detectionResult = result as DetectionModelOutput;

                detectionResult!.Box = outputs.First(o => o.Name == _settings.OutputColumnNames![0]).AsEnumerable<float>().ToArray();
                detectionResult.Score = outputs.First(o => o.Name == _settings.OutputColumnNames![2]).AsTensor<float>().FirstOrDefault();
            }

            return result;
        }

        private ModelSettings GetModelSettings(ModelType type)
        {
            // For new model, add 
            return type switch
            {
                ModelType.CarObjectDetection => _modelSettingsBase.CarObjectDetection!,
                ModelType.LicensePlateObjectDetection => _modelSettingsBase.LicensePlateObjectDetection!,
                ModelType.Undefined => throw new ArgumentNullException(nameof(ModelType), $"Unsupported input type: {type}"),
            };
        }


        /// <summary>
        /// Converts an base64string to a float array, where each pixel's RGB values are normalized to [0, 1].
        /// This is done because the input of the models takes an Vector shaped float.
        /// </summary>
        /// <param name="image">The input image of type Image<Rgb24> to be converted.</param>
        private static Image<Rgb24> ConvertBase64StringToImage(string base64String, int modelWidth, int modelHeight)
        {
            // Valid input validation
            if (string.IsNullOrEmpty(base64String)) throw new ArgumentNullException(nameof(base64String), "Base64String is not given");

            if (!IsBase64String(base64String)) throw new FormatException("Invalid imageData");

            if (modelHeight <= 0 || modelWidth <= 0) throw new ArgumentNullException("Invalid image width or height");

            // Decode Base64 string to byte array
            byte[] imageBytes = Convert.FromBase64String(base64String);

            // Load the image using SixLabors.ImageSharp
            using var image = Image.Load<Rgb24>(imageBytes);

            // Resize the image to size of what model input needs
            return image.Clone(x => x.Resize(modelWidth, modelHeight));
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
