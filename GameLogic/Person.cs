using System;

namespace ConsoleApp46
{
    public class Person
    {
        public int MaxHP = 100;
        public int HP = 100;
        public int Strenght = 0;
        public int coin = 0;
        public string NamePerson;

        public Person(int HP = 100, string Name = "Враг")
        {
            try
            {
                if (HP <= 0)
                    throw new GameException("Здоровье не может быть меньше или равно 0", "P001", "Person", ErrorSeverity.Medium);

                if (string.IsNullOrWhiteSpace(Name))
                    throw new GameException("Имя персонажа не может быть пустым", "P002", "Person", ErrorSeverity.Medium);

                NamePerson = Name;
                this.HP = HP;
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Установлены значения по умолчанию");
                NamePerson = "Безымянный";
                this.HP = 100;
            }
        }

        static public void GetCharacter(Person Hero)
        {
            try
            {
                if (Hero == null)
                    throw new GameException("Передан пустой объект персонажа", "P003", "Person", ErrorSeverity.High);

                Console.WriteLine($"Имя героя = {Hero.NamePerson}");
                Console.WriteLine($"Здоровье = {Hero.HP}");
                Console.WriteLine($"MAX Здоровье = {Hero.MaxHP}");
                Console.WriteLine($"Деняк = {Hero.coin}");
                Console.WriteLine($"Уровень мира = {Map.levelWorld}");
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
            }
        }

        public void TakeDamage(int damage)
        {
            try
            {
                if (damage < 0)
                    throw new GameException("Урон не может быть отрицательным", "P004", "Person", ErrorSeverity.Low);

                HP -= damage;
                if (HP < 0) HP = 0;
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
            }
        }

        public void Heal(int amount)
        {
            try
            {
                if (amount < 0)
                    throw new GameException("Лечение не может быть отрицательным", "P005", "Person", ErrorSeverity.Low);

                HP += amount;
                if (HP > MaxHP) HP = MaxHP;
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.GetShortMessage());
            }
        }
    }
}