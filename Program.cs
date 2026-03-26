using System;
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
                GameFacade gameFacade = new GameFacade(hero);

                GameSession session = new GameSession
                {
                    FullMap = Map.CreateFullMap()
                };

                if (session.FullMap == null)
                {
                    throw new GameException("Не удалось создать карту", "P001", "Программа", ErrorSeverity.Critical);
                }

                session.PlayerX = session.FullMap.GetLength(0) / 2;
                session.PlayerY = session.FullMap.GetLength(1) / 2;

                InitializeGameState(session);
                InitializeTitanicState(session);
                InitializeHouseState(session);
                string lastLocation = "world";
                int lastMoveDx = 0;
                int lastMoveDy = 1;

                ClearAreaAroundPlayer(session.FullMap, session.PlayerX, session.PlayerY);

                while (hero.HP > 0)
                {
                    gameFacade.RenderCurrentLocation(session, ref lastLocation);

                    ConsoleKey key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.S)
                    {
                        gameFacade.SaveGame(session);
                    }
                    else if (key == ConsoleKey.L)
                    {
                        gameFacade.LoadGame(session);
                    }
                    else if (key == ConsoleKey.I)
                    {
                        gameFacade.OpenInventory(session);
                        lastLocation = string.Empty;
                    }
                    else if (key == ConsoleKey.Spacebar && session.InHouse)
                    {
                        gameFacade.ThrowFish(session, lastMoveDx, lastMoveDy);
                    }
                    else
                    {
                        if (key == ConsoleKey.UpArrow) { lastMoveDx = -1; lastMoveDy = 0; }
                        else if (key == ConsoleKey.DownArrow) { lastMoveDx = 1; lastMoveDy = 0; }
                        else if (key == ConsoleKey.LeftArrow) { lastMoveDx = 0; lastMoveDy = -1; }
                        else if (key == ConsoleKey.RightArrow) { lastMoveDx = 0; lastMoveDy = 1; }

                        gameFacade.HandleMovement(key, session);
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
        private static void InitializeGameState(GameSession session)
        {
            session.InCave = false;
            session.CaveMap = null;
            session.CavePlayerX = 0;
            session.CavePlayerY = 0;
            session.PuzzleSolved = false;
            session.ChestOpened = false;
        }

        /// <summary>
        /// Инициализирует состояние Титаника
        /// </summary>
        private static void InitializeTitanicState(GameSession session)
        {
            session.InTitanic = false;
            session.TitanicMap = null;
            session.TitanicPlayerX = 0;
            session.TitanicPlayerY = 0;
            session.FishCount = 0;
            session.HasFish = false;
        }

        /// <summary>
        /// Инициализирует состояние домика
        /// </summary>
        private static void InitializeHouseState(GameSession session)
        {
            session.InHouse = false;
            session.HouseMap = null;
            session.HousePlayerX = 0;
            session.HousePlayerY = 0;
            session.HasReward = false;
            session.CatX = 0;
            session.CatY = 0;
            session.CatCatched = false;
            session.FishEquipped = false;
            session.FishDropped = false;
            session.DroppedFishX = -1;
            session.DroppedFishY = -1;
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
