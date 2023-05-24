using DeckHullScanner.Interface;
using DeckHullScanner.Models;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace DeckHullScanner.BusinessLogic
{
    public class Services : IServices
    {
        private IConfiguration _configuration { get; set; }
        private string _connectionString { get; set; }

        public Services(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
        }

        public string GetUser(string UserNumber)
        {
            string username = String.Empty;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sqlQuery = "Select top 1 EMP_lastname from sapscaleusers WHERE emp_number = @EmpNumber";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@EmpNumber", UserNumber);
                        con.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            username = UserNumber;
                        }
                        else
                        {
                            username = "Bad";
                        }
                        con.Close();
                    }
                    catch (Exception Ex)
                    {
                        if (con != null && con.State == ConnectionState.Open)
                            con.Close();
                    }
                    finally
                    {
                        if (con != null && con.State == ConnectionState.Open)
                            con.Close();
                    }
                }
            }
            return username;
        }

        public bool ValidateEndUnitPartNumber(string EndUnitPartNumber)
        {
            bool IsEndUnitPartNumberValid = false;
            if (EndUnitPartNumber.Contains("~"))
            {
                IsEndUnitPartNumberValid = true;
            }
            return IsEndUnitPartNumberValid;
        }

        public string GetDataWV(string sEndUnitPartNumber, string sItemPartNumber, string sCardNumber, string sEmpNumber)
        {
            string returnString = string.Empty;

            string CurTime = string.Empty;
            string MOWorkstation = string.Empty;
            string MO = string.Empty;
            string WorkStation = string.Empty;
            string Engine = string.Empty;
            int SerialNo_Position = 0;
            string StartDate = string.Empty;
            string EndUnitSerialNumber = string.Empty;
            string EndUnitPartNumber = string.Empty;
            string PrevEndUnitSerialNumber = string.Empty;
            string PrevEndUnitPartNumber = string.Empty;
            string StartEUSerialNumber = string.Empty;
            string EngineNo = string.Empty;
            string End_Unit = string.Empty;
            string PartNumber = string.Empty;
            string MOWorkStationString = string.Empty;
            string EUPartNumberString = string.Empty;
            string PrevEUPartNumberString = string.Empty;
            string Nakago = string.Empty;
            bool IsNakago = false;
            string PrevEUPN = string.Empty;


            try
            {

                CurTime = DateTime.Now.ToString("M/d/yyyy HH:mm:ss");
                EUPartNumberString = sEndUnitPartNumber;
                PrevEUPartNumberString = sEmpNumber;
                EndUnitPartNumber = EUPartNumberString.Split("~")[0];
                EndUnitSerialNumber = EUPartNumberString.Split("~")[1];
                PrevEndUnitPartNumber = PrevEUPartNumberString.Split("~")[0];
                if(PrevEUPartNumberString.IndexOf("~") != -1)
                    PrevEndUnitSerialNumber = PrevEUPartNumberString.Split("~")[1];
                PartNumber = sItemPartNumber;
                MOWorkStationString = sCardNumber;

                if (PartNumber.Length != 0)
                {
                    Nakago = PartNumber.Substring(3, 5);
                }

                if (MOWorkStationString.Length != 0)
                {
                    MOWorkstation = GetMOWorkStationData("ZPWM_VALIDATE_CARD", MOWorkStationString, "", "", "");
                    if (MOWorkstation.Split("|").Length > 3)
                    {
                        MO = MOWorkstation.Split("|")[3];
                        WorkStation = MOWorkstation.Split("|")[2];
                    }
                    Console.WriteLine(MO);
                }

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string sqlQuery = "select nakago from Nakago where nakago = @Nakago";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        try
                        {
                            cmd.Parameters.AddWithValue("@Nakago", Nakago);
                            con.Open();
                            SqlDataReader rdr = cmd.ExecuteReader();
                            if (rdr.HasRows)
                            {
                                IsNakago = true;
                            }
                            else
                            {
                                IsNakago = false;
                            }
                            con.Close();
                        }
                        catch (Exception Ex)
                        {
                            if (con != null && con.State == ConnectionState.Open)
                                con.Close();
                        }
                        finally
                        {
                            if (con != null && con.State == ConnectionState.Open)
                                con.Close();
                        }
                    }
                }

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string sqlQuery = "Select top 1 SERIALNO_POS from VIC_INT_BDBSASSYPL_ENDSN WHERE PART_NO = @EndUnitPartNumber";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        try
                        {
                            cmd.Parameters.AddWithValue("@EndUnitPartNumber", EndUnitPartNumber);
                            con.Open();
                            SqlDataReader rdr = cmd.ExecuteReader();
                            if (rdr.HasRows)
                            {
                                while (rdr.Read())
                                {
                                    SerialNo_Position = rdr.GetInt32(0);
                                }
                                StartEUSerialNumber = EndUnitSerialNumber.Substring(SerialNo_Position - 1);
                                StartEUSerialNumber = StartEUSerialNumber.Replace("*", "");
                            }
                            else
                            {
                                IsNakago = false;
                            }
                            con.Close();
                        }
                        catch (Exception Ex)
                        {
                            if (con != null && con.State == ConnectionState.Open)
                                con.Close();
                        }
                        finally
                        {
                            if (con != null && con.State == ConnectionState.Open)
                                con.Close();
                        }
                    }
                }


                if (sItemPartNumber.Substring(0, 1).ToLower() == "P" && sItemPartNumber.Length == 15)
                    PartNumber = sItemPartNumber.Substring(1, 14);

                if (sItemPartNumber.Substring(0, 2) == "35")
                {
                    string leftside = sItemPartNumber.Substring(2, 3);
                    string rightside = sItemPartNumber.Substring(7, 7);
                    string StampKey = string.Empty;
                    StampKey = leftside + "  " + rightside;
                    OracleConnection dbEngine = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=pdvicdb3)(PORT=12102)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=PDVICS01)));User Id=PYMAC3_YMMC_PUB2;Password=Z2kt74!;");
                    dbEngine.Open();
                    OracleCommand oraCommandEngine = new OracleCommand("Select ENGINE_NO from VIC_INT_PARTS_ITEM WHERE (STAMP_KEY = '" + StampKey + "') ", dbEngine);
                    oraCommandEngine.BindByName = true;
                    OracleDataReader oraReaderEngine = oraCommandEngine.ExecuteReader();
                    if (oraReaderEngine.HasRows)
                    {
                        while (oraReaderEngine.Read())
                        {
                            EngineNo = oraReaderEngine.GetString(0);
                        }
                    }
                    oraReaderEngine.Close();
                    dbEngine.Close();
                    dbEngine.Dispose();
                    PartNumber = EngineNo.Trim() + "0080";
                }

                using (SqlConnection dbStartDate = new SqlConnection(_connectionString))
                {
                    string sqlQuery = "Select Start_date from VIC_INT_BDBSASSYPL_ENDSN WHERE PART_NO = '" + EndUnitPartNumber + " 'AND START_SERIALNO <= '" + StartEUSerialNumber + "' AND END_SN >= '" + StartEUSerialNumber + "' ORDER BY START_DATE DESC";
                    using (SqlCommand sqlCommandStartDate = new SqlCommand(sqlQuery, dbStartDate))
                    {
                        try
                        {
                            dbStartDate.Open();
                            SqlDataReader sqlReaderStartDate = sqlCommandStartDate.ExecuteReader();
                            if (sqlReaderStartDate.HasRows)
                            {
                                while (sqlReaderStartDate.Read())
                                {
                                    StartDate = sqlReaderStartDate.GetString(0);
                                }
                            }

                            dbStartDate.Close();
                        }
                        catch (Exception Ex)
                        {
                            if (dbStartDate != null && dbStartDate.State == ConnectionState.Open)
                                dbStartDate.Close();
                        }
                        finally
                        {
                            if (dbStartDate != null && dbStartDate.State == ConnectionState.Open)
                                dbStartDate.Close();
                        }
                    }
                }




                using (OracleConnection dbEndUnit = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=pdymmc03.ymus.yamaha-motor.com)(PORT=9010)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=PDPYSAP12)));User Id=YMMC_LINVAL_01;Password=Trx7kqp65;"))
                {
                    dbEndUnit.Open();
                    using (OracleCommand oraCommandEndUnit = new OracleCommand("Select END_UNIT from PYBOM WHERE (substr(item_no,2,10) = '"+ PartNumber.Substring(0, 10) +"' or substr(item_no,1,10) = '"+ PartNumber.Substring(0, 10) +"') and END_UNIT = '"+ EndUnitPartNumber +"' and Start_Date ='"+StartDate+"'", dbEndUnit))
                    {
                        oraCommandEndUnit.BindByName = true;
                        OracleDataReader oraReaderEndUnit = oraCommandEndUnit.ExecuteReader();
                        if (oraReaderEndUnit.HasRows)
                        {
                            while (oraReaderEndUnit.Read())
                            {
                                End_Unit = oraReaderEndUnit.GetString(0);
                            }
                        }
                        oraReaderEndUnit.Close();
                    }
                    dbEndUnit.Close();
                }

                End_Unit = End_Unit.Replace(" ", "");


                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        if (End_Unit == EndUnitPartNumber)
                        {
                            if (IsNakago) {
                                con.Open();
                                cmd.Connection = con;
                                cmd.CommandText = "Insert into LinesideValidationLog ([User], [DateTime], [EUPN], [PYIN], [MSG], [Workstation], [MO_Number],[Nakago],[Scanned],[PrevEUPN]) values ('" + sEmpNumber + "', '" +CurTime + "', '" + EUPartNumberString +"', '" +PartNumber +"', 'Good','"+ WorkStation +"','"+ MO +"',1,1,'"+ PrevEUPartNumberString +"')";
                                cmd.ExecuteNonQuery();
                                con.Close();
                                return $"Good" + "|" + CurTime + "|" + WorkStation;
                            }
                            else
                            {
                                con.Open();
                                cmd.Connection = con;
                                cmd.CommandText = "Insert into LinesideValidationLog ([User], [DateTime], [EUPN], [PYIN], [MSG], [Workstation], [MO_Number],[Nakago],[Scanned],[PrevEUPN]) values ('" + sEmpNumber + "', '" + CurTime + "', '"+ EUPartNumberString + "', '" + PartNumber + "', 'Good','" + WorkStation +"','" + MO + "',0,1,'" + PrevEUPartNumberString +"')";
                                cmd.ExecuteNonQuery();
                                con.Close();
                                return "Bad" + "|" + CurTime + "|" + "None";
                            }
                        }
                        else
                        {
                            con.Open();
                            cmd.Connection = con;
                            cmd.CommandText = "Insert into LinesideValidationLog ([User], [DateTime], [EUPN], [PYIN], [MSG], [Workstation], [MO_Number],[Nakago],[Scanned],[PrevEUPN]) values ('" + sEmpNumber + "', '" + CurTime + "', '" + EUPartNumberString + "', '" + PartNumber + "', 'Bad','" + WorkStation + "','" + MO + "',0,0,'" + PrevEUPartNumberString + "')";
                            cmd.ExecuteNonQuery();
                            con.Close();
                            return "Bad" + "|" + CurTime + "|" + "None";
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        con.Open();
                        cmd.Connection = con;
                   //     cmd.CommandText = "Insert into LinesideValidationLog ([User], [DateTime], [EUPN], [PYIN], [MSG], [Workstation], [MO_Number]) values ('" + sEmpNumber + "', '" +CurTime + "', '" + EUPartNumberString + "', '" + PartNumber+ "', 'Bad','" + WorkStation + "','" +MO +"',0,0,'" + PrevEUPartNumberString +"')";
                        cmd.CommandText = "Insert into LinesideValidationLog ([User], [DateTime], [EUPN], [PYIN], [MSG], [Workstation], [MO_Number]) values ('" + sEmpNumber + "', '" +CurTime + "', '" + EUPartNumberString + "', '" + PartNumber+ "', 'Bad','" + WorkStation + "','" +MO +"')";
                        cmd.ExecuteNonQuery();
                        con.Close();
                        return "Bad" + "|" + CurTime + "|" + "None";
                    }
                }


            }

            return returnString;
        }
        
        public string GetMOWorkStationData(string rfcname, string value1, string value2, string value3, string value4) {
      //      SapNCo sapinfo = new SapNCo();
            string SAPReturnValue = String.Empty;
       //     bool issap = sapinfo.InitSap();
            // 'If issap Then  for some strange reason does net dispose the object like c# does 
            if (rfcname.Contains("ZPWM_VALIDATE_CARD"))
            {
        //        SAPReturnValue = sapinfo.GetCardDataWithMOWorkStation(value1);
            }
            return SAPReturnValue;
        }

    }
}
