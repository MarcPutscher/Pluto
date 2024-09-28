using Pluto.Pages;
using Pluto.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pluto.Logic.Algorithmen
{
    public class BruteForce_Advanced
    {
        public BruteForce_Advanced() { }

        /// <summary>
        /// Der Hauptprozzes wo überprüft wird ob der Block korrekt ist. 
        /// </summary>
        public async Task<bool> MainProcess(CancellationToken token, MainPage mainPage)
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
                    if (new List<int>() { 1 + (i * 3), 2 + (i * 3), 3 + (i * 3) }.Contains(f.Grid_Number) == true)
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
                foreach (Field f in all_fields)
                {
                    if (new List<int>() { 1 + i, 4 + i, 7 + i }.Contains(f.Grid_Number) == true)
                    {
                        list.Add(f);
                    }
                }
                vertikal_blocks.Add(list);
            }

            //Ermittelt zuerst die Eindeutigen Zahlen
            for (bool playground_is_solved = false; playground_is_solved == false;)
            {
                await Task.Delay(1);

                //Wenn keine 0 mehr auf dem Spielfeld ist,dann ist das Spielfeld gelöst
                if (all_fields.Where(x => x.Number == 0).Count() == 0)
                {
                    playground_is_solved = true;
                    break;
                }

                //Erstellt einen Zähler der zählt wie viele Felder geändert wurden
                int counter = 0;

                //Geht die Blockslevel durch

                for (int i = 0; i < 3; i++)
                {
                    await Task.Delay(1);

                    if(await Check_Blocklevel_For_Clearly(all_fields, horizontal_blocks[i], "horizontal", token) == true)
                        counter++;

                    if(await Check_Blocklevel_For_Clearly(all_fields, vertikal_blocks[i], "vertikal", token) == true)
                        counter++;
                }


                //Wenn keine Felder geändert wurden anhalten
                if (counter == 0)
                {
                    break;
                }
            }

            // Geht jeden Block nacheinander durch und füllt die Felder mit Zahlen
            for (int current_field_position = MainPage.field_position_marker; current_field_position < 81; current_field_position++)
            {
                //Überprüft ob aktuelles Feld das gespeichte Feld ist
                if (all_fields[current_field_position].Id >= MainPage.field_position_marker)
                {
                    // Wenn aktuelles Feld gelockt das gehe weiter
                    if (all_fields[current_field_position].Is_Locked == true || all_fields[current_field_position].Is_Clearly == true)
                    {
                        continue;
                    }

                    //Setzt die Zahl des aktuellen Feldes auf Standard
                    all_fields[current_field_position].Number = 0;

                    //Setzt das gespeicherte Feld auf Standard
                    MainPage.field_position_marker = 0;

                    //Füllt das aktuelle Feld und gibt zurück ob das Füllen erfolgreich war
                    bool resutl = await Check_Field(token, current_field_position, all_fields);

                    //Wenn füllen nicht geklappt hat, dann vorhergehendes Feld anders füllen
                    if (resutl == false)
                    {
                        //Setzt die Anzeigen des aktuellen Feldes auf Standard
                        all_fields[current_field_position].Number = 0;
                        all_fields[current_field_position].Skips = 0;
                        all_fields[current_field_position].Is_Saturated = false;

                        //Ermittelt und legt das vorhergehende Feld fest im Block
                        int follower = current_field_position - 1;
                        for (int h = current_field_position - 1; h > -1; h--)
                        {
                            //Wenn das vorhergehende Feld gelockt ist dann gehe eins weiter zurück
                            if (all_fields[h].Is_Locked == true || all_fields[h].Is_Clearly == true)
                            {
                                follower--;
                                continue;
                            }

                            //Wenn der Skip vom vorhergehenden Feld nicht gesätigt ist dann halte an, wenn nicht dann setze den Skip und die Zahl des Feldes auf Standard
                            if (all_fields[h].Is_Saturated == false)
                            {
                                //Gibt das max von Skips in diesem Feld
                                List<bool> checklist = new List<bool>();
                                List<Field> faults = new List<Field>();
                                Field field = new Field() { Id = all_fields[h].Id, Number = all_fields[h].Number, Column_Number = all_fields[h].Column_Number, Row_Number = all_fields[h].Row_Number, Grid_Number = all_fields[h].Grid_Number, Is_Fault = all_fields[h].Is_Fault, Is_Select = all_fields[h].Is_Select, Number_Background_Color = all_fields[h].Number_Background_Color, Number_Color = all_fields[h].Number_Color, Visible_Number = all_fields[h].Visible_Number, Is_Locked = all_fields[h].Is_Locked };
                                int skipcounter = -1;
                                for (int k = 1; k < 10; k++)
                                {
                                    field.Number = k;

                                    (checklist, faults) = GameRules.Check_Rules_OneMove(all_fields, field);

                                    if (checklist.Contains(false) == false && faults.Count == 0)
                                    {
                                        skipcounter++;
                                    }
                                }

                                //Wenn Max Skip erreicht dann setze Skip auf gesättigt
                                if (skipcounter - all_fields[h].Skips <= 0)
                                {
                                    all_fields[h].Is_Saturated = true;
                                }

                                //Wenn Skip nicht gesättigt ist dann erhöhe die Skipanzahl, sonst gehe weiter und setze das Feld und den Skip auf Standard
                                if (all_fields[h].Is_Saturated == false)
                                {
                                    //Erhöhe den Skip des vorhergehenden Feldes 
                                    all_fields[h].Skips++;

                                    //Wenn Max Skip erreicht dann setze Skip auf gesättigt
                                    if (skipcounter - all_fields[h].Skips <= 0)
                                    {
                                        all_fields[h].Is_Saturated = true;
                                    }

                                    current_field_position = h - 1;

                                    mainPage.Attampts_Label++;
                                    break;
                                }
                                else
                                {
                                    //Wenn das erste Feld gesättigt ist und es keine möglichkeiten mehr gibt dann gib false zurück
                                    if (h == 0)
                                        return false;

                                    all_fields[h].Number = 0;
                                    all_fields[h].Skips = 0;
                                    all_fields[h].Is_Saturated = false;

                                    follower--;
                                    continue;
                                }
                            }
                            else
                            {
                                all_fields[h].Number = 0;
                                all_fields[h].Skips = 0;
                                all_fields[h].Is_Saturated = false;
                            }

                            follower--;
                        }
                    }
                }
            }

            //Setzt alle Speicherpunkte und Skipliste auf Standard 
            MainPage.field_position_marker = 0;
            MainPage.field_stop_position = 1;
            MainPage.block_stop_position = 0;
            return true;
        }

        /// <summary>
        /// Überprüft im Feld die Zahlen und ob sie korrekt sind
        /// </summary>
        public async Task<bool> Check_Field(CancellationToken token, int field_position, List<Field> all_fields)
        {
            List<bool> checklist = new List<bool>();
            List<Field> faults = new List<Field>();

            // Platzhalter Feld für das aktuelle Feld
            Field field = new Field() { Id = all_fields[field_position].Id, Number = all_fields[field_position].Number, Column_Number = all_fields[field_position].Column_Number, Row_Number = all_fields[field_position].Row_Number, Grid_Number = all_fields[field_position].Grid_Number, Is_Fault = all_fields[field_position].Is_Fault, Is_Select = all_fields[field_position].Is_Select, Number_Background_Color = all_fields[field_position].Number_Background_Color, Number_Color = all_fields[field_position].Number_Color, Visible_Number = all_fields[field_position].Visible_Number, Is_Locked = all_fields[field_position].Is_Locked };

            //Setzt die Skipanzahl für dieses Feld fest
            int skipiterration = MainPage.skip_stop_position;

            // Geht jede Zahl nacheinander durch
            for (int picked_number = MainPage.field_stop_position; picked_number < 10; picked_number++)
            {
                //Setzt den Nummerspeicher auf Standard
                MainPage.field_stop_position = 1;

                //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                if (token.IsCancellationRequested)
                {
                    MainPage.field_position_marker = all_fields[field_position].Id;
                    MainPage.field_stop_position = picked_number;
                    MainPage.block_stop_position = all_fields[field_position].Grid_Number;
                    MainPage.skip_stop_position = skipiterration;
                    token.ThrowIfCancellationRequested();
                    return false;
                }

                //Erhöht die Zahl im platzhalter Feld
                field.Number = field.Number + 1;

                //Überprüft ob diese Zahl im aktuellen Spielfeld erlaubt ist
                (checklist, faults) = GameRules.Check_Rules_OneMove(all_fields, field);

                //Zeigt die Zahl auf dem Feld in der UI
                all_fields[field_position].Number = field.Number;

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

                //Wenn alle Regeln korrekt sind und es keine Fehlerfelder gibt
                if (checklist.Contains(false) == false && faults.Count == 0)
                {
                    //Wenn alle Skipanzahlen die möglich sind erreicht sind, sond erhöhe die Skipanzahl um 1
                    if (skipiterration >= all_fields[field_position].Skips)
                    {
                        //Setzt den Skipanzahlspeicher auf Standard
                        MainPage.skip_stop_position = 0;
                        return true;
                    }
                    else
                    {
                        skipiterration++;

                        //Wenn die Skipanzahl größer ist als überhaut möglich
                        if (skipiterration > 8)
                        {
                            //Setzt den Skipanzahlspeicher auf Standard
                            MainPage.skip_stop_position = 0;
                            return false;
                        }
                    }
                }
            }

            //Setzt den Skipanzahlspeicher auf Standard
            MainPage.skip_stop_position = 0;
            return false;
        }

        /// <summary>
        /// Überprüft ob es in dem Blocks auf einem Level eine Zahl gibt die nur eine Möglichkeit besitzt und wenn ja dann wir dieses Zahl eingesetzt   
        /// </summary>
        public async Task<bool> Check_Blocklevel_For_Clearly(List<Field> all_fields, List<Field> blocks, string flow, CancellationToken token)
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
                if (blocks.Where(y => y.Number == i).Count() == 2)
                {
                    //Erstellt eine Liste von den Feldern die die gleiche Zahl haben und gelockt sind
                    List<Field> selectet_fields = blocks.Where(y => y.Number == i).ToList();

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

                    //Wenn für dieses Feld nur diese einzige Möglichkeit besteht
                    if (realy_possible.Count() == 1)
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

                        //Setzt die Zahl zum aktuellen Feld
                        selectet_field.Number = i;

                        //Setzt das Feld auf Einzige
                        selectet_field.Is_Clearly = true;

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
                        for(int p = 0;p<9;p++)
                        {
                            //Wenn der Block schon komplett ausgefüllt ist überspringe
                            if (MainPage.Fields[p].Where(x => x.Number == 0).Count() == 0)
                                continue;

                            //Geht die Zahlen durch
                            for(int z = 1; z<10;z++)
                            {
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
                                    MainPage.Fields[p].FirstOrDefault<Field>(x=>x.Id == realy_possible.First()).Number = z;

                                    //Setzt das Feld auf Einzige
                                    MainPage.Fields[p].FirstOrDefault<Field>(x => x.Id == realy_possible.First()).Is_Clearly = true;

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

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
