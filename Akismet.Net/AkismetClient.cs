using Akismet.Net.Helpers;
#if NETSTANDARD
using Microsoft.Extensions.Options;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
#if !NETSTANDARD
using System.Reflection;
#endif
using System.Text.Json;
using System.Threading.Tasks;

namespace Akismet.Net
{
    /// <summary>
    /// Main client
    /// </summary>
    public class AkismetClient
    {
        private readonly string apiKey;
        private readonly string[] allowedIntervals = new[] { "60-days", "6-months", "all" };

        private readonly HttpClient client;

#if !NETSTANDARD
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="applicationName"></param>
        public AkismetClient(string apiKey, string applicationName)
        {
            this.apiKey = apiKey;

            client = new HttpClient()
            {
                BaseAddress = new Uri("https://rest.akismet.com/"),
            };
            client.DefaultRequestHeaders.Add("User-Agent", $"{applicationName} | Akismet.NET/{Assembly.GetExecutingAssembly().GetName().Version} (https://github.com/ahwm/Akismet.Net)");
        }
#else
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_httpClient"></param>
        /// <param name="options"></param>
        public AkismetClient(HttpClient _httpClient, IOptions<AkismetClientOptions> options)
        {
            this.apiKey = options.Value.Key;

            client = _httpClient;
        }
#endif

        /// <summary>
        /// Verify key
        /// </summary>
        /// <returns></returns>
        public async Task<bool> VerifyKeyAsync(string blogUrl)
        {
            var formData = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("key", apiKey),
                new KeyValuePair<string, string>("blog", blogUrl)
            });
            var resp = await client.PostAsync("1.1/verify-key", formData);

            return await resp.Content.ReadAsStringAsync() == "valid";
        }

        /// <summary>
        /// Check comment
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<AkismetResponse> CheckAsync(AkismetComment comment)
        {
            var data = new List<KeyValuePair<string, string>>();

            data.AddRange(AttributeHelper.GetAttributes(comment));

            var resp = await client.PostAsync("1.1/comment-check", new FormUrlEncodedContent(data));

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
                response.AkismetDebugHelp = resp.Headers.First(r => r.Key.ToLower() == "x-akismet-debug-help").Value.ToString();
            if (resp.Headers.Any(r => r.Key.ToLower() == "x-akismet-pro-tip"))
                response.ProTip = resp.Headers.First(r => r.Key.ToLower() == "x-akismet-pro-tip").Value.ToString();
            if (resp.Headers.Any(r => r.Key.ToLower() == "x-akismet-alert-code"))
                response.AkismetErrors.Add(resp.Headers.First(r => r.Key.ToLower() == "x-akismet-alert-code").Value.ToString() + ": " + resp.Headers.First(r => r.Key.ToLower() == "x-akismet-alert-msg").Value.ToString());
            if (resp.Headers.Any(r => r.Key.ToLower() == "x-akismet-recheck-after"))
                response.RecheckAfter = Convert.ToInt32(resp.Headers.First(r => r.Key.ToLower() == "x-akismet-recheck-after").Value.ToString());

            return response;
        }

        /// <summary>
        /// Submit missed spam
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<string> SubmitSpamAsync(AkismetComment comment)
        {
            var data = new List<KeyValuePair<string, string>>();

            data.AddRange(AttributeHelper.GetAttributes(comment));

            var resp = await client.PostAsync("1.1/submit-spam", new FormUrlEncodedContent(data));

            var responseData = await resp.Content.ReadAsStringAsync();

            return responseData;
        }

        /// <summary>
        /// Submit false positive (ham marked as spam)
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<string> SubmitHamAsync(AkismetComment comment)
        {
            var data = new List<KeyValuePair<string, string>>();

            data.AddRange(AttributeHelper.GetAttributes(comment));

            var resp = await client.PostAsync("1.1/submit-ham", new FormUrlEncodedContent(data));

            var responseData = await resp.Content.ReadAsStringAsync();

            return responseData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<AkismetAccount> GetAccountStatusAsync(string blogUrl)
        {
            var formData = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("key", apiKey),
                new KeyValuePair<string, string>("blog", blogUrl)
            });
            var resp = await client.PostAsync("1.1/get-subscription", formData);
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
        /// <param name="blogUrl"></param>
        /// <param name="interval">Allowed options: 60-days, 6-months, all</param>
        /// <returns></returns>
        public async Task<SpamStats> GetStatisticsAsync(string blogUrl, string interval = "")
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
            var resp = await client.PostAsync("1.1/get-stats", new FormUrlEncodedContent(data));

            var stats = JsonSerializer.Deserialize<SpamStats>(await resp.Content.ReadAsStringAsync());

            return stats;
        }

        /// <summary>
        /// An endpoint to keep track of the sites that are using your Akismet API key.
        /// </summary>
        /// <param name="month">The month for which you would like to get the report (e.g. 2022‑09). Defaults to the current month.</param>
        /// <param name="filter">
        /// <para>Filter results by site URL or partial site URL.</para>
        /// <para>For example, a filter of “domain.tld” will return activity for all sites with “domain.tld” in their URL.</para>
        /// </param>
        /// <param name="format">The format in which you would like the results to be returned. Valid values: json, csv</param>
        /// <param name="order">
        /// <para>The column by which you would like the results to be sorted.</para>
        /// <list type="bullet">
        /// <item>total (default): Total number of API calls (spam + ham).</item>
        /// <item>spam: Order by the number of spam caught.</item>
        /// <item>ham: Order by the number of non‑spam let through.</item>
        /// <item>missed_spam: Order by the number of reported missed spam.</item>
        /// <item>false_positives: Order by the number of reported false positives.</item>
        /// </list>
        /// </param>
        /// <param name="limit">The maximum number of results returned in the report (defaults to 500).</param>
        /// <param name="offset">The offset of the results returned in the report (defaults to 0).</param>
        /// <returns></returns>
        public async Task<string> GetKeySitesAsync(string month = "", string filter = "", string format = "json", string order = "total", int limit = 500, int offset = 0)
        {
            var data = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("key", apiKey),
                new KeyValuePair<string, string>("format", format),
                new KeyValuePair<string, string>("order", order),
                new KeyValuePair<string, string>("limit", limit.ToString()),
                new KeyValuePair<string, string>("offset", offset.ToString()),
            };
            if (!String.IsNullOrWhiteSpace(month))
                data.Add(new KeyValuePair<string, string>("month", month));
            if (!String.IsNullOrWhiteSpace(filter))
                data.Add(new KeyValuePair<string, string>("filter", filter));
            var resp = await client.PostAsync("1.2/key-sites", new FormUrlEncodedContent(data));

            return await resp.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// An endpoint to keep track of your Akismet API usage for the current month.
        /// </summary>
        /// <returns></returns>
        public async Task<UsageLimit> GetUsageLimitAsync()
        {
            var data = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("key", apiKey)
            };
            var resp = await client.PostAsync("1.2/usage-limit", new FormUrlEncodedContent(data));

            var limit = JsonSerializer.Deserialize<UsageLimit>(await resp.Content.ReadAsStringAsync());

            return limit;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> DecativateAsync(string blogUrl)
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
        public async Task<string> CustomCallAsync(string blogUrl, string command, Dictionary<string, string> attributes)
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

    /// <summary>
    /// 
    /// </summary>
    public class AkismetClientOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public string Key { get; set; } = "";
    }
}