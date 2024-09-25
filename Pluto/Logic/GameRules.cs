using Pluto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pluto.Logic
{
    public class GameRules
    {
        public static List<Field> fields = [];


        /// <summary>
        /// Checkt die Regeln zum aktuellen Spielstand. 
        /// 
        /// </summary>
        /// <param name="origin">Die Spielstandbrettdaten</param>
        /// <param name="current">Das zu prüfende Feld</param>
        /// <returns>Checkliste => 0 = Row | 1 = Column | 2 = Block</returns>
        public static (List<Boolean>,List<Field>) Check_Rules_OneMove(List<Field> origin, Field current)    
        {
            List<Boolean> checklist = new List<Boolean>() {false,false,false,false};
            List<Field> faults = new List<Field>();

            fields.Clear();
            origin.AsReadOnly();
            fields.AddRange(origin);

            if(fields.Count < 81)
            {
                return (checklist,faults);
            }

            List<Field> Result = new List<Field>();
            (checklist[0], Result) = Check_Row(fields,current);
            faults.AddRange(Result);
            (checklist[1], Result) = Check_Column(fields,current);
            if (Result.Count != 0)
            {
                foreach (Field f in Result)
                {
                    if (faults.Where(x => x.Id == f.Id).Count() == 0)
                    {
                        faults.Add(f);
                    }
                }
            }
            (checklist[2], Result) = Check_Block(fields,current);
            if (Result.Count != 0)
            {
                foreach (Field f in Result)
                {
                    if (faults.Where(x => x.Id == f.Id).Count() == 0)
                    {
                        faults.Add(f);
                    }
                }
            }
            checklist[3] = Check_Number_Limits(current);

            return (checklist,faults);
        }

        /// <summary>
        /// Checkt die Regeln zum aktuellen Spielstand. 
        /// 
        /// </summary>
        /// <param name="origin">Die Spielstandbrettdaten</param>
        /// <returns>Checkliste => 0 = Row | 1 = Column | 2 = Block</returns>
        public static (List<Boolean>, List<Field>) Check_Rules_All(List<Field> origin)
        {
            List<Boolean> checklist = new List<Boolean>() { false, false, false };
            List<Field> faults = new List<Field>();

            fields.Clear();
            origin.AsReadOnly();
            fields.AddRange(origin);

            if (fields.Count < 81)
            {
                return (checklist, faults);
            }

            List<Field> Result = new List<Field>();
            (checklist[0], Result) = Check_Rows(fields);
            faults.AddRange (Result);
            (checklist[1], Result) = Check_Columns(fields);
            if (Result.Count != 0)
            {
                foreach (Field f in Result)
                {
                    if (faults.Where(x => x.Id == f.Id).Count() == 0)
                    {
                        faults.Add(f);
                    }
                }
            }
            (checklist[2], Result) = Check_Blocks(fields);
            if (Result.Count != 0)
            {
                foreach (Field f in Result)
                {
                    if (faults.Where(x => x.Id == f.Id).Count() == 0)
                    {
                        faults.Add(f);
                    }
                }
            }

            return (checklist, faults);
        }


        public static bool Check_Number_Limits(Field current)
        {
            if (current.Number < 10 && current.Number >= 0)
            {
                return true;
            }

            return false;
        }


        public static (bool,List<Field>) Check_Column(List<Field> input,Field current)
        {
            bool inspector = true;
            List<Field> faults = new List<Field>();
            List<Field> inspectfields = input.Where(x => x.Column_Number == current.Column_Number).ToList();

            foreach (Field field in inspectfields)
            {
                if(field.Number == current.Number && field.Id != current.Id && current.Number != 0)
                {
                    inspector = false;
                    faults.Add(field);
                }
            }

            return (inspector, faults);
        }

        public static (bool, List<Field>) Check_Row(List<Field> input, Field current)
        {
            bool inspector = true;
            List<Field> faults = new List<Field>();
            List<Field> inspectfields = input.Where(x => x.Row_Number == current.Row_Number).ToList();

            foreach (Field field in inspectfields)
            {
                if (field.Number == current.Number && field.Id != current.Id && current.Number != 0)
                {
                    inspector = false;
                    faults.Add(field);
                }
            }

            return (inspector, faults);
        }

        public static (bool, List<Field>) Check_Block(List<Field> input, Field current) 
        {
            bool inspector = true;
            List<Field> faults = new List<Field>();
            List<Field> inspectfields = input.Where(x => x.Grid_Number == current.Grid_Number).ToList();

            foreach (Field field in inspectfields)
            {
                if (field.Number == current.Number && field.Id != current.Id && current.Number != 0)
                {
                    inspector = false;
                    faults.Add(field);
                }
            }

            return (inspector, faults);
        }


        public static (bool, List<Field>) Check_Columns(List<Field> input)
        {
            bool inspector = true;
            List<Field> faults = new List<Field>();

            for (int i = 1; i < 10; i++)
            {
                List<Field> result = input.Where(x=>x.Column_Number == i).ToList();

                foreach(Field field in result)
                {
                    if(result.Where(x=>x.Number == field.Number).Count() >= 2 && field.Number != 0)
                    {
                        inspector = false;
                        faults.Add(field);
                    }
                }

            }

            return (inspector, faults);
        }

        public static (bool, List<Field>) Check_Rows(List<Field> input)
        {
            bool inspector = true;
            List<Field> faults = new List<Field>();

            for (int i = 1; i < 10; i++)
            {
                List<Field> result = input.Where(x => x.Row_Number == i).ToList();

                foreach (Field field in result)
                {
                    if (result.Where(x => x.Number == field.Number).Count() >= 2 && field.Number != 0)
                    {
                        inspector = false;
                        faults.Add(field);
                    }
                }

            }

            return (inspector, faults);
        }

        public static (bool, List<Field>) Check_Blocks(List<Field> input)
        {
            bool inspector = true;
            List<Field> faults = new List<Field>();

            for (int i = 1; i < 10; i++)
            {
                List<Field> result = input.Where(x => x.Grid_Number == i).ToList();

                foreach (Field field in result)
                {
                    if (result.Where(x => x.Number == field.Number).Count() >= 2 && field.Number != 0)
                    {
                        inspector = false;
                        faults.Add(field);
                    }
                }

            }

            return (inspector, faults);
        }


        public static List<Field> Count_Columnfaults(List<Field> input, Field current)
        {
            List<Field> result = new List<Field>();

            foreach (Field field in input.Where(x => x.Column_Number == current.Column_Number))
            {
                if (field.Number == current.Number && field.Id != current.Id)
                {
                    result.Add(field);
                }
            }

            return result;
        }

        public static List<Field> Count_Rowfaults(List<Field> input, Field current)
        {
            List<Field> result = new List<Field>();

            foreach (Field field in input.Where(x => x.Row_Number == current.Row_Number))
            {
                if (field.Number == current.Number && field.Id != current.Id)
                {
                    result.Add(field);
                }
            }

            return result;
        }

        public static List<Field> Count_Gridfaults(List<Field> input, Field current)
        {
            List<Field> result = new List<Field>();

            foreach (Field field in input.Where(x => x.Grid_Number == current.Grid_Number))
            {
                if (field.Number == current.Number && field.Id != current.Id)
                {
                    result.Add(field);
                }
            }

            return result;
        }
    }
}
