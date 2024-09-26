using Pluto.Logic.Algorithmen;
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
            if(algorythmus == "Brute-Force")
            {
                BruteForce bruteForce = new BruteForce();
                return await bruteForce.MainProcess(token);
            }
            if (algorythmus == "Check one direction")
            {
                Check_One_Direction check_One_Direction = new Check_One_Direction(); 
                return await check_One_Direction.MainProcess(token);
            }

            return false;
        }

    }
}
