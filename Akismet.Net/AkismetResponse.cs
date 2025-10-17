
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
        public List<string> AkismetErrors { get; } = new List<string>();

        /// <summary>
        /// Value of X-akismet-debug-help header, if present
        /// </summary>
        public string AkismetDebugHelp { get; set; }

        /// <summary>
        /// Value of X-akismet-recheck-after header, if present. Indicates the number of seconds after which the comment should be rechecked with the added parameter of recheck_reason=recheck.
        /// </summary>
        public int? RecheckAfter { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum SpamStatus
    {
        /// <summary>
        /// 
        /// </summary>
        Spam,
        /// <summary>
        /// 
        /// </summary>
        Ham,
        /// <summary>
        /// 
        /// </summary>
        Unspecified
    }
}
