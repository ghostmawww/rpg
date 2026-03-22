using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ConsoleApp46
{
    [Serializable]
    public class SaveData
    {
        public string PlayerName { get; set; }
        public int PlayerHP { get; set; }
        public int PlayerMaxHP { get; set; }
        public int PlayerStrength { get; set; }
        public int PlayerCoins { get; set; }
        public int WorldLevel { get; set; }
        public int PlayerX { get; set; }
        public int PlayerY { get; set; }
        public List<string> MapRows { get; set; }
        public DateTime SaveTime { get; set; }

        public SaveData()
        {
            try
            {
                MapRows = new List<string>();
                SaveTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                // Используем конструктор с 4 параметрами (message, errorCode, component, innerException)
                throw new GameException("Ошибка инициализации SaveData", "S001", "SaveSystem", ex);
            }
        }

        public SaveData(Person hero, char[,] map, int playerX, int playerY)
        {
            try
            {
                if (hero == null)
                    throw new GameException("Объект героя не инициализирован", "S002", "SaveSystem", ErrorSeverity.High);
                if (map == null)
                    throw new GameException("Карта не инициализирована", "S003", "SaveSystem", ErrorSeverity.High);

                PlayerName = hero.NamePerson ?? "Безымянный";
                PlayerHP = hero.HP;
                PlayerMaxHP = hero.MaxHP;
                PlayerStrength = hero.Strenght;
                PlayerCoins = hero.coin;
                WorldLevel = Map.levelWorld;
                PlayerX = playerX;
                PlayerY = playerY;
                MapRows = new List<string>();
                SaveTime = DateTime.Now;

                for (int i = 0; i < map.GetLength(0); i++)
                {
                    string row = "";
                    for (int j = 0; j < map.GetLength(1); j++)
                        row += map[i, j];
                    MapRows.Add(row);
                }
            }
            catch (GameException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // Используем конструктор с 4 параметрами (message, errorCode, component, innerException)
                throw new GameException("Ошибка при создании сохранения", "S004", "SaveSystem", ex);
            }
        }

        public char[,] GetMap()
        {
            try
            {
                if (MapRows == null || MapRows.Count == 0)
                    throw new GameException("Нет данных для восстановления карты", "S005", "SaveSystem", ErrorSeverity.High);

                int height = MapRows.Count;
                int width = MapRows[0].Length;
                char[,] map = new char[height, width];

                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                        map[i, j] = MapRows[i][j];

                return map;
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
                return null;
            }
        }

        public static void Save(Person hero, char[,] map, string fileName, int playerX, int playerY)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new GameException("Имя файла не может быть пустым", "S006", "SaveSystem", ErrorSeverity.Medium);

                if (!Directory.Exists("Saves"))
                    Directory.CreateDirectory("Saves");

                SaveData data = new SaveData(hero, map, playerX, playerY);
                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));

                using (StreamWriter writer = new StreamWriter($"Saves/{fileName}.xml"))
                {
                    serializer.Serialize(writer, data);
                }

                Console.WriteLine($"Игра сохранена в файл: {fileName}.xml");
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Нет прав для сохранения файла: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Ошибка ввода-вывода при сохранении: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неизвестная ошибка при сохранении: {ex.Message}");
            }
        }

        public static bool Load(string fileName, Person hero, char[,] map, ref int playerX, ref int playerY)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new GameException("Имя файла не может быть пустым", "S007", "SaveSystem", ErrorSeverity.Medium);
                if (hero == null)
                    throw new GameException("Объект героя не инициализирован", "S008", "SaveSystem", ErrorSeverity.High);
                if (map == null)
                    throw new GameException("Карта не инициализирована", "S009", "SaveSystem", ErrorSeverity.High);

                string path = $"Saves/{fileName}.xml";
                if (!File.Exists(path))
                {
                    Console.WriteLine("Файл сохранения не найден!");
                    return false;
                }

                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                using (StreamReader reader = new StreamReader(path))
                {
                    SaveData data = (SaveData)serializer.Deserialize(reader);

                    hero.NamePerson = data.PlayerName;
                    hero.HP = data.PlayerHP;
                    hero.MaxHP = data.PlayerMaxHP;
                    hero.Strenght = data.PlayerStrength;
                    hero.coin = data.PlayerCoins;
                    Map.levelWorld = data.WorldLevel;
                    playerX = data.PlayerX;
                    playerY = data.PlayerY;

                    char[,] loadedMap = data.GetMap();
                    for (int i = 0; i < map.GetLength(0); i++)
                        for (int j = 0; j < map.GetLength(1); j++)
                            map[i, j] = loadedMap[i, j];

                    Console.WriteLine($"Игра загружена из файла: {fileName}.xml");
                    return true;
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Файл не найден: {ex.Message}");
                return false;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Ошибка формата XML: {ex.Message}");
                return false;
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неизвестная ошибка при загрузке: {ex.Message}");
                return false;
            }
        }

        public static List<string> GetSaveList()
        {
            List<string> saves = new List<string>();

            try
            {
                if (Directory.Exists("Saves"))
                {
                    string[] files = Directory.GetFiles("Saves", "*.xml");
                    foreach (string file in files)
                        saves.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Нет доступа к папке сохранений: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Ошибка чтения папки сохранений: {ex.Message}");
            }

            return saves;
        }
    }
}