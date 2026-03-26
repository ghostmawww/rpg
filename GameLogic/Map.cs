using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ConsoleApp46
{
    /// <summary>
    /// Класс, отвечающий за генерацию, отображение и управление игровой картой
    /// </summary>
    public class Map
    {
        #region Поля и константы

        [StructLayout(LayoutKind.Sequential)]
        private struct Coord
        {
            public short X;
            public short Y;

            public Coord(short x, short y)
            {
                X = x;
                Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        private struct CharInfo
        {
            [FieldOffset(0)]
            public char UnicodeChar;

            [FieldOffset(2)]
            public short Attributes;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool WriteConsoleOutput(
            IntPtr hConsoleOutput,
            CharInfo[] lpBuffer,
            Coord dwBufferSize,
            Coord dwBufferCoord,
            ref SmallRect lpWriteRegion);

        private static char[,] _groundTypes;
        private static readonly Random _random = new Random();
        private static readonly List<(int X, int Y)> _caveTargets = new List<(int X, int Y)>();
        private static char _titanicPlayerBaseCell = '.';
        private static string _statusMessage = string.Empty;
        private static ConsoleColor _statusColor = ConsoleColor.White;

        private const int MapWidth = 1500;
        private const int MapHeight = 1500;
        private const int ViewWidth = 25;
        private const int ViewHeight = 25;
        private const int CaveSize = 25;
        private const int CaveLogicalSize = 8;
        private const int CavePassageWidth = 2;
        private const int CaveStep = CavePassageWidth + 1;
        private const int CaveStartX = 1;
        private const int CaveStartY = 1;
        private const int CaveChestX = 22;
        private const int CaveChestY = 1;
        private const int CaveExitX = 22;
        private const int CaveExitY = 2;
        private const int TitanicMessageRow = 43;
        private const int HouseExitX = 22;
        private const int HouseExitY = 22;
        private const int FrameWidth = 60;
        private const int FrameHeight = 36;
        private const int StdOutputHandle = -11;

        /// <summary>
        /// Текущий уровень мира
        /// </summary>
        public static int LevelWorld = 1;

        #endregion

        #region Основные методы отображения

        /// <summary>
        /// Формирует строковое представление игрового мира
        /// </summary>
        public static string BuildWorldFrame(char[,] fullMap, int playerWorldX, int playerWorldY, Person hero)
        {
            try
            {
                if (fullMap == null)
                {
                    throw new GameException("Карта не инициализирована", "M001", "Map", ErrorSeverity.Critical);
                }

                StringBuilder frame = new StringBuilder();

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
                        frame.Append(fullMap[mapX, mapY]).Append(' ');
                    }
                    frame.AppendLine();
                }

                frame.AppendLine();
                frame.AppendLine($"Имя: {hero.Name}");
                frame.AppendLine($"Здоровье: {hero.HP}/{hero.MaxHP}");
                frame.AppendLine($"Сила: {hero.Strength}");
                frame.AppendLine($"Монеты: {hero.Coins}");
                frame.AppendLine($"Уровень: {LevelWorld}");
                if (hero.HasAquaLung) frame.AppendLine("Акваланг: есть");
                frame.Append("I - инвентарь | S - сохранить | L - загрузить");

                return frame.ToString();
            }
            catch (GameException ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Отображает фрагмент карты в консоли
        /// </summary>
        public static void GetMap(char[,] fullMap, int playerWorldX, int playerWorldY)
        {
            try
            {
                if (fullMap == null)
                {
                    throw new GameException("Карта не инициализирована", "M001", "Map", ErrorSeverity.Critical);
                }

                BeginFrame();
                Console.Write(BuildWorldFrame(fullMap, playerWorldX, playerWorldY, new Person(100, "Герой")));
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Отображает основной мир с цветами.
        /// </summary>
        /// <param name="fullMap">Полная карта мира.</param>
        /// <param name="playerWorldX">Координата игрока по X.</param>
        /// <param name="playerWorldY">Координата игрока по Y.</param>
        /// <param name="hero">Герой.</param>
        public static void RenderWorldMap(char[,] fullMap, int playerWorldX, int playerWorldY, Person hero)
        {
            if (fullMap == null)
            {
                throw new GameException("Карта не инициализирована", "M001", "Map", ErrorSeverity.Critical);
            }

            NormalizeWorldMap(fullMap, playerWorldX, playerWorldY);
            CharInfo[] buffer = CreateFrameBuffer(FrameWidth, FrameHeight);

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
                    bool isPlayerCell = mapX == playerWorldX && mapY == playerWorldY;
                    char visibleCell = isPlayerCell ? '@' : fullMap[mapX, mapY];
                    PutMapCell(buffer, FrameWidth, FrameHeight, j, i, visibleCell, GetCellColor(visibleCell));
                }
            }

            PutBufferText(buffer, FrameWidth, FrameHeight, 0, ViewHeight + 1, $"Имя: {hero.Name}", ConsoleColor.White);
            PutBufferText(buffer, FrameWidth, FrameHeight, 0, ViewHeight + 2, $"Здоровье: {hero.HP}/{hero.MaxHP}", ConsoleColor.White);
            PutBufferText(buffer, FrameWidth, FrameHeight, 0, ViewHeight + 3, $"Сила: {hero.Strength}", ConsoleColor.White);
            PutBufferText(buffer, FrameWidth, FrameHeight, 0, ViewHeight + 4, $"Монеты: {hero.Coins}", ConsoleColor.White);
            PutBufferText(buffer, FrameWidth, FrameHeight, 0, ViewHeight + 5, $"Уровень: {LevelWorld}", ConsoleColor.White);
            if (hero.HasAquaLung)
            {
                PutBufferText(buffer, FrameWidth, FrameHeight, 0, ViewHeight + 6, "Акваланг: есть", ConsoleColor.White);
            }

            PutBufferText(buffer, FrameWidth, FrameHeight, 0, ViewHeight + 7, "I - инвентарь | S - сохранить | L - загрузить", ConsoleColor.White);
            PutBufferText(buffer, FrameWidth, FrameHeight, 0, ViewHeight + 8, _statusMessage, _statusColor);
            PresentBuffer(buffer, FrameWidth, FrameHeight);
        }

        /// <summary>
        /// Удаляет устаревшие метки игрока с карты мира.
        /// </summary>
        /// <param name="fullMap">Карта мира.</param>
        /// <param name="playerWorldX">Текущая координата игрока по X.</param>
        /// <param name="playerWorldY">Текущая координата игрока по Y.</param>
        public static void NormalizeWorldMap(char[,] fullMap, int playerWorldX, int playerWorldY)
        {
            for (int i = 0; i < fullMap.GetLength(0); i++)
            {
                for (int j = 0; j < fullMap.GetLength(1); j++)
                {
                    if (fullMap[i, j] == '@' && (i != playerWorldX || j != playerWorldY))
                    {
                        fullMap[i, j] = '.';
                    }
                }
            }
        }

        /// <summary>
        /// Устанавливает курсор в начало консоли
        /// </summary>
        private static void BeginFrame()
        {
            Console.SetCursorPosition(0, 0);
        }

        /// <summary>
        /// Выводит готовый кадр в консоль одним действием.
        /// </summary>
        /// <param name="frame">Текст кадра.</param>
        private static void DrawFrame(string frame)
        {
            BeginFrame();
            Console.Write(frame);
        }

        /// <summary>
        /// Очищает текущую строку консоли.
        /// </summary>
        private static void ClearConsoleLine()
        {
            Console.Write(new string(' ', Math.Max(1, Console.WindowWidth - 1)));
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        /// <summary>
        /// Сохраняет последнее игровое сообщение.
        /// </summary>
        /// <param name="message">Текст сообщения.</param>
        /// <param name="color">Цвет сообщения.</param>
        private static void SetStatusMessage(string message, ConsoleColor color = ConsoleColor.White)
        {
            _statusMessage = message ?? string.Empty;
            _statusColor = color;
        }

        /// <summary>
        /// Отрисовывает последнее игровое сообщение.
        /// </summary>
        /// <param name="row">Строка вывода.</param>
        private static void DrawStatusMessage(int row)
        {
            Console.SetCursorPosition(0, row);
            ClearConsoleLine();
            Console.ForegroundColor = _statusColor;
            Console.Write(_statusMessage);
            Console.ResetColor();
        }

        /// <summary>
        /// Создает пустой буфер кадра.
        /// </summary>
        private static CharInfo[] CreateFrameBuffer(int width, int height)
        {
            CharInfo[] buffer = new CharInfo[width * height];
            short attributes = BuildAttributes(ConsoleColor.White, ConsoleColor.Black);

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i].UnicodeChar = ' ';
                buffer[i].Attributes = attributes;
            }

            return buffer;
        }

        /// <summary>
        /// Записывает символ в буфер кадра.
        /// </summary>
        private static void PutBufferCell(CharInfo[] buffer, int width, int height, int x, int y, char cell, ConsoleColor foreground, ConsoleColor background = ConsoleColor.Black)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
            {
                return;
            }

            int index = y * width + x;
            buffer[index].UnicodeChar = cell;
            buffer[index].Attributes = BuildAttributes(foreground, background);
        }

        /// <summary>
        /// Записывает текст в буфер кадра.
        /// </summary>
        private static void PutBufferText(CharInfo[] buffer, int width, int height, int x, int y, string text, ConsoleColor foreground, ConsoleColor background = ConsoleColor.Black)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            for (int i = 0; i < text.Length && x + i < width; i++)
            {
                PutBufferCell(buffer, width, height, x + i, y, text[i], foreground, background);
            }
        }

        /// <summary>
        /// Записывает клетку карты шириной в два символа.
        /// </summary>
        private static void PutMapCell(CharInfo[] buffer, int width, int height, int cellX, int cellY, char cell, ConsoleColor foreground, ConsoleColor background = ConsoleColor.Black)
        {
            int x = cellX * 2;
            PutBufferCell(buffer, width, height, x, cellY, cell, foreground, background);
            PutBufferCell(buffer, width, height, x + 1, cellY, ' ', foreground, background);
        }

        /// <summary>
        /// Показывает буфер кадра в консоли одним выводом.
        /// </summary>
        private static void PresentBuffer(CharInfo[] buffer, int width, int height)
        {
            IntPtr handle = GetStdHandle(StdOutputHandle);
            SmallRect region = new SmallRect
            {
                Left = 0,
                Top = 0,
                Right = (short)(width - 1),
                Bottom = (short)(height - 1)
            };

            WriteConsoleOutput(handle, buffer, new Coord((short)width, (short)height), new Coord(0, 0), ref region);
        }

        /// <summary>
        /// Создает атрибуты цвета для буфера консоли.
        /// </summary>
        private static short BuildAttributes(ConsoleColor foreground, ConsoleColor background)
        {
            return (short)(((int)background << 4) | (int)foreground);
        }

        /// <summary>
        /// Возвращает цвет символа карты.
        /// </summary>
        /// <param name="cell">Символ клетки.</param>
        /// <returns>Цвет символа.</returns>
        private static ConsoleColor GetCellColor(char cell)
        {
            switch (cell)
            {
                case '0':
                    return ConsoleColor.Blue;
                case '&':
                    return ConsoleColor.Green;
                case 'H':
                    return ConsoleColor.Red;
                case '+':
                    return ConsoleColor.Yellow;
                case '%':
                    return ConsoleColor.Gray;
                case '@':
                    return ConsoleColor.Cyan;
                case '^':
                    return ConsoleColor.DarkGray;
                case '~':
                    return ConsoleColor.Blue;
                case '#':
                    return ConsoleColor.DarkGreen;
                case 'O':
                    return ConsoleColor.Magenta;
                case 'F':
                    return ConsoleColor.Yellow;
                case 'o':
                    return ConsoleColor.DarkGray;
                case '?':
                    return ConsoleColor.DarkYellow;
                case 'K':
                    return ConsoleColor.DarkYellow;
                case 'W':
                    return ConsoleColor.DarkGreen;
                case '*':
                    return ConsoleColor.Cyan;
                default:
                    return ConsoleColor.White;
            }
        }

        /// <summary>
        /// Проверяет, является ли клетка зоной всплытия в Титанике.
        /// </summary>
        private static bool IsTitanicSurfaceCell(int x, int y, char cell)
        {
            if (x < 0 || x >= 25 || y < 0 || y >= 25)
            {
                return false;
            }

            if (x != 0 && x != 24 && y != 0 && y != 24)
            {
                return false;
            }

            return cell != '#';
        }

        /// <summary>
        /// Выводит одну цветную клетку.
        /// </summary>
        /// <param name="cell">Символ клетки.</param>
        /// <param name="color">Цвет символа.</param>
        private static void WriteCell(char cell, ConsoleColor color, ConsoleColor? backgroundColor = null)
        {
            Console.ForegroundColor = color;
            if (backgroundColor.HasValue)
            {
                Console.BackgroundColor = backgroundColor.Value;
            }

            Console.Write(cell);
            Console.Write(' ');
            Console.ResetColor();
        }

        #endregion

        #region Генерация мира

        /// <summary>
        /// Создает полную карту мира
        /// </summary>
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

                for (int i = 0; i < 1600; i++) GenerateRiver(fullMap);
                for (int i = 0; i < 2000; i++) GenerateForest(fullMap);
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
                            _groundTypes[currentX, currentY] = '~';
                        }
                    }
                    else break;

                    if (_random.Next(100) < 30) direction = _random.Next(4);

                    switch (direction)
                    {
                        case 0: currentX--; break;
                        case 1: currentY++; break;
                        case 2: currentX++; break;
                        case 3: currentY--; break;
                    }

                    if (currentX < 5 || currentX >= MapHeight - 5 || currentY < 5 || currentY >= MapWidth - 5)
                    {
                        direction = (direction + 2) % 4;
                        currentX = Math.Max(5, Math.Min(currentX, MapHeight - 6));
                        currentY = Math.Max(5, Math.Min(currentY, MapWidth - 6));
                    }
                }
            }
            catch (GameException ex) { Console.WriteLine(ex.GetShortMessage()); }
        }

        /// <summary>
        /// Генерирует лесной массив на карте
        /// </summary>
        private static void GenerateForest(char[,] fullMap)
        {
            try
            {
                int centerX = _random.Next(10, MapHeight - 10);
                int centerY = _random.Next(10, MapWidth - 10);
                int radius = _random.Next(5, 16);
                int density = _random.Next(40, 81);

                for (int x = centerX - radius; x <= centerX + radius; x++)
                    for (int y = centerY - radius; y <= centerY + radius; y++)
                    {
                        if (x < 0 || x >= MapHeight || y < 0 || y >= MapWidth) continue;
                        double distance = Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));
                        if (distance <= radius)
                        {
                            double probability = density / 100.0 * (1 - (distance / radius) * 0.5);
                            if (_random.NextDouble() < probability && fullMap[x, y] == '.')
                            {
                                fullMap[x, y] = '#';
                                _groundTypes[x, y] = '#';
                            }
                        }
                    }
            }
            catch (GameException ex) { Console.WriteLine(ex.GetShortMessage()); }
        }

        /// <summary>
        /// Создает гору на карте
        /// </summary>
        private static void CreateMountain(char[,] fullMap, int centerX, int centerY)
        {
            try
            {
                if (centerX < 0 || centerX >= fullMap.GetLength(0) || centerY < 0 || centerY >= fullMap.GetLength(1))
                    throw new GameException("Координаты горы вне границ карты", "M005", "Map", ErrorSeverity.Medium);

                fullMap[centerX, centerY] = '^';
                _groundTypes[centerX, centerY] = '^';
                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                        if (dx != 0 || dy != 0)
                        {
                            int x = centerX + dx, y = centerY + dy;
                            if (x >= 0 && x < fullMap.GetLength(0) && y >= 0 && y < fullMap.GetLength(1))
                            {
                                fullMap[x, y] = '^';
                                _groundTypes[x, y] = '^';
                            }
                        }

                int[] probabilities = { 85, 70, 55, 40 };
                for (int circle = 2; circle <= 5; circle++)
                    for (int dx = -circle; dx <= circle; dx++)
                        for (int dy = -circle; dy <= circle; dy++)
                            if (Math.Abs(dx) == circle || Math.Abs(dy) == circle)
                            {
                                int x = centerX + dx, y = centerY + dy;
                                if (x >= 0 && x < fullMap.GetLength(0) && y >= 0 && y < fullMap.GetLength(1))
                                    if (_random.Next(100) < probabilities[circle - 2])
                                    {
                                        fullMap[x, y] = '^';
                                        _groundTypes[x, y] = '^';
                                    }
                            }

                if (_random.Next(100) < 30)
                    for (int dx = -6; dx <= 6; dx++)
                        for (int dy = -6; dy <= 6; dy++)
                            if (Math.Abs(dx) == 6 || Math.Abs(dy) == 6)
                            {
                                int x = centerX + dx, y = centerY + dy;
                                if (x >= 0 && x < fullMap.GetLength(0) && y >= 0 && y < fullMap.GetLength(1))
                                    if (_random.Next(100) < 25)
                                    {
                                        fullMap[x, y] = '^';
                                        _groundTypes[x, y] = '^';
                                    }
                            }
            }
            catch (GameException ex) { Console.WriteLine(ex.GetShortMessage()); }
        }

        /// <summary>
        /// Генерирует порталы в уникальные локации
        /// </summary>
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
            catch (GameException ex) { Console.WriteLine(ex.GetShortMessage()); }
        }

        /// <summary>
        /// Проверяет наличие целевого символа в соседних клетках
        /// </summary>
        private static bool HasNearby(char[,] fullMap, int x, int y, char target)
        {
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                    if (dx != 0 || dy != 0)
                    {
                        int nx = x + dx, ny = y + dy;
                        if (nx >= 0 && nx < MapHeight && ny >= 0 && ny < MapWidth)
                            if (fullMap[nx, ny] == target)
                                return true;
                    }
            return false;
        }

        /// <summary>
        /// Генерирует объекты на карте (враги, сердца, стены)
        /// </summary>
        private static void GenerateObjects(char[,] fullMap)
        {
            try
            {
                for (int i = 0; i < MapHeight; i++)
                    for (int j = 0; j < MapWidth; j++)
                        if (_random.Next(100) < 3 && fullMap[i, j] == '.')
                            fullMap[i, j] = '&';

                for (int i = 0; i < MapHeight; i++)
                    for (int j = 0; j < MapWidth; j++)
                        if (_random.Next(100) < 3 && fullMap[i, j] == '.')
                            fullMap[i, j] = 'H';

                for (int i = 0; i < MapHeight; i++)
                    for (int j = 0; j < MapWidth; j++)
                    {
                        int count = _random.Next(100);
                        if (count >= 10 && count < 13 && fullMap[i, j] == '.')
                            fullMap[i, j] = '%';
                    }
            }
            catch (Exception ex) { Console.WriteLine($"Ошибка генерации объектов: {ex.Message}"); }
        }

        /// <summary>
        /// Проверяет наличие врагов в видимой области
        /// </summary>
        public static bool HasEnemiesInView(char[,] fullMap, int playerX, int playerY)
        {
            try
            {
                if (fullMap == null) throw new GameException("Карта не инициализирована", "M006", "Map", ErrorSeverity.Critical);
                int startX = playerX - ViewWidth / 2;
                int startY = playerY - ViewHeight / 2;
                startX = Math.Max(0, Math.Min(startX, MapHeight - ViewHeight));
                startY = Math.Max(0, Math.Min(startY, MapWidth - ViewWidth));
                for (int i = 0; i < ViewHeight; i++)
                    for (int j = 0; j < ViewWidth; j++)
                        if (fullMap[startX + i, startY + j] == '&')
                            return true;
                return false;
            }
            catch (GameException ex) { Console.WriteLine(ex.GetShortMessage()); return false; }
        }

        /// <summary>
        /// Проверяет наличие портала на карте
        /// </summary>
        public static bool IsPortalOnMap(char[,] fullMap)
        {
            for (int i = 0; i < fullMap.GetLength(0); i++)
                for (int j = 0; j < fullMap.GetLength(1); j++)
                    if (fullMap[i, j] == '0')
                        return true;
            return false;
        }

        /// <summary>
        /// Создает портал рядом с игроком
        /// </summary>
        public static void CheckAndSpawnPortal(char[,] fullMap, ref int playerX, ref int playerY)
        {
            try
            {
                if (!HasEnemiesInView(fullMap, playerX, playerY) && !IsPortalOnMap(fullMap))
                {
                    for (int dx = -1; dx <= 1; dx++)
                        for (int dy = -1; dy <= 1; dy++)
                            if (dx != 0 || dy != 0)
                            {
                                int portalX = playerX + dx, portalY = playerY + dy;
                                if (portalX >= 5 && portalX < fullMap.GetLength(0) - 5 &&
                                    portalY >= 5 && portalY < fullMap.GetLength(1) - 5 &&
                                    fullMap[portalX, portalY] == '.')
                                {
                                    fullMap[portalX, portalY] = '0';
                                    _groundTypes[portalX, portalY] = '.';
                                    SetStatusMessage("Рядом появился портал!", ConsoleColor.Yellow);
                                    Console.SetCursorPosition(0, 30);
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("РЯДОМ ПОЯВИЛСЯ ПОРТАЛ!");
                                    Console.ResetColor();
                                    System.Threading.Thread.Sleep(2000);
                                    Console.SetCursorPosition(0, 30);
                                    Console.WriteLine(new string(' ', 60));
                                    return;
                                }
                            }
                }
            }
            catch (GameException ex) { Console.WriteLine(ex.GetShortMessage()); }
        }

        #endregion

        #region Пещера-лабиринт

        /// <summary>
        /// Генерирует случайный лабиринт для пещеры
        /// </summary>
        public static char[,] GenerateRandomLabyrinth()
        {
            char[,] maze = new char[CaveSize, CaveSize];
            bool[,] visited = new bool[CaveLogicalSize, CaveLogicalSize];
            Stack<(int X, int Y)> stack = new Stack<(int X, int Y)>();
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            for (int i = 0; i < CaveSize; i++)
                for (int j = 0; j < CaveSize; j++)
                    maze[i, j] = '#';

            visited[0, 0] = true;
            stack.Push((0, 0));
            CarveCaveRoom(maze, 0, 0);

            while (stack.Count > 0)
            {
                var current = stack.Peek();
                List<(int X, int Y, int Direction)> neighbors = new List<(int X, int Y, int Direction)>();

                for (int d = 0; d < 4; d++)
                {
                    int nextX = current.X + dx[d];
                    int nextY = current.Y + dy[d];
                    if (nextX >= 0 && nextX < CaveLogicalSize &&
                        nextY >= 0 && nextY < CaveLogicalSize &&
                        !visited[nextX, nextY])
                    {
                        neighbors.Add((nextX, nextY, d));
                    }
                }

                if (neighbors.Count > 0)
                {
                    var next = neighbors[_random.Next(neighbors.Count)];
                    CarveCaveRoom(maze, next.X, next.Y);
                    CarveCaveConnection(maze, current.X, current.Y, next.Direction);
                    visited[next.X, next.Y] = true;
                    stack.Push((next.X, next.Y));
                }
                else
                {
                    stack.Pop();
                }
            }

            PlaceCavePuzzleObjects(maze);
            maze[CaveChestX, CaveChestY] = '?';
            maze[CaveExitX, CaveExitY] = 'O';

            return maze;
        }

        /// <summary>
        /// Вырезает комнату в лабиринте
        /// </summary>
        private static void CarveCaveRoom(char[,] maze, int logicalX, int logicalY)
        {
            int startX = 1 + logicalX * CaveStep;
            int startY = 1 + logicalY * CaveStep;

            for (int x = startX; x < startX + CavePassageWidth; x++)
                for (int y = startY; y < startY + CavePassageWidth; y++)
                    maze[x, y] = '.';
        }

        /// <summary>
        /// Создает проход между комнатами
        /// </summary>
        private static void CarveCaveConnection(char[,] maze, int logicalX, int logicalY, int direction)
        {
            int roomX = 1 + logicalX * CaveStep;
            int roomY = 1 + logicalY * CaveStep;

            switch (direction)
            {
                case 0:
                    for (int y = roomY; y < roomY + CavePassageWidth; y++)
                        maze[roomX - 1, y] = '.';
                    break;
                case 1:
                    for (int y = roomY; y < roomY + CavePassageWidth; y++)
                        maze[roomX + CavePassageWidth, y] = '.';
                    break;
                case 2:
                    for (int x = roomX; x < roomX + CavePassageWidth; x++)
                        maze[x, roomY - 1] = '.';
                    break;
                case 3:
                    for (int x = roomX; x < roomX + CavePassageWidth; x++)
                        maze[x, roomY + CavePassageWidth] = '.';
                    break;
            }
        }

        /// <summary>
        /// Размещает объекты загадки в пещере
        /// </summary>
        private static void PlaceCavePuzzleObjects(char[,] maze)
        {
            _caveTargets.Clear();

            List<(int X, int Y, int StoneX, int StoneY)> safeCandidates = new List<(int X, int Y, int StoneX, int StoneY)>();
            List<(int X, int Y, int StoneX, int StoneY)> wallCandidates = new List<(int X, int Y, int StoneX, int StoneY)>();
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            for (int x = 1; x < CaveSize - 1; x++)
            {
                for (int y = 1; y < CaveSize - 1; y++)
                {
                    if (maze[x, y] != '.') continue;
                    if ((x >= 9 && x <= 15) && (y >= 9 && y <= 15)) continue;
                    if ((x == CaveStartX && y == CaveStartY) || (x == CaveChestX && y == CaveChestY) || (x == CaveExitX && y == CaveExitY)) continue;

                    for (int d = 0; d < 4; d++)
                    {
                        int playerX = x - dx[d];
                        int playerY = y - dy[d];
                        int stoneX = x + dx[d] * 5;
                        int stoneY = y + dy[d] * 5;

                        if (stoneX <= 0 || stoneX >= CaveSize - 1 || stoneY <= 0 || stoneY >= CaveSize - 1) continue;
                        if (playerX <= 0 || playerX >= CaveSize - 1 || playerY <= 0 || playerY >= CaveSize - 1) continue;

                        bool clearPath = maze[playerX, playerY] == '.';
                        for (int step = 1; step <= 5 && clearPath; step++)
                        {
                            int pathX = x + dx[d] * step;
                            int pathY = y + dy[d] * step;
                            if (maze[pathX, pathY] != '.')
                                clearPath = false;
                        }

                        if (clearPath)
                        {
                            bool nearWall =
                                maze[stoneX - 1, stoneY] == '#' ||
                                maze[stoneX + 1, stoneY] == '#' ||
                                maze[stoneX, stoneY - 1] == '#' ||
                                maze[stoneX, stoneY + 1] == '#';

                            if (nearWall)
                                wallCandidates.Add((x, y, stoneX, stoneY));
                            else
                                safeCandidates.Add((x, y, stoneX, stoneY));
                        }
                    }
                }
            }

            for (int i = 0; i < safeCandidates.Count; i++)
            {
                int randomIndex = _random.Next(i, safeCandidates.Count);
                var temp = safeCandidates[i];
                safeCandidates[i] = safeCandidates[randomIndex];
                safeCandidates[randomIndex] = temp;
            }

            for (int i = 0; i < wallCandidates.Count; i++)
            {
                int randomIndex = _random.Next(i, wallCandidates.Count);
                var temp = wallCandidates[i];
                wallCandidates[i] = wallCandidates[randomIndex];
                wallCandidates[randomIndex] = temp;
            }

            int wallStoneQuota = wallCandidates.Count > 0 && _random.Next(100) < 35 ? 1 : 0;

            if (wallStoneQuota > 0)
            {
                foreach (var candidate in wallCandidates)
                {
                    bool overlaps = false;
                    foreach (var target in _caveTargets)
                    {
                        if (Math.Abs(target.X - candidate.X) + Math.Abs(target.Y - candidate.Y) < 6)
                        {
                            overlaps = true;
                            break;
                        }
                    }

                    if (overlaps) continue;
                    if (maze[candidate.X, candidate.Y] != '.' || maze[candidate.StoneX, candidate.StoneY] != '.') continue;

                    maze[candidate.X, candidate.Y] = 'O';
                    maze[candidate.StoneX, candidate.StoneY] = 'o';
                    _caveTargets.Add((candidate.X, candidate.Y));
                    wallStoneQuota--;

                    if (wallStoneQuota == 0 || _caveTargets.Count == 4) break;
                }
            }

            foreach (var candidate in safeCandidates)
            {
                bool overlaps = false;
                foreach (var target in _caveTargets)
                {
                    if (Math.Abs(target.X - candidate.X) + Math.Abs(target.Y - candidate.Y) < 6)
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (overlaps) continue;
                if (maze[candidate.X, candidate.Y] != '.' || maze[candidate.StoneX, candidate.StoneY] != '.') continue;

                maze[candidate.X, candidate.Y] = 'O';
                maze[candidate.StoneX, candidate.StoneY] = 'o';
                _caveTargets.Add((candidate.X, candidate.Y));

                if (_caveTargets.Count == 4) break;
            }

            if (_caveTargets.Count < 4)
            {
                foreach (var candidate in safeCandidates)
                {
                    if (maze[candidate.X, candidate.Y] != '.' || maze[candidate.StoneX, candidate.StoneY] != '.') continue;

                    maze[candidate.X, candidate.Y] = 'O';
                    maze[candidate.StoneX, candidate.StoneY] = 'o';
                    _caveTargets.Add((candidate.X, candidate.Y));

                    if (_caveTargets.Count == 4) break;
                }
            }

            if (_caveTargets.Count < 4)
            {
                foreach (var candidate in wallCandidates)
                {
                    if (maze[candidate.X, candidate.Y] != '.' || maze[candidate.StoneX, candidate.StoneY] != '.') continue;

                    maze[candidate.X, candidate.Y] = 'O';
                    maze[candidate.StoneX, candidate.StoneY] = 'o';
                    _caveTargets.Add((candidate.X, candidate.Y));

                    if (_caveTargets.Count == 4) break;
                }
            }

            EnsureFourCaveStones(maze);
        }

        /// <summary>
        /// Гарантирует наличие четырех камней в пещере
        /// </summary>
        private static void EnsureFourCaveStones(char[,] maze)
        {
            List<(int X, int Y)> freeCells = new List<(int X, int Y)>();

            for (int x = 1; x < CaveSize - 1; x++)
                for (int y = 1; y < CaveSize - 1; y++)
                    if (maze[x, y] == '.' && !IsCaveTarget(x, y) &&
                        !(x == CaveStartX && y == CaveStartY) &&
                        !(x == CaveChestX && y == CaveChestY) &&
                        !(x == CaveExitX && y == CaveExitY))
                    {
                        freeCells.Add((x, y));
                    }

            for (int i = CountCaveStones(maze); i < 4 && freeCells.Count > 0; i++)
            {
                int index = _random.Next(freeCells.Count);
                var cell = freeCells[index];
                freeCells.RemoveAt(index);
                maze[cell.X, cell.Y] = 'o';
            }

            for (int i = _caveTargets.Count; i < 4 && freeCells.Count > 0; i++)
            {
                int index = _random.Next(freeCells.Count);
                var cell = freeCells[index];
                freeCells.RemoveAt(index);
                maze[cell.X, cell.Y] = 'O';
                _caveTargets.Add(cell);
            }
        }

        /// <summary>
        /// Подсчитывает количество камней в пещере
        /// </summary>
        private static int CountCaveStones(char[,] maze)
        {
            int count = 0;
            for (int x = 0; x < CaveSize; x++)
                for (int y = 0; y < CaveSize; y++)
                    if (maze[x, y] == 'o')
                        count++;

            return count;
        }

        /// <summary>
        /// Проверяет, является ли клетка целью
        /// </summary>
        private static bool IsCaveTarget(int x, int y)
        {
            foreach (var target in _caveTargets)
                if (target.X == x && target.Y == y)
                    return true;

            return false;
        }

        /// <summary>
        /// Отображает карту пещеры
        /// </summary>
        public static void RenderCaveWithPuzzle(char[,] caveMap, Person hero, bool puzzleSolved, bool chestOpened)
        {
            CharInfo[] buffer = CreateFrameBuffer(FrameWidth, FrameHeight);
            for (int i = 0; i < CaveSize; i++)
            {
                for (int j = 0; j < CaveSize; j++)
                {
                    char cell = i == CaveExitX && j == CaveExitY ? 'O' : caveMap[i, j];
                    ConsoleColor color = GetCellColor(cell);

                    if (cell == '#')
                    {
                        color = ConsoleColor.White;
                    }

                    if (cell == 'O' && caveMap[i, j] == 'O')
                    {
                        color = ConsoleColor.Yellow;
                    }

                    if (i == CaveExitX && j == CaveExitY)
                    {
                        color = ConsoleColor.Magenta;
                    }

                    PutMapCell(buffer, FrameWidth, FrameHeight, j, i, cell, color);
                }
            }

            PutBufferText(buffer, FrameWidth, FrameHeight, 0, CaveSize + 1, $"Здоровье: {hero.HP}/{hero.MaxHP} | Монеты: {hero.Coins} | Камни: 4 | {(puzzleSolved ? "Идите к сундуку (?)" : "Поставьте камни на O")}", ConsoleColor.White);
            PutBufferText(buffer, FrameWidth, FrameHeight, 0, CaveSize + 2, "Стрелки - движение | I - инвентарь | S - сохранить | L - загрузить", ConsoleColor.White);
            PutBufferText(buffer, FrameWidth, FrameHeight, 0, CaveSize + 3, _statusMessage, _statusColor);
            PresentBuffer(buffer, FrameWidth, FrameHeight);
        }

        /// <summary>
        /// Восстанавливает клетку пещеры
        /// </summary>
        private static void RestoreCaveTile(ref char[,] caveMap, int x, int y, bool chestOpened)
        {
            if (IsCaveTarget(x, y))
            {
                caveMap[x, y] = 'O';
                return;
            }

            if (x == CaveChestX && y == CaveChestY && !chestOpened)
            {
                caveMap[x, y] = '?';
                return;
            }

            if (x == CaveExitX && y == CaveExitY)
            {
                caveMap[x, y] = 'O';
                return;
            }

            caveMap[x, y] = '.';
        }

        /// <summary>
        /// Проверяет, решена ли загадка в пещере.
        /// </summary>
        public static bool CheckCavePuzzleSolved(char[,] caveMap)
        {
            if (_caveTargets.Count != 4)
            {
                return false;
            }

            foreach (var target in _caveTargets)
            {
                if (caveMap[target.X, target.Y] != 'o')
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Толкает камень в пещере.
        /// </summary>
        public static void PushStoneInCave(ref char[,] caveMap, ref int playerX, ref int playerY, int dx, int dy, bool chestOpened)
        {
            int stoneX = playerX + dx;
            int stoneY = playerY + dy;
            int newStoneX = stoneX + dx;
            int newStoneY = stoneY + dy;

            if (stoneX < 0 || stoneX >= CaveSize || stoneY < 0 || stoneY >= CaveSize) return;
            if (newStoneX < 0 || newStoneX >= CaveSize || newStoneY < 0 || newStoneY >= CaveSize) return;
            if (caveMap[stoneX, stoneY] != 'o') return;

            char targetCell = caveMap[newStoneX, newStoneY];
            if (targetCell == '#' || targetCell == 'o' || targetCell == '?' || (newStoneX == CaveExitX && newStoneY == CaveExitY))
            {
                return;
            }

            RestoreCaveTile(ref caveMap, playerX, playerY, chestOpened);
            RestoreCaveTile(ref caveMap, stoneX, stoneY, chestOpened);

            caveMap[newStoneX, newStoneY] = 'o';
            playerX = stoneX;
            playerY = stoneY;
            caveMap[playerX, playerY] = '@';
        }

        /// <summary>
        /// Обрабатывает движение игрока в пещере.
        /// </summary>
        public static void MoveInCaveWithPuzzle(ref int playerX, ref int playerY, int dx, int dy,
            ref char[,] caveMap, ref bool inCave, ref bool puzzleSolved, ref bool chestOpened, Person hero)
        {
            int newX = playerX + dx;
            int newY = playerY + dy;

            if (newX < 0 || newX >= CaveSize || newY < 0 || newY >= CaveSize) return;

            if (newX == CaveExitX && newY == CaveExitY && chestOpened)
            {
                inCave = false;
                return;
            }

            if (newX == CaveExitX && newY == CaveExitY && !chestOpened)
            {
                SetStatusMessage("Вы попытались уйти без разгадки.", ConsoleColor.Red);
                BeginFrame();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ВЫ ПОПЫТАЛИСЬ УЙТИ БЕЗ РАЗГАДКИ!");
                Console.WriteLine("ИГРА ОКОНЧЕНА");
                Console.ResetColor();
                Console.ReadKey();
                hero.HP = 0;
                inCave = false;
                return;
            }

            char cell = caveMap[newX, newY];

            if (cell == '?' && !chestOpened)
            {
                if (!puzzleSolved)
                {
                    return;
                }

                hero.HasAquaLung = true;
                SetStatusMessage("Вы нашли акваланг!", ConsoleColor.Yellow);
                chestOpened = true;
                RestoreCaveTile(ref caveMap, playerX, playerY, chestOpened);
                playerX = newX;
                playerY = newY;
                caveMap[playerX, playerY] = '@';
                return;
            }

            if (cell == '#') return;

            if (cell == 'o')
            {
                PushStoneInCave(ref caveMap, ref playerX, ref playerY, dx, dy, chestOpened);
                if (!puzzleSolved && CheckCavePuzzleSolved(caveMap))
                {
                    puzzleSolved = true;
                }
                return;
            }

            RestoreCaveTile(ref caveMap, playerX, playerY, chestOpened);
            playerX = newX;
            playerY = newY;
            caveMap[playerX, playerY] = '@';
        }

        /// <summary>
        /// Создает карту Титаника.
        /// </summary>
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

            int[] leftBounds =
            {
                0, 0, 11, 10, 9, 8, 7, 6, 5, 4, 4, 4, 4, 4, 4, 4, 5, 6, 7, 8, 9, 10, 11, 0, 0
            };
            int[] rightBounds =
            {
                0, 0, 13, 14, 15, 16, 17, 18, 19, 20, 20, 20, 20, 20, 20, 20, 19, 18, 17, 16, 15, 14, 13, 0, 0
            };

            for (int x = 2; x <= 22; x++)
            {
                for (int y = leftBounds[x]; y <= rightBounds[x]; y++)
                {
                    titanicMap[x, y] = '.';
                }
            }

            for (int x = 2; x <= 22; x++)
            {
                for (int y = leftBounds[x]; y <= rightBounds[x]; y++)
                {
                    if (x == 2 || x == 22 || y == leftBounds[x] || y == rightBounds[x])
                    {
                        titanicMap[x, y] = '#';
                    }
                }
            }

            int[] roomRows = { 6, 10, 14, 18 };
            int[] roomCols = { 8, 12, 16 };

            foreach (int row in roomRows)
            {
                for (int y = leftBounds[row] + 1; y <= rightBounds[row] - 1; y++)
                {
                    bool isDoorColumn =
                        y == (leftBounds[row] + rightBounds[row]) / 2 ||
                        y == leftBounds[row] + 2 ||
                        y == rightBounds[row] - 2;

                    if (!isDoorColumn)
                    {
                        titanicMap[row, y] = '#';
                    }
                }
            }

            foreach (int col in roomCols)
            {
                for (int x = 3; x <= 21; x++)
                {
                    if (leftBounds[x] == 0 || rightBounds[x] == 0)
                    {
                        continue;
                    }

                    if (col > leftBounds[x] && col < rightBounds[x])
                    {
                        bool isDoorRow = x == 4 || x == 5 || x == 9 || x == 13 || x == 17 || x == 20;
                        if (!isDoorRow)
                        {
                            titanicMap[x, col] = '#';
                        }
                    }
                }
            }

            titanicMap[12, 20] = '.';
            titanicMap[12, 21] = 'T';
            titanicMap[12, leftBounds[12]] = '.';
            titanicMap[12, rightBounds[12]] = '.';
            titanicMap[12, 8] = '.';
            titanicMap[12, 16] = '.';

            for (int i = 0; i < 12; i++)
            {
                int x = _random.Next(1, 24);
                int y = _random.Next(1, 24);
                if (titanicMap[x, y] == '~') titanicMap[x, y] = 'F';
            }

            for (int i = 0; i < 10; i++)
            {
                int x = _random.Next(1, 24);
                int y = _random.Next(1, 24);
                if (titanicMap[x, y] == '~') titanicMap[x, y] = 'W';
            }

            for (int i = 0; i < 20; i++)
            {
                int x = _random.Next(1, 24);
                int y = _random.Next(1, 24);
                if (titanicMap[x, y] == '~') titanicMap[x, y] = '*';
            }

            return titanicMap;
        }

        /// <summary>
        /// Отрисовывает Титаник.
        /// </summary>
        public static void RenderTitanicMap(char[,] titanicMap, Person hero, int fishCount)
        {
            CharInfo[] buffer = CreateFrameBuffer(FrameWidth, FrameHeight);
            PutBufferText(buffer, FrameWidth, FrameHeight, 0, 0, "ТИТАНИК", ConsoleColor.White);
            PutBufferText(buffer, FrameWidth, FrameHeight, 0, 1, "Красные края - всплытие | ~ вода | # стены | T выход | F рыба | W водоросли | * течение", ConsoleColor.White);
            PutBufferText(buffer, FrameWidth, FrameHeight, 0, 2, hero.HasAquaLung ? "Акваланг: -1 HP за шаг" : "Без акваланга: -5 HP за шаг", ConsoleColor.White);

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    ConsoleColor color = GetCellColor(titanicMap[i, j]);
                    if (IsTitanicSurfaceCell(i, j, titanicMap[i, j]))
                    {
                        color = ConsoleColor.Red;
                    }

                    PutMapCell(buffer, FrameWidth, FrameHeight, j, i + 4, titanicMap[i, j], color);
                }
            }

            PutBufferText(buffer, FrameWidth, FrameHeight, 0, 30, $"Здоровье: {hero.HP}/{hero.MaxHP} | Монет: {hero.Coins} | Рыба: {fishCount}", ConsoleColor.White);
            PutBufferText(buffer, FrameWidth, FrameHeight, 0, 31, "Стрелки - движение | I - инвентарь | S - сохранить | L - загрузить", ConsoleColor.White);
            PutBufferText(buffer, FrameWidth, FrameHeight, 0, 32, _statusMessage, _statusColor);
            PresentBuffer(buffer, FrameWidth, FrameHeight);
        }

        /// <summary>
        /// Обрабатывает движение в Титанике.
        /// </summary>
        public static void MoveInTitanic(ref int x, ref int y, int dx, int dy, char[,] map, ref bool inTitanic, Person hero, ref int fishCount, ref bool hasFish)
        {
            int nx = x + dx;
            int ny = y + dy;

            if (nx < 0 || nx >= 25 || ny < 0 || ny >= 25) return;

            char cell = map[nx, ny];

            if (IsTitanicSurfaceCell(nx, ny, cell))
            {
                inTitanic = false;
                hasFish = fishCount > 0;
                SetStatusMessage("Вы всплыли на поверхность.", ConsoleColor.Cyan);
                return;
            }

            if (cell == 'T')
            {
                inTitanic = false;
                hasFish = fishCount > 0;
                SetStatusMessage("Вы выбрались из Титаника.", ConsoleColor.White);
                return;
            }

            if (cell == '#') return;
            if (cell == 'W') return;

            map[x, y] = _titanicPlayerBaseCell;
            _titanicPlayerBaseCell = cell == 'F' ? '~' : cell;

            if (cell == 'F')
            {
                fishCount++;
                hasFish = true;
                SetStatusMessage("Вы нашли рыбу.", ConsoleColor.Yellow);
            }

            if ((cell == '~' || cell == '*' || cell == 'F') && !IsTitanicSurfaceCell(nx, ny, cell))
            {
                int hpLoss = hero.HasAquaLung ? 1 : 5;
                hero.HP -= hpLoss;
                SetStatusMessage($"Вы потеряли {hpLoss} HP.", ConsoleColor.Red);
                if (hero.HP <= 0)
                {
                    return;
                }
            }

            x = nx;
            y = ny;
            map[x, y] = '@';

            if (cell == '*')
            {
                int pushDistance = _random.Next(1, 11);
                int movedByCurrent = 0;

                for (int step = 0; step < pushDistance; step++)
                {
                    int pushX = x + dx;
                    int pushY = y + dy;

                    if (pushX < 0 || pushX >= 25 || pushY < 0 || pushY >= 25)
                    {
                        break;
                    }

                    char nextCell = map[pushX, pushY];
                    if (nextCell == '#' || nextCell == 'W')
                    {
                        break;
                    }

                    map[x, y] = _titanicPlayerBaseCell;

                    if (nextCell == 'T')
                    {
                        x = pushX;
                        y = pushY;
                        inTitanic = false;
                        hasFish = fishCount > 0;
                        SetStatusMessage($"Течение унесло вас на {movedByCurrent + 1} клеток к выходу.", ConsoleColor.Cyan);
                        return;
                    }

                    _titanicPlayerBaseCell = nextCell == 'F' ? '~' : nextCell;

                    if (nextCell == 'F')
                    {
                        fishCount++;
                        hasFish = true;
                    }

                    x = pushX;
                    y = pushY;
                    map[x, y] = '@';
                    movedByCurrent++;
                }

                SetStatusMessage(
                    movedByCurrent > 0
                        ? $"Течение отбросило вас на {movedByCurrent} клеток."
                        : "Течение упёрлось в преграду.",
                    ConsoleColor.Cyan);
            }
        }

        /// <summary>
        /// Создает карту домика.
        /// </summary>
        public static char[,] CreateHouseMap()
        {
            char[,] houseMap = new char[25, 25];

            for (int i = 0; i < 25; i++)
                for (int j = 0; j < 25; j++)
                    houseMap[i, j] = '.';

            for (int i = 0; i < 25; i++)
            {
                houseMap[0, i] = '#';
                houseMap[24, i] = '#';
                houseMap[i, 0] = '#';
                houseMap[i, 24] = '#';
            }

            for (int y = 1; y < 24; y++)
            {
                if (y != 3 && y != 8 && y != 14 && y != 19) houseMap[6, y] = '#';
                if (y != 4 && y != 10 && y != 15 && y != 21) houseMap[12, y] = '#';
                if (y != 5 && y != 9 && y != 16 && y != 20) houseMap[18, y] = '#';
            }

            for (int x = 1; x < 24; x++)
            {
                if (x != 3 && x != 8 && x != 14 && x != 19) houseMap[x, 6] = '#';
                if (x != 4 && x != 9 && x != 15 && x != 20) houseMap[x, 12] = '#';
                if (x != 5 && x != 10 && x != 16 && x != 21) houseMap[x, 18] = '#';
            }

            houseMap[4, 20] = 'K';
            return houseMap;
        }

        /// <summary>
        /// Отрисовывает домик.
        /// </summary>
        public static void RenderHouseMap(char[,] houseMap, Person hero, bool hasFish, bool hasReward, bool catCatched, bool fishEquipped, bool fishDropped)
        {
            CharInfo[] buffer = CreateFrameBuffer(FrameWidth, FrameHeight);
            PutBufferText(buffer, FrameWidth, FrameHeight, 0, 0, "ДОМИК", ConsoleColor.White);
            PutBufferText(buffer, FrameWidth, FrameHeight, 0, 1, "K - кот | f - брошенная рыба | F - телепорт", ConsoleColor.White);
            PutBufferText(buffer, FrameWidth, FrameHeight, 0, 2, !catCatched
                ? "Возьмите рыбу в руки, бросьте её на Пробел и отойдите."
                : "Кот побежден! Телепорт открыт.", ConsoleColor.White);

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    ConsoleColor color = GetCellColor(houseMap[i, j]);
                    PutMapCell(buffer, FrameWidth, FrameHeight, j, i + 4, houseMap[i, j], color);
                }
            }

            PutBufferText(buffer, FrameWidth, FrameHeight, 0, 30, $"Здоровье: {hero.HP}/{hero.MaxHP} | Монет: {hero.Coins} | Рыба: {(hasFish ? "есть" : "нет")} | В руках: {(fishEquipped ? "да" : "нет")}", ConsoleColor.White);
            PutBufferText(buffer, FrameWidth, FrameHeight, 0, 31, "Стрелки - движение | Пробел - бросить рыбу | I - инвентарь", ConsoleColor.White);
            PutBufferText(buffer, FrameWidth, FrameHeight, 0, 32, _statusMessage, _statusColor);
            PresentBuffer(buffer, FrameWidth, FrameHeight);
        }

        /// <summary>
        /// Проверяет, можно ли пройти по клетке домика.
        /// </summary>
        private static bool IsHouseWalkable(char cell)
        {
            return cell == '.' || cell == 'f';
        }

        /// <summary>
        /// Перемещает кота в случайную доступную клетку.
        /// </summary>
        private static void MoveCatRandom(ref char[,] houseMap, ref int catX, ref int catY, int playerX, int playerY)
        {
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            for (int attempt = 0; attempt < 8; attempt++)
            {
                int dir = _random.Next(4);
                int nx = catX + dx[dir];
                int ny = catY + dy[dir];

                if (nx < 0 || nx >= 25 || ny < 0 || ny >= 25) continue;
                if (nx == playerX && ny == playerY) continue;
                if (!IsHouseWalkable(houseMap[nx, ny])) continue;

                houseMap[catX, catY] = '.';
                catX = nx;
                catY = ny;
                houseMap[catX, catY] = 'K';
                return;
            }
        }

        /// <summary>
        /// Телепортирует кота в случайную клетку домика.
        /// </summary>
        private static void TeleportCatToRandomPlace(ref char[,] houseMap, ref int catX, ref int catY, int playerX, int playerY)
        {
            houseMap[catX, catY] = '.';

            int newCatX;
            int newCatY;
            do
            {
                newCatX = _random.Next(1, 24);
                newCatY = _random.Next(1, 24);
            }
            while (!IsHouseWalkable(houseMap[newCatX, newCatY]) || (newCatX == playerX && newCatY == playerY));

            catX = newCatX;
            catY = newCatY;
            houseMap[catX, catY] = 'K';
        }

        /// <summary>
        /// Обновляет поведение кота в домике.
        /// </summary>
        private static void MoveCat(ref char[,] houseMap, ref int catX, ref int catY, int playerX, int playerY, ref bool fishDropped, ref int droppedFishX, ref int droppedFishY, ref bool catCatched, ref bool hasReward, Person hero)
        {
            if (catX == -1 && catY == -1) return;

            if (fishDropped && droppedFishX >= 0 && droppedFishY >= 0)
            {
                int distanceToFish = Math.Abs(catX - droppedFishX) + Math.Abs(catY - droppedFishY);
                int playerToFish = Math.Abs(playerX - droppedFishX) + Math.Abs(playerY - droppedFishY);

                if (playerToFish <= 1)
                {
                    houseMap[droppedFishX, droppedFishY] = '.';
                    fishDropped = false;
                    droppedFishX = -1;
                    droppedFishY = -1;
                    TeleportCatToRandomPlace(ref houseMap, ref catX, ref catY, playerX, playerY);
                    SetStatusMessage("Вы подошли к рыбе, и кот убежал.", ConsoleColor.Yellow);
                    return;
                }

                if (distanceToFish <= 6)
                {
                    houseMap[catX, catY] = '.';

                    if (catX < droppedFishX && IsHouseWalkable(houseMap[catX + 1, catY])) catX++;
                    else if (catX > droppedFishX && IsHouseWalkable(houseMap[catX - 1, catY])) catX--;
                    else if (catY < droppedFishY && IsHouseWalkable(houseMap[catX, catY + 1])) catY++;
                    else if (catY > droppedFishY && IsHouseWalkable(houseMap[catX, catY - 1])) catY--;

                    if (catX == droppedFishX && catY == droppedFishY)
                    {
                        fishDropped = false;
                        droppedFishX = -1;
                        droppedFishY = -1;
                        catCatched = true;
                        hasReward = true;
                        hero.Coins += 300;
                        hero.MaxHP += 30;
                        hero.HP = Math.Min(hero.MaxHP, hero.HP + 30);
                        houseMap[HouseExitX, HouseExitY] = 'F';
                        SetStatusMessage("Кот отвлекся на рыбу. Телепорт открыт!", ConsoleColor.Green);
                        return;
                    }

                    houseMap[catX, catY] = 'K';
                    return;
                }
            }

            MoveCatRandom(ref houseMap, ref catX, ref catY, playerX, playerY);
        }

        /// <summary>
        /// Обрабатывает движение игрока в домике.
        /// </summary>
        public static void MoveInHouse(ref int housePlayerX, ref int housePlayerY, int dx, int dy,
            ref char[,] houseMap, ref bool inHouse, ref bool hasFish, ref bool hasReward,
            ref int catX, ref int catY, ref bool catCatched, ref int fishCount, Person hero,
            ref bool fishEquipped, ref bool fishDropped, ref int droppedFishX, ref int droppedFishY)
        {
            int newX = housePlayerX + dx;
            int newY = housePlayerY + dy;

            if (newX < 0 || newX >= 25 || newY < 0 || newY >= 25) return;

            char cell = houseMap[newX, newY];

            if (cell == 'F')
            {
                inHouse = false;
                return;
            }

            if (cell == '#') return;

            if (cell == 'K' && !catCatched)
            {
                if (fishEquipped || hasFish)
                {
                    catCatched = true;
                    hasReward = true;
                    houseMap[catX, catY] = '.';
                    catX = -1;
                    catY = -1;
                    hero.Coins += 300;
                    hero.MaxHP += 30;
                    hero.HP = Math.Min(hero.MaxHP, hero.HP + 30);
                    houseMap[HouseExitX, HouseExitY] = 'F';
                    SetStatusMessage("Вы победили кота! Телепорт открыт.", ConsoleColor.Green);
                }
                else
                {
                    TeleportCatToRandomPlace(ref houseMap, ref catX, ref catY, housePlayerX, housePlayerY);
                    SetStatusMessage("Кот убежал в другую комнату.", ConsoleColor.Yellow);
                }

                return;
            }

            houseMap[housePlayerX, housePlayerY] = '.';
            housePlayerX = newX;
            housePlayerY = newY;
            houseMap[housePlayerX, housePlayerY] = '@';

            if (!catCatched)
            {
                MoveCat(ref houseMap, ref catX, ref catY, housePlayerX, housePlayerY, ref fishDropped, ref droppedFishX, ref droppedFishY, ref catCatched, ref hasReward, hero);
            }
        }

        /// <summary>
        /// Бросает рыбу в домике.
        /// </summary>
        public static void ThrowFishInHouse(ref char[,] houseMap, int playerX, int playerY, int dx, int dy,
            ref int fishCount, ref bool hasFish, ref bool fishEquipped, ref bool fishDropped, ref int droppedFishX, ref int droppedFishY)
        {
            if (!fishEquipped || fishCount <= 0 || fishDropped) return;
            if (dx == 0 && dy == 0) dy = 1;

            int targetX = playerX + dx * 3;
            int targetY = playerY + dy * 3;

            if (targetX <= 0 || targetX >= 24 || targetY <= 0 || targetY >= 24) return;
            if (houseMap[targetX, targetY] != '.') return;

            houseMap[targetX, targetY] = 'f';
            fishCount--;
            hasFish = fishCount > 0;
            fishEquipped = false;
            fishDropped = true;
            droppedFishX = targetX;
            droppedFishY = targetY;
            SetStatusMessage("Вы бросили рыбу.", ConsoleColor.Yellow);
        }

        /// <summary>
        /// Показывает инвентарь.
        /// </summary>
        public static bool ShowInventory(Person hero, int fishCount, bool hasArtifact, bool fishEquipped)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      ИНВЕНТАРЬ                          ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
            Console.WriteLine($"║  Имя: {hero.Name}                                      ║");
            Console.WriteLine($"║  Здоровье: {hero.HP}/{hero.MaxHP}                      ║");
            Console.WriteLine($"║  Сила: {hero.Strength}                                 ║");
            Console.WriteLine($"║  Монет: {hero.Coins}                                   ║");
            Console.WriteLine($"║  Рыба: {fishCount}                                     ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
            if (hero.HasAquaLung) Console.WriteLine("║  Акваланг: есть                                        ║");
            if (fishCount > 0) Console.WriteLine($"║  Рыба в руках: {(fishEquipped ? "да" : "нет")}                            ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            if (fishCount > 0) Console.WriteLine("1 - взять или убрать рыбу в руки");
            Console.WriteLine("Enter или Esc - назад");

            while (true)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                if ((key == ConsoleKey.D1 || key == ConsoleKey.NumPad1) && fishCount > 0)
                {
                    return !fishEquipped;
                }

                if (key == ConsoleKey.Enter || key == ConsoleKey.Escape)
                {
                    return fishEquipped;
                }
            }
        }

        /// <summary>
        /// Обрабатывает перемещение по основному миру.
        /// </summary>
        public static void MovePlayer(ref int playerX, ref int playerY, int dx, int dy, char[,] fullMap, Person hero,
            ref bool inCave, ref char[,] caveMap, ref int cavePlayerX, ref int cavePlayerY, ref bool puzzleSolved, ref bool chestOpened,
            ref bool inTitanic, ref char[,] titanicMap, ref int titanicPlayerX, ref int titanicPlayerY,
            ref bool inHouse, ref char[,] houseMap, ref int housePlayerX, ref int housePlayerY,
            ref bool hasFish, ref bool hasReward, ref int catX, ref int catY,
            ref bool catCatched, ref int fishCount)
        {
            int newX = playerX + dx;
            int newY = playerY + dy;

            if (newX < 0 || newX >= fullMap.GetLength(0) || newY < 0 || newY >= fullMap.GetLength(1)) return;

            char cell = fullMap[newX, newY];

            if (cell == '^' || cell == '%')
            {
                return;
            }

            if (cell == 'O')
            {
                UniqueLocationFactory caveFactory = new CaveLocationFactory();
                UniqueLocationData caveLocation = caveFactory.CreateLocation();
                inCave = true;
                puzzleSolved = false;
                chestOpened = false;
                cavePlayerX = caveLocation.PlayerX;
                cavePlayerY = caveLocation.PlayerY;
                caveMap = caveLocation.Map;
                caveMap[cavePlayerX, cavePlayerY] = '@';
                return;
            }

            if (cell == 'T')
            {
                UniqueLocationFactory titanicFactory = new TitanicLocationFactory();
                UniqueLocationData titanicLocation = titanicFactory.CreateLocation();
                titanicPlayerX = titanicLocation.PlayerX;
                titanicPlayerY = titanicLocation.PlayerY;
                titanicMap = titanicLocation.Map;
                _titanicPlayerBaseCell = '.';
                titanicMap[titanicPlayerX, titanicPlayerY] = '@';
                inTitanic = true;
                return;
            }

            if (cell == 'F')
            {
                UniqueLocationFactory houseFactory = new HouseLocationFactory();
                UniqueLocationData houseLocation = houseFactory.CreateLocation();
                houseMap = houseLocation.Map;
                housePlayerX = houseLocation.PlayerX;
                housePlayerY = houseLocation.PlayerY;
                houseMap[housePlayerX, housePlayerY] = '@';
                inHouse = true;
                hasReward = false;
                catCatched = false;
                hasFish = fishCount > 0;

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

                return;
            }

            if (cell == '&')
            {
                Random battle = new Random();
                Person enemy = new Person(LevelWorld * 10);

                while (enemy.HP > 0 && hero.HP > 0)
                {
                    enemy.HP -= battle.Next(10) + hero.Strength;
                    hero.HP -= battle.Next(10) + LevelWorld * 5;
                }

                if (hero.HP <= 0)
                {
                    return;
                }

                hero.Coins += battle.Next(100);
                SetStatusMessage("Вы победили врага и получили монеты.", ConsoleColor.Green);
                fullMap[newX, newY] = _groundTypes[newX, newY];
            }
            else if (cell == 'H')
            {
                hero.MaxHP += 10;
                hero.HP = Math.Min(hero.MaxHP, hero.HP + 10);
                SetStatusMessage("Вы нашли улучшение здоровья.", ConsoleColor.Green);
                fullMap[newX, newY] = _groundTypes[newX, newY];
            }
            else if (cell == '0')
            {
                hero.HP = hero.MaxHP;
                LevelWorld++;
                SetStatusMessage("Вы перешли на следующий уровень мира.", ConsoleColor.Cyan);
                char[,] newMap = CreateFullMap();
                for (int i = 0; i < fullMap.GetLength(0); i++)
                    for (int j = 0; j < fullMap.GetLength(1); j++)
                        fullMap[i, j] = newMap[i, j];

                playerX = fullMap.GetLength(0) / 2;
                playerY = fullMap.GetLength(1) / 2;
                return;
            }
            else if (cell == '+')
            {
                Forge(hero);
                SetStatusMessage("Вы посетили кузницу.", ConsoleColor.Yellow);
                fullMap[newX, newY] = _groundTypes[newX, newY];
            }

            playerX = newX;
            playerY = newY;
            CheckAndSpawnPortal(fullMap, ref playerX, ref playerY);
        }

        /// <summary>
        /// Открывает кузницу.
        /// </summary>
        public static void Forge(Person hero)
        {
            Console.Clear();
            Console.WriteLine("КУЗНИЦА");
            Console.WriteLine("1 - Улучшить силу (+2) за 250 монет");
            Console.WriteLine("Enter - выйти");
            Console.WriteLine($"Монет: {hero.Coins}");

            while (true)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Enter)
                {
                    return;
                }

                if ((key == ConsoleKey.D1 || key == ConsoleKey.NumPad1) && hero.Coins >= 250)
                {
                    hero.Coins -= 250;
                    hero.Strength += 2;
                    Console.WriteLine($"Сила увеличена. Текущее значение: {hero.Strength}");
                }
            }
        }

        #endregion
    }
}
