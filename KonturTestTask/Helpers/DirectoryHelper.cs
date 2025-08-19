using KonturTestTask.Exceptions;

namespace KonturTestTask.Helpers
{
    internal static class DirectoryHelper
    {
        /// <summary>
        /// Формирование выходной директории на основании аргумента командной строки
        /// </summary>
        /// <param name="outputDir"></param>
        /// <returns></returns>
        /// <exception cref="CustomException"></exception>
        public static string GetValidOutputDirectory(string outputDir)
        {
            var fullPathOutputDir = Path.GetFullPath(outputDir);
            if (!Directory.Exists(fullPathOutputDir))
            {
                try
                {
                    Directory.CreateDirectory(fullPathOutputDir);
                }
                catch
                {
                    throw new CustomException($"Невозможно создать папку {fullPathOutputDir}");
                }

            }

            return fullPathOutputDir;
        }
    }
}
