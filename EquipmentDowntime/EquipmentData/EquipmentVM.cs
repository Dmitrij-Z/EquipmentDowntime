using EquipmentDowntime.BaseClasses;
using EquipmentDowntime.HelpClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace EquipmentDowntime.EquipmentData
{
    class EquipmentVM : BaseInpc
    {
        private bool equipmentExistsInDb = true;
        DBRequests dBRequests = new DBRequests();
        public EquipmentVM(DBRequests dBRequests)
        {
            Equipments = new ObservableCollection<Equipment>();
            this.dBRequests = dBRequests;
            List<Equipment> _equipments = new List<Equipment>();
            _equipments = dBRequests.Equipments();
            foreach (var eq in _equipments)
            {
                Equipments.Add(eq);
            }
        }
        #region Коллекция
        private ObservableCollection<Equipment> _equipments;
        public ObservableCollection<Equipment> Equipments
        {
            get
            {
                return _equipments;
            }
            set
            {
                _equipments = value;
                RaisePropertyChanged("Equipments");
            }
        }
        private Equipment _selectedEquipment;
        public Equipment SelectedEquipment
        {
            get
            {
                return _selectedEquipment;
            }
            set
            {
                _selectedEquipment = value;
                RaisePropertyChanged("SelectedEquipment");
                SetParams();
            }
        }
        #endregion
        #region Параметры
        private string _tbDepartment = string.Empty;
        public string TbDepartment
        {
            get { return _tbDepartment; }
            set
            {
                _tbDepartment = value;
                RaisePropertyChanged("TbDepartment");
                PropertyChanging();
            }
        }

        private string _tbEquipmentName = string.Empty;
        public string TbEquipmentName
        {
            get { return _tbEquipmentName; }
            set
            {
                _tbEquipmentName = value;
                RaisePropertyChanged("TbEquipmentName");
                PropertyChanging();
            }
        }

        private string _tbInventoryId = string.Empty;
        public string TbInventoryId
        {
            get { return _tbInventoryId; }
            set
            {
                _tbInventoryId = value;
                RaisePropertyChanged("TbInventoryId");
                equipmentExistsInDb = dBRequests.EquipmentExists(TbInventoryId.Trim());
                PropertyChanging();
            }
        }
        public Boolean AddingIsPossible
        {
            get
            {
                if (EquipmentParametersAreNullOrEmpty() || equipmentExistsInDb)
                {
                    return false;
                }
                return true;
            }
        }
        public Boolean UpdatingIsPossible
        {
            get
            {
                if (EquipmentParametersAreNullOrEmpty())
                {
                    return false;
                }
                if (EquipmentParametersHaveChanged() && TbInventoryId.Trim() == SelectedEquipment.InventoryId)
                {
                    return true;
                }
                return false;
            }
        }
        #endregion
        private void PropertyChanging()
        {
            RaisePropertyChanged("UpdatingIsPossible");
            RaisePropertyChanged("AddingIsPossible");
        }
        private bool EquipmentParametersAreNullOrEmpty()
        {
            if (string.IsNullOrEmpty(TbDepartment.Trim()) ||
                string.IsNullOrEmpty(TbEquipmentName.Trim()) ||
                string.IsNullOrEmpty(TbInventoryId.Trim()))
            {
                return true;
            }
            return false;
        }
        private bool EquipmentParametersHaveChanged()
        {
            if(SelectedEquipment == null)
            {
                return false;
            }
            if(TbDepartment.Trim() == SelectedEquipment.Department && 
                TbEquipmentName.Trim() == SelectedEquipment.EquipmentName)
            {
                return false;
            }
            return true;
        }
        private void SetParams()
        {
            if (SelectedEquipment == null)
            {
                TbDepartment = string.Empty;
                TbEquipmentName = string.Empty;
                TbInventoryId = string.Empty;
            }
            else
            {
                TbDepartment = SelectedEquipment.Department;
                TbEquipmentName = SelectedEquipment.EquipmentName;
                TbInventoryId = SelectedEquipment.InventoryId;
            }
        }
        #region Команды
        private RelayCommand _updateEquipmentCommand;
        public RelayCommand UpdateEquipmentCommand => _updateEquipmentCommand ?? (_updateEquipmentCommand = new RelayCommand(UpdateEquipment));
        private void UpdateEquipment()
        {
            Equipment equipment = new Equipment();
            equipment.Id = SelectedEquipment.Id;
            equipment.Department = TbDepartment.Trim();
            equipment.EquipmentName = TbEquipmentName.Trim();
            equipment.InventoryId = TbInventoryId.Trim();
            dBRequests.EquipmentUpdate(equipment);
            for (int i = 0; i < Equipments.Count; i++)
            {
                if(Equipments[i].Id == equipment.Id)
                {
                    Equipments[i] = equipment;
                }
            }
            RaisePropertyChanged("Equipments");
            RaisePropertyChanged("UpdatingIsPossible");
        }
        private RelayCommand _addEquipmentCommand;
        public RelayCommand AddEquipmentCommand => _addEquipmentCommand ?? (_addEquipmentCommand = new RelayCommand(AddEquipment));
        private void AddEquipment()
        {
            Equipment equipment = new Equipment();
            equipment.Department = TbDepartment.Trim();
            equipment.EquipmentName = TbEquipmentName.Trim();
            equipment.InventoryId = TbInventoryId.Trim();
            equipment.Id = dBRequests.EquipmentIsert(equipment);
            if (equipment.Id != -1)
            {
                Equipments.Add(equipment);
                SelectedEquipment = Equipments[Equipments.Count-1];
            }
        }
        private RelayCommand _excludeEquipmentCommand;
        public RelayCommand ExcludeEquipmentCommand => _excludeEquipmentCommand ?? (_excludeEquipmentCommand = new RelayCommand(ExcludeEquipment));
        private void ExcludeEquipment(object parameter)
        {
            if(parameter != null)
            {
                List<Equipment> eqs = new List<Equipment>();
                DataGrid dg = parameter as DataGrid;

                for (int i = 0; i < dg.SelectedItems.Count; i++)
                {
                    Equipment eq = dg.SelectedItems[i] as Equipment;
                    eqs.Add(eq);
                }
                if (dBRequests.ExcludeEquipment(eqs))
                {
                    for (int i = Equipments.Count - 1; i >= 0; i--)
                    {
                        if (eqs.Contains(Equipments[i]))
                        {
                            Equipments.RemoveAt(i);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
