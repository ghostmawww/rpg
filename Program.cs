using System;
using System.Collections.Generic;

namespace ConsoleApp46
{
    /// <summary>
    /// Главный класс программы, точка входа в игру
    /// </summary>
    internal class Program
    {
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
                    throw new GameException("Не удалось создать карту", "P001", "Program", ErrorSeverity.Critical);
                }

                int playerX = fullMap.GetLength(0) / 2;
                int playerY = fullMap.GetLength(1) / 2;

                InitializeGameState(out bool inCave, out char[,] caveMap, out int cavePlayerX, out int cavePlayerY, out bool puzzleSolved);
                InitializeTitanicState(out bool inTitanic, out char[,] titanicMap, out int titanicPlayerX, out int titanicPlayerY, out int fishCount);
                InitializeHouseState(out bool inHouse, out char[,] houseMap, out int housePlayerX, out int housePlayerY, out bool hasReward, out int catX, out int catY, out bool catCatched);
                bool hasFish = false;

                ClearAreaAroundPlayer(fullMap, playerX, playerY);
                fullMap[playerX, playerY] = '@';

                while (hero.HP > 0)
                {
                    RenderCurrentLocation(inCave, caveMap, hero, puzzleSolved, inTitanic, titanicMap, fishCount, inHouse, houseMap, hasFish, hasReward, catCatched, fullMap, playerX, playerY);

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
                        Map.ShowInventory(hero, fishCount, false);
                    }
                    else
                    {
                        HandleMovement(key, ref playerX, ref playerY, fullMap, hero,
                            ref inCave, ref caveMap, ref cavePlayerX, ref cavePlayerY,
                            ref inTitanic, ref titanicMap, ref titanicPlayerX, ref titanicPlayerY, ref fishCount, ref hasFish,
                            ref inHouse, ref houseMap, ref housePlayerX, ref housePlayerY,
                            ref puzzleSolved, ref hasReward, ref catX, ref catY, ref catCatched);
                    }
                }

                GameOver(hero);
            }
            catch (GameException ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex}");
                Console.WriteLine("Игра будет завершена.");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Необработанная ошибка: {ex.Message}");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Создает героя с обработкой ошибок
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
        /// <param name="inCave">Флаг нахождения в пещере</param>
        /// <param name="caveMap">Карта пещеры</param>
        /// <param name="cavePlayerX">Координата X игрока в пещере</param>
        /// <param name="cavePlayerY">Координата Y игрока в пещере</param>
        /// <param name="puzzleSolved">Флаг решения загадки</param>
        private static void InitializeGameState(out bool inCave, out char[,] caveMap, out int cavePlayerX, out int cavePlayerY, out bool puzzleSolved)
        {
            inCave = false;
            caveMap = null;
            cavePlayerX = 0;
            cavePlayerY = 0;
            puzzleSolved = false;
        }

        /// <summary>
        /// Инициализирует состояние Титаника
        /// </summary>
        /// <param name="inTitanic">Флаг нахождения в Титанике</param>
        /// <param name="titanicMap">Карта Титаника</param>
        /// <param name="titanicPlayerX">Координата X игрока в Титанике</param>
        /// <param name="titanicPlayerY">Координата Y игрока в Титанике</param>
        /// <param name="fishCount">Количество пойманной рыбы</param>
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
        /// <param name="inHouse">Флаг нахождения в домике</param>
        /// <param name="houseMap">Карта домика</param>
        /// <param name="housePlayerX">Координата X игрока в домике</param>
        /// <param name="housePlayerY">Координата Y игрока в домике</param>
        /// <param name="hasReward">Флаг получения награды</param>
        /// <param name="catX">Координата X кота</param>
        /// <param name="catY">Координата Y кота</param>
        /// <param name="catCatched">Флаг поимки кота</param>
        private static void InitializeHouseState(out bool inHouse, out char[,] houseMap, out int housePlayerX, out int housePlayerY, out bool hasReward, out int catX, out int catY, out bool catCatched)
        {
            inHouse = false;
            houseMap = null;
            housePlayerX = 0;
            housePlayerY = 0;
            hasReward = false;
            catX = 0;
            catY = 0;
            catCatched = false;
        }

        /// <summary>
        /// Очищает область вокруг игрока от объектов
        /// </summary>
        /// <param name="fullMap">Полная карта мира</param>
        /// <param name="playerX">Координата X игрока</param>
        /// <param name="playerY">Координата Y игрока</param>
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

        /// <summary>
        /// Отображает текущую локацию игрока
        /// </summary>
        /// <param name="inCave">Флаг нахождения в пещере</param>
        /// <param name="caveMap">Карта пещеры</param>
        /// <param name="hero">Объект героя</param>
        /// <param name="puzzleSolved">Флаг решения загадки</param>
        /// <param name="inTitanic">Флаг нахождения в Титанике</param>
        /// <param name="titanicMap">Карта Титаника</param>
        /// <param name="fishCount">Количество пойманной рыбы</param>
        /// <param name="inHouse">Флаг нахождения в домике</param>
        /// <param name="houseMap">Карта домика</param>
        /// <param name="hasFish">Флаг наличия рыбы</param>
        /// <param name="hasReward">Флаг получения награды</param>
        /// <param name="catCatched">Флаг поимки кота</param>
        /// <param name="fullMap">Полная карта мира</param>
        /// <param name="playerX">Координата X игрока</param>
        /// <param name="playerY">Координата Y игрока</param>
        private static void RenderCurrentLocation(bool inCave, char[,] caveMap, Person hero, bool puzzleSolved,
            bool inTitanic, char[,] titanicMap, int fishCount, bool inHouse, char[,] houseMap, bool hasFish, bool hasReward, bool catCatched,
            char[,] fullMap, int playerX, int playerY)
        {
            if (inCave)
            {
                Map.RenderCaveWithPuzzle(caveMap, hero, puzzleSolved);
            }
            else if (inTitanic)
            {
                Map.RenderTitanicMap(titanicMap, hero, fishCount);
            }
            else if (inHouse)
            {
                Map.RenderHouseMap(houseMap, hero, hasFish, hasReward, catCatched);
            }
            else
            {
                Map.GetMap(fullMap, playerX, playerY);
                Console.WriteLine();
                Person.GetCharacter(hero);
                Console.WriteLine("I - инвентарь | S - сохранить | L - загрузить");
            }
        }

        /// <summary>
        /// Сохраняет игру
        /// </summary>
        /// <param name="hero">Объект героя</param>
        /// <param name="fullMap">Полная карта мира</param>
        /// <param name="playerX">Координата X игрока</param>
        /// <param name="playerY">Координата Y игрока</param>
        private static void SaveGame(Person hero, char[,] fullMap, int playerX, int playerY)
        {
            Console.Clear();
            Console.WriteLine("Введите имя для сохранения: ");
            string saveName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(saveName))
            {
                SaveData.Save(hero, fullMap, saveName, playerX, playerY);
            }
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }

        /// <summary>
        /// Загружает игру
        /// </summary>
        /// <param name="hero">Объект героя</param>
        /// <param name="fullMap">Полная карта мира</param>
        /// <param name="playerX">Координата X игрока</param>
        /// <param name="playerY">Координата Y игрока</param>
        private static void LoadGame(Person hero, char[,] fullMap, ref int playerX, ref int playerY)
        {
            Console.Clear();
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

        /// <summary>
        /// Обрабатывает движение игрока
        /// </summary>
        /// <param name="key">Нажатая клавиша</param>
        /// <param name="playerX">Координата X игрока</param>
        /// <param name="playerY">Координата Y игрока</param>
        /// <param name="fullMap">Полная карта мира</param>
        /// <param name="hero">Объект героя</param>
        /// <param name="inCave">Флаг нахождения в пещере</param>
        /// <param name="caveMap">Карта пещеры</param>
        /// <param name="cavePlayerX">Координата X в пещере</param>
        /// <param name="cavePlayerY">Координата Y в пещере</param>
        /// <param name="inTitanic">Флаг нахождения в Титанике</param>
        /// <param name="titanicMap">Карта Титаника</param>
        /// <param name="titanicPlayerX">Координата X в Титанике</param>
        /// <param name="titanicPlayerY">Координата Y в Титанике</param>
        /// <param name="fishCount">Количество пойманной рыбы</param>
        /// <param name="hasFish">Флаг наличия рыбы</param>
        /// <param name="inHouse">Флаг нахождения в домике</param>
        /// <param name="houseMap">Карта домика</param>
        /// <param name="housePlayerX">Координата X в домике</param>
        /// <param name="housePlayerY">Координата Y в домике</param>
        /// <param name="puzzleSolved">Флаг решения загадки</param>
        /// <param name="hasReward">Флаг получения награды</param>
        /// <param name="catX">Координата X кота</param>
        /// <param name="catY">Координата Y кота</param>
        /// <param name="catCatched">Флаг поимки кота</param>
        private static void HandleMovement(ConsoleKey key, ref int playerX, ref int playerY, char[,] fullMap, Person hero,
            ref bool inCave, ref char[,] caveMap, ref int cavePlayerX, ref int cavePlayerY,
            ref bool inTitanic, ref char[,] titanicMap, ref int titanicPlayerX, ref int titanicPlayerY, ref int fishCount, ref bool hasFish,
            ref bool inHouse, ref char[,] houseMap, ref int housePlayerX, ref int housePlayerY,
            ref bool puzzleSolved, ref bool hasReward, ref int catX, ref int catY, ref bool catCatched)
        {
            int dx = 0;
            int dy = 0;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    dx = -1;
                    dy = 0;
                    break;
                case ConsoleKey.DownArrow:
                    dx = 1;
                    dy = 0;
                    break;
                case ConsoleKey.LeftArrow:
                    dx = 0;
                    dy = -1;
                    break;
                case ConsoleKey.RightArrow:
                    dx = 0;
                    dy = 1;
                    break;
                default:
                    return;
            }

            if (inCave)
            {
                Map.MoveInCaveWithPuzzle(ref cavePlayerX, ref cavePlayerY, dx, dy,
                    ref caveMap, ref inCave, ref puzzleSolved, hero);
            }
            else if (inTitanic)
            {
                Map.MoveInTitanic(ref titanicPlayerX, ref titanicPlayerY, dx, dy, titanicMap, ref inTitanic, hero, ref fishCount, ref hasFish);
            }
            else if (inHouse)
            {
                Map.MoveInHouse(ref housePlayerX, ref housePlayerY, dx, dy,
                    ref houseMap, ref inHouse, ref hasFish, ref hasReward, ref catX, ref catY, ref catCatched, ref fishCount, hero);
            }
            else
            {
                Map.MovePlayer(ref playerX, ref playerY, dx, dy, fullMap, hero,
                    ref inCave, ref caveMap, ref cavePlayerX, ref cavePlayerY,
                    ref inTitanic, ref titanicMap, ref titanicPlayerX, ref titanicPlayerY,
                    ref inHouse, ref houseMap, ref housePlayerX, ref housePlayerY,
                    ref puzzleSolved, ref hasFish, ref hasReward, ref catX, ref catY,
                    ref catCatched, ref fishCount);
            }
        }

        /// <summary>
        /// Выводит сообщение о завершении игры
        /// </summary>
        /// <param name="hero">Объект героя</param>
        private static void GameOver(Person hero)
        {
            Console.Clear();
            Console.WriteLine("ИГРА ОКОНЧЕНА!");
            Console.WriteLine($"Герой {hero.Name} погиб...");
            Console.WriteLine($"Достигнутый уровень: {Map.LevelWorld}");
            Console.WriteLine($"Собрано монет: {hero.Coins}");
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}