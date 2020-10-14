using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.Azure.Services.AppAuthentication;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KeyVaultTest
{
    class Program
    {
        static string keyIdentifier = "https://trainingkeyvaultey.vault.azure.net/keys/KeyForJWT/d1208c831beb406bbc3b89d37f484cb8";

        static void Main(string[] args)
        {
            Console.WriteLine("Start JWT process");

            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            try
            {
                /* The next two lines of code show you how to use AppAuthentication library */
                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                // header and payload
                string header = System.Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"alg\": \"RS256\",\"typ\": \"JWT\"}"));
                string payload = System.Convert.ToBase64String(Encoding.UTF8.GetBytes("{ \"name\": \"Agus Luques\", \"admin\": true}"));

                // Sign header.payload
                var signedMessage = await keyVaultClient.SignAsync(
                        keyIdentifier,
                        JsonWebKeySignatureAlgorithm.RS256,
                        SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(header + "." + payload))
                    );

                //JWT is header.payload.sign
                Console.WriteLine("\nJWT signed:");
                Console.WriteLine(header + "." + payload + "." + System.Convert.ToBase64String(signedMessage.Result));

                JObject HeaderDecoded = JObject.Parse(Encoding.UTF8.GetString(System.Convert.FromBase64String(header)));
                
                // verify the signature
                var verified = await keyVaultClient.VerifyAsync(
                        keyIdentifier,
                        HeaderDecoded["alg"].ToString(),
                        SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(header + "." + payload)),
                        signedMessage.Result
                    );

                Console.WriteLine("\nVerified: {0}", verified);
            }
            /// <exception cref="KeyVaultErrorException">
            /// Thrown when the operation returned an invalid status code
            /// </exception>
            catch (KeyVaultErrorException keyVaultException)
            {
                throw keyVaultException;
            }
        }
    }
}
