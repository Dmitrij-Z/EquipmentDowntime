using EquipmentDowntime.BaseClasses;
using EquipmentDowntime.HelpClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EquipmentDowntime.OperatorData
{
    class OperatorVM : BaseInpc
    {
        DBRequests dBRequests = new DBRequests();
        public ObservableCollection<Operator> Operators { get; set; } = new ObservableCollection<Operator>();
        public OperatorVM(DBRequests dBRequests)
        {
            this.dBRequests = dBRequests;
            List<Operator> _operators = new List<Operator>();
            _operators = dBRequests.Operators();
            foreach (var oper in _operators)
            {
                Operators.Add(oper);
            }
            RaisePropertyChanged("OperatorsList");
        }
        public void OperatorsFormStarted()
        {
            FilteredName = string.Empty;
            UpdateFilter();
        }
        #region CollectionView
        private ICollectionView _operatorsList = new ListCollectionView(Array.Empty<Operator>());
        public ICollectionView OperatorsList
        {
            get => _operatorsList;
            set
            {
                _operatorsList = value;
                Count = _operatorsList.Cast<object>().Count();
            }
        }
        private static readonly Predicate<object> defaultFilter = item =>
        {
            if (item is null) return false;
            if (item is Operator) return true;

            throw new NotImplementedException($"{nameof(item)} должен быть или null, или Operator");
        };
        private Predicate<object> filter = defaultFilter;
        private string _filteredName = string.Empty;
        public string FilteredName
        {
            get => _filteredName;
            set
            {
                _filteredName = value ?? string.Empty;
                if(string.IsNullOrEmpty(_filteredName.Trim()))
                {
                    _filteredName =string.Empty;
                }
                _filteredName = _filteredName.Replace("  ", " ");
                RaisePropertyChanged("FilteredName");
                UpdateFilter();
                RaisePropertyChanged("OperatorsList");
            }
        }
        private int _count = 0;
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                RaisePropertyChanged("Count");
            }
        }
        private void UpdateFilter()
        {
            if (_operatorsList is null)
            {
                return;
            }
            string name = FilteredName.ToLower().Trim();
            if (name.Length == 0)
            {
                filter = defaultFilter;
            }
            else
            {
                filter = item =>
                {
                    if (item is null) return false;
                    if (item is Operator _operator)
                    {
                        return _operator.Name.ToLower().Contains(name);
                    }
                    throw new NotImplementedException($"{nameof(item)} должен быть или null, или Operator");
                };
            }
            _operatorsList.Filter = filter;
            RaisePropertyChanged("AddingIsPossible");
            Count = _operatorsList.Cast<object>().Count();
        }
        public static ICollectionView GetItems(ItemsControl ic)
        {
            return (ICollectionView)ic.GetValue(ItemsProperty);
        }

        public static void SetItems(ItemsControl ic, ICollectionView value)
        {
            ic.SetValue(ItemsProperty, value);
        }
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.RegisterAttached(
                "Items",
                typeof(ICollectionView),
                typeof(OperatorVM),
                new FrameworkPropertyMetadata(null, (d, e) =>
                {
                    var ic = (ItemsControl)d;
                    if (!Equals(ic.Items, e.NewValue))
                        ic.Dispatcher.BeginInvoke(new Action(() => SetItems(ic, ic.Items)));
                })
                {
                    BindsTwoWayByDefault = true
                });
        #endregion
        private RelayCommand _addOperatorCommand;
        public RelayCommand AddOperatorCommand => _addOperatorCommand ?? (_addOperatorCommand = new RelayCommand(AddOperator));
        private void AddOperator(object parameter)
        {
            MessageBoxResult result = MessageBox.Show("Добавить " + FilteredName.Trim() + " в список сотрудников?", "Новый сотрудник", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                int addedRowId = dBRequests.AddOperator(FilteredName);
                if (addedRowId > -1)
                {
                    Operator oper = new Operator();
                    oper.Id = addedRowId;
                    oper.Name = FilteredName;
                    Operators.Add(oper);
                    FilteredName = string.Empty;
                }
            }
        }
        public Boolean AddingIsPossible
        {
            get
            {
                if (string.IsNullOrEmpty(FilteredName) || !OperatorsList.IsEmpty)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
