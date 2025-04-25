namespace WindowsFormsApp1
{
  public interface ITicketFactory
  {
    ITicket CreateTicket(string ticketDetails);
  }
}