using System;

namespace Akismet.Net.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    sealed internal class AkismetNameAttribute : Attribute
    {
        public readonly string AkismetName;

        public AkismetNameAttribute(string name)
        {
            AkismetName = name;
        }
    }
}
