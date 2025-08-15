namespace KonturTestTask.Helpers
{
    internal static class ReportHelper
    {
        public static void ReportOk(string outputXmlPath)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{Environment.NewLine}Преобразование прошло успешно, результат сохранен по следующему пути:");
            Console.ResetColor();
            Console.WriteLine($"{outputXmlPath}");
        }

        public static void ReportError(string errorMessage = null)
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
    }
}
