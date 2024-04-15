using System;

namespace GT.RText.Core.Exceptions
{
    public class XorKeyTooShortException : Exception
    {
        public XorKeyTooShortException(string message) : base(message)
        {
        }
    }
}
