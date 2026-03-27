using System;

namespace ConsoleApp46
{   //порождающий паттерн
    /// <summary>
    /// Данные созданной уникальной локации.
    /// </summary>
    public class UniqueLocationData
    {
        /// <summary>
        /// Карта локации.
        /// </summary>
        public char[,] Map { get; set; }

        /// <summary>
        /// Стартовая координата игрока по X.
        /// </summary>
        public int PlayerX { get; set; }

        /// <summary>
        /// Стартовая координата игрока по Y.
        /// </summary>
        public int PlayerY { get; set; }
    }

    /// <summary>
    /// Абстрактная фабрика уникальных локаций.
    /// </summary>
    public abstract class UniqueLocationFactory
    {
        /// <summary>
        /// Создает уникальную локацию.
        /// </summary>
        /// <returns>Данные новой локации.</returns>
        public abstract UniqueLocationData CreateLocation();
    }

    /// <summary>
    /// Фабрика создания пещеры.
    /// </summary>
    public sealed class CaveLocationFactory : UniqueLocationFactory
    {
        /// <summary>
        /// Создает локацию пещеры.
        /// </summary>
        /// <returns>Данные пещеры.</returns>
        public override UniqueLocationData CreateLocation()
        {
            return new UniqueLocationData
            {
                Map = Map.GenerateRandomLabyrinth(),
                PlayerX = 1,
                PlayerY = 1
            };
        }
    }

    /// <summary>
    /// Фабрика создания Титаника.
    /// </summary>
    public sealed class TitanicLocationFactory : UniqueLocationFactory
    {
        /// <summary>
        /// Создает локацию Титаника.
        /// </summary>
        /// <returns>Данные Титаника.</returns>
        public override UniqueLocationData CreateLocation()
        {
            return new UniqueLocationData
            {
                Map = Map.CreateTitanicMap(),
                PlayerX = 12,
                PlayerY = 12
            };
        }
    }

    /// <summary>
    /// Фабрика создания домика.
    /// </summary>
    public sealed class HouseLocationFactory : UniqueLocationFactory
    {
        /// <summary>
        /// Создает локацию домика.
        /// </summary>
        /// <returns>Данные домика.</returns>
        public override UniqueLocationData CreateLocation()
        {
            char[,] houseMap = Map.CreateHouseMap();
            int playerX = 2;
            int playerY = 2;

            while (houseMap[playerX, playerY] != '.')
            {
                playerY++;
                if (playerY >= 24)
                {
                    playerY = 1;
                    playerX++;
                }
            }

            return new UniqueLocationData
            {
                Map = houseMap,
                PlayerX = playerX,
                PlayerY = playerY
            };
        }
    }
}
