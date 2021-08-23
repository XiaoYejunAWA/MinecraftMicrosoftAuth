using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Runtime.InteropServices;

namespace microsoft_launcher
{
    public class MicrosoftAPIs
    {
        private const int INTERNET_OPTION_END_BROWSER_SESSION = 42;
        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
        public class XSTSTokens
        {
            public string xstsToken;
            public string uhs;

        }
        [System.Runtime.InteropServices.DllImport("wininet.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetOption(int hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public string cutUri = ("https://login.live.com/oauth20_desktop.srf?code=");
        /// <summary>
        /// 微软登录网址
        /// </summary>
        public Uri loginWebsite = new Uri(@"https://login.live.com/oauth20_authorize.srf?client_id=00000000402b5328&response_type=code&scope=service%3A%3Auser.auth.xboxlive.com%3A%3AMBI_SSL&redirect_uri=https%3A%2F%2Flogin.live.com%2Foauth20_desktop.srf");
        //public Uri loginWebsite = new Uri("https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize?prompt=login&client_id=00000000402b5328&response_type=code&scope=service%3A%3Auser.auth.xboxlive.com%3A%3AMBI_SSL&redirect_uri=https:%2F%2Flogin.live.com%2Foauth20_desktop.srf");
        //https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize?prompt=login&client_id=00000000402b5328&response_type=code&scope=service%3A%3Auser.auth.xboxlive.com%3A%3AMBI_SSL&redirect_uri=https:%2F%2Flogin.live.com%2Foauth20_desktop.srf
        /// <summary>
        /// 微软登录后传回令牌、刷新令牌
        /// </summary>
        public class TokensPostBack
        {
            public class Root
            {
                /// <summary>
                /// 
                /// </summary>
                public string token_type { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public int expires_in { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string scope { get; set; }
                /// <summary>
                /// 令牌
                /// </summary>
                public string access_token { get; set; }
                /// <summary>
                /// 刷新令牌
                /// </summary>
                public string refresh_token { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string user_id { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string foci { get; set; }
            }
        }
        /// <summary>
        /// 两个令牌
        /// </summary>
        public class Tokens
        {
            /// <summary>
            /// 令牌
            /// </summary>
            public string access_token;
            /// <summary>
            /// 刷新令牌
            /// </summary>
            public string refresh_token;
        }
        /// <summary>
        /// 获取XBL Token用的json
        /// </summary>
        public class SendXBLToken
        {
            public class Properties
            {
                /// <summary>
                /// 
                /// </summary>
                public string AuthMethod { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string SiteName { get; set; }
                /// <summary>
                /// access_token
                /// </summary>
                public string RpsTicket { get; set; }
            }

            public class Root
            {
                /// <summary>
                /// 
                /// </summary>
                public Properties Properties { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string RelyingParty { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string TokenType { get; set; }
            }
        }
        public class Properties
        {
            /// <summary>
            /// 
            /// </summary>
            public string AuthMethod { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string SiteName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string RpsTicket { get; set; }
        }
        /// <summary>
        /// 发送以获取XBL Token
        /// </summary>
        public class XBLToken
        {
            /// <summary>
            /// 
            /// </summary>
            public Properties Properties { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string RelyingParty { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string TokenType { get; set; }
        }
        public class PropertiesXSTS
        {
            /// <summary>
            /// 
            /// </summary>
            public string SandboxId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> UserTokens { get; set; }
        }
        public class XSTS
        {
            /// <summary>
            /// 
            /// </summary>
            public PropertiesXSTS Properties { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string RelyingParty { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string TokenType { get; set; }
        }
        /// <summary>
        /// 清空web缓存,用于重新登录
        /// </summary>
        public unsafe void SuppressWininetBehavior()
        {

            int option = (int)3/* INTERNET_SUPPRESS_COOKIE_PERSIST*/;
            int* optionPtr = &option;

            bool success = InternetSetOption(0, 81/*INTERNET_OPTION_SUPPRESS_BEHAVIOR*/, new IntPtr(optionPtr), sizeof(int));
            if (!success)
            {

            }
        }
        /// <summary>
        /// 获取令牌
        /// </summary>
        /// <param name="code">授权代码</param>
        /// <param name="isDebuging">是否为测试</param>
        /// <returns></returns>
        public async Task<Tokens> GetAccessTokenAsync(string code, bool isDebuging)
        {
            try
            {
                Tokens tokens = new Tokens();
                TokensPostBack tokensPostBack = new TokensPostBack();
                IEnumerable<KeyValuePair<string, string>> queries = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("client_id","00000000402b5328"),
                    new KeyValuePair<string, string>("code",code),
                    new KeyValuePair<string, string>("grant_type","authorization_code"),
                    new KeyValuePair<string, string>("redirect_uri","https://login.live.com/oauth20_desktop.srf"),
                    new KeyValuePair<string, string>("scope","service::user.auth.xboxlive.com::MBI_SSL"),
                    
                    //new KeyValuePair<string, string>("Content_Type","application/x-www-form-urlencoded"),
                };
                //HttpContent httpContent = new FormUrlEncodedContent(queries);
                // StringContent stringContent = new StringContent(httpContent.ReadAsStringAsync().Result, Encoding.UTF8, "application/x-www-form-urlencoded");
                StringContent stringContent = new StringContent(
    "client_id=00000000402b5328" +
    "&code=" + code +
    "&grant_type=authorization_code" +
    "&redirect_uri=https://login.live.com/oauth20_desktop.srf" +
    "&scope=service::user.auth.xboxlive.com::MBI_SSL",
    Encoding.UTF8,
    "application/x-www-form-urlencoded");

                if (isDebuging)
                    MessageBox.Show("创建发送信息" + "client_id=00000000402b5328" + "&code=" + code + "&grant_type=authorization_code" + "&redirect_uri=https%3A%2F%2Flogin.live.com%2Foauth20_desktop.srf" + "&scope=service%3A%3Auser.auth.xboxlive.com%3A%3AMBI_SSL");
                using (HttpClient httpClient = new HttpClient())
                {
                    if (isDebuging)
                        MessageBox.Show("创建发送客户端");
                    //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
                    using (HttpResponseMessage res = await httpClient.PostAsync("https://login.live.com/oauth20_token.srf", stringContent))
                    {
                        if (isDebuging)
                            MessageBox.Show("获取服务端回复");
                        //if (isDebuging)
                        //MessageBox.Show(stringContent.ReadAsStringAsync().Result);
                        using (HttpContent content = res.Content)
                        {
                            if (isDebuging)
                                MessageBox.Show("解析信息");
                            string myContent = await content.ReadAsStringAsync();
                            //调试用
                            if (isDebuging)
                                MessageBox.Show(myContent);
                            JObject obj = JObject.Parse(myContent);
                            //tokensPostBack = JsonConvert.DeserializeObject<TokensPostBack>(myContent);
                            //tokens.access_token = tokensPostBack.access_token;
                            //tokens.refresh_token = tokensPostBack.refresh_token;
                            try
                            {
                                tokens.access_token = obj["access_token"].ToString();
                                tokens.refresh_token = obj["refresh_token"].ToString();
                            }
                            catch
                            {
                                return null;
                            }
                            return tokens;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("获取令牌失败，请检查网络是否连接，或尝试使用代理.错误信息：" + e);
                return null;
            }
        }
        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <param name="refresh_token">之前的刷新令牌</param>
        /// <param name="isDebuging">是否为测试</param>
        /// <returns></returns>
        public async Task<Tokens> RefreshTokenAsync(string refresh_token, bool isDebuging)
        {
            Tokens tokens = new Tokens();
            TokensPostBack tokensPostBack = new TokensPostBack();
            IEnumerable<KeyValuePair<string, string>> queries = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("client_id","00000000402b5328"),
                new KeyValuePair<string, string>("refresh_token",refresh_token),
                new KeyValuePair<string, string>("grant_type","refresh_token"),
                new KeyValuePair<string, string>("redirect_uri","https://login.live.com/oauth20_desktop.srf"),
                new KeyValuePair<string, string>("scope","service::user.auth.xboxlive.com::MBI_SSL")
            };
            StringContent stringContent = new StringContent("client_id=00000000402b5328" +
    "&refresh_token=" + refresh_token +
    "&grant_type=refresh_token" +
    "&redirect_uri=https://login.live.com/oauth20_desktop.srf" +
    "&scope=service::user.auth.xboxlive.com::MBI_SSL",
    Encoding.UTF8,
    "application/x-www-form-urlencoded");
            //HttpContent httpContent = new FormUrlEncodedContent(queries);
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
                    using (HttpResponseMessage res = await httpClient.PostAsync("https://login.live.com/oauth20_token.srf", stringContent))
                    {

                        using (HttpContent content = res.Content)
                        {
                            string myContent = await content.ReadAsStringAsync();
                            //调试用
                            if (isDebuging)
                                MessageBox.Show(myContent);
                            tokensPostBack = JsonConvert.DeserializeObject<TokensPostBack>(myContent);
                            JObject obj = Newtonsoft.Json.Linq.JObject.Parse(myContent);
                            tokens.access_token = obj["access_token"].ToString();
                            tokens.refresh_token = obj["refresh_token"].ToString();
                            return tokens;
                        }
                    }
                }
                //tokens.access_token = tokensPostBack.access_token;
                //tokens.refresh_token = tokensPostBack.refresh_token;
                //return tokens;
            }
            catch (Exception e)
            {
                MessageBox.Show("刷新令牌失败，请检查网络是否连接，或尝试使用代理,详细信息：" + e);
                return null;
            }
        }
        /// <summary>
        /// 获取XBL令牌
        /// </summary>
        /// <param name="access_token">登陆令牌</param>
        /// <param name="isDebuging">是否为调试</param>
        /// <returns></returns>
        public async Task<string> GetXBoxLiveToken(string access_token, bool isDebuging)
        {
            XBLToken xBLToken = new XBLToken();
            Properties properties = new Properties();
            properties.AuthMethod = "RPS";
            properties.SiteName = "user.auth.xboxlive.com";
            properties.RpsTicket = access_token;
            xBLToken.Properties = properties;
            xBLToken.RelyingParty = "http://auth.xboxlive.com";
            xBLToken.TokenType = "JWT";


            string json = "{" +
                                "\"Properties\":{" +
                                                    "\"AuthMethod\":\"RPS\"," +
                                                    "\"SiteName\":\"user.auth.xboxlive.com\"," +
                                                    "\"RpsTicket\":" + access_token +
                                "}," +
                                "\"RelyingParty\":\"http://auth.xboxlive.com\"," +
                                "\"TokenType\":\"JWT\"" +
                            "}";
            json = JsonConvert.SerializeObject(xBLToken);
            //HttpContent httpContent = new StringContent(json);
            StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            //stringContent.Headers.Add("Accept", "application/json");
            try
            {
                if (isDebuging)
                    MessageBox.Show(json);
                using (HttpClient httpClient = new HttpClient())
                {

                    //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    using (HttpResponseMessage res = await httpClient.PostAsync("https://user.auth.xboxlive.com/user/authenticate", stringContent))
                    {
                        if (isDebuging)
                            MessageBox.Show(res.IsSuccessStatusCode.ToString());

                        using (HttpContent content = res.Content)
                        {

                            string myContent = await content.ReadAsStringAsync();
                            //调试用
                            if (isDebuging)
                                MessageBox.Show(myContent);
                            try
                            {
                                JObject obj = Newtonsoft.Json.Linq.JObject.Parse(myContent);
                                if (myContent.Contains("Token"))
                                {
                                    return obj["Token"].ToString();
                                }
                                else
                                {
                                    return null;
                                }

                            }
                            catch (Exception e)
                            {
                                MessageBox.Show("获取xbl令牌失败，详细信息：" + e);
                                return null;
                            }

                        }
                    }

                }
                //tokens.access_token = tokensPostBack.access_token;
                //tokens.refresh_token = tokensPostBack.refresh_token;
                //return tokens;
            }
            catch
            {
                MessageBox.Show("刷新令牌失败，请检查网络是否连接，或尝试使用代理");
                return null;
            }
        }
        /// <summary>
        /// 获取XSTS令牌
        /// </summary>
        /// <param name="XBLtoken">XBL令牌</param>
        /// <param name="isDebuging">是否调试</param>
        /// <returns></returns>
        public async Task<XSTSTokens> GetXSTSToken(string XBLtoken, bool isDebuging)
        {
            XSTSTokens xSTSTokens = new XSTSTokens();
            XSTS xSTS = new XSTS();
            PropertiesXSTS properties = new PropertiesXSTS();
            properties.SandboxId = "RETAIL";
            List<string> vs = new List<string>();
            vs.Add(XBLtoken);
            properties.UserTokens = vs;
            xSTS.Properties = properties;
            xSTS.RelyingParty = "rp://api.minecraftservices.com/";
            xSTS.TokenType = "JWT";
            string json = "{" +
                                "\"Properties\":{" +
                                                    "\"SandboxId\":\"RETAIL\"," +
                                                    "\"UserTokens\": [" +
                                                    XBLtoken +
                                                    "]" +
                                "}," +
                                "\"RelyingParty\":\"rp://api.minecraftservices.com/\"," +
                                "\"TokenType\":\"JWT\"" +
                            "}";
            json = JsonConvert.SerializeObject(xSTS);
            //HttpContent httpContent = new StringContent(json);
            StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            //stringContent.Headers.Add("Accept", "application/json");
            try
            {
                if (isDebuging)
                    MessageBox.Show(json);
                using (HttpClient httpClient = new HttpClient())
                {

                    //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    using (HttpResponseMessage res = await httpClient.PostAsync("https://xsts.auth.xboxlive.com/xsts/authorize", stringContent))
                    {
                        if (isDebuging)
                            MessageBox.Show(res.IsSuccessStatusCode.ToString());

                        using (HttpContent content = res.Content)
                        {

                            string myContent = await content.ReadAsStringAsync();
                            //调试用
                            if (isDebuging)
                                MessageBox.Show(myContent);
                            try
                            {
                                JObject obj = Newtonsoft.Json.Linq.JObject.Parse(myContent);
                                if (myContent.Contains("Token"))
                                {

                                    xSTSTokens.xstsToken = obj["Token"].ToString();
                                    xSTSTokens.uhs = obj["DisplayClaims"]["xui"][0]["uhs"].ToString();
                                    return xSTSTokens;
                                }
                                else
                                {
                                    return null;
                                }

                            }
                            catch (Exception e)
                            {
                                MessageBox.Show("获取xbl令牌失败，详细信息：" + e);
                                return null;
                            }

                        }
                    }

                }
                //tokens.access_token = tokensPostBack.access_token;
                //tokens.refresh_token = tokensPostBack.refresh_token;
                //return tokens;
            }
            catch
            {
                MessageBox.Show("刷新令牌失败，请检查网络是否连接，或尝试使用代理");
                return null;
            }
        }
        /// <summary>
        /// 获取我的世界令牌
        /// </summary>
        /// <param name="xSTSTokens">XSTS令牌</param>
        /// <param name="isDebuging">是否为调试</param>
        /// <returns></returns>
        public async Task<string> GetMinecraftToken(XSTSTokens xSTSTokens, bool isDebuging)
        {

            string json = "{\"identityToken\":\"XBL3.0 x=" + xSTSTokens.uhs + ";" + xSTSTokens.xstsToken + "\"}";
            json.Replace(" ", string.Empty);
            json.Replace("\n", string.Empty);
            //HttpContent httpContent = new StringContent(json);
            StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            //stringContent.Headers.Add("Accept", "application/json");
            try
            {
                if (isDebuging)
                    MessageBox.Show(json);
                using (HttpClient httpClient = new HttpClient())
                {

                    //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    using (HttpResponseMessage res = await httpClient.PostAsync("https://user.auth.xboxlive.com/user/authenticate", stringContent))
                    {
                        if (isDebuging)
                            MessageBox.Show(res.IsSuccessStatusCode.ToString());

                        using (HttpContent content = res.Content)
                        {

                            string myContent = await content.ReadAsStringAsync();
                            //调试用
                            if (isDebuging)
                                MessageBox.Show(myContent);
                            try
                            {
                                JObject obj = Newtonsoft.Json.Linq.JObject.Parse(myContent);
                                if (myContent.Contains("access_token"))
                                {
                                    return obj["access_token"].ToString();
                                }
                                else
                                {
                                    return null;
                                }

                            }
                            catch (Exception e)
                            {
                                MessageBox.Show("获取xbl令牌失败，详细信息：" + e);
                                return null;
                            }

                        }
                    }

                }
                //tokens.access_token = tokensPostBack.access_token;
                //tokens.refresh_token = tokensPostBack.refresh_token;
                //return tokens;
            }
            catch
            {
                MessageBox.Show("刷新令牌失败，请检查网络是否连接，或尝试使用代理");
                return null;
            }
        }
        /// <summary>
        /// 确认是否购买过mc
        /// </summary>
        /// <param name="mcToken">我的世界令牌</param>
        /// <returns></returns>
        public async Task<bool> GetIfYouHaveMinecraft(string mcToken)
        {
            try
            {


                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + mcToken);
                    using (HttpResponseMessage res = await httpClient.GetAsync("https://api.minecraftservices.com/entitlements/mcstore"))
                    {
                        using (HttpContent content = res.Content)
                        {
                            string myContent = await content.ReadAsStringAsync();
                            return (myContent != string.Empty);
                        }
                    }
                }
            }
            catch
            {
                return false;
            }



        }
        /// <summary>
        /// 获取登录的UUID
        /// </summary>
        /// <param name="mcToken">我的世界令牌</param>
        /// <returns></returns>
        public async Task<string> GetMinecraftUUID(string mcToken)
        {
            try
            {


                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + mcToken);
                    using (HttpResponseMessage res = await httpClient.GetAsync("https://api.minecraftservices.com/minecraft/profile"))
                    {
                        using (HttpContent content = res.Content)
                        {

                            string myContent = await content.ReadAsStringAsync();
                            JObject obj = Newtonsoft.Json.Linq.JObject.Parse(myContent);
                            if (myContent.Contains("id"))
                            {
                                return obj["id"].ToString();
                            }
                            else
                            {
                                return null;
                            }

                        }
                    }
                }
            }
            catch
            {
                return null;
            }



        }

    }
}
