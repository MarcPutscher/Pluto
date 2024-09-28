using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pluto.Models
{
    /// <summary>
    /// 1 = Richtung | 2 = Zahl im Feld | 3 = max Möglichkeiten | 4 = Anzahl an übersprungenen Möglicheiten | 5 = Gridnummer | 6 = Level | 7 = aktuelle Feld | 8 = true wenn es eine Eindeutige Lösung ist und es nur von gelockten Feldern determiniert wurde | 9 = disjunkte Felder 
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
        public List<Field> Others = new List<Field>();

        /// <summary>
        /// Fügt die Zahl zu den Felder die für diesen Zug möglich wären als Makierung hinzu
        /// </summary>
        public void Create_Placeholder_Number_In_Fields()
        { 
            if(Others.Count() == 0 && Others.Count() == 1)
                return;


            foreach(Field f in Others)
            {
                if(Direction == "horizontal")
                {
                    if(f.Placeholder_Number_Horizontal?.Contains(Number) == false)
                    f.Placeholder_Number_Horizontal?.Add(Number);
                }
                else
                {
                    if (f.Placeholder_Number_Vertikal?.Contains(Number) == false)
                        f.Placeholder_Number_Vertikal?.Add(Number);
                }
            }
        }

        /// <summary>
        /// Entfernt die Zahl zu den Felder die für diesen Zug möglich wären
        /// </summary>
        public void Remove_Placeholder_Number_In_Fields()
        {
            if (Others.Count() == 0)
                return;

            foreach (Field f in Others)
            {
                if (Direction == "horizontal")
                {
                    f.Placeholder_Number_Horizontal?.Remove(Number);
                }
                else
                {
                    f.Placeholder_Number_Vertikal?.Remove(Number);
                }
            }
        }
    }
}
