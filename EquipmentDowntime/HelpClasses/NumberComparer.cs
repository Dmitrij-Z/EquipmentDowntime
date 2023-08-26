using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace EquipmentDowntime.HelpClasses
{
    public class NumberComparer : IComparer
    {
        public NumberComparer()
        {
        }
        public NumberComparer(ListSortDirection sortDirection, DataGridColumn column)
        {
            SortDirection = sortDirection;
            this.column = column;
        }
        private ListSortDirection SortDirection { get; set; }
        private DataGridColumn column { get; set; }
        public int Compare(object obj1, object obj2)
        {
            int num1, num2;

            var Prop1 = obj1.GetType().GetProperty(column.SortMemberPath);
            string text1 = Prop1.GetValue(obj1).ToString();

            var Prop2 = obj2.GetType().GetProperty(column.SortMemberPath);
            string text2 = Prop2.GetValue(obj2).ToString();

            if (text1 == null || text2 == null)
            {
                return 0;
            }
            var i = 0;
            while (i < text1.Length && i < text2.Length && text1[i] == text2[i])
            {
                i++;
            }
            string strNumber1 = new string(text1.Substring(i).ToList().TakeWhile(c => Char.IsDigit(c)).ToArray());
            string strNumber2 = new string(text2.Substring(i).ToList().TakeWhile(c => Char.IsDigit(c)).ToArray());

            string number1 = new string(text1.Substring(0).ToList().TakeWhile(c => Char.IsDigit(c)).ToArray());
            string number2 = new string(text2.Substring(0).ToList().TakeWhile(c => Char.IsDigit(c)).ToArray());

            string strText1 = text1.Substring(number1.Length);
            string strText2 = text2.Substring(number2.Length);

            if (int.TryParse(text1, out num1) && int.TryParse(text2, out num2))
            {
                return SortDirection == ListSortDirection.Ascending ? num1.CompareTo(num2) : num2.CompareTo(num1);
            }
            if (!string.IsNullOrEmpty(number1) && !string.IsNullOrEmpty(number2) && int.TryParse(number1, out num1) && int.TryParse(number2, out num2))
            {
                if (number1 != number2)
                {
                    return SortDirection == ListSortDirection.Ascending ? num1.CompareTo(num2) : num2.CompareTo(num1);
                }
                return SortDirection == ListSortDirection.Ascending ? strText1.CompareTo(strText2) : strText2.CompareTo(strText1);
            }
            if (int.TryParse(text1, out num1) ^ int.TryParse(text2, out num2))
            {
                return SortDirection == ListSortDirection.Ascending ? text1.CompareTo(text2) : text2.CompareTo(text1);
            }
            if (int.TryParse(strNumber1, out num1) && int.TryParse(strNumber2, out num2))
            {
                return SortDirection == ListSortDirection.Ascending ? num1.CompareTo(num2) : num2.CompareTo(num1);
            }
            return SortDirection == ListSortDirection.Ascending ? text1.CompareTo(text2) : text2.CompareTo(text1);
        }
    }
}
