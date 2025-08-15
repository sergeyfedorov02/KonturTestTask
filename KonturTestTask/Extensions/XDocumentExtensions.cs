using System.Globalization;
using System.Xml.Linq;

namespace KonturTestTask.Extensions
{
    internal static class XDocumentExtensions
    {

        /// <summary>
        /// Обновление Employees.xml и вычисление общего amount
        /// </summary>
        /// <param name="employeesDocument"></param>
        /// <returns></returns>
        public static decimal UpdateEmployees(this XDocument employeesDocument)
        {
            // Получаем все элементы Employee
            var employees = employeesDocument.Descendants("Employee");

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
            }

            return totalSalary;
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
        /// Обновление inputData.xml
        /// </summary>
        /// <param name="inputDataDocument"></param>
        /// <param name="totalSalary"></param>
        public static void UpdateInputData(this XDocument inputDataDocument, decimal totalSalary)
        {
            // Получение корневого тега ()
            XElement payElement = inputDataDocument.Root;

            // Добавляем/обновляем атрибут totalSalary
            payElement.SetAttributeValue("totalSalary", totalSalary.ToString(CultureInfo.InvariantCulture));
        }
    }
}
