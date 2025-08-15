using System.Globalization;
using System.Xml.Linq;

namespace KonturTestTask.Extensions
{
    internal static class XDocumentExtensions
    {

        /// <summary>
        /// Обновление документа employeesDocument и вычисление общего amount
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
        /// Обновление документа inputData.xml
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

        /// <summary>
        /// Формирование HTML отчета по Employees
        /// </summary>
        /// <param name="employeesDocument"></param>
        /// <returns></returns>
        public static XDocument CreateResultHtml(this XDocument employeesDocument)
        {
            var htmlDoc = new XDocument(
                new XDocumentType("html", null, null, null),
                new XElement("html",
                    new XElement("head",
                        new XElement("meta", new XAttribute("charset", "utf-8")),
                        new XElement("title", "Отчет по сотрудникам"),
                        new XElement("style",
                            @"table {
                                border-collapse: collapse;
                                width: 100%;
                                margin-top: 20px;
                              }
                              th, td {
                                border: 1px solid #ddd;
                                padding: 8px;
                                text-align: left;
                              }
                              th {
                                background-color: #f2f2f2;
                                position: sticky;
                                top: 0;
                              }
                              .total-row {
                                font-weight: bold;
                                background-color: #f9f9f9;
                              }"
                        )
                    ),
                    new XElement("body",
                        new XElement("h1", "Отчет по сотрудникам"),
                        new XElement("table",

                            // Формирование заголовка
                            new XElement("thead",
                                new XElement("tr",
                                    new XElement("th", new XAttribute("rowspan", "2"), "Имя"),
                                    new XElement("th", new XAttribute("rowspan", "2"), "Фамилия"),
                                    new XElement("th", new XAttribute("colspan", "2"), "Выплаты по месяцам")
                                ),
                                new XElement("tr",
                                    new XElement("th", "Месяц"),
                                    new XElement("th", "Сумма")
                                )
                            ),

                            // Формирование записей для каждого Employee
                            new XElement("tbody",
                                employeesDocument.Root.Elements("Employee")
                                    .Select(employee => {

                                        // Группируем выплаты по месяцам и суммируем
                                        var mountPayments = employee.Elements("salary")
                                            .GroupBy(salary => salary.Attribute("mount")?.Value?.ToLower())
                                            .Select(g => new {
                                                Mount = g.Key,
                                                TotalAmount = g.Sum(s => ParseSalary(s.Attribute("amount")?.Value))
                                            }).ToList();

                                        // Получим количество уникальных месяцев с выплатами для сотрудников
                                        var paymentCount = mountPayments.Count;

                                        // Получим первую запись о выплате
                                        var firstPayment = mountPayments.FirstOrDefault();

                                        // Создание основной строки таблицы
                                        return new XElement("tr",

                                            // Добавляем ячейку с именем
                                            new XElement("td",
                                                new XAttribute("rowspan", paymentCount > 0 ? paymentCount.ToString() : "1"),
                                                employee.Attribute("name")?.Value
                                            ),

                                            // Добавляем ячейку с фамилией
                                            new XElement("td",
                                                new XAttribute("rowspan", paymentCount > 0 ? paymentCount.ToString() : "1"),
                                                employee.Attribute("surname")?.Value
                                            ),

                                            // Добавляем данных о первой выплате 
                                            firstPayment != null ?
                                                new XElement("td", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(firstPayment.Mount ?? ""))
                                                : null,
                                            firstPayment != null ?
                                                new XElement("td", firstPayment.TotalAmount.ToString(CultureInfo.InvariantCulture))
                                                : null,

                                            // Добавляем остальные месяцы(здесь новая строка, но без имени и фамилии)
                                            mountPayments.Skip(1).Select(payment =>
                                                new XElement("tr",
                                                    new XElement("td", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(payment.Mount ?? "")),
                                                    new XElement("td", payment.TotalAmount.ToString(CultureInfo.InvariantCulture))
                                                )
                                            )
                                        );
                                    })
                            )
                        ),

                        // Добавляем время создания отчета
                        new XElement("div",
                            new XAttribute("style", "margin-top: 20px; font-style: italic;"),
                            $"Отчет сформирован {DateTime.Now:yyyy-MM-dd HH:mm}"
                        )
                    )
                )
            );

            return htmlDoc;
        }
    }
}
