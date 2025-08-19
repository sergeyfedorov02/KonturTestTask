using FluentAssertions;
using KonturTestTask.Exceptions;
using KonturTestTask.Helpers;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace KonturTests
{
    public class TransformXmlTest : BaseTest
    {
        private static XDocument TransformDocument(string documentName)
        {
            using var inputXmlReader = XmlReader.Create(GetEmbeddedResourceStream(documentName));
            // создание XmlWriter для outputXmlPath

            using var m = new MemoryStream();
            using var outputXmlWriter = XmlWriter.Create(m);

            // Выполнение XSLT-преобразования
            XmlHelper.TransformXml(inputXmlReader, outputXmlWriter);

            m.Seek(0, SeekOrigin.Begin);
            return XDocument.Load(m);
        }

        [Fact]
        public void TransformTest()
        {
            var doc = TransformDocument("Data1.xml");

            // проверка структуры выходного Xml
            var employees = doc.XPathSelectElements("/Employees/Employee").ToList();
            employees.Should().HaveCount(2); // 2 элемента Employee

            // Проверка первого Employee (при помощи Attribute)
            var lena = employees[0];
            lena.Attribute("name").Value.Should().Be("Lena");
            lena.Attribute("surname").Value.Should().Be("Ivanova");

            var lenaSalaries = lena.Elements("salary").ToList();
            lenaSalaries.Should().HaveCount(3);
            lenaSalaries[0].Attribute("amount").Value.Should().Be("1001.1");
            lenaSalaries[0].Attribute("mount").Value.Should().Be("march");
            lenaSalaries[1].Attribute("amount").Value.Should().Be("2001");
            lenaSalaries[1].Attribute("mount").Value.Should().Be("january");
            lenaSalaries[2].Attribute("amount").Value.Should().Be("3001,10");
            lenaSalaries[2].Attribute("mount").Value.Should().Be("february");

            // Проверка второго Employee (при помощи XPath)
            var masha = employees.FirstOrDefault(e =>
                ((string)e.XPathEvaluate("string(@name)")) == "Masha");
            masha.Should().NotBeNull();

            ((string)masha.XPathEvaluate("string(@surname)")).Should().Be("Ivanova");

            var mashaSalaries = ((IEnumerable<object>)masha.XPathEvaluate("salary")).Cast<XElement>().ToList();
            mashaSalaries.Should().HaveCount(3);

            ((string)mashaSalaries[0].XPathEvaluate("string(@amount)")).Should().Be("1000");
            ((string)mashaSalaries[0].XPathEvaluate("string(@mount)")).Should().Be("march");

            ((string)mashaSalaries[1].XPathEvaluate("string(@amount)")).Should().Be("2000.0");
            ((string)mashaSalaries[1].XPathEvaluate("string(@mount)")).Should().Be("january");

            ((string)mashaSalaries[2].XPathEvaluate("string(@amount)")).Should().Be("3000");
            ((string)mashaSalaries[2].XPathEvaluate("string(@mount)")).Should().Be("february");
        }

        [Fact]
        public void UpdateTotalSalaryTest()
        {
            var resultDocument = TransformDocument("Data1.xml");

            var sourceDoc = LoadDocumentFromTestData("Data1.xml");

            XmlHelper.UpdateEmployeesAndInputXml(sourceDoc, resultDocument);

            // проверяем, что после преобразования появился атрибут total-salary
            sourceDoc.Root.Attribute("total-salary").Value.Should().Be("12003.20");

            var employees = resultDocument.XPathSelectElements("/Employees/Employee").ToList();
            employees.Should().HaveCount(2);

            var lena = employees[0];
            lena.Attribute("total-salary").Value.Should().Be("6003.20");

            var masha = employees[1];
            masha.Attribute("total-salary").Value.Should().Be("6000.0");
        }
    }
}
