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
    /// Ein Algorithmus der stupide alle möglichkeiten nacheinander durchgeht.
    /// </summary>
    public class BruteForce
    {
        public BruteForce() { }

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

            // Geht jeden Block nacheinander durch und füllt die Felder mit Zahlen
            for (int current_field_position = MainPage.field_position_marker; current_field_position < 81; current_field_position++)
            {
                //Überprüft ob aktuelles Feld das gespeichte Feld ist
                if (all_fields[current_field_position].Id >= MainPage.field_position_marker)
                {
                    // Wenn aktuelles Feld gelockt das gehe weiter
                    if (all_fields[current_field_position].Is_Locked == true)
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
                            if (all_fields[h].Is_Locked == true)
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
    }
}
