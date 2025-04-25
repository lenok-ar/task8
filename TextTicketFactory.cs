namespace WindowsFormsApp1
{
  public class TextTicketFactory : ITicketFactory
  {
    public ITicket CreateTicket(string ticketDetails)
    {
      return new TextTicket(ticketDetails);
    }
  }
}