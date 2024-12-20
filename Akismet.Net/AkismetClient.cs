using Akismet.Net.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

namespace Akismet.Net
{
    /// <summary>
    /// Main client
    /// </summary>
    public class AkismetClient
    {
        private readonly string blogUrl;
        private readonly string apiKey;
        private readonly string[] allowedIntervals = new[] { "60-days", "6-months", "all" };

        private readonly HttpClient client;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="blogUrl"></param>
        /// <param name="applicationName"></param>
        public AkismetClient(string apiKey, Uri blogUrl, string applicationName)
        {
            this.apiKey = apiKey;
            this.blogUrl = blogUrl?.ToString() ?? throw new ArgumentNullException("blogUrl");

            client = new HttpClient()
            {
                BaseAddress = new Uri($"https://{apiKey}.rest.akismet.com/1.1"),
            };
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue($"{applicationName} | Akismet.NET/{Assembly.GetExecutingAssembly().GetName().Version} (https://github.com/ahwm/Akismet.Net)"));
        }

        /// <summary>
        /// Verify key
        /// </summary>
        /// <returns></returns>
        public async Task<bool> VerifyKeyAsync()
        {
            var formData = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("key", apiKey),
                new KeyValuePair<string, string>("blog", blogUrl)
            });
            var resp = await client.PostAsync("verify-key", formData);

            return await resp.Content.ReadAsStringAsync() == "valid";
        }

        /// <summary>
        /// Verify key
        /// </summary>
        /// <returns></returns>
        public bool VerifyKey()
        {
            var req = new RestRequest("verify-key", Method.POST)
                .AddParameter("key", apiKey)
                .AddParameter("blog", blogUrl);
            var resp = client.Execute(req);

            return resp.Content == "valid";
        }

        /// <summary>
        /// Check comment
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<AkismetResponse> CheckAsync(AkismetComment comment)
        {
            var req = new RestRequest("comment-check", Method.POST)
                .AddParameter("blog", blogUrl);

            var attributes = AttributeHelper.GetAttributes(comment);
            foreach (var kv in attributes)
                req.AddParameter(kv.Key, kv.Value);

            var resp = await client.ExecuteAsync(req);

            AkismetResponse response = new AkismetResponse();

            if (!Boolean.TryParse(resp.Content, out bool result))
            {
                response.AkismetErrors.Add(resp.Content);
                response.SpamStatus = SpamStatus.Unspecified;
            }
            else
                response.SpamStatus = result ? SpamStatus.Spam : SpamStatus.Ham;

            if (resp.Headers.Any(r => r.Name.ToLower() == "x-akismet-debug-help"))
                response.AkismetDebugHelp = resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-debug-help").First().Value.ToString();
            if (resp.Headers.Any(r => r.Name.ToLower() == "x-akismet-pro-tip"))
                response.ProTip = resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-pro-tip").First().Value.ToString();
            if (resp.Headers.Any(r => r.Name.ToLower() == "x-akismet-alert-code"))
                response.AkismetErrors.Add(resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-alert-code").First().Value.ToString() + ": " + resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-alert-msg").First().Value.ToString());

            return response;
        }

        /// <summary>
        /// Check comment
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public AkismetResponse Check(AkismetComment comment)
        {
            var req = new RestRequest("comment-check", Method.POST)
               .AddParameter("blog", blogUrl);

            var attributes = AttributeHelper.GetAttributes(comment);
            foreach (var kv in attributes)
                req.AddParameter(kv.Key, kv.Value);

            var resp = client.Execute(req);

            AkismetResponse response = new AkismetResponse();

            if (!Boolean.TryParse(resp.Content, out bool result))
            {
                response.AkismetErrors.Add(resp.Content);
                response.SpamStatus = SpamStatus.Unspecified;
            }
            else
                response.SpamStatus = result ? SpamStatus.Spam : SpamStatus.Ham;

            if (resp.Headers.Any(r => r.Name.ToLower() == "x-akismet-debug-help"))
                response.AkismetDebugHelp = resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-debug-help").First().Value.ToString();
            if (resp.Headers.Any(r => r.Name.ToLower() == "x-akismet-pro-tip"))
                response.ProTip = resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-pro-tip").First().Value.ToString();
            if (resp.Headers.Any(r => r.Name.ToLower() == "x-akismet-alert-code"))
                response.AkismetErrors.Add(resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-alert-code").First().Value.ToString() + ": " + resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-alert-msg").First().Value.ToString());

            return response;
        }

        /// <summary>
        /// Submit missed spam
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<AkismetResponse> SubmitSpamAsync(AkismetComment comment)
        {
            var req = new RestRequest("submit-spam", Method.POST)
                .AddParameter("blog", blogUrl);

            var attributes = AttributeHelper.GetAttributes(comment);
            foreach (var kv in attributes)
                req.AddParameter(kv.Key, kv.Value);

            var resp = await client.ExecuteAsync(req);

            AkismetResponse response = new AkismetResponse();

            if (!Boolean.TryParse(resp.Content, out bool result))
            {
                response.AkismetErrors.Add(resp.Content);
                response.SpamStatus = SpamStatus.Unspecified;
            }
            else
                response.SpamStatus = result ? SpamStatus.Spam : SpamStatus.Ham;

            if (resp.Headers.Any(r => r.Name.ToLower() == "x-akismet-debug-help"))
                response.AkismetDebugHelp = resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-debug-help").First().Value.ToString();
            if (resp.Headers.Any(r => r.Name.ToLower() == "x-akismet-pro-tip"))
                response.ProTip = resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-pro-tip").First().Value.ToString();
            if (resp.Headers.Any(r => r.Name.ToLower() == "x-akismet-alert-code"))
                response.AkismetErrors.Add(resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-alert-code").First().Value.ToString() + ": " + resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-alert-msg").First().Value.ToString());

            return response;
        }

        /// <summary>
        /// Submit missed spam
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public AkismetResponse SubmitSpam(AkismetComment comment)
        {
            var req = new RestRequest("submit-spam", Method.POST)
                .AddParameter("blog", blogUrl);

            var attributes = AttributeHelper.GetAttributes(comment);
            foreach (var kv in attributes)
                req.AddParameter(kv.Key, kv.Value);

            var resp = client.Execute(req);

            AkismetResponse response = new AkismetResponse();

            if (!Boolean.TryParse(resp.Content, out bool result))
            {
                response.AkismetErrors.Add(resp.Content);
                response.SpamStatus = SpamStatus.Unspecified;
            }
            else
                response.SpamStatus = result ? SpamStatus.Spam : SpamStatus.Ham;

            if (resp.Headers.Any(r => r.Name.ToLower() == "x-akismet-debug-help"))
                response.AkismetDebugHelp = resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-debug-help").First().Value.ToString();
            if (resp.Headers.Any(r => r.Name.ToLower() == "x-akismet-pro-tip"))
                response.ProTip = resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-pro-tip").First().Value.ToString();
            if (resp.Headers.Any(r => r.Name.ToLower() == "x-akismet-alert-code"))
                response.AkismetErrors.Add(resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-alert-code").First().Value.ToString() + ": " + resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-alert-msg").First().Value.ToString());

            return response;
        }

        /// <summary>
        /// Submit false positive (ham marked as spam)
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<AkismetResponse> SubmitHamAsync(AkismetComment comment)
        {
            var req = new RestRequest("submit-ham", Method.POST)
                .AddParameter("blog", blogUrl);

            var attributes = AttributeHelper.GetAttributes(comment);
            foreach (var kv in attributes)
                req.AddParameter(kv.Key, kv.Value);

            var resp = await client.ExecuteAsync(req);

            AkismetResponse response = new AkismetResponse();

            if (!Boolean.TryParse(resp.Content, out bool result))
            {
                response.AkismetErrors.Add(resp.Content);
                response.SpamStatus = SpamStatus.Unspecified;
            }
            else
                response.SpamStatus = result ? SpamStatus.Spam : SpamStatus.Ham;

            if (resp.Headers.Any(r => r.Name.ToLower() == "x-akismet-debug-help"))
                response.AkismetDebugHelp = resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-debug-help").First().Value.ToString();
            if (resp.Headers.Any(r => r.Name.ToLower() == "x-akismet-pro-tip"))
                response.ProTip = resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-pro-tip").First().Value.ToString();
            if (resp.Headers.Any(r => r.Name.ToLower() == "x-akismet-alert-code"))
                response.AkismetErrors.Add(resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-alert-code").First().Value.ToString() + ": " + resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-alert-msg").First().Value.ToString());

            return response;
        }

        /// <summary>
        /// Submit false positive (ham marked as spam)
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public AkismetResponse SubmitHam(AkismetComment comment)
        {
            var req = new RestRequest("submit-ham", Method.POST)
                .AddParameter("blog", blogUrl);

            var attributes = AttributeHelper.GetAttributes(comment);
            foreach (var kv in attributes)
                req.AddParameter(kv.Key, kv.Value);

            var resp = client.Execute(req);

            AkismetResponse response = new AkismetResponse();

            if (!Boolean.TryParse(resp.Content, out bool result))
            {
                response.AkismetErrors.Add(resp.Content);
                response.SpamStatus = SpamStatus.Unspecified;
            }
            else
                response.SpamStatus = result ? SpamStatus.Spam : SpamStatus.Ham;

            if (resp.Headers.Any(r => r.Name.ToLower() == "x-akismet-debug-help"))
                response.AkismetDebugHelp = resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-debug-help").First().Value.ToString();
            if (resp.Headers.Any(r => r.Name.ToLower() == "x-akismet-pro-tip"))
                response.ProTip = resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-pro-tip").First().Value.ToString();
            if (resp.Headers.Any(r => r.Name.ToLower() == "x-akismet-alert-code"))
                response.AkismetErrors.Add(resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-alert-code").First().Value.ToString() + ": " + resp.Headers.Where(r => r.Name.ToLower() == "x-akismet-alert-msg").First().Value.ToString());

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<AkismetAccount> GetAccountStatusAsync()
        {
            var req = new RestRequest("get-subscription", Method.POST)
                .AddParameter("key", apiKey)
                .AddParameter("blog", blogUrl);

            var resp = await client.ExecuteAsync(req);
            dynamic data = JsonConvert.DeserializeObject(resp.Content);

            AkismetAccount account = new AkismetAccount
            {
                AccountId = (int)data["account_id"],
                AccountName = (string)data["account_name"],
                AccountType = (string)data["account_type"],
                Status = (string)data["status"],
                LimitReached = (bool)data["limit_reached"]
            };
            if (!(data["next_billing_date"] is bool))
                account.NextBillingDate = DateTimeHelper.UnixTimeStampToDateTime(Convert.ToInt32(data["next_billing_date"]));

            return account;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AkismetAccount GetAccountStatus()
        {
            var req = new RestRequest("get-subscription", Method.POST)
                .AddParameter("key", apiKey)
                .AddParameter("blog", blogUrl);

            var resp = client.Execute(req);
            dynamic data = JsonConvert.DeserializeObject(resp.Content);

            AkismetAccount account = new AkismetAccount
            {
                AccountId = (int)data["account_id"],
                AccountName = (string)data["account_name"],
                AccountType = (string)data["account_type"],
                Status = (string)data["status"],
                LimitReached = (bool)data["limit_reached"]
            };
            if (!(data["next_billing_date"] is bool))
                account.NextBillingDate = DateTimeHelper.UnixTimeStampToDateTime(Convert.ToInt32(data["next_billing_date"]));

            return account;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval">Allowed options: 60-days, 6-months, all</param>
        /// <returns></returns>
        public async Task<SpamStats> GetStatisticsAsync(string interval = "")
        {
            if (!String.IsNullOrWhiteSpace(interval) && !allowedIntervals.Contains(interval))
                throw new ArgumentException("Invalid interval", nameof(interval));

            var req = new RestRequest("get-stats", Method.POST)
                .AddParameter("key", apiKey)
                .AddParameter("blog", blogUrl);

            if (!String.IsNullOrWhiteSpace(interval))
                req.AddParameter("from", interval);

            var resp = await client.ExecuteAsync(req);
            var data = JsonConvert.DeserializeObject<SpamStats>(resp.Content);

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval">Allowed options: 60-days, 6-months, all</param>
        /// <returns></returns>
        public SpamStats GetStatistics(string interval = "")
        {
            if (!String.IsNullOrWhiteSpace(interval) && !allowedIntervals.Contains(interval))
                throw new ArgumentException("Invalid interval", nameof(interval));

            var req = new RestRequest("get-stats", Method.POST)
                .AddParameter("key", apiKey)
                .AddParameter("blog", blogUrl);

            if (!String.IsNullOrWhiteSpace(interval))
                req.AddParameter("from", interval);

            var resp = client.Execute(req);
            var data = JsonConvert.DeserializeObject<SpamStats>(resp.Content);

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> DecativateAsync()
        {
            var req = new RestRequest("deactivate", Method.POST)
                .AddParameter("key", apiKey)
                .AddParameter("blog", blogUrl);

            var resp = await client.ExecuteAsync(req);

            return resp.Content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Deactivate()
        {
            var req = new RestRequest("deactivate", Method.POST)
                .AddParameter("key", apiKey)
                .AddParameter("blog", blogUrl);

            var resp = client.Execute(req);

            return resp.Content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<RestResponse> CustomCallAsync(string command, Dictionary<string, string> attributes)
        {
            var req = new RestRequest(command, Method.POST)
                .AddParameter("key", apiKey)
                .AddParameter("blog", blogUrl);

            foreach (var kv in attributes)
                req.AddParameter(kv.Key, kv.Value);

            RestResponse resp = (RestResponse)await client.ExecuteAsync(req);

            return resp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RestResponse CustomCall(string command, Dictionary<string, string> attributes)
        {
            var req = new RestRequest(command, Method.POST)
                .AddParameter("key", apiKey)
                .AddParameter("blog", blogUrl);

            foreach (var kv in attributes)
                req.AddParameter(kv.Key, kv.Value);

            RestResponse resp = (RestResponse)client.Execute(req);

            return resp;
        }
    }
}
