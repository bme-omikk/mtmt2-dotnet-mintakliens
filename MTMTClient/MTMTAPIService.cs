using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MTMTClient
{
    class MTMTAPIService : IDisposable
    {
        private readonly string username;
        private readonly string password;
        private string csrfToken;
        public const string LOGIN = "login";
        public const string LOGOUT = "logout";
        public const string LOGIN_CSRF = "/login/csrf";
        public const string USERNAME = "username";
        public const string PASSWORD = "password";
        protected CookieContainer cookieContainer;
        protected HttpClientHandler handler;
        protected HttpClient client;
        protected readonly Uri baseUrl;        
        public MTMTAPIService(Uri url, string username, string password)
        {
            cookieContainer = new CookieContainer();
            handler = new HttpClientHandler() { CookieContainer = cookieContainer, UseCookies = true };
            client = new HttpClient(handler, false);
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            baseUrl = url;
            this.username = username;
            this.password = password;
            Task.Run(() => Login()).Wait();
        }
        public void Dispose()
        {
            Task.Run(() => Logout()).Wait();
        }        
        public async Task GetCsrfToken()
        {
            JObject csrfResponse = await CallRestMethod(HttpMethod.Get, LOGIN_CSRF);
            ParseCsrfToken(csrfResponse);
        }
        public async Task Login()
        {
            await GetCsrfToken();
            var Auth = new List<KeyValuePair<string, string>>();
            Auth.Add(new KeyValuePair<string, string>(USERNAME, username));
            Auth.Add(new KeyValuePair<string, string>(PASSWORD, password));            
            JObject LoginResponse = await CallRestMethod(HttpMethod.Post, LOGIN, new FormUrlEncodedContent(Auth));
            ParseCsrfToken(LoginResponse);
        }
        public async Task Logout()
        {
            JObject loginResponse = await CallRestMethod(HttpMethod.Post, LOGOUT);
            ParseCsrfToken(loginResponse);
        }
        public void ParseCsrfToken(JObject jObject)
        {
            csrfToken = (string)jObject["csrf"]["token"];
            client.DefaultRequestHeaders.Remove("X-CSRF-TOKEN");
            client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", csrfToken);            
        }
        public async Task<JObject> DeserializeResponseContent(HttpResponseMessage response)
        {
            try
            {
                var json = await response.Content.ReadAsStringAsync();
                JObject result = JObject.Parse(json);
                return result;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return default;
            }
        }
        public async Task<JObject> CallRestMethod(HttpMethod method, string Path, HttpContent content = null, Parameters parameters = null)
        {
            Uri uri = parameters != null ? new Uri(baseUrl, Path + parameters.ToQuery()) : new Uri(baseUrl, Path);
            var request = content != null ? new HttpRequestMessage(method, uri){ Content = content } : new HttpRequestMessage(method, uri);
            var response = await client.SendAsync(request);
            return await DeserializeResponseContent(response);
        }
        public async Task<JObject> Get(string Path, Parameters parameters = null)
        {
            return await CallRestMethod(HttpMethod.Get, Path, null, parameters);
        }
        public async Task<JObject> Post(string Path, string JsonContent, Parameters parameters = null)
        {
            return await CallRestMethod(HttpMethod.Post, Path, new StringContent(JsonContent, Encoding.UTF8, "application/vnd.mtmt2-1.0+json"), parameters);
        }
    }
}
