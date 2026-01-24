using Platform.Service.Business.Domain.Business;
using Platform.Service.Business.Domain.Customer;
using Platform.Service.Business.Infrastructure.Db;
using Platform.Service.Business.Infrastructure.Db.Entity;
using Platform.Shared.Messaging.Contracts.Events.Business;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Platform.Logging.MongoDb.Contracts;
using Platform.Logging.MongoDb;
using Platform.Service.Business.Infrastructure.Logging;
using Platform.Shared.Results;
using Platform.Shared.Results.Enums;

namespace Platform.Service.Business.Application;

public class BusinessService
{
    private readonly IBusinessRepository _businessRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly BusinessContext _context;
    private readonly ILoggingService _logging;

    public BusinessService(IBusinessRepository businessRepository, ICustomerRepository customerRepository, ILoggingService logging, BusinessContext context)
    {
        _businessRepository = businessRepository;
        _customerRepository = customerRepository;
        _context = context;
        _logging = logging;
    }

    public async Task<Result<Domain.Business.Business>> CreateBusinessAsync(string businessName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(businessName))
        {
            throw new ArgumentNullException(nameof(businessName));
        }

        var existingBusiness = await _businessRepository.GetByBusinessNameAsync(businessName);

        if (existingBusiness is not null)
        {
            return Result<Domain.Business.Business>.Fail(new Error($"Business with name '{businessName}' already exists", ResultErrorCategory.Conflict));
        }

        var newBusiness = new Domain.Business.Business(businessName);

        newBusiness = await _businessRepository.CreateBusinessAsync(newBusiness);

        var businessCreatedEvent = new BusinessCreatedEvent
        {
            EventId = Guid.NewGuid().ToString("D"),
            OccuredAt = DateTimeOffset.UtcNow,
            BusinessId = newBusiness.BusinessId,
            BusinessName = newBusiness.BusinessName,
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(newBusiness.CreatedAt)
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = Guid.Parse(businessCreatedEvent.EventId),
            Type = nameof(BusinessCreatedEvent),
            RoutingKey = "business.businessCreated",
            Payload = JsonSerializer.Serialize(businessCreatedEvent),
            OccurredAt = businessCreatedEvent.OccuredAt
        }, ct);

        await _businessRepository.Save();

        await _logging.WriteAsync(LogType.Activity, LoggingAction.BusinessCreated, businessCreatedEvent, ct);

        return Result<Domain.Business.Business>.Ok(newBusiness);
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
            EventId = Guid.NewGuid().ToString("D"),
            OccuredAt = DateTimeOffset.UtcNow,
            BusinessId = businessId
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = Guid.Parse(businessDeletedEvent.EventId),
            Type = nameof(BusinessDeletedEvent),
            RoutingKey = "business.businessDeleted",
            Payload = JsonSerializer.Serialize(businessDeletedEvent),
            OccurredAt = businessDeletedEvent.OccuredAt
        }, ct);

        await _logging.WriteAsync(LogType.Activity, LoggingAction.BusinessDeleted, businessDeletedEvent, ct);

        await _businessRepository.Save();
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
            EventId = Guid.NewGuid().ToString("D"),
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
            EventId = Guid.Parse(customerAddedEvent.EventId),
            Type = nameof(CustomerAddedEvent),
            RoutingKey = "business.customerAdded",
            Payload = JsonSerializer.Serialize(customerAddedEvent),
            OccurredAt = customerAddedEvent.OccuredAt
        }, ct);

        await _customerRepository.Save();

        await _logging.WriteAsync(LogType.Activity, LoggingAction.BusinessAddCustomer, customerAddedEvent, ct);

        return customer;
    }
}