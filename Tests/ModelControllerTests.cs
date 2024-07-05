//using _2TCI_Backend.Controllers;
//using ApplicationCore.DomainServices.Services.Int;
//using ApplicationCore.Entities.DataTransferObjects;
//using ApplicationCore.Entities;
//using NSubstitute;
//using ApplicationCore.DomainServices.Interfaces;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using NSubstitute.ExceptionExtensions;
//using Microsoft.AspNetCore.Http.HttpResults;

//namespace Tests
//{
//    public class ModelControllerTests
//    {
//        private readonly ModelController _sut;
//        private readonly IModelService<ModelInputDto, DetectionModelOutputDto> _service;
//        private readonly IRepositoryBase<DetectionModelOutputDto> _repository;


//        public ModelControllerTests()
//        {
//            _service = Substitute.For<IModelService<ModelInputDto, DetectionModelOutputDto>>();
//            _repository = Substitute.For<IRepositoryBase<DetectionModelOutputDto>>();
//            _sut = new ModelController(_service, _repository);
//        }

//        [Fact]
//        public async Task Predict_Should_Return_200OK_When_Everything_Is_Valid()
//        {
//            // Arrange
//            var inputDto = TestUtilities.MockCarObjectDetectionModelInput();

//            var predictionResult = new DetectionModelOutputDto
//            {
//                ImageBase64String = inputDto.ImageBase64String,
//                Class = "Vehicle",
//                Score = 0.92f,
//                Box = [4.2f, 200f, 238f, 1027f]
//            };

//            // Act
//            var response = await _sut.Predict(inputDto) as ObjectResult;

//            // Assert
//            Assert.NotNull(response);
//            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);

//        }

//        [Fact]
//        public async Task Predict_Should_Return_BadRequest_When_Input_Is_Invalid()
//        {
//            // Arrange
//            ModelInputDto input = null;

//            // Act
//            var result = await _sut.Predict(input);

//            // Assert
//            Assert.IsType<BadRequestResult>(result);

//            await _service.DidNotReceive().Predict(Arg.Any<ModelInputDto>(), "");
//            await _repository.DidNotReceive().AddAsync(Arg.Any<DetectionModelOutputDto>());
//        }


//        [Fact]
//        public async Task Predict_Should_Return_Status_Code_200_When_Prediction_Is_Null()
//        {
//            // Arrange
//            var inputDto = TestUtilities.MockCarObjectDetectionModelInput();
//            DetectionModelOutputDto predictionResult = null;

//            _service.Predict(inputDto, "")!.Returns(predictionResult);

//            // Act
//            var response = await _sut.Predict(inputDto) as ObjectResult;

//            // Assert
//            Assert.Equal(StatusCodes.Status200OK, response!.StatusCode);
//            await _repository.DidNotReceive().AddAsync(Arg.Any<DetectionModelOutputDto>());
//        }
//    }
//}
