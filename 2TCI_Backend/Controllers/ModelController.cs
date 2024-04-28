using ApplicationCore.DomainServices.Services.Int;
using ApplicationCore.Entities;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Entities.DataTransferObjects;
using ApplicationCore.DomainServices.Interfaces;

namespace _2TCI_Backend.Controllers
{
    [ApiController]
    [Route("api/models")]
    public class ModelController : ControllerBase
    {
        private readonly IModelService<ClassificationModelInput, ModelOutputDto> _service;
        private readonly IRepositoryBase<ModelOutputDto> _repository;

        public ModelController(IModelService<ClassificationModelInput, ModelOutputDto> service, IRepositoryBase<ModelOutputDto> repository)
        {
            _service = service;
            _repository = repository;
        }

        [HttpPost("classify")]
        public async Task<IActionResult> Predict([FromBody] ModelInputDto inputForModel)
        {
            // Check if empty or invalid
            if (inputForModel == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            // Predict
            var prediction = await _service.Predict(inputForModel) ?? throw new ArgumentNullException("Prediction came with no result");
            // [Non-vehicle probability score, Vehicle Probability score]
            var predictedClass = prediction.ProbabilityScores![1] > 0.5 ? "Vehicle" : "Non-Vehicle";

            // Initialize a dto to save in database
            var result = new ModelOutputDto() { ImageBase64String = inputForModel.ImageBase64String, PredictedClass = predictedClass, ProbabilityScores = prediction.ProbabilityScores };

            await _repository.AddAsync(result);

            return StatusCode(StatusCodes.Status200OK, result);
        }
    }
}
