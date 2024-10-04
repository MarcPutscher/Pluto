using Microsoft.Maui.Controls.PlatformConfiguration;
using Pluto.Models;
using Pluto.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pluto.Logic.Algorithmen
{
    /// <summary>
    /// Eine Klasse die alle Strategien beinhaltet und diese Strategien werden solange ausgeübt, bis sie nicht mehr angewendet werden können.
    /// </summary>
    public class Strategy
    {
        public  List<Field> all_fields = new List<Field>();
        public  List<bool> checklist = new List<bool>();
        public  List<Field> faults = new List<Field>();
        private  CancellationToken token;


        public Strategy(List<Field> fields, CancellationToken cancellation) 
        { all_fields = fields; token = cancellation; }


        /// <summary>
        /// Prinzip: Wenn eine Möglichkeit alleine in einem Feld steht und kein anderer Kandidat diesen Platz beansprucht, dann muss diese Zahl eingetragen werden.
        /// <paramref name="input"/> 0 = Eindeutig | 1 = Semi-Eindeutig (wenn sie duch Raten entstehen)
        /// </summary>
        public async Task Naked_Single(int input)
        {
            //Geht solange die Blöcke durch bis es keine Änderungen mehr gibt
            bool is_saturadet = false;
            while (is_saturadet == false)
            {
                is_saturadet = true;

                //Geht alle Blöcke durch
                for (int p = 0; p < 9; p++)
                {
                    //Wenn der Block schon komplett ausgefüllt ist überspringe
                    if (!MainPage.Fields[p].Any(x => x.Number == 0))
                        continue;

                    //Geht die Felder in dem Block durch, die keine Zahl eingetragen haben und die nur eine Möglichkeit besitzten
                    foreach (Field f in MainPage.Fields[p].Where(x=>x.Number == 0 && x.Possebilities.Count == 1).ToList())
                    {
                        //Wenn in der Spalte auch diese Möglichkeit existiert dann überspringe
                        if (all_fields.Any(x => x.Column_Number == f.Column_Number && x.Possebilities.Contains(f.Possebilities.First()) == true && x != f))
                            continue;

                        //Wenn in der Reihe auch diese Möglichkeit existiert dann überspringe
                        if (all_fields.Any(x => x.Row_Number == f.Row_Number && x.Possebilities.Contains(f.Possebilities.First()) == true && x != f))
                            continue;

                        // Platzhalter Feld für das ausgewählte Feld
                        Field Placeholder = new Field() { Id = f.Id, Column_Number = f.Column_Number, Number = f.Possebilities.First(), Row_Number = f.Row_Number, Grid_Number = f.Grid_Number, Is_Fault = f.Is_Fault, Is_Select = f.Is_Select, Number_Background_Color = f.Number_Background_Color, Number_Color = f.Number_Color, Visible_Number = f.Visible_Number, Is_Locked = f.Is_Locked };

                        //Überprüft ob diese Zahl im aktuellen Spielfeld erlaubt ist
                        checklist.Clear();
                        faults.Clear();

                        (checklist, faults) = GameRules.Check_Rules_OneMove(all_fields, Placeholder);

                        //Wenn alle Regeln korrekt sind und es keine Fehlerfelder gibt
                        if (checklist.Contains(false) == false && faults.Count == 0)
                        {
                            //Setzt die Zahl zum aktuellen Feld
                            f.Number = Placeholder.Number;

                            //Setzt das Feld auf Einzige
                            if (input == 0)
                                f.Is_Clearly = true;

                            //Setzt das Feld auf Semi-Einzige
                            if (input == 1)
                                f.Is_Semi_Clearly = true;

                            //leere die Möglichkeitsliste und Verweigerungsliste
                            f.Possebilities.Clear();
                            //f.Denails.Clear();

                            //löscht alle gleichen Zahlen die in der Reihe, Spalte und Block als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                            foreach (Field field in all_fields.Where(x => x.Number != 0).Where(x => x.Grid_Number == f.Grid_Number || x.Column_Number == f.Column_Number || x.Row_Number == f.Row_Number).ToList())
                            {
                                field.Possebilities.Remove(f.Number);
                            }

                            //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                            await Waiter.Waiting();

                            is_saturadet = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Prinzip: Wenn eine richtige Möglichkeit zusammen mit einer oder weiteren Zahlen in einem Feld versteckt sind, dann muss diese Zahl eingetragen werden.
        /// <paramref name="input"/> 0 = Eindeutig | 1 = Semi-Eindeutig (wenn sie duch Raten entstehen)
        /// </summary>
        public  async Task Hidden_Single(int input)
        {
            //Geht solange die Blöcke durch bis es keine Änderungen mehr gibt
            bool is_saturadet = false;
            while (is_saturadet == false)
            {
                is_saturadet = true;

                //Geht alle Blöcke durch
                for (int p = 0; p < 9; p++)
                {
                    //Wenn der Block schon komplett ausgefüllt ist überspringe
                    if (!MainPage.Fields[p].Any(x => x.Number == 0))
                        continue;

                    //Geht jede Zahl durch
                    for(int z = 1; z<10;z++)
                    {
                        //Wenn es in dem Block nur ein Feld gibt was diese Zahl als Möglichkeit hat
                        if (MainPage.Fields[p].Count(x=>x.Possebilities.Any(y=>y==z) == true) == 1)
                        {
                            //Das Feld was diese Zahl als möglichkeit besitzt
                            Field f = MainPage.Fields[p].First(x => x.Possebilities.Any(y => y == z));

                            ////Wenn in der Spalte auch diese Möglichkeit existiert dann überspringe
                            //if (all_fields.Any(x => x.Column_Number == f.Column_Number && x.Possebilities.Contains(f.Possebilities.First()) == true && x != f))
                            //    continue;

                            ////Wenn in der Reihe auch diese Möglichkeit existiert dann überspringe
                            //if (all_fields.Any(x => x.Row_Number == f.Row_Number && x.Possebilities.Contains(f.Possebilities.First()) == true && x != f))
                            //    continue;

                            // Platzhalter Feld für das ausgewählte Feld
                            Field Placeholder = new Field() { Id = f.Id, Column_Number = f.Column_Number, Number = z, Row_Number = f.Row_Number, Grid_Number = f.Grid_Number, Is_Fault = f.Is_Fault, Is_Select = f.Is_Select, Number_Background_Color = f.Number_Background_Color, Number_Color = f.Number_Color, Visible_Number = f.Visible_Number, Is_Locked = f.Is_Locked };

                            //Überprüft ob diese Zahl im aktuellen Spielfeld erlaubt ist
                            checklist.Clear();
                            faults.Clear();

                            (checklist, faults) = GameRules.Check_Rules_OneMove(all_fields, Placeholder);

                            //Wenn alle Regeln korrekt sind und es keine Fehlerfelder gibt
                            if (checklist.Contains(false) == false && faults.Count == 0)
                            {
                                //Setzt die Zahl zum aktuellen Feld
                                f.Number = Placeholder.Number;

                                //Setzt das Feld auf Einzige
                                if (input == 0)
                                    f.Is_Clearly = true;

                                //Setzt das Feld auf Semi-Einzige
                                if (input == 1)
                                    f.Is_Semi_Clearly = true;

                                //leere die Möglichkeitsliste und Verweigerungsliste
                                f.Possebilities.Clear();
                                //f.Denails.Clear();

                                //löscht alle gleichen Zahlen die in der Reihe, Spalte und Block als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                                foreach (Field field in all_fields.Where(x=>x.Number != 0).Where(x=>x.Grid_Number == f.Grid_Number || x.Column_Number == f.Column_Number || x.Row_Number == f.Row_Number).ToList())
                                {
                                    field.Possebilities.Remove(f.Number);
                                }

                                //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                await Waiter.Waiting();

                                is_saturadet = false;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Prinzip: Wenn in zwei Felder eines gemeinschaftlichen Bereiches zwei gleiche Zahlen alleine stehen, dann werden alle anderen gleichen Zahlen im Einflußbereich als Möglichkeit verworfen
        /// </summary>
        public  async Task Naked_Pair()
        {
            //Die offenen Zwillinge die die gleiche Möglichkeitsliste besitzten 
            List<Field> naked_pair = new List<Field>();

            //Geht solange die Bereiche durch bis es keine Änderungen mehr gibt
            bool is_saturadet = false;
            while (is_saturadet == false)
            {
                is_saturadet = true;

                //Geht jedes Feld was keine Zahl hat durch
                foreach(Field f in all_fields.Where(x=>x.Number != 0))
                {
                    //Wenn es in dem Block nur zwei Felder gibt die die gleiche Möglichkeitsliste besitzt
                    if (all_fields.Count(x => x.Possebilities.Equals(f.Possebilities) == true && x.Grid_Number == f.Grid_Number) == 2)
                    {
                        //Ermittelt die offenen Zwillinge
                        naked_pair = all_fields.Where(x => x.Possebilities.Equals(f.Possebilities) == true && x.Grid_Number == f.Grid_Number).ToList();

                        //löscht alle gleichen Zahlen die in dem Block als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        if(all_fields.Any(x => x.Number != 0 && x.Grid_Number == f.Grid_Number && naked_pair.Contains(x) == false))
                        {
                            foreach (Field field in all_fields.Where(x => x.Number != 0).Where(x => x.Grid_Number == f.Grid_Number && naked_pair.Contains(x) == false).ToList())
                            {
                                foreach (int np in naked_pair.First().Possebilities)
                                {
                                    field.Possebilities.Remove(np);

                                    //if (!field.Denails.Any(x => x == np))
                                    //    field.Denails.Add(np);

                                    is_saturadet = false;
                                }
                            }
                        }

                        //löscht alle gleichen Zahlen die in der Reihe als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        if (naked_pair[0].Row_Number == naked_pair[1].Row_Number)
                        {
                            foreach (Field field in all_fields.Where(x => x.Number != 0).Where(x => x.Row_Number == f.Row_Number && naked_pair.Contains(x) == false).ToList())
                            {
                                foreach (int np in naked_pair.First().Possebilities)
                                {
                                    field.Possebilities.Remove(np);

                                    //if (!field.Denails.Any(x => x == np))
                                    //    field.Denails.Add(np);

                                    is_saturadet = false;
                                }
                            }
                        }

                        //löscht alle gleichen Zahlen die in der Spalte als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        if (naked_pair[0].Column_Number == naked_pair[1].Column_Number)
                        {
                            foreach (Field field in all_fields.Where(x => x.Number != 0).Where(x => x.Column_Number == f.Column_Number && naked_pair.Contains(x) == false).ToList())
                            {
                                foreach (int np in naked_pair.First().Possebilities)
                                {
                                    field.Possebilities.Remove(np);

                                    //if (!field.Denails.Any(x => x == np))
                                    //    field.Denails.Add(np);


                                    is_saturadet = false;
                                }
                            }
                        }

                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                        await Waiter.Waiting();
                    }

                    //Wenn es in der Reihe nur zwei Felder gibt die die gleiche Möglichkeitsliste besitzt
                    if (all_fields.Count(x => x.Possebilities.Equals(f.Possebilities) == true && x.Row_Number == f.Row_Number) == 2)
                    {
                        //Ermittelt die offenen Zwillinge
                        naked_pair = all_fields.Where(x => x.Possebilities.Equals(f.Possebilities) == true && x.Row_Number == f.Row_Number).ToList();

                        //löscht alle gleichen Zahlen die in der Reihe als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        if (naked_pair[0].Row_Number == naked_pair[1].Row_Number)
                        {
                            foreach (Field field in all_fields.Where(x => x.Number != 0).Where(x => x.Row_Number == f.Row_Number && naked_pair.Contains(x) == false).ToList())
                            {
                                foreach (int np in naked_pair.First().Possebilities)
                                {
                                    field.Possebilities.Remove(np);

                                    //if (!field.Denails.Any(x => x == np))
                                    //    field.Denails.Add(np);

                                    is_saturadet = false;
                                }
                            }
                        }

                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                        await Waiter.Waiting();
                    }

                    //Wenn es in der Spalte nur zwei Felder gibt die die gleiche Möglichkeitsliste besitzt
                    if (all_fields.Count(x => x.Possebilities.Equals(f.Possebilities) == true && x.Column_Number == f.Column_Number) == 2)
                    {
                        //Ermittelt die offenen Zwillinge
                        naked_pair = all_fields.Where(x => x.Possebilities.Equals(f.Possebilities) == true && x.Column_Number == f.Column_Number).ToList();

                        //löscht alle gleichen Zahlen die in der Spalte als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        if (naked_pair[0].Column_Number == naked_pair[1].Column_Number)
                        {
                            foreach (Field field in all_fields.Where(x => x.Number != 0).Where(x => x.Column_Number == f.Column_Number && naked_pair.Contains(x) == false).ToList())
                            {
                                foreach (int np in naked_pair.First().Possebilities)
                                {
                                    field.Possebilities.Remove(np);

                                    //if (!field.Denails.Any(x => x == np))
                                    //    field.Denails.Add(np);

                                    is_saturadet = false;
                                }
                            }
                        }

                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                        await Waiter.Waiting();
                    }
                }
            }
        }

        /// <summary>
        /// Prinzip: Wenn nur in zwei Felder eines gemeinschaftlichen Bereiches zwei gleiche Zahlen stehen und sonst im Bereich diese Paar nicht vorkommt, dann werden alle anderen Zahlen in den Felder als Möglichkeit verworfen sowie alle gleiche Zahlen von dem Paar die sich im Einflussbereich befinden
        /// </summary>
        public  async Task Hidden_Pair()
        {
            //Die verdeckten Zwillinge die die gleiche Möglichkeitsliste besitzten 
            List<Field> hidden_pair = new List<Field>();

            //Geht solange die Bereiche durch bis es keine Änderungen mehr gibt
            bool is_saturadet = false;
            while (is_saturadet == false)
            {
                is_saturadet = true;

                //Geht jedes Feld was keine Zahl hat durch
                foreach (Field f in all_fields.Where(x => x.Number == 0))
                {
                    //Wenn es in dem Block nur zwei Feld gibt die zwei Zahlen als gleiche Teilmenge an Möglichkeiten besitzen
                    if (all_fields.Count(x => x.Possebilities.Intersect(f.Possebilities).Count() == 2 && x.Grid_Number == f.Grid_Number) == 2)
                    {
                        //Ermittelt die verdeckten Zwillinge
                        hidden_pair = all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() == 2 && x.Grid_Number == f.Grid_Number).ToList();

                        //Erstellt eine Liste an Zahlen die zu der Teilmenge gehören
                        List<int> pairs = new List<int>(hidden_pair[0].Possebilities.Except(hidden_pair[1].Possebilities).Union(hidden_pair[1].Possebilities.Except(hidden_pair[0].Possebilities)).ToList());

                        //löscht all anderen Möglichkeiten in den versteckten Zwillingen die nicht zu der gleichen Teilmenge gehören
                        foreach (int np in pairs)
                        {
                            hidden_pair[0].Possebilities.Remove(np);
                            hidden_pair[1].Possebilities.Remove(np);

                            //if (!hidden_pair[0].Denails.Any(x => x == np))
                            //    hidden_pair[0].Denails.Add(np);
                            //if (!hidden_pair[1].Denails.Any(x => x == np))
                            //    hidden_pair[1].Denails.Add(np);
                        }

                        //löscht alle gleichen Zahlen die in dem Block als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        if (all_fields.Any(x => x.Number != 0 && x.Grid_Number == f.Grid_Number && hidden_pair.Contains(x) == false))
                        {
                            foreach (Field field in all_fields.Where(x => x.Number != 0 && x.Grid_Number == f.Grid_Number && hidden_pair.Contains(x) == false && hidden_pair[0].Possebilities.Intersect(hidden_pair[1].Possebilities).Intersect(x.Possebilities).Any()).ToList())
                            {
                                foreach (int np in hidden_pair[0].Possebilities.Intersect(hidden_pair[1].Possebilities).Intersect(field.Possebilities))
                                {
                                    field.Possebilities.Remove(np);

                                    //if (!field.Denails.Any(x => x == np))
                                    //    field.Denails.Add(np);

                                    is_saturadet = false;
                                }
                            }
                        }

                        //löscht alle gleichen Zahlen die in der Reihe als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        if (hidden_pair[0].Row_Number == hidden_pair[1].Row_Number)
                        {
                            foreach (Field field in all_fields.Where(x => x.Number != 0).Where(x => x.Row_Number == f.Row_Number && hidden_pair.Contains(x) == false && hidden_pair[0].Possebilities.Intersect(hidden_pair[1].Possebilities).Intersect(x.Possebilities).Any()).ToList())
                            {
                                foreach (int np in hidden_pair[0].Possebilities.Intersect(hidden_pair[1].Possebilities).Intersect(field.Possebilities))
                                {
                                    field.Possebilities.Remove(np);

                                    //if (!field.Denails.Any(x => x == np))
                                    //    field.Denails.Add(np);

                                    is_saturadet = false;
                                }
                            }
                        }

                        //löscht alle gleichen Zahlen die in der Spalte als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        if (hidden_pair[0].Column_Number == hidden_pair[1].Column_Number)
                        {
                            foreach (Field field in all_fields.Where(x => x.Number != 0).Where(x => x.Column_Number == f.Column_Number && hidden_pair.Contains(x) == false && hidden_pair[0].Possebilities.Intersect(hidden_pair[1].Possebilities).Intersect(x.Possebilities).Any()).ToList())
                            {
                                foreach (int np in hidden_pair[0].Possebilities.Intersect(hidden_pair[1].Possebilities).Intersect(field.Possebilities))
                                {
                                    field.Possebilities.Remove(np);

                                    //if (!field.Denails.Any(x => x == np))
                                    //    field.Denails.Add(np);

                                    is_saturadet = false;
                                }
                            }
                        }

                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                        await Waiter.Waiting();
                    }

                    //Wenn es in der Reihe nur zwei Feld gibt was die gleiche Möglichkeitsliste besitzt
                    if (all_fields.Count(x => x.Possebilities.Intersect(f.Possebilities).Count() == 2 && x.Row_Number == f.Row_Number) == 2)
                    {
                        //Ermittelt die offenen Zwillinge
                        hidden_pair = all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() == 2 && x.Row_Number == f.Row_Number).ToList();

                        //Erstellt eine Liste an Zahlen die zu der Teilmenge gehören
                        List<int> pairs = new List<int>(hidden_pair[0].Possebilities.Except(hidden_pair[1].Possebilities).Union(hidden_pair[1].Possebilities.Except(hidden_pair[0].Possebilities)).ToList());

                        //löscht all anderen Möglichkeiten in den versteckten Zwillingen die nicht zu der gleichen Teilmenge gehören
                        foreach (int np in pairs)
                        {
                            hidden_pair[0].Possebilities.Remove(np);
                            hidden_pair[1].Possebilities.Remove(np);

                            //if (!hidden_pair[0].Denails.Any(x => x == np))
                            //    hidden_pair[0].Denails.Add(np);
                            //if (!hidden_pair[1].Denails.Any(x => x == np))
                            //    hidden_pair[1].Denails.Add(np);
                        }

                        //löscht alle gleichen Zahlen die in der Reihe als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        if (hidden_pair[0].Row_Number == hidden_pair[1].Row_Number)
                        {
                            foreach (Field field in all_fields.Where(x => x.Number != 0).Where(x => x.Row_Number == f.Row_Number && hidden_pair.Contains(x) == false && hidden_pair[0].Possebilities.Intersect(hidden_pair[1].Possebilities).Intersect(x.Possebilities).Any()).ToList())
                            {
                                foreach (int np in hidden_pair[0].Possebilities.Intersect(hidden_pair[1].Possebilities).Intersect(field.Possebilities))
                                {
                                    field.Possebilities.Remove(np);

                                    //if (!field.Denails.Any(x => x == np))
                                    //    field.Denails.Add(np);

                                    is_saturadet = false;
                                }
                            }
                        }

                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                        await Waiter.Waiting();
                    }

                    //Wenn es in der Spalte nur zwei Feld gibt was die gleiche Möglichkeitsliste besitzt
                    if (all_fields.Count(x => x.Possebilities.Intersect(f.Possebilities).Count() == 2 && x.Column_Number == f.Column_Number) == 2)
                    {
                        //Ermittelt die offenen Zwillinge
                        hidden_pair = all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() == 2 && x.Column_Number == f.Column_Number).ToList();

                        //Erstellt eine Liste an Zahlen die zu der Teilmenge gehören
                        List<int> pairs = new List<int>(hidden_pair[0].Possebilities.Except(hidden_pair[1].Possebilities).Union(hidden_pair[1].Possebilities.Except(hidden_pair[0].Possebilities)).ToList());

                        //löscht all anderen Möglichkeiten in den versteckten Zwillingen die nicht zu der gleichen Teilmenge gehören
                        foreach (int np in pairs)
                        {
                            hidden_pair[0].Possebilities.Remove(np);
                            hidden_pair[1].Possebilities.Remove(np);

                            //if (!hidden_pair[0].Denails.Any(x => x == np))
                            //    hidden_pair[0].Denails.Add(np);
                            //if (!hidden_pair[1].Denails.Any(x => x == np))
                            //    hidden_pair[1].Denails.Add(np);
                        }

                        //löscht alle gleichen Zahlen die in der Spalte als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        if (hidden_pair[0].Column_Number == hidden_pair[1].Column_Number)
                        {
                            foreach (Field field in all_fields.Where(x => x.Number != 0).Where(x => x.Column_Number == f.Column_Number && hidden_pair.Contains(x) == false && hidden_pair[0].Possebilities.Intersect(hidden_pair[1].Possebilities).Intersect(x.Possebilities).Any()).ToList())
                            {
                                foreach (int np in hidden_pair[0].Possebilities.Intersect(hidden_pair[1].Possebilities).Intersect(field.Possebilities))
                                {
                                    field.Possebilities.Remove(np);

                                    //if (!field.Denails.Any(x => x == np))
                                    //    field.Denails.Add(np);

                                    is_saturadet = false;
                                }
                            }
                        }

                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                        await Waiter.Waiting();
                    }
                }
            }
        }

        /// <summary>
        /// Prinzip: Wenn in drei Felder eines gemeinschaftlichen Bereiches eine Teilmenge gleicher Zahlen alleine (max 3) stehen, dann werden alle anderen gleichen Zahlen im Einflußbereich als Möglichkeit verworfen
        /// </summary>
        public  async Task Naked_Trible()
        {
            //Die offenen Drillinge die die gleiche Möglichkeitsliste besitzten 
            List<Field> naked_trible = new List<Field>();

            //Geht solange die Bereiche durch bis es keine Änderungen mehr gibt
            bool is_saturadet = false;
            while (is_saturadet == false)
            {
                is_saturadet = true;

                //Geht jedes Feld was keine Zahl hat durch
                foreach (Field f in all_fields.Where(x => x.Number == 0))
                {
                    //Wenn die Möglichkeiten dieses Feldes nicht 3 sind dann überspringe
                    if (f.Possebilities.Count != 3)
                        continue;

                    //Wenn es in dem Block nur drei Feld gibt die zwischen zwei und drei Zahlen als gleiche Teilmenge an Möglichkeiten besitzen
                    if (all_fields.Count(x => x.Possebilities.Intersect(f.Possebilities).Count() == 2 || x.Possebilities.Intersect(f.Possebilities).Count() == 3 && x.Grid_Number == f.Grid_Number) == 3)
                    {
                        //Ermittelt die offenen Drillinge
                        naked_trible = all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() == 2 || x.Possebilities.Intersect(f.Possebilities).Count() == 3 && x.Grid_Number == f.Grid_Number).ToList();

                        //löscht alle gleichen Zahlen die in dem Block als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        if (all_fields.Any(x => x.Number != 0 && x.Grid_Number == f.Grid_Number && naked_trible.Contains(x) == false))
                        {
                            foreach (Field field in all_fields.Where(x => x.Number != 0 && x.Grid_Number == f.Grid_Number && naked_trible.Contains(x) == false && f.Possebilities.Intersect(x.Possebilities).Any()).ToList())
                            {
                                foreach (int np in f.Possebilities.Intersect(field.Possebilities))
                                {
                                    field.Possebilities.Remove(np);

                                    //if (!field.Denails.Any(x => x == np))
                                    //    field.Denails.Add(np);

                                    is_saturadet = false;
                                }
                            }
                        }

                        //löscht alle gleichen Zahlen die in der Reihe als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        if (naked_trible[0].Row_Number == naked_trible[1].Row_Number && naked_trible[1].Row_Number == naked_trible[2].Row_Number)
                        {
                            foreach (Field field in all_fields.Where(x => x.Number != 0 && x.Row_Number == f.Row_Number && naked_trible.Contains(x) == false && f.Possebilities.Intersect(x.Possebilities).Any()).ToList())
                            {
                                foreach (int np in f.Possebilities.Intersect(field.Possebilities))
                                {
                                    field.Possebilities.Remove(np);

                                    //if (!field.Denails.Any(x => x == np))
                                    //    field.Denails.Add(np);

                                    is_saturadet = false;
                                }
                            }
                        }

                        //löscht alle gleichen Zahlen die in der Spalte als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        if (naked_trible[0].Column_Number == naked_trible[1].Column_Number && naked_trible[1].Column_Number == naked_trible[2].Column_Number)
                        {
                            foreach (Field field in all_fields.Where(x => x.Number != 0 && x.Column_Number == f.Column_Number && naked_trible.Contains(x) == false && f.Possebilities.Intersect(x.Possebilities).Any()).ToList())
                            {
                                foreach (int np in f.Possebilities.Intersect(field.Possebilities))
                                {
                                    field.Possebilities.Remove(np);

                                    //if (!field.Denails.Any(x => x == np))
                                    //    field.Denails.Add(np);

                                    is_saturadet = false;
                                }
                            }
                        }

                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                        await Waiter.Waiting();
                    }

                    //Wenn es in der Reihe nur drei Feld gibt die zwischen zwei und drei Zahlen als gleiche Teilmenge an Möglichkeiten besitzen
                    if (all_fields.Count(x => x.Possebilities.Intersect(f.Possebilities).Count() == 2 || x.Possebilities.Intersect(f.Possebilities).Count() == 3 && x.Row_Number == f.Row_Number) == 3)
                    {
                        //Ermittelt die offenen Drillinge
                        naked_trible = all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() == 2 || x.Possebilities.Intersect(f.Possebilities).Count() == 3 && x.Row_Number == f.Row_Number).ToList();

                        //löscht alle gleichen Zahlen die in der Reihe als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        if (naked_trible[0].Row_Number == naked_trible[1].Row_Number && naked_trible[1].Row_Number == naked_trible[2].Row_Number)
                        {
                            foreach (Field field in all_fields.Where(x => x.Number != 0 && x.Row_Number == f.Row_Number && naked_trible.Contains(x) == false && f.Possebilities.Intersect(x.Possebilities).Any()).ToList())
                            {
                                foreach (int np in f.Possebilities.Intersect(field.Possebilities))
                                {
                                    field.Possebilities.Remove(np);

                                    //if (!field.Denails.Any(x => x == np))
                                    //    field.Denails.Add(np);

                                    is_saturadet = false;
                                }
                            }
                        }

                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                        await Waiter.Waiting();
                    }

                    //Wenn es in der Spalte nur drei Feld gibt die zwischen zwei und drei Zahlen als gleiche Teilmenge an Möglichkeitsliste besitzt
                    if (all_fields.Count(x => x.Possebilities.Intersect(f.Possebilities).Count() == 2 || x.Possebilities.Intersect(f.Possebilities).Count() == 3 && x.Column_Number == f.Column_Number) == 3)
                    {
                        //Ermittelt die offenen Drillinge
                        naked_trible = all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() == 2 || x.Possebilities.Intersect(f.Possebilities).Count() == 3 && x.Column_Number == f.Column_Number).ToList();

                        //löscht alle gleichen Zahlen die in der Spalte als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        if (naked_trible[0].Column_Number == naked_trible[1].Column_Number && naked_trible[1].Column_Number == naked_trible[2].Column_Number)
                        {
                            foreach (Field field in all_fields.Where(x => x.Number != 0 && x.Column_Number == f.Column_Number && naked_trible.Contains(x) == false && f.Possebilities.Intersect(x.Possebilities).Any()).ToList())
                            {
                                foreach (int np in f.Possebilities.Intersect(field.Possebilities))
                                {
                                    field.Possebilities.Remove(np);

                                    //if (!field.Denails.Any(x => x == np))
                                    //    field.Denails.Add(np);

                                    is_saturadet = false;
                                }
                            }
                        }

                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                        await Waiter.Waiting();
                    }
                }
            }
        }

        /// <summary>
        /// Prinzip: Wenn in drei Felder eines gemeinschaftlichen Bereiches eine Teilmenge gleicher Zahlen (max 3) stehen, dann werden alle anderen gleichen Zahlen im Einflußbereich als Möglichkeit verworfen
        /// </summary>
        public  async Task Hidden_Trible()
        {
            //Die verdeckten Drillinge die die gleiche Möglichkeitsliste besitzten 
            List<Field> hidden_trible = new List<Field>();

            //Geht solange die Bereiche durch bis es keine Änderungen mehr gibt
            bool is_saturadet = false;
            while (is_saturadet == false)
            {
                is_saturadet = true;

                //Geht jedes Feld was keine Zahl hat durch
                foreach (Field f in all_fields.Where(x => x.Number == 0))
                {
                    //Wenn die Möglichkeiten dieses Feldes kleiner 3 sind dann überspringe
                    if (f.Possebilities.Count >= 3)
                        continue;

                    //Wenn es in dem Block nur drei Feld gibt die zwischen zwei und drei Zahlen als gleiche Teilmenge an Möglichkeiten besitzen
                    if (all_fields.Count(x => x.Possebilities.Intersect(f.Possebilities).Count() == 2 || x.Possebilities.Intersect(f.Possebilities).Count() == 3 && x.Grid_Number == f.Grid_Number) == 3)
                    {
                        //Ermittelt die verdeckten Drillinge
                        hidden_trible = all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() == 2 || x.Possebilities.Intersect(f.Possebilities).Count() == 3 && x.Grid_Number == f.Grid_Number).ToList();

                        //Eine Liste an Möglichkeiten die zu den verdeckten Drillingen gehören 
                        List<int> trible_numbers = new List<int>(f.Possebilities);

                        //Ermittelt die Möglichkeiten die nicht zum Drilling gehören (Möglichkeiten die auch noch in einem anderen Feld zufinden ist ausßer den Drillingen)
                        foreach (int i in f.Possebilities)
                        {
                            if(all_fields.Any(x=>x.Grid_Number == f.Grid_Number && x.Number != 0 && x.Possebilities.Contains(i) && hidden_trible.Contains(x) == false))
                                trible_numbers.Remove(i);
                        }

                        //löscht all anderen Möglichkeiten in den versteckten Drillinge die nicht zu der gleichen Teilmenge gehören
                        foreach (Field ht in hidden_trible)
                        {
                            List<int> tribles = ht.Possebilities.Where(x => trible_numbers.Contains(x) == false).ToList();
                            foreach (int np in tribles)
                            {
                                ht.Possebilities.Remove(np);

                                //if (!ht.Denails.Any(x => x == np))
                                //    ht.Denails.Add(np);

                                is_saturadet = false;
                            }
                        }

                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                        await Waiter.Waiting();
                    }

                    //Wenn es in der Reihe nur drei Feld gibt die zwischen zwei und drei Zahlen als gleiche Teilmenge an Möglichkeiten besitzen
                    if (all_fields.Count(x => x.Possebilities.Intersect(f.Possebilities).Count() == 2 || x.Possebilities.Intersect(f.Possebilities).Count() == 3 && x.Row_Number == f.Row_Number) == 3)
                    {
                        //Ermittelt die verdeckten Drillinge
                        hidden_trible = all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() == 2 || x.Possebilities.Intersect(f.Possebilities).Count() == 3 && x.Row_Number == f.Row_Number).ToList();

                        //Eine Liste an Möglichkeiten die zu den verdeckten Drillingen gehören 
                        List<int> trible_numbers = new List<int> (f.Possebilities);

                        //Ermittelt die Möglichkeiten die nicht zum Drilling gehören (Möglichkeiten die auch noch in einem anderen Feld zufinden ist ausßer den Drillingen)
                        foreach (int i in f.Possebilities)
                        {
                            if (all_fields.Any(x => x.Row_Number == f.Row_Number && x.Number != 0 && x.Possebilities.Contains(i) && hidden_trible.Contains(x) == false))
                                trible_numbers.Remove(i);
                        }

                        //löscht all anderen Möglichkeiten in den versteckten Drillinge die nicht zu der gleichen Teilmenge gehören
                        foreach (Field ht in hidden_trible)
                        {
                            List<int> tribles = ht.Possebilities.Where(x => trible_numbers.Contains(x) == false).ToList();
                            foreach (int np in tribles)
                            {
                                ht.Possebilities.Remove(np);

                                //if (!ht.Denails.Any(x => x == np))
                                //    ht.Denails.Add(np);

                                is_saturadet = false;
                            }
                        }

                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                        await Waiter.Waiting();
                    }

                    //Wenn es in der Spalte nur drei Feld gibt die zwischen zwei und drei Zahlen als gleiche Teilmenge an Möglichkeitsliste besitzt
                    if (all_fields.Count(x => x.Possebilities.Intersect(f.Possebilities).Count() == 2 || x.Possebilities.Intersect(f.Possebilities).Count() == 3 && x.Column_Number == f.Column_Number) == 3)
                    {
                        //Ermittelt die verdeckten Drillinge
                        hidden_trible = all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() == 2 || x.Possebilities.Intersect(f.Possebilities).Count() == 3 && x.Column_Number == f.Column_Number).ToList();

                        //Eine Liste an Möglichkeiten die zu den verdeckten Drillingen gehören 
                        List<int> trible_numbers = new List<int>(f.Possebilities);

                        //Ermittelt die Möglichkeiten die nicht zum Drilling gehören (Möglichkeiten die auch noch in einem anderen Feld zufinden ist ausßer den Drillingen)
                        foreach (int i in f.Possebilities)
                        {
                            if (all_fields.Any(x => x.Column_Number == f.Column_Number && x.Number != 0 && x.Possebilities.Contains(i) && hidden_trible.Contains(x) == false))
                                trible_numbers.Remove(i);
                        }

                        //löscht all anderen Möglichkeiten in den versteckten Drillinge die nicht zu der gleichen Teilmenge gehören
                        foreach (Field ht in hidden_trible)
                        {
                            List<int> tribles = ht.Possebilities.Where(x => trible_numbers.Contains(x) == false).ToList();
                            foreach (int np in tribles)
                            {
                                ht.Possebilities.Remove(np);

                                //if (!ht.Denails.Any(x => x == np))
                                //    ht.Denails.Add(np);

                                is_saturadet = false;
                            }
                        }

                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                        await Waiter.Waiting();
                    }
                }
            }
        }

        /// <summary>
        /// Wenn innerhalb eines Blocks alle Kandidaten einer bestimmten Ziffer auf eine Zeile oder eine Spalte beschränkt sind, kann diese Ziffer in dieser Zeile oder Spalte außerhalb des Blocks nicht mehr auftreten.
        /// </summary>
        public  async Task Locked_Candidates_Typ_1()
        {
            //Geht solange die Bereiche durch bis es keine Änderungen mehr gibt
            bool is_saturadet = false;
            while (is_saturadet == false)
            {
                is_saturadet = true;

                //Geht jeden Block durch wo noch nicht alles gefüllt ist
                foreach (ObservableCollection<Field> lf in MainPage.Fields.Where(x=> x.Any(y => y.Number == 0)))
                {
                    //Geht jede Zahl von den Möglichkeiten durch
                    for(int i = 1; i<10;i++)
                    {
                        //Die Felder wo die Möglichkeiten gelöscht werden sollen
                        List<Field> targets = new List<Field>();

                        //Wenn in dem Block Felder gibt die diese Möglichkeit besitzen
                        if (lf.Any(x => x.Possebilities.Contains(i)))
                        {
                            //Wenn es im Block nur Felder gibt die in einer Spalte sind und diese Möglichkeit besitzten
                            if(!lf.Where(x => x.Possebilities.Contains(i)).Any(y=>y.Column_Number != lf.First(x => x.Possebilities.Contains(i)).Column_Number))
                            {
                                //Der erste Kandidat der diese Möglichkeit besitzt in dem Block
                                Field locked_candidate = lf.First(x => x.Possebilities.Contains(i));

                                //Wenn es Felder gibt wo es diese Möglichkeit gibt und sie in der selben Splate sind 
                                if(all_fields.Any(x=>x.Column_Number == locked_candidate.Column_Number && x.Possebilities.Contains(i)))
                                {
                                    //Die Felder wo die Möglichkeiten gelöscht werden sollen
                                    targets = all_fields.Where(x => x.Column_Number == locked_candidate.Column_Number && x.Possebilities.Contains(i)).ToList();
                                }
                            }

                            //Wenn es im Block nur Felder gibt die in einer Reihe sind und diese Möglichkeit besitzten
                            if (!lf.Where(x => x.Possebilities.Contains(i)).Any(y => y.Row_Number != lf.First(x => x.Possebilities.Contains(i)).Row_Number))
                            {
                                //Der erste Kandidat der diese Möglichkeit besitzt in dem Block
                                Field locked_candidate = lf.First(x => x.Possebilities.Contains(i));

                                //Wenn es Felder gibt wo es diese Möglichkeit gibt und sie in der selben Splate sind 
                                if (all_fields.Any(x => x.Row_Number == locked_candidate.Row_Number && x.Possebilities.Contains(i)))
                                {
                                    //Die Felder wo die Möglichkeiten gelöscht werden sollen
                                    targets = all_fields.Where(x => x.Row_Number == locked_candidate.Row_Number && x.Possebilities.Contains(i)).ToList();
                                }
                            }

                            //Wenn keine Felder gibt wo die Möglichkeit gelöscht werden kann
                            if (targets.Count == 0)
                                continue;

                            //Geht die Felder wo die Möglichkeiten gelöscht werden sollen durch
                            foreach (Field f in targets)
                            {
                                //Wenn es Felder gibt in dem gleichen Block wo die Möglichkeit gelöscht werden soll, die auch diese Möglichkeit besitzten
                                if (all_fields.Any(x => !targets.Contains(x) && x.Possebilities.Contains(i) && x.Grid_Number == f.Grid_Number))
                                {
                                    //löscht diese Möglichkeit in diesem Feld
                                    f.Possebilities.Remove(i);

                                    //if (!f.Denails.Any(x => x == i))
                                    //    f.Denails.Add(i);

                                    //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                    await Waiter.Waiting();

                                    is_saturadet = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Wenn in einer Zeile (oder eine Spalte) alle Kandidaten einer Ziffer auf einen Block beschränkt sind, kann man diesen Kandidaten von allen anderen Zellen des Blocks eliminieren.
        /// </summary>
        public  async Task Locked_Candidates_Typ_2()
        {
            //Geht solange die Bereiche durch bis es keine Änderungen mehr gibt
            bool is_saturadet = false;
            while (is_saturadet == false)
            {
                is_saturadet = true;

                //Geht jeden Block durch wo noch nicht alles gefüllt ist
                foreach (ObservableCollection<Field> lf in MainPage.Fields.Where(x => x.Any(y => y.Number == 0)))
                {
                    //Geht jede Zahl von den Möglichkeiten durch
                    for (int i = 1; i < 10; i++)
                    {
                        //Die Felder wo die Möglichkeiten besteht und in der selben Reihe sind 
                        List<List<Field>> rows = new List<List<Field>>();

                        //Die Felder wo die Möglichkeiten besteht und in der selben Spalte sind 
                        List<List<Field>> columns = new List<List<Field>>();

                        //Wenn in dem Block Felder gibt die diese Möglichkeit besitzen
                        if (lf.Any(x => x.Possebilities.Contains(i)))
                        {
                            //Erstellt die Reihen und Spalten eines Blockes als einzelne Listen
                            foreach(Field f in lf.Where(x => x.Possebilities.Contains(i)))
                            {
                                if(rows.Any(x=>x.Any(y=>y.Row_Number == f.Row_Number)))
                                    rows.First(x=>x.Any(x=>x.Row_Number == f.Row_Number)).Add(f);

                                if (columns.Any(x => x.Any(y => y.Column_Number == f.Column_Number)))
                                    columns.First(x => x.Any(x => x.Column_Number == f.Column_Number)).Add(f);
                            }


                            //Geht die Reihe durch wo die Möglichkeit auftritt und schaut ob die Möglichkeit auf diese Felder in diesem Block beschränkt sind 
                            bool ignore = false;
                            foreach (List<Field> fl in rows.Where(x=>x.Count >=2))
                            {
                                //Wenn in dierser Reihe nur diese Felder die Mäglichkeiten besitzten
                                if(all_fields.Any(x=>x.Row_Number == fl.First().Row_Number && fl.Contains(x) == false && x.Possebilities.Contains(i) == false))
                                {
                                    //Löscht die Möglichkeit in den anderen Feldern
                                    foreach (Field f in lf.Where(x => x.Possebilities.Contains(i) && fl.Contains(x) == false))
                                    {
                                        //löscht diese Möglichkeit in diesem Feld
                                        f.Possebilities.Remove(i);

                                        //if (!f.Denails.Any(x => x == i))
                                        //    f.Denails.Add(i);

                                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                        await Waiter.Waiting();

                                        is_saturadet = false;
                                        ignore = true;
                                        break;
                                    }
                                }
                            }

                            //Wenn schon eine Reihe als beschränkt wurde dann überspringe
                            if (ignore == true)
                                continue;

                            //Geht die Spalte durch wo die Möglichkeit auftritt und schaut ob die Möglichkeit auf diese Felder in diesem Block beschränkt sind 
                            foreach (List<Field> fl in columns.Where(x => x.Count >= 2))
                            {
                                //Wenn in dierser Spalte nur diese Felder die Mäglichkeiten besitzten
                                if (all_fields.Any(x => x.Column_Number == fl.First().Column_Number && fl.Contains(x) == false && x.Possebilities.Contains(i) == false))
                                {
                                    //Löscht die Möglichkeit in den anderen Feldern
                                    foreach (Field f in lf.Where(x => x.Possebilities.Contains(i) && fl.Contains(x) == false))
                                    {
                                        //löscht diese Möglichkeit in diesem Feld
                                        f.Possebilities.Remove(i);

                                        //if (!f.Denails.Any(x => x == i))
                                        //    f.Denails.Add(i);

                                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                        await Waiter.Waiting();

                                        is_saturadet = false;

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
