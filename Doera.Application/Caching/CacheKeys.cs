namespace Doera.Application.Caching;

public static class CacheKeys
{
    public static class TodoLists
    {
        public static string ForUser(Guid userId) => $"lists:u:{userId}";
    }

    public static class Tags
    {
        public static string ForUserList(Guid userId, Guid listId) => $"tags:u:{userId}:l:{listId}";
    }

}