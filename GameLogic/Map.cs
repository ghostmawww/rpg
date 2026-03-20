using System;
using System.Collections.Generic;

namespace ConsoleApp46
{
    public class Map
    {
        private static char[,] groundTypes;
        static public int levelWorld = 1;
        static Random rnd = new Random();

        // Константы для размеров карты
        private const int MAP_WIDTH = 1500;
        private const int MAP_HEIGHT = 1500;
        private const int VIEW_WIDTH = 25;
        private const int VIEW_HEIGHT = 25;

        static public void GetMap(char[,] fullMap, int playerWorldX, int playerWorldY)
        {
            Console.Clear();

            int startX = playerWorldX - VIEW_WIDTH / 2;
            int startY = playerWorldY - VIEW_HEIGHT / 2;

            startX = Math.Max(0, Math.Min(startX, MAP_HEIGHT - VIEW_HEIGHT));
            startY = Math.Max(0, Math.Min(startY, MAP_WIDTH - VIEW_WIDTH));

            for (int i = 0; i < VIEW_HEIGHT; i++)
            {
                for (int j = 0; j < VIEW_WIDTH; j++)
                {
                    int mapX = startX + i;
                    int mapY = startY + j;

                    char cell = fullMap[mapX, mapY];

                    if (cell == '0')
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(cell + " ");
                        Console.ResetColor();
                    }
                    else if (cell == '&')
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(cell + " ");
                        Console.ResetColor();
                    }
                    else if (cell == 'H')
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(cell + " ");
                        Console.ResetColor();
                    }
                    else if (cell == '+')
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(cell + " ");
                        Console.ResetColor();
                    }
                    else if (cell == '%')
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(cell + " ");
                        Console.ResetColor();
                    }
                    else if (cell == '@')
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(cell + " ");
                        Console.ResetColor();
                    }
                    else if (cell == '^')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(cell + " ");
                        Console.ResetColor();
                    }
                    else if (cell == '~')
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(cell + " ");
                        Console.ResetColor();
                    }
                    else if (cell == '#')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(cell + " ");
                        Console.ResetColor();
                    }
                    else if (cell == 'O')
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write(cell + " ");
                        Console.ResetColor();
                    }
                    else if (cell == 'T')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write(cell + " ");
                        Console.ResetColor();
                    }
                    else if (cell == 'F')
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
        }

        static public char[,] CreateFullMap()
        {
            char[,] fullMap = new char[MAP_HEIGHT, MAP_WIDTH];
            groundTypes = new char[MAP_HEIGHT, MAP_WIDTH];

            for (int i = 0; i < MAP_HEIGHT; i++)
            {
                for (int j = 0; j < MAP_WIDTH; j++)
                {
                    fullMap[i, j] = '.';
                    groundTypes[i, j] = '.';
                }
            }

            // Генерация рек
            for (int r = 0; r < 1600; r++)
            {
                GenerateRiver(fullMap);
            }

            // Генерация лесов
            for (int f = 0; f < 2000; f++)
            {
                GenerateForest(fullMap);
            }

            // Генерация гор
            for (int g = 0; g < 2700; g++)
            {
                int x = rnd.Next(20, MAP_HEIGHT - 20);
                int y = rnd.Next(20, MAP_WIDTH - 20);
                CreateMountain(fullMap, x, y);
            }

            // Входы в пещеры (рядом с горами)
            int cavePortalsPlaced = 0;
            int caveAttempts = 0;

            while (cavePortalsPlaced < 800 && caveAttempts < 200000)
            {
                int x = rnd.Next(5, MAP_HEIGHT - 5);
                int y = rnd.Next(5, MAP_WIDTH - 5);
                caveAttempts++;

                if (fullMap[x, y] != '.')
                    continue;

                bool hasMountainNearby = false;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        int nx = x + dx;
                        int ny = y + dy;

                        if (nx >= 0 && nx < MAP_HEIGHT && ny >= 0 && ny < MAP_WIDTH)
                        {
                            if (fullMap[nx, ny] == '^')
                            {
                                hasMountainNearby = true;
                                break;
                            }
                        }
                    }
                    if (hasMountainNearby) break;
                }

                if (hasMountainNearby)
                {
                    fullMap[x, y] = 'O';
                    groundTypes[x, y] = 'O';
                    cavePortalsPlaced++;
                }
            }

            // Входы в Титаник (рядом с реками)
            int titanicPortalsPlaced = 0;
            int titanicAttempts = 0;

            while (titanicPortalsPlaced < 400 && titanicAttempts < 100000)
            {
                int x = rnd.Next(5, MAP_HEIGHT - 5);
                int y = rnd.Next(5, MAP_WIDTH - 5);
                titanicAttempts++;

                if (fullMap[x, y] != '.')
                    continue;

                bool hasRiverNearby = false;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        int nx = x + dx;
                        int ny = y + dy;

                        if (nx >= 0 && nx < MAP_HEIGHT && ny >= 0 && ny < MAP_WIDTH)
                        {
                            if (fullMap[nx, ny] == '~')
                            {
                                hasRiverNearby = true;
                                break;
                            }
                        }
                    }
                    if (hasRiverNearby) break;
                }

                if (hasRiverNearby)
                {
                    fullMap[x, y] = 'T';
                    groundTypes[x, y] = 'T';
                    titanicPortalsPlaced++;
                }
            }

            // Входы в домик Бабы Яги (рядом с лесами)
            int hutPortalsPlaced = 0;
            int hutAttempts = 0;

            while (hutPortalsPlaced < 300 && hutAttempts < 100000)
            {
                int x = rnd.Next(5, MAP_HEIGHT - 5);
                int y = rnd.Next(5, MAP_WIDTH - 5);
                hutAttempts++;

                if (fullMap[x, y] != '.')
                    continue;

                bool hasForestNearby = false;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        int nx = x + dx;
                        int ny = y + dy;

                        if (nx >= 0 && nx < MAP_HEIGHT && ny >= 0 && ny < MAP_WIDTH)
                        {
                            if (fullMap[nx, ny] == '#')
                            {
                                hasForestNearby = true;
                                break;
                            }
                        }
                    }
                    if (hasForestNearby) break;
                }

                if (hasForestNearby)
                {
                    fullMap[x, y] = 'F';
                    groundTypes[x, y] = 'F';
                    hutPortalsPlaced++;
                }
            }

            // Враги
            for (int i = 0; i < MAP_HEIGHT; i++)
            {
                for (int j = 0; j < MAP_WIDTH; j++)
                {
                    if (rnd.Next(100) < 3 && fullMap[i, j] == '.')
                    {
                        fullMap[i, j] = '&';
                    }
                }
            }

            // Сердца
            for (int i = 0; i < MAP_HEIGHT; i++)
            {
                for (int j = 0; j < MAP_WIDTH; j++)
                {
                    if (rnd.Next(100) < 3 && fullMap[i, j] == '.')
                    {
                        fullMap[i, j] = 'H';
                    }
                }
            }

            // Стены
            for (int i = 0; i < MAP_HEIGHT; i++)
            {
                for (int j = 0; j < MAP_WIDTH; j++)
                {
                    int count = rnd.Next(100);
                    if (count >= 10 && count < 13 && fullMap[i, j] == '.')
                    {
                        fullMap[i, j] = '%';
                    }
                }
            }

            return fullMap;
        }

        private static void GenerateRiver(char[,] fullMap)
        {
            int startX = rnd.Next(10, MAP_HEIGHT - 10);
            int startY = rnd.Next(10, MAP_WIDTH - 10);
            int riverLength = rnd.Next(100, 301);
            int currentX = startX;
            int currentY = startY;
            int direction = rnd.Next(4);

            for (int step = 0; step < riverLength; step++)
            {
                if (currentX >= 0 && currentX < MAP_HEIGHT && currentY >= 0 && currentY < MAP_WIDTH)
                {
                    if (fullMap[currentX, currentY] != '^')
                    {
                        fullMap[currentX, currentY] = '~';
                    }
                }

                if (rnd.Next(100) < 30)
                {
                    direction = rnd.Next(4);
                }

                switch (direction)
                {
                    case 0: currentX--; break;
                    case 1: currentY++; break;
                    case 2: currentX++; break;
                    case 3: currentY--; break;
                }

                if (currentX < 5 || currentX >= MAP_HEIGHT - 5 || currentY < 5 || currentY >= MAP_WIDTH - 5)
                {
                    direction = (direction + 2) % 4;
                }
            }
        }

        private static void GenerateForest(char[,] fullMap)
        {
            int centerX = rnd.Next(10, MAP_HEIGHT - 10);
            int centerY = rnd.Next(10, MAP_WIDTH - 10);
            int radius = rnd.Next(5, 16);
            int density = rnd.Next(40, 81);

            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                for (int y = centerY - radius; y <= centerY + radius; y++)
                {
                    if (x < 0 || x >= MAP_HEIGHT || y < 0 || y >= MAP_WIDTH)
                        continue;

                    double distance = Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));

                    if (distance <= radius)
                    {
                        double probability = density / 100.0;
                        probability *= (1 - (distance / radius) * 0.5);

                        if (rnd.NextDouble() < probability)
                        {
                            if (fullMap[x, y] == '.')
                            {
                                fullMap[x, y] = '#';
                            }
                        }
                    }
                }
            }
        }

        private static void CreateMountain(char[,] fullMap, int centerX, int centerY)
        {
            if (centerX >= 0 && centerX < fullMap.GetLength(0) && centerY >= 0 && centerY < fullMap.GetLength(1))
            {
                fullMap[centerX, centerY] = '^';
            }

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int x = centerX + dx;
                    int y = centerY + dy;

                    if (x >= 0 && x < fullMap.GetLength(0) && y >= 0 && y < fullMap.GetLength(1))
                    {
                        fullMap[x, y] = '^';
                    }
                }
            }

            int[] probabilities = { 85, 70, 55, 40 };
            for (int circle = 2; circle <= 5; circle++)
            {
                for (int dx = -circle; dx <= circle; dx++)
                {
                    for (int dy = -circle; dy <= circle; dy++)
                    {
                        if (Math.Abs(dx) == circle || Math.Abs(dy) == circle)
                        {
                            int x = centerX + dx;
                            int y = centerY + dy;

                            if (x >= 0 && x < fullMap.GetLength(0) && y >= 0 && y < fullMap.GetLength(1))
                            {
                                if (rnd.Next(100) < probabilities[circle - 2])
                                {
                                    fullMap[x, y] = '^';
                                }
                            }
                        }
                    }
                }
            }

            if (rnd.Next(100) < 30)
            {
                for (int dx = -6; dx <= 6; dx++)
                {
                    for (int dy = -6; dy <= 6; dy++)
                    {
                        if (Math.Abs(dx) == 6 || Math.Abs(dy) == 6)
                        {
                            int x = centerX + dx;
                            int y = centerY + dy;

                            if (x >= 0 && x < fullMap.GetLength(0) && y >= 0 && y < fullMap.GetLength(1))
                            {
                                if (rnd.Next(100) < 25)
                                {
                                    fullMap[x, y] = '^';
                                }
                            }
                        }
                    }
                }
            }
        }

        public static bool HasEnemiesInView(char[,] fullMap, int playerX, int playerY)
        {
            int startX = playerX - VIEW_WIDTH / 2;
            int startY = playerY - VIEW_HEIGHT / 2;
            startX = Math.Max(0, Math.Min(startX, MAP_HEIGHT - VIEW_HEIGHT));
            startY = Math.Max(0, Math.Min(startY, MAP_WIDTH - VIEW_WIDTH));

            for (int i = 0; i < VIEW_HEIGHT; i++)
            {
                for (int j = 0; j < VIEW_WIDTH; j++)
                {
                    if (fullMap[startX + i, startY + j] == '&')
                        return true;
                }
            }
            return false;
        }

        public static bool IsPortalOnMap(char[,] fullMap)
        {
            for (int i = 0; i < fullMap.GetLength(0); i++)
            {
                for (int j = 0; j < fullMap.GetLength(1); j++)
                {
                    if (fullMap[i, j] == '0')
                        return true;
                }
            }
            return false;
        }

        public static void CheckAndSpawnPortal(char[,] fullMap, ref int playerX, ref int playerY)
        {
            if (!HasEnemiesInView(fullMap, playerX, playerY) && !IsPortalOnMap(fullMap))
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        int portalX = playerX + dx;
                        int portalY = playerY + dy;

                        if (portalX >= 5 && portalX < fullMap.GetLength(0) - 5 &&
                            portalY >= 5 && portalY < fullMap.GetLength(1) - 5 &&
                            fullMap[portalX, portalY] == '.')
                        {
                            fullMap[portalX, portalY] = '0';
                            groundTypes[portalX, portalY] = '.';
                            Console.SetCursorPosition(0, 28);
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("⭐ ПОРТАЛ ПОЯВИЛСЯ РЯДОМ! ⭐");
                            Console.ResetColor();
                            System.Threading.Thread.Sleep(2000);
                            Console.SetCursorPosition(0, 28);
                            Console.WriteLine("                                          ");
                            return;
                        }
                    }
                }
            }
        }

        public static char[,] CreateEmptyCave()
        {
            char[,] caveMap = new char[25, 25];

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    caveMap[i, j] = '.';
                }
            }

            caveMap[12, 13] = 'O';
            return caveMap;
        }

        public static char[,] CreateTitanicMap()
        {
            char[,] titanicMap = new char[25, 25];

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    titanicMap[i, j] = '.';
                }
            }

            titanicMap[12, 13] = 'T';
            return titanicMap;
        }

        public static char[,] CreateHutMap()
        {
            char[,] hutMap = new char[25, 25];

            // Заполняем всю локацию точками (пустота)
            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    hutMap[i, j] = '.';
                }
            }

            // Выход из домика (справа от центра)
            hutMap[12, 13] = 'F';

            return hutMap;
        }

        public static void MoveInCave(ref int cavePlayerX, ref int cavePlayerY, int dx, int dy, char[,] caveMap, ref bool inCave)
        {
            int newX = cavePlayerX + dx;
            int newY = cavePlayerY + dy;

            if (newX < 0 || newX >= 25 || newY < 0 || newY >= 25)
                return;

            if (caveMap[newX, newY] == 'O')
            {
                inCave = false;
                Console.SetCursorPosition(0, 28);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("⛰️ ВЫ ВЫШЛИ ИЗ ПЕЩЕРЫ! ⛰️");
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
                return;
            }

            caveMap[cavePlayerX, cavePlayerY] = '.';
            cavePlayerX = newX;
            cavePlayerY = newY;
            caveMap[cavePlayerX, cavePlayerY] = '@';
        }

        public static void MoveInTitanic(ref int titanicPlayerX, ref int titanicPlayerY, int dx, int dy, char[,] titanicMap, ref bool inTitanic)
        {
            int newX = titanicPlayerX + dx;
            int newY = titanicPlayerY + dy;

            if (newX < 0 || newX >= 25 || newY < 0 || newY >= 25)
                return;

            if (titanicMap[newX, newY] == 'T')
            {
                inTitanic = false;
                Console.SetCursorPosition(0, 28);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("🚢 ВЫ ВЫШЛИ ИЗ ТИТАНИКА! 🚢");
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
                return;
            }

            titanicMap[titanicPlayerX, titanicPlayerY] = '.';
            titanicPlayerX = newX;
            titanicPlayerY = newY;
            titanicMap[titanicPlayerX, titanicPlayerY] = '@';
        }

        public static void MoveInHut(ref int hutPlayerX, ref int hutPlayerY, int dx, int dy, char[,] hutMap, ref bool inHut)
        {
            int newX = hutPlayerX + dx;
            int newY = hutPlayerY + dy;

            if (newX < 0 || newX >= 25 || newY < 0 || newY >= 25)
                return;

            if (hutMap[newX, newY] == 'F')
            {
                inHut = false;
                Console.SetCursorPosition(0, 28);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("🏚️ ВЫ ВЫШЛИ ИЗ ДОМИКА! 🏚️");
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
                return;
            }

            // Перемещаем игрока
            hutMap[hutPlayerX, hutPlayerY] = '.';
            hutPlayerX = newX;
            hutPlayerY = newY;
            hutMap[hutPlayerX, hutPlayerY] = '@';
        }

        public static void MovePlayer(ref int playerX, ref int playerY, int dx, int dy, char[,] fullMap, Person hero,
    ref bool inCave, ref char[,] caveMap, ref int cavePlayerX, ref int cavePlayerY,
    ref bool inTitanic, ref char[,] titanicMap, ref int titanicPlayerX, ref int titanicPlayerY,
    ref bool inHut, ref char[,] hutMap, ref int hutPlayerX, ref int hutPlayerY)
        {
            int newX = playerX + dx;
            int newY = playerY + dy;

            if (newX < 0 || newX >= fullMap.GetLength(0) || newY < 0 || newY >= fullMap.GetLength(1))
                return;

            char cell = fullMap[newX, newY];

            // Проверяем, можно ли пройти
            if (cell == '^' || cell == '%')
            {
                Console.SetCursorPosition(0, 26);
                Console.WriteLine("Вы не можете пройти!                          ");
                System.Threading.Thread.Sleep(500);
                return;
            }

            // Вход в пещеру
            if (cell == 'O')
            {
                cavePlayerX = 12;
                cavePlayerY = 12;
                caveMap = CreateEmptyCave();
                caveMap[cavePlayerX, cavePlayerY] = '@';
                inCave = true;

                Console.SetCursorPosition(0, 28);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("⛰️ ВЫ ВОШЛИ В ПЕЩЕРУ! Выход справа от вас ⛰️");
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
                return;
            }

            // Вход в Титаник
            if (cell == 'T')
            {
                titanicPlayerX = 12;
                titanicPlayerY = 12;
                titanicMap = CreateTitanicMap();
                titanicMap[titanicPlayerX, titanicPlayerY] = '@';
                inTitanic = true;

                Console.SetCursorPosition(0, 28);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("🚢 ВЫ ПОПАЛИ НА ТИТАНИК! Выход справа от вас 🚢");
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
                return;
            }

            // Вход в домик Бабы Яги
            if (cell == 'F')
            {
                hutPlayerX = 12;
                hutPlayerY = 12;
                hutMap = CreateHutMap();
                hutMap[hutPlayerX, hutPlayerY] = '@';
                inHut = true;

                Console.SetCursorPosition(0, 28);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("🏚️ ВЫ ВОШЛИ В ДОМИК БАБЫ ЯГИ! 🏚️");
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
                return;
            }

            // Бой с врагом
            if (cell == '&')
            {
                Console.Clear();
                Person Enemy = new Person(levelWorld * 10);
                Random battleRnd = new Random();

                while (Enemy.HP > 0 && hero.HP > 0)
                {
                    int Shot = battleRnd.Next(10);
                    Enemy.HP -= Shot + hero.Strenght;
                    Shot = battleRnd.Next(10);
                    hero.HP -= Shot + levelWorld * 5;
                }

                if (Enemy.HP < hero.HP)
                {
                    hero.coin += battleRnd.Next(100);
                    // Враг исчезает, восстанавливаем тип местности
                    fullMap[newX, newY] = groundTypes[newX, newY];
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Поражение");
                    Console.ReadKey();
                    hero.HP = 0;
                    return;
                }
            }
            // Лечение
            else if (cell == 'H')
            {
                hero.MaxHP += 10;
                hero.HP += hero.MaxHP / 10;
                // Сердце исчезает, восстанавливаем тип местности
                fullMap[newX, newY] = groundTypes[newX, newY];
            }
            // Портал
            else if (cell == '0')
            {
                Console.SetCursorPosition(0, 28);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("🌀 ВЫ ВОШЛИ В ПОРТАЛ! Переход на следующий уровень... 🌀");
                Console.ResetColor();
                System.Threading.Thread.Sleep(2000);

                hero.HP = hero.MaxHP;
                levelWorld++;
                fullMap[playerX, playerY] = groundTypes[playerX, playerY];
                playerX = fullMap.GetLength(0) / 2;
                playerY = fullMap.GetLength(1) / 2;

                char[,] newMap = CreateFullMap();
                for (int i = 0; i < fullMap.GetLength(0); i++)
                    for (int j = 0; j < fullMap.GetLength(1); j++)
                        fullMap[i, j] = newMap[i, j];

                fullMap[playerX, playerY] = '@';
                return;
            }
            // Кузница
            else if (cell == '+')
            {
                Forge(hero);
                // Кузница исчезает после использования
                fullMap[newX, newY] = groundTypes[newX, newY];
            }

            // Сохраняем тип местности, на которую переходим
            char groundType = fullMap[newX, newY];

            // Если это река или лес, запоминаем их в groundTypes при первом входе
            if ((groundType == '~' || groundType == '#') && groundTypes[newX, newY] == '.')
            {
                groundTypes[newX, newY] = groundType;
            }

            // Перемещаем игрока
            // На старой позиции восстанавливаем тип местности
            fullMap[playerX, playerY] = groundTypes[playerX, playerY];

            playerX = newX;
            playerY = newY;

            // На новой позиции ставим игрока
            fullMap[playerX, playerY] = '@';

            // Проверяем, нужно ли создать портал
            CheckAndSpawnPortal(fullMap, ref playerX, ref playerY);
        }

        public static void Forge(Person Hero)
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Улучшить силу на 2 (250 монет)");
            Console.WriteLine("Для выхода нажмите Enter");
            Console.WriteLine($"Оставшиеся деньги: {Hero.coin}");

            ConsoleKey key;
            while ((key = Console.ReadKey().Key) != ConsoleKey.Enter)
            {
                if (key == ConsoleKey.NumPad1 && Hero.coin > 250)
                {
                    Hero.Strenght += 2;
                    Hero.coin -= 250;
                    Console.WriteLine($"\nСила увеличена! Текущая сила: {Hero.Strenght}");
                }
                else if (key == ConsoleKey.NumPad1)
                {
                    Console.WriteLine("\nНедостаточно монет!");
                }
            }
        }
    }
}