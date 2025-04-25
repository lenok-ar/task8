namespace WindowsFormsApp1
{
  public interface ITicket
  {
    string GetTicketDetails();
    void SaveTicket(string filePath);
    void DeleteTicket(string filePath);
  }
}