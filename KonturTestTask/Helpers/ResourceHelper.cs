using KonturTestTask.Exceptions;
using System.Reflection;
using System.Xml;

namespace KonturTestTask.Helpers
{
    internal static class ResourceHelper
    {
        private const string XmlConfigNamespace = "KonturTestTask.XmlConfig";

        private const string TransformsNamespace = $"{XmlConfigNamespace}.Transforms";
        private const string SchemasNamespace = $"{XmlConfigNamespace}.Chemas";

        public static Stream GetTransformStream()
        {
            return GetEmbeddedResourceStream($"{TransformsNamespace}.transform_with_group.xslt");
        }

        public static Stream GetSchemaStream()
        {
            return GetEmbeddedResourceStream($"{SchemasNamespace}.schema.xsd");
        }

        private static Stream GetEmbeddedResourceStream(string fullResourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(fullResourceName);

            return stream ?? throw new CustomException($"Embedded ресурс не найден:{Environment.NewLine}{fullResourceName}");
        }

        public static XmlReader ReadTransformContent()
        {
            var stream = GetTransformStream();
            return XmlReader.Create(stream);
        }

        public static XmlReader ReadSchemaContent()
        {
            var stream = GetSchemaStream();
            return XmlReader.Create(stream);
        }
    }
}
