using ApplicationCore.DomainServices.Services.Int;
using ApplicationCore.Entities;
using ApplicationCore.Entities.DataTransferObjects;
using Infrastructure.Implementations;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Tests
{
    public class ModelServiceTests
    {
        private readonly IModelService<ClassificationModelInput, ModelOutputDto> _sut;
        private readonly IOptions<ModelSettingsBase> _modelSettingsBase;

        public ModelServiceTests()
        {
            _modelSettingsBase = Substitute.For<IOptions<ModelSettingsBase>>();
            _modelSettingsBase.Value.Returns(TestUtilities.MockModelSettingsBase());
            _sut =  Substitute.For<ModelService<ClassificationModelInput, ModelOutputDto>>(_modelSettingsBase);
        }

        #region Convert Base64String to float[] Tests

        [Fact]
        public async Task Predict_Should_Throw_ArgumentNullException_When_Image_Width_Is_Null()
        {
            // Arrange
            var settingBase = Options.Create(new ModelSettingsBase() { ImageClassification = new ModelSettings() { ImageHeight = 1 } });

            var sut = Substitute.For<IModelService<ClassificationModelInput, ModelOutputDto>>();
            sut = new ModelService<ClassificationModelInput, ModelOutputDto>(settingBase);

            var input = TestUtilities.MockClassificationModelInput();

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.Predict(input));
        }

        [Fact]
        public async Task Predict_Should_Throw_ArgumentNullException_When_Image_Height_Is_Null()
        {
            // Arrange
            var settingBase = Options.Create(new ModelSettingsBase() { ImageClassification = new ModelSettings() { ImageWidth = 1 } });

            var sut = Substitute.For<IModelService<ClassificationModelInput, ModelOutputDto>>();
            sut = new ModelService<ClassificationModelInput, ModelOutputDto>(settingBase);

            var input = TestUtilities.MockClassificationModelInput();

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.Predict(input));
        }

        [Fact]
        public async Task Predict_Should_Throw_ArgumentNullException_When_ImageBase64String_Is_Null_Or_Empty()
        {
            // Arrange
            var input = new ModelInputDto { ModelType = ModelType.ImageClassification, ImageBase64String = "" };


            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.Predict(input));
        }

        [Fact]
        public async Task Predict_Should_Throw_FormatException_When_ImageBase64String_Is_Invalid()
        {
            // Arrange
            var input = new ModelInputDto { ModelType = ModelType.ImageClassification, ImageBase64String = "a" };

            // Act & Assert
            var result = await Assert.ThrowsAsync<FormatException>(async () => await _sut.Predict(input));
        }

        #endregion

        #region Loading Settings Tests

        [Fact]
        public async Task Predict_Should_Throw_ArgumentNullException_When_ModelType_Is_Invalid()
        {
            // Arrange
            var input = new ModelInputDto() { ImageBase64String = "" };

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.Predict(input));
        }

        [Fact]
        public async Task Predict_Should_Throw_Exception_When_Loading_Model_Goes_Wrong()
        {
            // Arrange
            var settingBase = Options.Create(new ModelSettingsBase() { ImageClassification = new ModelSettings() { ImageHeight = 1, ImageWidth = 1 } });

            var sut = Substitute.For<IModelService<ClassificationModelInput, ModelOutputDto>>();
            sut = new ModelService<ClassificationModelInput, ModelOutputDto>(settingBase);

            var input = TestUtilities.MockClassificationModelInput();

            // Act & Assert
            var result = await Assert.ThrowsAsync<Exception>(async () => await sut.Predict(input));
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task Predict_Should_Return_Prediction_When_Everything_Is_Valid()
        {
            // Arrange
            var input = TestUtilities.MockClassificationModelInput();

            // Act
            var prediction = await _sut.Predict(input);

            // Assert
            Assert.NotNull(prediction);
        }

        #endregion
    }
}