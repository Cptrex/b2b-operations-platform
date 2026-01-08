namespace Platform.Service.Business.Domain.User;

public class User
{
    public int Id { get; set; }
    public Guid AccountId { get; set; }
    public string UserName { get; set; }
    public long CreatedAt { get; set; }


    public int BusinessId { get; set; }
    public Business.Business Business { get; set; }
}