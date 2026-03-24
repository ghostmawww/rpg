using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ConsoleApp46
{
    /// <summary>
    /// Класс для сохранения и загрузки данных игры
    /// </summary>
    [Serializable]
    public class SaveData
    {
        private string _playerName;
        private int _playerHp;
        private int _playerMaxHp;
        private int _playerStrength;
        private int _playerCoins;
        private int _worldLevel;
        private int _playerX;
        private int _playerY;
        private List<string> _mapRows;
        private DateTime _saveTime;

        /// <summary>Имя игрока</summary>
        public string PlayerName
        {
            get => _playerName;
            set => _playerName = value;
        }

        /// <summary>Текущее здоровье игрока</summary>
        public int PlayerHP
        {
            get => _playerHp;
            set => _playerHp = value;
        }

        /// <summary>Максимальное здоровье игрока</summary>
        public int PlayerMaxHP
        {
            get => _playerMaxHp;
            set => _playerMaxHp = value;
        }

        /// <summary>Сила игрока</summary>
        public int PlayerStrength
        {
            get => _playerStrength;
            set => _playerStrength = value;
        }

        /// <summary>Количество монет игрока</summary>
        public int PlayerCoins
        {
            get => _playerCoins;
            set => _playerCoins = value;
        }

        /// <summary>Уровень мира</summary>
        public int WorldLevel
        {
            get => _worldLevel;
            set => _worldLevel = value;
        }

        /// <summary>Координата X игрока</summary>
        public int PlayerX
        {
            get => _playerX;
            set => _playerX = value;
        }

        /// <summary>Координата Y игрока</summary>
        public int PlayerY
        {
            get => _playerY;
            set => _playerY = value;
        }

        /// <summary>Строки карты</summary>
        public List<string> MapRows
        {
            get => _mapRows;
            set => _mapRows = value;
        }

        /// <summary>Время сохранения</summary>
        public DateTime SaveTime
        {
            get => _saveTime;
            set => _saveTime = value;
        }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public SaveData()
        {
            try
            {
                _mapRows = new List<string>();
                _saveTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                throw new GameException("Ошибка инициализации SaveData", "S001", "SaveSystem", ex);
            }
        }

        /// <summary>
        /// Конструктор для создания сохранения
        /// </summary>
        /// <param name="hero">Объект героя</param>
        /// <param name="map">Карта мира</param>
        /// <param name="playerX">Координата X игрока</param>
        /// <param name="playerY">Координата Y игрока</param>
        public SaveData(Person hero, char[,] map, int playerX, int playerY)
        {
            try
            {
                if (hero == null)
                {
                    throw new GameException("Объект героя не инициализирован", "S002", "SaveSystem", ErrorSeverity.High);
                }

                if (map == null)
                {
                    throw new GameException("Карта не инициализирована", "S003", "SaveSystem", ErrorSeverity.High);
                }

                _playerName = hero.Name ?? "Безымянный";
                _playerHp = hero.HP;
                _playerMaxHp = hero.MaxHP;
                _playerStrength = hero.Strength;
                _playerCoins = hero.Coins;
                _worldLevel = Map.LevelWorld;
                _playerX = playerX;
                _playerY = playerY;
                _mapRows = new List<string>();
                _saveTime = DateTime.Now;

                for (int i = 0; i < map.GetLength(0); i++)
                {
                    string row = string.Empty;
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        row += map[i, j];
                    }
                    _mapRows.Add(row);
                }
            }
            catch (GameException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new GameException("Ошибка при создании сохранения", "S004", "SaveSystem", ex);
            }
        }

        /// <summary>
        /// Восстанавливает карту из сохраненных данных
        /// </summary>
        /// <returns>Двумерный массив символов карты</returns>
        public char[,] GetMap()
        {
            try
            {
                if (_mapRows == null || _mapRows.Count == 0)
                {
                    throw new GameException("Нет данных для восстановления карты", "S005", "SaveSystem", ErrorSeverity.High);
                }

                int height = _mapRows.Count;
                int width = _mapRows[0].Length;
                char[,] map = new char[height, width];

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        map[i, j] = _mapRows[i][j];
                    }
                }

                return map;
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
                return null;
            }
        }

        /// <summary>
        /// Сохраняет игру в файл
        /// </summary>
        /// <param name="hero">Объект героя</param>
        /// <param name="map">Карта мира</param>
        /// <param name="fileName">Имя файла сохранения</param>
        /// <param name="playerX">Координата X игрока</param>
        /// <param name="playerY">Координата Y игрока</param>
        public static void Save(Person hero, char[,] map, string fileName, int playerX, int playerY)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    throw new GameException("Имя файла не может быть пустым", "S006", "SaveSystem", ErrorSeverity.Medium);
                }

                if (!Directory.Exists("Saves"))
                {
                    Directory.CreateDirectory("Saves");
                }

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

        /// <summary>
        /// Загружает игру из файла
        /// </summary>
        /// <param name="fileName">Имя файла сохранения</param>
        /// <param name="hero">Объект героя для восстановления</param>
        /// <param name="map">Карта для восстановления</param>
        /// <param name="playerX">Координата X игрока</param>
        /// <param name="playerY">Координата Y игрока</param>
        /// <returns>true, если загрузка успешна</returns>
        public static bool Load(string fileName, Person hero, char[,] map, ref int playerX, ref int playerY)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    throw new GameException("Имя файла не может быть пустым", "S007", "SaveSystem", ErrorSeverity.Medium);
                }

                if (hero == null)
                {
                    throw new GameException("Объект героя не инициализирован", "S008", "SaveSystem", ErrorSeverity.High);
                }

                if (map == null)
                {
                    throw new GameException("Карта не инициализирована", "S009", "SaveSystem", ErrorSeverity.High);
                }

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

                    hero.Name = data.PlayerName;
                    hero.HP = data.PlayerHP;
                    hero.MaxHP = data.PlayerMaxHP;
                    hero.Strength = data.PlayerStrength;
                    hero.Coins = data.PlayerCoins;
                    Map.LevelWorld = data.WorldLevel;
                    playerX = data.PlayerX;
                    playerY = data.PlayerY;

                    char[,] loadedMap = data.GetMap();
                    for (int i = 0; i < map.GetLength(0); i++)
                    {
                        for (int j = 0; j < map.GetLength(1); j++)
                        {
                            map[i, j] = loadedMap[i, j];
                        }
                    }

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

        /// <summary>
        /// Получает список всех сохранений
        /// </summary>
        /// <returns>Список имен файлов сохранений</returns>
        public static List<string> GetSaveList()
        {
            List<string> saves = new List<string>();

            try
            {
                if (Directory.Exists("Saves"))
                {
                    string[] files = Directory.GetFiles("Saves", "*.xml");
                    foreach (string file in files)
                    {
                        saves.Add(Path.GetFileNameWithoutExtension(file));
                    }
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