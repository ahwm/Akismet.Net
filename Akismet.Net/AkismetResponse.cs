
using System.Collections.Generic;

namespace Akismet.Net
{
    /// <summary>
    /// Describes a response from Akismet
    /// </summary>
    public class AkismetResponse
    {
        /// <summary>
        /// Indicates the status of the submitted comment
        /// </summary>
        public SpamStatus SpamStatus { get; set; }

        /// <summary>
        /// Value of X-akismet-pro-tip header, if present
        /// </summary>
        public string ProTip { get; set; }

        /// <summary>
        /// Errors if any
        /// </summary>
        public List<string> AkismetErrors { get; set; }

        /// <summary>
        /// Value of X-akismet-debug-help header, if present
        /// </summary>
        public string AkismetDebugHelp { get; set; }
    }

    public enum SpamStatus
    {
        Spam,
        Ham,
        Unspecified
    }
}
