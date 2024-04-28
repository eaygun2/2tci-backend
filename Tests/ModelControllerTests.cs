using _2TCI_Backend.Controllers;
using ApplicationCore.DomainServices.Services.Int;
using ApplicationCore.Entities.DataTransferObjects;
using ApplicationCore.Entities;
using NSubstitute;
using ApplicationCore.DomainServices.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tests
{
    public class ModelControllerTests
    {
        private readonly ModelController _sut;
        private readonly IModelService<ClassificationModelInput, ModelOutputDto> _service;
        private readonly IRepositoryBase<ModelOutputDto> _repository;


        public ModelControllerTests()
        {
            _service = Substitute.For<IModelService<ClassificationModelInput, ModelOutputDto>>();
            _repository = Substitute.For<IRepositoryBase<ModelOutputDto>>();
            _sut = new ModelController(_service, _repository);
        }

        [Fact]
        public async Task Predict_Should_Return_200OK_When_Everything_Is_Valid()
        {
            // Arrange
            var inputDto = TestUtilities.MockClassificationModelInput();
            var predictionResult = new ModelOutputDto
            {
                ImageBase64String = inputDto.ImageBase64String,
                PredictedClass = "Non-Vehicle",
                ProbabilityScores = [0.8f, 0.2f] // Example prediction probabilities
            };

            _service.Predict(inputDto).Returns(predictionResult);

            // Act
            var response = await _sut.Predict(inputDto) as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);

            var result = response.Value as ModelOutputDto;
            Assert.NotNull(result);
            Assert.Equal(inputDto.ImageBase64String, result.ImageBase64String);
            Assert.Equal(predictionResult.PredictedClass, result.PredictedClass);
            Assert.Equal(predictionResult.ProbabilityScores, result.ProbabilityScores);

            // Verify repository interaction
            await _repository.Received(1).AddAsync(Arg.Any<ModelOutputDto>());
        }

        [Fact]
        public async Task Predict_Should_Return_BadRequest_When_Input_Is_Null()
        {
            // Arrange
            ModelInputDto input = null;

            // Act
            var result = await _sut.Predict(input);

            // Assert
            Assert.IsType<BadRequestResult>(result);

            await _service.DidNotReceive().Predict(Arg.Any<ModelInputDto>());
            await _repository.DidNotReceive().AddAsync(Arg.Any<ModelOutputDto>());
        }


        [Fact]
        public async Task Predict_Should_Return_ArgumentNullException_When_Prediction_Is_Null()
        {
            // Arrange
            var inputDto = TestUtilities.MockClassificationModelInput();
            ModelOutputDto predictionResult = null;

            _service.Predict(inputDto)!.Returns(predictionResult);

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.Predict(inputDto));
            await _repository.DidNotReceive().AddAsync(Arg.Any<ModelOutputDto>());
        }

    }
}
