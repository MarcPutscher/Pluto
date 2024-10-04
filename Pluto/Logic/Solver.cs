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
            if (algorythmus == "Brute-Force Next Gen")
            {
                Brute_Force_Next_Gen brute_Force_Next_Gen = new Brute_Force_Next_Gen();
                return await brute_Force_Next_Gen.MainProcess(token, mainPage);
            }
            if (algorythmus == "Check one direction")
            {
                Check_One_Direction check_One_Direction = new Check_One_Direction(); 
                return await check_One_Direction.MainProcess(token, mainPage);
            }

            return false;
        }

    }
}
