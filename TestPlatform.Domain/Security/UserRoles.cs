namespace TestPlatform.Domain.Security;

public static class UserRoles
{
    public const string TestTaker = "TestTaker";
    public const string TestAuthor = "TestAuthor";
    public const string Admin = "Admin";

    public static bool IsSupported(string role) => role is TestTaker or TestAuthor or Admin;
}
