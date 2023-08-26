using EquipmentDowntime.Archive;
using EquipmentDowntime.DowntimeData;
using EquipmentDowntime.EquipmentData;
using EquipmentDowntime.HelpClasses;
using EquipmentDowntime.OperatorData;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EquipmentDowntime
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
            dgDowntime.Sorting += new DataGridSortingEventHandler(CustomSorting);
        }
        private void EditEquipmentList_Click(object sender, RoutedEventArgs e)
        {
            EquipmentsForm equipmentsForm = new EquipmentsForm();
            equipmentsForm.Owner = this;
            equipmentsForm.DataContext = this.DataContext;
            equipmentsForm.ShowDialog();
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

        private void EditOperatorsList_Click(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).operatorVM.OperatorsFormStarted();
            OperatorsForm operatorsForm = new OperatorsForm();
            operatorsForm.Owner = this;
            operatorsForm.DataContext = this.DataContext;
            operatorsForm.ShowDialog();
        }

        private void AddNewTask_Click(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).downtimeVM.GetOperatorsList();
            AddNewTask addNewTask = new AddNewTask();
            addNewTask.Owner = this;
            addNewTask.DataContext = this.DataContext;
            addNewTask.ShowDialog();
        }
        ArchiveForm archiveForm;
        private void ShowArchive_Click(object sender, RoutedEventArgs e)
        {
            archiveForm = Application.Current.Windows.OfType<ArchiveForm>().FirstOrDefault();
            if (archiveForm == null)
            {
                archiveForm = new ArchiveForm();
                archiveForm.Owner = this;
                archiveForm.DataContext = this.DataContext;
                archiveForm.Show();
            }
            else
            {
                archiveForm.Activate();
            }
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            archiveForm = Application.Current.Windows.OfType<ArchiveForm>().FirstOrDefault();
            if (archiveForm != null)
            {
                archiveForm.Close();
            }
        }
    }
}
