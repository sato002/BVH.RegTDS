using AnyCaptchaHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVH.RegTDS
{
    public class AnyCaptchaProxy
    {
        private static string CAPTCHA_API_Key = "b2be39b39fd9437e9577452450381519";

        public string ResolveReCaptchaV2(string siteKey, string siteUrl)
        {
            var recaptchaResponse = new AnyCaptcha().RecaptchaV2Proxyless(CAPTCHA_API_Key, siteKey, siteUrl);
            if (!recaptchaResponse.IsSuccess)
            {
                throw new Exception("resolve captcha failed");
            }

            return recaptchaResponse.Result;
        }
    }
}
