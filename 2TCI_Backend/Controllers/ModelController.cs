using ApplicationCore.DomainServices.Services.Int;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Entities.DataTransferObjects;
using ApplicationCore.DomainServices.Interfaces;
using System.Drawing;

namespace _2TCI_Backend.Controllers
{
    [ApiController]
    [Route("api/models")]
    public class ModelController : ControllerBase
    {
        private readonly IRepositoryBase<DetectionModelOutputDto> _detectionrepository;

        private readonly IModelService<ModelInputDto, DetectionModelOutputDto> _detectionService;

        public ModelController(IModelService<ModelInputDto, DetectionModelOutputDto> detectionService, IRepositoryBase<DetectionModelOutputDto> detectionrepository)
        {
            _detectionService = detectionService;
            _detectionrepository = detectionrepository;
        }

        [HttpPost("predict")]
        public async Task<IActionResult> Predict([FromBody] ModelInputDto inputForModel)
        {
            // Check if empty or invalid
            if (inputForModel == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            // Predict vehicle
            var carObjectDetectionResults = await _detectionService.Predict(inputForModel, "Vehicle");

            // Look for one present vehicle, else return no vehicle present
            if (carObjectDetectionResults == null || carObjectDetectionResults.Box?.Length <= 0)
            {
                return StatusCode(StatusCodes.Status200OK, "No Vehicle Present");
            }

            // Add to repo
            await _detectionrepository.AddAsync(carObjectDetectionResults);

            // Crop the image
            // TODO: Repair cropping method, it is not giving expected results
            var box = carObjectDetectionResults.Box;
            var changedImage = ApplyChangeToImage(inputForModel.ImageBase64String!, (int)box[0], (int)box[1], (int)box[2], (int)box[3]);

            // Create new input
            var licensePlateInput = new ModelInputDto()
            {
                ImageBase64String = changedImage,
                ModelType = ModelType.LicensePlateObjectDetection
            };

            // Predict License plate
            var licenseDetectionResults = await _detectionService.Predict(licensePlateInput, "Vehicle License Plate");

            // Add to DB if license plate is present
            if (licenseDetectionResults.Box?.Length > 0)
            {
                await _detectionrepository.AddAsync(licenseDetectionResults);
            }

            // Return results
            return StatusCode(StatusCodes.Status200OK, new Dictionary<string, object> {
                { "Vehicle Detection Results", carObjectDetectionResults },
                { "License Detection Results", licenseDetectionResults }
            });
        }

        private string ApplyChangeToImage(string base64, int xMin, int yMin, int xMax, int yMax)
        {
            // Convert Base64 to byte array
            byte[] imageBytes = Convert.FromBase64String(base64);

            // Create a MemoryStream from byte array to work with Bitmap
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                using (Bitmap originalImage = new Bitmap(ms))
                {
                    // Define the rectangle to crop
                    Rectangle cropRectangle = new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);

                    // Ensure the rectangle is within the bounds of the original image
                    cropRectangle.Intersect(new Rectangle(0, 0, originalImage.Width, originalImage.Height));

                    // Create a new bitmap for the cropped image
                    using (Bitmap croppedImage = new Bitmap(cropRectangle.Width, cropRectangle.Height))
                    {
                        // Create graphics object from the new bitmap
                        using (Graphics graphics = Graphics.FromImage(croppedImage))
                        {
                            // Draw the cropped area onto the new bitmap
                            graphics.DrawImage(originalImage,
                                               new Rectangle(0, 0, croppedImage.Width, croppedImage.Height),
                                               cropRectangle,
                                               GraphicsUnit.Pixel);
                        }

                        // Save the cropped image to a MemoryStream
                        using (MemoryStream msCropped = new MemoryStream())
                        {
                            croppedImage.Save(msCropped, System.Drawing.Imaging.ImageFormat.Png);

                            croppedImage.Save("Test.png");

                            // Convert the MemoryStream to byte array (Base64 string representation)
                            byte[] imageBytesCropped = msCropped.ToArray();
                            return Convert.ToBase64String(imageBytesCropped);
                        }
                    }
                }
            }
        }
    }
}
