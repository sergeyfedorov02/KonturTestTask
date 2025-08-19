using KonturTestTask.Exceptions;
using KonturTestTask.Extensions;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Xsl;

namespace KonturTestTask.Helpers
{
    internal static class XmlHelper
    {
        /// <summary>
        /// Преобразование XSLT
        /// </summary>
        /// <param name="inputXmlReader"></param>
        /// <param name="outputXmlWriter"></param>
        public static void TransformXml(XmlReader inputXmlReader, XmlWriter outputXmlWriter)
        {
            // создание и настройка XSLT преобразователя
            var xslt = new XslCompiledTransform();

            // загрузка XSLT схемы
            using var xsltReader = ResourceHelper.ReadTransformContent();
            xslt.Load(xsltReader);

            // выполнение преобразования
            xslt.Transform(inputXmlReader, outputXmlWriter);           
        }

        /// <summary>
        /// Обновление Employees.xml и inputData.xml (добавление атрибута totalSalary)
        /// </summary>
        /// <param name="inputDataDocument"></param>
        /// <param name="employeesDocument"></param>
        public static void UpdateEmployeesAndInputXml(XDocument inputDataDocument, XDocument employeesDocument)
        {
            // обновление Employees.xml и вычисление общего amount
            var totalSalary = employeesDocument.UpdateEmployees();

            // обновление inputData.xml
            inputDataDocument.UpdateInputData(totalSalary);
        }

        /// <summary>
        /// Загрузка и валидация документа (InputData.xml)
        /// </summary>
        /// <param name="inputXmlPath"></param>
        /// <returns></returns>
        /// <exception cref="CustomException"></exception>
        public static XDocument LoadDocumentAndValidate(string inputXmlPath)
        {
            // загрузка схемы
            var schemas = ResourceHelper.LoadSchemaSetFromResources();

            // загрузка документа
            var doc = XDocument.Load(inputXmlPath);

            // валидация документа 
            doc.ValidateDocument(schemas);

            return doc;
        }
    }
}
