using ApplicationCore.DomainServices.Services.Int;
using ApplicationCore.Entities;
using ApplicationCore.Entities.DataTransferObjects;
using Infrastructure.Implementations;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Tests
{
    public class ModelServiceTests
    {
        private IModelService<ModelInputDto, DetectionModelOutputDto> _sut;
        private readonly IOptions<ModelSettingsBase> _modelSettingsBase;

        public ModelServiceTests()
        {
            _modelSettingsBase = Substitute.For<IOptions<ModelSettingsBase>>();
            _modelSettingsBase.Value.Returns(TestUtilities.MockModelSettingsBase());

            _sut = Substitute.For<ModelService<ModelInputDto, DetectionModelOutputDto>>(_modelSettingsBase);
        }

        #region Convert Base64String to float[] Tests


        //[Fact]
        //public async Task Predict_Should_Throw_ArgumentNullException_When_ImageBase64String_Is_Null_Or_Empty()
        //{
        //    // Arrange
        //    var input = new ModelInputDto { ModelType = ModelType.CarObjectDetection, ImageBase64String = "" };
        //    _sut.When(x => x.Predict(input, "")).Do(x => { throw new ArgumentNullException(); });

        //    // Act & Assert
        //    var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.Predict(input, ""));
        //    Assert.Equal("base64String", result.ParamName);
        //}

        //[Fact]
        //public async Task Predict_Should_Throw_FormatException_When_ImageBase64String_Is_Invalid()
        //{
        //    // Arrange
        //    var input = new ModelInputDto { ModelType = ModelType.CarObjectDetection, ImageBase64String = "a" };
        //    _sut.Predict(input, "").ThrowsAsync<FormatException>();
        //    // Act & Assert
        //    // Param name not checked, since FormatException does not inherit from ArgumentException (field: ParamName)
        //    // TODO: Optional: Assert for equal error message
        //    var result = await Assert.ThrowsAsync<FormatException>(async () => await _sut.Predict(input, ""));
        //}

        #endregion

        #region Loading Settings Tests

        [Fact]
        public void Constructor_Should_ThrowArgumentNullException_When_ModelSettingsBase_IsNull()
        {
            // Arrange
            _modelSettingsBase.Value.Returns((ModelSettingsBase)null);

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new ModelService<ModelInputDto, DetectionModelOutputDto>(_modelSettingsBase));
            Assert.Equal("ModelSettingsBase", exception.ParamName);
        }

        [Fact]
        public async Task GetModelSettings_Should_Throw_ArgumentNullException_When_ModelType_Is_Null()
        {
            // Arrange
            var input = new ModelInputDto() { ImageBase64String = "" };

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.Predict(input, ""));
            Assert.Equal("ModelType", result.ParamName);
        }


        [Fact]
        public async Task GetModelSettings_Should_ThrowArgumentNullException_When_ModelType_Is_Undefined()
        {
            // Arrange
            var input = new ModelInputDto() { ImageBase64String = "" };

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.Predict(input, ""));
            Assert.Equal("ModelType", result.ParamName);
        }

        #endregion

        #region Integration Tests

        //[Fact]
        //public async Task Predict_Should_Return_Prediction_When_Everything_Is_Valid()
        //{
        //    // Arrange
        //    var input = TestUtilities.MockCarObjectDetectionModelInput();

        //    // Act
        //    var prediction = await _sut.Predict(input, "");

        //    // Assert
        //    Assert.NotNull(prediction);
        //}

        #endregion
    }
}