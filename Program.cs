using System;

namespace ConsoleApp46
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            string heroName = Console.ReadLine();
            Person hero = new Person(100, heroName);

            char[,] map = new char[25, 25];
            Map.Array(map);
            
            while (hero.HP > 0)
            {
                Console.SetCursorPosition(0, 0);
               

                ConsoleKey key = Console.ReadKey().Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        
                        if (Map.GetIvent(hero, map, -1, 0))
                        {
                            Map.UpArray(map);
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        
                        if (Map.GetIvent(hero, map, 1, 0))
                        {
                            Map.DownArray(map);
                        }
                        break;

                    case ConsoleKey.LeftArrow:
                        
                        if (Map.GetIvent(hero, map, 0, -1))
                        {
                            Map.LeftArray(map);
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        
                        if (Map.GetIvent(hero, map, 0, 1))
                        {
                            Map.RightArray(map);
                        }
                        break;

                        
                }
                Person.GetCharacter(hero);
            }
        }
    }
}