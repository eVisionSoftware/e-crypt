namespace eVision.Encryption.Helpers
{
    using System;

    internal static class Contract
    {
        internal static void Requires<TException>(bool condition) where TException : Exception
        {
            if (!condition)
            {
                throw Activator.CreateInstance<TException>();
            }
        }

        internal static void Requires<TException>(bool condition, params object[] parameters) where TException : Exception
        {
            if (!condition)
            {
                throw (TException)Activator.CreateInstance(typeof(TException), parameters);
            }
        }
    }
}
