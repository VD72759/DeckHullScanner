namespace DeckHullScanner.Interface
{
    public interface IServices
    {
        public string GetUser(string UserNumber);
        public string GetDataWV(string EndUnitPartNumber, string ItemPartNumber, string CardNumber, string EmpNumber);
        public bool ValidateEndUnitPartNumber(string EndUnitPartNumber);

    //    public bool ValidateItemPartNumber(string ItemPartNumber, string EndUnitPartNumber = "");
    //    public void Verify();
    }
}
