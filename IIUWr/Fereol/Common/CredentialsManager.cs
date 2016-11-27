using IIUWr.Fereol.Interface;
using System.Linq;
using Windows.Security.Credentials;

namespace IIUWr.Fereol.Common
{
    public class CredentialsManager : ISessionManager, ICredentialsManager
    {
        private const string MiddlewareResource = "IIUWr session";
        private const string SessionIdentifierResource = "IIUWr session";
        private const string CredentialResource = "IIUWr credential";

        private readonly PasswordVault Vault;

        public CredentialsManager()
        {
            Vault = new PasswordVault();
        }

        public string MiddlewareToken
        {
            get
            {
                var credential = TryGetCredentialByResource(MiddlewareResource);
                credential?.RetrievePassword();
                return credential?.Password;
            }
            set
            {
                var credential = TryGetCredentialByResource(MiddlewareResource) ?? new PasswordCredential();
                credential.Resource = MiddlewareResource;
                credential.UserName = MiddlewareResource;
                credential.Password = value;
                Vault.Add(credential);
            }
        }

        public string SessionIdentifier
        {
            get
            {
                var credential = TryGetCredentialByResource(SessionIdentifierResource);
                credential?.RetrievePassword();
                return credential?.Password;
            }
            set
            {
                var credential = TryGetCredentialByResource(SessionIdentifierResource) ?? new PasswordCredential();
                credential.Resource = SessionIdentifierResource;
                credential.UserName = SessionIdentifierResource;
                credential.Password = value;
                Vault.Add(credential);
            }
        }

        public string Username
        {
            get
            {
                var credential = TryGetCredentialByResource(CredentialResource);
                return credential?.UserName;
            }

            set
            {
                var credential = TryGetCredentialByResource(CredentialResource) ?? new PasswordCredential();
                credential.Resource = CredentialResource;
                credential.UserName = value;
                Vault.Add(credential);
            }
        }

        public string Password
        {
            get
            {
                var credential = TryGetCredentialByResource(CredentialResource);
                credential?.RetrievePassword();
                return credential?.Password;
            }

            set
            {
                var credential = TryGetCredentialByResource(CredentialResource) ?? new PasswordCredential();
                credential.Resource = CredentialResource;
                credential.Password = value;
                Vault.Add(credential);
            }
        }

        private PasswordCredential TryGetCredentialByResource(string resource)
        {
            try
            {
                return Vault.FindAllByResource(resource).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
    }
}
