using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HM5
{
    public class Builder
    {
        public string Name { get; set; }
        public void StartBuild(ref int obj, string s)
        {
            Console.WriteLine($"{Name} приступил к строительству {s}");
            Random random = new Random();
            Thread.Sleep(random.Next(1000, 7000));
            obj += 10;
            Console.WriteLine($"{Name} закончил");
        }
    }
    public class Foreman
    {
        public void Report(ref int foundation, int[] walls, ref int roof)
        {
            Random random = new Random();
            DateTime dateTime = DateTime.Now;
            Thread.Sleep(random.Next(7000,13000));
            int totalProgress = foundation + roof;
            int wallsProgress = 0;
            foreach (var item in walls)
            {
                wallsProgress += item;
            }
            totalProgress += wallsProgress;
            totalProgress /= (2 + walls.Length);
            Console.WriteLine($"{dateTime} Прогресс строительства: {totalProgress}% (Фундамент - {foundation}%, Стены - {wallsProgress/4}%, Крыша - {roof}%)");
        }
    }
    public class Construction
    {
        static readonly object _lock = new object();

        private int Foundation;
        private int[] Wall;
        private int Roof;

        public List<Builder> Builders { get; set; }
        public Foreman Foreman { get; set; }
        public Construction(List<Builder> builders, Foreman foreman, int wallCount)
        {
            Builders = builders;
            Foreman = foreman;
            Foundation = 0;
            Wall = new int[wallCount];
            for (int i = 0; i < wallCount; i++)
            {
                Wall[i] = 0; 
            }
            Roof = 0;
        }
        public void Build()
        {
            List<Thread> threads = new List<Thread>();
            while (true)
            {
                threads.Clear();
                if (Foundation < 100)
                {
                    lock (_lock)
                    {
                        for (int i = 0; i < Builders.Count; i++)
                        {
                            int index = i;
                            threads.Add(new Thread(() => { Builders[index].StartBuild(ref Foundation, "фундамента"); }));
                            threads[i].Start();
                        }
                    }
                }
                else if (Wall[Wall.Length - 1] < 100)
                {
                    lock (_lock) { 
                        int currentWallIndex = -1;
                        for (int i = 0; i < Wall.Length; i++)
                        {
                            if (Wall[i] < 100)
                            {
                                currentWallIndex = i;
                                break;
                            }
                        }
                        if (currentWallIndex != -1)
                        {
                            for(int i = 0; i < Builders.Count; i++)
                            {
                                int index = i;
                                threads.Add(new Thread(() => { Builders[index].StartBuild(ref Wall[currentWallIndex], "cтены"); }));
                                threads[i].Start();
                            }
                        }
                    }
                }
                else if (Roof < 100)
                {
                    lock (_lock) { 
                        for (int i = 0; i < Builders.Count; i++)
                        {
                            int index = i;
                            threads.Add(new Thread(() => { Builders[index].StartBuild(ref Roof, "крыши"); }));
                            threads[i].Start();
                        }
                    }
                }
                else
                {
                    Foreman.Report(ref Foundation, Wall, ref Roof);
                    Console.WriteLine("The building was built");
                    return;
                }

                threads.Add(new Thread(() => { Foreman.Report(ref Foundation, Wall, ref Roof); }));
                threads[threads.Count - 1].Start();

                foreach (var item in threads)
                {
                    item.Join();
                }
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            // Создаем строителей
            List<Builder> builders = new List<Builder>();
            for (int i = 1; i <= 5; i++)
            {
                builders.Add(new Builder { Name = $"Builder {i}" });
            }

            Foreman foreman = new Foreman();

            Construction construction = new Construction(builders, foreman, 4);

            construction.Build();

            Console.ReadLine();
        }
    }
}
