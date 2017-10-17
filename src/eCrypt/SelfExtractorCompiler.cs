namespace eVision.eCrypt
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Helpers.Extensions;
    using Transformations;
    using Encryption.Helpers;
    using Target = AssemblyCompiler.Target;

    public class SelfExtractorCompiler : IDisposable
    {
        private const string SourcesFolder = "SelfExtractorSources";
        private const string Resources = "Resources";
        private const string CodeExtension = "cs";
        private const string IconExtension = "ico";
        private const string ProjectExtension = "csproj";
        private static readonly string AppNamespace = $"{nameof(eVision)}.{nameof(eCrypt)}";
        private static readonly string AllResourcePrefix = $"{AppNamespace}.{SourcesFolder}";
        private static readonly string DecryptorResourcePrefix = $"{AppNamespace}.{SourcesFolder}.{Resources}.";
        private readonly List<string> _filePathsToInclude = new List<string>();
        private readonly TempFileCollection _transformedFiles = new TempFileCollection();
        private readonly TempDirectory _resourcesDir = TempDirectory.InBaseDirectory();
        private readonly IFileTransformation _transformation;

        public SelfExtractorCompiler(IFileTransformation transformation)
        {
            _transformation = transformation;
        }

        public string Compile(string outputAssemblyName)
        {
            ProjectInfo projectInfo = GetProjectInfo();

            TransformFiles();
            ExtractResources();

            string[] references = projectInfo.References
                .Select(reference => _resourcesDir.Files.FirstOrDefault(f => f.Contains(reference)) ?? reference)
                .ToArray();

            CompilerResults result = new AssemblyCompiler()
                .Reference(references)
                .WithCode(SourceCode)
                .WithIcon(_resourcesDir.Files.First(r => r.EndsWith(IconExtension)))
                .WithName(outputAssemblyName)
                .WithEmbeddedResource(_transformedFiles.Cast<string>())
                .WithEmbeddedResource(_resourcesDir.Files)
                .DefineSymbol("SELF_EXTRACTOR_COMPILER")
                .As(Target.WinExe)
                .Compile();

            return result.PathToAssembly;
        }

        public SelfExtractorCompiler IncludingFiles(params string[] filePathsToInclude)
        {
            _filePathsToInclude.AddRange(filePathsToInclude);
            return this;
        }

        private static IEnumerable<string> ResourceNames => Assembly.GetExecutingAssembly()
            .GetManifestResourceNames()
            .Where(name => name.StartsWith(AllResourcePrefix));

        private static string[] SourceCode => ResourceNames
            .Where(name => name.EndsWith(CodeExtension))
            .Select(name => Assembly.GetExecutingAssembly().GetStringResource(name))
            .ToArray();

        private static ProjectInfo GetProjectInfo()
        {
            string projResourceName = ResourceNames
                .Single(name => name.EndsWith(ProjectExtension));

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(projResourceName))
            {
                return new ProjectInfo(stream);
            }
        }

        private void TransformFiles()
        {
            foreach (string filePathToInclude in _filePathsToInclude)
            {
                _transformedFiles.AddFile(_transformation.TransformToNewFile(filePathToInclude), false);
            }
        }

        private void ExtractResources()
        {
            string[] resourceKeys = Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .Where(r => r.StartsWith(DecryptorResourcePrefix))
                .ToArray();

            foreach (string resourceKey in resourceKeys)
            {
                string path = Path.Combine(_resourcesDir.Path, resourceKey.TrimStart(DecryptorResourcePrefix));
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceKey))
                using (FileStream fileStream = File.Create(path))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }

        public void Dispose()
        {
            _transformedFiles.Delete();
            _resourcesDir.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
