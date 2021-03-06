﻿using System;
using System.Collections.Generic;
using System.Text;

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
        public long MissedSpam { get; set; }

        /// <summary>
        /// 
        /// </summary>
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
        public long MissedSpam { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long FalsePositives { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset Da { get; set; }
    }
}
