namespace eVision.eCrypt
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CSharp;
    //TODO: use Roslyn compiler so we will be able to compile C#6.
    //Now in projects "Decryption.UI" and "Encryption" we use only C#5
    //Roslyn can be used via nuget package "Microsoft.CodeDom.Providers.DotNetCompilerPlatform"
    //using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;

    internal class AssemblyCompiler
    {
        private const string Csharp = "csharp";
        private const string LibraryExtension = "dll";
        private readonly List<string> _references = new List<string>();
        private readonly List<string> _sourceCode = new List<string>();
        private readonly List<string> _symbols = new List<string>();
        private readonly List<string> _embeddedResources = new List<string>();
        private Target _target = Target.Library;
        private string _iconFilename;
        private string _language = Csharp;
        private string _outputFileName = Guid.NewGuid().ToString();

        public CompilerResults Compile()
        {
            using (CodeDomProvider domProvider = CreateProvider())
            {
                CompilerResults result = domProvider
                    .CompileAssemblyFromSource(BuildParameters(domProvider), _sourceCode.ToArray());

                ThrowOnError(result);
                return result;
            }
        }

        public AssemblyCompiler UsingLanguage(string language)
        {
            _language = language;
            return this;
        }

        public AssemblyCompiler WithIcon(string iconFilename)
        {
            _iconFilename = iconFilename;
            return this;
        }

        public AssemblyCompiler Reference(IEnumerable<string> references)
        {
            _references.AddRange(references);
            return this;
        }

        public AssemblyCompiler WithCode(IEnumerable<string> sourceCode)
        {
            _sourceCode.AddRange(sourceCode);
            return this;
        }

        public AssemblyCompiler WithEmbeddedResource(IEnumerable<string> embeddedResources)
        {
            _embeddedResources.AddRange(embeddedResources);
            return this;
        }

        public AssemblyCompiler WithName(string name)
        {
            _outputFileName = name;
            return this;
        }

        public AssemblyCompiler DefineSymbol(string compilationSymbol)
        {
            _symbols.Add(compilationSymbol);
            return this;
        }

        public AssemblyCompiler AsDebug
        {
            get
            {
                DefineSymbol("DEBUG");
                return this;
            }
        }

        public AssemblyCompiler As(Target target)
        {
            _target = target;
            return this;
        }

        private CodeDomProvider CreateProvider() =>
            _language == Csharp || !CodeDomProvider.IsDefinedLanguage(_language)
                ? new CSharpCodeProvider()
                : CodeDomProvider.CreateProvider(_language);

        private CompilerParameters BuildParameters(CodeDomProvider provider)
        {
            var parameters = new CompilerParameters
            {
                OutputAssembly = _outputFileName,
                GenerateExecutable = _target != Target.Library,
                CompilerOptions = $"/target:{_target.ToString().ToLowerInvariant()}",
            };

            if (_symbols.Any())
            {
                parameters.CompilerOptions += $" /define:{string.Join(";", _symbols)}";
            }

            if (!string.IsNullOrEmpty(_iconFilename))
            {
                parameters.CompilerOptions += $" /win32icon:\"{_iconFilename}\"";
            }

            var references = _references
                .Select(r => r.EndsWith(LibraryExtension) ? r : $"{r}.{LibraryExtension}")
                .ToArray();

            parameters.ReferencedAssemblies.AddRange(references);

            if (provider.Supports(GeneratorSupport.Resources))
            {
                parameters.EmbeddedResources.AddRange(_embeddedResources.ToArray());
            }

            return parameters;
        }

        private void ThrowOnError(CompilerResults result)
        {
            if (result.Errors.Count > 0)
            {
                var errors = string.Join(Environment.NewLine, result.Errors
                    .Cast<CompilerError>().Select(e => e));

                throw new ApplicationException($"Errors building {errors}");
            }
        }

        public enum Target
        {
            Library,
            Exe,
            WinExe
        }
    }
}
