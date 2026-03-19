using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp46
{
    public class Map
    {
        static public int levelWorld = 1;
        static Random rnd = new Random();

        static public void GetMap(char[,] mas)
        {
            
            Console.Clear();

            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    if (mas[i, j] == '0')
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(mas[i, j] + " ");
                        Console.ResetColor();
                    }
                    else if (mas[i, j] == '&')
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(mas[i, j] + " ");
                        Console.ResetColor();
                    }
                    else if (mas[i, j] == 'H')
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(mas[i, j] + " ");
                        Console.ResetColor();
                    }
                    else if (mas[i, j] == '+')
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(mas[i, j] + " ");
                        Console.ResetColor();
                    }
                    else if (mas[i, j] == '%')
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(mas[i, j] + " ");
                        Console.ResetColor();
                    }
                    else if (mas[i, j] == '@')
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(mas[i, j] + " ");
                        Console.ResetColor();
                    }
                    else if (mas[i, j] == '^')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(mas[i, j] + " ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(mas[i, j] + " ");
                    }
                }
                Console.WriteLine();
            }
        }

        static public void Array(char[,] mas)
        {
            
            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    mas[i, j] = '.';
                }
            }

            
            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    int count = rnd.Next(100);

                    if (count < 5)
                    {
                        mas[i, j] = '&';
                    }
                }
            }

            
            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    int count = rnd.Next(100);

                    if (count < 5 && mas[i, j] == '.')
                    {
                        mas[i, j] = 'H';
                    }
                }
            }

            
            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    int count = rnd.Next(100);

                    if (count >= 10 && count < 15 && mas[i, j] == '.')
                    {
                        mas[i, j] = '%';
                    }
                }
            }

            
            if (levelWorld > 1)
            {
                mas[mas.GetLength(0) / 4, mas.GetLength(1) / 2] = '+';
            }

            int centerX = (mas.GetLength(0) - 1) / 2;
            int centerY = (mas.GetLength(1) - 1) / 2;

            
            for (int i = centerX - 1; i <= centerX + 1; i++)
            {
                for (int j = centerY - 1; j <= centerY + 1; j++)
                {
                    if (i >= 0 && i < mas.GetLength(0) && j >= 0 && j < mas.GetLength(1))
                    {
                        mas[i, j] = '.';
                    }
                }
            }

            
            for (int k = 0; k < 3; k++)
            {
                int enemyX = centerX - 3 + rnd.Next(7);
                int enemyY = centerY - 3 + rnd.Next(7);
                if (enemyX >= 0 && enemyX < mas.GetLength(0) && enemyY >= 0 && enemyY < mas.GetLength(1))
                {
                    if (mas[enemyX, enemyY] == '.')
                        mas[enemyX, enemyY] = '&';
                }
            }

           
            for (int k = 0; k < 5; k++)
            {
                int heartX = centerX - 4 + rnd.Next(9);
                int heartY = centerY - 4 + rnd.Next(9);
                if (heartX >= 0 && heartX < mas.GetLength(0) && heartY >= 0 && heartY < mas.GetLength(1))
                {
                    if (mas[heartX, heartY] == '.')
                        mas[heartX, heartY] = 'H';
                }
            }

            mas[centerX, centerY] = '.';

            
            int mountainCount = rnd.Next(4, 8); 

            for (int m = 0; m < mountainCount; m++)
            {
                int mountainX, mountainY;
                int attempts = 0;
                do
                {
                    mountainX = rnd.Next(4, mas.GetLength(0) - 4);
                    mountainY = rnd.Next(4, mas.GetLength(1) - 4);
                    attempts++;
                    if (attempts > 300) break;
                }
                
                while (Math.Abs(mountainX - centerX) < 3 && Math.Abs(mountainY - centerY) < 3);

                
                bool isLargeMountain = rnd.Next(100) < 40;
                CreateMountain(mas, mountainX, mountainY, isLargeMountain);
            }
        }

        
        public static bool HasEnemies(char[,] mas)
        {
            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    if (mas[i, j] == '&')
                    {
                        return true; 
                    }
                }
            }
            return false; 
        }

        
        public static void CheckAndSpawnPortal(char[,] mas)
        {
           
            if (!HasEnemies(mas) && !IsPortalOnMap(mas))
            {
                int centerX = (mas.GetLength(0) - 1) / 2;
                int centerY = (mas.GetLength(1) - 1) / 2;

                int portalX, portalY;
                int attempts = 0;
                do
                {
                    portalX = rnd.Next(2, mas.GetLength(0) - 2);
                    portalY = rnd.Next(2, mas.GetLength(1) - 2);
                    attempts++;
                    if (attempts > 200) break;
                }
                while (Math.Abs(portalX - centerX) < 3 && Math.Abs(portalY - centerY) < 3 && mas[portalX, portalY] != '.');

                
                mas[portalX, portalY] = '0';
            }
        }

       
        private static bool IsPortalOnMap(char[,] mas)
        {
            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    if (mas[i, j] == '0')
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void CreateMountain(char[,] mas, int centerX, int centerY, bool isLarge = false)
        {
            if (centerX >= 0 && centerX < mas.GetLength(0) && centerY >= 0 && centerY < mas.GetLength(1))
            {
                mas[centerX, centerY] = '^';
            }

            
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int x = centerX + dx;
                    int y = centerY + dy;

                    if (x >= 0 && x < mas.GetLength(0) && y >= 0 && y < mas.GetLength(1))
                    {
                        if (mas[x, y] != '@' && mas[x, y] != '0' && mas[x, y] != '+')
                        {
                            mas[x, y] = '^';
                        }
                    }
                }
            }

            
            int probability2 = isLarge ? 45 : 35; 

            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dy = -2; dy <= 2; dy++)
                {
                    if (Math.Abs(dx) == 2 || Math.Abs(dy) == 2)
                    {
                        int x = centerX + dx;
                        int y = centerY + dy;

                        if (x >= 0 && x < mas.GetLength(0) && y >= 0 && y < mas.GetLength(1))
                        {
                            if (rnd.Next(100) < probability2)
                            {
                                if (mas[x, y] != '@' && mas[x, y] != '0' && mas[x, y] != '+')
                                {
                                    mas[x, y] = '^';
                                }
                            }
                        }
                    }
                }
            }

            
            bool addThirdCircle = isLarge ? (rnd.Next(100) < 80) : (rnd.Next(100) < 40); 

            if (addThirdCircle)
            {
                int probability3 = isLarge ? 35 : 25; 

                for (int dx = -3; dx <= 3; dx++)
                {
                    for (int dy = -3; dy <= 3; dy++)
                    {
                        if (Math.Abs(dx) == 3 || Math.Abs(dy) == 3)
                        {
                            int x = centerX + dx;
                            int y = centerY + dy;

                            if (x >= 0 && x < mas.GetLength(0) && y >= 0 && y < mas.GetLength(1))
                            {
                                if (rnd.Next(100) < probability3)
                                {
                                    if (mas[x, y] != '@' && mas[x, y] != '0' && mas[x, y] != '+')
                                    {
                                        mas[x, y] = '^';
                                    }
                                }
                            }
                        }
                    }
                }
            }

            
            if (isLarge && rnd.Next(100) < 20)
            {
                for (int dx = -4; dx <= 4; dx++)
                {
                    for (int dy = -4; dy <= 4; dy++)
                    {
                        if (Math.Abs(dx) == 4 || Math.Abs(dy) == 4)
                        {
                            int x = centerX + dx;
                            int y = centerY + dy;

                            if (x >= 0 && x < mas.GetLength(0) && y >= 0 && y < mas.GetLength(1))
                            {
                                if (rnd.Next(100) < 20) 
                                {
                                    if (mas[x, y] != '@' && mas[x, y] != '0' && mas[x, y] != '+')
                                    {
                                        mas[x, y] = '^';
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        static public void UpArray(char[,] mas)
        {
            char[] temp = new char[mas.GetLength(0)];

            for (int i = (mas.GetLength(0) - 1); i >= 0; i--)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    if (i == (mas.GetLength(0) - 1))
                    {
                        temp[j] = mas[i, j];
                    }
                    else if (i == 0)
                    {
                        mas[i, j] = temp[j];
                    }
                    if (i != 0)
                    {
                        mas[i, j] = mas[i - 1, j];
                    }
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2)
                    {
                        mas[i, j] = '@';
                    }
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2)
                    {
                        mas[i + 1, j] = '.';
                    }
                }
            }

            
            CheckAndSpawnPortal(mas);
        }

        static public void DownArray(char[,] mas)
        {
            char[] temp = new char[mas.GetLength(0)];

            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    if (i == 0)
                    {
                        temp[j] = mas[i, j];
                    }
                    else if (i == (mas.GetLength(0) - 1))
                    {
                        mas[i, j] = temp[j];
                    }
                    if (i != (mas.GetLength(0) - 1))
                    {
                        mas[i, j] = mas[i + 1, j];
                    }

                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2)
                    {
                        mas[i, j] = '@';
                    }
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2)
                    {
                        mas[i - 1, j] = '.';
                    }
                }
            }


            CheckAndSpawnPortal(mas);
        }

        static public void LeftArray(char[,] mas)
        {
            char[] temp = new char[mas.GetLength(1)];

            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = (mas.GetLength(1) - 1); j >= 0; j--)
                {
                    if (j == (mas.GetLength(1) - 1))
                    {
                        temp[i] = mas[i, j];
                    }
                    else if (j == 0)
                    {
                        mas[i, j] = temp[i];
                    }
                    if (j != 0)
                    {
                        mas[i, j] = mas[i, j - 1];
                    }
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2)
                    {
                        mas[i, j] = '@';
                    }
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2)
                    {
                        mas[i, j + 1] = '.';
                    }
                }
            }

            CheckAndSpawnPortal(mas);
        }

        static public void RightArray(char[,] mas)
        {
            char[] temp = new char[mas.GetLength(1)];

            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    if (j == 0)
                    {
                        temp[i] = mas[i, j];
                    }
                    else if (j == (mas.GetLength(1) - 1))
                    {
                        mas[i, j] = temp[i];
                    }
                    if (j != (mas.GetLength(1) - 1))
                    {
                        mas[i, j] = mas[i, j + 1];
                    }
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2)
                    {
                        mas[i, j] = '@';
                    }
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2)
                    {
                        mas[i, j - 1] = '.';
                    }
                }
            }

            
            CheckAndSpawnPortal(mas);
        }

        static bool Win(char[,] mas)
        {
            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    if (mas[i, j] == '&')
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        static void Batle(Person Hero, char[,] mas)
        {
            Console.Clear();
            Person Enemy = new Person(Map.levelWorld * 10);
            Random rnd = new Random();

            while (Enemy.HP > 0 && Hero.HP > 0)
            {
                int Shot = rnd.Next(10);
                Enemy.HP -= Shot + Hero.Strenght;
                Shot = rnd.Next(10);
                Hero.HP -= Shot + levelWorld * 5;
            }
            if (Enemy.HP < Hero.HP)
            {
                Hero.coin += rnd.Next(100);
            }
            else
            {
                Console.Clear();
                Console.WriteLine($"Поражение");
                Console.ReadKey();
            }
        }

        static void Heart(Person Hero, char[,] mas)
        {
            Console.Clear();
            Hero.MaxHP += 10;
            Hero.HP += Hero.MaxHP / 10;
        }

        static void Portal(Person Hero, char[,] mas)
        {
            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    if (mas[i, j] == 'H')
                    {
                        Hero.coin += 100;
                    }
                }
            }
            Hero.HP = Hero.MaxHP;
            levelWorld++;
            Array(mas);
        }

        static void Forge(Person Hero)
        {
            Console.WriteLine("Выберите действие");
            Console.WriteLine("1. Улучшить силу на 2");
            Console.WriteLine("Для выхода нажмите Enter");
            Console.WriteLine($"Оставшиеся деньги {Hero.coin}");

            ConsoleKey key;
            while ((key = Console.ReadKey().Key) != ConsoleKey.Enter)
            {
                switch (key)
                {
                    case ConsoleKey.NumPad1:
                        if (Hero.coin > 250)
                        {
                            Hero.Strenght += 2;
                            Hero.coin -= 250;
                            Console.WriteLine($"Сила увеличена на 2, Текущая сила = {Hero.Strenght}");
                            Console.WriteLine($"Оставшиеся деньги {Hero.coin}");
                        }
                        else
                        {
                            Console.WriteLine("Недостаточно деняк");
                        }
                        break;
                }
            }
        }

        static public bool GetIvent(Person Hero, char[,] mas, int A = 0, int B = 0)
        {
            char key = mas[((mas.GetLength(0) - 1) / 2) + A, ((mas.GetLength(1) - 1) / 2) + B];

            switch (key)
            {
                case '&':
                    Batle(Hero, mas);
                    break;
                case 'H':
                    Heart(Hero, mas);
                    break;
                case '0':
                    Portal(Hero, mas);
                    break;
                case '+':
                    Forge(Hero);
                    break;
                case '%':
                    return false;
                case '^':
                    return false;
                default:
                    break;
            }
            return true;
        }
    }
}