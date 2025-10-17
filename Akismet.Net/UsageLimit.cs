using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akismet.Net
{
    public class UsageLimit
    {
        /// <summary>
        /// The number of monthly API calls your plan entitles you to.
        /// </summary>
        public long Limit { get; set; }

        /// <summary>
        /// Number of calls (spam + ham) since the beginning of the current month, to date.
        /// </summary>
        public long Usage { get; set; }

        /// <summary>
        /// The percentage of your limit used since the beginning of the current month, to date.
        /// </summary>
        public string Percentage { get; set; }

        /// <summary>
        /// Indicates if your requests are currently being throttled for having consistently gone over your plan’s limit, or not.
        /// </summary>
        public bool Throttled { get; set; }
    }
}
