using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace KonturTestTask.Helpers
{
    internal static class XmlHelper
    {
        // настройки для записи XML
        private static readonly XmlWriterSettings XmlSettings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "\t",
            NewLineChars = "\n"
        };

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
        /// <param name="inputXmlPath"></param>
        /// <param name="outputXmlPath"></param>
        public static void UpdateEmployeesAndInputXml(string inputXmlPath, string outputXmlPath)
        {
            // обновление Employees.xml и вычисление общего amount
            var totalSalary = UpdateEmployees(outputXmlPath);

            // обновление inputData.xml
            UpdateInputData(inputXmlPath, totalSalary);
        }

        /// <summary>
        /// Обновление Employees.xml и вычисление общего amount
        /// </summary>
        /// <param name="employeesXmlPath"></param>
        /// <returns></returns>
        private static decimal UpdateEmployees(string employeesXmlPath)
        {
            // Загружаем XML документ
            var doc = XDocument.Load(employeesXmlPath);

            // Получаем все элементы Employee
            var employees = doc.Descendants("Employee");

            // Общее значение amount для всего Pay
            var totalSalary = 0m;

            foreach (var employee in employees)
            {
                // Вычисляем сумму amount
                var totalEmployeeSalary = employee.Elements("salary")
                    .Select(s => ParseSalary(s.Attribute("amount")?.Value))
                    .Sum();

                totalSalary += totalEmployeeSalary;

                // Добавляем/обновляем атрибут total-salary
                employee.SetAttributeValue("total-salary", totalEmployeeSalary.ToString(CultureInfo.InvariantCulture));

                // Сохраняем обновленный документ
                SaveXmlDocument(doc, employeesXmlPath);
            }

            return totalSalary;
        }

        /// <summary>
        /// Обновление inputData.xml
        /// </summary>
        /// <param name="inputXmlPath"></param>
        /// <param name="totalSalary"></param>
        private static void UpdateInputData(string inputXmlPath, decimal totalSalary) 
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\n"
            };

            XDocument doc = XDocument.Load(inputXmlPath);
            XElement payElement = doc.Root;

            // Добавляем/обновляем атрибут totalSalary
            payElement.SetAttributeValue("totalSalary", totalSalary.ToString(CultureInfo.InvariantCulture));

            // Сохраняем обновленный документ
            SaveXmlDocument(doc, inputXmlPath);
        }

        /// <summary>
        /// Парсинг чисел в атрибуте amount
        /// </summary>
        /// <param name="amountValue"></param>
        /// <returns></returns>
        private static decimal ParseSalary(string amountValue)
        {
            if (string.IsNullOrEmpty(amountValue))
            {
                return 0m;
            }

            // Заменяем запятую на точку для унификации
            var normalized = amountValue.Replace(',', '.');

            if (decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }
                
            return 0m;
        }

        /// <summary>
        /// Сохранение обновленного документа с настройками из XmlSettings
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="filePath"></param>
        private static void SaveXmlDocument(XDocument doc, string filePath)
        {
            using (var writer = XmlWriter.Create(filePath, XmlSettings))
            {
                doc.Save(writer);
            }
        }
    }
}
