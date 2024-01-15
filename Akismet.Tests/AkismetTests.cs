﻿using Akismet.Net;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Akismet.Tests
{
    public class AkismetTests
    {
        private readonly string ApiKey;
        private readonly string ApiKeyUrl;
        private readonly AkismetClient Client;

        public AkismetTests()
        {
            ApiKey = Environment.GetEnvironmentVariable("AKISMET_API_KEY").Trim();
            ApiKeyUrl = Environment.GetEnvironmentVariable("AKISMET_API_KEY_URL").Trim();

            Client = new AkismetClient(ApiKey, new Uri("https://www.adamh.us"), "Akismet Test Application");
        }

        [Fact]
        public void VerifyKeyTest()
        {
            Client.VerifyKey().ShouldBe(true);
        }

        [Fact]
        public async Task VerifyKeyAsyncTest()
        {
            var isValid = await Client.VerifyKeyAsync();

            isValid.ShouldBe(true);
        }

        [Fact]
        public void CheckSpamComment()
        {
            AkismetComment comment = new AkismetComment
{
                CommentAuthor = "viagra-test-123",
                CommentAuthorEmail = "akismet-guaranteed-spam@example.com",
                CommentAuthorUrl = "http://www.spamwebsite.com",
                Referrer = "https://www.google.com",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.63 Safari/537.36",
                CommentContent = "This is a test spam comment",
                CommentType = AkismentCommentType.ContactForm, // multiple defined values, or use new AkismetCommentType("new-comment-type") for a custom option
                Permalink = $"{ApiKeyUrl}/contact",
                IsTest = "true",
                BlogCharset = "UTF-8",
                BlogLanguage = "en-US",
                CommentDate = DateTime.UtcNow.ToString("s"), // ISO-8601 format
                CommentPostModified = DateTime.UtcNow.ToString("s") // ISO-8601 format
            };
            var spamResult = Client.Check(comment);
            spamResult.SpamStatus.ShouldBe(SpamStatus.Spam);
        }

        [Fact]
        public async Task CheckSpamCommentAsync()
        {
            AkismetComment comment = new AkismetComment
            {
                CommentAuthor = "viagra-test-123",
                CommentAuthorEmail = "akismet-guaranteed-spam@example.com",
                CommentAuthorUrl = "http://www.spamwebsite.com",
                Referrer = "https://www.google.com",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.63 Safari/537.36",
                CommentContent = "This is a test spam comment",
                CommentType = AkismentCommentType.ContactForm,
                Permalink = $"{ApiKeyUrl}/contact",
                IsTest = "true",
                BlogCharset = "UTF-8",
                BlogLanguage = "en-US",
                CommentDate = DateTime.UtcNow.ToString("s"),
                CommentPostModified = DateTime.UtcNow.ToString("s")
            };
            var spamResult = await Client.CheckAsync(comment);
            spamResult.SpamStatus.ShouldBe(SpamStatus.Spam);
        }

        [Fact]
        public void CheckHamComment()
        {
            AkismetComment comment = new AkismetComment
            {
                CommentAuthor = "Test",
                CommentAuthorEmail = "test@example.com",
                CommentAuthorUrl = "http://www.spamwebsite.com",
                Referrer = "https://www.google.com",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.63 Safari/537.36",
                CommentContent = "This is a test ham comment",
                CommentType = AkismentCommentType.ContactForm,
                Permalink = $"{ApiKeyUrl}/contact",
                IsTest = "true",
                BlogCharset = "UTF-8",
                BlogLanguage = "en-US",
                CommentDate = DateTime.UtcNow.ToString("s"),
                CommentPostModified = DateTime.UtcNow.ToString("s"),
                UserRole = "administrator"
            };
            var spamResult = Client.Check(comment);
            spamResult.SpamStatus.ShouldBe(SpamStatus.Ham);
        }

        [Fact]
        public async Task CheckHamCommentAsync()
        {
            AkismetComment comment = new AkismetComment
            {
                CommentAuthor = "Test",
                CommentAuthorEmail = "test@example.com",
                CommentAuthorUrl = "http://www.spamwebsite.com",
                Referrer = "https://www.google.com",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.63 Safari/537.36",
                CommentContent = "This is a test ham comment",
                CommentType = AkismentCommentType.ContactForm,
                Permalink = $"{ApiKeyUrl}/contact",
                IsTest = "true",
                BlogCharset = "UTF-8",
                BlogLanguage = "en-US",
                CommentDate = DateTime.UtcNow.ToString("s"),
                CommentPostModified = DateTime.UtcNow.ToString("s"),
                UserRole = "administrator"
            };
            var spamResult = await Client.CheckAsync(comment);
            spamResult.SpamStatus.ShouldBe(SpamStatus.Ham);
        }
    }
}
