using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;


namespace threadsample1
{
    class Program
    {
        static void Main(string[] args)
        {

            //mainrun2();
            mainrun3();
            Console.ReadKey();
        }
        static void mainrun3()
        {
            var sample = new ThreadSample3(10);

            var threadone = new Thread(sample.CountNumbers);
            threadone.Name = "ThreadOne";
            threadone.Start();
            threadone.Join();
            Console.WriteLine("--------------");

            var threadtwo = new Thread(Count);
            threadtwo.Name = "ThreadTwo";
            threadtwo.IsBackground = true;
            threadtwo.Start(8);
            //threadtwo.Join();
            Console.WriteLine("-------------------");

            var threadThread = new Thread(() => CountNumbers(12));
            threadThread.Name = "ThreadThree";
            threadThread.Start();
            threadThread.Join();
            Console.WriteLine("-------------------");

            int i = 10;
            var threadFour = new Thread(() => PrintNumber(i));
            i = 20;
            var threadfive = new Thread(() => PrintNumber(i));
            threadFour.Start();
            threadfive.Start();

        }
        class ThreadSample3
        {
            private readonly int _iterations;
            public ThreadSample3(int iterations)
            {
                _iterations = iterations;
            }
            public void CountNumbers()
            {
                for (int i = 0; i < _iterations; i++)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                    Console.WriteLine("{0} prints {1}",
                            Thread.CurrentThread.Name, i);
                }
            }
        }
        static void Count(object iterations)
        {
            CountNumbers((int)iterations);
        }
        static void CountNumbers(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
                Console.WriteLine("{0} prints {1} ", Thread.CurrentThread.Name, i);
            }
        }
        static void PrintNumber(int number)
        {
            Console.WriteLine(number);
        }
        static void mainrun2()
        {
            var samplef = new ThreadSample1(20);
            var sampleb = new ThreadSample1(30);

            var threadOne = new Thread(samplef.CountNumbers);
            threadOne.Name = "ForegroundThread";
            var threadtwo = new Thread(sampleb.CountNumbers);
            threadtwo.Name = "BackgroundThread";
            //threadtwo.IsBackground = true;

            threadOne.Start();
            threadtwo.Start();
        }
        class ThreadSample1
        {
            private readonly int _iterations;

            public ThreadSample1(int iterations)
            {
                _iterations = iterations;
            }
            public void CountNumbers()
            {
                for (int i = 0; i < _iterations; i++)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(0.1));
                    Console.WriteLine("{0} prints {1}",
                        Thread.CurrentThread.Name, i);
                }
            }
        }
        static void sampelrun1()
        {
            Console.WriteLine("Current thread priority: {0}", Thread.CurrentThread.Priority);

            Console.WriteLine("Running on all cores available");

            RunThreads();

            Thread.Sleep(TimeSpan.FromSeconds(2));
            Console.WriteLine("Running on a single core");

            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(1);

            RunThreads();

            Console.WriteLine("A thread has been abrted");
        }
        static void RunThreads()
        {
            var sample = new ThreadSample();
            var threadOne = new Thread(sample.CountNumbers);
            threadOne.Name = "ThreadOne";
            var threadTwo = new Thread(sample.CountNumbers);
            threadTwo.Name = "ThreadTwo";

            threadOne.Priority = ThreadPriority.Highest;
            threadTwo.Priority = ThreadPriority.Lowest;
            threadOne.Start();
            threadTwo.Start();
            Thread.Sleep(TimeSpan.FromSeconds(2));
            sample.Stop();
        }
        class ThreadSample
        {
            private bool _isStopped = false;
            public void Stop()
            {
                _isStopped = true;
            }
            public void CountNumbers()
            {
                long counter = 0;
                while (!_isStopped)
                {
                    counter++;
                }
                Console.WriteLine("{0} with {1,11} priority " +
                        "has a count = {2,13}",
                         Thread.CurrentThread.Name,
                         Thread.CurrentThread.Priority,
                         counter.ToString("N0"));
            }


        }
        static void DoNothing()
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
        }
        static void PrintNumbersWithStatus()
        {
            Console.WriteLine("Starting....");
            Console.WriteLine(Thread.CurrentThread.ThreadState.ToString());
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
                Console.WriteLine(i);
            }
        }
        static void PrintNumbers()
        {
            Console.WriteLine("Starting...");
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(i);
            }
        }
        static void PrintNumbersWithDelay()
        {
            Console.WriteLine("Starting...wd");
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
                Console.WriteLine(i);
            }
        }
    }

}
