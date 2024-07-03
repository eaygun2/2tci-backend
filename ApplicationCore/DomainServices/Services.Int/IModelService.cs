using ApplicationCore.Entities.DataTransferObjects;
using ApplicationCore.Entities.Interfaces;

namespace ApplicationCore.DomainServices.Services.Int
{
    public interface IModelService<TInput, TOutput> where TInput : IModelInput
    {
        public Task<TOutput> Predict(ModelInputDto input, string associatedClass);
    }
}
