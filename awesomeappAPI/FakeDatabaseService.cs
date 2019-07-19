using System.Collections.Generic;

namespace awesomeappAPI
{
    public static class FakeDatabaseService
    {
        public static IEnumerable<string> Tenants = new List<string> { "8876513a-b0df-4255-ab35-50e4c3eb7b81", "5a72f0c6-eb0a-4723-aa72-780c4445c501" };

        public static Dictionary<string, string[]> Tenant1Database = new Dictionary<string, string[]>
        {
            { "testuser1", new string[] { "testuser2 has replied to your comment.", "testuser2 liked your comment." } },
            { "testuser2", new string[] { "You have 2 new unread messages.", "testuser1 sent you a friend request." } }
        };

        public static Dictionary<string, string[]> Tenant2Database = new Dictionary<string, string[]>
        {
            { "testuser3", new string[] { "testuser4 has replied to your comment.", "testuser4 liked your comment." } },
            { "testuser4", new string[] { "You have 3 new unread messages.", "testuser3 sent you a friend request." } }
        };
    }
}
