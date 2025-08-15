using CommandLine;
using KonturTestTask.Exceptions;
using KonturTestTask.Extensions;
using KonturTestTask.Helpers;
using System.Xml;
using System.Xml.Linq;


namespace KonturTestTask
{
    internal class Program
    {

        static void Main(string[] args)
        {
            // Парсинг аргументов командной строки
            var parserResult = Parser.Default.ParseArguments<Options>(args);

            string inputXmlPath = parserResult.MapResult(
                parsedOptions => parsedOptions.InputFilePath, // Если аргументы валидны
                errors => null  // Если есть ошибки
            );

            if (string.IsNullOrEmpty(inputXmlPath))
            {
                return;
            }
            // запустим преобразование
            RunTransform(inputXmlPath);
        }

        /// <summary>
        /// Запуск преобразования
        /// </summary>
        /// <param name="inputXmlPath"></param>
        public static void RunTransform(string inputXmlPath)
        {
            try
            {
                // Проверка существования входного файла
                if (!File.Exists(inputXmlPath))
                {
                    throw new CustomException($"Входной XML файл не найден: {inputXmlPath}");
                }

                // TODO - добавить валидацию XSD

                // Получаем путь к корневой папке проекта/папка для результатов
                var projectRoot = AppContext.BaseDirectory;
                var outputXmlPath = Path.Combine(
                    projectRoot,
                    "..", "..", "..", // Поднимаемся из bin/Debug/netX.0
                    "Output",
                    "Employees.xml"
                );
                outputXmlPath = Path.GetFullPath(outputXmlPath);

                // Создаем выходную папку, если ее нет
                Directory.CreateDirectory(Path.GetDirectoryName(outputXmlPath));

                // создание XmlReader для inputXmlPath
                using (var inputXmlReader = XmlReader.Create(inputXmlPath))
                {
                    // создание XmlWriter для outputXmlPath
                    using (var outputXmlWriter = XmlWriter.Create(outputXmlPath))
                    {
                        // Выполнение XSLT-преобразования
                        XmlHelper.TransformXml(inputXmlReader, outputXmlWriter);
                    }
                }

                //обновление Employees.xml и на основе inputXmlPath обновить его (изначальный inputData.xml)
                var employeesDocument = XDocument.Load(outputXmlPath);
                var inputDataDocument = XDocument.Load(inputXmlPath);

                XmlHelper.UpdateEmployeesAndInputXml(inputDataDocument, employeesDocument);

                employeesDocument.Save(outputXmlPath);
                inputDataDocument.Save(inputXmlPath);

                // создание HTML отчета
                var hrmlFilePath = Path.Combine(
                    projectRoot,
                    "..", "..", "..", // Поднимаемся из bin/Debug/netX.0
                    "Output",
                    "Employees.html"
                );
                hrmlFilePath = Path.GetFullPath(hrmlFilePath);
                var htmlDoc = employeesDocument.CreateResultHtml();

                htmlDoc.Save(hrmlFilePath);

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
