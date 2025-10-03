using Doera.Application.Interfaces.Caching;
using Doera.Application.Interfaces.Identity;

namespace Doera.Application.Caching {

    public static class CacheInvalidators {
        public static void TodoLists(ICacheService cache, Guid userId)
            => cache.Remove(CacheKeys.TodoLists.ForUser(userId));
        public static void TodoLists(ICacheService cache, ICurrentUser currentUser)
            => TodoLists(cache, currentUser.UserId!.Value);

        public static void Tags(ICacheService cache, Guid userId, Guid listId)
        => cache.Remove(CacheKeys.Tags.ForUserList(userId, listId));
        public static void Tags(ICacheService cache, ICurrentUser currentUser, Guid listId)
            => Tags(cache, currentUser.RequireUserId(), listId);
    }
    public static class CacheInvalidatorExtensions {
        public static void InvalidateTodoLists(this ICacheService cache, ICurrentUser currentUser)
            => CacheInvalidators.TodoLists(cache, currentUser);
        public static void InvalidateTagsForList(this ICacheService cache, ICurrentUser currentUser, Guid listId)
        => CacheInvalidators.Tags(cache, currentUser, listId);
    }
}