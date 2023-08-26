using System;

namespace EquipmentDowntime.DowntimeData
{
    class Downtime
    {
        private int id = 0;
        private string department = string.Empty;
        private string equipmentName = string.Empty;
        private string inventoryId = string.Empty;
        private string causeOfFailure = string.Empty;
        private DateTime receiptDateForRepair;
        private string solution = string.Empty;
        private DateTime? dateOfExitFromRepair;
        private string operatorName = string.Empty;
        private int state = 0;
        private DateTime? dateOfStateChange;

        public int Id { get => id; set => id = value; }
        public string Department { get => department; set => department = value ?? string.Empty; }
        public string EquipmentName { get => equipmentName; set => equipmentName = value ?? string.Empty; }
        public string InventoryId { get => inventoryId; set => inventoryId = value ?? string.Empty; }
        public string CauseOfFailure { get => causeOfFailure; set => causeOfFailure = value ?? string.Empty; }
        public DateTime ReceiptDateForRepair { get => receiptDateForRepair; set => receiptDateForRepair = value; }
        public string Solution { get => solution; set => solution = value ?? string.Empty; }
        public DateTime? DateOfExitFromRepair { get => dateOfExitFromRepair; set => dateOfExitFromRepair = value ?? null; }
        public string OperatorName { get => operatorName; set => operatorName = value ?? string.Empty; }
        /// <summary>
        /// Состояние задачи:
        /// <list type="table">
        /// <item><term>0</term> Задача добавлена/изменена (по умолчанию)</item>
        /// <item><term>1</term> Задача выполнена</item>
        /// <item><term>2</term> Задача отменена</item>
        /// </list>
        /// </summary>
        public int State { get => state; set => state = value; }
        public DateTime? DateOfStateChange { get => dateOfStateChange; set => dateOfStateChange = value ?? null; }
    }
}
