namespace eVision.eCrypt.Tests.Runners
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Common;

    internal class SelfExtractorRunner
    {
        private readonly string _selfExtractorPath;

        public SelfExtractorRunner(string selfExtractorPath)
        {
            _selfExtractorPath = selfExtractorPath;
        }

        public async Task<ProcessResult> RunAsync(string privateKeyPath, string destinationPath)
        {
            return await new ProcessRunner(_selfExtractorPath)
                .RunAsync(new Dictionary<string, string>()
                {
                    {"key-path",privateKeyPath},
                    {"destination-path",destinationPath},
                    {"verbose", null }
                });
        }
    }
}
