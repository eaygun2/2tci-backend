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
        private readonly IModelService<ModelInputDto, DetectionModelOutput> _service;
        private readonly IRepositoryBase<DetectionModelOutput> _repository;


        public ModelControllerTests()
        {
            _service = Substitute.For<IModelService<ModelInputDto, DetectionModelOutput>>();
            _repository = Substitute.For<IRepositoryBase<DetectionModelOutput>>();
            _sut = new ModelController(_service, _repository);
        }

        [Fact]
        public async Task Predict_Should_Return_200OK_When_Everything_Is_Valid()
        {
            // Arrange
            var inputDto = TestUtilities.MockCarObjectDetectionModelInput();

            var predictionResult = new DetectionModelOutput
            {
                ImageBase64String = inputDto.ImageBase64String,
                Class = "Vehicle",
                Score = 0.92f,
                Box = [4.2f, 200f, 238f, 1027f]
            };

            _service.Predict(inputDto).Returns(predictionResult);

            // Act
            var response = await _sut.Predict(inputDto) as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);

            // Verify repository interaction
            // TODO: Add more objects later, f.e. OCR, Classification
            await _repository.Received(2).AddAsync(Arg.Any<DetectionModelOutput>());
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
            await _repository.DidNotReceive().AddAsync(Arg.Any<DetectionModelOutput>());
        }


        [Fact]
        public async Task Predict_Should_Return_Status_Code_200_When_Prediction_Is_Null()
        {
            // Arrange
            var inputDto = TestUtilities.MockCarObjectDetectionModelInput();
            DetectionModelOutput predictionResult = null;

            _service.Predict(inputDto)!.Returns(predictionResult);

            // Act
            var response = await _sut.Predict(inputDto) as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status200OK, response!.StatusCode);
            await _repository.DidNotReceive().AddAsync(Arg.Any<DetectionModelOutput>());
        }
    }
}
