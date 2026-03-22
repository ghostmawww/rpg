using System;
using System.Collections.Generic;

namespace ConsoleApp46
{
    public class Map
    {
        private static char[,] groundTypes;
        static public int levelWorld = 1;
        static Random rnd = new Random();

        private const int MAP_WIDTH = 1500;
        private const int MAP_HEIGHT = 1500;
        private const int VIEW_WIDTH = 25;
        private const int VIEW_HEIGHT = 25;

        static public void GetMap(char[,] fullMap, int playerWorldX, int playerWorldY)
        {
            try
            {
                if (fullMap == null)
                    throw new GameException("Карта не инициализирована", "M001", "Map", ErrorSeverity.Critical);

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
                        DrawCell(cell);
                    }
                    Console.WriteLine();
                }
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        private static void DrawCell(char cell)
        {
            switch (cell)
            {
                case '0': Console.ForegroundColor = ConsoleColor.Blue; break;
                case '&': Console.ForegroundColor = ConsoleColor.Green; break;
                case 'H': Console.ForegroundColor = ConsoleColor.Red; break;
                case '+': Console.ForegroundColor = ConsoleColor.Yellow; break;
                case '%': Console.ForegroundColor = ConsoleColor.Gray; break;
                case '@': Console.ForegroundColor = ConsoleColor.Cyan; break;
                case '^': Console.ForegroundColor = ConsoleColor.DarkGray; break;
                case '~': Console.ForegroundColor = ConsoleColor.Blue; break;
                case '#': Console.ForegroundColor = ConsoleColor.DarkGreen; break;
                case 'O': Console.ForegroundColor = ConsoleColor.Magenta; break;
                case 'T': Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                case 'F': Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                case 'o': Console.ForegroundColor = ConsoleColor.DarkGray; break;
                case '★': Console.ForegroundColor = ConsoleColor.Yellow; break;
                case 'C': Console.ForegroundColor = ConsoleColor.Yellow; break;
                case 'B': Console.ForegroundColor = ConsoleColor.Red; break;
                default: Console.ResetColor(); break;
            }
            Console.Write(cell + " ");
            Console.ResetColor();
        }

        static public char[,] CreateFullMap()
        {
            try
            {
                char[,] fullMap = new char[MAP_HEIGHT, MAP_WIDTH];
                groundTypes = new char[MAP_HEIGHT, MAP_WIDTH];

                for (int i = 0; i < MAP_HEIGHT; i++)
                    for (int j = 0; j < MAP_WIDTH; j++)
                    {
                        fullMap[i, j] = '.';
                        groundTypes[i, j] = '.';
                    }

                for (int r = 0; r < 1600; r++) GenerateRiver(fullMap);
                for (int f = 0; f < 2000; f++) GenerateForest(fullMap);
                for (int g = 0; g < 2700; g++)
                {
                    int x = rnd.Next(20, MAP_HEIGHT - 20);
                    int y = rnd.Next(20, MAP_WIDTH - 20);
                    CreateMountain(fullMap, x, y);
                }

                GeneratePortals(fullMap);
                GenerateObjects(fullMap);

                return fullMap;
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private static void GenerateRiver(char[,] fullMap)
        {
            try
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
                            fullMap[currentX, currentY] = '~';
                    }
                    else
                    {
                        break;
                    }

                    if (rnd.Next(100) < 30) direction = rnd.Next(4);

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
                        currentX = Math.Max(5, Math.Min(currentX, MAP_HEIGHT - 6));
                        currentY = Math.Max(5, Math.Min(currentY, MAP_WIDTH - 6));
                    }
                }
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
            }
        }

        private static void GenerateForest(char[,] fullMap)
        {
            try
            {
                int centerX = rnd.Next(10, MAP_HEIGHT - 10);
                int centerY = rnd.Next(10, MAP_WIDTH - 10);
                int radius = rnd.Next(5, 16);
                int density = rnd.Next(40, 81);

                for (int x = centerX - radius; x <= centerX + radius; x++)
                {
                    for (int y = centerY - radius; y <= centerY + radius; y++)
                    {
                        if (x < 0 || x >= MAP_HEIGHT || y < 0 || y >= MAP_WIDTH) continue;
                        double distance = Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));
                        if (distance <= radius)
                        {
                            double probability = density / 100.0 * (1 - (distance / radius) * 0.5);
                            if (rnd.NextDouble() < probability && fullMap[x, y] == '.')
                                fullMap[x, y] = '#';
                        }
                    }
                }
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
            }
        }

        private static void CreateMountain(char[,] fullMap, int centerX, int centerY)
        {
            try
            {
                if (centerX < 0 || centerX >= fullMap.GetLength(0) || centerY < 0 || centerY >= fullMap.GetLength(1))
                    throw new GameException("Координаты горы вне границ карты", "M005", "Map", ErrorSeverity.Medium);

                fullMap[centerX, centerY] = '^';
                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        int x = centerX + dx, y = centerY + dy;
                        if (x >= 0 && x < fullMap.GetLength(0) && y >= 0 && y < fullMap.GetLength(1))
                            fullMap[x, y] = '^';
                    }

                int[] probabilities = { 85, 70, 55, 40 };
                for (int circle = 2; circle <= 5; circle++)
                    for (int dx = -circle; dx <= circle; dx++)
                        for (int dy = -circle; dy <= circle; dy++)
                            if (Math.Abs(dx) == circle || Math.Abs(dy) == circle)
                            {
                                int x = centerX + dx, y = centerY + dy;
                                if (x >= 0 && x < fullMap.GetLength(0) && y >= 0 && y < fullMap.GetLength(1))
                                    if (rnd.Next(100) < probabilities[circle - 2])
                                        fullMap[x, y] = '^';
                            }

                if (rnd.Next(100) < 30)
                    for (int dx = -6; dx <= 6; dx++)
                        for (int dy = -6; dy <= 6; dy++)
                            if (Math.Abs(dx) == 6 || Math.Abs(dy) == 6)
                            {
                                int x = centerX + dx, y = centerY + dy;
                                if (x >= 0 && x < fullMap.GetLength(0) && y >= 0 && y < fullMap.GetLength(1))
                                    if (rnd.Next(100) < 25)
                                        fullMap[x, y] = '^';
                            }
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
            }
        }

        private static void GeneratePortals(char[,] fullMap)
        {
            try
            {
                int cavePortals = 0;
                while (cavePortals < 800)
                {
                    int x = rnd.Next(5, MAP_HEIGHT - 5), y = rnd.Next(5, MAP_WIDTH - 5);
                    if (fullMap[x, y] == '.' && HasNearby(fullMap, x, y, '^'))
                    {
                        fullMap[x, y] = 'O';
                        groundTypes[x, y] = 'O';
                        cavePortals++;
                    }
                }

                int titanicPortals = 0;
                while (titanicPortals < 400)
                {
                    int x = rnd.Next(5, MAP_HEIGHT - 5), y = rnd.Next(5, MAP_WIDTH - 5);
                    if (fullMap[x, y] == '.' && HasNearby(fullMap, x, y, '~'))
                    {
                        fullMap[x, y] = 'T';
                        groundTypes[x, y] = 'T';
                        titanicPortals++;
                    }
                }

                int hutPortals = 0;
                while (hutPortals < 300)
                {
                    int x = rnd.Next(5, MAP_HEIGHT - 5), y = rnd.Next(5, MAP_WIDTH - 5);
                    if (fullMap[x, y] == '.' && HasNearby(fullMap, x, y, '#'))
                    {
                        fullMap[x, y] = 'F';
                        groundTypes[x, y] = 'F';
                        hutPortals++;
                    }
                }
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
            }
        }

        private static bool HasNearby(char[,] fullMap, int x, int y, char target)
        {
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = x + dx, ny = y + dy;
                    if (nx >= 0 && nx < MAP_HEIGHT && ny >= 0 && ny < MAP_WIDTH)
                        if (fullMap[nx, ny] == target) return true;
                }
            return false;
        }

        private static void GenerateObjects(char[,] fullMap)
        {
            try
            {
                for (int i = 0; i < MAP_HEIGHT; i++)
                    for (int j = 0; j < MAP_WIDTH; j++)
                        if (rnd.Next(100) < 3 && fullMap[i, j] == '.')
                            fullMap[i, j] = '&';

                for (int i = 0; i < MAP_HEIGHT; i++)
                    for (int j = 0; j < MAP_WIDTH; j++)
                        if (rnd.Next(100) < 3 && fullMap[i, j] == '.')
                            fullMap[i, j] = 'H';

                for (int i = 0; i < MAP_HEIGHT; i++)
                    for (int j = 0; j < MAP_WIDTH; j++)
                        if (rnd.Next(100) >= 10 && rnd.Next(100) < 13 && fullMap[i, j] == '.')
                            fullMap[i, j] = '%';
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка генерации объектов: {ex.Message}");
            }
        }

        public static bool HasEnemiesInView(char[,] fullMap, int playerX, int playerY)
        {
            try
            {
                if (fullMap == null)
                    throw new GameException("Карта не инициализирована", "M006", "Map", ErrorSeverity.Critical);

                int startX = playerX - VIEW_WIDTH / 2;
                int startY = playerY - VIEW_HEIGHT / 2;
                startX = Math.Max(0, Math.Min(startX, MAP_HEIGHT - VIEW_HEIGHT));
                startY = Math.Max(0, Math.Min(startY, MAP_WIDTH - VIEW_WIDTH));

                for (int i = 0; i < VIEW_HEIGHT; i++)
                    for (int j = 0; j < VIEW_WIDTH; j++)
                        if (fullMap[startX + i, startY + j] == '&')
                            return true;
                return false;
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
                return false;
            }
        }

        public static bool IsPortalOnMap(char[,] fullMap)
        {
            for (int i = 0; i < fullMap.GetLength(0); i++)
                for (int j = 0; j < fullMap.GetLength(1); j++)
                    if (fullMap[i, j] == '0')
                        return true;
            return false;
        }

        public static void CheckAndSpawnPortal(char[,] fullMap, ref int playerX, ref int playerY)
        {
            try
            {
                if (!HasEnemiesInView(fullMap, playerX, playerY) && !IsPortalOnMap(fullMap))
                {
                    for (int dx = -1; dx <= 1; dx++)
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;
                            int portalX = playerX + dx, portalY = playerY + dy;
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
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
            }
        }

        // ==================== ПЕЩЕРА С ЗАГАДКОЙ ====================

        public static char[,] CreateCaveWithPuzzle()
        {
            char[,] caveMap = new char[25, 25];

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    caveMap[i, j] = '.';
                }
            }

            // Стены по краям
            for (int i = 0; i < 25; i++)
            {
                caveMap[0, i] = '#';
                caveMap[24, i] = '#';
                caveMap[i, 0] = '#';
                caveMap[i, 24] = '#';
            }

            // Целевые места для камней
            caveMap[5, 5] = 'O';
            caveMap[5, 19] = 'O';
            caveMap[19, 5] = 'O';
            caveMap[19, 19] = 'O';

            // Камни
            caveMap[3, 12] = 'o';
            caveMap[12, 3] = 'o';
            caveMap[12, 21] = 'o';
            caveMap[21, 12] = 'o';

            // Выход (появится после решения)
            caveMap[12, 12] = ' ';

            return caveMap;
        }

        public static void RenderCaveWithPuzzle(char[,] caveMap, Person hero, bool puzzleSolved)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    ПЕЩЕРА С ЗАГАДКОЙ                    ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  Нужно сдвинуть камни (o) на целевые места (O)          ║");
            Console.WriteLine("║  Управление: стрелки - движение                         ║");
            Console.WriteLine("║  Камни двигаются, когда вы на них наступаете            ║");
            if (puzzleSolved)
            {
                Console.WriteLine("║  ✨ ЗАГАДКА РЕШЕНА! Выход открыт! ✨                   ║");
            }
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    char cell = caveMap[i, j];

                    if (cell == '@')
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("@ ");
                        Console.ResetColor();
                    }
                    else if (cell == '#')
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("# ");
                        Console.ResetColor();
                    }
                    else if (cell == 'o')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("o ");
                        Console.ResetColor();
                    }
                    else if (cell == 'O')
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("O ");
                        Console.ResetColor();
                    }
                    else if (cell == '★')
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("★ ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(". ");
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine("\n=== ПРАВИЛА ===");
            Console.WriteLine("Подойдите к камню (o) и нажмите стрелку в его сторону, чтобы толкнуть его");
            Console.WriteLine("Камень двигается на 1 клетку в направлении движения");
            Console.WriteLine("Поставьте все камни на желтые цели (O)");
            Console.WriteLine($"\nЗдоровье: {hero.HP}/{hero.MaxHP} | Монет: {hero.coin}");
        }

        public static bool CheckCavePuzzleSolved(char[,] caveMap)
        {
            if (caveMap[5, 5] != 'o') return false;
            if (caveMap[5, 19] != 'o') return false;
            if (caveMap[19, 5] != 'o') return false;
            if (caveMap[19, 19] != 'o') return false;
            return true;
        }

        public static void PushStoneInCave(ref char[,] caveMap, ref int playerX, ref int playerY, int dx, int dy)
        {
            int stoneX = playerX + dx;
            int stoneY = playerY + dy;

            if (stoneX < 0 || stoneX >= 25 || stoneY < 0 || stoneY >= 25)
                return;

            if (caveMap[stoneX, stoneY] != 'o')
                return;

            int newX = stoneX + dx;
            int newY = stoneY + dy;

            if (newX < 0 || newX >= 25 || newY < 0 || newY >= 25)
                return;

            char targetCell = caveMap[newX, newY];

            if (targetCell == '#' || targetCell == 'o')
                return;

            bool wasTarget = (targetCell == 'O');
            bool currentWasTarget = (caveMap[stoneX, stoneY] == 'O');

            if (currentWasTarget)
            {
                caveMap[stoneX, stoneY] = 'O';
            }
            else
            {
                caveMap[stoneX, stoneY] = '.';
            }

            caveMap[newX, newY] = 'o';

            caveMap[playerX, playerY] = '.';
            playerX = stoneX;
            playerY = stoneY;

            if (currentWasTarget)
            {
                caveMap[playerX, playerY] = 'O';
            }
            else
            {
                caveMap[playerX, playerY] = '@';
            }
        }

        public static void MoveInCaveWithPuzzle(ref int playerX, ref int playerY, int dx, int dy,
            ref char[,] caveMap, ref bool inCave, ref bool puzzleSolved, Person hero)
        {
            int newX = playerX + dx;
            int newY = playerY + dy;

            if (newX < 0 || newX >= 25 || newY < 0 || newY >= 25)
                return;

            char cell = caveMap[newX, newY];

            if (cell == '★' && puzzleSolved)
            {
                inCave = false;
                Console.SetCursorPosition(0, 28);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("⛰️ ВЫ ВЫШЛИ ИЗ ПЕЩЕРЫ! ⛰️");
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
                return;
            }

            if (cell == '#')
                return;

            if (cell == 'o')
            {
                PushStoneInCave(ref caveMap, ref playerX, ref playerY, dx, dy);

                if (!puzzleSolved && CheckCavePuzzleSolved(caveMap))
                {
                    puzzleSolved = true;
                    caveMap[12, 12] = '★';

                    Console.SetCursorPosition(0, 28);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("✨ ЗАГАДКА РЕШЕНА! Выход открыт в центре пещеры! ✨");
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(2000);

                    hero.coin += 200;
                    hero.MaxHP += 20;
                    hero.HP += 20;
                    if (hero.HP > hero.MaxHP) hero.HP = hero.MaxHP;
                }
                return;
            }

            bool wasTarget = (cell == 'O');

            caveMap[playerX, playerY] = '.';
            playerX = newX;
            playerY = newY;

            if (wasTarget)
            {
                caveMap[playerX, playerY] = 'O';
            }
            else
            {
                caveMap[playerX, playerY] = '@';
            }

            if (!puzzleSolved && CheckCavePuzzleSolved(caveMap))
            {
                puzzleSolved = true;
                caveMap[12, 12] = '★';

                Console.SetCursorPosition(0, 28);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("✨ ЗАГАДКА РЕШЕНА! Выход открыт в центре пещеры! ✨");
                Console.ResetColor();
                System.Threading.Thread.Sleep(2000);

                hero.coin += 200;
                hero.MaxHP += 20;
                hero.HP += 20;
                if (hero.HP > hero.MaxHP) hero.HP = hero.MaxHP;
            }
        }

        // ==================== ЛОКАЦИЯ ТИТАНИК ====================

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

        public static void MoveInTitanic(ref int titanicPlayerX, ref int titanicPlayerY, int dx, int dy, char[,] titanicMap, ref bool inTitanic, Person hero)
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

            int damage = 5;
            hero.HP -= damage;

            Console.SetCursorPosition(0, 29);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❄️ Холодная вода! Вы потеряли {damage} здоровья! ❄️");
            Console.ResetColor();

            if (hero.HP <= 0)
            {
                Console.Clear();
                Console.WriteLine("❄️ ВЫ ЗАМЕРЗЛИ В ЛЕДЯНОЙ ВОДЕ! ❄️");
                Console.WriteLine("Ваше тело ушло на дно океана...");
                Console.ReadKey();
                return;
            }

            System.Threading.Thread.Sleep(800);

            titanicMap[titanicPlayerX, titanicPlayerY] = '.';
            titanicPlayerX = newX;
            titanicPlayerY = newY;
            titanicMap[titanicPlayerX, titanicPlayerY] = '@';
        }

        // ==================== ЛОКАЦИЯ ДОМИК БАБЫ ЯГИ ====================

        public static char[,] CreateHutMap()
        {
            char[,] hutMap = new char[25, 25];

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    hutMap[i, j] = '.';
                }
            }

            // Стены по краям
            for (int i = 0; i < 25; i++)
            {
                hutMap[0, i] = '#';
                hutMap[24, i] = '#';
                hutMap[i, 0] = '#';
                hutMap[i, 24] = '#';
            }

            // Домик Бабы Яги (внутренние стены)
            for (int i = 8; i <= 16; i++)
            {
                for (int j = 8; j <= 16; j++)
                {
                    if (i == 8 || i == 16 || j == 8 || j == 16)
                    {
                        hutMap[i, j] = '#';
                    }
                }
            }

            // Вход в домик
            hutMap[12, 8] = '.';

            // Сундук с артефактом
            hutMap[10, 10] = 'C';

            // Противник (Баба Яга)
            hutMap[14, 14] = 'B';

            // Выход из локации
            hutMap[12, 20] = 'F';

            return hutMap;
        }

        public static void RenderHutMap(char[,] hutMap, Person hero, bool hasArtifact)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  ДОМИК БАБЫ ЯГИ                          ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  Баба Яга (B) двигается по полю!                        ║");
            Console.WriteLine("║  Найдите сундук (C) и получите артефакт                 ║");
            if (hasArtifact)
            {
                Console.WriteLine("║  ✨ У вас есть артефакт! Поймайте Бабу Ягу (B) ✨        ║");
            }
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    char cell = hutMap[i, j];

                    if (cell == '@')
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("@ ");
                        Console.ResetColor();
                    }
                    else if (cell == '#')
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("# ");
                        Console.ResetColor();
                    }
                    else if (cell == 'C')
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("C ");
                        Console.ResetColor();
                    }
                    else if (cell == 'B')
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("B ");
                        Console.ResetColor();
                    }
                    else if (cell == 'F')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("F ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(". ");
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine("\n=== ПРАВИЛА ===");
            if (!hasArtifact)
            {
                Console.WriteLine("🔍 Найдите сундук (C) внутри домика!");
                Console.WriteLine("👻 Баба Яга двигается по полю и наносит урон при столкновении!");
            }
            else
            {
                Console.WriteLine("⚔️ Поймайте Бабу Ягу (B), чтобы использовать артефакт!");
            }
            Console.WriteLine($"\nЗдоровье: {hero.HP}/{hero.MaxHP} | Монет: {hero.coin}");
        }

        // Метод для движения Бабы Яги
        public static void MoveBabaYaga(ref char[,] hutMap, ref int babaX, ref int babaY, int playerX, int playerY, Person hero, ref bool inHut)
        {
            if (babaX == -1 && babaY == -1) return; // Баба Яга побеждена

            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            // Перемешиваем направления для случайности
            List<int> directions = new List<int> { 0, 1, 2, 3 };
            for (int i = 0; i < directions.Count; i++)
            {
                int randomIndex = rnd.Next(i, directions.Count);
                int temp = directions[i];
                directions[i] = directions[randomIndex];
                directions[randomIndex] = temp;
            }

            // Пытаемся найти направление, где Баба Яга может двигаться
            bool moved = false;
            foreach (int dir in directions)
            {
                int newX = babaX + dx[dir];
                int newY = babaY + dy[dir];

                if (newX < 0 || newX >= 25 || newY < 0 || newY >= 25)
                    continue;

                char targetCell = hutMap[newX, newY];

                // Если Баба Яга наступает на игрока
                if (targetCell == '@')
                {
                    int damage = 10;
                    hero.HP -= damage;
                    Console.SetCursorPosition(0, 28);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"💀 Баба Яга ударила вас! Вы потеряли {damage} здоровья! 💀");
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(1000);

                    if (hero.HP <= 0)
                    {
                        inHut = false;
                        return;
                    }
                    // Баба Яга не занимает клетку игрока, просто наносит урон
                    return;
                }

                // Баба Яга может ходить по пустым клеткам
                if (targetCell == '.')
                {
                    hutMap[babaX, babaY] = '.';
                    babaX = newX;
                    babaY = newY;
                    hutMap[babaX, babaY] = 'B';
                    moved = true;
                    break;
                }
            }

            // 30% шанс, что Баба Яга сделает дополнительный шаг
            if (moved && rnd.Next(100) < 30)
            {
                foreach (int dir in directions)
                {
                    int newX = babaX + dx[dir];
                    int newY = babaY + dy[dir];

                    if (newX < 0 || newX >= 25 || newY < 0 || newY >= 25)
                        continue;

                    char targetCell = hutMap[newX, newY];

                    if (targetCell == '@')
                    {
                        int damage = 10;
                        hero.HP -= damage;
                        Console.SetCursorPosition(0, 28);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"💀 Баба Яга ударила вас! Вы потеряли {damage} здоровья! 💀");
                        Console.ResetColor();
                        System.Threading.Thread.Sleep(1000);

                        if (hero.HP <= 0)
                        {
                            inHut = false;
                            return;
                        }
                        return;
                    }

                    if (targetCell == '.')
                    {
                        hutMap[babaX, babaY] = '.';
                        babaX = newX;
                        babaY = newY;
                        hutMap[babaX, babaY] = 'B';
                        break;
                    }
                }
            }
        }

        public static void MoveInHut(ref int hutPlayerX, ref int hutPlayerY, int dx, int dy,
            ref char[,] hutMap, ref bool inHut, ref bool hasArtifact, ref int babaX, ref int babaY, Person hero)
        {
            int newX = hutPlayerX + dx;
            int newY = hutPlayerY + dy;

            if (newX < 0 || newX >= 25 || newY < 0 || newY >= 25)
                return;

            char cell = hutMap[newX, newY];

            // Выход из домика
            if (cell == 'F')
            {
                inHut = false;
                hasArtifact = false;
                Console.SetCursorPosition(0, 28);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("🏚️ ВЫ ВЫШЛИ ИЗ ДОМИКА! 🏚️");
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
                return;
            }

            // Стены
            if (cell == '#')
                return;

            // Сундук
            if (cell == 'C' && !hasArtifact)
            {
                hasArtifact = true;
                hutMap[newX, newY] = '.';
                Console.SetCursorPosition(0, 28);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("✨ ВЫ НАШЛИ АРТЕФАКТ! Теперь можете поймать Бабу Ягу! ✨");
                Console.ResetColor();
                System.Threading.Thread.Sleep(2000);

                hero.coin += 100;

                hutMap[hutPlayerX, hutPlayerY] = '.';
                hutPlayerX = newX;
                hutPlayerY = newY;
                hutMap[hutPlayerX, hutPlayerY] = '@';

                // Баба Яга двигается после взятия артефакта
                MoveBabaYaga(ref hutMap, ref babaX, ref babaY, hutPlayerX, hutPlayerY, hero, ref inHut);
                return;
            }

            // Баба Яга
            if (cell == 'B')
            {
                if (hasArtifact)
                {
                    hutMap[newX, newY] = '.';
                    Console.SetCursorPosition(0, 28);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("⚔️ ВЫ ПОБЕДИЛИ БАБУ ЯГУ! Артефакт сработал! ⚔️");
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(2000);

                    hero.coin += 300;
                    hero.MaxHP += 30;
                    hero.HP += 30;
                    if (hero.HP > hero.MaxHP) hero.HP = hero.MaxHP;

                    hutMap[hutPlayerX, hutPlayerY] = '.';
                    hutPlayerX = newX;
                    hutPlayerY = newY;
                    hutMap[hutPlayerX, hutPlayerY] = '@';

                    // Баба Яга побеждена, удаляем её
                    babaX = -1;
                    babaY = -1;
                }
                else
                {
                    // Баба Яга убегает
                    Console.SetCursorPosition(0, 28);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("👻 Баба Яга убежала! Вам нужно найти артефакт! 👻");
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(1000);

                    // Игрок не двигается, а Баба Яга двигается
                    MoveBabaYaga(ref hutMap, ref babaX, ref babaY, hutPlayerX, hutPlayerY, hero, ref inHut);
                }
                return;
            }

            // Обычное движение игрока
            hutMap[hutPlayerX, hutPlayerY] = '.';
            hutPlayerX = newX;
            hutPlayerY = newY;
            hutMap[hutPlayerX, hutPlayerY] = '@';

            // Баба Яга двигается после каждого шага игрока
            MoveBabaYaga(ref hutMap, ref babaX, ref babaY, hutPlayerX, hutPlayerY, hero, ref inHut);
        }

        // ==================== ОСНОВНОЕ ДВИЖЕНИЕ ====================

        public static void MovePlayer(ref int playerX, ref int playerY, int dx, int dy, char[,] fullMap, Person hero,
            ref bool inCave, ref char[,] caveMap, ref int cavePlayerX, ref int cavePlayerY,
            ref bool inTitanic, ref char[,] titanicMap, ref int titanicPlayerX, ref int titanicPlayerY,
            ref bool inHut, ref char[,] hutMap, ref int hutPlayerX, ref int hutPlayerY,
            ref bool puzzleSolved, ref bool hasArtifact, ref int babaX, ref int babaY)
        {
            try
            {
                if (fullMap == null)
                    throw new GameException("Карта не инициализирована", "M008", "Map", ErrorSeverity.Critical);
                if (hero == null)
                    throw new GameException("Объект героя не инициализирован", "M009", "Map", ErrorSeverity.Critical);

                int newX = playerX + dx;
                int newY = playerY + dy;

                if (newX < 0 || newX >= fullMap.GetLength(0) || newY < 0 || newY >= fullMap.GetLength(1))
                    throw new GameException("Попытка выйти за границы карты", "M010", "Map", ErrorSeverity.Medium);

                char cell = fullMap[newX, newY];

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
                    inCave = true;
                    puzzleSolved = false;
                    cavePlayerX = 12;
                    cavePlayerY = 12;
                    caveMap = CreateCaveWithPuzzle();
                    caveMap[cavePlayerX, cavePlayerY] = '@';

                    Console.SetCursorPosition(0, 28);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("🧩 ВЫ ВОШЛИ В ПЕЩЕРУ С ЗАГАДКОЙ! 🧩");
                    Console.WriteLine("   Нужно поставить все камни (o) на желтые цели (O)!");
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(3000);
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
                    Console.WriteLine("🚢 ВЫ ПОПАЛИ НА ТИТАНИК! Каждый шаг отнимает здоровье! 🚢");
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(2000);
                    return;
                }

                // Вход в домик
                if (cell == 'F')
                {
                    hutPlayerX = 12;
                    hutPlayerY = 12;
                    hutMap = CreateHutMap();
                    hutMap[hutPlayerX, hutPlayerY] = '@';
                    inHut = true;
                    hasArtifact = false;

                    for (int i = 0; i < 25; i++)
                    {
                        for (int j = 0; j < 25; j++)
                        {
                            if (hutMap[i, j] == 'B')
                            {
                                babaX = i;
                                babaY = j;
                                break;
                            }
                        }
                    }

                    Console.SetCursorPosition(0, 28);
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("🏚️ ВЫ ВОШЛИ В ДОМИК БАБЫ ЯГИ! 🏚️");
                    Console.WriteLine("   Баба Яга двигается по полю! Найдите сундук (C) с артефактом!");
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(3000);
                    return;
                }

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
                else if (cell == 'H')
                {
                    hero.MaxHP += 10;
                    hero.HP += hero.MaxHP / 10;
                    fullMap[newX, newY] = groundTypes[newX, newY];
                }
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
                else if (cell == '+')
                {
                    Forge(hero);
                    fullMap[newX, newY] = groundTypes[newX, newY];
                }

                char groundType = fullMap[newX, newY];
                if ((groundType == '~' || groundType == '#') && groundTypes[newX, newY] == '.')
                    groundTypes[newX, newY] = groundType;

                fullMap[playerX, playerY] = groundTypes[playerX, playerY];
                playerX = newX;
                playerY = newY;
                fullMap[playerX, playerY] = '@';

                CheckAndSpawnPortal(fullMap, ref playerX, ref playerY);
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }

        public static void Forge(Person Hero)
        {
            try
            {
                if (Hero == null)
                    throw new GameException("Объект героя не инициализирован", "M011", "Map", ErrorSeverity.High);

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
                        Console.WriteLine("\nНедостаточно монет!");
                }
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
            }
        }
    }
}