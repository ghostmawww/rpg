using System;
using System.Collections.Generic;

namespace ConsoleApp46
{   // структурный паттерн
    /// <summary>
    /// Предоставляет упрощенный интерфейс для работы с основными игровыми подсистемами.
    /// </summary>
    public sealed class GameFacade
    {
        private readonly Person _hero;

        /// <summary>
        /// Инициализирует новый экземпляр фасада игры.
        /// </summary>
        /// <param name="hero">Текущий герой.</param>
        public GameFacade(Person hero)
        {
            _hero = hero;
        }

        /// <summary>
        /// Отображает текущую локацию.
        /// </summary>
        /// <param name="session">Состояние игры.</param>
        /// <param name="lastLocation">Ключ последней отображенной локации.</param>
        public void RenderCurrentLocation(GameSession session, ref string lastLocation)
        {
            ILocationBehavior behavior = LocationBehaviorResolver.Resolve(session);
            behavior.Render(session, _hero, ref lastLocation);
        }

        /// <summary>
        /// Обрабатывает движение игрока.
        /// </summary>
        /// <param name="key">Нажатая клавиша.</param>
        /// <param name="session">Состояние игры.</param>
        public void HandleMovement(ConsoleKey key, GameSession session)
        {
            int dx = 0;
            int dy = 0;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    dx = -1;
                    break;
                case ConsoleKey.DownArrow:
                    dx = 1;
                    break;
                case ConsoleKey.LeftArrow:
                    dy = -1;
                    break;
                case ConsoleKey.RightArrow:
                    dy = 1;
                    break;
                default:
                    return;
            }

            ILocationBehavior behavior = LocationBehaviorResolver.Resolve(session);
            behavior.HandleMovement(session, dx, dy, _hero);
        }

        /// <summary>
        /// Открывает инвентарь.
        /// </summary>
        /// <param name="session">Состояние игры.</param>
        public void OpenInventory(GameSession session)
        {
            Console.Clear();
            session.FishEquipped = Map.ShowInventory(_hero, session.FishCount, false, session.FishEquipped);
            Console.Clear();
        }

        /// <summary>
        /// Бросает рыбу в домике.
        /// </summary>
        /// <param name="session">Состояние игры.</param>
        /// <param name="lastMoveDx">Последнее направление по X.</param>
        /// <param name="lastMoveDy">Последнее направление по Y.</param>
        public void ThrowFish(GameSession session, int lastMoveDx, int lastMoveDy)
        {
            Map.ThrowFishInHouse(ref session.HouseMap, session.HousePlayerX, session.HousePlayerY, lastMoveDx, lastMoveDy,
                ref session.FishCount, ref session.HasFish, ref session.FishEquipped, ref session.FishDropped, ref session.DroppedFishX, ref session.DroppedFishY);
        }

        /// <summary>
        /// Сохраняет игру.
        /// </summary>
        /// <param name="session">Состояние игры.</param>
        public void SaveGame(GameSession session)
        {
            Console.Clear();
            Console.WriteLine("Введите название сохранения:");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                SaveData.Save(_hero, session.FullMap, name, session.PlayerX, session.PlayerY);
            }

            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }

        /// <summary>
        /// Загружает игру.
        /// </summary>
        /// <param name="session">Состояние игры.</param>
        public void LoadGame(GameSession session)
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
                    SaveData.Load(saves[choice - 1], _hero, session.FullMap, ref session.PlayerX, ref session.PlayerY);
                    Map.NormalizeWorldMap(session.FullMap, session.PlayerX, session.PlayerY);
                }
            }

            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
