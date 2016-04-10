using IIUWr.Fereol.HTMLParsing.Interface;
using IIUWr.Fereol.Interface;
using System;
using System.Linq;
using Windows.Security.Credentials;
using Windows.Web.Http;

namespace IIUWr.Fereol.HTMLParsing
{
    public class CredentialsManager : ISessionManager, ICredentialsManager
    {
        private const string SessionCookieResource = "IIUWr SessionCookie";
        private const string SessionCookieUsername = "session";
        private const string CredentialResource = "IIUWr";

        private readonly PasswordVault Vault;

        public CredentialsManager()
        {
            Vault = new PasswordVault();
        }

        public string SessionCookie
        {
            get
            {
                var credential = Vault.FindAllByResource(SessionCookieResource).FirstOrDefault();
                credential?.RetrievePassword();
                return credential?.Password;
            }
            set
            {
                Vault.Add(new PasswordCredential(SessionCookieResource, SessionCookieUsername, value));
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
