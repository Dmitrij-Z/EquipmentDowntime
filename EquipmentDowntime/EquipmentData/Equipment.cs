namespace EquipmentDowntime.EquipmentData
{
    class Equipment
    {
        private int id = -1;
        private string department = string.Empty;
        private string equipmentName = string.Empty;
        private string inventoryId = string.Empty;

        public int Id { get => id; set => id = value; }
        public string Department { get => department; set => department = value ?? string.Empty; }
        public string EquipmentName { get => equipmentName; set => equipmentName = value ?? string.Empty; }
        public string InventoryId { get => inventoryId; set => inventoryId = value ?? string.Empty; }

        public void Clear()
        {
            id = -1;
            department = string.Empty;
            equipmentName = string.Empty;
            inventoryId = string.Empty;
        }
    }
}
