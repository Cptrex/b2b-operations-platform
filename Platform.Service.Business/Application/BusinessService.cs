using Platform.Service.Business.Domain.Business;
using Platform.Service.Business.Domain.User;
using Platform.Service.Business.Domain.Product;
using Platform.Service.Business.Domain.Customer;
using Platform.Service.Business.Infrastructure.Db;
using Platform.Service.Business.Infrastructure.Db.Entity;
using Platform.Shared.Messaging.Contracts.Events.Business;
using Platform.Shared.Messaging.Contracts.Events.User;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Platform.Logging.MongoDb.Contracts;
using Platform.Logging.MongoDb;
using Platform.Service.Business.Infrastructure.Logging;

namespace Platform.Service.Business.Application;

public class BusinessService
{
    private readonly IBusinessRepository _businessRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly BusinessContext _context;
    private readonly ILoggingService _logging;

    public BusinessService(IBusinessRepository businessRepository, IProductRepository productRepository, ICustomerRepository customerRepository, ILoggingService logging, BusinessContext context)
    {
        _businessRepository = businessRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _context = context;
        _logging = logging;
    }

    public async Task<Domain.Business.Business> CreateBusinessAsync(string businessId, string businessName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(businessId))
        {
            throw new ArgumentNullException(nameof(businessId));
        }

        if (string.IsNullOrWhiteSpace(businessName))
        {
            throw new ArgumentNullException(nameof(businessName));
        }

        var existingBusiness = await _businessRepository.GetByBusinessByIdAsync(businessId);

        if (existingBusiness is not null)
        {
            throw new InvalidOperationException($"Business with id '{businessId}' already exists");
        }

        var newBusiness = new Domain.Business.Business(businessId, businessName);

        await _businessRepository.CreateBusinessAsync(newBusiness);

        var businessCreatedEvent = new BusinessCreatedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            BusinessId = newBusiness.BusinessId,
            BusinessName = newBusiness.BusinessName,
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(newBusiness.CreatedAt)
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = businessCreatedEvent.EventId,
            Type = nameof(BusinessCreatedEvent),
            RoutingKey = "business.businessCreated",
            Payload = JsonSerializer.Serialize(businessCreatedEvent),
            OccurredAt = businessCreatedEvent.OccuredAt
        }, ct);

        await _businessRepository.Save();

        await _logging.WriteAsync(LogType.Activitty, LoggingAction.BusinessCreated, businessCreatedEvent, ct);

        return newBusiness;
    }

    public async Task DeleteBusinessAsync(string businessId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(businessId))
        {
            throw new ArgumentNullException(nameof(businessId));
        }

        var business = await _businessRepository.GetByBusinessByIdAsync(businessId);

        if (business is null)
        {
            throw new InvalidOperationException($"Business with id '{businessId}' not found");
        }

        await _businessRepository.DeleteBusinessAsync(business);

        var businessDeletedEvent = new BusinessDeletedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            BusinessId = businessId
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = businessDeletedEvent.EventId,
            Type = nameof(BusinessDeletedEvent),
            RoutingKey = "business.businessDeleted",
            Payload = JsonSerializer.Serialize(businessDeletedEvent),
            OccurredAt = businessDeletedEvent.OccuredAt
        }, ct);

        await _logging.WriteAsync(LogType.Activitty, LoggingAction.BusinessDeleted, businessDeletedEvent, ct);

        await _businessRepository.Save();
    }

    public async Task<User> CreateBusinessUserAsync(string username, Guid accountId, string businessId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(nameof(username));
        }

        if (accountId == Guid.Empty)
        {
            throw new ArgumentException("AccountId cannot be empty", nameof(accountId));
        }

        if (string.IsNullOrWhiteSpace(businessId))
        {
            throw new ArgumentNullException(nameof(businessId));
        }

        var business = await _businessRepository.GetByBusinessByIdAsync(businessId);

        if (business is null)
        {
            throw new InvalidOperationException($"Business with id '{businessId}' not found");
        }

        var newUser = new User(username, accountId, businessId);

        business.AddUser(newUser);

        await _businessRepository.UpdateBusinessAsync(business);

        var userCreatedEvent = new UserCreatedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            UserId = newUser.Id,
            AccountId = newUser.AccountId,
            UserName = newUser.UserName,
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(newUser.CreatedAt)
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = userCreatedEvent.EventId,
            Type = nameof(UserCreatedEvent),
            RoutingKey = "auth.service.userCreated",
            Payload = JsonSerializer.Serialize(userCreatedEvent),
            OccurredAt = userCreatedEvent.OccuredAt
        }, ct);

        await _businessRepository.Save();

        await _logging.WriteAsync(LogType.Activitty, LoggingAction.BusinessNewUserCreated, userCreatedEvent, ct);

        return newUser;
    }

    public async Task DeleteUserAsync(int userId, string businessId, CancellationToken ct)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("UserId must be greater than 0", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(businessId))
        {
            throw new ArgumentNullException(nameof(businessId));
        }

        var business = await _context.Businesses.Include(b => b.Users).FirstOrDefaultAsync(b => b.BusinessId == businessId, ct);

        if (business is null)
        {
            throw new InvalidOperationException($"Business with id '{businessId}' not found");
        }

        var user = business.Users.FirstOrDefault(u => u.Id == userId);

        if (user is null)
        {
            throw new InvalidOperationException($"User with id '{userId}' not found in business '{businessId}'");
        }

        business.RemoveUser(user);

        var userDeletedEvent = new UserDeletedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            UserId = userId
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = userDeletedEvent.EventId,
            Type = nameof(UserDeletedEvent),
            RoutingKey = "auth.service.userDeleted",
            Payload = JsonSerializer.Serialize(userDeletedEvent),
            OccurredAt = userDeletedEvent.OccuredAt
        }, ct);

        await _businessRepository.Save();

        await _logging.WriteAsync(LogType.Activitty, LoggingAction.BusinessUserDeleted, userDeletedEvent, ct);
    }

    public async Task<Product> AddProductToCatalogAsync(string businessId, string productName, string description, decimal price, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(businessId))
        {
            throw new ArgumentNullException(nameof(businessId));
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentNullException(nameof(productName));
        }

        if (price < 0)
        {
            throw new ArgumentException("Price cannot be negative", nameof(price));
        }

        var business = await _businessRepository.GetByBusinessByIdAsync(businessId);

        if (business == null)
        {
            throw new InvalidOperationException($"Business with id '{businessId}' not found");
        }

        var product = new Product(businessId, productName, description, price);

        business.AddProduct(product);

        await _productRepository.CreateProductAsync(product);

        var productAddedEvent = new ProductAddedToCatalogEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            ProductId = product.ProductId,
            BusinessId = product.BusinessId,
            ProductName = product.ProductName,
            Description = product.Description,
            Price = product.Price,
            IsAvailable = product.IsAvailable,
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(product.CreatedAt)
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = productAddedEvent.EventId,
            Type = nameof(ProductAddedToCatalogEvent),
            RoutingKey = "business.productAddedToCatalog",
            Payload = JsonSerializer.Serialize(productAddedEvent),
            OccurredAt = productAddedEvent.OccuredAt
        }, ct);

        await _productRepository.Save();

        await _logging.WriteAsync(LogType.Activitty, LoggingAction.BusinessNewCatalogProduct, productAddedEvent, ct);

        return product;
    }

    public async Task<Product> RemoveProductFromCatalogAsync(Guid productId, CancellationToken ct)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));
        }

        var product = await _productRepository.GetProductByIdAsync(productId);

        if (product == null)
        {
            throw new InvalidOperationException($"Product with id '{productId}' not found");
        }

        var business = await _context.Businesses.Include(b => b.Products).FirstOrDefaultAsync(b => b.BusinessId == product.BusinessId, ct);

        if (business != null)
        {
            business.RemoveProduct(product);
        }

        await _productRepository.DeleteProductAsync(product);

        var productRemovedEvent = new ProductRemovedFromCatalogEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            ProductId = product.ProductId,
            BusinessId = product.BusinessId
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = productRemovedEvent.EventId,
            Type = nameof(ProductRemovedFromCatalogEvent),
            RoutingKey = "business.productRemovedFromCatalog",
            Payload = JsonSerializer.Serialize(productRemovedEvent),
            OccurredAt = productRemovedEvent.OccuredAt
        }, ct);

        await _productRepository.Save();

        await _logging.WriteAsync(LogType.Activitty, LoggingAction.BusinessRemoveCatalogProduct, productRemovedEvent, ct);

        return product;
    }

    public async Task<Product> UpdateProductInfoAsync(Guid productId, string productName, string description, decimal price, CancellationToken ct)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentNullException(nameof(productName));
        }

        if (price < 0)
        {
            throw new ArgumentException("Price cannot be negative", nameof(price));
        }

        var product = await _productRepository.GetProductByIdAsync(productId);

        if (product == null)
        {
            throw new InvalidOperationException($"Product with id '{productId}' not found");
        }

        product.UpdateInfo(productName, description, price);

        await _productRepository.UpdateProductAsync(product);

        var productUpdatedEvent = new ProductInfoUpdatedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            ProductId = product.ProductId,
            BusinessId = product.BusinessId,
            ProductName = product.ProductName,
            Description = product.Description,
            Price = product.Price,
            UpdatedAt = DateTimeOffset.FromUnixTimeSeconds(product.UpdatedAt ?? 0)
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = productUpdatedEvent.EventId,
            Type = nameof(ProductInfoUpdatedEvent),
            RoutingKey = "business.productInfoUpdated",
            Payload = JsonSerializer.Serialize(productUpdatedEvent),
            OccurredAt = productUpdatedEvent.OccuredAt
        }, ct);

        await _productRepository.Save();

        await _logging.WriteAsync(LogType.Activitty, LoggingAction.BusinessUpdateProduct, productUpdatedEvent, ct);

        return product;
    }

    public async Task<Product> SetProductAvailabilityAsync(Guid productId, bool isAvailable, CancellationToken ct)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));
        }

        var product = await _productRepository.GetProductByIdAsync(productId);

        if (product == null)
        {
            throw new InvalidOperationException($"Product with id '{productId}' not found");
        }

        product.SetAvailability(isAvailable);

        await _productRepository.UpdateProductAsync(product);

        var productAvailabilityChangedEvent = new ProductAvailabilityChangedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            ProductId = product.ProductId,
            BusinessId = product.BusinessId,
            IsAvailable = product.IsAvailable
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = productAvailabilityChangedEvent.EventId,
            Type = nameof(ProductAvailabilityChangedEvent),
            RoutingKey = "business.productAvailabilityChanged",
            Payload = JsonSerializer.Serialize(productAvailabilityChangedEvent),
            OccurredAt = productAvailabilityChangedEvent.OccuredAt
        }, ct);

        await _productRepository.Save();

        await _logging.WriteAsync(LogType.Activitty, LoggingAction.BusinessSetProductAvailability, productAvailabilityChangedEvent, ct);

        return product;
    }

    public async Task<Customer> AddCustomerAsync(Guid customerId, string businessId, string customerName, string customerEmail, string customerPhone, CancellationToken ct)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("CustomerId cannot be empty", nameof(customerId));
        }

        if (string.IsNullOrWhiteSpace(businessId))
        {
            throw new ArgumentNullException(nameof(businessId));
        }

        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ArgumentNullException(nameof(customerName));
        }

        var business = await _context.Businesses.Include(b => b.Customers).FirstOrDefaultAsync(b => b.BusinessId == businessId, ct);

        if (business == null)
        {
            throw new InvalidOperationException($"Business with id '{businessId}' not found");
        }

        var existingCustomer = await _customerRepository.GetCustomerByIdAsync(customerId, businessId);

        if (existingCustomer != null)
        {
            return existingCustomer;
        }

        var customer = new Customer(customerId, businessId, customerName, customerEmail, customerPhone);

        business.AddCustomer(customer);

        await _customerRepository.CreateCustomerAsync(customer);

        var customerAddedEvent = new CustomerAddedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            CustomerId = customer.CustomerId,
            BusinessId = customer.BusinessId,
            CustomerName = customer.CustomerName,
            CustomerEmail = customer.CustomerEmail,
            CustomerPhone = customer.CustomerPhone,
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(customer.CreatedAt)
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = customerAddedEvent.EventId,
            Type = nameof(CustomerAddedEvent),
            RoutingKey = "business.customerAdded",
            Payload = JsonSerializer.Serialize(customerAddedEvent),
            OccurredAt = customerAddedEvent.OccuredAt
        }, ct);

        await _customerRepository.Save();

        await _logging.WriteAsync(LogType.Activitty, LoggingAction.BusinessAddCustomer, customerAddedEvent, ct);

        return customer;
    }
}