using System;
using System.Threading;

class Banker
{
    const int NUMBER_OF_CUSTOMERS = 5;
    const int NUMBER_OF_RESOURCES = 3;

    static int[] available = new int[NUMBER_OF_RESOURCES];
    static int[,] maximum = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES];
    static int[,] allocation = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES];
    static int[,] need = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES];

    static object lockObj = new object();
    static Random rand = new Random();

    static bool IsSafe()
    {
        int[] work = new int[NUMBER_OF_RESOURCES];
        bool[] finish = new bool[NUMBER_OF_CUSTOMERS];

        Array.Copy(available, work, NUMBER_OF_RESOURCES);

        int count = 0;

        while (count < NUMBER_OF_CUSTOMERS)
        {
            bool found = false;

            for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++)
            {
                if (!finish[i])
                {
                    int j;
                    for (j = 0; j < NUMBER_OF_RESOURCES; j++)
                    {
                        if (need[i, j] > work[j])
                            break;
                    }

                    if (j == NUMBER_OF_RESOURCES)
                    {
                        for (int k = 0; k < NUMBER_OF_RESOURCES; k++)
                            work[k] += allocation[i, k];

                        finish[i] = true;
                        found = true;
                        count++;
                    }
                }
            }

            if (!found)
                return false;
        }

        return true;
    }

    static int RequestResources(int customer, int[] request)
    {
        lock (lockObj)
        {
            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            {
                if (request[i] > need[customer, i] || request[i] > available[i])
                    return -1;
            }

            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            {
                available[i] -= request[i];
                allocation[customer, i] += request[i];
                need[customer, i] -= request[i];
            }

            if (!IsSafe())
            {
                for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
                {
                    available[i] += request[i];
                    allocation[customer, i] -= request[i];
                    need[customer, i] += request[i];
                }
                return -1;
            }

            return 0;
        }
    }

    static int ReleaseResources(int customer, int[] release)
    {
        lock (lockObj)
        {
            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            {
                if (release[i] > allocation[customer, i])
                    return -1;

                available[i] += release[i];
                allocation[customer, i] -= release[i];
                need[customer, i] += release[i];
            }

            return 0;
        }
    }

    static void Customer(object obj)
    {
        int id = (int)obj;

        while (true)
        {
            int[] request = new int[NUMBER_OF_RESOURCES];
            int[] release = new int[NUMBER_OF_RESOURCES];

            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
                request[i] = rand.Next(need[id, i] + 1);

            if (RequestResources(id, request) == 0)
                Console.WriteLine($"Cliente {id} solicitou recursos");

            Thread.Sleep(1000);

            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
                release[i] = rand.Next(allocation[id, i] + 1);

            ReleaseResources(id, release);

            Thread.Sleep(1000);
        }
    }

    static void Main(string[] args)
    {
        if (args.Length != NUMBER_OF_RESOURCES)
        {
            Console.WriteLine("Uso: dotnet run 10 5 7");
            return;
        }

        for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            available[i] = int.Parse(args[i]);

        for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++)
        {
            for (int j = 0; j < NUMBER_OF_RESOURCES; j++)
            {
                maximum[i, j] = rand.Next(available[j] + 1);
                allocation[i, j] = 0;
                need[i, j] = maximum[i, j];
            }
        }

        Thread[] threads = new Thread[NUMBER_OF_CUSTOMERS];

        for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++)
        {
            threads[i] = new Thread(Customer);
            threads[i].Start(i);
        }
    }
}
