namespace Platform.Service.Business.Domain.Business;

public class Business
{
    public string BusinessId { get; set; }
    public string BusinessName { get; set; }
    public long CreatedAt { get; set; }

    private List<User.User> _users = [];
    public List<User.User> Users 
    { 
        get 
        { 
            return _users; 
        } 
        set 
        { 
            if (value == null)
            {
                _users = [];
            }
            else
            {
                _users = value;
            }
        } 
    }

    private List<Product.Product> _products = [];
    public List<Product.Product> Products 
    { 
        get 
        { 
            return _products; 
        } 
        set 
        { 
            if (value == null)
            {
                _products = [];
            }
            else
            {
                _products = value;
            }
        } 
    }

    private List<Customer.Customer> _customers = [];
    public List<Customer.Customer> Customers 
    { 
        get 
        { 
            return _customers; 
        } 
        set 
        { 
            if (value == null)
            {
                _customers = [];
            }
            else
            {
                _customers = value;
            }
        } 
    }

    private Business() 
    {
        BusinessId = string.Empty;
        BusinessName = string.Empty;
        _users = [];
        _products = [];
        _customers = [];
    }

    public Business(string businessId, string businessName)
    {
        BusinessId = businessId;
        BusinessName = businessName;
        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        _users = [];
        _products = [];
        _customers = [];
    }

    public void AddUser(User.User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        _users.Add(user);
    }

    public void RemoveUser(User.User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        _users.Remove(user);
    }

    public void AddProduct(Product.Product product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        _products.Add(product);
    }

    public void RemoveProduct(Product.Product product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        _products.Remove(product);
    }

    public void AddCustomer(Customer.Customer customer)
    {
        if (customer == null)
        {
            throw new ArgumentNullException(nameof(customer));
        }

        var existingCustomer = _customers.FirstOrDefault(c => c.CustomerId == customer.CustomerId);

        if (existingCustomer == null)
        {
            _customers.Add(customer);
        }
    }
}
