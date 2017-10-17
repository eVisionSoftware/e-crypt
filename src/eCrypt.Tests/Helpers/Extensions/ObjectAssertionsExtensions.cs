namespace eVision.eCrypt.Tests.Helpers.Extensions
{
    using System;
    using FluentAssertions.Primitives;
    using Common;

    internal static class ObjectAssertionsExtensions
    {
        public static void BeSuccessfullProcessResult(this ObjectAssertions assertions)
        {
            assertions.BeOfType<ProcessResult>();
            var result = (ProcessResult)assertions.Subject;
            if (!result.IsSuccess)
            {
                throw new Exception($"Executable {result.ExecutablePath} returned code {result.Code}. Output: {result.Output}");
            }
        }
    }
}
