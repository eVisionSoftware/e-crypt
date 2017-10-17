namespace eVision.eCrypt.Common
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public static class EmbeddedAssemblyResolver
    {
        private const string DllExtension = ".dll";
        private static readonly Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();
        private static readonly string[] ResourceNames = ExecutingAssembly.GetManifestResourceNames();
        private static Action<AssemblyName> foundAction;
        private static Action<AssemblyName> missingAction;

        public static void Init(Action<AssemblyName> foundHandler, Action<AssemblyName> missingHandler)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (s, a) => ResolveAssembly(new AssemblyName(a.Name));
            foundAction = foundHandler;
            missingAction = missingHandler;
        }

        private static Assembly ResolveAssembly(AssemblyName name)
        {
            byte[] assembly = Resolve(name);
            if (assembly == null)
            {
                if (missingAction != null)
                {
                    missingAction(name);
                }
                
                return null;
            }

            if (foundAction != null)
            {
                foundAction(name);
            }
            return Assembly.Load(assembly);
        }

        private static byte[] Resolve(AssemblyName name)
        {
            string resourceName = ResourceNames
                .FirstOrDefault(s => s.EndsWith(name.Name + DllExtension, true, CultureInfo.InvariantCulture));

            if (resourceName == null)
            {
                return null;
            }

            using (Stream stream = ExecutingAssembly.GetManifestResourceStream(resourceName))
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }
    }
}

