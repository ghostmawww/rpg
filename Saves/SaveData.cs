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
        public List<string> MapRows { get; set; }
        public DateTime SaveTime { get; set; }

        public SaveData()
        {
            MapRows = new List<string>();
            SaveTime = DateTime.Now;
        }

        public SaveData(Person hero, char[,] map)
        {
            PlayerName = hero.NamePerson;
            PlayerHP = hero.HP;
            PlayerMaxHP = hero.MaxHP;
            PlayerStrength = hero.Strenght;
            PlayerCoins = hero.coin;
            WorldLevel = Map.levelWorld;
            MapRows = new List<string>();
            SaveTime = DateTime.Now;

            for (int i = 0; i < map.GetLength(0); i++)
            {
                string row = "";
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    row += map[i, j];
                }
                MapRows.Add(row);
            }
        }

        public char[,] GetMap()
        {
            int height = MapRows.Count;
            int width = MapRows[0].Length;
            char[,] map = new char[height, width];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    map[i, j] = MapRows[i][j];

            return map;
        }

        public static void Save(Person hero, char[,] map, string fileName)
        {
            if (!Directory.Exists("Saves"))
                Directory.CreateDirectory("Saves");

            SaveData data = new SaveData(hero, map);
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));

            using (StreamWriter writer = new StreamWriter($"Saves/{fileName}.xml"))
            {
                serializer.Serialize(writer, data);
            }

            Console.WriteLine($"Игра сохранена в файл: {fileName}.xml");
        }

        public static bool Load(string fileName, Person hero, char[,] map)
        {
            string path = $"Saves/{fileName}.xml";

            if (!File.Exists(path))
            {
                Console.WriteLine("Файл сохранения не найден!");
                return false;
            }

            try
            {
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

                    char[,] loadedMap = data.GetMap();
                    for (int i = 0; i < map.GetLength(0); i++)
                        for (int j = 0; j < map.GetLength(1); j++)
                            map[i, j] = loadedMap[i, j];

                    Console.WriteLine($"Игра загружена из файла: {fileName}.xml");
                    return true;
                }
            }
            catch
            {
                Console.WriteLine("Ошибка при загрузке файла!");
                return false;
            }
        }

        public static List<string> GetSaveList()
        {
            List<string> saves = new List<string>();

            if (Directory.Exists("Saves"))
            {
                string[] files = Directory.GetFiles("Saves", "*.xml");
                foreach (string file in files)
                {
                    saves.Add(Path.GetFileNameWithoutExtension(file));
                }
            }

            return saves;
        }
    }
}