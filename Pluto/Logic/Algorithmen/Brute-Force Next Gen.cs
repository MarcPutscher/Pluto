using Pluto.Pages;
using Pluto.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core.Extensions;

namespace Pluto.Logic.Algorithmen
{
    public class Brute_Force_Next_Gen
    {
        public Brute_Force_Next_Gen() { }

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

            //Erstellt einen Strategienkontainer
            Strategy strategy = new Strategy(all_fields, token);

            //Ermittelt die möglichkeiten zu Beginn
            if (MainPage.started == false)
            {
                MainPage.started = true;
                 await Set_Possebilities_And_Denail(all_fields);
            }

            bool is_finished = false;
            while (is_finished == false)
            {
                if(!all_fields.Any(x=>x.Number == 0))
                    is_finished = true;

                //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                    return false;
                }

                switch(MainPage.next_gen_marker)
                {
                    case 0:
                        {
                            MainPage.next_gen_marker = 1;
                            //Ermittle Hidden Single
                            await strategy.Hidden_Single(0);

                            MainPage.next_gen_marker = 2;
                            //Ermittle Naked Single
                            await strategy.Naked_Single(0);

                            MainPage.next_gen_marker = 3;
                            //Ermittle Hidden Trible
                            await strategy.Hidden_Trible();

                            MainPage.next_gen_marker = 4;
                            //Ermittle Naked Trible
                            await strategy.Naked_Trible();

                            MainPage.next_gen_marker = 5;
                            ////Ermittle Hidden Pair 
                            await strategy.Hidden_Pair();

                            MainPage.next_gen_marker = 6;
                            //Ermittle Naked Pair
                            await strategy.Naked_Pair();

                            //MainPage.next_gen_marker = 7;
                            ////Ermittle Locked Candidates Typ 1
                            //await strategy.Locked_Candidates_Typ_1();

                            //MainPage.next_gen_marker = 8;
                            ////Ermittle Locked Candidates Typ 2
                            //await strategy.Locked_Candidates_Typ_2();

                            MainPage.next_gen_marker = 0;
                            break;
                        }

                    case 1:
                        //Ermittle Hidden Single
                        await strategy.Hidden_Single(0);
                        MainPage.next_gen_marker = 0;
                        break;

                    case 2:
                        //Ermittle Naked Single
                        await strategy.Naked_Single(0);
                        MainPage.next_gen_marker = 0;
                        break;

                    case 3:
                    //Ermittle Hidden Trible
                    await strategy.Hidden_Trible();
                    MainPage.next_gen_marker = 0;
                    break;

                    case 4:
                        //Ermittle Naked Trible
                        await strategy.Naked_Trible();
                        MainPage.next_gen_marker = 0;
                        break;

                    case 5:
                    ////Ermittle Hidden Pair 
                    await strategy.Hidden_Pair();
                    MainPage.next_gen_marker = 0;
                    break;

                    case 6:
                    //Ermittle Naked Pair
                    await strategy.Naked_Pair();
                    MainPage.next_gen_marker = 0;
                    break;

                    case 7:
                    //Ermittle Locked Candidates Typ 1
                    await strategy.Locked_Candidates_Typ_1();
                    MainPage.next_gen_marker = 0;
                    break;

                    case 8:
                    //Ermittle Locked Candidates Typ 2
                    await strategy.Locked_Candidates_Typ_2();
                    MainPage.next_gen_marker = 0;
                    break;
                }

                await Task.Delay(1);

                mainPage.Attampts_Label++;
            }

            return true;
        }

        ///// <summary>
        ///// Überprüft im Feld die Zahlen und ob sie korrekt sind
        ///// </summary>
        //public async Task<bool> Check_Field(CancellationToken token, int field_position, List<Field> all_fields)
        //{
        //    List<bool> checklist = new List<bool>();
        //    List<Field> faults = new List<Field>();

        //    // Platzhalter Feld für das aktuelle Feld
        //    Field field = new Field() { Id = all_fields[field_position].Id, Number = all_fields[field_position].Number, Column_Number = all_fields[field_position].Column_Number, Row_Number = all_fields[field_position].Row_Number, Grid_Number = all_fields[field_position].Grid_Number, Is_Fault = all_fields[field_position].Is_Fault, Is_Select = all_fields[field_position].Is_Select, Number_Background_Color = all_fields[field_position].Number_Background_Color, Number_Color = all_fields[field_position].Number_Color, Visible_Number = all_fields[field_position].Visible_Number, Is_Locked = all_fields[field_position].Is_Locked };

        //    //Setzt die Skipanzahl für dieses Feld fest
        //    int skipiterration = MainPage.skip_stop_position;

        //    // Geht jede Zahl nacheinander durch
        //    for (int picked_number = MainPage.field_stop_position; picked_number < 10; picked_number++)
        //    {
        //        //Setzt den Nummerspeicher auf Standard
        //        MainPage.field_stop_position = 1;

        //        //Wenn der Prozess abgebrochen wird, setzt alle Markierungen auf die aktuellen Werte
        //        if (token.IsCancellationRequested)
        //        {
        //            MainPage.field_position_marker = all_fields[field_position].Id;
        //            MainPage.field_stop_position = picked_number;
        //            MainPage.skip_stop_position = skipiterration;
        //            token.ThrowIfCancellationRequested();
        //            return false;
        //        }

        //        //Erhöht die Zahl im platzhalter Feld
        //        field.Number = field.Number + 1;

        //        //Überprüft ob diese Zahl in diesem Feld überhaupt möglich ist
        //        if (MainPage.Denails.Where(x => x.Number == field.Number && x.Fields.Contains(field) == true).Any())
        //            continue;

        //        //Überprüft ob diese Zahl im aktuellen Spielfeld erlaubt ist
        //        (checklist, faults) = GameRules.Check_Rules_OneMove(all_fields, field);

        //        //Zeigt die Zahl auf dem Feld in der UI
        //        all_fields[field_position].Number = field.Number;

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

        //        //Wenn alle Regeln korrekt sind und es keine Fehlerfelder gibt
        //        if (checklist.Contains(false) == false && faults.Count == 0)
        //        {
        //            //Wenn alle Skipanzahlen die möglich sind erreicht sind, sond erhöhe die Skipanzahl um 1
        //            if (skipiterration >= all_fields[field_position].Skips)
        //            {
        //                //Setzt den Skipanzahlspeicher auf Standard
        //                MainPage.skip_stop_position = 0;
        //                return true;
        //            }
        //            else
        //            {
        //                skipiterration++;

        //                //Wenn die Skipanzahl größer ist als überhaut möglich
        //                if (skipiterration > 8)
        //                {
        //                    //Setzt den Skipanzahlspeicher auf Standard
        //                    MainPage.skip_stop_position = 0;
        //                    return false;
        //                }
        //            }
        //        }
        //    }

        //    //Setzt den Skipanzahlspeicher auf Standard
        //    MainPage.skip_stop_position = 0;
        //    return false;
        //}

        /// <summary>
        /// Ermittelt für jedes Feld die Möglichkeiten und Verweigerer
        /// </summary>
        public async Task Set_Possebilities_And_Denail(List<Field> all_fields)
        {
            foreach (Field field in all_fields)
            {
                await Task.Delay(1);

                // Wenn das aktuelle Feld gelockt, eindeutig oder semi-eindeutig dann überspringe
                if (field.Is_Clearly == true || field.Is_Semi_Clearly == true || field.Is_Locked == true)
                    continue;

                //Erstellt alle Möglichkieten in der UI
                for (int j = 1; j < 10; j++)
                {
                    field.Possebilities.Add(j);
                }

                //Geht die Zahlen in dem Feld durch
                for (int i = 1; i < 10; i++)
                {
                    // Platzhalter Feld für das aktuelle Feld
                    Field Placeholder = new Field() { Id = field.Id, Number = i, Column_Number = field.Column_Number, Row_Number = field.Row_Number, Grid_Number = field.Grid_Number, Is_Fault = field.Is_Fault, Is_Select = field.Is_Select, Number_Background_Color = field.Number_Background_Color, Number_Color = field.Number_Color, Visible_Number = field.Visible_Number, Is_Locked = field.Is_Locked };

                    //Überprüft ob diese Zahl im aktuellen Spielfeld erlaubt ist
                    List<bool> checklist = new List<bool>();
                    List<Field> faults = new List<Field>();
                    (checklist, faults) = GameRules.Check_Rules_OneMove(all_fields, Placeholder);

                    //Wenn alle Regeln korrekt sind und es keine Fehlerfelder gibt
                    if (checklist.Contains(false) == true && faults.Count != 0)
                    {
                        field.Possebilities.Remove(i);
                    }
                }
            }
        }
    }
}
