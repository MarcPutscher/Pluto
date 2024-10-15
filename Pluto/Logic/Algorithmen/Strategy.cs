using CommunityToolkit.Maui.Core.Extensions;
using Microsoft.Maui.Controls.PlatformConfiguration;
using MvvmHelpers;
using Pluto.Models;
using Pluto.Pages;
using System;
using System.Collections;
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

                //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                //Geht alle Blöcke durch
                for (int p = 0; p < 9; p++)
                {
                    //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    //Wenn der Block schon komplett ausgefüllt ist überspringe
                    if (!MainPage.Fields[p].Any(x => x.Number == 0))
                        continue;

                    //Geht die Felder in dem Block durch, die keine Zahl eingetragen haben und die nur eine Möglichkeit besitzten
                    foreach (Field f in MainPage.Fields[p].Where(x=>x.Number == 0 && x.Possebilities.Count == 1).ToList())
                    {
                        //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                        if (token.IsCancellationRequested)
                        {
                            token.ThrowIfCancellationRequested();
                        }

                        ////Wenn in der Spalte auch diese Möglichkeit existiert dann überspringe
                        //if (all_fields.Where(x=>x.Number == 0).Any(x => x.Column_Number == f.Column_Number && x.Possebilities.Contains(f.Possebilities.First()) == true && x != f))
                        //    continue;

                        ////Wenn in der Reihe auch diese Möglichkeit existiert dann überspringe
                        //if (all_fields.Where(x => x.Number == 0).Any(x => x.Row_Number == f.Row_Number && x.Possebilities.Contains(f.Possebilities.First()) == true && x != f))
                        //    continue;

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

                            //Logbucheintrag
                            MainPage.Logs.Insert(0, new Logdata() { ID = f.Id, Grid = f.Grid_Number, Row = f.Row_Number, Column = f.Column_Number }.Place_Number_In_Field(f.Number, all_fields, Logdata.Strategies.Naked_Single));

                            //Wenn diese Technik noch nicht angewand wurde dann erhöhe den Counter dieser Technik, sonst erstelle die Technik in den Used_Technics
                            if(MainPage.Used_Technics.Any(x=>x.Technic == Used_Technic.Technics.Naked_Single))
                            {
                                MainPage.Used_Technics.FirstOrDefault(x => x.Technic == Used_Technic.Technics.Naked_Single).Count++;
                            }
                            else
                            {
                                MainPage.Used_Technics.Add(new Used_Technic() { Count = 1 , Technic = Used_Technic.Technics.Naked_Single});
                            }

                            //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                            await Waiter.Waiting();

                            //löscht alle gleichen Zahlen die in dem Block als Möglichkeit stehen
                            foreach (Field field in all_fields.Where(x => x.Number == 0).Where(x => x.Grid_Number == f.Grid_Number).ToList())
                            {
                                if(field.Possebilities.Remove(f.Number) == true)
                                {
                                    //Logbucheintrag
                                    MainPage.Logs.Insert(0, new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(f.Number, all_fields, Logdata.Strategies.Naked_Single));                            
                                    //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                    await Waiter.Waiting();
                                }
                            }
                            //löscht alle gleichen Zahlen die in der Reihe als Möglichkeit stehen
                            foreach (Field field in all_fields.Where(x => x.Number == 0).Where(x => x.Row_Number == f.Row_Number).ToList())
                            {
                                if (field.Possebilities.Remove(f.Number) == true)
                                {
                                    //Logbucheintrag
                                    MainPage.Logs.Insert(0, new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(f.Number, all_fields, Logdata.Strategies.Naked_Single));
                                    //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                    await Waiter.Waiting();
                                }
                            }
                            //löscht alle gleichen Zahlen die in der Spalte als Möglichkeit stehen
                            foreach (Field field in all_fields.Where(x => x.Number == 0).Where(x => x.Column_Number == f.Column_Number).ToList())
                            {
                                if (field.Possebilities.Remove(f.Number) == true)
                                {
                                    //Logbucheintrag
                                    MainPage.Logs.Insert(0, new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(f.Number, all_fields, Logdata.Strategies.Naked_Single));
                                    //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                    await Waiter.Waiting();
                                }
                            }

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

                //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                //Geht alle Blöcke durch
                for (int p = 0; p < 9; p++)
                {
                    //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    //Wenn der Block schon komplett ausgefüllt ist überspringe
                    if (!MainPage.Fields[p].Any(x => x.Number == 0))
                        continue;

                    //Geht jede Zahl durch
                    for(int z = 1; z<10;z++)
                    {
                        //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                        if (token.IsCancellationRequested)
                        {
                            token.ThrowIfCancellationRequested();
                        }

                        //Wenn es in dem Block nur ein Feld gibt was diese Zahl als Möglichkeit besitzt
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

                                //Logbucheintrag
                                MainPage.Logs.Insert(0, new Logdata() { ID = f.Id, Grid = f.Grid_Number, Row = f.Row_Number, Column = f.Column_Number }.Place_Number_In_Field(f.Number, all_fields, Logdata.Strategies.Hidden_Single));

                                //Wenn diese Technik noch nicht angewand wurde dann erhöhe den Counter dieser Technik, sonst erstelle die Technik in den Used_Technics
                                if (MainPage.Used_Technics.Any(x => x.Technic == Used_Technic.Technics.Hidden_Single))
                                {
                                    MainPage.Used_Technics.FirstOrDefault(x => x.Technic == Used_Technic.Technics.Hidden_Single).Count++;
                                }
                                else
                                {
                                    MainPage.Used_Technics.Add(new Used_Technic() { Count = 1, Technic = Used_Technic.Technics.Hidden_Single });
                                }

                                //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                await Waiter.Waiting();

                                //löscht alle gleichen Zahlen die in dem Block als Möglichkeit stehen
                                foreach (Field field in all_fields.Where(x=>x.Number == 0).Where(x=>x.Grid_Number == f.Grid_Number).ToList())
                                {
                                    if (field.Possebilities.Remove(f.Number) == true)
                                    {
                                        //Logbucheintrag
                                        MainPage.Logs.Insert(0, new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(f.Number, all_fields, Logdata.Strategies.Hidden_Single));
                                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                        await Waiter.Waiting();
                                    }
                                }
                                //löscht alle gleichen Zahlen die in der Reihe als Möglichkeit stehen
                                foreach (Field field in all_fields.Where(x => x.Number == 0).Where(x => x.Row_Number == f.Row_Number).ToList())
                                {
                                    if (field.Possebilities.Remove(f.Number) == true)
                                    {
                                        //Logbucheintrag
                                        MainPage.Logs.Insert(0, new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(f.Number, all_fields, Logdata.Strategies.Hidden_Single));
                                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                        await Waiter.Waiting();
                                    }
                                }
                                //löscht alle gleichen Zahlen die in der Spalte als Möglichkeit stehen
                                foreach (Field field in all_fields.Where(x => x.Number == 0).Where(x => x.Column_Number == f.Column_Number).ToList())
                                {
                                    if (field.Possebilities.Remove(f.Number) == true)
                                    {
                                        //Logbucheintrag
                                        MainPage.Logs.Insert(0, new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(f.Number, all_fields, Logdata.Strategies.Hidden_Single));
                                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                        await Waiter.Waiting();
                                    }
                                }

                                is_saturadet = false;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Prinzip: Wenn in zwei Felder eines gemeinschaftlichen Bereiches zwei gleiche Zahlen alleine stehen, dann werden alle anderen gleichen Zahlen
        /// im Einflußbereich als Möglichkeit verworfen
        /// </summary>
        public  async Task Naked_Pair()
        {
            //Die offenen Zwillinge die die gleiche Möglichkeitsliste besitzten 
            List<Field> naked_pair = new List<Field>();

            //Geht solange die Bereiche durch bis es keine Änderungen mehr gibt
            bool is_saturadet = false;
            while (is_saturadet == false)
            {
                //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                is_saturadet = true;

                //Geht jedes Feld was keine Zahl hat durch
                foreach(Field f in all_fields.Where(x=>x.Number == 0))
                {
                    //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    //Wenn die Möglichkeiten dieses Feldes nicht 2 sind dann überspringe
                    if (f.Possebilities.Count != 2)
                        continue;

                    //Wenn es in dem Block nur zwei Felder gibt die die gleiche Möglichkeitsliste besitzt
                    if (all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() == f.Possebilities.Count && x.Possebilities.Count == 2).Count(x=> x.Grid_Number == f.Grid_Number) == 2)
                    {
                        //Ermittelt die offenen Zwillinge
                        naked_pair = all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() == f.Possebilities.Count && x.Possebilities.Count == 2).Where(x => x.Grid_Number == f.Grid_Number).ToList();

                        //löscht alle gleichen Zahlen die in dem Block als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        foreach (Field field in all_fields.Where(x => x.Number == 0).Where(x => x.Grid_Number == f.Grid_Number && naked_pair.Contains(x) == false).Where(x => x.Possebilities.Intersect(f.Possebilities).Any() == true).ToList())
                        {
                            foreach (int np in naked_pair.First().Possebilities)
                            {
                                if (field.Possebilities.Remove(np) == true)
                                {
                                    //Logbucheintrag
                                    MainPage.Logs.Insert(0, new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Naked_Pair));
                                    //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                    await Waiter.Waiting();

                                    is_saturadet = false;
                                }
                            }
                        }

                        ////löscht alle gleichen Zahlen die in der Reihe als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        //if (naked_pair[0].Row_Number == naked_pair[1].Row_Number)
                        //{
                        //    foreach (Field field in all_fields.Where(x => x.Number == 0).Where(x => x.Row_Number == f.Row_Number && naked_pair.Contains(x) == false).Where(x => x.Possebilities.Intersect(f.Possebilities).Any() == true).ToList())
                        //    {
                        //        foreach (int np in naked_pair.First().Possebilities)
                        //        {
                        //            if (field.Possebilities.Remove(np) == true)
                        //            {
                        //                //Logbucheintrag
                        //                MainPage.Logs.Insert(0, new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Naked_Pair));
                        //                //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                        //                await Waiter.Waiting();

                        //                is_saturadet = false;
                        //            }
                        //        }
                        //    }
                        //}

                        ////löscht alle gleichen Zahlen die in der Spalte als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        //if (naked_pair[0].Column_Number == naked_pair[1].Column_Number)
                        //{
                        //    foreach (Field field in all_fields.Where(x => x.Number == 0).Where(x => x.Column_Number == f.Column_Number && naked_pair.Contains(x) == false).Where(x=>x.Possebilities.Intersect(f.Possebilities).Any() == true).ToList())
                        //    {
                        //        foreach (int np in naked_pair.First().Possebilities)
                        //        {
                        //            if (field.Possebilities.Remove(np) == true)
                        //            {
                        //                //Logbucheintrag
                        //                MainPage.Logs.Insert(0, new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Naked_Pair));
                        //                //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                        //                await Waiter.Waiting();

                        //                is_saturadet = false;
                        //            }
                        //        }
                        //    }
                        //}
                    }

                    //Wenn es in der Reihe nur zwei Felder gibt die die gleiche Möglichkeitsliste besitzt
                    if (all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() == f.Possebilities.Count && x.Possebilities.Count == 2).Count(x=>x.Row_Number == f.Row_Number) == 2)
                    {
                        //Ermittelt die offenen Zwillinge
                        naked_pair = all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() == f.Possebilities.Count && x.Possebilities.Count == 2).Where(x => x.Row_Number == f.Row_Number).ToList();

                        //löscht alle gleichen Zahlen die in der Reihe als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        foreach (Field field in all_fields.Where(x => x.Number == 0).Where(x => x.Row_Number == f.Row_Number && naked_pair.Contains(x) == false).Where(x => x.Possebilities.Intersect(f.Possebilities).Any() == true).ToList())
                        {
                            foreach (int np in naked_pair.First().Possebilities)
                            {
                                if (field.Possebilities.Remove(np) == true)
                                {
                                    //Logbucheintrag
                                    MainPage.Logs.Insert(0, new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Naked_Pair));
                                    //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                    await Waiter.Waiting();

                                    is_saturadet = false;
                                }
                            }
                        }
                    }

                    //Wenn es in der Spalte nur zwei Felder gibt die die gleiche Möglichkeitsliste besitzt
                    if (all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() == f.Possebilities.Count && x.Possebilities.Count == 2).Count(x=>x.Column_Number == f.Column_Number) == 2)
                    {
                        //Ermittelt die offenen Zwillinge
                        naked_pair = all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() == f.Possebilities.Count && x.Possebilities.Count == 2).Where(x => x.Column_Number == f.Column_Number).ToList();

                        //löscht alle gleichen Zahlen die in der Spalte als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        if (naked_pair[0].Column_Number == naked_pair[1].Column_Number)
                        {
                            foreach (Field field in all_fields.Where(x => x.Number == 0).Where(x => x.Column_Number == f.Column_Number && naked_pair.Contains(x) == false).Where(x => x.Possebilities.Intersect(f.Possebilities).Any() == true).ToList())
                            {
                                foreach (int np in naked_pair.First().Possebilities)
                                {
                                    if (field.Possebilities.Remove(np) == true)
                                    {
                                        //Logbucheintrag
                                        MainPage.Logs.Insert(0, new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Naked_Pair));
                                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                        await Waiter.Waiting();

                                        is_saturadet = false;
                                    }
                                }
                            }
                        }
                    }

                    if (is_saturadet == false)
                    {
                        //Wenn diese Technik noch nicht angewand wurde dann erhöhe den Counter dieser Technik, sonst erstelle die Technik in den Used_Technics
                        if (MainPage.Used_Technics.Any(x => x.Technic == Used_Technic.Technics.Naked_Pair))
                        {
                            MainPage.Used_Technics.FirstOrDefault(x => x.Technic == Used_Technic.Technics.Naked_Pair).Count++;
                        }
                        else
                        {
                            MainPage.Used_Technics.Add(new Used_Technic() { Count = 1, Technic = Used_Technic.Technics.Naked_Pair });
                        }
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Prinzip: Wenn nur in zwei Felder eines gemeinschaftlichen Bereiches zwei gleiche Zahlen stehen die sonst im Bereich dieses Paars nichts
        /// vorkommt, dann werden alle anderen Zahlen in den Felder als Möglichkeit verworfen
        /// </summary>
        public  async Task Hidden_Pair()
        {
            //Die verdeckten Zwillinge die die zwei gleiche Möglichkeiten in der Möglichkeitsliste besitzten 
            List<Field> hidden_pair = new List<Field>();

            //Geht solange die Bereiche durch bis es keine Änderungen mehr gibt
            bool is_saturadet = false;
            while (is_saturadet == false)
            {
                //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                is_saturadet = true;

                //Geht jedes Feld was keine Zahl hat durch
                foreach (Field f in all_fields.Where(x => x.Number == 0))
                {
                    //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    //Wenn es in dem Block nur zwei Feld gibt die zwei Zahlen als gleiche Teilmenge an Möglichkeiten besitzen
                    if (all_fields.Count(x => x.Possebilities.Intersect(f.Possebilities).Count() >= 2 && x.Grid_Number == f.Grid_Number) == 2)
                    {
                        //Ermittelt die verdeckten Zwillinge
                        hidden_pair = all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() >= 2 && x.Grid_Number == f.Grid_Number).ToList();

                        //Erstellt eine Liste an Zahlen die zu der Teilmenge gehören
                        List<int> pairs = new List<int>(hidden_pair[0].Possebilities.Intersect(hidden_pair[1].Possebilities)).ToList();

                        //Wenn es in dem Block kein anderes Feld existiert was eine von den Möglichkeiten besitzt die die Zwillinge definieren
                        if (!all_fields.Where(x => x.Possebilities.Intersect(pairs).Count() >= 1 && x.Grid_Number == f.Grid_Number).Any(x=>hidden_pair.Contains(x) == false))
                        {

                            //Erstellt eine Liste an Zahlen die nicht zu der Teilmenge gehören
                            List<int> others = new List<int>(hidden_pair[0].Possebilities.Except(hidden_pair[1].Possebilities).Union(hidden_pair[1].Possebilities.Except(hidden_pair[0].Possebilities)).ToList());

                            if (others.Count == 0)
                            {
                                foreach (int z in hidden_pair.FirstOrDefault().Possebilities)
                                {
                                    if (all_fields.Where(x => x.Number == 0 && x.Grid_Number == hidden_pair.FirstOrDefault().Grid_Number).Where(x => x.Possebilities.Contains(z) == true && hidden_pair.Contains(x) == false).Any())
                                    {
                                        others.Add(z);
                                    }
                                }
                            }

                            //löscht all anderen Möglichkeiten in den versteckten Zwillingen die nicht zu der gleichen Teilmenge gehören
                            foreach (int np in others)
                            {
                                if (hidden_pair[0].Possebilities.Remove(np) == true)
                                {
                                    //Logbucheintrag
                                    MainPage.Logs.Insert(0, new Logdata() { ID = hidden_pair[0].Id, Grid = hidden_pair[0].Grid_Number, Row = hidden_pair[0].Row_Number, Column = hidden_pair[0].Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Hidden_Pair));
                                    //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                    await Waiter.Waiting();

                                    is_saturadet = false;
                                }
                                if (hidden_pair[1].Possebilities.Remove(np) == true)
                                {
                                    //Logbucheintrag
                                    MainPage.Logs.Insert(0, new Logdata() { ID = hidden_pair[1].Id, Grid = hidden_pair[1].Grid_Number, Row = hidden_pair[1].Row_Number, Column = hidden_pair[1].Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Hidden_Pair));
                                    //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                    await Waiter.Waiting();

                                    is_saturadet = false;
                                }
                            }

                            //löscht alle gleichen Zahlen die in dem Block als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                            if (all_fields.Any(x => x.Number == 0 && x.Grid_Number == f.Grid_Number && hidden_pair.Contains(x) == false))
                            {
                                foreach (Field field in all_fields.Where(x => x.Number == 0 && x.Grid_Number == f.Grid_Number && hidden_pair.Contains(x) == false).Where(x => hidden_pair[0].Possebilities.Intersect(hidden_pair[1].Possebilities).Intersect(x.Possebilities).Any()).ToList())
                                {
                                    foreach (int np in hidden_pair[0].Possebilities)
                                    {
                                        if (field.Possebilities.Remove(np) == true)
                                        {
                                            //Logbucheintrag
                                            MainPage.Logs.Add(new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Hidden_Pair));
                                            //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                            await Waiter.Waiting();

                                            is_saturadet = false;
                                        }
                                    }
                                }
                            }

                            ////löscht alle gleichen Zahlen die in der Reihe als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                            //if (hidden_pair[0].Row_Number == hidden_pair[1].Row_Number)
                            //{
                            //    foreach (Field field in all_fields.Where(x => x.Number == 0).Where(x => x.Row_Number == f.Row_Number && hidden_pair.Contains(x) == false).Where(x => hidden_pair[0].Possebilities.Intersect(hidden_pair[1].Possebilities).Intersect(x.Possebilities).Any()).ToList())
                            //    {
                            //        foreach (int np in hidden_pair[0].Possebilities)
                            //        {
                            //            if (field.Possebilities.Remove(np) == true)
                            //            {
                            //                //Logbucheintrag
                            //                MainPage.Logs.Add(new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Hidden_Pair));
                            //                //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                            //                await Waiter.Waiting();

                            //                is_saturadet = false;
                            //            }
                            //        }
                            //    }
                            //}

                            ////löscht alle gleichen Zahlen die in der Spalte als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                            //if (hidden_pair[0].Column_Number == hidden_pair[1].Column_Number)
                            //{
                            //    foreach (Field field in all_fields.Where(x => x.Number == 0).Where(x => x.Column_Number == f.Column_Number && hidden_pair.Contains(x) == false).Where(x => hidden_pair[0].Possebilities.Intersect(hidden_pair[1].Possebilities).Intersect(x.Possebilities).Any()).ToList())
                            //    {
                            //        foreach (int np in hidden_pair[0].Possebilities)
                            //        {
                            //            if (field.Possebilities.Remove(np) == true)
                            //            {
                            //                //Logbucheintrag
                            //                MainPage.Logs.Add(new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Hidden_Pair));
                            //                //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                            //                await Waiter.Waiting();

                            //                is_saturadet = false;
                            //            }
                            //        }
                            //    }
                            //}
                        }
                    }

                    //Wenn es in der Reihe nur zwei Feld gibt was die gleiche Möglichkeitsliste besitzt
                    if (all_fields.Count(x => x.Possebilities.Intersect(f.Possebilities).Count() >= 2 && x.Row_Number == f.Row_Number) == 2)
                    {
                        //Ermittelt die offenen Zwillinge
                        hidden_pair = all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() >= 2 && x.Row_Number == f.Row_Number).ToList();

                        //Erstellt eine Liste an Zahlen die zu der Teilmenge gehören
                        List<int> pairs = new List<int>(hidden_pair[0].Possebilities.Intersect(hidden_pair[1].Possebilities)).ToList();

                        //Wenn es in der Reihe kein anderes Feld existiert was eine von den Möglichkeiten besitzt die die Zwillinge definieren
                        if (!all_fields.Where(x => x.Possebilities.Intersect(pairs).Count() >= 1 && x.Row_Number == f.Row_Number).Any(x => hidden_pair.Contains(x) == false))
                        {
                            //Erstellt eine Liste an Zahlen die nicht zu der Teilmenge gehören
                            List<int> others = new List<int>(hidden_pair[0].Possebilities.Except(hidden_pair[1].Possebilities).Union(hidden_pair[1].Possebilities.Except(hidden_pair[0].Possebilities)).ToList());

                            if (others.Count == 0)
                            {
                                foreach (int z in hidden_pair.FirstOrDefault().Possebilities)
                                {
                                    if (all_fields.Where(x => x.Number == 0 && x.Row_Number == hidden_pair.FirstOrDefault().Row_Number).Where(x => x.Possebilities.Contains(z) == true && hidden_pair.Contains(x) == false).Any())
                                    {
                                        others.Add(z);
                                    }
                                }
                            }

                            //löscht all anderen Möglichkeiten in den versteckten Zwillingen die nicht zu der gleichen Teilmenge gehören
                            foreach (int np in others)
                            {
                                if (hidden_pair[0].Possebilities.Remove(np) == true)
                                {
                                    //Logbucheintrag
                                    MainPage.Logs.Insert(0, new Logdata() { ID = hidden_pair[0].Id, Grid = hidden_pair[0].Grid_Number, Row = hidden_pair[0].Row_Number, Column = hidden_pair[0].Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Hidden_Pair));
                                    //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                    await Waiter.Waiting();

                                    is_saturadet = false;
                                }
                                if (hidden_pair[1].Possebilities.Remove(np) == true)
                                {
                                    //Logbucheintrag
                                    MainPage.Logs.Insert(0, new Logdata() { ID = hidden_pair[1].Id, Grid = hidden_pair[1].Grid_Number, Row = hidden_pair[1].Row_Number, Column = hidden_pair[1].Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Hidden_Pair));
                                    //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                    await Waiter.Waiting();

                                    is_saturadet = false;
                                }
                            }
                        }
                    }

                    //Wenn es in der Spalte nur zwei Feld gibt was die gleiche Möglichkeitsliste besitzt
                    if (all_fields.Count(x => x.Possebilities.Intersect(f.Possebilities).Count() >= 2 && x.Column_Number == f.Column_Number) == 2)
                    {
                        //Ermittelt die offenen Zwillinge
                        hidden_pair = all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() >= 2 && x.Column_Number == f.Column_Number).ToList();

                        //Erstellt eine Liste an Zahlen die zu der Teilmenge gehören
                        List<int> pairs = new List<int>(hidden_pair[0].Possebilities.Intersect(hidden_pair[1].Possebilities)).ToList();

                        //Wenn es in der Reihe kein anderes Feld existiert was eine von den Möglichkeiten besitzt die die Zwillinge definieren
                        if (!all_fields.Where(x => x.Possebilities.Intersect(pairs).Count() >= 1 && x.Column_Number == f.Column_Number).Any(x => hidden_pair.Contains(x) == false))
                        {
                            //Erstellt eine Liste an Zahlen die nicht zu der Teilmenge gehören
                            List<int> others = new List<int>(hidden_pair[0].Possebilities.Except(hidden_pair[1].Possebilities).Union(hidden_pair[1].Possebilities.Except(hidden_pair[0].Possebilities)).ToList());

                            if (others.Count == 0)
                            {
                                foreach (int z in hidden_pair.FirstOrDefault().Possebilities)
                                {
                                    if (all_fields.Where(x => x.Number == 0 && x.Column_Number == hidden_pair.FirstOrDefault().Column_Number).Where(x => x.Possebilities.Contains(z) == true && hidden_pair.Contains(x) == false).Any())
                                    {
                                        others.Add(z);
                                    }
                                }
                            }

                            //löscht all anderen Möglichkeiten in den versteckten Zwillingen die nicht zu der gleichen Teilmenge gehören
                            foreach (int np in others)
                            {
                                if (hidden_pair[0].Possebilities.Remove(np) == true)
                                {
                                    //Logbucheintrag
                                    MainPage.Logs.Insert(0, new Logdata() { ID = hidden_pair[0].Id, Grid = hidden_pair[0].Grid_Number, Row = hidden_pair[0].Row_Number, Column = hidden_pair[0].Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Hidden_Pair));
                                    //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                    await Waiter.Waiting();

                                    is_saturadet = false;
                                }
                                if (hidden_pair[1].Possebilities.Remove(np) == true)
                                {
                                    //Logbucheintrag
                                    MainPage.Logs.Insert(0, new Logdata() { ID = hidden_pair[1].Id, Grid = hidden_pair[1].Grid_Number, Row = hidden_pair[1].Row_Number, Column = hidden_pair[1].Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Hidden_Pair));
                                    //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                    await Waiter.Waiting();

                                    is_saturadet = false;
                                }
                            }
                        }
                    }

                    if (is_saturadet == false)
                    {
                        //Wenn diese Technik noch nicht angewand wurde dann erhöhe den Counter dieser Technik, sonst erstelle die Technik in den Used_Technics
                        if (MainPage.Used_Technics.Any(x => x.Technic == Used_Technic.Technics.Hidden_Pair))
                        {
                            MainPage.Used_Technics.FirstOrDefault(x => x.Technic == Used_Technic.Technics.Hidden_Pair).Count++;
                        }
                        else
                        {
                            MainPage.Used_Technics.Add(new Used_Technic() { Count = 1, Technic = Used_Technic.Technics.Hidden_Pair });
                        }
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Prinzip: Wenn in drei Felder eines gemeinschaftlichen Bereiches eine Teilmenge gleicher Zahlen alleine (max 3) stehen, dann werden alle
        /// anderen gleichen Zahlen im Einflußbereich als Möglichkeit verworfen
        /// </summary>
        public  async Task Naked_Trible()
        {
            //Die offenen Drillinge die die gleiche Möglichkeitsliste besitzten 
            List<Field> naked_trible = new List<Field>();

            //Geht solange die Bereiche durch bis es keine Änderungen mehr gibt
            bool is_saturadet = false;
            while (is_saturadet == false)
            {
                //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                is_saturadet = true;

                //Geht jedes Feld was keine Zahl hat durch
                foreach (Field f in all_fields.Where(x => x.Number == 0))
                {
                    //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    //Wenn die Möglichkeiten dieses Feldes nicht 3 sind dann überspringe
                    if (f.Possebilities.Count != 3)
                        continue;

                    //Wenn es in dem Block nur drei Feld gibt die zwischen zwei und drei Zahlen als gleiche Teilmenge an Möglichkeiten besitzen
                    if (all_fields.Where(x => (x.Possebilities.Intersect(f.Possebilities).Count() == 2 && x.Possebilities.Count() == 2) || (x.Possebilities.Intersect(f.Possebilities).Count() == 3 && x.Possebilities.Count == 3)).Count(x => x.Grid_Number == f.Grid_Number) == 3)
                    {
                        //Ermittelt die offenen Drillinge
                        naked_trible = all_fields.Where(x => (x.Possebilities.Intersect(f.Possebilities).Count() == 2 && x.Possebilities.Count() == 2) || (x.Possebilities.Intersect(f.Possebilities).Count() == 3 && x.Possebilities.Count == 3)).Where(x => x.Grid_Number == f.Grid_Number).ToList();

                        //löscht alle gleichen Zahlen die in dem Block als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        foreach (Field field in all_fields.Where(x => x.Number == 0 && x.Grid_Number == f.Grid_Number && naked_trible.Contains(x) == false && f.Possebilities.Intersect(x.Possebilities).Any()).ToList())
                        {
                            foreach (int np in f.Possebilities.Intersect(field.Possebilities))
                            {
                                if (field.Possebilities.Remove(np) == true)
                                {
                                    //Logbucheintrag
                                    MainPage.Logs.Insert(0, new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Naked_Trible));
                                    //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                    await Waiter.Waiting();

                                    is_saturadet = false;
                                }
                            }
                        }

                        ////löscht alle gleichen Zahlen die in der Reihe als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        //if (naked_trible[0].Row_Number == naked_trible[1].Row_Number && naked_trible[1].Row_Number == naked_trible[2].Row_Number)
                        //{
                        //    foreach (Field field in all_fields.Where(x => x.Number != 0 && x.Row_Number == f.Row_Number && naked_trible.Contains(x) == false && f.Possebilities.Intersect(x.Possebilities).Any()).ToList())
                        //    {
                        //        foreach (int np in f.Possebilities.Intersect(field.Possebilities))
                        //        {
                        //            if (field.Possebilities.Remove(np) == true)
                        //            {
                        //                //Logbucheintrag
                        //                MainPage.Logs.Insert(0, new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Naked_Trible));
                        //                //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                        //                await Waiter.Waiting();

                        //                is_saturadet = false;
                        //            }
                        //        }
                        //    }
                        //}

                        ////löscht alle gleichen Zahlen die in der Spalte als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        //if (naked_trible[0].Column_Number == naked_trible[1].Column_Number && naked_trible[1].Column_Number == naked_trible[2].Column_Number)
                        //{
                        //    foreach (Field field in all_fields.Where(x => x.Number != 0 && x.Column_Number == f.Column_Number && naked_trible.Contains(x) == false && f.Possebilities.Intersect(x.Possebilities).Any()).ToList())
                        //    {
                        //        foreach (int np in f.Possebilities.Intersect(field.Possebilities))
                        //        {
                        //            if (field.Possebilities.Remove(np) == true)
                        //            {
                        //                //Logbucheintrag
                        //                MainPage.Logs.Insert(0, new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Naked_Trible));
                        //                //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                        //                await Waiter.Waiting();

                        //                is_saturadet = false;
                        //            }
                        //        }
                        //    }
                        //}
                    }

                    //Wenn es in der Reihe nur drei Feld gibt die zwischen zwei und drei Zahlen als gleiche Teilmenge an Möglichkeiten besitzen
                    if (all_fields.Where(x => (x.Possebilities.Intersect(f.Possebilities).Count() == 2 && x.Possebilities.Count() == 2) || (x.Possebilities.Intersect(f.Possebilities).Count() == 3 && x.Possebilities.Count == 3)).Count(x => x.Row_Number == f.Row_Number) == 3)
                    {
                        //Ermittelt die offenen Drillinge
                        naked_trible = all_fields.Where(x => (x.Possebilities.Intersect(f.Possebilities).Count() == 2 && x.Possebilities.Count() == 2) || (x.Possebilities.Intersect(f.Possebilities).Count() == 3 && x.Possebilities.Count == 3)).Where(x => x.Row_Number == f.Row_Number).ToList();

                        //löscht alle gleichen Zahlen die in der Reihe als Möglichkeit stehen und fügt sie zu der Verweigerungsliste hinzu
                        foreach (Field field in all_fields.Where(x => x.Number == 0 && x.Row_Number == f.Row_Number && naked_trible.Contains(x) == false && f.Possebilities.Intersect(x.Possebilities).Any()).ToList())
                        {
                            foreach (int np in f.Possebilities.Intersect(field.Possebilities))
                            {
                                if (field.Possebilities.Remove(np) == true)
                                {
                                    //Logbucheintrag
                                    MainPage.Logs.Insert(0, new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Naked_Trible));
                                    //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                    await Waiter.Waiting();

                                    is_saturadet = false;
                                }
                            }
                        }
                    }

                    //Wenn es in der Spalte nur drei Feld gibt die zwischen zwei und drei Zahlen als gleiche Teilmenge an Möglichkeitsliste besitzt
                    if (all_fields.Where(x => (x.Possebilities.Intersect(f.Possebilities).Count() == 2 && x.Possebilities.Count() == 2) || (x.Possebilities.Intersect(f.Possebilities).Count() == 3 && x.Possebilities.Count == 3)).Count(x => x.Column_Number == f.Column_Number) == 3)
                    {
                        //Ermittelt die offenen Drillinge
                        naked_trible = all_fields.Where(x => (x.Possebilities.Intersect(f.Possebilities).Count() == 2 && x.Possebilities.Count() == 2) || (x.Possebilities.Intersect(f.Possebilities).Count() == 3 && x.Possebilities.Count == 3)).Where(x => x.Column_Number == f.Column_Number).ToList();

                        //löscht alle gleichen Zahlen die in der Spalte als Möglichkeit stehen
                        foreach (Field field in all_fields.Where(x => x.Number == 0 && x.Column_Number == f.Column_Number && naked_trible.Contains(x) == false && f.Possebilities.Intersect(x.Possebilities).Any()).ToList())
                        {
                            foreach (int np in f.Possebilities.Intersect(field.Possebilities))
                            {
                                if (field.Possebilities.Remove(np) == true)
                                {
                                    //Logbucheintrag
                                    MainPage.Logs.Insert(0, new Logdata() { ID = field.Id, Grid = field.Grid_Number, Row = field.Row_Number, Column = field.Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Naked_Trible));
                                    //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                    await Waiter.Waiting();

                                    is_saturadet = false;
                                }
                            }
                        }
                    }

                    if (is_saturadet == false)
                    {
                        //Wenn diese Technik noch nicht angewand wurde dann erhöhe den Counter dieser Technik, sonst erstelle die Technik in den Used_Technics
                        if (MainPage.Used_Technics.Any(x => x.Technic == Used_Technic.Technics.Naked_Trible))
                        {
                            MainPage.Used_Technics.FirstOrDefault(x => x.Technic == Used_Technic.Technics.Naked_Trible).Count++;
                        }
                        else
                        {
                            MainPage.Used_Technics.Add(new Used_Technic() { Count = 1, Technic = Used_Technic.Technics.Naked_Trible });
                        }
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Prinzip: Wenn in drei Felder eines gemeinschaftlichen Bereiches eine Teilmenge gleicher Zahlen (max 3) stehen, dann werden alle anderen
        /// gleichen Zahlen im Einflußbereich als Möglichkeit verworfen
        /// </summary>
        public  async Task Hidden_Trible()
        {
            //Die verdeckten Drillinge die die gleiche Möglichkeitsliste besitzten 
            List<Field> hidden_trible = new List<Field>();

            //Geht solange die Bereiche durch bis es keine Änderungen mehr gibt
            bool is_saturadet = false;
            while (is_saturadet == false)
            {
                //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                is_saturadet = true;

                //Geht jedes Feld was keine Zahl hat durch
                foreach (Field f in all_fields.Where(x => x.Number == 0))
                {
                    //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    //Wenn die Möglichkeiten dieses Feldes kleiner 3 sind dann überspringe
                    if (f.Possebilities.Count < 3)
                        continue;

                    //Wenn es in dem Block nur drei Feld gibt die zwischen zwei und drei Zahlen als gleiche Teilmenge an Möglichkeiten besitzen
                    if (all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() >= 2 ).Count(x=> x.Grid_Number == f.Grid_Number) == 3)
                    {
                        //Ermittelt die verdeckten Drillinge
                        hidden_trible = all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() >= 2).Where(x=>x.Grid_Number == f.Grid_Number).ToList();

                        //Eine Liste an Möglichkeiten die zu den verdeckten Drillingen gehören 
                        List<int> trible_numbers = new List<int>();
                        foreach(Field ht in hidden_trible)
                        {
                            //Für alle Möglichkeiten die es in dem Block in kein anderen Feld existiert außer in den Drilling
                            foreach (int tripble in ht.Possebilities.Where(y => all_fields.Where(x => x.Number == 0 && x.Grid_Number == f.Grid_Number).Where(x=>x.Possebilities.Contains(y) == true).Any(x => hidden_trible.Contains(x) == false) == false).ToList())
                            {
                                if (trible_numbers.Contains(tripble) == false)
                                {
                                    trible_numbers.Add(tripble);
                                }
                            }
                        }

                        //Wenn die Drillinge drei Zahlen besitzten
                        if (trible_numbers.Count == 3)
                        {
                            //löscht all anderen Möglichkeiten in den versteckten Drillinge die nicht zu der gleichen Teilmenge gehören
                            foreach (Field ht in hidden_trible)
                            {
                                List<int> tribles = ht.Possebilities.Where(x => trible_numbers.Contains(x) == false).ToList();
                                foreach (int np in tribles)
                                {
                                    if (ht.Possebilities.Remove(np) == true)
                                    {
                                        //Logbucheintrag
                                        MainPage.Logs.Insert(0, new Logdata() { ID = ht.Id, Grid = ht.Grid_Number, Row = ht.Row_Number, Column = ht.Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Hidden_Trible));
                                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                        await Waiter.Waiting();

                                        is_saturadet = false;
                                    }
                                }
                            }
                        }
                    }

                    //Wenn es in der Reihe nur drei Feld gibt die zwischen zwei und drei Zahlen als gleiche Teilmenge an Möglichkeiten besitzen
                    if (all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() >= 2 ).Count(x => x.Row_Number == f.Row_Number) == 3)
                    {
                        //Ermittelt die verdeckten Drillinge
                        hidden_trible = all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() >= 2).Where(x => x.Row_Number == f.Row_Number).ToList();

                        //Eine Liste an Möglichkeiten die zu den verdeckten Drillingen gehören 
                        List<int> trible_numbers = new List<int>();
                        foreach (Field ht in hidden_trible)
                        {
                            //Für alle Möglichkeiten die es in der Reihe in kein anderen Feld existiert außer in den Drilling
                            foreach (int tripble in ht.Possebilities.Where(y => all_fields.Where(x => x.Number == 0 && x.Row_Number == f.Row_Number).Where(x => x.Possebilities.Contains(y) == true).Any(x => hidden_trible.Contains(x) == false) == false).ToList())
                            {
                                if (trible_numbers.Contains(tripble) == false)
                                {
                                    trible_numbers.Add(tripble);
                                }
                            }
                        }

                        //Wenn die Drillinge drei Zahlen besitzten
                        if (trible_numbers.Count == 3)
                        {
                            //löscht all anderen Möglichkeiten in den versteckten Drillinge die nicht zu der gleichen Teilmenge gehören
                            foreach (Field ht in hidden_trible)
                            {
                                List<int> tribles = ht.Possebilities.Where(x => trible_numbers.Contains(x) == false).ToList();
                                foreach (int np in tribles)
                                {
                                    if (ht.Possebilities.Remove(np) == true)
                                    {
                                        //Logbucheintrag
                                        MainPage.Logs.Insert(0, new Logdata() { ID = ht.Id, Grid = ht.Grid_Number, Row = ht.Row_Number, Column = ht.Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Hidden_Trible));
                                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                        await Waiter.Waiting();

                                        is_saturadet = false;
                                    }
                                }
                            }
                        }
                    }

                    //Wenn es in der Spalte nur drei Feld gibt die zwischen zwei und drei Zahlen als gleiche Teilmenge an Möglichkeitsliste besitzt
                    if(all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() >= 2 ).Count(x => x.Column_Number == f.Column_Number) == 3)
                    {
                        //Ermittelt die verdeckten Drillinge
                        hidden_trible = all_fields.Where(x => x.Possebilities.Intersect(f.Possebilities).Count() >= 2).Where(x => x.Column_Number == f.Column_Number).ToList();

                        //Eine Liste an Möglichkeiten die zu den verdeckten Drillingen gehören 
                        List<int> trible_numbers = new List<int>();
                        foreach (Field ht in hidden_trible)
                        {
                            //Für alle Möglichkeiten die es in der Spalte in kein anderen Feld existiert außer in den Drilling
                            foreach (int tripble in ht.Possebilities.Where(y => all_fields.Where(x => x.Number == 0 && x.Column_Number == f.Column_Number).Where(x => x.Possebilities.Contains(y) == true).Any(x => hidden_trible.Contains(x) == false) == false).ToList())
                            {
                                if (trible_numbers.Contains(tripble) == false)
                                {
                                    trible_numbers.Add(tripble);
                                }
                            }
                        }

                        //Wenn die Drillinge drei Zahlen besitzten
                        if (trible_numbers.Count == 3)
                        {
                            //löscht all anderen Möglichkeiten in den versteckten Drillinge die nicht zu der gleichen Teilmenge gehören
                            foreach (Field ht in hidden_trible)
                            {
                                List<int> tribles = ht.Possebilities.Where(x => trible_numbers.Contains(x) == false).ToList();
                                foreach (int np in tribles)
                                {
                                    if (ht.Possebilities.Remove(np) == true)
                                    {
                                        //Logbucheintrag
                                        MainPage.Logs.Insert(0, new Logdata() { ID = ht.Id, Grid = ht.Grid_Number, Row = ht.Row_Number, Column = ht.Column_Number }.Removed_Market_Number_From_Field(np, all_fields, Logdata.Strategies.Hidden_Trible));
                                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                        await Waiter.Waiting();

                                        is_saturadet = false;
                                    }
                                }
                            }
                        }
                    }

                    if (is_saturadet == false)
                    {
                        //Wenn diese Technik noch nicht angewand wurde dann erhöhe den Counter dieser Technik, sonst erstelle die Technik in den Used_Technics
                        if (MainPage.Used_Technics.Any(x => x.Technic == Used_Technic.Technics.Hidden_Trible))
                        {
                            MainPage.Used_Technics.FirstOrDefault(x => x.Technic == Used_Technic.Technics.Hidden_Trible).Count++;
                        }
                        else
                        {
                            MainPage.Used_Technics.Add(new Used_Technic() { Count = 1, Technic = Used_Technic.Technics.Hidden_Trible });
                        }
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Wenn innerhalb eines Blocks alle Kandidaten einer bestimmten Ziffer auf eine Zeile oder eine Spalte beschränkt sind, kann diese Ziffer in
        /// dieser Zeile oder Spalte außerhalb des Blocks nicht mehr auftreten.
        /// </summary>
        public  async Task Locked_Candidates_Typ_1()
        {
            //Geht solange die Bereiche durch bis es keine Änderungen mehr gibt
            bool is_saturadet = false;


            //Geht jeden Block durch wo noch nicht alles gefüllt ist
            foreach (ObservableCollection<Field> lf in MainPage.Fields.Where(x=> x.Any(y => y.Number == 0)))
            {
                //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                //Geht jede Zahl von den Möglichkeiten durch
                for (int i = 1; i<10;i++)
                {

                    is_saturadet = true;

                    //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    //Die Felder wo die Möglichkeiten gelöscht werden sollen
                    List<Field> targets = new List<Field>();

                    //Wenn es in dem Block mehr als 2 Felder gibt die diese Möglichkeit besitzen
                    if (lf.Count(x => x.Possebilities.Contains(i)) >=2)
                    {
                        //Wenn diese Felder alle in derselben Spalte liegen
                        if(!lf.Where(x => x.Possebilities.Contains(i)).Any(y=>y.Column_Number != lf.First(x => x.Possebilities.Contains(i)).Column_Number))
                        {
                            //Der erste Kandidat der diese Möglichkeit besitzt in dem Block
                            Field locked_candidate = lf.First(x => x.Possebilities.Contains(i));

                            //Die Felder wo die Möglichkeiten gelöscht werden sollen und sie in derselben Spalte liegen aber außerhalt dieses Blockes
                            targets = all_fields.Where(x => x.Column_Number == locked_candidate.Column_Number && x.Possebilities.Contains(i)).Where(x=>x.Grid_Number != locked_candidate.Grid_Number).ToList();
                        }

                        //Wenn diese Felder alle in derselben Reihe liegen
                        if (!lf.Where(x => x.Possebilities.Contains(i)).Any(y => y.Row_Number != lf.First(x => x.Possebilities.Contains(i)).Row_Number))
                        {
                            //Der erste Kandidat der diese Möglichkeit besitzt in dem Block
                            Field locked_candidate = lf.First(x => x.Possebilities.Contains(i));

                            //Die Felder wo die Möglichkeiten gelöscht werden sollen und sie in derselben Spalte liegen aber außerhalt dieses Blockes
                            targets = all_fields.Where(x => x.Row_Number == locked_candidate.Row_Number && x.Possebilities.Contains(i)).Where(x => x.Grid_Number != locked_candidate.Grid_Number).ToList();
                        }

                        //Geht die Felder wo die Möglichkeiten gelöscht werden sollen durch
                        foreach (Field f in targets)
                        {
                            if (f.Possebilities.Remove(i) == true)
                            {
                                //Logbucheintrag
                                MainPage.Logs.Insert(0, new Logdata() { ID = f.Id, Grid = f.Grid_Number, Row = f.Row_Number, Column = f.Column_Number }.Removed_Market_Number_From_Field(i, all_fields, Logdata.Strategies.Locked_Candidates_Typ1));
                                //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                await Waiter.Waiting();

                                is_saturadet = false;
                            }
                        }

                        if (is_saturadet == false)
                        {
                            //Wenn diese Technik noch nicht angewand wurde dann erhöhe den Counter dieser Technik, sonst erstelle die Technik in den Used_Technics
                            if (MainPage.Used_Technics.Any(x => x.Technic == Used_Technic.Technics.Locked_Candidates_Typ1))
                            {
                                MainPage.Used_Technics.FirstOrDefault(x => x.Technic == Used_Technic.Technics.Locked_Candidates_Typ1).Count++;
                            }
                            else
                            {
                                MainPage.Used_Technics.Add(new Used_Technic() { Count = 1, Technic = Used_Technic.Technics.Locked_Candidates_Typ1 });
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Wenn in einer Zeile (oder eine Spalte) alle Kandidaten einer Ziffer auf einen Block beschränkt sind, kann man diesen Kandidaten von allen 
        /// anderen Zellen des Blocks eliminieren.
        /// </summary>
        public  async Task Locked_Candidates_Typ_2()
        {
            //Geht solange die Bereiche durch bis es keine Änderungen mehr gibt
            bool is_saturadet = false;

            //Geht jeden Block durch wo noch nicht alles gefüllt ist
            foreach (ObservableCollection<Field> lf in MainPage.Fields.Where(x => x.Any(y => y.Number == 0)))
            {
                //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                List<List<Field>> rows = new List<List<Field>>();

                List<List<Field>> columns = new List<List<Field>>();

                foreach(Field f in lf)
                {
                    if (rows.Any(x => x.FirstOrDefault().Row_Number == f.Row_Number) == false)
                        rows.Add(new List<Field>() { f });
                    else
                        rows.FirstOrDefault(x => x.FirstOrDefault().Row_Number == f.Row_Number).Add(f);

                    if (columns.Any(x => x.FirstOrDefault().Column_Number == f.Column_Number) == false)
                        columns.Add(new List<Field>() { f });
                    else
                        columns.FirstOrDefault(x => x.FirstOrDefault().Column_Number == f.Column_Number).Add(f);
                }

                //Geht jede Zahl von den Möglichkeiten durch
                for (int i = 1; i < 10; i++)
                {

                    is_saturadet = true;

                    //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    //Die Felder wo die Möglichkeiten gelöscht werden sollen
                    List<Field> targets = new List<Field>();

                    //Geht jede Reihe in diesem Block durch wo diese Möglichkeit auftritt 
                    foreach(List<Field> row in rows.Where(x=>x.Any(x=>x.Possebilities.Contains(i))))
                    { 
                        //Wenn diese Möglichkeit nur auf diesen Block in dieser Reihe beschränkt ist
                        if (all_fields.Where(x=>x.Number == 0 && x.Row_Number == row.FirstOrDefault().Row_Number).Any(x=>x.Possebilities.Contains(i) && row.Contains(x) == false) == false)
                        {
                            //Die Felder wo die Möglichkeiten gelöscht werden sollen und sie in derselben Spalte liegen aber außerhalt dieses Blockes
                            foreach(List<Field> rrow in rows.Where(x=>x!= row))
                            {
                                targets.AddRange(rrow.Where(x=>x.Possebilities.Contains(i)));
                            }
                            break;
                        }
                    }
                    
                    //Geht jede Spalte in diesem Block durch wo diese Möglichkeit auftritt 
                    foreach (List<Field> col in columns.Where(x => x.Any(x => x.Possebilities.Contains(i))))
                    {
                        //Wenn diese Möglichkeit nur auf diesen Block in dieser Reihe beschränkt ist
                        if (all_fields.Where(x => x.Number == 0 && x.Column_Number == col.FirstOrDefault().Column_Number).Any(x => x.Possebilities.Contains(i) && col.Contains(x) == false) == false)
                        {
                            //Die Felder wo die Möglichkeiten gelöscht werden sollen und sie in derselben Spalte liegen aber außerhalt dieses Blockes
                            foreach (List<Field> ccol in columns.Where(x => x != col))
                            {
                                targets.AddRange(ccol.Where(x => x.Possebilities.Contains(i)));
                            }
                            break;
                        }
                    }

                    //Geht die Felder wo die Möglichkeiten gelöscht werden sollen durch
                    foreach (Field f in targets)
                    {
                        if (f.Possebilities.Remove(i) == true)
                        {
                            //Logbucheintrag
                            MainPage.Logs.Insert(0, new Logdata() { ID = f.Id, Grid = f.Grid_Number, Row = f.Row_Number, Column = f.Column_Number }.Removed_Market_Number_From_Field(i, all_fields, Logdata.Strategies.Locked_Candidates_Typ2));
                            //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                            await Waiter.Waiting();

                            is_saturadet = false;
                        }
                    }

                    if (is_saturadet == false)
                    {
                        //Wenn diese Technik noch nicht angewand wurde dann erhöhe den Counter dieser Technik, sonst erstelle die Technik in den Used_Technics
                        if (MainPage.Used_Technics.Any(x => x.Technic == Used_Technic.Technics.Locked_Candidates_Typ2))
                        {
                            MainPage.Used_Technics.FirstOrDefault(x => x.Technic == Used_Technic.Technics.Locked_Candidates_Typ2).Count++;
                        }
                        else
                        {
                            MainPage.Used_Technics.Add(new Used_Technic() { Count = 1, Technic = Used_Technic.Technics.Locked_Candidates_Typ2 });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Wenn zwei Felder, in einer Reihe oder Spalte, eine Möglichkeit besitzen die nur in diesen Feldern in der jeweiligen Reihe oder Spalte
        /// auftraucht und außerdem ein weiterer Bereich existiert wo die Felder parallel liegen, dann werden in diesen orthogonalen Bereichen 
        /// diese Möglichkeiten in den Feldern elemeniert die nicht zu dem X-Wing gehören.
        /// </summary>
        public async Task X_Wing()
        {
            //Die Felder die den X-Wing bilden 
            List<Field> x_wing = new List<Field>();

            //Der Indikator in welcher richtung der X-Wing ausgeht
            string direktion = string.Empty;

            //Geht solange die Bereiche durch bis es keine Änderungen mehr gibt
            bool is_saturadet = true;

            //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            //Geht jede Möglichkeit durch
            for (int i = 1; i < 10; i++)
            {
                //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                //Erstellt eine Liste an Reihen wo es diese Möglichkeit nur zweimal gibt
                List<List<Field>> rows = new List<List<Field>>();
                //Erstellt eine Liste an Spalten wo es diese Möglichkeit nur zweimal gibt
                List<List<Field>> columns = new List<List<Field>>();

                //Geht jedes Feld durch das noch offen ist und diese Möglichkeit hat
                foreach (Field f in all_fields.Where(x => x.Number == 0 && x.Possebilities.Contains(i)))
                {
                    //Wenn in der Reihe nur zwei Felder existieren die diese Möglichkeit besitzen
                    if(all_fields.Count(x=> x.Possebilities.Contains(i) && x.Row_Number == f.Row_Number)==2 && all_fields.Where(x => x.Possebilities.Contains(i) && x.Row_Number == f.Row_Number).ToList()[0].Grid_Number != all_fields.Where(x => x.Possebilities.Contains(i) && x.Row_Number == f.Row_Number).ToList()[1].Grid_Number)
                    {
                        //Wenn diese Reihe noch nicht in der Liste existiert dann füge sie hinzu
                        if(!rows.Any(x=>x.Any(x=>x.Row_Number == f.Row_Number)))
                            rows.Add(all_fields.Where(x => x.Possebilities.Contains(i) && x.Row_Number == f.Row_Number).ToList());
                    }

                    //Wenn in der Spalte nur zwei Felder existieren die diese Möglichkeit besitzen
                    if (all_fields.Count(x=> x.Possebilities.Contains(i) && x.Column_Number == f.Column_Number) == 2 && all_fields.Where(x => x.Possebilities.Contains(i) && x.Column_Number == f.Column_Number).ToList()[0].Grid_Number != all_fields.Where(x => x.Possebilities.Contains(i) && x.Column_Number == f.Column_Number).ToList()[1].Grid_Number)
                    {
                        //Wenn diese Spalte noch nicht in der Liste existiert dann füge sie hinzu
                        if (!columns.Any(x => x.Any(x => x.Row_Number == f.Row_Number)))
                            columns.Add(all_fields.Where(x => x.Possebilities.Contains(i) && x.Column_Number == f.Column_Number).ToList());
                    }
                }

                //Geht jede Reihe durch und schaut ob es ein X-Wing existiert
                foreach(List<Field> row in rows)
                {
                    if (rows.Count(x => x.Select(x => x.Column_Number).ToList().Intersect(row.Select(x => x.Column_Number).ToList()).Count() == 2) == 2 && x_wing.Intersect(rows.Where(x => x.Select(x => x.Column_Number).ToList().Intersect(row.Select(x => x.Column_Number).ToList()).Count() == 2).SelectMany(x => x)).Count() == 0)
                    {
                        //Ein Platzhalter um zu schauen ob dieser X-Wing etwas elemenieren kann oder nicht
                        List<Field> placholder = rows.Where(x => x.Select(x => x.Column_Number).ToList().Intersect(row.Select(x => x.Column_Number).ToList()).Count() == 2).SelectMany(x => x).ToList();

                        //Geht jeden Kandidaten in der ersten Reihe von dem Platzhalter durch
                        foreach(Field f in rows.Where(x => x.Select(x => x.Column_Number).ToList().Intersect(row.Select(x => x.Column_Number).ToList()).Count() == 2).ToList().FirstOrDefault())
                        {
                            //Wenn durch diesen X-Wing eine Möglichkeit elemeniert werden kann dann füge den X-Wing hinzu und unterbreche die Suche
                            if(all_fields.Where(x=>x.Number == 0 && x.Column_Number == f.Column_Number).Any(x=>placholder.Contains(x) == false && x.Possebilities.Contains(i)))
                            {
                                x_wing.AddRange(rows.Where(x => x.Select(x => x.Column_Number).ToList().Intersect(row.Select(x => x.Column_Number).ToList()).Count() == 2).SelectMany(x => x));
                                direktion = "Row";
                                goto Catch_X_Wing;
                            }
                        }
                    }
                }

                //Geht jede Spalte durch und schaut ob es ein X-Wing existiert
                foreach (List<Field> column in columns)
                {
                    if (columns.Count(x => x.Select(x => x.Row_Number).ToList().Intersect(column.Select(x => x.Row_Number).ToList()).Count() == 2) == 2 && x_wing.Intersect(columns.Where(x => x.Select(x => x.Row_Number).ToList().Intersect(column.Select(x => x.Row_Number).ToList()).Count() == 2).SelectMany(x => x)).Count() == 0)
                    {
                        //Ein Platzhalter um zu schauen ob dieser X-Wing etwas elemenieren kann oder nicht
                        List<Field> placholder = rows.Where(x => x.Select(x => x.Row_Number).ToList().Intersect(column.Select(x => x.Row_Number).ToList()).Count() == 2).SelectMany(x => x).ToList();

                        //Geht jeden Kandidaten in der ersten Reihe von dem Platzhalter durch
                        foreach (Field f in rows.Where(x => x.Select(x => x.Row_Number).ToList().Intersect(column.Select(x => x.Row_Number).ToList()).Count() == 2).ToList().FirstOrDefault())
                        {
                            //Wenn durch diesen X-Wing eine Möglichkeit elemeniert werden kann dann füge den X-Wing hinzu und unterbreche die Suche
                            if (all_fields.Where(x => x.Number == 0 && x.Row_Number == f.Row_Number).Any(x => placholder.Contains(x) == false && x.Possebilities.Contains(i)))
                            {
                                x_wing.AddRange(columns.Where(x => x.Select(x => x.Row_Number).ToList().Intersect(column.Select(x => x.Row_Number).ToList()).Count() == 2).SelectMany(x => x));
                                direktion = "Column";
                                goto Catch_X_Wing;
                            }
                        }
                    }

                }


                Catch_X_Wing:
                //Geht jedes Kandidat im X-Wing duch
                foreach (Field f in x_wing)
                {
                    //Wenn der X-Wing Bereich in der Reihe ist
                    if(direktion == "Row")
                    {
                        //Geht jedes Feld was orthogonal zu dem X-Wing berech ist, diese Möglichkeit hat, noch offen ist und nicht im X-Wing ist durch
                        foreach (Field gf in all_fields.Where(x => x.Number == 0 && x.Column_Number == f.Column_Number).Where(x => x_wing.Contains(x) == false && x.Possebilities.Contains(i)).ToList())
                        {
                            if (gf.Possebilities.Remove(i) == true)
                            {
                                //Logbucheintrag
                                MainPage.Logs.Insert(0, new Logdata() { ID = gf.Id, Grid = gf.Grid_Number, Row = gf.Row_Number, Column = gf.Column_Number }.Removed_Market_Number_From_Field(i, all_fields, Logdata.Strategies.X_Wing));
                                //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                await Waiter.Waiting();
                                is_saturadet = false;
                            }
                        }
                    }

                    //Wenn der X-Wing Bereich in der Spalte ist
                    if(direktion == "Column")
                    {
                        //Geht jedes Feld was orthogonal zu dem X-Wing berech ist, diese Möglichkeit hat, noch offen ist und nicht im X-Wing ist durch
                        foreach (Field gf in all_fields.Where(x => x.Number == 0 && x.Row_Number == f.Row_Number).Where(x => x_wing.Contains(x) == false && x.Possebilities.Contains(i)).ToList())
                        {
                            if (gf.Possebilities.Remove(i) == true)
                            {
                                //Logbucheintrag
                                MainPage.Logs.Insert(0, new Logdata() { ID = gf.Id, Grid = gf.Grid_Number, Row = gf.Row_Number, Column = gf.Column_Number }.Removed_Market_Number_From_Field(i, all_fields, Logdata.Strategies.X_Wing));
                                //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                                await Waiter.Waiting();
                                is_saturadet = false;
                            }
                        }
                    }
                }

                //Wenn ein X-Wing gefunden wurde dann unterbreche die Suche
                if (is_saturadet == false)
                {
                    //Wenn diese Technik noch nicht angewand wurde dann erhöhe den Counter dieser Technik, sonst erstelle die Technik in den Used_Technics
                    if (MainPage.Used_Technics.Any(x => x.Technic == Used_Technic.Technics.X_Wing))
                    {
                        MainPage.Used_Technics.FirstOrDefault(x => x.Technic == Used_Technic.Technics.X_Wing).Count++;
                    }
                    else
                    {
                        MainPage.Used_Technics.Add(new Used_Technic() { Count = 1, Technic = Used_Technic.Technics.X_Wing });
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// Wenn in einem Feld(X) nur zwei Möglichkeiten bestehen(A und B) und es zusätzlich im gleichen Block/Spalte eine weiteres Feld(Y) existiert
        /// mit nur zwei Möglichkeiten (A und C) und im gleichen Block/Reihe ein weiteres Feld(Z) existiert mit nur zwei Möglichkeiten (B und C) dann 
        /// kann in den Bereichen von den Feldern (Y und Z) die Möglichkeit (C) entfernt werden
        /// </summary>
        public async Task Y_Wing()
        {
            //Die Angelpunkte dieser Methode 
            Field X = new Field();
            Field Y = new Field();
            Field Z = new Field();

            //Die Schlüsselmöglichkeiten vom Feld(X)
            List<int> AB = new List<int>();

            //Die zulöschente Möglichkeit
            int C = 0;

            //Geht solange die Bereiche durch bis es keine Änderungen mehr gibt
            bool is_saturadet = true;

            //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            //Geht jedes Feld durch das noch offen und nur zwei Möglichkeiten besitzt
            foreach (Field f in all_fields.Where(x => x.Number == 0 && x.Possebilities.Count == 2))
            {
                //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                //Setzt den Angelpunkt fest
                X = f;
                //Setzt die Schlüsselmöglichkeiten fest
                AB = f.Possebilities.ToList();

                //Wenn es kein Feld exisitert was offen,zwei Möglichkeiten besitzt, sich in der selben Block/Spalte/Reihe befindet 
                if(all_fields.Where(x=>x.Number == 0 && x.Possebilities.Count == 2).Where(x=>x!=f).Any(x=> x.Column_Number == f.Column_Number || x.Grid_Number == f.Grid_Number) == false || all_fields.Where(x => x.Number == 0 && x.Possebilities.Count == 2).Where(x => x != f).Any(x => x.Row_Number == f.Row_Number || x.Grid_Number == f.Grid_Number) == false)
                    continue;

                //Erstellt eine Liste an Möglichen Y-Feldern
                List<Field> Ys = all_fields.Where(x => x.Number == 0 && x.Possebilities.Count == 2).Where(x => x != f).Where(x => x.Column_Number == f.Column_Number || x.Grid_Number == f.Grid_Number).ToList();

                //Erstellt eine Liste an Möglichen Z-Feldern
                List<Field> Zs = all_fields.Where(x => x.Number == 0 && x.Possebilities.Count == 2).Where(x => x != f).Where(x => x.Row_Number == f.Row_Number || x.Grid_Number == f.Grid_Number).ToList();

                //Geht jedes Y-Feld durch was genau eine gleiche Möglichkeit besitzt wie Feld(X)
                foreach (Field y in Ys.Where(x=>x.Possebilities.Intersect(AB).Count() == 1))
                {
                    //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    //Geht jedes Z-Feld durch was genau eine gleiche Möglichkeit besitzt wie Feld(X), genau eine gleiche Möglichkeit besitzt wie Feld(Y) und diese Möglichkeiten ungleich sind
                    foreach (Field z in Zs.Where(x => x.Possebilities.Intersect(AB).Count() == 1).Where(x => x.Possebilities.Intersect(y.Possebilities).Count() == 1 && x.Possebilities.Intersect(y.Possebilities) != y.Possebilities.Intersect(AB)).Where(x => x.Possebilities.Intersect(AB).Intersect(y.Possebilities).Any() == false))
                    {
                        //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                        if (token.IsCancellationRequested)
                        {
                            token.ThrowIfCancellationRequested();
                        }

                        //Wenn die Felder gleich sind dann überspringe
                        if (y == z)
                            continue;

                        //Setzt die Angelpunktfelder fest
                        Y = y;
                        Z = z;

                        //Wenn es ein Feld exisitert wo sich der Einflussbereich von Feld(Y) und Feld(Z) überschneiden und die Möglichkeit (C) besitzt
                        if (all_fields.Where(x => x.Number == 0).Where(x => ((x.Grid_Number == Z.Grid_Number && x.Row_Number == Y.Row_Number) || (x.Grid_Number == Y.Grid_Number && x.Column_Number == Z.Column_Number)) || (x.Column_Number == Z.Column_Number && x.Row_Number == Y.Row_Number)).Where(x => (x.Column_Number == Z.Column_Number || x.Row_Number == Y.Row_Number)).Where(x => (x != X && x != Y) && (x != X && x != Z)).Any(x => x.Possebilities.Contains(Y.Possebilities.Intersect(Z.Possebilities).FirstOrDefault())))
                        {
                            //Setzt die zulöschente Möglichkeit fest und gehe zum Y-Wing Catcher 
                            C = Y.Possebilities.Intersect(Z.Possebilities).FirstOrDefault();
                            goto Catch_Y_Wing;
                        }
                    }
                }
            }

            //Wenn keine zulöschente Möglichkeit gefunden wurde dann beende diese Technik
            if (C == 0)
                return;

            Catch_Y_Wing:

            //Geht jedes Feld durch wo sich der Einflussbereich von Feld(Y) und Feld(Z) überschneiden und die Möglichkeit (C) besitzt
            foreach (Field gf in all_fields.Where(x => x.Number == 0).Where(x => ((x.Grid_Number == Z.Grid_Number && x.Row_Number == Y.Row_Number) || (x.Grid_Number == Y.Grid_Number && x.Column_Number == Z.Column_Number)) || (x.Column_Number == Z.Column_Number && x.Row_Number == Y.Row_Number)).Where(x => (x.Column_Number == Z.Column_Number || x.Row_Number == Y.Row_Number)).Where(x => (x != X && x != Y) && (x != X && x != Z)).Where(x => x.Possebilities.Contains(Y.Possebilities.Intersect(Z.Possebilities).FirstOrDefault())).ToList())
            {
                //Wenn die Möglichkeit (C) in diesem Feld gelöscht werden kann
                if (gf.Possebilities.Remove(C) == true)
                {
                    //Logbucheintrag
                    MainPage.Logs.Insert(0, new Logdata() { ID = gf.Id, Grid = gf.Grid_Number, Row = gf.Row_Number, Column = gf.Column_Number }.Removed_Market_Number_From_Field(C, all_fields, Logdata.Strategies.Y_Wing));
                    //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                    await Waiter.Waiting();
                    is_saturadet = false;
                }
            }

            //Wenn ein X-Wing gefunden wurde dann unterbreche die Suche
            if (is_saturadet == false)
            {
                //Wenn diese Technik noch nicht angewand wurde dann erhöhe den Counter dieser Technik, sonst erstelle die Technik in den Used_Technics
                if (MainPage.Used_Technics.Any(x => x.Technic == Used_Technic.Technics.Y_Wing))
                {
                    MainPage.Used_Technics.FirstOrDefault(x => x.Technic == Used_Technic.Technics.Y_Wing).Count++;
                }
                else
                {
                    MainPage.Used_Technics.Add(new Used_Technic() { Count = 1, Technic = Used_Technic.Technics.Y_Wing });
                }
                return;
            }
        }
    }
}
