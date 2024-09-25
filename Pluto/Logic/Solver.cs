using Pluto.Models;
using Pluto.Pages;
using System.Collections.ObjectModel;

namespace Pluto.Logic
{
    public class Solver
    {
        public Solver() { }

        public async Task<bool> Process(string algorythmus, CancellationToken token)
        {
            if(algorythmus == "Brute Force")
            {
                BruteForce bruteForce = new BruteForce();
                return await bruteForce.MainProcess(token);
            }

            return false;
        }

    }
}
