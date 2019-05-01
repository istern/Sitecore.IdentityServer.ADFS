namespace Sitecore.IdentityServer.ADFS
{
    public class AppSettings
    {
        public static readonly string SectionName = "Sitecore:ExternalIdentityProviders:IdentityProviders:ADFS";

        public ADFSIdentityProvider ADFSIdentityProvider { get; set; } = new ADFSIdentityProvider();
    }
}
