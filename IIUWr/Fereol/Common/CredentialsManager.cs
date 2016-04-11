using IIUWr.Fereol.Interface;
using System.Linq;
using Windows.Security.Credentials;

namespace IIUWr.Fereol.Common
{
    public class CredentialsManager : ISessionManager, ICredentialsManager
    {
        private const string SessionIdentifierResource = "IIUWr session";
        private const string SessionIdentifierUsername = "session";
        private const string CredentialResource = "IIUWr credential";

        private readonly PasswordVault Vault;

        public CredentialsManager()
        {
            Vault = new PasswordVault();
        }

        public string SessionIdentifier
        {
            get
            {
                var credential = Vault.FindAllByResource(SessionIdentifierResource).FirstOrDefault();
                credential?.RetrievePassword();
                return credential?.Password;
            }
            set
            {
                Vault.Add(new PasswordCredential(SessionIdentifierResource, SessionIdentifierUsername, value));
            }
        }

        public string Username
        {
            get
            {
                var credential = Vault.FindAllByResource(CredentialResource).FirstOrDefault();
                return credential?.UserName;
            }

            set
            {
                var credential = Vault.FindAllByResource(CredentialResource).FirstOrDefault() ?? new PasswordCredential();
                credential.Resource = CredentialResource;
                credential.UserName = value;
                Vault.Add(credential);
            }
        }

        public string Password
        {
            get
            {
                var credential = Vault.FindAllByResource(CredentialResource).FirstOrDefault();
                credential?.RetrievePassword();
                return credential?.Password;
            }

            set
            {
                var credential = Vault.FindAllByResource(CredentialResource).FirstOrDefault() ?? new PasswordCredential();
                credential.Resource = CredentialResource;
                credential.Password = value;
                Vault.Add(credential);
            }
        }
    }
}
