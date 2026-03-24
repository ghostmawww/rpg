using System;

namespace ConsoleApp46
{
    /// <summary>
    /// Класс, представляющий игрового персонажа
    /// </summary>
    public class Person
    {
        private int _maxHP = 100;
        private int _hp = 100;
        private int _strength = 0;
        private int _coins = 0;
        private string _name;

        /// <summary>
        /// Максимальное здоровье персонажа
        /// </summary>
        public int MaxHP
        {
            get => _maxHP;
            set => _maxHP = value;
        }

        /// <summary>
        /// Текущее здоровье персонажа
        /// </summary>
        public int HP
        {
            get => _hp;
            set => _hp = value;
        }

        /// <summary>
        /// Сила персонажа, влияющая на урон в бою
        /// </summary>
        public int Strength
        {
            get => _strength;
            set => _strength = value;
        }

        /// <summary>
        /// Количество монет у персонажа
        /// </summary>
        public int Coins
        {
            get => _coins;
            set => _coins = value;
        }

        /// <summary>
        /// Имя персонажа
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// Конструктор класса Person
        /// </summary>
        /// <param name="hp">Начальное здоровье</param>
        /// <param name="name">Имя персонажа</param>
        public Person(int hp = 100, string name = "Враг")
        {
            try
            {
                if (hp <= 0)
                {
                    throw new GameException("Здоровье не может быть меньше или равно 0", "P001", "Person", ErrorSeverity.Medium);
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new GameException("Имя персонажа не может быть пустым", "P002", "Person", ErrorSeverity.Medium);
                }

                _name = name;
                _hp = hp;
                _maxHP = hp;
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Установлены значения по умолчанию");
                _name = "Безымянный";
                _hp = 100;
                _maxHP = 100;
            }
        }

        /// <summary>
        /// Отображает характеристики персонажа в консоли
        /// </summary>
        /// <param name="hero">Объект персонажа</param>
        public static void GetCharacter(Person hero)
        {
            try
            {
                if (hero == null)
                {
                    throw new GameException("Передан пустой объект персонажа", "P003", "Person", ErrorSeverity.High);
                }

                Console.WriteLine($"Имя героя = {hero._name}");
                Console.WriteLine($"Здоровье = {hero._hp}");
                Console.WriteLine($"MAX Здоровье = {hero._maxHP}");
                Console.WriteLine($"Монет = {hero._coins}");
                Console.WriteLine($"Уровень мира = {Map.LevelWorld}");
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
            }
        }

        /// <summary>
        /// Наносит урон персонажу
        /// </summary>
        /// <param name="damage">Количество урона</param>
        public void TakeDamage(int damage)
        {
            try
            {
                if (damage < 0)
                {
                    throw new GameException("Урон не может быть отрицательным", "P004", "Person", ErrorSeverity.Low);
                }

                _hp -= damage;
                if (_hp < 0)
                {
                    _hp = 0;
                }
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
            }
        }

        /// <summary>
        /// Лечит персонажа
        /// </summary>
        /// <param name="amount">Количество здоровья для восстановления</param>
        public void Heal(int amount)
        {
            try
            {
                if (amount < 0)
                {
                    throw new GameException("Лечение не может быть отрицательным", "P005", "Person", ErrorSeverity.Low);
                }

                _hp += amount;
                if (_hp > _maxHP)
                {
                    _hp = _maxHP;
                }
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
            }
        }
    }
}