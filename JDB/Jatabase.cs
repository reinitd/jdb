using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JDB
{
    public class Jatabase
    {
        private string path;
        // private bool opened;
        private IEnumerable<string>? tables;

        public Jatabase(string dbpath, IEnumerable<string>? dbtables = null)
        {
            path = dbpath;
            // opened = true;
            tables = dbtables;

            if (!FileExists()) { SetupDB(); }
            else
            {
                string content = File.ReadAllText(path);
                if (content.Trim().Length == 0)
                {
                    Console.WriteLine("File is empty, setting up DB.");
                    SetupDB();
                }
            }
        }
        public async Task<bool> CreateTableAsync(string tableName)
        {
            if (!FileExists())
            {
                SetupDB();
            }

            string content = await File.ReadAllTextAsync(path);
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, Table>>(content);

            long timestamp = DateTime.Now.ToFileTimeUtc();
            Table table = new()
            {
                LastUpdated = timestamp,
                CreatedAt = timestamp,
                Data = null
            };

            jsonObject.Add(tableName, table);

            string jsonString = JsonConvert.SerializeObject(jsonObject);

            await File.WriteAllTextAsync(path, jsonString);

            return true;
        }

        public async Task<List<string>> GetTablesAsync()
        {
            if (File.Exists(path))
            {
                string content = await File.ReadAllTextAsync(path);
                var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, Table>>(content);
                tables = jsonObject?.Keys.ToList() ?? [];
                return (List<string>)tables;
            }
            return [];
        }
        public async Task<Table?> GetTableAsync(string tableName)
        {
            if (!FileExists())
            {
                SetupDB();
            }
            string content = await File.ReadAllTextAsync(path);
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, Table>>(content);

            if (jsonObject.TryGetValue(tableName, out Table? value))
            {
                return value;
            }
            return null;
        }
        public async Task<bool> UpdateTableAtKeyAsync(string tableName, string key, object data)
        {
            if (!FileExists())
            {
                SetupDB();
                Console.WriteLine("File not found, creating it.");
                return false;
            }

            Table? tableData = await GetTableAsync(tableName);
            if (tableData == null) { Console.WriteLine("Table data is null."); return false; }

            tableData.LastUpdated = DateTime.Now.ToFileTimeUtc();
            tableData.Data ??= [];

            tableData.Data[key] = data;

            bool updateRes = await UpdateTableAsync(tableName, tableData.Data);

            return updateRes;
        }
        public async Task<bool> UpdateTableAsync(string tableName, Dictionary<string, object>? data)
        {
            if (!FileExists())
            {
                SetupDB();
                Console.WriteLine("File not found, creating it.");
                return false;
            }

            Table? tableData = await GetTableAsync(tableName);
            if (tableData == null) { Console.WriteLine("Table data is null."); return false; }

            tableData.LastUpdated = DateTime.Now.ToFileTimeUtc();
            tableData.Data = data;

            string content = await File.ReadAllTextAsync(path);
            var fileData = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

            fileData[tableName] = tableData;

            await File.WriteAllTextAsync(
                path, JsonConvert.SerializeObject(fileData)
            );

            return true;
        }
        private bool FileExists()
        {
            return File.Exists(path);
        }
        private void SetupDB()
        {
            string? directory = Path.GetDirectoryName(path);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // if (!File.Exists(path))
            // {
                long timestamp = DateTime.Now.ToFileTimeUtc();
                Table table = new()
                {
                    LastUpdated = timestamp,
                    CreatedAt = timestamp,
                    Data = null
                };

                var jsonObject = new Dictionary<string, Table>();

                if (tables == null || !tables.Any())
                {

                    jsonObject["default"] = table;
                }
                else
                {
                    foreach (string tableName in tables)
                    {
                        jsonObject[tableName] = table;
                    }
                }

                string jsonString = JsonConvert.SerializeObject(jsonObject);
                File.WriteAllText(path, jsonString);
            // }
        }
    }
}
