using EquipmentDowntime.DowntimeData;
using EquipmentDowntime.EquipmentData;
using EquipmentDowntime.OperatorData;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;

namespace EquipmentDowntime.HelpClasses
{
    class DBRequests
    {
        private SQLiteConnection dbConnection;
        private SQLiteCommand command;
        private string dbPath = System.Environment.CurrentDirectory;
        private string dbFilePath;
        /// <summary>Метод проверки существования БД рядом с исполняемым файлом
        /// и её создание в случае отсутствия.</summary>
        public void CreatingDbIfNotExist()
        {
            dbFilePath = dbPath + "\\EquipmentsDowntime.db";
            string strCon = string.Format("Data Source={0};", dbFilePath);
            dbConnection = new SQLiteConnection(strCon);
            SQLiteTransaction qLiteTransaction = null;
            if (!System.IO.File.Exists(dbFilePath))
            {
                try
                {
                    SQLiteConnection.CreateFile(dbFilePath);
                    
                    dbConnection.Open();
                    qLiteTransaction = dbConnection.BeginTransaction();
                    command = dbConnection.CreateCommand();
                    command.Transaction = qLiteTransaction;

                    command.CommandText = "CREATE TABLE Equipments (Id INTEGER PRIMARY KEY AUTOINCREMENT, Department TEXT NOT NULL, " +
                            "EquipmentName TEXT NOT NULL, InventoryId TEXT NOT NULL UNIQUE, Current NUMERIC NOT NULL)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE Operators ('Id' INTEGER PRIMARY KEY AUTOINCREMENT, 'Operator' TEXT NOT NULL UNIQUE)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE DownTime ('Id' INTEGER PRIMARY KEY AUTOINCREMENT, 'ReceiptDateForRepair' TEXT, " +
                            "'CauseOfFailure' TEXT NOT NULL, 'Solution' TEXT, 'DateOfExitFromRepair' TEXT)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE Overview ('Id' INTEGER, 'DownTimeId' INTEGER NOT NULL, 'EquipmentId' INTEGER NOT NULL, " +
                            "'OperatorId' INTEGER NOT NULL, 'State' INTEGER NOT NULL, 'DateOfChange' TEXT NOT NULL)";
                    command.ExecuteNonQuery();

                    qLiteTransaction.Commit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    qLiteTransaction.Rollback();
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }
        #region downtime data requests
        public List<Downtime> GetDowntimeByDay(DateTime dateTime)
        {
            List<Downtime> equipmentList = new List<Downtime>();
            try
            {
                dbConnection.Open();
                command = dbConnection.CreateCommand();

                command.CommandText = "WITH LastChange AS (SELECT eq.Department AS Department, eq.EquipmentName AS EquipmentName, " +
                    "eq.InventoryId AS InventoryId, dt.CauseOfFailure AS CauseOfFailure, " +
                    "dt.ReceiptDateForRepair AS ReceiptDateForRepair, dt.Solution AS Solution, " +
                    "dt.DateOfExitFromRepair AS DateOfExitFromRepair, op.Operator AS Operator, " +
                    "MAX(ov.DateOfChange) AS DateOfChange, ov.Id AS Id, ov.State AS State " +
                    "FROM Overview ov INNER JOIN Equipments eq ON ov.EquipmentId = eq.Id INNER JOIN DownTime dt " +
                    "ON ov.DownTimeId = dt.Id INNER JOIN Operators op ON ov.OperatorId = op.Id " +
                    "WHERE dt.ReceiptDateForRepair < '" + dateTime.AddDays(1).ToString("yyyy-MM-dd") + "' AND ov.DateOfChange < '" +
                    dateTime.AddDays(1).ToString("yyyy-MM-dd") + "' GROUP BY ov.Id) SELECT Id, Department, EquipmentName, InventoryId, " +
                    "CauseOfFailure, ReceiptDateForRepair, Solution, DateOfExitFromRepair, Operator, State, DateOfChange " +
                    "FROM LastChange WHERE State = 0 OR (State > 0 AND DateOfChange LIKE '" +
                    dateTime.ToString("yyyy-MM-dd") + "%')";

                SQLiteDataReader rdr = command.ExecuteReader();

                while (rdr.Read())
                {
                    Downtime downtime = new Downtime();
                    downtime.Id = rdr.GetInt32(0);
                    downtime.Department = rdr.GetString(1);
                    downtime.EquipmentName = rdr.GetString(2);
                    downtime.InventoryId = rdr.GetString(3);
                    downtime.CauseOfFailure = rdr.GetString(4);
                    downtime.ReceiptDateForRepair = rdr.GetDateTime(5);
                    downtime.Solution = rdr.GetString(6);
                    downtime.DateOfExitFromRepair = rdr.IsDBNull(7) ? (DateTime?)null : rdr.GetDateTime(7);
                    downtime.OperatorName = rdr.GetString(8);
                    downtime.State = rdr.GetInt32(9);
                    equipmentList.Add(downtime);
                }
                rdr.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }
            return equipmentList;
        }
        #endregion
        #region operator data requests
        public int AddOperator(string _operator)
        {
            dbConnection.Open();
            command = dbConnection.CreateCommand();
            try
            {
                command.CommandText = "INSERT INTO Operators (Operator) VALUES ('" + _operator + "');";
                command.ExecuteNonQuery();

                command.CommandText = "SELECT last_insert_rowid();";
                long rowid = (long)command.ExecuteScalar();

                return (int)rowid;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return -1;
            }
            finally
            {
                dbConnection.Close();
            }
        }
        public List<Operator> Operators()
        {
            List<Operator> operators = new List<Operator>();
            try
            {
                dbConnection.Open();
                command = dbConnection.CreateCommand();
                command.CommandText = "SELECT Id, Operator FROM Operators ORDER BY Operator";
                SQLiteDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    Operator oper = new Operator();
                    oper.Id = rdr.GetInt32(0);
                    oper.Name = rdr.GetString(1);
                    operators.Add(oper);
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }
            return operators;
        }
        #endregion
        #region equipment data requests
        public List<Equipment> Equipments()
        {
            List<Equipment> equipments = new List<Equipment>();
            try
            {
                dbConnection.Open();
                command = dbConnection.CreateCommand();
                command.CommandText = "SELECT Id, Department, EquipmentName, InventoryId FROM Equipments WHERE Current=1 ORDER BY InventoryId";
                SQLiteDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    Equipment equipment = new Equipment();
                    equipment.Id = rdr.GetInt32(0);
                    equipment.Department = rdr.GetString(1);
                    equipment.EquipmentName = rdr.GetString(2);
                    equipment.InventoryId = rdr.GetString(3);
                    equipments.Add(equipment);
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }
            return equipments;
        }
        public void EquipmentUpdate(Equipment equipment)
        {
            dbConnection.Open();
            SQLiteTransaction qLiteTransaction = dbConnection.BeginTransaction();
            command = dbConnection.CreateCommand();
            command.Transaction = qLiteTransaction;
            try
            {
                command.CommandText = "UPDATE Equipments SET Current = 0 WHERE Id = '" + equipment.Id + "'";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO Equipments (Department, EquipmentName, InventoryId, Current) VALUES " +
                    "('" + equipment.Department + "', '" + equipment.EquipmentName + "', '" + equipment.InventoryId + "', 1)";
                command.ExecuteNonQuery();

                qLiteTransaction.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                qLiteTransaction.Rollback();
            }
            finally
            {
                dbConnection.Close();
            }
        }
        public int EquipmentIsert(Equipment equipment)
        {
            dbConnection.Open();
            SQLiteTransaction qLiteTransaction = dbConnection.BeginTransaction();
            command = dbConnection.CreateCommand();
            command.Transaction = qLiteTransaction;
            try
            {
                command.CommandText = "INSERT INTO Equipments (Department, EquipmentName, InventoryId, Current) VALUES " +
                    "('" + equipment.Department + "', '" + equipment.EquipmentName + "', '" + equipment.InventoryId + "', 1);";
                command.ExecuteNonQuery();

                command.CommandText = "SELECT last_insert_rowid();";
                long rowid = (long)command.ExecuteScalar();
                
                qLiteTransaction.Commit();
                return (int)rowid;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                qLiteTransaction.Rollback();
                return -1;
            }
            finally
            {
                dbConnection.Close();
            }
        }
        public bool EquipmentExists(string inventoryNr)
        {
            try
            {
                dbConnection.Open();

                command.CommandText = "SELECT COUNT(*) FROM Equipments WHERE InventoryId = '" + inventoryNr + "' AND Current = '1'";// COLLATE NOCASE
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                dbConnection.Close();
            }
        }
        public Equipment GetEquipmentById(string inventoryNr)
        {
            Equipment equipment = new Equipment();
            try
            {
                dbConnection.Open();

                command = dbConnection.CreateCommand();
                command.CommandText = "SELECT Id, Department, EquipmentName, InventoryId FROM Equipments WHERE Current=1 AND InventoryId = '" + inventoryNr + "'";
                SQLiteDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    equipment.Id = rdr.GetInt32(0);
                    equipment.Department = rdr.GetString(1);
                    equipment.EquipmentName = rdr.GetString(2);
                    equipment.InventoryId = rdr.GetString(3);
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                equipment.Clear();
            }
            finally
            {
                dbConnection.Close();
            }
            return equipment;
        }
        public bool ExcludeEquipment(List<Equipment> equipments)
        {
            try
            {
                dbConnection.Open();
                for (int i = 0; i < equipments.Count; i++)
                {
                    command.CommandText = "UPDATE Equipments SET Current = 0 WHERE Id = '" + equipments[i].Id + "'";
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                dbConnection.Close();
            }
        }
        #endregion
        #region Новые задачи
        public DateTime StartdDate()
        {
            DateTime dt = DateTime.Now;
            dbConnection.Open();
            command = dbConnection.CreateCommand();
            try
            {
                command.CommandText = "SELECT MIN(DateOfChange) FROM Overview ;";
                SQLiteDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    dt = rdr.GetDateTime(0);
                }
                rdr.Close();

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return dt;
            }
            finally
            {
                dbConnection.Close();
            }
        }
        public bool RequestToAddNewTask(DateTime receiptDateForRepair, string causeOfFailure, string solution,
            DateTime? dateOfExitFromRepair, int equipmentId, int operatorId)
        {
            string strdateOfExitFromRepair = dateOfExitFromRepair == null ? "NULL" : "'" + Convert.ToDateTime(dateOfExitFromRepair).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
            
            dbConnection.Open();
            SQLiteTransaction qLiteTransaction = dbConnection.BeginTransaction();
            command = dbConnection.CreateCommand();
            command.Transaction = qLiteTransaction;
            try
            {
                command.CommandText = "INSERT INTO DownTime (ReceiptDateForRepair, CauseOfFailure, Solution, DateOfExitFromRepair) " +
                    "VALUES ('" + receiptDateForRepair.ToString("yyyy-MM-dd HH:mm:ss.fff") + "', '" + causeOfFailure + 
                    "', '" + solution + "', " + strdateOfExitFromRepair + ");";
                command.ExecuteNonQuery();
                command.CommandText = "SELECT last_insert_rowid();";
                long dtrid = (long)command.ExecuteScalar();

                command.CommandText = "SELECT IFNULL(MAX(Id),-1) FROM Overview";
                long ovid = (long)command.ExecuteScalar() + 1;
                command.CommandText = "INSERT INTO Overview (Id, DownTimeId, EquipmentId, OperatorId, State, DateOfChange) " +
                    "VALUES ('" + ovid + "','" + dtrid + "', '" + equipmentId + "', '" + operatorId + "', '0', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "');";
                command.ExecuteNonQuery();

                qLiteTransaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                qLiteTransaction.Rollback();
                return false;
            }
            finally
            {
                dbConnection.Close();
            }
        }
        public void ChangeTaskState(int id, int stateId)
        {
            try
            {
                dbConnection.Open();
                command.CommandText = "INSERT INTO Overview (Id, DownTimeId, EquipmentId, OperatorId, State, DateOfChange) " +
                    "SELECT Id, DownTimeId, EquipmentId, OperatorId, '" + stateId + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +
                    "' FROM Overview WHERE Id = '" + id + "'";
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }
        }
        #endregion
    }
}
