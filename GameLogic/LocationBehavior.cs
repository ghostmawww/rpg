using System;

namespace ConsoleApp46
{
    /// <summary>
    /// Хранит текущее состояние игры и уникальных локаций.
    /// </summary>
    public sealed class GameSession
    {
        /// <summary>
        /// Основная карта мира.
        /// </summary>
        public char[,] FullMap;

        /// <summary>
        /// Координата игрока по X в основном мире.
        /// </summary>
        public int PlayerX;

        /// <summary>
        /// Координата игрока по Y в основном мире.
        /// </summary>
        public int PlayerY;

        /// <summary>
        /// Находится ли игрок в пещере.
        /// </summary>
        public bool InCave;

        /// <summary>
        /// Карта пещеры.
        /// </summary>
        public char[,] CaveMap;

        /// <summary>
        /// Координата игрока по X в пещере.
        /// </summary>
        public int CavePlayerX;

        /// <summary>
        /// Координата игрока по Y в пещере.
        /// </summary>
        public int CavePlayerY;

        /// <summary>
        /// Решена ли загадка в пещере.
        /// </summary>
        public bool PuzzleSolved;

        /// <summary>
        /// Открыт ли сундук в пещере.
        /// </summary>
        public bool ChestOpened;

        /// <summary>
        /// Находится ли игрок в Титанике.
        /// </summary>
        public bool InTitanic;

        /// <summary>
        /// Карта Титаника.
        /// </summary>
        public char[,] TitanicMap;

        /// <summary>
        /// Координата игрока по X в Титанике.
        /// </summary>
        public int TitanicPlayerX;

        /// <summary>
        /// Координата игрока по Y в Титанике.
        /// </summary>
        public int TitanicPlayerY;

        /// <summary>
        /// Количество найденной рыбы.
        /// </summary>
        public int FishCount;

        /// <summary>
        /// Есть ли рыба у игрока.
        /// </summary>
        public bool HasFish;

        /// <summary>
        /// Находится ли игрок в домике.
        /// </summary>
        public bool InHouse;

        /// <summary>
        /// Карта домика.
        /// </summary>
        public char[,] HouseMap;

        /// <summary>
        /// Координата игрока по X в домике.
        /// </summary>
        public int HousePlayerX;

        /// <summary>
        /// Координата игрока по Y в домике.
        /// </summary>
        public int HousePlayerY;

        /// <summary>
        /// Получена ли награда в домике.
        /// </summary>
        public bool HasReward;

        /// <summary>
        /// Координата кошки по X.
        /// </summary>
        public int CatX;

        /// <summary>
        /// Координата кошки по Y.
        /// </summary>
        public int CatY;

        /// <summary>
        /// Поймана ли кошка.
        /// </summary>
        public bool CatCatched;

        /// <summary>
        /// Взята ли рыба в руки.
        /// </summary>
        public bool FishEquipped;

        /// <summary>
        /// Была ли рыба брошена на карту.
        /// </summary>
        public bool FishDropped;

        /// <summary>
        /// Координата брошенной рыбы по X.
        /// </summary>
        public int DroppedFishX;

        /// <summary>
        /// Координата брошенной рыбы по Y.
        /// </summary>
        public int DroppedFishY;
    }

    /// <summary>
    /// Определяет поведение для текущей локации.
    /// </summary>
    public interface ILocationBehavior
    {
        /// <summary>
        /// Уникальный ключ локации.
        /// </summary>
        string LocationKey { get; }

        /// <summary>
        /// Отображает текущую локацию.
        /// </summary>
        void Render(GameSession session, Person hero, ref string lastLocation);

        /// <summary>
        /// Обрабатывает движение в текущей локации.
        /// </summary>
        void HandleMovement(GameSession session, int dx, int dy, Person hero);
    }

    /// <summary>
    /// Содержит общую логику стратегий локаций.
    /// </summary>
    public abstract class LocationBehaviorBase : ILocationBehavior
    {
        /// <summary>
        /// Уникальный ключ локации.
        /// </summary>
        public abstract string LocationKey { get; }

        /// <summary>
        /// Отображает текущую локацию.
        /// </summary>
        public void Render(GameSession session, Person hero, ref string lastLocation)
        {
            if (lastLocation != LocationKey)
            {
                Console.Clear();
                lastLocation = LocationKey;
            }

            RenderCore(session, hero);
        }

        /// <summary>
        /// Выполняет фактическую отрисовку локации.
        /// </summary>
        protected abstract void RenderCore(GameSession session, Person hero);

        /// <summary>
        /// Обрабатывает движение в текущей локации.
        /// </summary>
        public abstract void HandleMovement(GameSession session, int dx, int dy, Person hero);
    }

    /// <summary>
    /// Поведение основного мира.
    /// </summary>
    public sealed class WorldBehavior : LocationBehaviorBase
    {
        /// <summary>
        /// Экземпляр стратегии основного мира.
        /// </summary>
        public static readonly WorldBehavior Instance = new WorldBehavior();

        private WorldBehavior()
        {
        }

        /// <inheritdoc/>
        public override string LocationKey => "world";

        /// <inheritdoc/>
        protected override void RenderCore(GameSession session, Person hero)
        {
            Map.RenderWorldMap(session.FullMap, session.PlayerX, session.PlayerY, hero);
        }

        /// <inheritdoc/>
        public override void HandleMovement(GameSession session, int dx, int dy, Person hero)
        {
            Map.MovePlayer(ref session.PlayerX, ref session.PlayerY, dx, dy, session.FullMap, hero,
                ref session.InCave, ref session.CaveMap, ref session.CavePlayerX, ref session.CavePlayerY, ref session.PuzzleSolved, ref session.ChestOpened,
                ref session.InTitanic, ref session.TitanicMap, ref session.TitanicPlayerX, ref session.TitanicPlayerY,
                ref session.InHouse, ref session.HouseMap, ref session.HousePlayerX, ref session.HousePlayerY,
                ref session.HasFish, ref session.HasReward, ref session.CatX, ref session.CatY,
                ref session.CatCatched, ref session.FishCount);
        }
    }

    /// <summary>
    /// Поведение пещеры.
    /// </summary>
    public sealed class CaveBehavior : LocationBehaviorBase
    {
        /// <summary>
        /// Экземпляр стратегии пещеры.
        /// </summary>
        public static readonly CaveBehavior Instance = new CaveBehavior();

        private CaveBehavior()
        {
        }

        /// <inheritdoc/>
        public override string LocationKey => "cave";

        /// <inheritdoc/>
        protected override void RenderCore(GameSession session, Person hero)
        {
            Map.RenderCaveWithPuzzle(session.CaveMap, hero, session.PuzzleSolved, session.ChestOpened);
        }

        /// <inheritdoc/>
        public override void HandleMovement(GameSession session, int dx, int dy, Person hero)
        {
            Map.MoveInCaveWithPuzzle(ref session.CavePlayerX, ref session.CavePlayerY, dx, dy,
                ref session.CaveMap, ref session.InCave, ref session.PuzzleSolved, ref session.ChestOpened, hero);
        }
    }

    /// <summary>
    /// Поведение Титаника.
    /// </summary>
    public sealed class TitanicBehavior : LocationBehaviorBase
    {
        /// <summary>
        /// Экземпляр стратегии Титаника.
        /// </summary>
        public static readonly TitanicBehavior Instance = new TitanicBehavior();

        private TitanicBehavior()
        {
        }

        /// <inheritdoc/>
        public override string LocationKey => "titanic";

        /// <inheritdoc/>
        protected override void RenderCore(GameSession session, Person hero)
        {
            Map.RenderTitanicMap(session.TitanicMap, hero, session.FishCount);
        }

        /// <inheritdoc/>
        public override void HandleMovement(GameSession session, int dx, int dy, Person hero)
        {
            Map.MoveInTitanic(ref session.TitanicPlayerX, ref session.TitanicPlayerY, dx, dy, session.TitanicMap,
                ref session.InTitanic, hero, ref session.FishCount, ref session.HasFish);
        }
    }

    /// <summary>
    /// Поведение домика.
    /// </summary>
    public sealed class HouseBehavior : LocationBehaviorBase
    {
        /// <summary>
        /// Экземпляр стратегии домика.
        /// </summary>
        public static readonly HouseBehavior Instance = new HouseBehavior();

        private HouseBehavior()
        {
        }

        /// <inheritdoc/>
        public override string LocationKey => "house";

        /// <inheritdoc/>
        protected override void RenderCore(GameSession session, Person hero)
        {
            Map.RenderHouseMap(session.HouseMap, hero, session.HasFish, session.HasReward, session.CatCatched, session.FishEquipped, session.FishDropped);
        }

        /// <inheritdoc/>
        public override void HandleMovement(GameSession session, int dx, int dy, Person hero)
        {
            Map.MoveInHouse(ref session.HousePlayerX, ref session.HousePlayerY, dx, dy,
                ref session.HouseMap, ref session.InHouse, ref session.HasFish, ref session.HasReward, ref session.CatX, ref session.CatY,
                ref session.CatCatched, ref session.FishCount, hero, ref session.FishEquipped, ref session.FishDropped,
                ref session.DroppedFishX, ref session.DroppedFishY);
        }
    }

    /// <summary>
    /// Выбирает стратегию поведения по текущей локации.
    /// </summary>
    public static class LocationBehaviorResolver
    {
        /// <summary>
        /// Возвращает активную стратегию поведения.
        /// </summary>
        public static ILocationBehavior Resolve(GameSession session)
        {
            if (session.InCave)
            {
                return CaveBehavior.Instance;
            }

            if (session.InTitanic)
            {
                return TitanicBehavior.Instance;
            }

            if (session.InHouse)
            {
                return HouseBehavior.Instance;
            }

            return WorldBehavior.Instance;
        }
    }
}
