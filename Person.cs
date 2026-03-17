using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp46
{
    internal class Person
    {
        public int MaxHP = 100;
        public int HP = 100;
        public int Strenght = 0;

        public int coin = 0;

        public string NamePerson;

        public Person(int HP = 100, string Name = "Враг")
        {
            NamePerson = Name;
            this.HP = HP;
        }


        static public void GetCharacter(Person Hero)
        {
            Console.WriteLine($"Имя героя = {Hero.NamePerson}");
            Console.WriteLine($"Здоровье = {Hero.HP}");
            Console.WriteLine($"MAX Здоровье = {Hero.MaxHP}");
            Console.WriteLine($"Деняк = {Hero.coin}");
            Console.WriteLine($"Уровень мира = {Map.levelWorld}");
        }

    }
}
