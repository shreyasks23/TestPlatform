namespace TestPlatform.Application.Common.Security;

public static class AuthConstants
{
    public static class Roles
    {
        public const string Admin = "Admin";
    }

    public static class Permissions
    {
        public const string ApiWrite = "api.write";
    }

    public static class Policies
    {
        public const string CanWriteApi = "CanWriteApi";
    }
}
