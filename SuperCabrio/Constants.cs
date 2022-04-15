using System;
namespace SuperCabrio
{
    public static class Constants
    {
        public static readonly string TenantName = "SuperCabrio";
        public static readonly string TenantId = "SuperCabrio.onmicrosoft.com";
        public static readonly string ClientId = "3ca1649f-252f-48f0-808e-54085870e19c";
        public static readonly string SignInPolicy = "B2C_1_SignInUp";
        public static readonly string IosKeychainSecurityGroups = "com.microsoft.adalcache";
        public static readonly string[] Scopes = new string[] { "openid", "offline_access" };
        public static readonly string AuthorityBase = $"https://{TenantName}.b2clogin.com/tfp/{TenantId}";
        public static readonly string AuthoritySignIn = $"{AuthorityBase}/{SignInPolicy}";
    }
}
