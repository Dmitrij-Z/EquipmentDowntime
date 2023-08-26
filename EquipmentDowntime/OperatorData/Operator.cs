namespace EquipmentDowntime.OperatorData
{
    class Operator
    {
        private int id;
        private string name = string.Empty;

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value ?? string.Empty; }
    }
}
