using DeckHullScanner.BusinessLogic;
using DeckHullScanner.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeckHullScanner.Controllers
{
    public class Ajax1Controller : Controller
    {
        TSQL _sqlHelper;
        private AjaxResponse _ajaxResponse { get; set; }
        private IHttpContextAccessor _iHttpContextAccessor { get; set; }

        public Ajax1Controller(AjaxResponse ajaxResponse, IHttpContextAccessor iHttpContextAccessor, TSQL sqlHelper)
        {
            _ajaxResponse = ajaxResponse;
            _iHttpContextAccessor = iHttpContextAccessor;
            _sqlHelper = sqlHelper;
        }

        public AjaxResponse IsEndUnitNumberValid([FromBody]InputParmas inputParmas)
        {
            string retString = string.Empty;
            string EndUnitPartNumber = inputParmas.EndUnitPartNumber;

       //     _sqlHelper = new TSQL();
            string ExecutionCommand;
            if (string.IsNullOrEmpty(EndUnitPartNumber))
                ExecutionCommand = "Select top 1 *  from VIC_INT_BDBSASSYPL_ENDSN  where PART_NO = '" + EndUnitPartNumber.Substring(0, EndUnitPartNumber.Length > 11 ? 10 : EndUnitPartNumber.Length) + "' order by START_DATE desc";
            else
                ExecutionCommand = "[dbo].[sp_getEndUnit]  '" + EndUnitPartNumber + "'";
            var result = _sqlHelper.ExecuteTSQL<serialClass>(ExecutionCommand, TSQL.CommandTypes.Query);

            return _ajaxResponse;
        }

        public AjaxResponse IsItemPartNumberValid([FromBody]InputParmas inputParmas)
        {
            string retString = string.Empty;
            string ItemPartNumber = inputParmas.ItemPartNumber;
            string currentjob = inputParmas.currentjob;
            string currentStartDate = inputParmas.currentStartDate;
            ItemPartNumber = System.Text.RegularExpressions.Regex.Replace(ItemPartNumber, @"\p{C}+", string.Empty);

            //    _sqlHelper = new TSQL(sql_pysapp);
            string ExecutionCommand;
            if (string.IsNullOrEmpty(currentjob))
                ExecutionCommand = "[dbo].[sp_get_EndUnit_schedule_by_itemno]  '" + ItemPartNumber + "'";
            else
                ExecutionCommand = "[dbo].[sp_get_EndUnit_schedule_by_itemno]  '" + ItemPartNumber + "','" + currentjob + "','" + currentStartDate + "'";
            var result = _sqlHelper.ExecuteTSQL<lookupClass>(ExecutionCommand, TSQL.CommandTypes.Query, "pysap");

            return _ajaxResponse;
        }

        public AjaxResponse Logout()
        {
            if (IsUserSessionExists())
            {
                try
                {
                    _iHttpContextAccessor.HttpContext.Session.Clear();
                    _ajaxResponse.status = "1";
                    _ajaxResponse.message = "User Logged out successfully";
                }
                catch (Exception Ex)
                {
                    _ajaxResponse.status = "0";
                    _ajaxResponse.message = "Some error occoured";
                }
            }
            else
            {
                _ajaxResponse.status = "0";
                _ajaxResponse.message = "User not found. Redirecting to Home page";
            }
            _ajaxResponse.redirecturl = (string)Url.Action("Index", "Home");
            return _ajaxResponse;
        }

        private bool IsUserSessionExists()
        {
            var EmployeeNumber = _iHttpContextAccessor.HttpContext.Session.GetString("EmployeeNumber");
            if (!string.IsNullOrEmpty(EmployeeNumber))
                return true;
            return false;

        }

    }
}
