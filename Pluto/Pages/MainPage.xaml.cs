using CommunityToolkit.Maui.Views;
using Pluto.Pages;
using Pluto.Models;
using System.Collections.ObjectModel;
using Field = Pluto.Models.Field;
using MvvmHelpers;
using CommunityToolkit.Maui.Core.Extensions;
using System.Collections.Specialized;
using System.ComponentModel;
using Pluto.Logic;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Pluto.Service;
using System.Diagnostics;

namespace Pluto.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            this.BindingContext = this;

            InitializeComponent();

            //int id_follower = 0;
            //int row_follower = 1;
            //int column_follower = 1;
            //int count = 1;
            //for (int i = 1; i < 10; i++)
            //{
            //    if (i == 1 || i == 2 || i == 3)
            //    {
            //        row_follower = 1;
            //        count = 1;
            //    }
            //    if (i == 4 || i == 5 || i == 6)
            //    {
            //        row_follower = 4;
            //        count = 2;
            //    }
            //    if (i == 7 || i == 8 || i == 9)
            //    {
            //        row_follower = 7;
            //        count = 3;
            //    }
            //    if (i == 1 || i == 4 || i == 7)
            //    {
            //        column_follower = 1;
            //    }
            //    if (i == 2 || i == 5 || i == 8)
            //    {
            //        column_follower = 4;
            //    }
            //    if (i == 3 || i == 6 || i == 9)
            //    {
            //        column_follower = 7;
            //    }

            //    List<Field> list = new List<Field>();
            //    for (int j = 1; j < 10; j++)
            //    {
            //        if (row_follower > 3*count)
            //        {
            //            row_follower = 3*count -2;
            //            column_follower++;
            //        }


            //        list.Add(new Field() { Id = id_follower, Grid_Number = i, Row_Number = row_follower, Column_Number = column_follower});
            //        id_follower++;
            //        row_follower++;
            //    }

            //    Fields.Add(list.ToObservableCollection());
            //}


            int id_follower = 0;
            int row_follower = 1;
            int grid_follower = 0;
            int column_follower = 1;
            int count = 1;
            for (int i = 1; i < 10; i++)
            {
                if(i == 1)
                { row_follower = 1; column_follower = 1; count = 1;}

                if (i == 2)
                { row_follower = 1; column_follower = 4; count = 1; }

                if (i == 3)
                { row_follower = 1; column_follower = 7; count = 1; }

                if (i == 4)
                { row_follower = 4; column_follower = 7; count = 2; grid_follower = 2; }

                if (i == 5)
                { row_follower = 4; column_follower = 4; count = 2; grid_follower = 0; }

                if (i == 6)
                { row_follower = 4; column_follower = 1; count = 2; grid_follower = -2; }

                if (i == 7)
                { row_follower = 7; column_follower = 1; count = 3; grid_follower = 0; }

                if (i == 8)
                { row_follower = 7; column_follower = 4; count = 3; grid_follower = 0; }

                if (i == 9)
                { row_follower = 7; column_follower = 7; count = 3; grid_follower = 0; }

                List<Field> list = new List<Field>();
                for (int j = 1; j < 10; j++)
                {
                    if (row_follower > 3 * count)
                    {
                        row_follower = 3 * count - 2;
                        column_follower++;
                    }


                    list.Add(new Field() { Id = id_follower,  Grid_Number = i+grid_follower, Real_Grid_Number = i, Row_Number = row_follower, Column_Number = column_follower });
                    id_follower++;
                    row_follower++;
                }

                Fields.Add(list.ToObservableCollection());
            }


            Grid1.ItemsSource = Fields[0];
            Grid2.ItemsSource = Fields[1];
            Grid3.ItemsSource = Fields[2];
            Grid4.ItemsSource = Fields[5];
            Grid5.ItemsSource = Fields[4];
            Grid6.ItemsSource = Fields[3];
            Grid7.ItemsSource = Fields[6];
            Grid8.ItemsSource = Fields[7];
            Grid9.ItemsSource = Fields[8];

            for (int i = 1; i < 10; i++)
            {
                Numbers.Add(new Number() { Visible_Number = i.ToString() });
            }

            number_collectionview.ItemsSource = Numbers;

        }

        public async void Field_Tapped(object sender, TappedEventArgs e)
        {
            if (Status_Solver == "Arbeitet")
                return;

            Field? current = (sender as StackLayout)?.BindingContext as Field ;

            if (current == null)
                return;

            if (current.Is_Locked == true)
                return;

            if(current.Is_Select == false)
            {
                if (current_Field == new Field())
                {
                    current_Field = current;
                }
                else
                {
                    if(current_Field != current)
                    {
                        previous_Field = current_Field;
                    }
                    current_Field = current;
                }

                current.Is_Select = !current.Is_Select;
                current.Was_Manuel_Select = true;

                if (previous_Field != new Field())
                {
                    previous_Field.Is_Select = false;
                }

                if (Numbers.Where(x => x.Visible_Number == current.Visible_Number).FirstOrDefault() != null)
                {
                    Numbers.Where(x => x.Visible_Number == current.Visible_Number).First().Is_Select = true;

                    foreach (Number n in Numbers)
                    {
                        if (n.Visible_Number != current.Visible_Number)
                        {
                            n.Is_Select = false;
                        }
                    }
                }
                else
                {
                    foreach (Number n in Numbers)
                    {
                        n.Is_Select = false;
                    }
                }

                if (current.Visible_Number == string.Empty)
                {
                    foreach (Number n in Numbers)
                    {
                        n.Is_Select = false;
                    }
                }

                if(settings.IsVisible == true)
                {
                    var a3 = settings.FadeTo(0, 500, Easing.Linear);
                    var a4 = settings.TranslateTo(0, -30, 500, Easing.Linear);

                    await Task.WhenAll(a3, a4);

                    settings.IsVisible = false;
                }

                number_collectionview.IsVisible = true;

                var a1 = number_collectionview.FadeTo(1, 500, Easing.Linear);
                var a2 = number_collectionview.TranslateTo(0, -30, 500, Easing.Linear);

                await Task.WhenAll(a1, a2);
                numberGrid = true;
            }
            else
            {
                current.Is_Select = false;
                current.Was_Manuel_Select = false;
                var a1 = number_collectionview.FadeTo(0, 500, Easing.Linear);
                var a2 = number_collectionview.TranslateTo(0, 30, 500, Easing.Linear);
                await Task.WhenAll(a1, a2);
                number_collectionview.IsVisible = false;
                numberGrid = false;
            }
        }
        public async void Number_Tapped(object sender, TappedEventArgs e)
        {
            List<Field> fields = new List<Field>();
            List<Field> current_faults = new List<Field>();
            List<Boolean> checklist = new List<bool>();
            List<Field> faults = new List<Field>();

            try
            {
                Number? current = (sender as Frame)?.BindingContext as Number;

                if (current == null)
                    return;

                current.Is_Select = !current.Is_Select;

                if (Fields[current_Field.Real_Grid_Number - 1].Where(x => x == current_Field).FirstOrDefault() != null)
                {
                    int showednumber = 0;
                    if (current.Is_Select == true)
                    {
                        showednumber = int.Parse(current.Visible_Number);
                    }

                    Fields[current_Field.Real_Grid_Number - 1].Where(x => x == current_Field).First().Number = showednumber;
                }

                foreach (Number n in Numbers)
                {
                    if (n.Visible_Number != current.Visible_Number)
                    {
                        n.Is_Select = false;
                    }
                }

                foreach (ObservableCollection<Field> lf in Fields)
                {
                    foreach (Field f in lf)
                    {
                        fields.Add(f);
                        if (f.Is_Fault == true)
                        {
                            current_faults.Add(f);
                        }
                    }
                }

                if (current_faults.Count == 0)
                {
                    (checklist, faults) = GameRules.Check_Rules_OneMove(fields, current_Field);
                }
                else
                {
                    (checklist, faults) = GameRules.Check_Rules_All(fields);
                }

                if (current_faults.Count != 0)
                {
                    foreach (Field f in current_faults)
                    {
                        if (faults.Where(x => x.Id == f.Id).Count() == 0)
                        {
                            foreach (ObservableCollection<Field> fs in Fields)
                            {
                                if (fs.Contains(f))
                                {
                                    fs.Where(x => x.Id == f.Id).First().Is_Fault = false;
                                }
                            }
                        }
                    }
                }

                if (faults.Count == 0)
                {
                    return;
                }

                if (checklist.Contains(false) == true)
                {
                    foreach (Field f in faults)
                    {
                        foreach (ObservableCollection<Field> fs in Fields)
                        {
                            if (fs.Contains(f))
                            {
                                fs.Where(x => x.Id == f.Id).First().Is_Fault = true;
                            }
                        }
                    }
                    if (faults.Where(y => y.Number == current_Field.Number).Count() != 0)
                    {
                        current_Field.Is_Fault = true;
                    }
                    else
                    {
                        current_Field.Is_Fault = false;
                    }
                }
                else
                {
                    foreach (Field f in faults)
                    {
                        foreach (ObservableCollection<Field> fs in Fields)
                        {
                            if (fs.Contains(f))
                            {
                                fs.Where(x => x.Id == f.Id).First().Is_Fault = false;
                            }
                        }
                    }
                    current_Field.Is_Fault = false;
                }
            }
            finally
            {
                foreach (ObservableCollection<Field> lf in Fields)
                {
                    foreach (Field f in lf)
                    {
                        fields.Add(f);
                    }
                }

                if (fields.Where(x=>x.Number != 0).Count() != 0 && faults.Count == 0 && checklist.Contains(false) == false)
                {
                    Spielfeldoptionen.IsVisible = true;

                    var a1 = Spielfeldoptionen.FadeTo(1, 500, Easing.Linear);
                    var a2 = Spielfeldoptionen.TranslateTo(0, 30, 500, Easing.Linear);

                    await Task.WhenAll(a1, a2);

                    Create_Button_IsEnabled = true;
                    Delete_Button_IsEnabled = true;
                    playoption = true;
                }
                else
                {
                    var a1 = Spielfeldoptionen.FadeTo(0, 500, Easing.Linear);
                    var a2 = Spielfeldoptionen.TranslateTo(0, -30, 500, Easing.Linear);

                    await Task.WhenAll(a1, a2);
                    Spielfeldoptionen.IsVisible = false;
                    Create_Button_IsEnabled = false;
                    Delete_Button_IsEnabled = false;
                    playoption = false;
                }
            }
        }


        public async void Create_Tapped(object sender, TappedEventArgs e)
        {
            if (Fields.Count == 9 && Fields[0].Count == 9)
            {
                Locked_Fields.Clear();

                foreach (ObservableCollection<Field> fs in Fields)
                {
                    foreach (Field f in fs)
                    {
                        if(f.Number != 0 && f.Is_Fault == false && f.Was_Manuel_Select == true)
                        {
                            f.Is_Select = false;
                            f.Is_Locked = true;
                            Locked_Fields.Add(f);
                        }
                    }
                }

                Create_Button_IsEnabled = false;
                Delete_Button_IsEnabled = true;
                Edit_Button_IsEnabled = true;

                Status_Solver = "Undefiniert";
                Algorithmus_Label = string.Empty;
                algorithmuslist.Opacity = 1;

                if (numberGrid == true)
                {
                    current_Field.Is_Select = false;
                    current_Field.Was_Manuel_Select = false;
                    var a1 = number_collectionview.FadeTo(0, 500, Easing.Linear);
                    var a2 = number_collectionview.TranslateTo(0, 30, 500, Easing.Linear);
                    await Task.WhenAll(a1, a2);
                    number_collectionview.IsVisible = false;
                    numberGrid = false;
                }
            }
        }
        public async void Delete_Tapped(object sender, TappedEventArgs e)
        {
            if (Fields.Count == 9 && Fields[0].Count == 9)
            {
                tokensource?.Cancel();

                foreach (ObservableCollection<Field> fs in Fields)
                {
                    foreach (Field f in fs)
                    {
                        f.Is_Locked = false;
                        f.Is_Select = false;
                        f.Is_Fault = false;
                        f.Number = 0;
                        f.Skips = 0;
                        f.Is_Saturated = false;
                        //f.Placeholder_Number_Horizontal = new ObservableCollection<int>();
                        //f.Placeholder_Number_Vertikal = new ObservableCollection<int>();
                        f.Is_Clearly = false;
                        f.Is_Semi_Clearly = false;
                        f.Possebilities.Clear();
                    }
                }

                var a1 = Spielfeldoptionen.FadeTo(0, 500, Easing.Linear);
                var a2 = Spielfeldoptionen.TranslateTo(0, -30, 500, Easing.Linear);

                var a3 = number_collectionview.FadeTo(0, 500, Easing.Linear);
                var a4 = number_collectionview.TranslateTo(0, 30, 500, Easing.Linear);

                await Task.WhenAll(a1, a2, a3,a4);

                Status_Solver = "Wartet";
                Difficulty = null;

                field_position_marker =  0 ;
                field_stop_position = 1;
                skip_stop_position = 0;
                Possebilities_Log.Clear();
                Denails.Clear();
                Attampts_Label = 0;
                started = false;

                number_collectionview.IsVisible = false;
                Spielfeldoptionen.IsVisible = false;
                Create_Button_IsEnabled = false;
                Delete_Button_IsEnabled = false;
                Edit_Button_IsEnabled = false;
                playoption = false;
            }
        }
        public void Edit_Tapped(object sender, TappedEventArgs e)
        {
            if (Locked_Fields.Count != 0)
            {
                foreach (ObservableCollection<Field> fs in Fields)
                {
                    foreach (Field f in fs)
                    {
                        if (Locked_Fields.Where(x=>x.Id == f.Id).Count() != 0)
                        {
                            f.Is_Locked = false;
                            Locked_Fields.Remove(f);
                        }
                    }
                }

                Status_Solver = "Wartet";

                Create_Button_IsEnabled = true;
                Delete_Button_IsEnabled = true;
                Edit_Button_IsEnabled = false;
            }
        }


        private void Algorithmus_Clicked(object sender, TappedEventArgs e)
        {
            try
            {
                if (sender is Frame)
                {
                    Algorithmus_Label = (sender as Frame).BindingContext.ToString();

                    if (Algorithmus_Label == string.Empty)
                        return;

                    algorithmuslist.FadeTo(0, 300);

                    Status_Solver = "Bereit";
                }
            }
            catch (Exception ex)
            {
            }
        }


        public void Cancel_Tapped(object sender, TappedEventArgs e)
        {
            tokensource?.Cancel();
        }
        public async void Continue_Tapped(object sender, TappedEventArgs e)
        {
            if (numberGrid == true)
            {
                current_Field.Is_Select = false;
                current_Field.Was_Manuel_Select = false;
                var a1 = number_collectionview.FadeTo(0, 500, Easing.Linear);
                var a2 = number_collectionview.TranslateTo(0, 30, 500, Easing.Linear);
                await Task.WhenAll(a1, a2);
                number_collectionview.IsVisible = false;
                numberGrid = false;
            }

            await Start_Solving();
        }
        public async void Restart_Tapped(object sender, TappedEventArgs e)
        {
            if (Fields.Count == 9 && Fields[0].Count == 9)
            {
                foreach (ObservableCollection<Field> fs in Fields)
                {
                    foreach (Field f in fs)
                    {
                        if(f.Is_Locked == false)
                        {
                            f.Is_Locked = false;
                            f.Is_Select = false;
                            f.Is_Fault = false;
                            f.Number = 0;
                            f.Skips = 0;
                            f.Is_Saturated = false;
                            //f.Placeholder_Number_Horizontal = new ObservableCollection<int>();
                            //f.Placeholder_Number_Vertikal = new ObservableCollection<int>();
                            f.Is_Clearly = false;
                            f.Is_Semi_Clearly = false;
                        }
                    }
                }

                field_position_marker = 0;
                field_stop_position = 1;
                skip_stop_position = 0;
                Possebilities_Log = new List<Possebilitie>();
                Attampts_Label = 0;
                started = false;

                if (numberGrid == true)
                {
                    current_Field.Is_Select = false;
                    current_Field.Was_Manuel_Select = false;
                    var a1 = number_collectionview.FadeTo(0, 500, Easing.Linear);
                    var a2 = number_collectionview.TranslateTo(0, 30, 500, Easing.Linear);
                    await Task.WhenAll(a1, a2);
                    number_collectionview.IsVisible = false;
                    numberGrid = false;
                }

                stopwatch = null;

                await Start_Solving();
            }
        }
        public async void Reset_Tapped(object sender, TappedEventArgs e)
        {
            Status_Solver = "Bereit";

            tokensource?.Cancel();

            if (Fields.Count == 9 && Fields[0].Count == 9)
            {
                foreach (ObservableCollection<Field> fs in Fields)
                {
                    foreach (Field f in fs)
                    {
                        if (f.Is_Locked == false)
                        {
                            f.Is_Locked = false;
                            f.Is_Select = false;
                            f.Is_Fault = false;
                            f.Skips = 0;
                            f.Is_Saturated = false;
                            f.Number = 0;
                            //f.Placeholder_Number_Horizontal = new ObservableCollection<int>();
                            //f.Placeholder_Number_Vertikal = new ObservableCollection<int>();
                            f.Is_Clearly = false;
                            f.Possebilities.Clear();
                            f.Is_Semi_Clearly = false;
                        }
                    }
                }


                if (numberGrid == true)
                {
                    current_Field.Is_Select = false;
                    current_Field.Was_Manuel_Select = false;
                    var a1 = number_collectionview.FadeTo(0, 500, Easing.Linear);
                    var a2 = number_collectionview.TranslateTo(0, 30, 500, Easing.Linear);
                    await Task.WhenAll(a1, a2);
                    number_collectionview.IsVisible = false;
                    numberGrid = false;
                }

                stopwatch = null;
                field_position_marker = 0;
                field_stop_position = 1;
                skip_stop_position = 0;
                Possebilities_Log = new List<Possebilitie>();
                Attampts_Label = 0;
                started = false;
            }
        }


        public void Logs_Proberty_Cahnged(Logdata logdata)
        {
            if(Logs.Any(x=>x.Recorded_Field.Id== logdata.Recorded_Field.Id) == true || Logs.Any(y=>y.Grid_Number == logdata.Grid_Number))
            {

            }
            else
            {

            }

            //loglist.ItemsSource = Logs;

        }


        public async Task Start_Solving()
        {
            tokensource = new CancellationTokenSource();

            var token = tokensource.Token;

            try
            {
                if(Algorithmus_Label == "")
                    return;

                Status_Solver = "Arbeitet";

                bool result = await solver.Process(Algorithmus_Label,token, this);

                if (result == true)
                {
                    Status_Solver = "Fertig";
                    await Successfully_Solved();
                }
                else
                {
                    Status_Solver = "Ungelöst";
                }

            }
            catch (OperationCanceledException ex)
            {
                if(Status_Solver == "Bereit")
                {
                    field_position_marker = 0;
                    field_stop_position = 1;
                    skip_stop_position = 0;
                    Possebilities_Log = new List<Possebilitie>();
                    Attampts_Label = 0;
                    started = false;

                    Timer_Label = string.Empty;
                }
                else
                {
                    Status_Solver = "Abgebrochen";
                }
            }
            finally
            {
                tokensource.Dispose();
                tokensource = null;
            }
        }
        public async Task Successfully_Solved()
        {
            for(int i = 0; i < 5; i++)
            {
                List<Field> fields = new List<Field>();
                int follower = 0;   
                foreach (ObservableCollection<Field> fs in Fields)
                {
                    if(i == 0 )
                    {
                        fields.Add(fs[0]);
                    }
                    if (i == 1)
                    {
                        fields.AddRange([fs[1], fs[3]]);
                    }
                    if (i == 2)
                    {
                        fields.AddRange([fs[2], fs[4],fs[6]]);
                    }
                    if (i == 3)
                    {
                        fields.AddRange([fs[5], fs[7]]);
                    }
                    if (i == 4)
                    {
                        fields.Add(fs[8]);
                    }
                }

                List<string> colorlist = new List<string>();
                foreach (Field f in fields)
                {
                    colorlist.Add(f.Number_Background_Color.ToHex());
                    f.Number_Background_Color = Color.FromArgb("#006453");
                }

                await Task.Delay(200);

                follower = 0;
                foreach (Field f in fields)
                {
                    f.Number_Background_Color = Color.FromArgb(colorlist[follower]);
                    follower++;
                }
            }

            int follower1 = 0;
            List<string> colorlist1 = new List<string>();
            foreach (ObservableCollection<Field> fs in Fields)
            {
                foreach (Field f in fs)
                {
                    colorlist1.Add(f.Number_Background_Color.ToHex());
                    f.Number_Background_Color = Color.FromArgb("#006453");
                }
            }
            await Task.Delay(300);
            foreach (ObservableCollection<Field> fs in Fields)
            {
                foreach (Field f in fs)
                {
                    f.Number_Background_Color = Color.FromArgb(colorlist1[follower1]);
                    follower1++;
                }
            }
        }


        private async void Settings_Clicked(object sender, EventArgs e)
        {
            if (number_collectionview.IsVisible == true)
            {
                var a1 = number_collectionview.FadeTo(0, 500, Easing.Linear);
                var a2 = number_collectionview.TranslateTo(0, 30, 500, Easing.Linear);
                await Task.WhenAll(a1, a2);
                number_collectionview.IsVisible = false;
            }
            if (Spielfeldoptionen.IsVisible == true)
            {
                var a1 = Spielfeldoptionen.FadeTo(0, 500, Easing.Linear);
                var a2 = Spielfeldoptionen.TranslateTo(0, -30, 500, Easing.Linear);

                await Task.WhenAll(a1, a2);
                Spielfeldoptionen.IsVisible = false;
                Create_Button_IsEnabled = false;
                Delete_Button_IsEnabled = false;
            }


            if (settings.IsVisible == false)
            {
                tokensource?.Cancel();

                if(Status_Solver == "Arbeitet")
                {
                    Status_Solver = "Abgebrochen";
                }

                settings.IsVisible = true;

                var a1 = settings.FadeTo(1, 500, Easing.Linear);
                var a2 = settings.TranslateTo(0, 30, 500, Easing.Linear);

                playgroundlist.IsVisible = true;
                var a3 = playgroundlist.FadeTo(1, 300);
                var a4 = difficultylist.FadeTo(0, 300);
                await Task.WhenAll(a1, a2,a3,a4);
                difficultylist.IsVisible = false;


                ObservableCollection<Playground> playgrounds = await PlaygroundService.Get_all_Playgrounds_in_ObservableCollection();
                if (playgrounds.Count != 0)
                {
                    playgroundlist.ItemsSource = playgrounds;
                }

                Placholder_Fields.Clear();
                foreach (ObservableCollection<Field> fs in Fields)
                {
                    foreach (Field f in fs)
                    {
                        Placholder_Fields.Add(new Field()
                        {
                            Id = f.Id,
                            Number = f.Number,
                            Grid_Number = f.Grid_Number,
                            Real_Grid_Number = f.Real_Grid_Number,
                            Row_Number = f.Row_Number,
                            Column_Number = f.Column_Number,
                            Is_Fault = f.Is_Fault,
                            Is_Locked = f.Is_Locked,
                            Is_Select = false,
                            Number_Background_Color = f.Number_Background_Color,
                            Number_Color = f.Number_Color,
                            Visible_Number = f.Visible_Number,
                            Was_Manuel_Select = f.Was_Manuel_Select,
                        });
                    }
                }

                if (current_Field != new Field())
                {
                    current_Field.Is_Select = false;
                    current_Field.Was_Manuel_Select = false;
                    current_Field = new Field();
                }
                if (previous_Field != new Field())
                {
                    previous_Field.Is_Select = false;
                    previous_Field.Was_Manuel_Select = false;
                    previous_Field = new Field();
                }
            }
            else
            {
                int follower = 0;
                for (int g = 0; g < 9; g++)
                {
                    for (int f = 0; f < 9; f++)
                    {
                        Fields[g][f] = Placholder_Fields[follower];
                        follower++;
                    }
                }

                var a1 = settings.FadeTo(0, 500, Easing.Linear);
                var a2 = settings.TranslateTo(0, -30, 500, Easing.Linear);

                await Task.WhenAll(a1, a2);

                settings.IsVisible = false;
            }

            if(playoption == true && settings.IsVisible == false)
            {
                Spielfeldoptionen.IsVisible = true;

                var a1 = Spielfeldoptionen.FadeTo(1, 500, Easing.Linear);
                var a2 = Spielfeldoptionen.TranslateTo(0, 30, 500, Easing.Linear);

                await Task.WhenAll(a1, a2);

                Create_Button_IsEnabled = true;
                Delete_Button_IsEnabled = true;
            }
        }
        private async void Save_Playground__Clicked(object sender, EventArgs e)
        {


            Playground playground = new Playground();



            List<int> ints = new List<int>();
            int count = 0;
            foreach(ObservableCollection<Field> fs in Fields)
            {
                foreach(Field f in fs)
                {
                    ints.Add(f.Number);
                    if(f.Is_Fault == true)
                        return;
                    if (f.Number != 0)
                    {
                        count++;
                    }
                }
            }
            if (count == 0)
                return;

            if (sender is Frame)
            {
                Difficulty = (sender as Frame).BindingContext.ToString();

                playgroundlist.IsVisible = true;
                var a1 = playgroundlist.FadeTo(1, 300);
                var a2 = difficultylist.FadeTo(0, 300);
                await Task.WhenAll(a1, a2);
                difficultylist.IsVisible = false;
            }
            else
            {
                Difficulty = null;
                difficultylist.IsVisible = true;
                var a1 = playgroundlist.FadeTo(0, 300);
                var a2 = difficultylist.FadeTo(1, 300);
                await Task.WhenAll(a1, a2);
                playgroundlist.IsVisible = false;
                return;
            }

            playground.Difficulty = Difficulty;

            playground.Field_Numbers = new Playground_Field_Numbers_Converter().Serialize(ints);

            playground.Field_With_Numbers_Count = count;

            int result = await PlaygroundService.Add_Playground(playground);
            if(result != 0)
                return;

            playgroundlist.ItemsSource = await PlaygroundService.Get_all_Playgrounds_in_ObservableCollection();
        }
        private async void Load_Playground__Clicked(object sender, EventArgs e)
        {
            if(playgroundlist.SelectedItem != null && playgroundlist.SelectedItem is Playground)
            {
                Create_Button_IsEnabled = false;
                Delete_Button_IsEnabled = true;
                Edit_Button_IsEnabled = true;

                field_position_marker = 0;
                field_stop_position = 1;
                skip_stop_position = 0;
                Possebilities_Log.Clear();
                Denails.Clear();
                Attampts_Label = 0;
                started = false;

                Difficulty = (playgroundlist.SelectedItem as Playground).Difficulty;
                Status_Solver = "Undefiniert";
                algorithmuslist.Opacity = 1;
                Algorithmus_Label = string.Empty;

                var a1 = settings.FadeTo(0, 500, Easing.Linear);
                var a2 = settings.TranslateTo(0, -30, 500, Easing.Linear);

                await Task.WhenAll(a1, a2);

                settings.IsVisible = false;

                if (current_Field != new Field())
                {
                    current_Field = new Field();
                    previous_Field = new Field();
                }
                numberGrid = false;
                playoption = true;

                Spielfeldoptionen.IsVisible = true;

                var a3 = Spielfeldoptionen.FadeTo(1, 500, Easing.Linear);
                var a4 = Spielfeldoptionen.TranslateTo(0, 30, 500, Easing.Linear);

                await Task.WhenAll(a3, a4);
            }
        }
        private async void Remove_Playground__Clicked(object sender, EventArgs e)
        {
            if(playgroundlist.SelectedItem != null && playgroundlist.SelectedItem is Playground)
            {
                bool ressult = await PlaygroundService.Remove_Playground((Playground)playgroundlist.SelectedItem);

                if(ressult == false)
                {
                    playgroundlist.ItemsSource=null;
                }
                else
                {
                    playgroundlist.ItemsSource = await PlaygroundService.Get_all_Playgrounds_in_ObservableCollection();
                }

                remove_playgroundButton.IsVisible = false;
                load_playgroundButton.IsVisible = false;
            }
        }
        private void Playground_Clicked(object sender, TappedEventArgs e)
        {
            try
            {
                Playground? playground = (sender as StackLayout)?.BindingContext as Playground;

                if (playground != null)
                {
                    if (playground.Is_Select == true)
                    {
                        playground.Is_Select = false;
                        remove_playgroundButton.IsVisible = false;
                        load_playgroundButton.IsVisible = false;

                        int follower = 0;
                        for (int g = 0; g < 9; g++)
                        {
                            for (int f = 0; f < 9; f++)
                            {
                                Fields[g][f] = Placholder_Fields[follower];
                                follower++;
                            }
                        }
                    }
                    else
                    {
                        remove_playgroundButton.IsVisible = true;
                        load_playgroundButton.IsVisible = true;
                        playground.Is_Select = true;
                        IEnumerable<Playground> list = (IEnumerable<Playground>)playgroundlist.ItemsSource;

                        foreach (Playground pg in list)
                        {
                            if (pg.Id != playground.Id)
                            {
                                pg.Is_Select = false;
                            }
                        }
                        playgroundlist.SelectedItem = playground;

                        List<int> ints = new Playground_Field_Numbers_Converter().Deserialize(playground.Field_Numbers);

                        Locked_Fields.Clear();

                        int follower = 0;
                        foreach (ObservableCollection<Field> fs in Fields)
                        {
                            foreach (Field f in fs)
                            {
                                f.Is_Locked = false;
                                f.Number = ints[follower];
                                f.Was_Manuel_Select = false;
                                f.Is_Fault = false;
                                f.Skips = 0;
                                f.Is_Saturated = false;
                                //f.Placeholder_Number_Horizontal = new ObservableCollection<int>();
                                //f.Placeholder_Number_Vertikal = new ObservableCollection<int>();
                                f.Is_Clearly = false;
                                f.Is_Semi_Clearly = false;
                                follower++;

                                if (f.Number != 0 && f.Is_Fault == false)
                                {
                                    f.Was_Manuel_Select = true;
                                    f.Is_Select = false;
                                    f.Is_Locked = true;
                                    Locked_Fields.Add(f);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }


        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Timer_Label = stopwatch?.Elapsed.ToString("hh\\:mm\\:ss\\.f");
        }


        public ObservableRangeCollection<Number> Numbers = new ObservableRangeCollection<Number>();
        public static ObservableRangeCollection<ObservableCollection<Field>> Fields = new ObservableRangeCollection<ObservableCollection<Field>>();
        public static ObservableCollection<Field> Locked_Fields = new ObservableCollection<Field>();
        public static ObservableCollection<Logdata> Logs = new ObservableCollection<Logdata>();



        public static List<Possebilitie> Possebilities_Log = new List<Possebilitie>();

        public static List<Denail> Denails = new List<Denail>();


        public static List<Field> Placholder_Fields = new List<Field>();

        Field current_Field = new Field();
        Field previous_Field = new Field();

        Solver solver = new Solver();

        CancellationTokenSource tokensource = null;

        public static int field_position_marker = 0;
        public static int field_stop_position = 1;
        public static int block_stop_position = 0;
        public static int skip_stop_position = 0;
        public static string dificulty_marker = string.Empty;
        public static bool started = false; 


        public bool create_button_IsEnabled = false;
        public bool Create_Button_IsEnabled
        {
            get { return create_button_IsEnabled; }
            set
            {
                if (Create_Button_IsEnabled == value)
                    return;
                create_button_IsEnabled = value; OnPropertyChanged(nameof(Create_Button_IsEnabled));
            }
        }
        public bool delete_button_IsEnabled = false;
        public bool Delete_Button_IsEnabled
        {
            get { return delete_button_IsEnabled; }
            set
            {
                if (Delete_Button_IsEnabled == value)
                    return;
                delete_button_IsEnabled = value; OnPropertyChanged(nameof(Delete_Button_IsEnabled));
            }
        }
        public bool edit_button_IsEnabled = false;
        public bool Edit_Button_IsEnabled
        {
            get { return edit_button_IsEnabled; }
            set
            {
                if (Edit_Button_IsEnabled == value)
                    return;
                edit_button_IsEnabled = value; OnPropertyChanged(nameof(Edit_Button_IsEnabled));
            }
        }
        public bool numberGrid = false;
        public bool playoption = false;


        public string status_solver = "Wartet";
        public string Status_Solver
        {
            get { return status_solver; }
            set
            {
                if (Status_Solver == value)
                    return;
                status_solver = value; OnPropertyChanged(nameof(Status_Solver));

                if (value == "Arbeitet")
                {
                    if (stopwatch != null)
                    {
                        stopwatch.Start();
                    }
                    else
                    {
                        timer = new System.Timers.Timer(100);
                        timer.Elapsed += Timer_Elapsed;
                        timer.Start();
                        stopwatch = new Stopwatch();
                        stopwatch.Start();
                    }
                }
                if (value == "Wartet" || value == "Bereit" || value == "Undefiniert")
                {
                    timer = null;
                    stopwatch = null;
                }
                if (value == "Fertig" || value == "Ungelöst")
                {
                    if (stopwatch != null)
                    {
                        stopwatch.Stop();
                    }
                }
                if (value == "Abgebrochen")
                {
                    if (stopwatch != null)
                    {
                        stopwatch.Stop();
                    }
                }
            }
        }
        public string timer_label = "";
        public string Timer_Label
        {
            get { return timer_label; }
            set
            {
                if (Timer_Label == value)
                    return;
                timer_label = value; OnPropertyChanged(nameof(Timer_Label));
            }
        }
        public string algorithmus_label = "";
        public string Algorithmus_Label
        {
            get { return algorithmus_label; }
            set
            {
                if (Algorithmus_Label == value)
                    return;
                algorithmus_label = value; OnPropertyChanged(nameof(Algorithmus_Label));
            }
        }
        public string difficulty = null;
        public string Difficulty
        {
            get { return difficulty; }
            set
            {
                if (Difficulty == value)
                    return;
                difficulty = value; OnPropertyChanged(nameof(Difficulty));
                dificulty_marker = value;
            }
        }
        public int attampts_label = 0;
        public int Attampts_Label
        {
            get { return attampts_label; }
            set
            {
                if (Attampts_Label == value)
                    return;
                attampts_label = value; OnPropertyChanged(nameof(Attampts_Label));
            }
        }

        System.Timers.Timer timer;
        Stopwatch stopwatch;
    }

}
