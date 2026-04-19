using System;
using System.Threading;

class BankerV2
{
    const int CLIENTES = 5;
    const int RECURSOS = 3;

    static int[] available = new int[RECURSOS];
    static int[,] max = new int[CLIENTES, RECURSOS];
    static int[,] allocation = new int[CLIENTES, RECURSOS];
    static int[,] need = new int[CLIENTES, RECURSOS];

    static object mutex = new object();
    static Random rand = new Random();

    static void Main(string[] args)
    {
        if (args.Length != RECURSOS)
        {
            Console.WriteLine("Uso: dotnet run 10 5 7");
            return;
        }

        for (int i = 0; i < RECURSOS; i++)
            available[i] = int.Parse(args[i]);

        Inicializar();

        Thread[] clientes = new Thread[CLIENTES];

        for (int i = 0; i < CLIENTES; i++)
        {
            int id = i;
            clientes[i] = new Thread(() => ExecutarCliente(id));
            clientes[i].Start();
        }
    }

    static void Inicializar()
    {
        for (int i = 0; i < CLIENTES; i++)
        {
            for (int j = 0; j < RECURSOS; j++)
            {
                max[i, j] = rand.Next(1, available[j] + 1);
                allocation[i, j] = 0;
                need[i, j] = max[i, j];
            }
        }

        Console.WriteLine("Sistema iniciado\n");
    }

    static void ExecutarCliente(int id)
    {
        while (true)
        {
            int[] req = GerarRequisicao(id);

            if (Solicitar(id, req) == 0)
            {
                Thread.Sleep(rand.Next(500, 1500));

                Liberar(id, req);
            }

            Thread.Sleep(rand.Next(500, 1500));
        }
    }

    static int[] GerarRequisicao(int id)
    {
        int[] req = new int[RECURSOS];

        lock (mutex)
        {
            for (int i = 0; i < RECURSOS; i++)
                req[i] = rand.Next(need[id, i] + 1);
        }

        return req;
    }

    static int Solicitar(int id, int[] req)
    {
        lock (mutex)
        {
            Console.WriteLine($"Cliente {id} pede: [{string.Join(", ", req)}]");

            if (!PodeSolicitar(id, req))
            {
                Console.WriteLine("-> Negado (falta recurso)\n");
                return -1;
            }

            Aplicar(id, req);

            if (EstadoSeguro())
            {
                Console.WriteLine("-> Aprovado (seguro)\n");
                return 0;
            }
            else
            {
                Desfazer(id, req);
                Console.WriteLine("-> Negado (inseguro)\n");
                return -1;
            }
        }
    }

    static bool PodeSolicitar(int id, int[] req)
    {
        for (int i = 0; i < RECURSOS; i++)
        {
            if (req[i] > need[id, i] || req[i] > available[i])
                return false;
        }
        return true;
    }

    static void Aplicar(int id, int[] req)
    {
        for (int i = 0; i < RECURSOS; i++)
        {
            available[i] -= req[i];
            allocation[id, i] += req[i];
            need[id, i] -= req[i];
        }
    }

    static void Desfazer(int id, int[] req)
    {
        for (int i = 0; i < RECURSOS; i++)
        {
            available[i] += req[i];
            allocation[id, i] -= req[i];
            need[id, i] += req[i];
        }
    }

    static void Liberar(int id, int[] rel)
    {
        lock (mutex)
        {
            Console.WriteLine($"Cliente {id} libera: [{string.Join(", ", rel)}]\n");

            for (int i = 0; i < RECURSOS; i++)
            {
                available[i] += rel[i];
                allocation[id, i] -= rel[i];
                need[id, i] += rel[i];
            }
        }
    }

    static bool EstadoSeguro()
    {
        int[] work = (int[])available.Clone();
        bool[] finish = new bool[CLIENTES];

        for (int count = 0; count < CLIENTES; count++)
        {
            bool encontrou = false;

            for (int i = 0; i < CLIENTES; i++)
            {
                if (!finish[i] && PodeFinalizar(i, work))
                {
                    for (int j = 0; j < RECURSOS; j++)
                        work[j] += allocation[i, j];

                    finish[i] = true;
                    encontrou = true;
                }
            }

            if (!encontrou) break;
        }

        foreach (bool f in finish)
            if (!f) return false;

        return true;
    }

    static bool PodeFinalizar(int id, int[] work)
    {
        for (int i = 0; i < RECURSOS; i++)
        {
            if (need[id, i] > work[i])
                return false;
        }
        return true;
    }
}
