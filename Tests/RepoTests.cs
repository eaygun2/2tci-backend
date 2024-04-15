//using ApplicationCore.DomainServices.Interfaces;
//using ApplicationCore.Entities;
//using NSubstitute;

//namespace Tests
//{
//    public class RepoTests
//    {
//        private readonly IRepositoryBase<EntityBase> _sut;

//        public RepoTests()
//        {
//            _sut = Substitute.For<IRepositoryBase<EntityBase>>();
//        }

//        #region Create Test Methods

//        [Fact]
//        public async Task Create_Should_Add_To_Context_When_Valid_Entity_Is_Given()
//        {
//            Arrange
//           var entity = new EntityBase();

//            Act
//           await _sut.AddAsync(entity);

//            Assert
//           await _sut.Received(1).AddAsync(Arg.Any<EntityBase>());
//        }


//        [Fact]
//        public async Task Create_Should_Not_Return_ArgumentNullException_When_Entity_Is_Null()
//        {
//            Arrange
//           EntityBase? entity = null;

//            Act & Assert
//            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.AddAsync(entity));

//            await _sut.DidNotReceive().AddAsync(Arg.Any<EntityBase>());
//        }

//        #endregion

//        #region Read Test Methods

//        [Fact]
//        public void GetById_Returns_Entity_When_Valid_Id_Is_Given()
//        {
//            Arrange
//           var id = 1;
//            var entity = new EntityBase { Id = id };
//            _sut.GetById(id).Returns(entity);

//            Act
//           var result = _sut.GetById(1);

//            Assert
//            Assert.NotNull(result);
//            Assert.Equal(entity!.Id, result.Id);
//        }

//        [Fact]
//        public async Task GetById_Returns_null_When_Invalid_Id_Is_Given()
//        {
//            Arrange
//            int id = -1;

//            Act
//           var result = _sut.GetById(id);

//            Assert
//           await _sut.Received().GetById(id);
//        }

//        [Fact]
//        public async Task GetAll_Should_Return_List_Of_Entities()
//        {
//            Arrange
//           var entities = new List<EntityBase>();
//            _sut.GetAll().Returns(entities);

//            Act
//           var result = await _sut.GetAll();

//            Assert
//            Assert.NotNull(result);
//            Assert.Equal(entities.Count(), result.Count());
//        }

//        #endregion

//        #region Update Test Methods

//        [Fact]
//        public async Task Update_Should_Happen_When_Entity_Is_Given()
//        {
//            Arrange
//           var entity = TestUtilities.GetMockEntities()[0];

//            var new_class = "Non-Vehicle";
//            entity.PredictedClass = new_class;

//            Act
//           await _sut.Update(entity);

//            Assert
//            _mockContext.Set<ModelOutputDto>().Received(1).Update(Arg.Any<ModelOutputDto>());
//        }

//        [Fact]
//        public async Task Update_Should_Return_NullReferenceException_When_Entity_Is_Null()
//        {
//            Arrange
//           ModelOutputDto? entity = null;

//            Act & Assert
//            var result = await Assert.ThrowsAsync<NullReferenceException>(async () => await _sut.Update(entity));
//            _mockContext.Set<ModelOutputDto>().DidNotReceive().Update(Arg.Any<ModelOutputDto>());
//            await _mockContext.DidNotReceive().SaveChangesAsync();
//        }


//        [Fact]
//        public async Task Update_Should_Return_NullReferenceException_When_Entity_Does_Not_Exist()
//        {
//            Arrange
//           var entity = TestUtilities.GetMockEntities()[0];
//            entity.Id = -1;
//            entity.PredictedClass = "Non-Vehicle";

//            Act & Assert
//            var result = await Assert.ThrowsAsync<NullReferenceException>(async () => await _sut.Update(entity));

//            _mockContext.Set<ModelOutputDto>().DidNotReceive().Update(Arg.Any<ModelOutputDto>());
//            await _mockContext.DidNotReceive().SaveChangesAsync();
//        }

//        #endregion

//        #region Delete Test Methods

//        [Fact]
//        public async Task Delete_Should_Happen_When_Entity_Is_Given()
//        {
//            Arrange
//           var entity = TestUtilities.GetMockEntities()[0];

//            Act
//           await _sut.Delete(entity);

//            Assert
//            _mockContext.Set<ModelOutputDto>().Received(1).Remove(entity);
//            await _mockContext.Received(1).SaveChangesAsync();
//        }

//        [Fact]
//        public async Task Delete_Should_Return_NullReferenceException_When_Entity_Is_Null()
//        {
//            Arrange
//           ModelOutputDto? entity = null;


//            Act & Assert
//            var result = await Assert.ThrowsAsync<NullReferenceException>(async () => await _sut.Delete(entity));
//            _mockContext.Set<ModelOutputDto>().DidNotReceive().Remove(Arg.Any<ModelOutputDto>());
//            await _mockContext.DidNotReceive().SaveChangesAsync();
//        }

//        [Fact]
//        public async Task Delete_Should_Return_NullReferenceException_When_Entity_Does_Not_Exist()
//        {
//            Arrange
//           var entity = TestUtilities.GetMockEntities()[0];
//            entity.Id = -1;

//            Act & Assert
//            var result = await Assert.ThrowsAsync<NullReferenceException>(async () => await _sut.Delete(entity));

//            _mockContext.Set<ModelOutputDto>().DidNotReceive().Remove(Arg.Any<ModelOutputDto>());
//            await _mockContext.DidNotReceive().SaveChangesAsync();
//        }

//        #endregion
//    }
//}
