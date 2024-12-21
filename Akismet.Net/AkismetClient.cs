using Akismet.Net.Helpers;
#if NETSTANDARD
using Microsoft.Extensions.Options;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

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

#if !NETSTANDARD
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
                BaseAddress = new Uri($"https://{apiKey}.rest.akismet.com/1.1/"),
            };
            client.DefaultRequestHeaders.Add("User-Agent", $"{applicationName} | Akismet.NET/{Assembly.GetExecutingAssembly().GetName().Version} (https://github.com/ahwm/Akismet.Net)");
        }
#else
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="blogUrl"></param>
        /// <param name="applicationName"></param>
        public AkismetClient(HttpClient _httpClient, IOptions<AkismetClientOptions> options)
        {
            this.apiKey = options.Value.Key;
            this.blogUrl = options.Value.BlogUrl;

            client = _httpClient;
        }
#endif

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
        /// Check comment
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<AkismetResponse> CheckAsync(AkismetComment comment)
        {
            var data = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("blog", blogUrl)
            };

            data.AddRange(AttributeHelper.GetAttributes(comment));

            var resp = await client.PostAsync("comment-check", new FormUrlEncodedContent(data));

            AkismetResponse response = new AkismetResponse();
            var responseData = await resp.Content.ReadAsStringAsync();

            if (!Boolean.TryParse(responseData, out bool result))
            {
                response.AkismetErrors.Add(responseData);
                response.SpamStatus = SpamStatus.Unspecified;
            }
            else
                response.SpamStatus = result ? SpamStatus.Spam : SpamStatus.Ham;

            if (resp.Headers.Any(r => r.Key.ToLower() == "x-akismet-debug-help"))
                response.AkismetDebugHelp = resp.Headers.Where(r => r.Key.ToLower() == "x-akismet-debug-help").First().Value.ToString();
            if (resp.Headers.Any(r => r.Key.ToLower() == "x-akismet-pro-tip"))
                response.ProTip = resp.Headers.Where(r => r.Key.ToLower() == "x-akismet-pro-tip").First().Value.ToString();
            if (resp.Headers.Any(r => r.Key.ToLower() == "x-akismet-alert-code"))
                response.AkismetErrors.Add(resp.Headers.Where(r => r.Key.ToLower() == "x-akismet-alert-code").First().Value.ToString() + ": " + resp.Headers.Where(r => r.Key.ToLower() == "x-akismet-alert-msg").First().Value.ToString());

            return response;
        }

        /// <summary>
        /// Submit missed spam
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<AkismetResponse> SubmitSpamAsync(AkismetComment comment)
        {
            var data = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("blog", blogUrl)
            };

            data.AddRange(AttributeHelper.GetAttributes(comment));

            var resp = await client.PostAsync("submit-spam", new FormUrlEncodedContent(data));

            AkismetResponse response = new AkismetResponse();
            var responseData = await resp.Content.ReadAsStringAsync();

            if (!Boolean.TryParse(responseData, out bool result))
            {
                response.AkismetErrors.Add(responseData);
                response.SpamStatus = SpamStatus.Unspecified;
            }
            else
                response.SpamStatus = result ? SpamStatus.Spam : SpamStatus.Ham;

            if (resp.Headers.Any(r => r.Key.ToLower() == "x-akismet-debug-help"))
                response.AkismetDebugHelp = resp.Headers.Where(r => r.Key.ToLower() == "x-akismet-debug-help").First().Value.ToString();
            if (resp.Headers.Any(r => r.Key.ToLower() == "x-akismet-pro-tip"))
                response.ProTip = resp.Headers.Where(r => r.Key.ToLower() == "x-akismet-pro-tip").First().Value.ToString();
            if (resp.Headers.Any(r => r.Key.ToLower() == "x-akismet-alert-code"))
                response.AkismetErrors.Add(resp.Headers.Where(r => r.Key.ToLower() == "x-akismet-alert-code").First().Value.ToString() + ": " + resp.Headers.Where(r => r.Key.ToLower() == "x-akismet-alert-msg").First().Value.ToString());

            return response;
        }

        /// <summary>
        /// Submit false positive (ham marked as spam)
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<AkismetResponse> SubmitHamAsync(AkismetComment comment)
        {
            var data = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("blog", blogUrl)
            };

            data.AddRange(AttributeHelper.GetAttributes(comment));

            var resp = await client.PostAsync("submit-ham", new FormUrlEncodedContent(data));

            AkismetResponse response = new AkismetResponse();
            var responseData = await resp.Content.ReadAsStringAsync();

            if (!Boolean.TryParse(responseData, out bool result))
            {
                response.AkismetErrors.Add(responseData);
                response.SpamStatus = SpamStatus.Unspecified;
            }
            else
                response.SpamStatus = result ? SpamStatus.Spam : SpamStatus.Ham;

            if (resp.Headers.Any(r => r.Key.ToLower() == "x-akismet-debug-help"))
                response.AkismetDebugHelp = resp.Headers.Where(r => r.Key.ToLower() == "x-akismet-debug-help").First().Value.ToString();
            if (resp.Headers.Any(r => r.Key.ToLower() == "x-akismet-pro-tip"))
                response.ProTip = resp.Headers.Where(r => r.Key.ToLower() == "x-akismet-pro-tip").First().Value.ToString();
            if (resp.Headers.Any(r => r.Key.ToLower() == "x-akismet-alert-code"))
                response.AkismetErrors.Add(resp.Headers.Where(r => r.Key.ToLower() == "x-akismet-alert-code").First().Value.ToString() + ": " + resp.Headers.Where(r => r.Key.ToLower() == "x-akismet-alert-msg").First().Value.ToString());

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<AkismetAccount> GetAccountStatusAsync()
        {
            var formData = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("key", apiKey),
                new KeyValuePair<string, string>("blog", blogUrl)
            });
            var resp = await client.PostAsync("get-subscription", formData);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(await resp.Content.ReadAsStringAsync());

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

            var data = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("key", apiKey),
                new KeyValuePair<string, string>("blog", blogUrl)
            };
            if (!String.IsNullOrWhiteSpace(interval))
                data.Add(new KeyValuePair<string, string>("from", interval));
            var resp = await client.PostAsync("get-stats", new FormUrlEncodedContent(data));

            var stats = JsonSerializer.Deserialize<SpamStats>(await resp.Content.ReadAsStringAsync());

            return stats;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> DecativateAsync()
        {
            var formData = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("key", apiKey),
                new KeyValuePair<string, string>("blog", blogUrl)
            });

            var resp = await client.PostAsync("deactivate", formData);

            return await resp.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> CustomCallAsync(string command, Dictionary<string, string> attributes)
        {
            var data = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("key", apiKey),
                new KeyValuePair<string, string>("blog", blogUrl)
            };
            data.AddRange(attributes);
            var resp = await client.PostAsync(command, new FormUrlEncodedContent(data));

            return await resp.Content.ReadAsStringAsync();
        }
    }

    public class AkismetClientOptions
    {
        public string Key { get; set; } = "";
        public string BlogUrl { get; set; } = "";
    }
}
