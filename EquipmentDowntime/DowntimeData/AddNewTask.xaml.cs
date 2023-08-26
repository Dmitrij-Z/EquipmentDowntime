using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EquipmentDowntime.DowntimeData
{
    /// <summary>
    /// Interaktionslogik für AddNewTask.xaml
    /// </summary>
    public partial class AddNewTask : Window
    {
        public AddNewTask()
        {
            InitializeComponent();
        }

        private void AddNewTaskButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DatePicker_LostFocus(object sender, RoutedEventArgs e)
        {
            DatePicker dp = sender as DatePicker;
            if(string.IsNullOrEmpty(dp.Text))
            {
                dp.Text = DateTime.Now.ToString("dd.MM.yyyy");
                dp.DisplayDate = DateTime.Now;
            }

        }
    }
}
