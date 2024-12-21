using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Akismet.Net
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SpamStats
    {
        /// <summary>
        /// 
        /// </summary>
        public long Spam { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long Ham { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("missed_spam")]
        public long MissedSpam { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("false_positives")]
        public long FalsePositives { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Accuracy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, Breakdown> Breakdown { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("time_saved")]
        public long TimeSaved { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class Breakdown
    {
        /// <summary>
        /// 
        /// </summary>
        public long Spam { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long Ham { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("missed_spam")]
        public long MissedSpam { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("false_positives")]
        public long FalsePositives { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset Da { get; set; }
    }
}
