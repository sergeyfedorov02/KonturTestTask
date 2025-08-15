using CommandLine;
using KonturTestTask.Exceptions;
using KonturTestTask.Helpers;
using System.IO;
using System.Xml;


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

                // создание XmlReader для inputXmlPath
                using var inputXmlReader = XmlReader.Create(inputXmlPath);

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

                // создание XmlWriter для outputXmlPath
                using var outputXmlWriter = XmlWriter.Create(outputXmlPath);

                // Выполнение XSLT-преобразования
                XmlHelper.TransformXml(inputXmlReader, outputXmlWriter);
                
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
