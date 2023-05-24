using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using static System.Collections.Specialized.BitVector32;

namespace DeckHullScanner.Models
{
    public class lookupClass
    {
        public string usercode { get; set; }
        public string endUnit { get; set; }
        public string modelName { get; set; }
        public string startDate { get; set; }
        public string item_no { get; set; }
        public string item_no_ext
        {
            get
            {
                if (item_no != null)
                {
                    //return item_no.Length > 10 ? item_no.Substring(0, 3) + "-" + item_no.Substring(3, 5) + "-" + item_no.Substring(8, 2) + "-" + item_no.Substring(10, 2) + "-" + item_no.Substring(12, 2) : null;
                    return item_no.Length > 10 ? item_no.Substring(0, 3) + "-" + item_no.Substring(3, 5) + "-" + item_no.Substring(8, 2) : null;

                }
                else return null;
            }
        }
        public string itemdesc { get; set; }
        public string station { get; set; }
        public string job_Status { get; set; }
        public decimal totalRequired { get; set; }
        public DateTime status_datetime { get; set; }

    }
}
