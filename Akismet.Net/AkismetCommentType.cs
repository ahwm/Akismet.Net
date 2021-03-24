using System;
using System.Collections.Generic;
using System.Text;

namespace Akismet.Net
{
    /// <summary>
    /// Akismet comment type
    /// </summary>
    public struct AkismentCommentType
    {
        internal string CommentType { get; }

        /// <summary>
        /// A blog comment
        /// </summary>
        public static readonly AkismentCommentType Comment = new AkismentCommentType("comment");

        /// <summary>
        /// A top-level forum post
        /// </summary>
        public static readonly AkismentCommentType ForumPost = new AkismentCommentType("forum-post");

        /// <summary>
        /// A reply to a top-level forum post
        /// </summary>
        public static readonly AkismentCommentType Reply = new AkismentCommentType("reply");

        /// <summary>
        /// A blog post
        /// </summary>
        public static readonly AkismentCommentType BlogPost = new AkismentCommentType("blog-post");

        /// <summary>
        /// A contact form or feedback form submission
        /// </summary>
        public static readonly AkismentCommentType ContactForm = new AkismentCommentType("contact-form");

        /// <summary>
        /// A new user account
        /// </summary>
        public static readonly AkismentCommentType Signup = new AkismentCommentType("signup");

        /// <summary>
        /// A message sent between just a few users
        /// </summary>
        public static readonly AkismentCommentType Message = new AkismentCommentType("message");

        /// <summary>
        /// Specify a comment type
        /// </summary>
        /// <param name="type"></param>
        public AkismentCommentType(string type) => CommentType = type;

        /// <summary>
        /// Returns string representation of the comment type
        /// </summary>
        /// <returns></returns>
        public override string ToString() => CommentType;

        /// <inheritdoc/>
        public static implicit operator string(AkismentCommentType t) => t.CommentType;
        /// <inheritdoc/>
        public static implicit operator AkismentCommentType(string t) => new AkismentCommentType(t);
    }
}
