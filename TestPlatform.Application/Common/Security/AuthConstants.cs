using TestPlatform.Domain.Security;

namespace TestPlatform.Application.Common.Security;

public static class AuthConstants
{
    public static class Claims
    {
        public const string SessionId = "sid";
        public const string TokenVersion = "token_version";
    }

    public static class Roles
    {
        public const string TestTaker = UserRoles.TestTaker;
        public const string TestAuthor = UserRoles.TestAuthor;
        public const string Admin = UserRoles.Admin;

        public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
        {
            TestTaker,
            TestAuthor,
            Admin
        };
    }

    public static class Permissions
    {
        public const string ManageUsers = "users.manage";
        public const string ReadQuestionBank = "questions.read";
        public const string WriteContent = "content.write";
        public const string ReadTests = "tests.read";
        public const string SubmitTests = "tests.submit";
    }

    public static class Policies
    {
        public const string CanManageUsers = "CanManageUsers";
        public const string CanReadQuestionBank = "CanReadQuestionBank";
        public const string CanWriteContent = "CanWriteContent";
        public const string CanReadTests = "CanReadTests";
        public const string CanSubmitTests = "CanSubmitTests";
    }

    public static IReadOnlyCollection<string> GetPermissions(string role) => role switch
    {
        Roles.TestTaker => [Permissions.ReadTests, Permissions.SubmitTests],
        Roles.TestAuthor => [Permissions.ReadQuestionBank, Permissions.WriteContent, Permissions.ReadTests, Permissions.SubmitTests],
        Roles.Admin => [Permissions.ManageUsers, Permissions.ReadQuestionBank, Permissions.WriteContent, Permissions.ReadTests, Permissions.SubmitTests],
        _ => []
    };
}
