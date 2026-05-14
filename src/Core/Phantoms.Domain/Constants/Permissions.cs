namespace Phantoms.Domain.Constants;

public static class Permissions
{
    public static class Products
    {
        public const string View   = "Permissions.Products.View";
        public const string Create = "Permissions.Products.Create";
        public const string Edit   = "Permissions.Products.Edit";
        public const string Delete = "Permissions.Products.Delete";
    }

    public static class Users
    {
        public const string View   = "Permissions.Users.View";
        public const string Edit   = "Permissions.Users.Edit";
        public const string Delete = "Permissions.Users.Delete";
    }

    public static class Roles
    {
        public const string View   = "Permissions.Roles.View";
        public const string Manage = "Permissions.Roles.Manage";
    }

    public static IEnumerable<string> All()
    {
        yield return Products.View;
        yield return Products.Create;
        yield return Products.Edit;
        yield return Products.Delete;
        yield return Users.View;
        yield return Users.Edit;
        yield return Users.Delete;
        yield return Roles.View;
        yield return Roles.Manage;
    }
}
