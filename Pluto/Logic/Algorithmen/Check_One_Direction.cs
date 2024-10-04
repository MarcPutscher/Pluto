using Microsoft.Maui.Graphics.Text;
using Pluto.Models;
using Pluto.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Pluto.Logic.Algorithmen
{
    public class Check_One_Direction
    {
        public Check_One_Direction() { }

        /// <summary>
        /// Der Hauptprozzes wo überprüft wird ob das ganze Spielfeld korrekt ist. 
        /// </summary>
        public async Task<bool> MainProcess(CancellationToken token , MainPage mainPage)
        {
            //Erstellt eine syncrone Liste von allen Feldern
            List<Field> all_fields = new List<Field>();
            foreach (ObservableCollection<Field> fs in MainPage.Fields)
            {
                foreach (Field f in fs)
                {
                    all_fields.Add(f);
                }
            }


            //Erstellt eine Liste mit Listen die Felder beinhaltet, wo die Blöcke horizontal auf einem Level sind
            List<List<Field>> horizontal_blocks = new List<List<Field>>();
            for (int i = 0; i < 3; i++)
            {
                List<Field> list = new List<Field>();
                foreach (Field f in all_fields)
                {
                    if (new List<int>() { 1 + (i*3), 2 + (i * 3), 3 + (i * 3) }.Contains(f.Grid_Number) == true)
                    {
                        list.Add(f);
                    }
                }
                horizontal_blocks.Add(list);
            }

            //Erstellt eine Liste mit Listen die Felder beinhaltet, wo die Blöcke vertikal auf einem Level sind
            List<List<Field>> vertikal_blocks = new List<List<Field>>();
            for (int i = 0; i < 3; i++)
            {
                List<Field> list = new List<Field>();
                foreach(Field f in all_fields)
                {
                    if (new List<int>() { 1 + i, 4 + i, 7 + i }.Contains(f.Grid_Number) == true)
                    {
                        list.Add(f);
                    }
                }
                vertikal_blocks.Add(list);
            }


            //Ermittelt zuerst die Eindeutigen Zahlen
            for (int i = 0; i < 3; i++)
            {
                await Task.Delay(1);

                await Check_Blocklevel_For_Clearly(all_fields, horizontal_blocks[i], "horizontal", token);

                await Check_Blocklevel_For_Clearly(all_fields, vertikal_blocks[i], "vertikal", token);
            }

            //Solange das Spielfeld nicht gelöst ist fahre fort
            for (bool playground_is_solved = false; playground_is_solved == false;)
            {
                await Task.Delay(1);

                //Wenn keine 0 mehr auf dem Spielfeld ist,dann ist das Spielfeld gelöst
                if (all_fields.Where(x=>x.Number == 0).Count() == 0)
                {
                    playground_is_solved = true;
                    break;
                }

                //Erstellt einen Zähler der zählt wie viele Felder geändert wurden
                int counter = 0;

                //Geht die Blockslevel durch
                for (int i = 0;i < 3;i++)
                {
                     bool result = await Check_Blocklevel(all_fields, horizontal_blocks[i], "horizontal", token);
                    if(result == true)
                        counter++;

                     bool result2 = await Check_Blocklevel(all_fields, vertikal_blocks[i], "vertikal",token);
                    if (result2 == true)
                        counter++;
                }

                if(counter == 0)
                {              
                    //Geht alle Blocks durch und schaut ob es eindeutige Felder existieren und ändert sie zu diese
                    for (int p = 0; p < 9; p++)
                    {
                        //Wenn der Block schon komplett ausgefüllt ist überspringe
                        if (MainPage.Fields[p].Where(x => x.Number == 0).Count() == 0)
                            continue;

                        //Geht die Zahlen durch
                        for (int z = 1; z < 10; z++)
                        {
                            //Ermittelt die wirklichen Möglichkeiten für diese Zahl die den Regeln entsprechen
                            List<int> realy_possible = new List<int>();

                            //Geht die Felder in dem Block durch
                            foreach (Field f in MainPage.Fields[p])
                            {
                                //Wenn Feld gelockt oder eindeutig dann überspringe
                                if (f.Is_Clearly == true || f.Is_Locked == true || f.Number != 0)
                                    continue;

                                // Platzhalter Feld für das ausgewählte Feld
                                Field Placeholder2 = new Field() { Id = f.Id, Column_Number = f.Column_Number, Number = z, Row_Number = f.Row_Number, Grid_Number = f.Grid_Number, Is_Fault = f.Is_Fault, Is_Select = f.Is_Select, Number_Background_Color = f.Number_Background_Color, Number_Color = f.Number_Color, Visible_Number = f.Visible_Number, Is_Locked = f.Is_Locked };

                                //Überprüft ob diese Zahl im aktuellen Spielfeld erlaubt ist
                                List<bool>checklist = new List<bool>();
                                List<Field>faults = new List<Field>();

                                (checklist, faults) = GameRules.Check_Rules_OneMove(all_fields, Placeholder2);

                                //Wenn alle Regeln korrekt sind und es keine Fehlerfelder gibt
                                if (checklist.Contains(false) == false && faults.Count == 0)
                                {
                                    realy_possible.Add(Placeholder2.Id);
                                }
                            }

                            //Wenn für dieses Feld nur diese einzige Möglichkeit besteht
                            if (realy_possible.Count() == 1)
                            {
                                //Setzt die Zahl zum aktuellen Feld
                                MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Number = z;

                                //Wenn zu der Zahl, der Richtung, dem Grid und dem Level keine Logbucheintrag vorhanden ist
                                if (MainPage.Possebilities_Log.Where(x => x.Number == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Number && x.Direction == "vertikal" && x.Grid == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Grid_Number && x.Level == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Row_Number).Count() == 0)
                                {
                                    //Erstellt eine Möglichkeit zu diesem Zug und fügt sie dem Logbuch hinzu
                                    MainPage.Possebilities_Log.Add(new Possebilitie() { Direction = "vertikal", Number = MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Number, Max = 1, Level = MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Row_Number, Grid = MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Grid_Number, Current_Field = MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()) });
                                }
                                else
                                {
                                    MainPage.Possebilities_Log.Where(x => x.Number == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Number && x.Direction == "vertikal" && x.Grid == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Grid_Number && x.Level == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Row_Number).First().Max = 1;
                                    MainPage.Possebilities_Log.Where(x => x.Number == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Number && x.Direction == "vertikal" && x.Grid == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Grid_Number && x.Level == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Row_Number).First().Count_of_Skip = 0;
                                    MainPage.Possebilities_Log.Where(x => x.Number == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Number && x.Direction == "vertikal" && x.Grid == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Grid_Number && x.Level == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Row_Number).First().Current_Field = MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First());
                                }

                                //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
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

                                counter++;
                            }
                        }
                    }
                }

                //Wenn keine Felder geändert wurde
                if(counter == 0)
                {
                    mainPage.Attampts_Label++;
                    //Wenn im Feldlog Felder vorhanden sind
                    if(MainPage.Possebilities_Log.Count() !=0 )
                    {
                        //Geht die Logs durch bis es auf eine max Möglichkeiten größer 1 trifft und entferne die Einträge vor
                        for (int i = MainPage.Possebilities_Log.Count() - 1; i >= 0; i--)
                        {
                            //Wenn ein Feld als Eindeutig eingestufft wurde dann wird fortgefahren
                            if (MainPage.Possebilities_Log[i].Clearly == true)
                                continue;

                            if (MainPage.Possebilities_Log[i].Max > 1 && MainPage.Possebilities_Log[i].Max > MainPage.Possebilities_Log[i].Count_of_Skip + 1)
                            {
                                MainPage.Possebilities_Log[i].Count_of_Skip++;
                                MainPage.Possebilities_Log[i].Current_Field.Number = 0;
                                MainPage.Possebilities_Log[i].Current_Field.Is_Clearly = false;
                                MainPage.Possebilities_Log[i].Current_Field = new Field();
                                //MainPage.Possebilities_Log[i].Remove_Placeholder_Number_In_Fields();
                                //MainPage.Possebilities_Log[i].Others.Clear();
                                break;
                            }
                            else
                            {
                                MainPage.Possebilities_Log[i].Current_Field.Number = 0;
                                MainPage.Possebilities_Log[i].Current_Field.Is_Clearly = false;
                                //MainPage.Possebilities_Log[i].Remove_Placeholder_Number_In_Fields();
                                MainPage.Possebilities_Log.RemoveAt(i);
                            }
                        }

                        //int remove_from = 0;
                        ////Geht die Logs durch bis es auf eine max Möglichkeiten größer 1 trifft und entferne die Einträge vor
                        //for (int i = 0; i <= MainPage.Possebilities_Log.Count() - 1; i++)
                        //{
                        //    //Wenn ein Feld als Eindeutig eingestufft wurde dann wird fortgefahren
                        //    if (MainPage.Possebilities_Log[i].Clearly == true)
                        //        continue;

                        //    if (MainPage.Possebilities_Log[i].Max > 1 && MainPage.Possebilities_Log[i].Max > MainPage.Possebilities_Log[i].Count_of_Skip + 1)
                        //    {
                        //        MainPage.Possebilities_Log[i].Count_of_Skip++;
                        //        MainPage.Possebilities_Log[i].Current_Field.Number = 0;
                        //        MainPage.Possebilities_Log[i].Current_Field = new Field();
                        //        MainPage.Possebilities_Log[i].Remove_Placeholder_Number_In_Fields();
                        //        MainPage.Possebilities_Log[i].Others.Clear();
                        //        remove_from = i;
                        //        break;
                        //    }
                        //}

                        //for (int i = remove_from+1; i <= MainPage.Possebilities_Log.Count() - remove_from-1; i++)
                        //{
                        //    MainPage.Possebilities_Log[i].Current_Field.Number = 0;
                        //    MainPage.Possebilities_Log[i].Remove_Placeholder_Number_In_Fields();
                        //    MainPage.Possebilities_Log.RemoveAt(i);
                        //}
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Überprüft ob es in dem Blocks auf einem Level eine Zahl gibt die Möglichkeiten besitzt und wenn ja dann wir dieses Zahl eingesetzt   
        /// </summary>
        public async Task<bool> Check_Blocklevel(List<Field> all_fields,List<Field> blocks,string flow, CancellationToken token)
        {
            //Erstellt einen Zähler der zählt wie viele Felder geändert wurden
            int counter = 0;

            //Erstellt eine Liste die alle Gridnummern beinhalten die im Blocklevel sind 
            List<int> grids = new List<int>();
            for(int g = 0; g<27; g = g+9)
            {
                grids.Add(blocks[g].Grid_Number);
            }

            //Erstellt eine Liste die alle spezielen Levelnummern beinhalten die im Blocklevel sind 
            List<int> levels = new List<int>();
            for(int i = 0; i < 3; i++)
            {
                if (flow == "vertikal")
                {
                    levels.Add(blocks[3*i].Column_Number);
                }
                if (flow == "horizontal")
                {
                    levels.Add(blocks[i].Row_Number);
                }
            }

            //Geht alle Zahlen von 1 bis 10 durch
            for (int i = 1;i < 10;i++)
            {
                //Wenn der Prozess abgebrochen wird
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                //Schaut ob im Blocklevel von der aktuellen Zahl zwei Felder vorhanden sind 
                if (blocks.Where(y=>y.Number == i).Count() == 2)
                {
                    //Erstellt eine Liste von den Feldern die die gleiche Zahl haben
                    List<Field> selectet_fields = blocks.Where(y => y.Number == i).ToList();

                    //Ermittelt in welchem Block sich das Feld befindet wo die Zahl fehlt
                    int missing_block = 0;
                    foreach(int gr in grids)
                    {
                        if(selectet_fields.Where(x=>x.Grid_Number == gr).Count() == 0)
                        {
                            missing_block = gr;
                            break;
                        }
                    }

                    //Ermittelt auf welchem Level sich das Feld befindet wo die Zahl fehlt
                    int missing_level = 0;
                    foreach (int l in levels)
                    {
                        if(flow == "horizontal")
                        {
                            if (selectet_fields.Where(x => x.Row_Number == l).Count() == 0)
                            {
                                missing_level = l;
                                break;
                            }
                        }
                        if (flow == "vertikal")
                        {
                            if (selectet_fields.Where(x => x.Column_Number == l).Count() == 0)
                            {
                                missing_level = l;
                                break;
                            }
                        }
                    }

                    //Ermittelt wie viele Felder auf dem Level die Zahl 0 haben 
                    List<int> missing_fields = new List<int>();
                    if (flow == "horizontal")
                    {
                        if (blocks.Where(x => x.Row_Number == missing_level && x.Number == 0).Count() != 0)
                        {
                            foreach(Field f in blocks.Where(x => x.Row_Number == missing_level && x.Number == 0).ToList())
                            {
                                missing_fields.Add(f.Column_Number);
                            }
                        }
                    }
                    if (flow == "vertikal")
                    {
                        if (blocks.Where(x =>x.Column_Number == missing_level && x.Number == 0).Count() != 0)
                        {
                            foreach (Field f in blocks.Where(x => x.Column_Number == missing_level && x.Number == 0).ToList())
                            {
                                missing_fields.Add(f.Row_Number);
                            }
                        }
                    }

                    //Das aktuelle Feld was bearbeitet werden soll
                    Field selectet_field = new Field();

                    //Geht jede Position in dem Level durch
                    foreach(int l in missing_fields)
                    {
                        //Ermittelt das Feld wo die Zahl fehlt in der Reihe
                        if (flow == "horizontal")
                        {
                            if (blocks.Where(x => x.Grid_Number == missing_block && x.Row_Number == missing_level && x.Column_Number == l && x.Number == 0).Count() != 0)
                            {
                                selectet_field = blocks.Where(x => x.Grid_Number == missing_block && x.Row_Number == missing_level && x.Column_Number == l &&x.Number == 0).First();
                            }
                            else
                            {
                                continue;
                            }
                        }

                        //Ermittelt das Feld wo die Zahl fehlt in der Spalte
                        if (flow == "vertikal")
                        {
                            if (blocks.Where(x => x.Grid_Number == missing_block && x.Column_Number == missing_level && x.Row_Number == l && x.Number == 0).Count() != 0)
                            {
                                selectet_field = blocks.Where(x => x.Grid_Number == missing_block && x.Column_Number == missing_level && x.Row_Number == l && x.Number == 0).First();
                            }
                            else
                            {
                                continue;
                            }
                        }

                        // Platzhalter Feld für das aktuelle Feld
                        Field Placeholder = new Field() { Id = selectet_field.Id, Number = i, Column_Number = selectet_field.Column_Number, Row_Number = selectet_field.Row_Number, Grid_Number = selectet_field.Grid_Number, Is_Fault = selectet_field.Is_Fault, Is_Select = selectet_field.Is_Select, Number_Background_Color = selectet_field.Number_Background_Color, Number_Color = selectet_field.Number_Color, Visible_Number = selectet_field.Visible_Number, Is_Locked = selectet_field.Is_Locked };

                        //Überprüft ob diese Zahl im aktuellen Spielfeld erlaubt ist
                        List<bool> checklist = new List<bool>();
                        List<Field> faults = new List<Field>();
                        (checklist, faults) = GameRules.Check_Rules_OneMove(all_fields, Placeholder);

                        //Wenn alle Regeln korrekt sind und es keine Fehlerfelder gibt
                        if (checklist.Contains(false) == false && faults.Count == 0)
                        {
                            //Wenn zu der Zahl, der Richtung, dem Grid und dem Level keine Logbucheintrag vorhanden ist
                            if (MainPage.Possebilities_Log.Where(x=>x.Number == Placeholder.Number && x.Direction == flow && x.Grid == missing_block && x.Level == missing_level).Count() == 0)
                            {
                                //Erstellt eine Möglichkeit zu diesem Zug und fügt sie dem Logbuch hinzu
                                MainPage.Possebilities_Log.Add(new Possebilitie() { Direction = flow, Number = Placeholder.Number, Level = missing_level, Grid = missing_block, Current_Field = selectet_field });

                                //Geht die Möglichkeiten durch
                                foreach (int o in missing_fields)
                                {
                                    //Erstellt ein imaginäres Feld um die Möglichkeiten zu ermitteln
                                    Field selectet_field3 = new Field();

                                    //Ermittelt das Feld wo die Zahl fehlt in der Reihe
                                    if (flow == "horizontal")
                                    {
                                        if (blocks.Where(x => x.Grid_Number == missing_block && x.Row_Number == missing_level && x.Column_Number == o && x.Number == 0).Count() != 0)
                                        {
                                            selectet_field3 = blocks.Where(x => x.Grid_Number == missing_block && x.Row_Number == missing_level && x.Column_Number == o && x.Number == 0).First();
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }

                                    //Ermittelt das Feld wo die Zahl fehlt in der Spalte
                                    if (flow == "vertikal")
                                    {
                                        if (blocks.Where(x => x.Grid_Number == missing_block && x.Column_Number == missing_level && x.Row_Number == o && x.Number == 0).Count() != 0)
                                        {
                                            selectet_field3 = blocks.Where(x => x.Grid_Number == missing_block && x.Column_Number == missing_level && x.Row_Number == o && x.Number == 0).First();
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }

                                    // Platzhalter Feld für das imaginäre Feld
                                    Field Placeholder3 = new Field() { Id = selectet_field3.Id, Number = i, Column_Number = selectet_field3.Column_Number, Row_Number = selectet_field3.Row_Number, Grid_Number = selectet_field3.Grid_Number, Is_Fault = selectet_field3.Is_Fault, Is_Select = selectet_field3.Is_Select, Number_Background_Color = selectet_field3.Number_Background_Color, Number_Color = selectet_field3.Number_Color, Visible_Number = selectet_field3.Visible_Number, Is_Locked = selectet_field3.Is_Locked };

                                    //Überprüft ob diese Zahl im aktuellen Spielfeld erlaubt ist
                                    checklist = new List<bool>();
                                    faults = new List<Field>();
                                    (checklist, faults) = GameRules.Check_Rules_OneMove(all_fields, Placeholder3);

                                    if (checklist.Contains(false) == false && faults.Count == 0)
                                    {
                                        //Erhöht die max Möglichkeiten für den Zug und fügt das Feld zu der disjunkten Menge hinzu
                                        MainPage.Possebilities_Log.Last().Max++;
                                        //MainPage.Possebilities_Log.Last().Others.Add(selectet_field3);
                                    }
                                }

                                ////Fügt die Plazhalter zu den Feldern in der disjunkten Menge hinzu
                                //MainPage.Possebilities_Log.Last().Create_Placeholder_Number_In_Fields();
                            }

                            //Erstellt die Möglichkeit die für diesen Zug zuständig ist
                            Possebilitie possebilitie = MainPage.Possebilities_Log.Where(x => x.Number == Placeholder.Number && x.Direction == flow && x.Grid == missing_block && x.Level == missing_level).First();

                            ////Fügt das Feld zu der disjunkten Menge hinzu wenn die Menge dieses Feld noch nicht beinhaltet
                            //if (possebilitie.Others.Contains(selectet_field) == false)
                            //    possebilitie.Others.Add(selectet_field);

                            ////Fügt die Plazhalter zu den Feldern in der disjunkten Menge hinzu
                            //if (missing_fields.Count > 1)
                            //    possebilitie.Create_Placeholder_Number_In_Fields();

                            //Wenn die Möglichkeiten größer 1 sind und Möglichkeiten übersprungen werden sollen dann überspringe dieses Feld
                            if (possebilitie.Max > 1 && possebilitie.Current_Field.Id != selectet_field.Id)
                            {
                                continue;
                            }

                            //Setzt die Zahl zum aktuellen Feld
                            selectet_field.Number = Placeholder.Number;

                            //Erhöht den Zähler der die geänderten Felder zählt
                            counter++;

                            //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
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

                            ////Prüft ob in der Spalte vom Feld nur noch eine Zahl frei ist
                            //if(all_fields.Where(y=>y.Column_Number == selectet_field.Column_Number).Where(x => x.Number != 0).Count() == 8)
                            //{
                            //    // Das Feld das als einziges leer in der Spalte ist
                            //    Field selectet_field2 = all_fields.Where(y => y.Column_Number == Placeholder.Column_Number).Where(x => x.Number == 0).First();

                            //    // Platzhalter Feld für das aktuelle Feld
                            //    Field Placeholder2 = new Field() { Id = selectet_field2.Id, Column_Number = selectet_field2.Column_Number, Row_Number = selectet_field2.Row_Number, Grid_Number = selectet_field2.Grid_Number, Is_Fault = selectet_field2.Is_Fault, Is_Select = selectet_field2.Is_Select, Number_Background_Color = selectet_field2.Number_Background_Color, Number_Color = selectet_field2.Number_Color, Visible_Number = selectet_field2.Visible_Number, Is_Locked = selectet_field2.Is_Locked };

                            //    //Ermittel die letzte Zahl für das Feld
                            //    for (int d =1; d<10;d++)
                            //    {
                            //        if (all_fields.Where(y => y.Column_Number == Placeholder2.Column_Number).Where(x => x.Number == d).Count() == 0)
                            //        {
                            //            Placeholder2.Number = d;
                            //            break;
                            //        }
                            //    }

                            //    //Überprüft ob diese Zahl im aktuellen Spielfeld erlaubt ist
                            //    checklist = new List<bool>();
                            //    faults = new List<Field>();

                            //    (checklist, faults) = GameRules.Check_Rules_OneMove(all_fields, Placeholder2);

                            //    //Wenn alle Regeln korrekt sind und es keine Fehlerfelder gibt
                            //    if (checklist.Contains(false) == false && faults.Count == 0)
                            //    {
                            //        //Setzt die Zahl zum aktuellen Feld
                            //        selectet_field2.Number = Placeholder2.Number;

                            //        //Wenn zu der Zahl, der Richtung, dem Grid und dem Level keine Logbucheintrag vorhanden ist
                            //        if (MainPage.Possebilities_Log.Where(x => x.Number == selectet_field2.Number && x.Direction == flow && x.Grid == selectet_field2.Grid_Number && x.Level == selectet_field2.Column_Number).Count() == 0)
                            //        {
                            //            //Erstellt eine Möglichkeit zu diesem Zug und fügt sie dem Logbuch hinzu
                            //            MainPage.Possebilities_Log.Add(new Possebilitie() { Direction = flow, Number = selectet_field2.Number, Max = 1, Level = selectet_field2.Column_Number, Grid = selectet_field2.Grid_Number, Current_Field = selectet_field2 });
                            //        }
                            //        else
                            //        {
                            //            MainPage.Possebilities_Log.Where(x => x.Number == selectet_field2.Number && x.Direction == flow && x.Grid == selectet_field2.Grid_Number && x.Level == selectet_field2.Column_Number).First().Max = 1;
                            //            MainPage.Possebilities_Log.Where(x => x.Number == selectet_field2.Number && x.Direction == flow && x.Grid == selectet_field2.Grid_Number && x.Level == selectet_field2.Column_Number).First().Count_of_Skip = 0;
                            //            MainPage.Possebilities_Log.Where(x => x.Number == selectet_field2.Number && x.Direction == flow && x.Grid == selectet_field2.Grid_Number && x.Level == selectet_field2.Column_Number).First().Current_Field = selectet_field2;
                            //        }

                            //        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                            //        if (MainPage.dificulty_marker == "Leicht")
                            //        {
                            //            await Task.Delay(100);
                            //        }
                            //        if (MainPage.dificulty_marker == "Mittel")
                            //        {
                            //            await Task.Delay(80);
                            //        }
                            //        if (MainPage.dificulty_marker == "Schwer")
                            //        {
                            //            await Task.Delay(50);
                            //        }
                            //        if (MainPage.dificulty_marker == "Experte")
                            //        {
                            //            await Task.Delay(40);
                            //        }
                            //        if (MainPage.dificulty_marker == "Meister")
                            //        {
                            //            await Task.Delay(30);
                            //        }
                            //        if (MainPage.dificulty_marker == "Extrem")
                            //        {
                            //            await Task.Delay(20);
                            //        }
                            //    }
                            //}

                            ////Prüft ob in der Reihe vom Feld nur noch eine Zahl frei ist
                            //if (all_fields.Where(y => y.Row_Number == selectet_field.Row_Number).Where(x => x.Number != 0).Count() == 8)
                            //{
                            //    // Das Feld das als einziges leer in der Spalte ist
                            //    Field selectet_field2 = all_fields.Where(y => y.Row_Number == Placeholder.Row_Number).Where(x => x.Number == 0).First();

                            //    // Platzhalter Feld für das aktuelle Feld
                            //    Field Placeholder2 = new Field() { Id = selectet_field2.Id, Column_Number = selectet_field2.Column_Number, Row_Number = selectet_field2.Row_Number, Grid_Number = selectet_field2.Grid_Number, Is_Fault = selectet_field2.Is_Fault, Is_Select = selectet_field2.Is_Select, Number_Background_Color = selectet_field2.Number_Background_Color, Number_Color = selectet_field2.Number_Color, Visible_Number = selectet_field2.Visible_Number, Is_Locked = selectet_field2.Is_Locked };

                            //    //Ermittel die letzte Zahl für das Feld
                            //    for (int d = 1; d < 10; d++)
                            //    {
                            //        if (all_fields.Where(y => y.Row_Number == Placeholder2.Row_Number).Where(x => x.Number == d).Count() == 0)
                            //        {
                            //            Placeholder2.Number = d;
                            //            break;
                            //        }
                            //    }

                            //    //Überprüft ob diese Zahl im aktuellen Spielfeld erlaubt ist
                            //    checklist = new List<bool>();
                            //    faults = new List<Field>();

                            //    (checklist, faults) = GameRules.Check_Rules_OneMove(all_fields, Placeholder2);

                            //    //Wenn alle Regeln korrekt sind und es keine Fehlerfelder gibt
                            //    if (checklist.Contains(false) == false && faults.Count == 0)
                            //    {
                            //        //Setzt die Zahl zum aktuellen Feld
                            //        selectet_field2.Number = Placeholder2.Number;

                            //        //Wenn zu der Zahl, der Richtung, dem Grid und dem Level keine Logbucheintrag vorhanden ist
                            //        if (MainPage.Possebilities_Log.Where(x => x.Number == selectet_field2.Number && x.Direction == flow && x.Grid == selectet_field2.Grid_Number && x.Level == selectet_field2.Row_Number).Count() == 0)
                            //        {
                            //            //Erstellt eine Möglichkeit zu diesem Zug und fügt sie dem Logbuch hinzu
                            //            MainPage.Possebilities_Log.Add(new Possebilitie() { Direction = flow, Number = selectet_field2.Number, Max = 1, Level = selectet_field2.Row_Number, Grid = selectet_field2.Grid_Number, Current_Field = selectet_field2 });
                            //        }
                            //        else
                            //        {
                            //            MainPage.Possebilities_Log.Where(x => x.Number == selectet_field2.Number && x.Direction == flow && x.Grid == selectet_field2.Grid_Number && x.Level == selectet_field2.Row_Number).First().Max = 1;
                            //            MainPage.Possebilities_Log.Where(x => x.Number == selectet_field2.Number && x.Direction == flow && x.Grid == selectet_field2.Grid_Number && x.Level == selectet_field2.Row_Number).First().Count_of_Skip = 0;
                            //            MainPage.Possebilities_Log.Where(x => x.Number == selectet_field2.Number && x.Direction == flow && x.Grid == selectet_field2.Grid_Number && x.Level == selectet_field2.Row_Number).First().Current_Field = selectet_field2;
                            //        }

                            //        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                            //        if (MainPage.dificulty_marker == "Leicht")
                            //        {
                            //            await Task.Delay(100);
                            //        }
                            //        if (MainPage.dificulty_marker == "Mittel")
                            //        {
                            //            await Task.Delay(80);
                            //        }
                            //        if (MainPage.dificulty_marker == "Schwer")
                            //        {
                            //            await Task.Delay(50);
                            //        }
                            //        if (MainPage.dificulty_marker == "Experte")
                            //        {
                            //            await Task.Delay(40);
                            //        }
                            //        if (MainPage.dificulty_marker == "Meister")
                            //        {
                            //            await Task.Delay(30);
                            //        }
                            //        if (MainPage.dificulty_marker == "Extrem")
                            //        {
                            //            await Task.Delay(20);
                            //        }
                            //    }
                            //}

                            ////Prüft ob in dem Block vom Feld nur noch eine Zahl frei ist
                            //if (all_fields.Where(y => y.Grid_Number == selectet_field.Grid_Number).Where(x => x.Number != 0).Count() == 8)
                            //{
                            //    // Das Feld das als einziges leer in der Spalte ist
                            //    Field selectet_field2 = all_fields.Where(y => y.Grid_Number == Placeholder.Grid_Number).Where(x => x.Number == 0).First();

                            //    // Platzhalter Feld für das aktuelle Feld
                            //    Field Placeholder2 = new Field() { Id = selectet_field2.Id, Column_Number = selectet_field2.Column_Number, Row_Number = selectet_field2.Row_Number, Grid_Number = selectet_field2.Grid_Number, Is_Fault = selectet_field2.Is_Fault, Is_Select = selectet_field2.Is_Select, Number_Background_Color = selectet_field2.Number_Background_Color, Number_Color = selectet_field2.Number_Color, Visible_Number = selectet_field2.Visible_Number, Is_Locked = selectet_field2.Is_Locked };

                            //    //Ermittel die letzte Zahl für das Feld
                            //    for (int d = 1; d < 10; d++)
                            //    {
                            //        if (all_fields.Where(y => y.Grid_Number == Placeholder2.Grid_Number).Where(x => x.Number == d).Count() == 0)
                            //        {
                            //            Placeholder2.Number = d;
                            //            break;
                            //        }
                            //    }

                            //    //Überprüft ob diese Zahl im aktuellen Spielfeld erlaubt ist
                            //    checklist = new List<bool>();
                            //    faults = new List<Field>();

                            //    (checklist, faults) = GameRules.Check_Rules_OneMove(all_fields, Placeholder2);

                            //    //Wenn alle Regeln korrekt sind und es keine Fehlerfelder gibt
                            //    if (checklist.Contains(false) == false && faults.Count == 0)
                            //    {
                            //        //Setzt die Zahl zum aktuellen Feld
                            //        selectet_field2.Number = Placeholder2.Number;

                            //        //Wenn zu der Zahl, der Richtung, dem Grid und dem Level keine Logbucheintrag vorhanden ist
                            //        if (MainPage.Possebilities_Log.Where(x => x.Number == selectet_field2.Number && x.Direction == flow && x.Grid == selectet_field2.Grid_Number && x.Level == selectet_field2.Row_Number).Count() == 0)
                            //        {
                            //            //Erstellt eine Möglichkeit zu diesem Zug und fügt sie dem Logbuch hinzu
                            //            MainPage.Possebilities_Log.Add(new Possebilitie() { Direction = flow, Number = selectet_field2.Number, Max = 1, Level = selectet_field2.Row_Number, Grid = selectet_field2.Grid_Number, Current_Field = selectet_field2 });
                            //        }
                            //        else
                            //        {
                            //            MainPage.Possebilities_Log.Where(x => x.Number == selectet_field2.Number && x.Direction == flow && x.Grid == selectet_field2.Grid_Number && x.Level == selectet_field2.Row_Number).First().Max = 1;
                            //            MainPage.Possebilities_Log.Where(x => x.Number == selectet_field2.Number && x.Direction == flow && x.Grid == selectet_field2.Grid_Number && x.Level == selectet_field2.Row_Number).First().Count_of_Skip = 0;
                            //            MainPage.Possebilities_Log.Where(x => x.Number == selectet_field2.Number && x.Direction == flow && x.Grid == selectet_field2.Grid_Number && x.Level == selectet_field2.Row_Number).First().Current_Field = selectet_field2;
                            //        }

                            //        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
                            //        if (MainPage.dificulty_marker == "Leicht")
                            //        {
                            //            await Task.Delay(100);
                            //        }
                            //        if (MainPage.dificulty_marker == "Mittel")
                            //        {
                            //            await Task.Delay(80);
                            //        }
                            //        if (MainPage.dificulty_marker == "Schwer")
                            //        {
                            //            await Task.Delay(50);
                            //        }
                            //        if (MainPage.dificulty_marker == "Experte")
                            //        {
                            //            await Task.Delay(40);
                            //        }
                            //        if (MainPage.dificulty_marker == "Meister")
                            //        {
                            //            await Task.Delay(30);
                            //        }
                            //        if (MainPage.dificulty_marker == "Extrem")
                            //        {
                            //            await Task.Delay(20);
                            //        }
                            //    }
                            //}

                            break;
                        }
                    }
                }
            }

            if(counter == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Überprüft ob es in dem Blocks auf einem Level eine Zahl gibt die nur eine Möglichkeit besitzt und wenn ja dann wir dieses Zahl eingesetzt   
        /// </summary>
        public async Task Check_Blocklevel_For_Clearly(List<Field> all_fields, List<Field> blocks, string flow, CancellationToken token)
        {
            //Erstellt eine Liste die alle Gridnummern beinhalten die im Blocklevel sind 
            List<int> grids = new List<int>();
            for (int g = 0; g < 27; g = g + 9)
            {
                grids.Add(blocks[g].Grid_Number);
            }

            //Erstellt eine Liste die alle spezielen Levelnummern beinhalten die im Blocklevel sind 
            List<int> levels = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                if (flow == "vertikal")
                {
                    levels.Add(blocks[3 * i].Column_Number);
                }
                if (flow == "horizontal")
                {
                    levels.Add(blocks[i].Row_Number);
                }
            }

            //Geht alle Zahlen von 1 bis 10 durch
            for (int i = 1; i < 10; i++)
            {
                //Wenn der Prozess abgebrochen wird
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                //Schaut ob im Blocklevel von der aktuellen Zahl zwei Felder vorhanden sind und ob diese gelockt sind 
                if (blocks.Where(y => y.Number == i && y.Is_Locked == true).Count() == 2)
                {
                    //Erstellt eine Liste von den Feldern die die gleiche Zahl haben und gelockt sind
                    List<Field> selectet_fields = blocks.Where(y => y.Number == i && y.Is_Locked == true).ToList();

                    //Ermittelt in welchem Block sich das Feld befindet wo die Zahl fehlt
                    int missing_block = 0;
                    foreach (int gr in grids)
                    {
                        if (selectet_fields.Where(x => x.Grid_Number == gr).Count() == 0)
                        {
                            missing_block = gr;
                            break;
                        }
                    }

                    //Ermittelt auf welchem Level sich das Feld befindet wo die Zahl fehlt
                    int missing_level = 0;
                    foreach (int l in levels)
                    {
                        if (flow == "horizontal")
                        {
                            if (selectet_fields.Where(x => x.Row_Number == l).Count() == 0)
                            {
                                missing_level = l;
                                break;
                            }
                        }
                        if (flow == "vertikal")
                        {
                            if (selectet_fields.Where(x => x.Column_Number == l).Count() == 0)
                            {
                                missing_level = l;
                                break;
                            }
                        }
                    }

                    //Ermittelt wie viele Felder auf dem Level die Zahl 0 haben 
                    List<int> missing_fields = new List<int>();
                    if (flow == "horizontal")
                    {
                        if (blocks.Where(x => x.Row_Number == missing_level && x.Number == 0).Count() != 0)
                        {
                            foreach (Field f in blocks.Where(x => x.Row_Number == missing_level && x.Number == 0).ToList())
                            {
                                missing_fields.Add(f.Column_Number);
                            }
                        }
                    }
                    if (flow == "vertikal")
                    {
                        if (blocks.Where(x => x.Column_Number == missing_level && x.Number == 0).Count() != 0)
                        {
                            foreach (Field f in blocks.Where(x => x.Column_Number == missing_level && x.Number == 0).ToList())
                            {
                                missing_fields.Add(f.Row_Number);
                            }
                        }
                    }

                    //Das aktuelle Feld was bearbeitet werden soll
                    Field selectet_field = new Field();

                    //Ermittelt die wirklichen Möglichkeiten für diese Zahl die den Regeln entsprechen
                    List<int> realy_possible = new List<int>();

                    //Geht jede Position in dem Level durch
                    foreach (int l in missing_fields)
                    {
                        //Ermittelt das Feld wo die Zahl fehlt in der Reihe
                        if (flow == "horizontal")
                        {
                            if (blocks.Where(x => x.Grid_Number == missing_block && x.Row_Number == missing_level && x.Column_Number == l && x.Number == 0).Count() != 0)
                            {
                                selectet_field = blocks.Where(x => x.Grid_Number == missing_block && x.Row_Number == missing_level && x.Column_Number == l && x.Number == 0).First();
                            }
                            else
                            {
                                continue;
                            }
                        }

                        //Ermittelt das Feld wo die Zahl fehlt in der Spalte
                        if (flow == "vertikal")
                        {
                            if (blocks.Where(x => x.Grid_Number == missing_block && x.Column_Number == missing_level && x.Row_Number == l && x.Number == 0).Count() != 0)
                            {
                                selectet_field = blocks.Where(x => x.Grid_Number == missing_block && x.Column_Number == missing_level && x.Row_Number == l && x.Number == 0).First();
                            }
                            else
                            {
                                continue;
                            }
                        }

                        // Platzhalter Feld für das aktuelle Feld
                        Field Placeholder = new Field() { Id = selectet_field.Id, Number = i, Column_Number = selectet_field.Column_Number, Row_Number = selectet_field.Row_Number, Grid_Number = selectet_field.Grid_Number, Is_Fault = selectet_field.Is_Fault, Is_Select = selectet_field.Is_Select, Number_Background_Color = selectet_field.Number_Background_Color, Number_Color = selectet_field.Number_Color, Visible_Number = selectet_field.Visible_Number, Is_Locked = selectet_field.Is_Locked };

                        //Überprüft ob diese Zahl im aktuellen Spielfeld erlaubt ist
                        List<bool> checklist = new List<bool>();
                        List<Field> faults = new List<Field>();
                        (checklist, faults) = GameRules.Check_Rules_OneMove(all_fields, Placeholder);

                        //Wenn alle Regeln korrekt sind und es keine Fehlerfelder gibt
                        if (checklist.Contains(false) == false && faults.Count == 0)
                        {
                            realy_possible.Add(l);
                        }
                    }

                    if(realy_possible.Count() == 1)
                    {
                        //Ermittelt das Feld wo die Zahl fehlt in der Reihe
                        if (flow == "horizontal")
                        {
                            if (blocks.Where(x => x.Grid_Number == missing_block && x.Row_Number == missing_level && x.Column_Number == realy_possible[0] && x.Number == 0).Count() != 0)
                            {
                                selectet_field = blocks.Where(x => x.Grid_Number == missing_block && x.Row_Number == missing_level && x.Column_Number == realy_possible[0] && x.Number == 0).First();
                            }
                            else
                            {
                                continue;
                            }
                        }

                        //Ermittelt das Feld wo die Zahl fehlt in der Spalte
                        if (flow == "vertikal")
                        {
                            if (blocks.Where(x => x.Grid_Number == missing_block && x.Column_Number == missing_level && x.Row_Number == realy_possible[0] && x.Number == 0).Count() != 0)
                            {
                                selectet_field = blocks.Where(x => x.Grid_Number == missing_block && x.Column_Number == missing_level && x.Row_Number == realy_possible[0] && x.Number == 0).First();
                            }
                            else
                            {
                                continue;
                            }
                        }

                        //Wenn zu der Zahl, der Richtung, dem Grid und dem Level keine Logbucheintrag vorhanden ist
                        if (MainPage.Possebilities_Log.Where(x => x.Number == i && x.Direction == flow && x.Grid == missing_block && x.Level == missing_level).Count() == 0)
                        {
                            //Erstellt eine Möglichkeit zu diesem Zug und fügt sie dem Logbuch hinzu
                            MainPage.Possebilities_Log.Add(new Possebilitie() { Direction = flow, Number = i, Level = missing_level, Grid = missing_block, Current_Field = selectet_field, Clearly = true });
                        }

                        //Erstellt die Möglichkeit die für diesen Zug zuständig ist
                        Possebilitie possebilitie = MainPage.Possebilities_Log.Where(x => x.Number == i && x.Direction == flow && x.Grid == missing_block && x.Level == missing_level).First();

                        //Setzt die Zahl zum aktuellen Feld
                        selectet_field.Number = i;

                        //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
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

                        //Geht alle Blocks durch und schaut ob es eindeutige Felder existieren und ändert sie zu diese
                        for (int p = 0; p < 9; p++)
                        {
                            //Wenn der Block schon komplett ausgefüllt ist überspringe
                            if (MainPage.Fields[p].Where(x => x.Number == 0).Count() == 0)
                                continue;

                            //Geht die Zahlen durch
                            for (int z = 1; z < 10; z++)
                            {
                                //Ermittelt die wirklichen Möglichkeiten für diese Zahl die den Regeln entsprechen
                                realy_possible = new List<int>();

                                //Geht die Felder in dem Block durch
                                foreach (Field f in MainPage.Fields[p])
                                {
                                    //Wenn Feld gelockt oder eindeutig dann überspringe
                                    if (f.Is_Clearly == true || f.Is_Locked == true)
                                        continue;

                                    // Platzhalter Feld für das ausgewählte Feld
                                    Field Placeholder2 = new Field() { Id = f.Id, Column_Number = f.Column_Number, Number = z, Row_Number = f.Row_Number, Grid_Number = f.Grid_Number, Is_Fault = f.Is_Fault, Is_Select = f.Is_Select, Number_Background_Color = f.Number_Background_Color, Number_Color = f.Number_Color, Visible_Number = f.Visible_Number, Is_Locked = f.Is_Locked };

                                    //Überprüft ob diese Zahl im aktuellen Spielfeld erlaubt ist
                                    List<bool> checklist = new List<bool>();
                                    List<Field> faults = new List<Field>();

                                    (checklist, faults) = GameRules.Check_Rules_OneMove(all_fields, Placeholder2);

                                    //Wenn alle Regeln korrekt sind und es keine Fehlerfelder gibt
                                    if (checklist.Contains(false) == false && faults.Count == 0)
                                    {
                                        realy_possible.Add(Placeholder2.Id);
                                    }
                                }

                                //Wenn für dieses Feld nur diese einzige Möglichkeit besteht
                                if (realy_possible.Count() == 1)
                                {
                                    //Setzt die Zahl zum aktuellen Feld
                                    MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Number = z;

                                    //Setzt das Feld auf Einzige
                                    MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Is_Clearly = true;

                                    //Wenn zu der Zahl, der Richtung, dem Grid und dem Level keine Logbucheintrag vorhanden ist
                                    if (MainPage.Possebilities_Log.Where(x => x.Number == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Number && x.Direction == flow && x.Grid == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Grid_Number && x.Level == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Row_Number).Count() == 0)
                                    {
                                        //Erstellt eine Möglichkeit zu diesem Zug und fügt sie dem Logbuch hinzu
                                        MainPage.Possebilities_Log.Add(new Possebilitie() { Direction = flow, Clearly = true,  Number = MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Number, Max = 1, Level = MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Row_Number, Grid = MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Grid_Number, Current_Field = MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()) });
                                    }
                                    else
                                    {
                                        MainPage.Possebilities_Log.Where(x => x.Number == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Number && x.Direction == flow && x.Grid == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Grid_Number && x.Level == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Row_Number).First().Clearly = true;
                                        MainPage.Possebilities_Log.Where(x => x.Number == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Number && x.Direction == flow && x.Grid == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Grid_Number && x.Level == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Row_Number).First().Max = 1;
                                        MainPage.Possebilities_Log.Where(x => x.Number == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Number && x.Direction == flow && x.Grid == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Grid_Number && x.Level == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Row_Number).First().Count_of_Skip = 0;
                                        MainPage.Possebilities_Log.Where(x => x.Number == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Number && x.Direction == flow && x.Grid == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Grid_Number && x.Level == MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Row_Number).First().Current_Field = MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First());
                                    }

                                    //Setzt die Zeit fest die gewartet wird um die aktuelle Zahl zusehen 
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
                    }
                }
            }
        }
    }
}
