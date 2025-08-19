using KonturTestTask.Exceptions;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace KonturTests
{
    public class BaseTest
    {
        protected static XDocument LoadDocumentFromTestData(string documentName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream($"KonturTests.TestData.{documentName}");

            return XDocument.Load(stream);
        }

        protected static Stream GetEmbeddedResourceStream(string documentName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream($"KonturTests.TestData.{documentName}");
        }
    }
}
