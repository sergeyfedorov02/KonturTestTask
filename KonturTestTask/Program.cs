using CommandLine;
using KonturTestTask.Exceptions;


namespace KonturTestTask
{
    internal class Program
    {

        /// <summary>
        /// Аргументы командной строки
        /// </summary>
        public class Options
        {
            [Option('i', "input", Required = true, HelpText = "Полный путь к входному XML файлу")]
            public string InputFilePath { get; set; }
        }

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

        private static void ReportError(string errorMessage = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Console.WriteLine("Ошибка:");
                Console.WriteLine(errorMessage);
            }
            else
            {
                Console.WriteLine("Возникла непредвиденная ошибка");
            }
            Console.ResetColor();
        }

        private static void ReportOk(string outputXmlPath)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{Environment.NewLine}Преобразование прошло успешно, результат сохранен по следующему пути:");
            Console.ResetColor();
            Console.WriteLine($"{outputXmlPath}");
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

                // Выполнение XSLT-преобразования
                XmlHelper.TransformXml(inputXmlPath, outputXmlPath);
                
                ReportOk(outputXmlPath);
            }
            catch(CustomException ex)
            {
                ReportError(ex.Message);
            }
            catch (Exception)
            {
                ReportError();
            }
        }
    }
}
