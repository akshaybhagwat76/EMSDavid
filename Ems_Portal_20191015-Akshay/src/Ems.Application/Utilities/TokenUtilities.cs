using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xero.NetStandard.OAuth2.Token;
using System.Text.Json;

namespace Ems.Utilities
{
    public static class TokenUtilities
    {
        public static void StoreToken(XeroOAuth2Token xeroToken)
        {
            string serializedXeroToken = JsonSerializer.Serialize(xeroToken);

            System.IO.File.WriteAllText("./xerotoken.txt", serializedXeroToken);
        }

        public static XeroOAuth2Token GetStoredToken()
        {
            string serializedXeroToken = System.IO.File.ReadAllText("./xerotoken.txt");
            var xeroToken = JsonSerializer.Deserialize<XeroOAuth2Token>(serializedXeroToken);

            return xeroToken;
        }

        public static bool TokenExists()
        {
            string serializedXeroTokenPath = "./xerotoken.txt";
            bool fileExist = File.Exists(serializedXeroTokenPath);

            return fileExist;
        }
    }
}
