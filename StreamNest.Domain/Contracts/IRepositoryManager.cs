using StreamNest.Domain.Entities.Models;

namespace StreamNest.Domain.Contracts
{
    public interface IRepositoryManager
    {
        IUserProfileRepository User {get;}
        IVideoRepository Video {get;}
        Task SaveAsync();
    }
}