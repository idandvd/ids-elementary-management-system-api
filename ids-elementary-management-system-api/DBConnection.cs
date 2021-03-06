﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace ids_elementary_management_system_api
{
    public class DBConnection
    {

        private readonly object connectionLock = new object();

        private string databaseName = "reshit";
        private MySqlConnection connection = null;
        private static DBConnection _instance = null;
        public static DBConnection Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DBConnection();
                return _instance;
            }
        }


        public MySqlConnection Connection
        {
            get { return connection; }
        }

        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        public string Password { get; set; }

        private DBConnection()
        {
        }

        public bool IsConnected
        {
            get
            {
                if (connection == null || connection.State != System.Data.ConnectionState.Open)
                {
                    return false;
                }
                return true;
            }
        }

        public void Connect()
        {
            if (!this.IsConnected)
            {
                try
                {
                    if (String.IsNullOrEmpty(databaseName))
                    {
                        return;
                    }
                    string strConnectionString = string.Format("Server=den1.mysql2.gear.host; database={0}; UID=reshit; password=Aa5407582@", databaseName);

                    //if (HttpContext.Current.Request.IsLocal && Environment.MachineName == "IDAN-PC")
                    {
                        strConnectionString = ConfigurationManager.ConnectionStrings["resheetDBConnectionString"].ConnectionString;
                    }

                    connection = new MySqlConnection(strConnectionString);
                    connection.Open();
                }
                catch (Exception e)
                {
                    e.GetType();
                }
            }
        }

        public void Close()
        {
            connection.Close();
        }

        /**
         * returns int
         * returns 0 if there was an error with saving
         * returns -1 if there is a duplication
         * otherwise returns the new item's Id
         */
        public int InsertData(string query)
        {
            int nInsertID = 0;
            lock (connectionLock)
            {
                Connect();
                if (IsConnected)
                {
                    try
                    {
                        MySqlCommand command = new MySqlCommand(query, this.connection);
                        command.ExecuteNonQuery();
                        nInsertID = Convert.ToInt32(command.LastInsertedId);
                    }
                    catch (MySqlException e)
                    {
                        if (e.Message.ToLower().Contains("duplicate"))
                            return -1;
                        return nInsertID;
                    }
                }
            }
            return nInsertID;
        }

        public bool RunQuery(string query)
        {
            lock (connectionLock)
            {
                Connect();
                if (IsConnected)
                {
                    try
                    {
                        MySqlCommand command = new MySqlCommand(query, this.connection);
                        command.ExecuteNonQuery();
                    }
                    catch (MySqlException e)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool UpdateData(string query)
        {

            lock (connectionLock)
            {
                Connect();
                if (IsConnected)
                {
                    try
                    {
                        MySqlCommand command = new MySqlCommand(query, this.connection);
                        command.ExecuteNonQuery();
                    }
                    catch (MySqlException e)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool Delete(string tableName,int id)
        {
            string query = "delete from " + tableName + " where id = " + id;
            lock (connectionLock)
            {
                Connect();
                if (IsConnected)
                {
                    try
                    {
                        MySqlCommand command = new MySqlCommand(query, this.connection);
                        command.ExecuteNonQuery();
                    }
                    catch (MySqlException e)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public DataTable GetDataTableByQuery(string strQuery, bool bWithKey = true)
        {
            lock (connectionLock)
            {
                this.Connect();
                DataTable dtTable = null;

                if (this.IsConnected)
                {
                    try
                    {
                        MySqlDataAdapter daAdapter = new MySqlDataAdapter(strQuery, connection);
                        if (bWithKey)
                        {
                            daAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                        }
                        dtTable = new DataTable();
                        daAdapter.Fill(dtTable);

                        //MySqlCommand command = new MySqlCommand(strQuery, this.connection);
                        //command.ExecuteNonQuery();
                    }
                    catch (MySqlException e)
                    {
                        return null;
                    }
                    finally
                    {
                        this.Close();

                    }
                }

                return dtTable;
            }

        }

        public DataTable GetClassSchedule(int id)
        {
            return GetDataTableByQuery("select * from Classes_Schedules where class_id = " + id);
        }

        public string GetPreference(string preferenceName)
        {
            return GetStringByQuery("select value from preferences where name = '"+ preferenceName + "'");
        }
        public bool SetPreference(string preferenceName,string value)
        {
            return UpdateData("update preferences set value = '" + value + "' where name = '" + preferenceName + "';");
        }
        public string GetStringByQuery(string strQuery)
        {
            return GetDataTableByQuery(strQuery).Rows[0][0].ToString();
        }

    }
}