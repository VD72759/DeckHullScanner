namespace DeckHullScanner.Models
{
    public class EmployeeBasicDetails
    {
        public string EmpId { get; set; }
        public string EmpType { get; set; }
    }

    public class ItemPartNumberDetails
    {
        public string ItemPartNumber { get; set; }
        public string EndUnitPartNumber { get; set; }
        public string EmployeeNumber { get; set; }
    }
    public class ApproveOverrideDetails
    {
        public string EmpId { get; set; }
        public string ItemPartNumber { get; set; }
        public string EndUnitPartNumber { get; set; }
    }


    public class InputParmas
    {
        public string EndUnitPartNumber { get; set; }
        public string ItemPartNumber { get; set; }
        public string currentjob { get; set; }
        public string currentStartDate { get; set; }

    }

}
