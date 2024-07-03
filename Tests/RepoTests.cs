using ApplicationCore.DomainServices.Interfaces;
using ApplicationCore.Entities;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReceivedExtensions;
using NSubstitute.ReturnsExtensions;

namespace Tests
{
    public class RepoTests
    {
        private readonly IRepositoryBase<DetectionModelOutput> _sut;

        public RepoTests()
        {
            _sut = Substitute.For<IRepositoryBase<DetectionModelOutput>>();
        }

        #region Create Test Methods

        [Fact]
        public async Task Create_Should_Add_Entity_When_Valid_Entity_Is_Given()
        {
            //Arrange
            var entity = new DetectionModelOutput()!;

            //Act
            await _sut.AddAsync(entity);

            //Assert
            await _sut.Received(1).AddAsync(Arg.Any<DetectionModelOutput>());
        }


        [Fact]
        public async Task Create_Should_Return_ArgumentNullException_When_Entity_Is_Null()
        {
            //Arrange
            DetectionModelOutput? entity = null;
            _sut.AddAsync(entity).ThrowsAsync<ArgumentNullException>();

            //Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.AddAsync(entity));
            await _sut.Received(1).AddAsync(Arg.Any<DetectionModelOutput>());
        }

        #endregion

        #region Read Test Methods

        [Fact]
        public void GetById_Returns_Entity_When_Valid_Id_Is_Given()
        {
            // Arrange
            var id = 1;
            var entity = new DetectionModelOutput { Id = id };
            _sut.GetById(id).Returns(entity);

            // Act
            var result = _sut.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity!.Id, result.Id);
            _sut.Received(1).GetById(id);
        }

        [Fact]
        public void GetById_Returns_null_When_Invalid_Id_Is_Given()
        {
            // Arrange
            int id = -1;
            _sut.GetById(id).ReturnsNull();

            // Act
            var result = _sut.GetById(id);

            // Assert
            Assert.Null(result);
            _sut.Received().GetById(id);
        }

        [Fact]
        public void GetAll_Should_Return_List_Of_Entities()
        {
            // Arrange
            var entities = new List<DetectionModelOutput>();
            _sut.GetAll().Returns(entities);

            // Act
            var result = _sut.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entities.Count, result.Count);
            _sut.Received(1).GetAll();
        }

        #endregion

        #region Update Test Methods

        [Fact]
        public async Task Update_Should_Happen_When_Entity_Is_Given()
        {
            // Arrange
            var entity = TestUtilities.GetMockEntities()[0];

            var new_class = "Non-Vehicle";
            entity.Class = new_class;

            // Act
            await _sut.Update(entity);

            // Assert
            await _sut.Received(1).Update(Arg.Any<DetectionModelOutput>());
        }

        [Fact]
        public async Task Update_Should_Return_ArgumentNullException_When_Entity_Is_Null()
        {
            // Arrange
            DetectionModelOutput? entity = null;
            _sut.Update(entity).ThrowsAsync<ArgumentNullException>();

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.Update(entity));
            await _sut.Received(1).Update(entity);
        }


        [Fact]
        public async Task Update_Should_Return_ArgumentNullException_When_Entity_Does_Not_Exist()
        {
            // Arrange
            var entity = TestUtilities.GetMockEntities()[0];
            entity.Id = -1;
            _sut.Update(entity).ThrowsAsync<ArgumentNullException>();

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.Update(entity));
            await _sut.Received(1).Update(entity);
        }

        #endregion

        #region Delete Test Methods

        [Fact]
        public async Task Delete_Should_Happen_When_Entity_Is_Given()
        {
            // Arrange
            var entity = TestUtilities.GetMockEntities()[0];

            // Act
            await _sut.Delete(entity);

            // Assert
            await _sut.Received(1).Delete(entity);
        }

        [Fact]
        public async Task Delete_Should_Return_ArgumentNullException_When_Entity_Is_Null()
        {
            //Arrange
            DetectionModelOutput? entity = null;
            _sut.Delete(entity).ThrowsAsync<ArgumentNullException>();

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.Delete(entity));
            await _sut.Received(1).Delete(entity);

        }

        [Fact]
        public async Task Delete_Should_Return_ArgumentNullException_When_Entity_Does_Not_Exist()
        {
            // Arrange
            var entity = TestUtilities.GetMockEntities()[0];
            entity.Id = -1;
            _sut.Delete(entity).ThrowsAsync<ArgumentNullException>();

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.Delete(entity));

            await _sut.Received(1).Delete(entity);
        }

        #endregion
    }
}
