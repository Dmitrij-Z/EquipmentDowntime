using EquipmentDowntime.BaseClasses;
using EquipmentDowntime.EquipmentData;
using EquipmentDowntime.HelpClasses;
using EquipmentDowntime.OperatorData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EquipmentDowntime.DowntimeData
{
    class DowntimeVM : BaseInpc
    {
        DBRequests dBRequests = new DBRequests();
        public ObservableCollection<Downtime> DowntimeCollection { get; set; }
        public ObservableCollection<Downtime> SelectedDowntimeList { get; } = new ObservableCollection<Downtime>();
        public DowntimeVM(DBRequests dBRequests)
        {
            this.dBRequests = dBRequests;
            List<Downtime>_equipment = dBRequests.GetDowntimeByDay(DateTime.Now);
            DowntimeCollection = new ObservableCollection<Downtime>(_equipment);
        }
        private Downtime selectedDowntime;
        public Downtime SelectedDowntime
        {
            get { return selectedDowntime; }
            set
            {
                selectedDowntime = value;
                RaisePropertyChanged("SelectedDowntime");
            }
        }
        #region Добавление новой задачи
        public bool isAlreadyInTask = false;
        public ObservableCollection<Equipment> EquipmentInfo { get; set; } = new ObservableCollection<Equipment>()
        {
            new Equipment() { Id = -1, Department = string.Empty, EquipmentName = string.Empty, InventoryId = string.Empty }
        };
        public ObservableCollection<Operator> operators { get; set; }

        private Operator selectedOperator;
        public Operator SelectedOperator
        {
            get { return selectedOperator; }
            set
            {
                selectedOperator = value;
                RaisePropertyChanged("AddingNewTaskIsPossible");
            }
        }
        #region Параметры
        private string _newTaskInventoryId = string.Empty;
        public string NewTaskInventoryId
        {
            get { return _newTaskInventoryId; }
            set
            {
                _newTaskInventoryId = value;
                Equipment eq = dBRequests.GetEquipmentById(NewTaskInventoryId.Trim());
                EquipmentInfo[0].Id = eq.Id;
                EquipmentInfo[0].Department = eq.Department;
                EquipmentInfo[0].EquipmentName = eq.EquipmentName;
                EquipmentInfo[0].InventoryId = eq.InventoryId;
                isAlreadyInTask = false;
                if (EquipmentInfo[0].Id > -1)
                {
                    isAlreadyInTask = DowntimeCollection.Any(itemF => itemF.InventoryId == eq.InventoryId);
                }
                RaisePropertyChanged("EquipmentInfo");
                RaisePropertyChanged("IsAlreadyInTask");
                RaisePropertyChanged("AddingDescriptionNewTaskIsPossible");
                RaisePropertyChanged("AddingNewTaskIsPossible");
            }
        }
        private string _newTaskCauseOfFailure = string.Empty;
        public string NewTaskCauseOfFailure
        {
            get { return _newTaskCauseOfFailure; }
            set
            {
                _newTaskCauseOfFailure = value;
                RaisePropertyChanged("NewTaskCauseOfFailure");
                RaisePropertyChanged("AddingNewTaskIsPossible");
            }
        }

        private DateTime _newTaskReceiptDateForRepair;
        public DateTime NewTaskReceiptDateForRepair
        {
            get { return _newTaskReceiptDateForRepair; }
            set
            {
                _newTaskReceiptDateForRepair = value;
                RaisePropertyChanged("NewTaskReceiptDateForRepair");
                RaisePropertyChanged("AddingNewTaskIsPossible");
            }
        }
        private string _newTaskSolution = string.Empty;
        public string NewTaskSolution
        {
            get { return _newTaskSolution; }
            set
            {
                _newTaskSolution = value;
                RaisePropertyChanged("NewTaskSolution");
                RaisePropertyChanged("AddingNewTaskIsPossible");
            }
        }
        private DateTime? _newTaskDateOfExitFromRepair;
        public DateTime? NewTaskDateOfExitFromRepair
        {
            get { return _newTaskDateOfExitFromRepair; }
            set
            {
                _newTaskDateOfExitFromRepair = value;
                RaisePropertyChanged("NewTaskDateOfExitFromRepair");
                RaisePropertyChanged("AddingNewTaskIsPossible");
            }
        }

        private string _selectedDate = string.Empty;
        public string SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                RaisePropertyChanged("SelectedDate");
            }
        }
        public Boolean IsAlreadyInTask
        {
            get
            {
                return isAlreadyInTask;
            }
        }
        public Boolean AddingDescriptionNewTaskIsPossible
        {
            get
            {
                if (EquipmentInfo[0].Id == -1 || isAlreadyInTask)
                {
                    return false;
                }
                return true;
            }
        }
        public Boolean AddingNewTaskIsPossible
        {
            get
            {
                if (SelectedOperator == null || string.IsNullOrEmpty(NewTaskInventoryId) ||
                    string.IsNullOrEmpty(NewTaskCauseOfFailure) || NewTaskReceiptDateForRepair == null ||
                    string.IsNullOrEmpty(NewTaskSolution))
                {
                    return false;
                }
                return true;
            }
        }
        #endregion
        #region Команды
        private RelayCommand _saveNewTaskCommand;
        public RelayCommand SaveNewTaskCommand => _saveNewTaskCommand ?? (_saveNewTaskCommand = new RelayCommand(SaveNewTask));
        public void SaveNewTask()
        {
            dBRequests.RequestToAddNewTask(NewTaskReceiptDateForRepair, NewTaskCauseOfFailure, NewTaskSolution,
                NewTaskDateOfExitFromRepair, EquipmentInfo[0].Id, SelectedOperator.Id);
            NewTaskDataClear();
            DowntimeCollection = new ObservableCollection<Downtime>(dBRequests.GetDowntimeByDay(DateTime.Now));
            RaisePropertyChanged("DowntimeCollection");
        }
        private RelayCommand _completedTaskCommand;
        public RelayCommand СompletedTaskCommand => _completedTaskCommand ?? (_completedTaskCommand = new RelayCommand(СompletedTask));
        public void СompletedTask()
        {
            for (int i = 0; i < SelectedDowntimeList.Count; i++)
            {
                dBRequests.ChangeTaskState(SelectedDowntimeList[i].Id, 1);
            }
            DowntimeCollection = new ObservableCollection<Downtime>(dBRequests.GetDowntimeByDay(DateTime.Now));
            RaisePropertyChanged("DowntimeCollection");
        }
        private RelayCommand _cancelledTaskCommand;
        public RelayCommand CancelledTaskCommand => _cancelledTaskCommand ?? (_cancelledTaskCommand = new RelayCommand(CancelledTask));
        public void CancelledTask()
        {
            for (int i = 0; i < SelectedDowntimeList.Count; i++)
            {
                dBRequests.ChangeTaskState(SelectedDowntimeList[i].Id, 2);
            }
            DowntimeCollection = new ObservableCollection<Downtime>(dBRequests.GetDowntimeByDay(DateTime.Now));
            RaisePropertyChanged("DowntimeCollection");
        }
        private RelayCommand _returnTaskCommand;
        public RelayCommand ReturnTaskCommand => _returnTaskCommand ?? (_returnTaskCommand = new RelayCommand(ReturnTask));
        public void ReturnTask()
        {
            for (int i = 0; i < SelectedDowntimeList.Count; i++)
            {
                dBRequests.ChangeTaskState(SelectedDowntimeList[i].Id, 0);
            }

            DowntimeCollection = new ObservableCollection<Downtime>(dBRequests.GetDowntimeByDay(DateTime.Now));
            RaisePropertyChanged("DowntimeCollection");
        }
        #endregion
        private void NewTaskDataClear()
        {
            NewTaskReceiptDateForRepair = DateTime.Now;
            NewTaskCauseOfFailure = string.Empty;
            NewTaskSolution = string.Empty;
            NewTaskDateOfExitFromRepair = null;
            NewTaskInventoryId = string.Empty;
            SelectedOperator = null;
        }
        public void GetOperatorsList()
        {
            NewTaskInventoryId = string.Empty;
            NewTaskSolution = string.Empty;
            NewTaskCauseOfFailure = string.Empty;
            SelectedOperator = null;
            NewTaskDateOfExitFromRepair = null;
            NewTaskReceiptDateForRepair = DateTime.Now;

            operators = new ObservableCollection<Operator>(dBRequests.Operators());
        }
        #endregion
    }
}
