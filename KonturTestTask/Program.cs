using CommandLine;
using KonturTestTask.Exceptions;
using KonturTestTask.Extensions;
using KonturTestTask.Helpers;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;


namespace KonturTestTask
{
    internal class Program
    {

        static void Main(string[] args)
        {
            // Парсинг аргументов командной строки
            var parserResult = Parser.Default.ParseArguments<Options>(args);

            parserResult.WithParsed(options =>
            {
                if (string.IsNullOrEmpty(options.InputFilePath))
                {
                    return;
                }

                // запустим преобразование
                RunTransform(options.InputFilePath, options.OutputFilePath);
            });

            parserResult.WithNotParsed(errors =>
            {
                ReportHelper.ReportError("Ошибка в параметрах командной строки");
            });
        }

        /// <summary>
        /// Запуск преобразования
        /// </summary>
        /// <param name="inputXmlPath"></param>
        public static void RunTransform(string inputXmlPath, string outputDirectory)
        {
            try
            {
                // Проверка существования входного файла
                if (!File.Exists(inputXmlPath))
                {
                    throw new CustomException($"Входной XML файл не найден: {inputXmlPath}");
                }

                // Определяем выходную директорию
                var outputDir = DirectoryHelper.GetValidOutputDirectory(outputDirectory);

                // Валидация при помощи XSD
                var inputDataDocument = XmlHelper.LoadDocumentAndValidate(inputXmlPath);

                // Сформируем путь для выходных файлов
                var outputXmlPath = Path.Combine(outputDir, "Employees.xml");
                var htmlFilePath = Path.Combine(outputDir, "Employees.html");

                // создание XmlReader для inputXmlPath
                using (var inputXmlReader = XmlReader.Create(inputXmlPath))
                {
                    // создание XmlWriter для outputXmlPath
                    using var outputXmlWriter = XmlWriter.Create(outputXmlPath);
                    // Выполнение XSLT-преобразования
                    XmlHelper.TransformXml(inputXmlReader, outputXmlWriter);
                }

                //обновление Employees.xml и на основе inputXmlPath обновить его (изначальный inputData.xml)
                var employeesDocument = XDocument.Load(outputXmlPath);

                XmlHelper.UpdateEmployeesAndInputXml(inputDataDocument, employeesDocument);

                employeesDocument.Save(outputXmlPath);
                inputDataDocument.Save(inputXmlPath);

                // создание HTML отчета
                var htmlDoc = employeesDocument.CreateResultHtml();

                htmlDoc.Save(htmlFilePath);

                ReportHelper.ReportOk(outputXmlPath);
            }
            catch(CustomException ex)
            {
                ReportHelper.ReportError(ex.Message);
            }
            catch (Exception)
            {
                ReportHelper.ReportError();
            }
        }
    }
}
