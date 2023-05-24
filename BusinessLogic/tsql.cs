using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Linq;
using System.Collections;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace DeckHullScanner.BusinessLogic
{
	public class TSQL
	{
		private string _connectionString; 
		private string _pySAPConnection; 
		private SqlConnection _sqlConnection;
		private SqlDataAdapter _sqlDataAdapter;
		/// <summary>
		/// Creates a new instance of the object using a SQL user name and password
		/// </summary>
		/// <param name="Server">The SQL Server to connect to</param>
		/// <param name="Database">The database to use</param>
		public TSQL(IConfiguration configuration)
		{
			//string dbserver = Startup.ConfigurationHelper.config["dbserver"];
			//ar dbserver = _configuration["dbserver"];
			//var dbname = Startup.ConfigurationHelper.config["dbname"];
			//var dbuser = Startup.ConfigurationHelper.config["dbuser"];
			//var dbpass = Startup.ConfigurationHelper.config["dbpass"];
			//_connectionString = "Server=" + dbserver +
			//		";Database=" + dbname +
			//		";Trusted_Connection=false;Password=" + dbpass +
			//		";User ID=" + dbuser;
         //   _connectionString = Startup.ConfigurationHelper.config[configName];
            _connectionString = configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            _pySAPConnection = configuration.GetValue<string>("ConnectionStrings:PySAPConnection");
            //try to connect to see if connection string is valid.  Let the exception fall to the
            //initiating call
            try
			{
				_sqlConnection = new SqlConnection(_connectionString);
				_sqlConnection.Open();
			}
			//catch (Exception ex)
			finally
			{
				//throw new Exception("Error connecting to database", ex);
				//Console.Log("Error connection to YCENTRAL DB: " + ex.Message);
			}
		}
		//public TSQL(string Server, string Database)
		//{


		//	_connectionString = "Data Source=" + Server +
		//		";Initial Catalog=" + Database +
		//		";Trusted_Connection=SSPI";

		//	//try to connect to see if connection string is valid.  Let the exception fall to the
		//	//initiating call
		//	try
		//	{
		//		_sqlConnection = new SqlConnection(_connectionString);
		//		_sqlConnection.Open();
		//	}
		//	catch (Exception ex)
		//	{
		//		throw new Exception("Error connecting to database", ex);
		//	}
		//}
		//public TSQL(string Server, string Database, string UserID, string Password)
		//{
		//	var dbserver = Startup.ConfigurationHelper.config["dbserver"];
		//	var dabname = Startup.ConfigurationHelper.config["dabname"];
		//	var dbuser = Startup.ConfigurationHelper.config["dbuser"];
		//	var dbpass = Startup.ConfigurationHelper.config["dbpass"];

		//	if (dbserver != null)	
				
		//	_connectionString = "Server=" + Server +
		//		";Database=" + Database +
		//		";Trusted_Connection=false;Password=" + Password +
		//		";User ID=" + UserID;

		//	//try to connect to see if connection string is valid.  Let the exception fall to the
		//	//initiating call
		//	try
		//	{
		//		_sqlConnection = new SqlConnection(_connectionString);
		//		_sqlConnection.Open();
		//	}
		//	catch (Exception ex)
		//	{
		//		throw new Exception("Error connecting to database", ex);
		//		//Console.Log("Error connection to YCENTRAL DB: " + ex.Message);
		//	}
		//}

		/// <summary>
		/// Clean up any resources being used
		/// </summary>
		//public void Dispose()
		//{
		//    if (_sqlConnection.State != System.Data.ConnectionState.Closed)
		//        _sqlConnection.Close();
		//}

		/// <summary>
		/// Executes a TSQL script on the current connection
		/// </summary>
		/// <param name="Command">The TSQL command to execute</param>
		/// <param name="Type">The type of TSQL command</param>
		/// <returns>Returns the resutls of query.  For a non-query, it will return the rows affected</returns>
		public IEnumerable<T> ExecuteTSQL<T>(string Command, CommandTypes Type, string useConnectionString = "default")
		{
			if(useConnectionString == "pysap")
                _sqlConnection = new SqlConnection(_pySAPConnection);

            //make sure that the Command is not null
            if (Command == "" || Command == string.Empty)
				throw new Exception("Error executing SQL script", new Exception("Command cannot be empty"));
			//make sure that there is a connection
			if (_sqlConnection.State != ConnectionState.Open)
			{
				_sqlConnection.Open();
			}
			try
			{
				if (Type == CommandTypes.Query)
				{
					_sqlDataAdapter = new SqlDataAdapter();
					_sqlDataAdapter.SelectCommand = new SqlCommand();
					_sqlDataAdapter.SelectCommand.CommandText = Command;
					_sqlDataAdapter.SelectCommand.CommandType = CommandType.Text;
					_sqlDataAdapter.SelectCommand.Connection = _sqlConnection;
					DataTable dtResult = new DataTable();
					_sqlDataAdapter.Fill(dtResult);
					_sqlConnection.Close();
					return GetEntities<T>(dtResult);
				}
				else
				{
					_sqlDataAdapter = new SqlDataAdapter();
					_sqlDataAdapter.SelectCommand = new SqlCommand();
					_sqlDataAdapter.SelectCommand.CommandText = Command;
					_sqlDataAdapter.SelectCommand.CommandType = CommandType.Text;
					_sqlDataAdapter.SelectCommand.Connection = _sqlConnection;
					int rows = _sqlDataAdapter.SelectCommand.ExecuteNonQuery();
					DataTable dtResult = new DataTable();
					DataColumn dc = new DataColumn("Rows affected", typeof(int));
					dtResult.Columns.Add(dc);
					DataRow dr = dtResult.NewRow();
					dr[0] = rows;
					dtResult.Rows.Add(dr);
					_sqlConnection.Close();
					return GetEntities<T>(dtResult);
				}
			}
			//catch (Exception ex)
			finally
			{
				//throw new Exception("Error executing TSQL command: " + Command, ex);
				//DataTable dtResult = new DataTable();
				//DataColumn dc = new DataColumn("error", typeof(int));
				//dtResult.Columns.Add(dc);
				//DataColumn dc2 = new DataColumn("message", typeof(string));
				//dtResult.Columns.Add(dc2);
				//DataRow dr = dtResult.NewRow();
				//dr[0] = 1;
				//dr[1] = ex.Message;
				//dtResult.Rows.Add(dr);
				//return GetEntities<T>(dtResult);
			}
		}

		/// <summary>
		/// Executes a SQL stored procedure.
		/// </summary>
		/// <param name="SP_Name">The name of the stored procedure</param>
		/// <param name="Parameters">An array of parameters.  The [0,x] index is the Parameter Name and the [1,x] index 
		/// is the value.</param>
		/// <returns></returns>
		/// 
		public DataTable ExecuteStoredProcedure(string SP_Name, object[,] Parameters)
		{
		    try
		    {
		        //make sure that the SP_Name is not null
		        if (SP_Name == "" || SP_Name == string.Empty)
		            throw new Exception("Error executing SQL stored procedure", new Exception("The name cannot be null"));
		        //make sure that there is a connection
		        if (_sqlConnection.State != ConnectionState.Open)
					_sqlConnection.Open();

				_sqlDataAdapter = new SqlDataAdapter();
		        _sqlDataAdapter.SelectCommand = new SqlCommand();

		        for (int i = 0; i < Parameters.Length / 2; i++)
		            _sqlDataAdapter.SelectCommand.Parameters.Add(
		                new SqlParameter(Parameters[0, i].ToString(), Parameters[1, i]));

		        _sqlDataAdapter.SelectCommand.CommandText = SP_Name;
		        _sqlDataAdapter.SelectCommand.Connection = _sqlConnection;
		        _sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
		        DataTable dtResults = new DataTable();
		        _sqlDataAdapter.Fill(dtResults);
				_sqlConnection.Close();
				return dtResults;
		    }
		    catch (Exception ex)
		    {
		        throw new Exception("Error executing stored procedure", ex);
		    }
		}

		/// <summary>
		/// Executes a SQL stored procedure.
		/// </summary>
		/// <param name="SP_Name">The name of the stored procedure</param>
		/// <returns></returns>
		public DataTable ExecuteStoredProcedure(string SP_Name)
		{
		    return ExecuteStoredProcedure(SP_Name, new object[0, 0]);
		}
		public IEnumerable<T> ExecuteSP<T>(string SP_Name, object[,] Parameters)
		{
			return GetEntities<T>(ExecuteStoredProcedure(SP_Name, Parameters));
		}

		/// <summary>
		/// Changes the current database
		/// </summary>
		/// <param name="Database">The name of the database to change to</param>
		//public void ChangeDatabase(string Database)
		//{
		//    //make sure that the Database is not null
		//    if (Database == "" || Database == string.Empty)
		//        throw new Exception("Error executing SQL script", new Exception("Command cannot be null"));
		//    //make sure that there is a connection
		//    if (_sqlConnection.State != ConnectionState.Open)
		//        throw new Exception("Error executing SQL script", new Exception("Connection to SQL does not exist"));

		//    try
		//    {
		//        _sqlConnection.ChangeDatabase(Database);
		//    }
		//    catch (Exception ex)
		//    {
		//        throw new Exception("Error changing database", ex);
		//    }
		//}

		/// <summary>
		/// Inserts values into a table
		/// </summary>
		/// <param name="Values">The data table holdind the values</param>
		/// <param name="Mapping">The data table holding the mapping information.  The mapping table should hold
		/// the database column name in the first column (string) and the value table column number in the second
		/// column.</param>
		/// <param name="Table">The name of the database table to store the values</param>
		//public void InsertValues(DataTable Values, DataTable Mapping, string Table)
		//{
		//    string CommandText;

		//    SqlTransaction myTrans;
		//    myTrans = _sqlConnection.BeginTransaction();

		//    _sqlDataAdapter = new SqlDataAdapter();
		//    _sqlDataAdapter.InsertCommand = new SqlCommand();
		//    _sqlDataAdapter.InsertCommand.Transaction = myTrans;
		//    _sqlDataAdapter.InsertCommand.CommandType = CommandType.Text;
		//    _sqlDataAdapter.InsertCommand.Connection = _sqlConnection;

		//    //begin updated the database
		//    try
		//    {
		//        for (int i = 0; i < Values.Rows.Count; i++)
		//        {
		//            CommandText = "INSERT INTO " + Table + "(";
		//            for (int j = 0; j < Mapping.Rows.Count; j++)
		//                CommandText += Mapping.Rows[j][0].ToString() + ", ";

		//            //remove the last ", " if it exist
		//            if (CommandText.EndsWith(", ")) CommandText = CommandText.Substring(0, CommandText.Length - 2);

		//            CommandText += ") VALUES(";
		//            //add the values
		//            for (int j = 0; j < Mapping.Rows.Count; j++)
		//            {
		//                if (Values.Rows[i].IsNull(Convert.ToInt32(Mapping.Rows[j][1])))
		//                    CommandText += "null, ";
		//                else
		//                    CommandText += "'" + Values.Rows[i][Convert.ToInt32(Mapping.Rows[j][1])].ToString() +
		//                        "', ";
		//            }
		//            //remove the last "', " if it exist
		//            if (CommandText.EndsWith("', ")) CommandText = CommandText.Substring(0, CommandText.Length - 2);
		//            CommandText += ")";
		//            _sqlDataAdapter.InsertCommand.CommandText = CommandText;
		//            _sqlDataAdapter.InsertCommand.ExecuteNonQuery();
		//        }
		//        myTrans.Commit();
		//    }
		//    catch (Exception ex)
		//    {
		//        myTrans.Rollback();
		//        throw new Exception("Error inserting data", ex);
		//    }
		//}

		public ConnectionState State { get { return _sqlConnection.State; } }

		public enum CommandTypes
		{
			Insert,
			Delete,
			Update,
			Query
		}

		/// Get entities from DataTable
		/// </summary>
		/// <typeparam name="T">Type of entity</typeparam>
		/// <param name="dt">DataTable</param>
		/// <returns></returns>
		public IEnumerable<T> GetEntities<T>(DataTable dt)
		{
			if (dt == null)
			{
				return null;
			}

			List<T> returnValue = new List<T>();
			List<string> typeProperties = new List<string>();

			T typeInstance = Activator.CreateInstance<T>();

			foreach (DataColumn column in dt.Columns)
			{

				//var prop = typeInstance.GetType().GetProperty(column.ColumnName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
				var prop = typeInstance.GetType().GetProperty(column.ColumnName);
				//typeProperties.Add(column.ColumnName);

				if (prop != null)
				{
					typeProperties.Add(column.ColumnName);
				}
			}

			foreach (DataRow row in dt.Rows)
			{
				T entity = Activator.CreateInstance<T>();

				foreach (var propertyName in typeProperties)
				{

					if (row[propertyName] != DBNull.Value)
					{
						string str = row[propertyName].GetType().FullName;

						if (entity.GetType().GetProperty(propertyName).PropertyType == typeof(System.String))
						{
							object Val = row[propertyName].ToString();
							entity.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).SetValue(entity, Val, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, null, null);
						}
						else if (entity.GetType().GetProperty(propertyName).PropertyType == typeof(System.Guid))
						{
							object Val = Guid.Parse(row[propertyName].ToString());
							entity.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).SetValue(entity, Val, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, null, null);
						}
						else
						{
							entity.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).SetValue(entity, row[propertyName], BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, null, null);
						}
					}
					else
					{
						entity.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).SetValue(entity, null, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, null, null);
					}
				}

				returnValue.Add(entity);
			}

			return returnValue.AsEnumerable();
		}
	}
}
