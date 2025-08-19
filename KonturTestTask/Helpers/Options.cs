using CommandLine;

namespace KonturTestTask.Helpers
{
    /// <summary>
    /// Аргументы командной строки
    /// </summary>
    internal class Options
    {
        [Option('i', "input", Required = true, HelpText = "Полный путь к входному XML файлу")]
        public string InputFilePath { get; set; }

        // опционально указать выходную директорию
        [Option('o', "output", Required = false, Default="Output", HelpText = "Полный путь к директории для выходных файлов")]
        public string OutputFilePath { get; set; }
    }

}
