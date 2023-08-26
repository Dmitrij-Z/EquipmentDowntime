using EquipmentDowntime.HelpClasses;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EquipmentDowntime.Archive
{
    /// <summary>
    /// Interaktionslogik für ArchiveForm.xaml
    /// </summary>
    public partial class ArchiveForm : Window
    {

        public ArchiveForm()
        {
            InitializeComponent();
            dgDowntime.Sorting += new DataGridSortingEventHandler(CustomSorting);
            dPicker.DisplayDateEnd = DateTime.Now;
            
        }
        private void CustomSorting(object sender, System.Windows.Controls.DataGridSortingEventArgs e)
        {
            DataGridColumn column = e.Column;

            if (column.SortMemberPath == "Department" || column.SortMemberPath == "InventoryId")
            {
                e.Handled = true;

                column.SortDirection = (column.SortDirection != ListSortDirection.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending;

                ListCollectionView lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(dgDowntime.ItemsSource);
                IComparer comparer = new NumberComparer(column.SortDirection.Value, column);
                lcv.CustomSort = comparer;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dPicker.DisplayDateStart = ((MainViewModel)DataContext).archiveVM.StartDate();
            ((MainViewModel)DataContext).archiveVM.GetArchiveData(DateTime.Now);
        }
    }
}
