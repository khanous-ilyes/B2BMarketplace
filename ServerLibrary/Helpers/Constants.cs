namespace ServerLibrary.Helpers
{
    public class Constants
    {
        public static string RoleAdmin = "Admin";
        public static string RoleSupplier = "Supplier";
        public static string RoleClient = "Client";
    }

    public class JwtSection
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
