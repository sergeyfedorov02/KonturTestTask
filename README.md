# KonturTestTask

## Как собрать и запустить приложение:

1. Клонировать репозиторий

```git clone https://github.com/sergeyfedorov02/KonturTestTask.git```

2. Перейти в рабочий каталог

```cd KonturTestTask```

3. Осуществить сборку проекта

```dotnet build --configuration=Release```

4. Перейти в каталог с исполняемым файлом

```cd KonturTestTask\bin\Release\net9.0\```

5. Запустить исполняемый файл с указанием параметров запуска

	- **Обязательные параметры:**
        - `-i`, `--input` - путь до входного файла в формате XML
            - Пример: `-i Input/Data3.xml`
    
    - **Необязательные параметры:**
        - `-o`, `--output` - путь до папки для результатов работы
            - Если не указан: создается папка `Output` в текущей директории
            - Файлы результатов: `Employees.xml` и `Employees.html`
	
    **Примеры запуска:**

    - Без указания выходной папки:
        ```
        KonturTestTask.exe -i Input/Data3.xml
        ```
    
    - С указанием выходной папки:
        ```
        KonturTestTask.exe -i Input/Data3.xml -o C:\Output
        ```

## Информация о проекте:

1. Требуется наличие .NET 9 SDK для того, чтобы построить и запустить приложение 

2. Юнит тесты реализованы в xUnit и находятся в проекте *KonturTests*

3. Тестовые XML-файлы находятся в папке *Input*

## Демонстрация работы

**Пример успешного запуска БЕЗ указания папки для результатов (выходной)**:

Изначальный файл **Data1.xml**

![data1 before](Images/data1_before.PNG)

Запуск приложения

![data1 without output](Images/data1_without_output.PNG)

Содержание папки с результатами

![result without output](Images/result_without_output.PNG)

Выходной XMl файл *Employees.xml*

![data1 without result xml](Images/data1_without_result_xml.PNG)

Выходной HTML файл *Employees.html*

![data1 without result html](Images/data1_without_result_html.PNG)

Измененный входной файл *Data1.xml*

![data1 after](Images/data1_after.PNG)

Изначальный файл **Data2.xml**

![data2 before](Images/data2_before.PNG)

Запуск приложения

![data2 without output](Images/data2_without_output.PNG)

Содержание папки с результатами

![result without output](Images/result_without_output.PNG)

Выходной XMl файл *Employees.xml*

![data2 without result xml](Images/data2_without_result_xml.PNG)

Выходной HTML файл *Employees.html*

![data2 without result html](Images/data2_without_result_html.PNG)

Измененный входной файл *Data2.xml*

![data2 after](Images/data2_after.PNG)


**Пример успешного запуска С указанием папки для результатов (выходной):**

Изначальный файл **Data3.xml**

![data3 before](Images/data3_before.PNG)

Запуск приложения

![data3 with args](Images/data3_with_args.PNG)

Содержание папки с результатами

![result with output](Images/result_with_output.PNG)

Выходной XMl файл *Employees.xml*

![data3 with result xml](Images/data3_with_result_xml.PNG)

Выходной HTML файл *Employees.html*

![data3 with result html](Images/data3_with_result_html.PNG)

Измененный входной файл *Data3.xml*

![data3 after](Images/data3_after.PNG)

**Примеры НЕуспешного запуска:**

Запуск приложения без указания каких-либо параметров

![without args](Images/without_args.PNG)

Указан неверный путь до входного файла

![invalid input file](Images/invalid_input_file.PNG)

Если во входном файле будет неверная структура

![invalid input struct](Images/invalid_input_struct.PNG)

Невозможно создать выходную папку (например, такое имя уже объявлено для файла)

![invalid output directory](Images/invalid_output_directory.PNG)
