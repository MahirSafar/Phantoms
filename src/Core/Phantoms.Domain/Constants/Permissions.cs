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

    public static class Teachers
    {
        public const string Edit = "Permissions.Teachers.Edit";
        public const string Delete = "Permissions.Teachers.Delete";
        public const string Create = "Permissions.Teachers.Create";
        public const string Share = "Permissions.Teachers.Share";
    }

    public static class Events
    {
        public const string View = "Permissions.Events.View";
        public const string Create = "Permissions.Events.Create";
        public const string Edit = "Permissions.Events.Edit";
        public const string Delete = "Permissions.Events.Delete";
        public const string Share = "Permissions.Events.Share";
        public const string Publish = "Permissions.Events.Publish";
    }

    public static class Announcements
    {
        public const string View = "Permissions.Announcements.View";
        public const string Create = "Permissions.Announcements.Create";
        public const string Edit = "Permissions.Announcements.Edit";
        public const string Delete = "Permissions.Announcements.Delete";
        public const string Share = "Permissions.Announcements.Share";
        public const string Publish = "Permissions.Announcements.Publish";
    }

    public static class Roles
    {
        public const string View   = "Permissions.Roles.View";
        public const string Manage = "Permissions.Roles.Manage";
    }
    public static class Students
    {
        public const string View = "Permissions.Students.View";
        public const string Create = "Permissions.Students.Create";
        public const string Edit = "Permissions.Students.Edit";
        public const string Delete = "Permissions.Students.Delete";
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
<<<<<<< Updated upstream
        yield return Teachers.Edit;
        yield return Teachers.Delete;
        yield return Teachers.Create;
        yield return Teachers.Share;
        yield return Events.View;
        yield return Events.Create;
        yield return Events.Edit;
        yield return Events.Delete;
        yield return Events.Share;
        yield return Events.Publish;
        yield return Announcements.View;
        yield return Announcements.Create;
        yield return Announcements.Edit;
        yield return Announcements.Delete;
        yield return Announcements.Share;
        yield return Announcements.Publish;
=======
        yield return Students.View;
        yield return Students.Create;
        yield return Students.Edit;
        yield return Students.Delete;
>>>>>>> Stashed changes
    }
}
