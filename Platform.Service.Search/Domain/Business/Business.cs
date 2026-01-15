namespace Platform.Service.Search.Domain.Business;

public class Business
{
    public string BusinessId { get; set; }
    public string BusinessName { get; set; }
    public long CreatedAt { get; set; }

    private Business()
    {
        BusinessId = string.Empty;
        BusinessName = string.Empty;
    }

    public Business(string businessId, string businessName, long createdAt)
    {
        BusinessId = businessId;
        BusinessName = businessName;
        CreatedAt = createdAt;
    }
}