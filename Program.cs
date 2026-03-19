using System;
using System.Collections.Generic;

namespace ConsoleApp46
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            
            Console.SetWindowSize(60, 35);

            Console.WriteLine("Введите имя героя:");
            string heroName = Console.ReadLine();
            Person hero = new Person(100, heroName);

            char[,] map = new char[25, 25];
            Map.Array(map);

            while (hero.HP > 0)
            {
                
                Console.SetCursorPosition(0, 0);

                
                Map.GetMap(map);

                
                Console.WriteLine();
                
                Person.GetCharacter(hero);
                
                Console.WriteLine("S - сохранить | L - загрузить");
                

                
                ConsoleKey key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.S)
                {
                    Console.WriteLine("Введите имя для сохранения: ");
                    string saveName = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(saveName))
                    {
                        SaveData.Save(hero, map, saveName);
                    }
                    Console.WriteLine("Нажмите любую клавишу...");
                    Console.ReadKey();
                }
                else if (key == ConsoleKey.L)
                {
                    List<string> saves = SaveData.GetSaveList();

                    if (saves.Count == 0)
                    {
                        Console.WriteLine("Нет сохранений!");
                    }
                    else
                    {
                        Console.WriteLine("Доступные сохранения:");
                        for (int i = 0; i < saves.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {saves[i]}");
                        }
                        Console.Write("Выберите номер: ");

                        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= saves.Count)
                        {
                            SaveData.Load(saves[choice - 1], hero, map);
                        }
                    }
                    Console.WriteLine("Нажмите любую клавишу...");
                    Console.ReadKey();
                }
                else
                {
                    
                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                            if (Map.GetIvent(hero, map, -1, 0))
                                Map.UpArray(map);
                            break;
                        case ConsoleKey.DownArrow:
                            if (Map.GetIvent(hero, map, 1, 0))
                                Map.DownArray(map);
                            break;
                        case ConsoleKey.LeftArrow:
                            if (Map.GetIvent(hero, map, 0, -1))
                                Map.LeftArray(map);
                            break;
                        case ConsoleKey.RightArrow:
                            if (Map.GetIvent(hero, map, 0, 1))
                                Map.RightArray(map);
                            break;
                    }
                    
                }
            }
        }
    }
}