namespace eVision.eCrypt
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using Helpers.Extensions;

    internal class ProjectInfo
    {
        public ProjectInfo(Stream projectStream)
        {
            var xdDoc = new XmlDocument();
            xdDoc.Load(projectStream);
            var xnManager = new XmlNamespaceManager(xdDoc.NameTable);
            var root = xdDoc.DocumentElement;
            xnManager.AddNamespace("ns", root.NamespaceURI);

            References = GetProjectReferences(root, xnManager);
            EmbeddedLibraries = GetEmbeddedLibraries(root, xnManager);
        }

        private static string[] GetProjectReferences(XmlElement root, XmlNamespaceManager xnManager)
        {
            string[] references = root.SelectNodes("//ns:Reference/@Include", xnManager)
                .Cast<XmlNode>()
                .Select(n => n.Value.Split(',').First())
                .ToArray();

            return references;
        }

        private static string[] GetEmbeddedLibraries(XmlElement root, XmlNamespaceManager xnManager)
        {
            string[] references = root.SelectNodes("//ns:EmbeddedResource/ns:Link", xnManager)
                .Cast<XmlNode>()
                .Select(n => Path.GetFileName(n.InnerText).TrimEnd(".dll"))
                .ToArray();

            return references;
        }

        public IReadOnlyList<string> References { get; }
        public IReadOnlyList<string> EmbeddedLibraries { get; }
    }
}
