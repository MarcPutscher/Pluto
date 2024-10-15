using Pluto.Logic.Algorithmen;
using Pluto.Models;
using Pluto.Pages;
using System.Collections.ObjectModel;

namespace Pluto.Logic
{
    public class Solver
    {
        public Solver() { }

        public async Task<bool> Process(string algorythmus, CancellationToken token, MainPage mainPage)
        {
            if(algorythmus == "Brute-Force")
            {
                BruteForce bruteForce = new BruteForce();
                return await bruteForce.MainProcess(token, mainPage);
            }
            if (algorythmus == "Brute-Force Advanced")
            {
                BruteForce_Advanced bruteForce_advanced = new BruteForce_Advanced();
                return await bruteForce_advanced.MainProcess(token, mainPage);
            }
            if (algorythmus == "Only Logic")
            {
                Only_Logic only_Logic = new Only_Logic();
                return await only_Logic.MainProcess(token, mainPage);
            }

            return false;
        }

    }
}
