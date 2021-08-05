using Akismet.Net.Attributes;
using Akismet.Net.Helpers;
using Newtonsoft.Json;
using System;

namespace Akismet.Net
{
    /// <summary>
    /// Describes a comment submission
    /// </summary>
    public class AkismetComment
    {
        /// <summary>
        /// IP address of the comment submitter.
        /// </summary>
        [AkismetName("user_ip")]
        public string UserIp { get; set; }

        /// <summary>
        /// User agent string of the web browser submitting the comment - typically the HTTP_USER_AGENT cgi variable. Not to be confused with the user agent of your Akismet library.
        /// </summary>
        [AkismetName("user_agent")]
        public string UserAgent { get; set; }

        /// <summary>
        /// This is an optional parameter. You can use it when submitting test queries to Akismet ("true" or "false")
        /// </summary>
        [AkismetName("is_test")]
        public string IsTest { get; set; }

        /// <summary>
        /// A string that describes the type of content being sent
        /// </summary>
        [AkismetName("comment_type")]
        [JsonConverter(typeof(CommentTypeConverter))]
        public AkismentCommentType CommentType { get; set; }

        /// <summary>
        /// The full permanent URL of the entry the comment was submitted to.
        /// </summary>
        [AkismetName("permalink")]
        public string Permalink { get; set; }

        /// <summary>
        /// The content of the HTTP_REFERER header should be sent here.
        /// </summary>
        [AkismetName("referrer")]
        public string Referrer { get; set; }

        /// <summary>
        /// Name submitted with the comment.
        /// </summary>
        [AkismetName("comment_author")]
        public string CommentAuthor { get; set; }

        /// <summary>
        /// Email address submitted with the comment.
        /// </summary>
        [AkismetName("comment_author_email")]
        public string CommentAuthorEmail { get; set; }

        /// <summary>
        /// URL submitted with comment.
        /// </summary>
        [AkismetName("comment_author_url")]
        public string CommentAuthorUrl { get; set; }

        /// <summary>
        /// The content that was submitted.
        /// </summary>
        [AkismetName("comment_content")]
        public string CommentContent { get; set; }

        /// <summary>
        /// The UTC timestamp of the creation of the comment, in ISO 8601 format. May be omitted for comment-check requests if the comment is sent to the API at the time it is created.
        /// </summary>
        [AkismetName("comment_date_gmt")]
        public string CommentDate { get; set; }

        /// <summary>
        /// The UTC timestamp of the publication time for the post, page or thread on which the comment was posted.
        /// </summary>
        [AkismetName("comment_post_modified_gmt")]
        public string CommentPostModified { get; set; }

        /// <summary>
        /// Indicates the language(s) in use on the blog or site, in ISO 639-1 format, comma-separated. A site with articles in English and French might use "en, fr_ca".
        /// </summary>
        [AkismetName("blog_lang")]
        public string BlogLanguage { get; set; }

        /// <summary>
        /// The character encoding for the form values included in comment_* parameters, such as "UTF-8" or "ISO-8859-1".
        /// </summary>
        [AkismetName("blog_charset")]
        public string BlogCharset { get; set; }

        /// <summary>
        /// The user role of the user who submitted the comment. This is an optional parameter. If you set it to "administrator", Akismet will always return false.
        /// </summary>
        [AkismetName("user_role")]
        public string UserRole { get; set; }

        /// <summary>
        /// If you are sending content to Akismet to be rechecked, such as a post that has been edited or old pending comments that you'd like to recheck, include the parameter recheck_reason with a string describing why the content is being rechecked. For example, recheck_reason=edit
        /// </summary>
        [AkismetName("recheck_reason")]
        public string RecheckReason { get; set; }

        /// <summary>
        /// If you use a honeypot field in your implementation, include the name of the field in your request as well as the value of that field
        /// </summary>
        [AkismetName("honeypot_field_name")]
        public string HoneypotFieldName { get; set; }

        /// <summary>
        /// Value of honeypot field
        /// </summary>
        public string HoneypotFieldValue { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            var attributes = AttributeHelper.GetAttributes(this);

            return JsonConvert.SerializeObject(attributes);
        }
    }

    public class CommentTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AkismentCommentType);
        }

        public override bool CanRead => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string commentType = (AkismentCommentType)value;
            writer.WriteValue(commentType);
        }
    }
}
