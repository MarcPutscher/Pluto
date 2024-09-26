using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pluto.Models
{
    /// <summary>
    /// 1 = Richtung | 2 = Zahl im Feld | 3 = max Möglichkeiten | 4 = Anzahl an übersprungenen Möglicheiten | 5 = Gridnummer | 6 = Level | 7 = aktuelle Feld | 8 = true wenn es eine Eindeutige Lösung ist und es nur von gelockten Feldern determiniert wurde 
    /// </summary>
    public class Possebilitie
    {
        public Possebilitie() { }

        public string Direction = string.Empty;
        public int Number;
        public int Max;
        public int Count_of_Skip;
        public int Grid;
        public int Level;
        public Field Current_Field = new Field();
        public bool Clearly = false;
    }
}
