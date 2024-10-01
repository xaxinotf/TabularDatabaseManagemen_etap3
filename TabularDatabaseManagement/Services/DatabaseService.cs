using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TabularDatabaseManagement.Models;
using Microsoft.Extensions.Logging;

namespace TabularDatabaseManagement.Services
{
    public class DatabaseService : IDisposable
    {
        private readonly ILogger<DatabaseService> _logger;
        private Database _database;

        public DatabaseService(ILogger<DatabaseService> logger)
        {
            _logger = logger;
            _database = new Database();

            try
            {
                LoadFromFile("database.json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка завантаження бази даних");
            }
        }

        public List<Table> GetTables()
        {
            return _database.Tables;
        }

        public Table GetTableByName(string name)
        {
            return _database.Tables.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void CreateTable(string name, List<Field> fields)
        {
            if (_database.Tables.Any(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new Exception("Таблиця з такою назвою вже існує.");

            var table = new Table { Name = name, Fields = fields };
            _database.Tables.Add(table);
        }

        public void AddRowToTable(string tableName, Dictionary<string, string> values)
        {
            var table = GetTableByName(tableName);
            if (table == null)
                throw new Exception("Таблиця не знайдена.");

            var row = new Row { Values = new Dictionary<string, object>() };

            foreach (var field in table.Fields)
            {
                if (!values.TryGetValue(field.Name, out string valueStr))
                    throw new Exception($"Значення для поля '{field.Name}' не задано.");

                var value = ConvertValue(valueStr, field.Type);
                row.Values[field.Name] = value;
            }

            table.Rows.Add(row);
        }



        public object ConvertValue(string valueStr, DataType type)
        {
            try
            {
                switch (type)
                {
                    case DataType.Integer:
                        return int.Parse(valueStr);
                    case DataType.Real:
                        return double.Parse(valueStr);
                    case DataType.Char:
                        if (valueStr.Length != 1)
                            throw new Exception("Значення типу Char має бути одним символом.");
                        return valueStr[0];
                    case DataType.String:
                        return valueStr;
                    case DataType.Date:
                        // Зберігаємо лише дату без часу
                        return DateTime.ParseExact(valueStr, "yyyy-MM-dd", null).Date;
                    case DataType.DateInterval:
                        // Розділяємо по " - " (пробіл-дефіс-пробіл)
                        var dates = valueStr.Split(new string[] { " - " }, StringSplitOptions.None);
                        if (dates.Length != 2)
                            throw new Exception($"Неправильний формат для інтервалу дати: {valueStr}. Використовуйте формат 'yyyy-MM-dd - yyyy-MM-dd'.");

                        var startDate = DateTime.ParseExact(dates[0].Trim(), "yyyy-MM-dd", null).Date;
                        var endDate = DateTime.ParseExact(dates[1].Trim(), "yyyy-MM-dd", null).Date;
                        if (startDate > endDate)
                            throw new Exception("Дата початку інтервалу не може бути пізнішою за дату завершення.");

                        return $"{startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd}";
                    default:
                        throw new Exception($"Невідомий тип даних: {type}");
                }
            }
            catch (FormatException fe)
            {
                throw new Exception($"Невірний формат значення: {fe.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка при конвертації значення: {ex.Message}");
            }
        }


        public void DifferenceTables(string tableName1, string tableName2, string resultTableName)
        {
            var table1 = GetTableByName(tableName1);
            var table2 = GetTableByName(tableName2);

            if (table1 == null || table2 == null)
                throw new Exception("Одна з таблиць не знайдена.");

            if (table1.Fields.Count != table2.Fields.Count ||
                !table1.Fields.Select(f => f.Type).SequenceEqual(table2.Fields.Select(f => f.Type)))
                throw new Exception("Таблиці мають різні структури.");

            var resultTable = new Table { Name = resultTableName, Fields = table1.Fields };

            // Додаємо рядки, які є в першій таблиці, але відсутні в другій
            foreach (var row1 in table1.Rows)
            {
                var found = table2.Rows.Any(row2 => row2.Values.SequenceEqual(row1.Values));
                if (!found)
                {
                    resultTable.Rows.Add(row1);
                }
            }

            _database.Tables.Add(resultTable);
        }

        public void DeleteTable(string name)
        {
            var table = GetTableByName(name);
            if (table == null)
                throw new Exception("Таблиця не знайдена.");

            _database.Tables.Remove(table);
        }

        public void SaveToFile(string filePath)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd" // Формат без часу
            };
            var json = JsonConvert.SerializeObject(_database, Formatting.Indented, jsonSettings);
            File.WriteAllText(filePath, json);
        }

        public void LoadFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                _database = JsonConvert.DeserializeObject<Database>(json);
            }
            else
            {
                _database = new Database();
            }
        }

        public void Dispose()
        {
            // Збереження бази даних при завершенні роботи
            try
            {
                SaveToFile("database.json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка збереження бази даних");
            }
        }
    }
}