namespace Client.Infrastructure.Api
{
    public static class ApiRoutes
    {
        public static class Contacts
        {
            public const string ContactList = "/api/contact";
            public static string ContactById(Guid id) => $"/api/contact/{id}";
            public const string CreateContact = "/api/contacts";
            public static string DeleteContact(Guid id) => $"/api/contact/{id}";

            public const string AddInfo = "/api/contactinfo";
            public static string DeleteInfo(Guid infoId) => $"/api/contactinfo/{infoId}";
        }

        public static class Reports
        {
            public const string List = "/api/reports";
            public static string ById(Guid id) => $"/api/reports/{id}";
            public const string Create = "/api/reports";
        }
    }
}
