﻿using Akismet.Net.Helpers;
using RestSharp;
using System;
using System.Linq;
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

        private readonly RestClient client;

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

            client = new RestClient($"https://{apiKey}.rest.akismet.com/1.1")
            {
                UserAgent = $"{applicationName} | Akismet.NET/{Assembly.GetExecutingAssembly().GetName().Version}",
            };
        }

        /// <summary>
        /// Verify key
        /// </summary>
        /// <returns></returns>
        public async Task<bool> VerifyKeyAsync()
        {
            var req = new RestRequest("verify-key", Method.POST)
                .AddParameter("key", apiKey)
                .AddParameter("blog", blogUrl);
            var resp = await client.ExecuteAsync(req);

            return resp.Content == "valid";
        }

        /// <summary>
        /// Verify key
        /// </summary>
        /// <returns></returns>
        public bool VerifyKey()
        {
            return VerifyKeyAsync().Result;
        }

        /// <summary>
        /// Check comment
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<AkismetResponse> CheckAsync(AkismetComment comment)
        {
            var req = new RestRequest("comment-check", Method.POST)
                .AddParameter("key", apiKey)
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
            return CheckAsync(comment).Result;
        }

        /// <summary>
        /// Submit missed spam
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<AkismetResponse> SubmitSpamAsync(AkismetComment comment)
        {
            var req = new RestRequest("submit-spam", Method.POST)
                .AddParameter("key", apiKey)
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
            return SubmitSpamAsync(comment).Result;
        }

        /// <summary>
        /// Submit false positive (ham marked as spam)
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<AkismetResponse> SubmitHamAsync(AkismetComment comment)
        {
            var req = new RestRequest("submit-ham", Method.POST)
                .AddParameter("key", apiKey)
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
            return SubmitHamAsync(comment).Result;
        }
    }
}