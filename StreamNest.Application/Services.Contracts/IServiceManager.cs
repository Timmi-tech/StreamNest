namespace StreamNest.Application.Services.Contracts
{

    public interface IServiceManager
    {
        IAuthenticationService AuthenticationService { get; }
        IUserProfileService UserProfileService { get; }
        IVideoService VideoService { get; }
        IVideoPostService VideoPostService { get; }
        ICommentService CommentService { get; }
        ILikeService LikeService { get; }
    }
}
