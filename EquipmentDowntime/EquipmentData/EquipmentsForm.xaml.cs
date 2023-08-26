using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
using EquipmentDowntime.EquipmentData;
using EquipmentDowntime.HelpClasses;

namespace EquipmentDowntime.EquipmentData
{
    /// <summary>
    /// Interaktionslogik für EquipmentsForm.xaml
    /// </summary>
    public partial class EquipmentsForm : Window
    {
        public EquipmentsForm()
        {
            InitializeComponent();
            dgEquipments.Sorting += new DataGridSortingEventHandler(CustomSorting);
        }
        private void CustomSorting(object sender, System.Windows.Controls.DataGridSortingEventArgs e)
        {
            
               DataGridColumn column = e.Column;

            if (column.SortMemberPath == "Department" || column.SortMemberPath == "InventoryId")
            {
                e.Handled = true;

                column.SortDirection = (column.SortDirection != ListSortDirection.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending;

                ListCollectionView lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(dgEquipments.ItemsSource);
                IComparer comparer = new NumberComparer(column.SortDirection.Value, column);
                lcv.CustomSort = comparer;
            }
        }
    }
}
