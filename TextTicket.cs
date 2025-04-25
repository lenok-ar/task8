using System.IO;

namespace WindowsFormsApp1
{
  public class TextTicket : ITicket
  {
    private string _ticketDetails;

    public TextTicket(string ticketDetails)
    {
      _ticketDetails = ticketDetails;
    }

    public string GetTicketDetails()
    {
      return _ticketDetails;
    }

    public void SaveTicket(string filePath)
    {
      File.WriteAllText(filePath, _ticketDetails);
    }

    public void DeleteTicket(string filePath)
    {
      if (File.Exists(filePath))
      {
        File.Delete(filePath);
      }
      else
      {
        throw new FileNotFoundException("Файл не найден для удаления: " + filePath);
      }
    }
  }
}