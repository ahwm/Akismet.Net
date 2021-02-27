using System;
using System.Collections.Generic;
using System.Text;

namespace Akismet.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class AkismetAccount
    {
        /// <summary>
        /// 
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? NextBillingDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool LimitReached { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AccountType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }
    }
}
