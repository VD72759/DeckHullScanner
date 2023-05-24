using DeckHullScanner.BusinessLogic;
using DeckHullScanner.Interface;
using DeckHullScanner.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace DeckHullScanner.Controllers
{
    public class AjaxController : Controller
    {
        private AjaxResponse _ajaxResponse { get; set; }

        private IHttpContextAccessor _iHttpContextAccessor { get; set; }
        private IUtility _iUtility { get; set; }
        private IServices _iServices { get; set; }
        private string _partSupplyCard { get; set; }

        public AjaxController(AjaxResponse ajaxResponse, IHttpContextAccessor iHttpContextAccessor,
             IUtility iUtility, IServices iServices)
        {
            _ajaxResponse = ajaxResponse;
            _iHttpContextAccessor = iHttpContextAccessor;
            _iUtility = iUtility;
            _iServices = iServices;
        }

        public AjaxResponse CheckEmployeeExists([FromBody]EmployeeBasicDetails employeeBasicDetails)
        {
            _ajaxResponse.status = "1";
            _ajaxResponse.message = "User details exists";
            
            if(employeeBasicDetails != null && !string.IsNullOrEmpty(employeeBasicDetails.EmpId))
            {
                string EmpId = employeeBasicDetails.EmpId;
                string EmpType = employeeBasicDetails.EmpType;
                _iHttpContextAccessor.HttpContext.Session.SetString("User", EmpId);

                try
                {
                    string returnstring = string.Empty;
                    if (EmpType.ToUpper() == "FULL")
                    {
                        string user = "00" + EmpId;
                        returnstring = _iServices.GetUser(user);
                        if (returnstring == user)
                        {
                            _iHttpContextAccessor.HttpContext.Session.SetString("EmployeeNumber", returnstring);
                 //           Response.Redirect("MainPage.aspx", true);

                            _ajaxResponse.status = "1";
                            _ajaxResponse.message = "User Information Found";
                            _ajaxResponse.redirecturl = Url.Action("MainPage", "Home");
                        }
                        else
                        {
                            _ajaxResponse.status = "0";
                            _ajaxResponse.message = "User Not Found";
                        }

                    }
                    else if (EmpType.ToUpper() == "TEMPORARY")
                    {
                        if (EmpId.Length == 5)
                        {
                            string user = "09" + EmpId;
                            returnstring = _iServices.GetUser(user);
                            if (returnstring == user)
                            {
                                _iHttpContextAccessor.HttpContext.Session.SetString("EmployeeNumber", returnstring);
                                _ajaxResponse.status = "1";
                                _ajaxResponse.message = "User Information Found";
                                _ajaxResponse.redirecturl = Url.Action("MainPage", "Home");

                            }
                            else
                            {
                                _ajaxResponse.status = "0";
                                _ajaxResponse.message = "User Not Found";
                            }

                        }
                        else if (EmpId.Length == 4)
                        {
                            string user = EmpId;
                            returnstring = _iServices.GetUser(user);
                            if (returnstring == user)
                            {
                                _iHttpContextAccessor.HttpContext.Session.SetString("EmployeeNumber", returnstring);
                                _ajaxResponse.status = "1";
                                _ajaxResponse.message = "User Information Found";
                                _ajaxResponse.redirecturl = Url.Action("MainPage", "Home");

                            }
                            else
                            {
                                _ajaxResponse.status = "0";
                                _ajaxResponse.message = "User Not Found";
                            }

                        }
                        else
                        {
                            _ajaxResponse.status = "0";
                            _ajaxResponse.message = "Invalid User Number";
                        }
                    }
                    else if (EmpId != "99999")
                    {
                        _ajaxResponse.status = "0";
                        _ajaxResponse.message = "Select Employee Type";
                    }
                    else
                    {
                        _ajaxResponse.status = "0";
                        _ajaxResponse.message = _iUtility.GetVersion();
                    }
                }
                catch(Exception Ex)
                {
                    Console.WriteLine(Ex.Message);
                }
      
            }
            else
            {
                _ajaxResponse.status = "0";
                _ajaxResponse.message = "Employee Id cannot be empty";
            }


            return _ajaxResponse;
        }

        public AjaxResponse ValidateEndUnitPartNumber([FromBody] ItemPartNumberDetails partNumberDetails)
        {
            if (partNumberDetails != null && !string.IsNullOrEmpty(partNumberDetails.EndUnitPartNumber))
            {
                if(_iServices.ValidateEndUnitPartNumber(partNumberDetails.EndUnitPartNumber))
                {
                    _ajaxResponse.status = "1";
                    _ajaxResponse.message = "Valid Barcode";
                }
                else
                {
                    _ajaxResponse.status = "0";
                    _ajaxResponse.message = "Invalid Barcode";
                }

            }
            else
            {
                _ajaxResponse.status = "0";
                _ajaxResponse.message = "End Unit Part Number connot be empty";
            }
            return _ajaxResponse;
        }

        public AjaxResponse ValidateItemPartNumber([FromBody] ItemPartNumberDetails partNumberDetails)
        {
            string EndUnitPartNumber = partNumberDetails.EndUnitPartNumber;
            string EmpNumber = partNumberDetails.EmployeeNumber;
            string ItemPartNumber = string.Empty;
            string CardNumber = string.Empty;
            string ReturnString = string.Empty;
            string Status = string.Empty;
            string CurDateTime = string.Empty;

            if (partNumberDetails != null && !string.IsNullOrEmpty(partNumberDetails.ItemPartNumber))
            {
                ItemPartNumber = System.Text.RegularExpressions.Regex.Replace(partNumberDetails.ItemPartNumber, @"\p{C}+", string.Empty);
                
                if (ItemPartNumber.Length == 14)
                {
                    _partSupplyCard = ItemPartNumber;
                }
                else
                if (ItemPartNumber.Length == 15)
                {
                    _partSupplyCard = ItemPartNumber.Substring(1, 14);
                }
                else
                if (ItemPartNumber.Length > 15)
                {
                    _partSupplyCard = ItemPartNumber;
                    ItemPartNumber = ItemPartNumber.Substring(11, 14); // Part supply card
                }
                
                if (_partSupplyCard.Length > 15)
                {
                    CardNumber = GetCardNumber(_partSupplyCard);
                }

                ReturnString = _iServices.GetDataWV(EndUnitPartNumber, ItemPartNumber, CardNumber, EmpNumber);

                Status = ReturnString.Split('|').ElementAt(0);
                CurDateTime = ReturnString.Split('|').ElementAt(1);
                if (EndUnitPartNumber == "")
                {
                    _ajaxResponse.status = "-1";
                    _ajaxResponse.message = "End Unit Part Number not Entered";
                }
                else if (Status == "Good")
                {
                    _ajaxResponse.status = "1";
                    _ajaxResponse.message = "Validation successful";
                    _ajaxResponse.data = ItemPartNumber;

                }
                else
                {
                    _ajaxResponse.status = "0";
                    _ajaxResponse.message = "Parts " + EndUnitPartNumber + " and " + ItemPartNumber + " do not match";
                }
            }
            else
            {
                _ajaxResponse.status = "0";
                _ajaxResponse.message = "Item Part Number connot be empty";

            }


            return _ajaxResponse;
        }

        public AjaxResponse ApproveOverride([FromBody]ApproveOverrideDetails approveOverride)
        {
            if (IsUserSessionExists())
            {
                if (approveOverride == null || (approveOverride != null && (string.IsNullOrEmpty(approveOverride.EmpId) 
                                                                            || string.IsNullOrEmpty(approveOverride.ItemPartNumber)
                                                                            || string.IsNullOrEmpty(approveOverride.EndUnitPartNumber))))
                {
                    _ajaxResponse.status = "0";
                    _ajaxResponse.message = "Invalid data. Please check and try again";
                }
                else
                {
                    _ajaxResponse = Logout();
                }
            }
            else
            {
                _ajaxResponse.status = "0";
                _ajaxResponse.message = "Error encounter while validating the user";
                _ajaxResponse.redirecturl = (string)Url.Action("Index", "Home");
            }

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
                catch(Exception Ex)
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
        private string GetCardNumber(string psc)
        {
            // Determine if bar code is a part supply card, and AIAG card or Engine card
            string CardNumber = "";
            if (psc.Length == 49)
            {
                return CardNumber;
            }
            else if (psc.Length == 62)
            {
                CardNumber = psc.Substring(46, 16);
                return CardNumber;
            }
            else
            {
                CardNumber = psc.Substring(52, 16);
                return CardNumber;
            }
        }

    }
}
