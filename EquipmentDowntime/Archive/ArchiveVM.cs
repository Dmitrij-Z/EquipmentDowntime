using EquipmentDowntime.BaseClasses;
using EquipmentDowntime.DowntimeData;
using EquipmentDowntime.HelpClasses;
using System;
using System.Collections.ObjectModel;

namespace EquipmentDowntime.Archive
{
    class ArchiveVM : BaseInpc
    {
        DBRequests dBRequests = new DBRequests();
        public ObservableCollection<Downtime> ArchiveDowntimeCollection { get; set; }
        public ArchiveVM(DBRequests dBRequests)
        {
            this.dBRequests = dBRequests;
            SelectedDate = DateTime.Now;
            ArchiveDowntimeCollection = new ObservableCollection<Downtime>(dBRequests.GetDowntimeByDay(DateTime.Now));
            RaisePropertyChanged("ArchiveDowntimeCollection");
        }

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                RaisePropertyChanged("SelectedDate");
                GetArchiveData(_selectedDate);
            }
        }
        public void GetArchiveData(DateTime dt)
        {
            ArchiveDowntimeCollection = new ObservableCollection<Downtime>(dBRequests.GetDowntimeByDay(dt));
            RaisePropertyChanged("ArchiveDowntimeCollection");
        }
        public DateTime StartDate()
        {
            return dBRequests.StartdDate();
        }
    }
}
