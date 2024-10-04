using Pluto.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pluto.Logic
{
    public class Waiter
    {
        //Wartet eine bestimmet Zeit ab
        public static async Task Waiting() 
        {
            if (MainPage.dificulty_marker == "Leicht")
            {
                await Task.Delay(100);
            }
            if (MainPage.dificulty_marker == "Mittel")
            {
                await Task.Delay(80);
            }
            if (MainPage.dificulty_marker == "Schwer")
            {
                await Task.Delay(50);
            }
            if (MainPage.dificulty_marker == "Experte")
            {
                await Task.Delay(40);
            }
            if (MainPage.dificulty_marker == "Meister")
            {
                await Task.Delay(30);
            }
            if (MainPage.dificulty_marker == "Extrem")
            {
                await Task.Delay(20);
            }
        }
    }
}
