using System;
using System.Collections.Generic;

namespace ConsoleApp46
{
    /// <summary>
    /// Главный класс программы, точка входа в приложение
    /// </summary>
    internal class Program
    {
        #region Точка входа

        /// <summary>
        /// Точка входа в приложение
        /// </summary>
        /// <param name="args">Аргументы командной строки</param>
        private static void Main(string[] args)
        {
            try
            {
                Console.CursorVisible = false;
                Console.SetWindowSize(60, 45);

                Console.WriteLine("Введите имя героя:");
                string heroName = Console.ReadLine();

                Person hero = CreateHero(heroName);

                char[,] fullMap = Map.CreateFullMap();
                if (fullMap == null)
                {
                    throw new GameException("Не удалось создать карту", "P001", "Программа", ErrorSeverity.Critical);
                }

                int playerX = fullMap.GetLength(0) / 2;
                int playerY = fullMap.GetLength(1) / 2;

                InitializeGameState(out bool inCave, out char[,] caveMap, out int cavePlayerX, out int cavePlayerY, out bool puzzleSolved, out bool chestOpened);
                InitializeTitanicState(out bool inTitanic, out char[,] titanicMap, out int titanicPlayerX, out int titanicPlayerY, out int fishCount);
                InitializeHouseState(out bool inHouse, out char[,] houseMap, out int housePlayerX, out int housePlayerY, out bool hasReward, out int catX, out int catY, out bool catCatched,
                    out bool fishEquipped, out bool fishDropped, out int droppedFishX, out int droppedFishY);
                bool hasFish = false;
                string lastLocation = "world";
                int lastMoveDx = 0;
                int lastMoveDy = 1;

                ClearAreaAroundPlayer(fullMap, playerX, playerY);

                while (hero.HP > 0)
                {
                    RenderCurrentLocation(inCave, caveMap, hero, puzzleSolved, chestOpened, inTitanic, titanicMap, fishCount, inHouse, houseMap, hasFish, hasReward, catCatched,
                        fishEquipped, fishDropped, fullMap, playerX, playerY, ref lastLocation);

                    ConsoleKey key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.S)
                    {
                        SaveGame(hero, fullMap, playerX, playerY);
                    }
                    else if (key == ConsoleKey.L)
                    {
                        LoadGame(hero, fullMap, ref playerX, ref playerY);
                    }
                    else if (key == ConsoleKey.I)
                    {
                        Console.Clear();
                        fishEquipped = Map.ShowInventory(hero, fishCount, false, fishEquipped);
                        Console.Clear();
                        lastLocation = string.Empty;
                    }
                    else if (key == ConsoleKey.Spacebar && inHouse)
                    {
                        Map.ThrowFishInHouse(ref houseMap, housePlayerX, housePlayerY, lastMoveDx, lastMoveDy,
                            ref fishCount, ref hasFish, ref fishEquipped, ref fishDropped, ref droppedFishX, ref droppedFishY);
                    }
                    else
                    {
                        if (key == ConsoleKey.UpArrow) { lastMoveDx = -1; lastMoveDy = 0; }
                        else if (key == ConsoleKey.DownArrow) { lastMoveDx = 1; lastMoveDy = 0; }
                        else if (key == ConsoleKey.LeftArrow) { lastMoveDx = 0; lastMoveDy = -1; }
                        else if (key == ConsoleKey.RightArrow) { lastMoveDx = 0; lastMoveDy = 1; }

                        HandleMovement(key, ref playerX, ref playerY, fullMap, hero,
                            ref inCave, ref caveMap, ref cavePlayerX, ref cavePlayerY, ref puzzleSolved, ref chestOpened,
                            ref inTitanic, ref titanicMap, ref titanicPlayerX, ref titanicPlayerY, ref fishCount, ref hasFish,
                            ref inHouse, ref houseMap, ref housePlayerX, ref housePlayerY,
                            ref hasReward, ref catX, ref catY, ref catCatched, ref fishEquipped, ref fishDropped, ref droppedFishX, ref droppedFishY);
                    }
                }

                GameOver(hero);
            }
            catch (GameException ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex}");
                Console.WriteLine("Игра завершена.");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.ReadKey();
            }
        }

        #endregion

        #region Инициализация

        /// <summary>
        /// Создает объект героя
        /// </summary>
        /// <param name="heroName">Имя героя</param>
        /// <returns>Объект героя</returns>
        private static Person CreateHero(string heroName)
        {
            try
            {
                return new Person(100, heroName);
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.ToString());
                return new Person(100, "Герой");
            }
        }

        /// <summary>
        /// Инициализирует состояние пещеры
        /// </summary>
        private static void InitializeGameState(out bool inCave, out char[,] caveMap, out int cavePlayerX, out int cavePlayerY, out bool puzzleSolved, out bool chestOpened)
        {
            inCave = false;
            caveMap = null;
            cavePlayerX = 0;
            cavePlayerY = 0;
            puzzleSolved = false;
            chestOpened = false;
        }

        /// <summary>
        /// Инициализирует состояние Титаника
        /// </summary>
        private static void InitializeTitanicState(out bool inTitanic, out char[,] titanicMap, out int titanicPlayerX, out int titanicPlayerY, out int fishCount)
        {
            inTitanic = false;
            titanicMap = null;
            titanicPlayerX = 0;
            titanicPlayerY = 0;
            fishCount = 0;
        }

        /// <summary>
        /// Инициализирует состояние домика
        /// </summary>
        private static void InitializeHouseState(out bool inHouse, out char[,] houseMap, out int housePlayerX, out int housePlayerY, out bool hasReward, out int catX, out int catY, out bool catCatched,
            out bool fishEquipped, out bool fishDropped, out int droppedFishX, out int droppedFishY)
        {
            inHouse = false;
            houseMap = null;
            housePlayerX = 0;
            housePlayerY = 0;
            hasReward = false;
            catX = 0;
            catY = 0;
            catCatched = false;
            fishEquipped = false;
            fishDropped = false;
            droppedFishX = -1;
            droppedFishY = -1;
        }

        /// <summary>
        /// Очищает область вокруг игрока от объектов
        /// </summary>
        private static void ClearAreaAroundPlayer(char[,] fullMap, int playerX, int playerY)
        {
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
        }

        #endregion

        #region Отображение

        /// <summary>
        /// Отображает текущую локацию игрока
        /// </summary>
        private static void RenderCurrentLocation(bool inCave, char[,] caveMap, Person hero, bool puzzleSolved, bool chestOpened,
            bool inTitanic, char[,] titanicMap, int fishCount, bool inHouse, char[,] houseMap, bool hasFish, bool hasReward, bool catCatched,
            bool fishEquipped, bool fishDropped, char[,] fullMap, int playerX, int playerY, ref string lastLocation)
        {
            string currentLocation = "world";
            if (inCave) currentLocation = "cave";
            else if (inTitanic) currentLocation = "titanic";
            else if (inHouse) currentLocation = "house";

            if (currentLocation != lastLocation)
            {
                Console.Clear();
                lastLocation = currentLocation;
            }

            if (inCave)
            {
                Map.RenderCaveWithPuzzle(caveMap, hero, puzzleSolved, chestOpened);
            }
            else if (inTitanic)
            {
                Map.RenderTitanicMap(titanicMap, hero, fishCount);
            }
            else if (inHouse)
            {
                Map.RenderHouseMap(houseMap, hero, hasFish, hasReward, catCatched, fishEquipped, fishDropped);
            }
            else
            {
                Map.RenderWorldMap(fullMap, playerX, playerY, hero);
            }
        }

        #endregion

        #region Сохранение и загрузка

        /// <summary>
        /// Сохраняет игру
        /// </summary>
        private static void SaveGame(Person hero, char[,] fullMap, int playerX, int playerY)
        {
            Console.Clear();
            Console.WriteLine("Введите название сохранения:");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                SaveData.Save(hero, fullMap, name, playerX, playerY);
            }
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }

        /// <summary>
        /// Загружает игру
        /// </summary>
        private static void LoadGame(Person hero, char[,] fullMap, ref int playerX, ref int playerY)
        {
            Console.Clear();
            List<string> saves = SaveData.GetSaveList();
            if (saves.Count == 0)
            {
                Console.WriteLine("Сохранения не найдены!");
            }
            else
            {
                Console.WriteLine("Сохранения:");
                for (int i = 0; i < saves.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {saves[i]}");
                }
                Console.Write("Выберите номер: ");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= saves.Count)
                {
                    SaveData.Load(saves[choice - 1], hero, fullMap, ref playerX, ref playerY);
                    Map.NormalizeWorldMap(fullMap, playerX, playerY);
                }
            }
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
            Console.Clear();
        }

        #endregion

        #region Обработка движения

        /// <summary>
        /// Обрабатывает движение игрока
        /// </summary>
        private static void HandleMovement(ConsoleKey key, ref int playerX, ref int playerY, char[,] fullMap, Person hero,
            ref bool inCave, ref char[,] caveMap, ref int cavePlayerX, ref int cavePlayerY, ref bool puzzleSolved, ref bool chestOpened,
            ref bool inTitanic, ref char[,] titanicMap, ref int titanicPlayerX, ref int titanicPlayerY, ref int fishCount, ref bool hasFish,
            ref bool inHouse, ref char[,] houseMap, ref int housePlayerX, ref int housePlayerY,
            ref bool hasReward, ref int catX, ref int catY, ref bool catCatched,
            ref bool fishEquipped, ref bool fishDropped, ref int droppedFishX, ref int droppedFishY)
        {
            int dx = 0, dy = 0;
            switch (key)
            {
                case ConsoleKey.UpArrow: dx = -1; break;
                case ConsoleKey.DownArrow: dx = 1; break;
                case ConsoleKey.LeftArrow: dy = -1; break;
                case ConsoleKey.RightArrow: dy = 1; break;
                default: return;
            }

            if (inCave)
            {
                Map.MoveInCaveWithPuzzle(ref cavePlayerX, ref cavePlayerY, dx, dy,
                    ref caveMap, ref inCave, ref puzzleSolved, ref chestOpened, hero);
            }
            else if (inTitanic)
            {
                Map.MoveInTitanic(ref titanicPlayerX, ref titanicPlayerY, dx, dy, titanicMap, ref inTitanic, hero, ref fishCount, ref hasFish);
            }
            else if (inHouse)
            {
                Map.MoveInHouse(ref housePlayerX, ref housePlayerY, dx, dy,
                    ref houseMap, ref inHouse, ref hasFish, ref hasReward, ref catX, ref catY, ref catCatched, ref fishCount, hero,
                    ref fishEquipped, ref fishDropped, ref droppedFishX, ref droppedFishY);
            }
            else
            {
                Map.MovePlayer(ref playerX, ref playerY, dx, dy, fullMap, hero,
                    ref inCave, ref caveMap, ref cavePlayerX, ref cavePlayerY, ref puzzleSolved, ref chestOpened,
                    ref inTitanic, ref titanicMap, ref titanicPlayerX, ref titanicPlayerY,
                    ref inHouse, ref houseMap, ref housePlayerX, ref housePlayerY,
                    ref hasFish, ref hasReward, ref catX, ref catY,
                    ref catCatched, ref fishCount);
            }
        }

        #endregion

        #region Завершение игры

        /// <summary>
        /// Выводит сообщение о завершении игры
        /// </summary>
        private static void GameOver(Person hero)
        {
            Console.Clear();
            Console.WriteLine("ИГРА ОКОНЧЕНА!");
            Console.WriteLine($"Герой {hero.Name} погиб...");
            Console.WriteLine($"Уровень: {Map.LevelWorld}");
            Console.WriteLine($"Монеты: {hero.Coins}");
            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }

        #endregion
    }
}
