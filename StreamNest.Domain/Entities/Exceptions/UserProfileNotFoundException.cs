
using StreamNest.Domain.Entities.Exceptions;

public sealed class UserProfileNotFoundException : NotFoundException
{
    public UserProfileNotFoundException(string userId) :
    base($"The User with id: {userId} doesn't exist in the database.")
    {
    }
}