using System;
using System.Collections.Generic;

namespace ConsoleApp46
{
    /// <summary>
    /// Класс, отвечающий за генерацию, отображение и управление игровой картой
    /// </summary>
    public class Map
    {
        private static char[,] _groundTypes;
        private static readonly Random _random = new Random();

        /// <summary>
        /// Текущий уровень мира
        /// </summary>
        public static int LevelWorld = 1;

        private const int MapWidth = 1500;
        private const int MapHeight = 1500;
        private const int ViewWidth = 25;
        private const int ViewHeight = 25;

        // ==================== ОСНОВНЫЕ МЕТОДЫ ====================

        /// <summary>
        /// Отображает фрагмент карты размером 25x25 вокруг игрока
        /// </summary>
        /// <param name="fullMap">Двумерный массив символов, представляющий полную карту</param>
        /// <param name="playerWorldX">Координата X игрока на карте</param>
        /// <param name="playerWorldY">Координата Y игрока на карте</param>
        public static void GetMap(char[,] fullMap, int playerWorldX, int playerWorldY)
        {
            try
            {
                if (fullMap == null)
                {
                    throw new GameException("Карта не инициализирована", "M001", "Map", ErrorSeverity.Critical);
                }

                Console.Clear();

                int startX = playerWorldX - ViewWidth / 2;
                int startY = playerWorldY - ViewHeight / 2;

                startX = Math.Max(0, Math.Min(startX, MapHeight - ViewHeight));
                startY = Math.Max(0, Math.Min(startY, MapWidth - ViewWidth));

                for (int i = 0; i < ViewHeight; i++)
                {
                    for (int j = 0; j < ViewWidth; j++)
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

        /// <summary>
        /// Отрисовывает отдельную клетку карты с соответствующим цветом
        /// </summary>
        /// <param name="cell">Символ клетки</param>
        private static void DrawCell(char cell)
        {
            switch (cell)
            {
                case '0':
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case '&':
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case 'H':
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case '+':
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case '%':
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case '@':
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case '^':
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case '~':
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case '#':
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case 'O':
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case 'T':
                case 'F':
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case 'o':
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case '★':
                case 'C':
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case 'K':
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case 'W':
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case '*':
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }
            Console.Write(cell + " ");
            Console.ResetColor();
        }

        /// <summary>
        /// Создает полную карту мира размером 1500x1500 со всеми объектами
        /// </summary>
        /// <returns>Двумерный массив символов, представляющий карту мира</returns>
        public static char[,] CreateFullMap()
        {
            try
            {
                char[,] fullMap = new char[MapHeight, MapWidth];
                _groundTypes = new char[MapHeight, MapWidth];

                for (int i = 0; i < MapHeight; i++)
                {
                    for (int j = 0; j < MapWidth; j++)
                    {
                        fullMap[i, j] = '.';
                        _groundTypes[i, j] = '.';
                    }
                }

                for (int i = 0; i < 1600; i++)
                {
                    GenerateRiver(fullMap);
                }

                for (int i = 0; i < 2000; i++)
                {
                    GenerateForest(fullMap);
                }

                for (int i = 0; i < 2700; i++)
                {
                    int x = _random.Next(20, MapHeight - 20);
                    int y = _random.Next(20, MapWidth - 20);
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

        /// <summary>
        /// Генерирует извилистую реку на карте
        /// </summary>
        /// <param name="fullMap">Карта, на которой генерируется река</param>
        private static void GenerateRiver(char[,] fullMap)
        {
            try
            {
                int startX = _random.Next(10, MapHeight - 10);
                int startY = _random.Next(10, MapWidth - 10);
                int riverLength = _random.Next(100, 301);
                int currentX = startX;
                int currentY = startY;
                int direction = _random.Next(4);

                for (int step = 0; step < riverLength; step++)
                {
                    if (currentX >= 0 && currentX < MapHeight && currentY >= 0 && currentY < MapWidth)
                    {
                        if (fullMap[currentX, currentY] != '^')
                        {
                            fullMap[currentX, currentY] = '~';
                        }
                    }
                    else
                    {
                        break;
                    }

                    if (_random.Next(100) < 30)
                    {
                        direction = _random.Next(4);
                    }

                    switch (direction)
                    {
                        case 0:
                            currentX--;
                            break;
                        case 1:
                            currentY++;
                            break;
                        case 2:
                            currentX++;
                            break;
                        case 3:
                            currentY--;
                            break;
                    }

                    if (currentX < 5 || currentX >= MapHeight - 5 || currentY < 5 || currentY >= MapWidth - 5)
                    {
                        direction = (direction + 2) % 4;
                        currentX = Math.Max(5, Math.Min(currentX, MapHeight - 6));
                        currentY = Math.Max(5, Math.Min(currentY, MapWidth - 6));
                    }
                }
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
            }
        }

        /// <summary>
        /// Генерирует лесной массив на карте
        /// </summary>
        /// <param name="fullMap">Карта, на которой генерируется лес</param>
        private static void GenerateForest(char[,] fullMap)
        {
            try
            {
                int centerX = _random.Next(10, MapHeight - 10);
                int centerY = _random.Next(10, MapWidth - 10);
                int radius = _random.Next(5, 16);
                int density = _random.Next(40, 81);

                for (int x = centerX - radius; x <= centerX + radius; x++)
                {
                    for (int y = centerY - radius; y <= centerY + radius; y++)
                    {
                        if (x < 0 || x >= MapHeight || y < 0 || y >= MapWidth)
                        {
                            continue;
                        }

                        double distance = Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));
                        if (distance <= radius)
                        {
                            double probability = density / 100.0 * (1 - (distance / radius) * 0.5);
                            if (_random.NextDouble() < probability && fullMap[x, y] == '.')
                            {
                                fullMap[x, y] = '#';
                            }
                        }
                    }
                }
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
            }
        }

        /// <summary>
        /// Создает гору с центром в указанных координатах
        /// </summary>
        /// <param name="fullMap">Карта, на которой создается гора</param>
        /// <param name="centerX">Координата X центра горы</param>
        /// <param name="centerY">Координата Y центра горы</param>
        private static void CreateMountain(char[,] fullMap, int centerX, int centerY)
        {
            try
            {
                if (centerX < 0 || centerX >= fullMap.GetLength(0) || centerY < 0 || centerY >= fullMap.GetLength(1))
                {
                    throw new GameException("Координаты горы вне границ карты", "M005", "Map", ErrorSeverity.Medium);
                }

                fullMap[centerX, centerY] = '^';
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0)
                        {
                            continue;
                        }

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
                                    if (_random.Next(100) < probabilities[circle - 2])
                                    {
                                        fullMap[x, y] = '^';
                                    }
                                }
                            }
                        }
                    }
                }

                if (_random.Next(100) < 30)
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
                                    if (_random.Next(100) < 25)
                                    {
                                        fullMap[x, y] = '^';
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
            }
        }

        /// <summary>
        /// Генерирует входы в уникальные локации (пещера, Титаник, домик)
        /// </summary>
        /// <param name="fullMap">Карта, на которой создаются порталы</param>
        private static void GeneratePortals(char[,] fullMap)
        {
            try
            {
                int cavePortals = 0;
                while (cavePortals < 800)
                {
                    int x = _random.Next(5, MapHeight - 5);
                    int y = _random.Next(5, MapWidth - 5);
                    if (fullMap[x, y] == '.' && HasNearby(fullMap, x, y, '^'))
                    {
                        fullMap[x, y] = 'O';
                        _groundTypes[x, y] = 'O';
                        cavePortals++;
                    }
                }

                int titanicPortals = 0;
                while (titanicPortals < 400)
                {
                    int x = _random.Next(5, MapHeight - 5);
                    int y = _random.Next(5, MapWidth - 5);
                    if (fullMap[x, y] == '.' && HasNearby(fullMap, x, y, '~'))
                    {
                        fullMap[x, y] = 'T';
                        _groundTypes[x, y] = 'T';
                        titanicPortals++;
                    }
                }

                int housePortals = 0;
                while (housePortals < 300)
                {
                    int x = _random.Next(5, MapHeight - 5);
                    int y = _random.Next(5, MapWidth - 5);
                    if (fullMap[x, y] == '.' && HasNearby(fullMap, x, y, '#'))
                    {
                        fullMap[x, y] = 'F';
                        _groundTypes[x, y] = 'F';
                        housePortals++;
                    }
                }
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
            }
        }

        /// <summary>
        /// Проверяет наличие целевого символа в соседних клетках
        /// </summary>
        /// <param name="fullMap">Карта</param>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <param name="target">Целевой символ</param>
        /// <returns>true, если рядом есть целевой символ</returns>
        private static bool HasNearby(char[,] fullMap, int x, int y, char target)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    int nx = x + dx;
                    int ny = y + dy;
                    if (nx >= 0 && nx < MapHeight && ny >= 0 && ny < MapWidth)
                    {
                        if (fullMap[nx, ny] == target)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Генерирует врагов, сердца и стены на карте
        /// </summary>
        /// <param name="fullMap">Карта</param>
        private static void GenerateObjects(char[,] fullMap)
        {
            try
            {
                for (int i = 0; i < MapHeight; i++)
                {
                    for (int j = 0; j < MapWidth; j++)
                    {
                        if (_random.Next(100) < 3 && fullMap[i, j] == '.')
                        {
                            fullMap[i, j] = '&';
                        }
                    }
                }

                for (int i = 0; i < MapHeight; i++)
                {
                    for (int j = 0; j < MapWidth; j++)
                    {
                        if (_random.Next(100) < 3 && fullMap[i, j] == '.')
                        {
                            fullMap[i, j] = 'H';
                        }
                    }
                }

                for (int i = 0; i < MapHeight; i++)
                {
                    for (int j = 0; j < MapWidth; j++)
                    {
                        int count = _random.Next(100);
                        if (count >= 10 && count < 13 && fullMap[i, j] == '.')
                        {
                            fullMap[i, j] = '%';
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка генерации объектов: {ex.Message}");
            }
        }

        /// <summary>
        /// Проверяет, есть ли враги в видимой области 25x25 вокруг игрока
        /// </summary>
        /// <param name="fullMap">Карта</param>
        /// <param name="playerX">Координата X игрока</param>
        /// <param name="playerY">Координата Y игрока</param>
        /// <returns>true, если враги найдены, иначе false</returns>
        public static bool HasEnemiesInView(char[,] fullMap, int playerX, int playerY)
        {
            try
            {
                if (fullMap == null)
                {
                    throw new GameException("Карта не инициализирована", "M006", "Map", ErrorSeverity.Critical);
                }

                int startX = playerX - ViewWidth / 2;
                int startY = playerY - ViewHeight / 2;
                startX = Math.Max(0, Math.Min(startX, MapHeight - ViewHeight));
                startY = Math.Max(0, Math.Min(startY, MapWidth - ViewWidth));

                for (int i = 0; i < ViewHeight; i++)
                {
                    for (int j = 0; j < ViewWidth; j++)
                    {
                        if (fullMap[startX + i, startY + j] == '&')
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
                return false;
            }
        }

        /// <summary>
        /// Проверяет, есть ли портал на карте
        /// </summary>
        /// <param name="fullMap">Карта</param>
        /// <returns>true, если портал найден, иначе false</returns>
        public static bool IsPortalOnMap(char[,] fullMap)
        {
            for (int i = 0; i < fullMap.GetLength(0); i++)
            {
                for (int j = 0; j < fullMap.GetLength(1); j++)
                {
                    if (fullMap[i, j] == '0')
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Создает портал рядом с игроком, если в видимой области нет врагов
        /// </summary>
        /// <param name="fullMap">Карта</param>
        /// <param name="playerX">Координата X игрока</param>
        /// <param name="playerY">Координата Y игрока</param>
        public static void CheckAndSpawnPortal(char[,] fullMap, ref int playerX, ref int playerY)
        {
            try
            {
                if (!HasEnemiesInView(fullMap, playerX, playerY) && !IsPortalOnMap(fullMap))
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0)
                            {
                                continue;
                            }

                            int portalX = playerX + dx;
                            int portalY = playerY + dy;
                            if (portalX >= 5 && portalX < fullMap.GetLength(0) - 5 &&
                                portalY >= 5 && portalY < fullMap.GetLength(1) - 5 &&
                                fullMap[portalX, portalY] == '.')
                            {
                                fullMap[portalX, portalY] = '0';
                                _groundTypes[portalX, portalY] = '.';
                                Console.SetCursorPosition(0, 30);
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("⭐ ПОРТАЛ ПОЯВИЛСЯ РЯДОМ! ⭐");
                                Console.ResetColor();
                                System.Threading.Thread.Sleep(2000);
                                Console.SetCursorPosition(0, 30);
                                Console.WriteLine(new string(' ', 60));
                                return;
                            }
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

        /// <summary>
        /// Создает карту пещеры с загадкой (камни и цели)
        /// </summary>
        /// <returns>Карта пещеры 25x25</returns>
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

            for (int i = 0; i < 25; i++)
            {
                caveMap[0, i] = '#';
                caveMap[24, i] = '#';
                caveMap[i, 0] = '#';
                caveMap[i, 24] = '#';
            }

            caveMap[5, 5] = 'O';
            caveMap[5, 19] = 'O';
            caveMap[19, 5] = 'O';
            caveMap[19, 19] = 'O';

            caveMap[3, 12] = 'o';
            caveMap[12, 3] = 'o';
            caveMap[12, 21] = 'o';
            caveMap[21, 12] = 'o';

            caveMap[12, 12] = ' ';
            return caveMap;
        }

        /// <summary>
        /// Отображает карту пещеры с загадкой
        /// </summary>
        /// <param name="caveMap">Карта пещеры</param>
        /// <param name="hero">Объект героя</param>
        /// <param name="puzzleSolved">Флаг решения загадки</param>
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
            Console.WriteLine($"\nЗдоровье: {hero.HP}/{hero.MaxHP} | Монет: {hero.Coins}");
            Console.WriteLine("Стрелки - движение | I - инвентарь | S - сохранить | L - загрузить");
        }

        /// <summary>
        /// Проверяет, решена ли загадка в пещере
        /// </summary>
        /// <param name="caveMap">Карта пещеры</param>
        /// <returns>true, если все камни на целях</returns>
        public static bool CheckCavePuzzleSolved(char[,] caveMap)
        {
            if (caveMap[5, 5] != 'o')
            {
                return false;
            }

            if (caveMap[5, 19] != 'o')
            {
                return false;
            }

            if (caveMap[19, 5] != 'o')
            {
                return false;
            }

            if (caveMap[19, 19] != 'o')
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Толкает камень в пещере на 1 клетку в направлении движения
        /// </summary>
        /// <param name="caveMap">Карта пещеры</param>
        /// <param name="playerX">Координата X игрока</param>
        /// <param name="playerY">Координата Y игрока</param>
        /// <param name="dx">Смещение по X</param>
        /// <param name="dy">Смещение по Y</param>
        public static void PushStoneInCave(ref char[,] caveMap, ref int playerX, ref int playerY, int dx, int dy)
        {
            int stoneX = playerX + dx;
            int stoneY = playerY + dy;

            if (stoneX < 0 || stoneX >= 25 || stoneY < 0 || stoneY >= 25)
            {
                return;
            }

            if (caveMap[stoneX, stoneY] != 'o')
            {
                return;
            }

            int newX = stoneX + dx;
            int newY = stoneY + dy;

            if (newX < 0 || newX >= 25 || newY < 0 || newY >= 25)
            {
                return;
            }

            char targetCell = caveMap[newX, newY];
            if (targetCell == '#' || targetCell == 'o')
            {
                return;
            }

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

        /// <summary>
        /// Обрабатывает движение игрока в пещере с загадкой
        /// </summary>
        /// <param name="playerX">Координата X игрока</param>
        /// <param name="playerY">Координата Y игрока</param>
        /// <param name="dx">Смещение по X</param>
        /// <param name="dy">Смещение по Y</param>
        /// <param name="caveMap">Карта пещеры</param>
        /// <param name="inCave">Флаг нахождения в пещере</param>
        /// <param name="puzzleSolved">Флаг решения загадки</param>
        /// <param name="hero">Объект героя</param>
        public static void MoveInCaveWithPuzzle(ref int playerX, ref int playerY, int dx, int dy,
            ref char[,] caveMap, ref bool inCave, ref bool puzzleSolved, Person hero)
        {
            int newX = playerX + dx;
            int newY = playerY + dy;

            if (newX < 0 || newX >= 25 || newY < 0 || newY >= 25)
            {
                return;
            }

            char cell = caveMap[newX, newY];

            if (cell == '★' && puzzleSolved)
            {
                inCave = false;
                Console.SetCursorPosition(0, 30);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("⛰️ ВЫ ВЫШЛИ ИЗ ПЕЩЕРЫ! ⛰️");
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
                Console.SetCursorPosition(0, 30);
                Console.WriteLine(new string(' ', 60));
                return;
            }

            if (cell == '#')
            {
                return;
            }

            if (cell == 'o')
            {
                PushStoneInCave(ref caveMap, ref playerX, ref playerY, dx, dy);

                if (!puzzleSolved && CheckCavePuzzleSolved(caveMap))
                {
                    puzzleSolved = true;
                    caveMap[12, 12] = '★';

                    Console.SetCursorPosition(0, 30);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("✨ ЗАГАДКА РЕШЕНА! Выход открыт в центре пещеры! ✨");
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(2000);

                    hero.Coins += 200;
                    hero.MaxHP += 20;
                    hero.HP += 20;
                    if (hero.HP > hero.MaxHP)
                    {
                        hero.HP = hero.MaxHP;
                    }

                    Console.SetCursorPosition(0, 30);
                    Console.WriteLine(new string(' ', 60));
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

                Console.SetCursorPosition(0, 30);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("✨ ЗАГАДКА РЕШЕНА! Выход открыт в центре пещеры! ✨");
                Console.ResetColor();
                System.Threading.Thread.Sleep(2000);

                hero.Coins += 200;
                hero.MaxHP += 20;
                hero.HP += 20;
                if (hero.HP > hero.MaxHP)
                {
                    hero.HP = hero.MaxHP;
                }

                Console.SetCursorPosition(0, 30);
                Console.WriteLine(new string(' ', 60));
            }
        }

        // ==================== ЛОКАЦИЯ ТИТАНИК ====================

        /// <summary>
        /// Создает карту Титаника с рыбами, водорослями и течениями
        /// </summary>
        /// <returns>Карта Титаника 25x25</returns>
        public static char[,] CreateTitanicMap()
        {
            char[,] titanicMap = new char[25, 25];

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    titanicMap[i, j] = '~';
                }
            }

            titanicMap[12, 23] = 'T';

            int seaweedCount = _random.Next(30, 50);
            for (int i = 0; i < seaweedCount; i++)
            {
                int x = _random.Next(1, 24);
                int y = _random.Next(1, 24);
                if (titanicMap[x, y] == '~' && !(x == 12 && y == 23))
                {
                    titanicMap[x, y] = 'W';
                }
            }

            int fishCount = _random.Next(8, 15);
            for (int i = 0; i < fishCount; i++)
            {
                int x = _random.Next(1, 24);
                int y = _random.Next(1, 24);
                if (titanicMap[x, y] == '~' && !(x == 12 && y == 23))
                {
                    titanicMap[x, y] = 'F';
                }
            }

            int currentCount = _random.Next(40, 60);
            for (int i = 0; i < currentCount; i++)
            {
                int x = _random.Next(1, 24);
                int y = _random.Next(1, 24);
                if (titanicMap[x, y] == '~' && !(x == 12 && y == 23))
                {
                    titanicMap[x, y] = '*';
                }
            }

            return titanicMap;
        }

        /// <summary>
        /// Отображает карту Титаника
        /// </summary>
        /// <param name="titanicMap">Карта Титаника</param>
        /// <param name="hero">Объект героя</param>
        /// <param name="fishCount">Количество пойманной рыбы</param>
        public static void RenderTitanicMap(char[,] titanicMap, Person hero, int fishCount)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                     ЗАТОНУВШИЙ ТИТАНИК                   ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  ~ Вода (отнимает 1 HP за шаг)                         ║");
            Console.WriteLine("║  F Рыба - можно поймать                                ║");
            Console.WriteLine("║  W Водоросли - нельзя пройти                           ║");
            Console.WriteLine("║  * Подводное течение - уносит в случайном направлении   ║");
            Console.WriteLine("║  T Выход из Титаника                                   ║");
            Console.WriteLine($"║  🎣 Поймано рыбы: {fishCount}                             ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    char cell = titanicMap[i, j];

                    if (cell == '@')
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("@ ");
                        Console.ResetColor();
                    }
                    else if (cell == 'T')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("T ");
                        Console.ResetColor();
                    }
                    else if (cell == 'W')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write("W ");
                        Console.ResetColor();
                    }
                    else if (cell == 'F')
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("F ");
                        Console.ResetColor();
                    }
                    else if (cell == '*')
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("* ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write("~ ");
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine($"\nЗдоровье: {hero.HP}/{hero.MaxHP} | Монет: {hero.Coins}");
            Console.WriteLine("Стрелки - движение | I - инвентарь | S - сохранить | L - загрузить");
        }

        /// <summary>
        /// Применяет эффект течения на игрока
        /// </summary>
        /// <param name="playerX">Координата X игрока</param>
        /// <param name="playerY">Координата Y игрока</param>
        /// <param name="titanicMap">Карта Титаника</param>
        /// <returns>true, если игрок был перемещен</returns>
        private static bool ApplyCurrent(ref int playerX, ref int playerY, char[,] titanicMap)
        {
            int direction = _random.Next(4);
            int dx = 0;
            int dy = 0;

            switch (direction)
            {
                case 0:
                    dx = -1;
                    break;
                case 1:
                    dy = 1;
                    break;
                case 2:
                    dx = 1;
                    break;
                case 3:
                    dy = -1;
                    break;
            }

            int newX = playerX;
            int newY = playerY;
            int steps = _random.Next(2, 5);

            for (int step = 0; step < steps; step++)
            {
                int nextX = newX + dx;
                int nextY = newY + dy;

                if (nextX < 0 || nextX >= 25 || nextY < 0 || nextY >= 25)
                {
                    break;
                }

                if (titanicMap[nextX, nextY] == 'W' || titanicMap[nextX, nextY] == 'T')
                {
                    break;
                }

                newX = nextX;
                newY = nextY;
            }

            if (newX != playerX || newY != playerY)
            {
                playerX = newX;
                playerY = newY;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Обрабатывает движение игрока в Титанике
        /// </summary>
        /// <param name="titanicPlayerX">Координата X игрока в Титанике</param>
        /// <param name="titanicPlayerY">Координата Y игрока в Титанике</param>
        /// <param name="dx">Смещение по X</param>
        /// <param name="dy">Смещение по Y</param>
        /// <param name="titanicMap">Карта Титаника</param>
        /// <param name="inTitanic">Флаг нахождения в Титанике</param>
        /// <param name="hero">Объект героя</param>
        /// <param name="fishCount">Количество пойманной рыбы</param>
        /// <param name="hasFish">Флаг наличия рыбы</param>
        public static void MoveInTitanic(ref int titanicPlayerX, ref int titanicPlayerY, int dx, int dy,
            char[,] titanicMap, ref bool inTitanic, Person hero, ref int fishCount, ref bool hasFish)
        {
            int newX = titanicPlayerX + dx;
            int newY = titanicPlayerY + dy;

            if (newX < 0 || newX >= 25 || newY < 0 || newY >= 25)
            {
                return;
            }

            char cell = titanicMap[newX, newY];

            if (cell == 'T')
            {
                inTitanic = false;
                hasFish = (fishCount > 0);
                Console.SetCursorPosition(0, 30);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"🚢 ВЫ ВЫШЛИ ИЗ ТИТАНИКА! Рыба осталась в инвентаре: {fishCount} 🚢");
                Console.ResetColor();
                System.Threading.Thread.Sleep(2000);
                Console.SetCursorPosition(0, 30);
                Console.WriteLine(new string(' ', 60));
                return;
            }

            if (cell == 'W')
            {
                Console.SetCursorPosition(0, 30);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("🌿 Вы не можете пройти через водоросли! 🌿");
                Console.ResetColor();
                System.Threading.Thread.Sleep(800);
                Console.SetCursorPosition(0, 30);
                Console.WriteLine(new string(' ', 60));
                return;
            }

            if (cell == 'F')
            {
                fishCount++;
                titanicMap[newX, newY] = '~';
                hasFish = (fishCount > 0);
                Console.SetCursorPosition(0, 30);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"🐟 Вы поймали рыбу! Всего рыбы: {fishCount} 🐟");
                Console.ResetColor();
                System.Threading.Thread.Sleep(800);
                Console.SetCursorPosition(0, 30);
                Console.WriteLine(new string(' ', 60));
            }

            int damage = 1;
            hero.HP -= damage;

            Console.SetCursorPosition(0, 30);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"🌊 Холодная вода! Вы потеряли {damage} здоровья! 🌊");
            Console.ResetColor();
            System.Threading.Thread.Sleep(500);
            Console.SetCursorPosition(0, 30);
            Console.WriteLine(new string(' ', 60));

            if (hero.HP <= 0)
            {
                Console.Clear();
                Console.WriteLine("🌊 ВЫ УТОНУЛИ В ЛЕДЯНОЙ ВОДЕ! 🌊");
                Console.WriteLine("Ваше тело ушло на дно океана...");
                Console.ReadKey();
                return;
            }

            titanicMap[titanicPlayerX, titanicPlayerY] = '~';
            titanicPlayerX = newX;
            titanicPlayerY = newY;

            char currentCell = titanicMap[titanicPlayerX, titanicPlayerY];

            if (currentCell == '*')
            {
                Console.SetCursorPosition(0, 30);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("🌊 Вас подхватило подводное течение! 🌊");
                Console.ResetColor();

                int oldX = titanicPlayerX;
                int oldY = titanicPlayerY;

                bool moved = ApplyCurrent(ref titanicPlayerX, ref titanicPlayerY, titanicMap);

                if (moved)
                {
                    titanicMap[oldX, oldY] = '~';
                    titanicMap[titanicPlayerX, titanicPlayerY] = '@';
                    System.Threading.Thread.Sleep(1500);
                }
                else
                {
                    titanicMap[titanicPlayerX, titanicPlayerY] = '@';
                }

                Console.SetCursorPosition(0, 30);
                Console.WriteLine(new string(' ', 60));
            }
            else
            {
                titanicMap[titanicPlayerX, titanicPlayerY] = '@';
            }

            System.Threading.Thread.Sleep(300);
        }

        // ==================== ЛОКАЦИЯ ДОМИК ====================

        /// <summary>
        /// Создает карту домика
        /// </summary>
        /// <returns>Карта домика 25x25</returns>
        public static char[,] CreateHouseMap()
        {
            char[,] houseMap = new char[25, 25];

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    houseMap[i, j] = '.';
                }
            }

            for (int i = 0; i < 25; i++)
            {
                houseMap[0, i] = '#';
                houseMap[24, i] = '#';
                houseMap[i, 0] = '#';
                houseMap[i, 24] = '#';
            }

            for (int i = 8; i <= 16; i++)
            {
                for (int j = 8; j <= 16; j++)
                {
                    if (i == 8 || i == 16 || j == 8 || j == 16)
                    {
                        houseMap[i, j] = '#';
                    }
                }
            }

            houseMap[12, 8] = '.';
            houseMap[14, 14] = 'K';
            houseMap[12, 20] = 'F';

            return houseMap;
        }

        /// <summary>
        /// Отображает карту домика
        /// </summary>
        /// <param name="houseMap">Карта домика</param>
        /// <param name="hero">Объект героя</param>
        /// <param name="hasFish">Флаг наличия рыбы</param>
        /// <param name="hasReward">Флаг получения награды</param>
        /// <param name="catCatched">Флаг поимки кота</param>
        public static void RenderHouseMap(char[,] houseMap, Person hero, bool hasFish, bool hasReward, bool catCatched)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                       ДОМИК                             ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  Кот (K) бегает по всему домику!                       ║");
            if (!catCatched)
            {
                Console.WriteLine("║  Подойдите к коту, чтобы поймать его!                 ║");
                Console.WriteLine("║  Кот убегает, если вы подходите без рыбы!            ║");
            }
            else
            {
                Console.WriteLine("║  ✨ Кот пойман! Можете выходить через F ✨             ║");
            }

            if (!hasFish && !catCatched)
            {
                Console.WriteLine("║  🐟 Вам нужна рыба, чтобы поймать кота!                ║");
                Console.WriteLine("║     Поймайте рыбу в Титанике!                         ║");
            }
            else if (hasFish && !catCatched)
            {
                Console.WriteLine("║  🐟 У вас есть рыба! Подойдите к коту (K)!             ║");
            }

            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    char cell = houseMap[i, j];

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
                    else if (cell == 'K')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("K ");
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
            if (!catCatched)
            {
                Console.WriteLine("🐱 Кот бегает по всему домику!");
                if (!hasFish)
                {
                    Console.WriteLine("🐟 Вам нужно поймать рыбу в Титанике, чтобы поймать кота!");
                    Console.WriteLine("🐱 Подойдите к коту - он убежит, если у вас нет рыбы!");
                }
                else
                {
                    Console.WriteLine("🐟 У вас есть рыба! Подойдите к коту (K), чтобы поймать его!");
                }
            }
            else
            {
                Console.WriteLine("✨ Кот пойман! Можете выходить через F");
            }

            Console.WriteLine($"\nЗдоровье: {hero.HP}/{hero.MaxHP} | Монет: {hero.Coins} | 🐟 Рыба: {(hasFish ? "есть" : "нет")}");
            Console.WriteLine("Стрелки - движение | I - инвентарь | S - сохранить | L - загрузить");
        }

        /// <summary>
        /// Двигает кота по карте в случайном направлении
        /// </summary>
        /// <param name="houseMap">Карта домика</param>
        /// <param name="catX">Координата X кота</param>
        /// <param name="catY">Координата Y кота</param>
        private static void MoveCat(ref char[,] houseMap, ref int catX, ref int catY)
        {
            if (catX == -1 && catY == -1)
            {
                return;
            }

            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            List<int> directions = new List<int> { 0, 1, 2, 3 };
            for (int i = 0; i < directions.Count; i++)
            {
                int randomIndex = _random.Next(i, directions.Count);
                int temp = directions[i];
                directions[i] = directions[randomIndex];
                directions[randomIndex] = temp;
            }

            bool moved = false;
            foreach (int dir in directions)
            {
                int newX = catX + dx[dir];
                int newY = catY + dy[dir];

                if (newX < 0 || newX >= 25 || newY < 0 || newY >= 25)
                {
                    continue;
                }

                char targetCell = houseMap[newX, newY];

                if (targetCell == '.' || targetCell == '@')
                {
                    houseMap[catX, catY] = '.';
                    catX = newX;
                    catY = newY;
                    houseMap[catX, catY] = 'K';
                    moved = true;
                    break;
                }
            }

            if (moved && _random.Next(100) < 30)
            {
                foreach (int dir in directions)
                {
                    int newX = catX + dx[dir];
                    int newY = catY + dy[dir];

                    if (newX < 0 || newX >= 25 || newY < 0 || newY >= 25)
                    {
                        continue;
                    }

                    char targetCell = houseMap[newX, newY];

                    if (targetCell == '.' || targetCell == '@')
                    {
                        houseMap[catX, catY] = '.';
                        catX = newX;
                        catY = newY;
                        houseMap[catX, catY] = 'K';
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Обрабатывает движение игрока в домике
        /// </summary>
        /// <param name="housePlayerX">Координата X игрока в домике</param>
        /// <param name="housePlayerY">Координата Y игрока в домике</param>
        /// <param name="dx">Смещение по X</param>
        /// <param name="dy">Смещение по Y</param>
        /// <param name="houseMap">Карта домика</param>
        /// <param name="inHouse">Флаг нахождения в домике</param>
        /// <param name="hasFish">Флаг наличия рыбы</param>
        /// <param name="hasReward">Флаг получения награды</param>
        /// <param name="catX">Координата X кота</param>
        /// <param name="catY">Координата Y кота</param>
        /// <param name="catCatched">Флаг поимки кота</param>
        /// <param name="fishCount">Количество пойманной рыбы</param>
        /// <param name="hero">Объект героя</param>
        public static void MoveInHouse(ref int housePlayerX, ref int housePlayerY, int dx, int dy,
            ref char[,] houseMap, ref bool inHouse, ref bool hasFish, ref bool hasReward,
            ref int catX, ref int catY, ref bool catCatched, ref int fishCount, Person hero)
        {
            int newX = housePlayerX + dx;
            int newY = housePlayerY + dy;

            if (newX < 0 || newX >= 25 || newY < 0 || newY >= 25)
            {
                return;
            }

            char cell = houseMap[newX, newY];

            if (cell == 'F')
            {
                inHouse = false;
                Console.SetCursorPosition(0, 30);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("🏠 ВЫ ВЫШЛИ ИЗ ДОМИКА! 🏠");
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
                Console.SetCursorPosition(0, 30);
                Console.WriteLine(new string(' ', 60));
                return;
            }

            if (cell == '#')
            {
                return;
            }

            if (cell == 'K' && !catCatched)
            {
                if (hasFish)
                {
                    fishCount--;
                    hasFish = (fishCount > 0);
                    catCatched = true;
                    hasReward = true;

                    houseMap[catX, catY] = '.';
                    catX = -1;
                    catY = -1;

                    Console.SetCursorPosition(0, 30);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("🐱 ВЫ ПОЙМАЛИ КОТА! Он был голодный и съел вашу рыбу! 🐱");
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(1500);

                    Console.SetCursorPosition(0, 31);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("✨ Кот благодарен! Вы получили 300 монет и +30 к здоровью! ✨");
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(2000);

                    hero.Coins += 300;
                    hero.MaxHP += 30;
                    hero.HP += 30;
                    if (hero.HP > hero.MaxHP)
                    {
                        hero.HP = hero.MaxHP;
                    }

                    houseMap[housePlayerX, housePlayerY] = '.';
                    housePlayerX = newX;
                    housePlayerY = newY;
                    houseMap[housePlayerX, housePlayerY] = '@';

                    Console.SetCursorPosition(0, 30);
                    Console.WriteLine(new string(' ', 60));
                    Console.SetCursorPosition(0, 31);
                    Console.WriteLine(new string(' ', 60));
                    return;
                }
                else
                {
                    int oldCatX = catX;
                    int oldCatY = catY;

                    int newCatX;
                    int newCatY;
                    do
                    {
                        newCatX = _random.Next(1, 24);
                        newCatY = _random.Next(1, 24);
                    }
                    while (houseMap[newCatX, newCatY] != '.' || (newCatX == housePlayerX && newCatY == housePlayerY));

                    houseMap[oldCatX, oldCatY] = '.';
                    catX = newCatX;
                    catY = newCatY;
                    houseMap[catX, catY] = 'K';

                    Console.SetCursorPosition(0, 30);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("🐱 Кот убежал! Вам нужна рыба, чтобы его поймать! 🐱");
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(1500);
                    Console.SetCursorPosition(0, 30);
                    Console.WriteLine(new string(' ', 60));
                    return;
                }
            }

            houseMap[housePlayerX, housePlayerY] = '.';
            housePlayerX = newX;
            housePlayerY = newY;
            houseMap[housePlayerX, housePlayerY] = '@';

            if (!catCatched)
            {
                MoveCat(ref houseMap, ref catX, ref catY);
            }
        }

        // ==================== ИНВЕНТАРЬ ====================

        /// <summary>
        /// Отображает инвентарь игрока
        /// </summary>
        /// <param name="hero">Объект героя</param>
        /// <param name="fishCount">Количество пойманной рыбы</param>
        /// <param name="hasArtifact">Флаг наличия артефакта</param>
        public static void ShowInventory(Person hero, int fishCount, bool hasArtifact)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      ИНВЕНТАРЬ                          ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
            Console.WriteLine($"║  Имя героя: {hero.Name}                                  ║");
            Console.WriteLine($"║  Здоровье: {hero.HP}/{hero.MaxHP}                        ║");
            Console.WriteLine($"║  Сила: {hero.Strength}                                  ║");
            Console.WriteLine($"║  Монет: {hero.Coins}                                    ║");
            Console.WriteLine($"║  🐟 Рыба: {fishCount} шт.                               ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  Предметы:                                              ║");
            if (hasArtifact)
            {
                Console.WriteLine("║  ✨ Артефакт (для победы над Бабой Ягой)             ║");
            }
            else
            {
                Console.WriteLine("║  ❌ Артефакт не найден                                ║");
            }
            if (fishCount > 0)
            {
                Console.WriteLine($"║  🐟 Рыба x{fishCount} (можно использовать в домике)      ║");
            }
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        // ==================== ОСНОВНОЕ ДВИЖЕНИЕ ====================

        /// <summary>
        /// Обрабатывает движение игрока на основной карте
        /// </summary>
        /// <param name="playerX">Координата X игрока</param>
        /// <param name="playerY">Координата Y игрока</param>
        /// <param name="dx">Смещение по X</param>
        /// <param name="dy">Смещение по Y</param>
        /// <param name="fullMap">Карта мира</param>
        /// <param name="hero">Объект героя</param>
        /// <param name="inCave">Флаг нахождения в пещере</param>
        /// <param name="caveMap">Карта пещеры</param>
        /// <param name="cavePlayerX">Координата X в пещере</param>
        /// <param name="cavePlayerY">Координата Y в пещере</param>
        /// <param name="inTitanic">Флаг нахождения в Титанике</param>
        /// <param name="titanicMap">Карта Титаника</param>
        /// <param name="titanicPlayerX">Координата X в Титанике</param>
        /// <param name="titanicPlayerY">Координата Y в Титанике</param>
        /// <param name="inHouse">Флаг нахождения в домике</param>
        /// <param name="houseMap">Карта домика</param>
        /// <param name="housePlayerX">Координата X в домике</param>
        /// <param name="housePlayerY">Координата Y в домике</param>
        /// <param name="puzzleSolved">Флаг решения загадки</param>
        /// <param name="hasFish">Флаг наличия рыбы</param>
        /// <param name="hasReward">Флаг получения награды</param>
        /// <param name="catX">Координата X кота</param>
        /// <param name="catY">Координата Y кота</param>
        /// <param name="catCatched">Флаг поимки кота</param>
        /// <param name="fishCount">Количество пойманной рыбы</param>
        public static void MovePlayer(ref int playerX, ref int playerY, int dx, int dy, char[,] fullMap, Person hero,
            ref bool inCave, ref char[,] caveMap, ref int cavePlayerX, ref int cavePlayerY,
            ref bool inTitanic, ref char[,] titanicMap, ref int titanicPlayerX, ref int titanicPlayerY,
            ref bool inHouse, ref char[,] houseMap, ref int housePlayerX, ref int housePlayerY,
            ref bool puzzleSolved, ref bool hasFish, ref bool hasReward, ref int catX, ref int catY,
            ref bool catCatched, ref int fishCount)
        {
            try
            {
                if (fullMap == null)
                {
                    throw new GameException("Карта не инициализирована", "M008", "Map", ErrorSeverity.Critical);
                }

                if (hero == null)
                {
                    throw new GameException("Объект героя не инициализирован", "M009", "Map", ErrorSeverity.Critical);
                }

                int newX = playerX + dx;
                int newY = playerY + dy;

                if (newX < 0 || newX >= fullMap.GetLength(0) || newY < 0 || newY >= fullMap.GetLength(1))
                {
                    throw new GameException("Попытка выйти за границы карты", "M010", "Map", ErrorSeverity.Medium);
                }

                char cell = fullMap[newX, newY];

                if (cell == '^' || cell == '%')
                {
                    Console.SetCursorPosition(0, 30);
                    Console.WriteLine("Вы не можете пройти!");
                    System.Threading.Thread.Sleep(500);
                    Console.SetCursorPosition(0, 30);
                    Console.WriteLine(new string(' ', 60));
                    return;
                }

                if (cell == 'O')
                {
                    inCave = true;
                    puzzleSolved = false;
                    cavePlayerX = 12;
                    cavePlayerY = 12;
                    caveMap = CreateCaveWithPuzzle();
                    caveMap[cavePlayerX, cavePlayerY] = '@';

                    Console.SetCursorPosition(0, 30);
                    Console.WriteLine("🧩 ВЫ ВОШЛИ В ПЕЩЕРУ С ЗАГАДКОЙ! 🧩");
                    Console.WriteLine("   Нужно поставить все камни (o) на желтые цели (O)!");
                    System.Threading.Thread.Sleep(3000);
                    Console.SetCursorPosition(0, 30);
                    Console.WriteLine(new string(' ', 60));
                    Console.SetCursorPosition(0, 31);
                    Console.WriteLine(new string(' ', 60));
                    return;
                }

                if (cell == 'T')
                {
                    titanicPlayerX = 12;
                    titanicPlayerY = 12;
                    titanicMap = CreateTitanicMap();
                    titanicMap[titanicPlayerX, titanicPlayerY] = '@';
                    inTitanic = true;

                    Console.SetCursorPosition(0, 30);
                    Console.WriteLine("🚢 ВЫ ПОПАЛИ НА ТИТАНИК! 🚢");
                    Console.WriteLine("   - Каждый шаг отнимает 1 HP");
                    Console.WriteLine("   - Ловите рыбу (F) для инвентаря");
                    Console.WriteLine("   - Обходите водоросли (W)");
                    Console.WriteLine("   - Остерегайтесь подводных течений (*)");
                    System.Threading.Thread.Sleep(4000);
                    Console.SetCursorPosition(0, 30);
                    Console.WriteLine(new string(' ', 60));
                    Console.SetCursorPosition(0, 31);
                    Console.WriteLine(new string(' ', 60));
                    Console.SetCursorPosition(0, 32);
                    Console.WriteLine(new string(' ', 60));
                    Console.SetCursorPosition(0, 33);
                    Console.WriteLine(new string(' ', 60));
                    Console.SetCursorPosition(0, 34);
                    Console.WriteLine(new string(' ', 60));
                    return;
                }

                if (cell == 'F')
                {
                    housePlayerX = 12;
                    housePlayerY = 12;
                    houseMap = CreateHouseMap();
                    houseMap[housePlayerX, housePlayerY] = '@';
                    inHouse = true;
                    hasReward = false;
                    catCatched = false;
                    hasFish = (fishCount > 0);

                    for (int i = 0; i < 25; i++)
                    {
                        for (int j = 0; j < 25; j++)
                        {
                            if (houseMap[i, j] == 'K')
                            {
                                catX = i;
                                catY = j;
                                break;
                            }
                        }
                    }

                    Console.SetCursorPosition(0, 30);
                    Console.WriteLine("🏠 ВЫ ВОШЛИ В ДОМИК! 🏠");
                    if (hasFish)
                    {
                        Console.WriteLine("   🐟 У вас есть рыба! Подойдите к коту (K), чтобы поймать его!");
                        Console.WriteLine("   🐱 Кот убегает, если вы подходите без рыбы!");
                    }
                    else
                    {
                        Console.WriteLine("   🐟 У вас нет рыбы! Поймайте рыбу в Титанике!");
                        Console.WriteLine("   🐱 Кот убегает, если вы к нему приближаетесь!");
                    }
                    System.Threading.Thread.Sleep(3000);
                    Console.SetCursorPosition(0, 30);
                    Console.WriteLine(new string(' ', 60));
                    Console.SetCursorPosition(0, 31);
                    Console.WriteLine(new string(' ', 60));
                    return;
                }

                if (cell == '&')
                {
                    Console.Clear();
                    Person enemy = new Person(LevelWorld * 10);
                    Random battleRandom = new Random();

                    while (enemy.HP > 0 && hero.HP > 0)
                    {
                        int shot = battleRandom.Next(10);
                        enemy.HP -= shot + hero.Strength;
                        shot = battleRandom.Next(10);
                        hero.HP -= shot + LevelWorld * 5;
                    }

                    if (enemy.HP < hero.HP)
                    {
                        hero.Coins += battleRandom.Next(100);
                        fullMap[newX, newY] = _groundTypes[newX, newY];
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
                    fullMap[newX, newY] = _groundTypes[newX, newY];
                }
                else if (cell == '0')
                {
                    Console.SetCursorPosition(0, 30);
                    Console.WriteLine("🌀 ВЫ ВОШЛИ В ПОРТАЛ! Переход на следующий уровень... 🌀");
                    System.Threading.Thread.Sleep(2000);

                    hero.HP = hero.MaxHP;
                    LevelWorld++;
                    fullMap[playerX, playerY] = _groundTypes[playerX, playerY];
                    playerX = fullMap.GetLength(0) / 2;
                    playerY = fullMap.GetLength(1) / 2;

                    char[,] newMap = CreateFullMap();
                    for (int i = 0; i < fullMap.GetLength(0); i++)
                    {
                        for (int j = 0; j < fullMap.GetLength(1); j++)
                        {
                            fullMap[i, j] = newMap[i, j];
                        }
                    }

                    fullMap[playerX, playerY] = '@';
                    return;
                }
                else if (cell == '+')
                {
                    Forge(hero);
                    fullMap[newX, newY] = _groundTypes[newX, newY];
                }

                char groundType = fullMap[newX, newY];
                if ((groundType == '~' || groundType == '#') && _groundTypes[newX, newY] == '.')
                {
                    _groundTypes[newX, newY] = groundType;
                }

                fullMap[playerX, playerY] = _groundTypes[playerX, playerY];
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

        /// <summary>
        /// Открывает меню кузницы для улучшения силы
        /// </summary>
        /// <param name="hero">Объект героя</param>
        public static void Forge(Person hero)
        {
            try
            {
                if (hero == null)
                {
                    throw new GameException("Объект героя не инициализирован", "M011", "Map", ErrorSeverity.High);
                }

                Console.Clear();
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1. Улучшить силу на 2 (250 монет)");
                Console.WriteLine("Для выхода нажмите Enter");
                Console.WriteLine($"Оставшиеся деньги: {hero.Coins}");

                ConsoleKey key;
                while ((key = Console.ReadKey().Key) != ConsoleKey.Enter)
                {
                    if (key == ConsoleKey.NumPad1 && hero.Coins > 250)
                    {
                        hero.Strength += 2;
                        hero.Coins -= 250;
                        Console.WriteLine($"\nСила увеличена! Текущая сила: {hero.Strength}");
                    }
                    else if (key == ConsoleKey.NumPad1)
                    {
                        Console.WriteLine("\nНедостаточно монет!");
                    }
                }
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
            }
        }
    }
}