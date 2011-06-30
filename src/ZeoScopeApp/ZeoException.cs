namespace ZeoScope
{
    using System;

    internal class ZeoException : Exception
    {
        public ZeoException()
            : base()
        {
        }

        public ZeoException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public ZeoException(Exception ex, string format, params object[] args)
            : base(string.Format(format, args), ex)
        {
        }
    }
}
