using Microsoft.EntityFrameworkCore;
using StreamNest.Domain.Contracts;
using StreamNest.Domain.Entities.Models;

namespace StreamNest.Infrastructure.Repository
{
    public class VideoRepository : RepositoryBase<Video>, IVideoRepository
    {
        public VideoRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public void CreateVideo(Video video) => Create(video);

        public async Task<IEnumerable<Video>> GetAllVideosAsync(bool trackChanges, int pageNumber = 1, int pageSize = 20) => await FindAll(trackChanges)
        .Include(v => v.Creator)
        .Include(v => v.VideoTags)
            .ThenInclude(vt => vt.Tag)
        .OrderByDescending(v => v.UploadDate)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

        public async Task<Video?> GetVideoByIdAsync(Guid videoPostId, bool trackChanges) =>
            await FindByCondition(v => v.Id == videoPostId, trackChanges)
            .Include(v => v.Creator)
            .Include(v => v.VideoTags)
                .ThenInclude(vt => vt.Tag)
            .FirstOrDefaultAsync();

        public async Task<IEnumerable<Video>> GetVideosByUserIdAsync(string creatorId, bool trackChanges) =>
            await FindByCondition(v => v.CreatorId == creatorId, trackChanges)
            .Include(v => v.Creator)
            .Include(v => v.VideoTags)
                .ThenInclude(vt => vt.Tag)
            .OrderByDescending(v => v.UploadDate)
            .ToListAsync();

        public void DeletePost(Video video) => Delete(video);
    }
}