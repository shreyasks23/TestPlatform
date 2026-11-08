using TestPlatform.Application.Common.Exceptions;

namespace TestPlatform.Application.Common.Security;

public static class OwnershipAuthorization
{
    public static void EnsureOwnerOrAdmin(Guid ownerId, Guid actorId, bool isAdmin)
    {
        if (!isAdmin && ownerId != actorId)
            throw new AuthorizationDeniedException("You do not own this resource.");
    }
}
