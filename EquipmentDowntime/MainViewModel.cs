using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EquipmentDowntime.Archive;
using EquipmentDowntime.DowntimeData;
using EquipmentDowntime.EquipmentData;
using EquipmentDowntime.HelpClasses;
using EquipmentDowntime.OperatorData;

namespace EquipmentDowntime
{
    class MainViewModel
    {
        DBRequests dBRequests = new DBRequests();
        public DowntimeVM downtimeVM { get; set; }
        public OperatorVM operatorVM { get; set; }
        public EquipmentVM equipmentVM { get; set; }
        public ArchiveVM archiveVM { get; set; }
        public MainViewModel()
        {
            dBRequests.CreatingDbIfNotExist();
            downtimeVM = new DowntimeVM(dBRequests);
            operatorVM = new OperatorVM(dBRequests);
            equipmentVM = new EquipmentVM(dBRequests);
            archiveVM = new ArchiveVM(dBRequests);
        }
    }
}
