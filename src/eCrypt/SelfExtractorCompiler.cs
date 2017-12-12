namespace eVision.eCrypt
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Helpers.Extensions;
    using Transformations;
    using Encryption.Helpers;
    using Target = AssemblyCompiler.Target;

    public class SelfExtractorCompiler : IDisposable
    {
        private const string SystemReflectionNamespace = nameof(System) + "." + nameof(System.Reflection);
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

        private readonly Regex ReplaceAssemblyVersionRegex = CreateReplaceAttrValueRegex<AssemblyVersionAttribute>();
        private readonly Regex ReplaceFileVersionRegex = CreateReplaceAttrValueRegex<AssemblyFileVersionAttribute>();
        private readonly Regex ReplaceInformVersionRegex = CreateReplaceAttrValueRegex<AssemblyInformationalVersionAttribute>();

        public SelfExtractorCompiler(IFileTransformation transformation)
        {
            _transformation = transformation;
        }

        public string Compile(string outputAssemblyName, string fileVersion = null)
        {
            ProjectInfo projectInfo = GetProjectInfo();

            TransformFiles();
            ExtractResources();

            string[] references = projectInfo.References
                .Select(reference => _resourcesDir.Files.FirstOrDefault(f => f.Contains(reference)) ?? reference)
                .ToArray();

            CompilerResults result = new AssemblyCompiler()
                .Reference(references)
                .Reference(SystemReflectionNamespace)
                .WithCode(GetSourceCode(fileVersion))
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

        private string[] GetSourceCode(string fileVersion)
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            return ResourceNames
                .Where(name => name.EndsWith(CodeExtension))
                .Select(name => Assembly.GetExecutingAssembly().GetStringResource(name))
                .Select(source => ReplaceVersion(source, version, fileVersion))
                .ToArray();
        }

        private string ReplaceVersion(string code, string version, string fileVersion)
        {
            code = ReplaceAssemblyVersionRegex.Replace(code, version);
            code = ReplaceFileVersionRegex.Replace(code, string.IsNullOrEmpty(fileVersion) ? version : fileVersion);
            code = ReplaceInformVersionRegex.Replace(code, version);
            return code;
        }

        private static Regex CreateReplaceAttrValueRegex<TAttr>() where TAttr : Attribute =>
            new Regex($@"(?<=\[assembly:\s{typeof(TAttr).Name.TrimEnd("Attribute")}\("").*?(?=""\)\s*])");

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
