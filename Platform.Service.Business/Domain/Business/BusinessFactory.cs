namespace Platform.Service.Business.Domain.Business;

public class BusinessFactory
{
    public Business Create(string businessId, string businessName)
    {
        return new Business(businessId, businessName);
    }
}