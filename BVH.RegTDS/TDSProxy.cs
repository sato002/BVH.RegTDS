using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BVH.RegTDS
{
    public class TDSProxy
    {
        private static string TDS_URL = "https://traodoisub.com";
        private static string COOKIE_NAME = "PHPSESSID";
        private CookieContainer _cookieContainer { get; set; }
        private HttpClient _client { get; set; }
        private HttpClientHandler _handler { get; set; }
        public TDSProxy()
        {
            SetupHttpRequest();
        }

        private void SetupHttpRequest()
        {
            _cookieContainer = new CookieContainer();
            _handler = new HttpClientHandler() { CookieContainer = _cookieContainer };
            _client = new HttpClient(_handler) { BaseAddress = new Uri(TDS_URL) };
            _client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.75 Safari/537.36");
            _client.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");
        }

        public async Task<string> GetRecaptchaSiteKey()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{TDS_URL}");
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseContent = await response.Content.ReadAsStringAsync();

            string siteKey = String.Empty;
            string pattern = "data-sitekey=\"(.*?)\"";
            foreach (Match match in Regex.Matches(responseContent, pattern))
            {
                if (match.Success && match.Groups.Count > 0)
                {
                    siteKey = match.Groups[1].Value;
                }
            }

            if (String.IsNullOrEmpty(siteKey))
            {
                throw new Exception("Lỗi không lấy được sitekey");
            }

            var captchaProxy = new AnyCaptchaProxy();
            string captcha = captchaProxy.ResolveReCaptchaV2(siteKey, TDS_URL);

            return captcha;
        }

        public async Task<string> RegAcc(AccountInfor row)
        {
            try
            {
                row.State = "Đang đợi lấy token";
                var request = new HttpRequestMessage(HttpMethod.Post, $"{TDS_URL}/scr/check_reg.php");
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(row.Username), "dkusername");
                content.Add(new StringContent(row.Password), "dkpassword");
                content.Add(new StringContent(row.Password), "rdkpassword");

                var siteKey = await GetRecaptchaSiteKey();
                content.Add(new StringContent(siteKey), "g-recaptcha-response");
                request.Content = content;
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                row.State = responseContent;
                return responseContent;
            }
            catch (Exception ex)
            {
                row.State = ex.Message;
            }

            return row.State;
        }
    }
}
