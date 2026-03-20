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

            // Создаем большую карту
            char[,] fullMap = Map.CreateFullMap();

            // Начальная позиция игрока
            int playerX = fullMap.GetLength(0) / 2;
            int playerY = fullMap.GetLength(1) / 2;

            // Переменные для пещеры
            bool inCave = false;
            char[,] caveMap = null;
            int cavePlayerX = 0, cavePlayerY = 0;

            // Переменные для Титаника
            bool inTitanic = false;
            char[,] titanicMap = null;
            int titanicPlayerX = 0, titanicPlayerY = 0;

            // Переменные для домика Бабы Яги
            bool inHut = false;
            char[,] hutMap = null;
            int hutPlayerX = 0, hutPlayerY = 0;

            // Очищаем область вокруг игрока
            for (int i = playerX - 5; i <= playerX + 5; i++)
            {
                for (int j = playerY - 5; j <= playerY + 5; j++)
                {
                    if (i >= 0 && i < fullMap.GetLength(0) && j >= 0 && j < fullMap.GetLength(1))
                    {
                        fullMap[i, j] = '.';
                    }
                }
            }

            // Ставим игрока
            fullMap[playerX, playerY] = '@';

            while (hero.HP > 0)
            {
                // Отображение карты
                if (!inCave && !inTitanic && !inHut)
                {
                    Map.GetMap(fullMap, playerX, playerY);
                }
                else if (inCave)
                {
                    Console.Clear();
                    for (int i = 0; i < 25; i++)
                    {
                        for (int j = 0; j < 25; j++)
                        {
                            char cell = caveMap[i, j];
                            if (cell == '@')
                            {
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.Write(cell + " ");
                                Console.ResetColor();
                            }
                            else if (cell == 'O')
                            {
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.Write(cell + " ");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.Write(cell + " ");
                            }
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine("\n=== ПЕЩЕРА ===");
                }
                else if (inTitanic)
                {
                    Console.Clear();
                    for (int i = 0; i < 25; i++)
                    {
                        for (int j = 0; j < 25; j++)
                        {
                            char cell = titanicMap[i, j];
                            if (cell == '@')
                            {
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.Write(cell + " ");
                                Console.ResetColor();
                            }
                            else if (cell == 'T')
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write(cell + " ");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.Write(cell + " ");
                            }
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine("\n=== ТИТАНИК ===");
                }
                else if (inHut)
                {
                    Console.Clear();
                    for (int i = 0; i < 25; i++)
                    {
                        for (int j = 0; j < 25; j++)
                        {
                            char cell = hutMap[i, j];
                            if (cell == '@')
                            {
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.Write(cell + " ");
                                Console.ResetColor();
                            }
                            else if (cell == 'F')
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write(cell + " ");
                                Console.ResetColor();
                            }
                            else if (cell == '%')
                            {
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.Write(cell + " ");
                                Console.ResetColor();
                            }
                            else if (cell == '&')
                            {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.Write(cell + " ");
                                Console.ResetColor();
                            }
                            else if (cell == 'H')
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(cell + " ");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.Write(cell + " ");
                            }
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine("\n=== ДОМИК БАБЫ ЯГИ ===");
                }

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
                        SaveData.Save(hero, fullMap, saveName, playerX, playerY);
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
                            SaveData.Load(saves[choice - 1], hero, fullMap, ref playerX, ref playerY);
                        }
                    }
                    Console.WriteLine("Нажмите любую клавишу...");
                    Console.ReadKey();
                }
                else
                {
                    int dx = 0, dy = 0;
                    switch (key)
                    {
                        case ConsoleKey.UpArrow: dx = -1; dy = 0; break;
                        case ConsoleKey.DownArrow: dx = 1; dy = 0; break;
                        case ConsoleKey.LeftArrow: dx = 0; dy = -1; break;
                        case ConsoleKey.RightArrow: dx = 0; dy = 1; break;
                        default: continue;
                    }

                    if (inCave)
                    {
                        Map.MoveInCave(ref cavePlayerX, ref cavePlayerY, dx, dy, caveMap, ref inCave);
                    }
                    else if (inTitanic)
                    {
                        Map.MoveInTitanic(ref titanicPlayerX, ref titanicPlayerY, dx, dy, titanicMap, ref inTitanic);
                    }
                    else if (inHut)
                    {
                        Map.MoveInHut(ref hutPlayerX, ref hutPlayerY, dx, dy, hutMap, ref inHut);
                    }
                    else
                    {
                        Map.MovePlayer(ref playerX, ref playerY, dx, dy, fullMap, hero,
                            ref inCave, ref caveMap, ref cavePlayerX, ref cavePlayerY,
                            ref inTitanic, ref titanicMap, ref titanicPlayerX, ref titanicPlayerY,
                            ref inHut, ref hutMap, ref hutPlayerX, ref hutPlayerY);
                    }
                }
            }

            Console.Clear();
            Console.WriteLine("ИГРА ОКОНЧЕНА!");
            Console.WriteLine($"Герой {hero.NamePerson} погиб...");
            Console.WriteLine($"Достигнутый уровень: {Map.levelWorld}");
            Console.WriteLine($"Собрано монет: {hero.coin}");
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}