using Pluto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pluto.Cells
{
    public class Field_View_Template_Selector : DataTemplateSelector
    {
        public DataTemplate Border_left { get; set; }
        public DataTemplate Border_left_inside_buttom { get; set; }
        public DataTemplate Border_left_inside_top { get; set; }
        public DataTemplate Border_right { get; set; }
        public DataTemplate Border_right_inside_buttom { get; set; }
        public DataTemplate Border_right_inside_top { get; set; }


        public DataTemplate Border_top { get; set; }
        public DataTemplate Border_top_left { get; set; }
        public DataTemplate Border_top_right { get; set; }
        public DataTemplate Border_top_inside_right { get; set; }
        public DataTemplate Border_top_inside_left { get; set; }


        public DataTemplate Border_buttom { get; set; }
        public DataTemplate Border_buttom_left { get; set; }
        public DataTemplate Border_buttom_right { get; set; }
        public DataTemplate Border_buttom_inside_left { get; set; }
        public DataTemplate Border_buttom_inside_right { get; set; }


        public DataTemplate Inside_buttom_right { get; set; }
        public DataTemplate Inside_buttom_center { get; set; }
        public DataTemplate Inside_buttom_left { get; set; }


        public DataTemplate Inside_top_right { get; set; }
        public DataTemplate Inside_top_center { get; set; }
        public DataTemplate Inside_top_left { get; set; }


        public DataTemplate Inside_center_left { get; set; }
        public DataTemplate Inside_center_right { get; set; }


        public DataTemplate Inside_center { get; set; }



        List<int> border_left = [2, 5, 8];
        List<int> border_left_inside_buttom = [3, 6];
        List<int> border_left_inside_top = [4, 7];
        List<int> border_right = [74, 77, 80];
        List<int> border_right_inside_buttom = [75, 78];
        List<int> border_right_inside_top = [76, 79];


        List<int> border_top = [10, 37, 64];
        List<int> border_top_left = [1];
        List<int> border_top_right = [73];
        List<int> border_top_inside_right = [28, 55];
        List<int> border_top_inside_left = [19, 46];

        List<int> border_buttom = [18, 45, 72];
        List<int> border_buttom_left = [9];
        List<int> border_buttom_right = [81];
        List<int> border_buttom_inside_left = [36, 63];
        List<int> border_buttom_inside_right = [27, 54];

        List<int> inside_buttom_right = [30, 57, 33, 60];
        List<int> inside_buttom_center = [12, 15, 66, 39, 42, 69];
        List<int> inside_buttom_left = [21, 24, 48, 51];

        List<int> inside_top_right = [31, 58, 34, 61];
        List<int> inside_top_center = [13, 16, 40, 43, 67, 70];
        List<int> inside_top_left = [22, 25, 49, 52];

        List<int> inside_center_left = [20, 23, 26, 47, 50,53];
        List<int> inside_center_right = [29, 32, 35, 56, 59, 62];

        List<int> inside_center = [11, 14, 17, 38, 41, 44, 65, 68, 71];



        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is Field)
            {
                Field input = (Field)item;

                if(border_left.Contains(input.Id))
                    return Border_left;
                if (border_left_inside_buttom.Contains(input.Id))
                    return Border_left_inside_buttom;
                if (border_left_inside_top.Contains(input.Id))
                    return Border_left_inside_top;
                if (border_right.Contains(input.Id))
                    return Border_right;
                if (border_right_inside_buttom.Contains(input.Id))
                    return Border_right_inside_buttom;
                if (border_right_inside_top.Contains(input.Id))
                    return Border_right_inside_top;
                if (border_left_inside_buttom.Contains(input.Id))
                    return Border_left_inside_buttom;
                if (border_top.Contains(input.Id))
                    return Border_top;
                if (border_top_left.Contains(input.Id))
                    return Border_top_left;
                if (border_top_right.Contains(input.Id))
                    return Border_top_right;
                if (border_top_inside_right.Contains(input.Id))
                    return Border_top_inside_right;
                if (border_top_inside_left.Contains(input.Id))
                    return Border_top_inside_left;
                if (border_buttom.Contains(input.Id))
                    return Border_buttom;
                if (border_buttom_left.Contains(input.Id))
                    return Border_buttom_left;
                if (border_buttom_right.Contains(input.Id))
                    return Border_buttom_right;
                if (border_buttom_inside_left.Contains(input.Id))
                    return Border_buttom_inside_left;
                if (border_buttom_inside_right.Contains(input.Id))
                    return Border_buttom_inside_right;
                if (inside_buttom_right.Contains(input.Id))
                    return Inside_buttom_right;
                if (inside_buttom_center.Contains(input.Id))
                    return Inside_buttom_center;
                if (inside_buttom_left.Contains(input.Id))
                    return Inside_buttom_left;
                if (inside_top_right.Contains(input.Id))
                    return Inside_top_right;
                if (inside_top_center.Contains(input.Id))
                    return Inside_top_center;
                if (inside_top_left.Contains(input.Id))
                    return Inside_top_left;
                if (inside_center_left.Contains(input.Id))
                    return Inside_center_left;
                if (inside_center_right.Contains(input.Id))
                    return Inside_center_right;
                if (inside_center.Contains(input.Id))
                    return Inside_center;
            }
            return Inside_center;
        }
    }
}
