using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
namespace ServiceEngine
{
    public class Credentials
    {
        public string Key { get; set; }
        public string MerchantId { get; set; }
    }
    public class Configuration
    {
       
        public static int _joloAPIMode = Convert.ToInt32(ConfigurationManager.AppSettings["JoloMode"]);

        public static Dictionary<ProviderType, Credentials> CredentialDict { get; set; }

        static Configuration()
        {
            #region CredentialDict
            CredentialDict = new Dictionary<ProviderType, Credentials>();
            CredentialDict.Add(ServiceEngine.ProviderType.JOLO, new Credentials { Key = "274156900496463", MerchantId = "innovative" });
            #endregion
        }
    }
}
