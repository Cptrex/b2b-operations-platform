using Platform.Logging.MongoDb;
using Platform.Logging.MongoDb.Contracts;
using Platform.Service.Business.Domain.Business;
using Platform.Service.Business.Domain.Product;
using Platform.Service.Business.Infrastructure.Db;
using Platform.Service.Business.Infrastructure.Db.Entity;
using Platform.Service.Business.Infrastructure.Logging;
using Platform.Shared.Messaging.Contracts.Events.Business;
using Platform.Shared.Results;
using Platform.Shared.Results.Enums;
using System.Text.Json;

namespace Platform.Service.Business.Application;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly BusinessContext _context;
    private readonly ILoggingService _logging;


    public ProductService(IProductRepository productRepository, IBusinessRepository businessRepository, BusinessContext context, ILoggingService logging)
    {
        _productRepository = productRepository;
        _businessRepository = businessRepository;
        _context = context;
        _logging = logging;
    }

    public async Task<Result<Product>> AddProductToCatalogAsync(string businessId, string productName, string description, decimal price, CancellationToken ct)
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

        if (business is null)
        {
            return Result<Product>.Fail(new Error($"Business with id '{businessId}' not found", ResultErrorCategory.NotFound));
        }

        var product = new Product(businessId, productName, description, price);

        business.AddProduct(product);

        await _productRepository.CreateProductAsync(product);

        var productAddedEvent = new ProductAddedToCatalogEvent
        {
            EventId = Guid.NewGuid().ToString("D"),
            OccuredAt = DateTimeOffset.UtcNow,
            ProductId = product.ProductId.ToString("D"),
            BusinessId = product.BusinessId,
            ProductName = product.ProductName,
            Description = product.Description,
            Price = product.Price,
            IsAvailable = product.IsAvailable,
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(product.CreatedAt)
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = Guid.Parse(productAddedEvent.EventId),
            Type = nameof(ProductAddedToCatalogEvent),
            RoutingKey = "business.productAddedToCatalog",
            Payload = JsonSerializer.Serialize(productAddedEvent),
            OccurredAt = productAddedEvent.OccuredAt
        }, ct);

        await _productRepository.Save();

        await _logging.WriteAsync(LogType.Activity, LoggingAction.BusinessNewCatalogProduct, productAddedEvent, ct);

        return Result<Product>.Ok(product);
    }

    public async Task RemoveProductFromCatalogAsync(Guid productId, CancellationToken ct)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));
        }

        var product = await _productRepository.GetProductByIdAsync(productId);

        if (product is null)
        {
            throw new InvalidOperationException($"Product with id '{productId}' not found");
        }

        var business = await _businessRepository.GetByBusinessByIdAsync(product.BusinessId);

        if (business is null)
        {
            throw new InvalidOperationException($"Business with id '{business}' not found");
        }

        business.RemoveProduct(product);

        await _productRepository.DeleteProductAsync(product);

        var productRemovedEvent = new ProductRemovedFromCatalogEvent
        {
            EventId = Guid.NewGuid().ToString("D"),
            OccuredAt = DateTimeOffset.UtcNow,
            ProductId = product.ProductId.ToString("D"),
            BusinessId = product.BusinessId
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = Guid.Parse(productRemovedEvent.EventId),
            Type = nameof(ProductRemovedFromCatalogEvent),
            RoutingKey = "business.productRemovedFromCatalog",
            Payload = JsonSerializer.Serialize(productRemovedEvent),
            OccurredAt = productRemovedEvent.OccuredAt
        }, ct);

        await _productRepository.Save();

        await _logging.WriteAsync(LogType.Activity, LoggingAction.BusinessRemoveCatalogProduct, productRemovedEvent, ct);
    }

    public async Task<Result<Product>> UpdateProductInfoAsync(Guid productId, string productName, string description, decimal price, CancellationToken ct)
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

        if (product is null)
        {
            return Result<Product>.Fail(new Error($"Product with id '{productId}' not found", ResultErrorCategory.NotFound));
        }

        product.UpdateInfo(productName, description, price);

        await _productRepository.UpdateProductAsync(product);

        var productUpdatedEvent = new ProductInfoUpdatedEvent
        {
            EventId = Guid.NewGuid().ToString("D"),
            OccuredAt = DateTimeOffset.UtcNow,
            ProductId = product.ProductId.ToString("D"),
            BusinessId = product.BusinessId,
            ProductName = product.ProductName,
            Description = product.Description,
            Price = product.Price,
            UpdatedAt = DateTimeOffset.FromUnixTimeSeconds(product.UpdatedAt ?? 0)
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = Guid.Parse(productUpdatedEvent.EventId),
            Type = nameof(ProductInfoUpdatedEvent),
            RoutingKey = "business.productInfoUpdated",
            Payload = JsonSerializer.Serialize(productUpdatedEvent),
            OccurredAt = productUpdatedEvent.OccuredAt
        }, ct);

        await _productRepository.Save();

        await _logging.WriteAsync(LogType.Activity, LoggingAction.BusinessUpdateProduct, productUpdatedEvent, ct);

        return Result<Product>.Ok(product);
    }

    public async Task<Result<Product>> SetProductAvailabilityAsync(Guid productId, bool isAvailable, CancellationToken ct)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));
        }

        var product = await _productRepository.GetProductByIdAsync(productId);

        if (product is null)
        {
            return Result<Product>.Fail(new Error($"Product with id '{productId}' not found", ResultErrorCategory.NotFound));
        }

        product.SetAvailability(isAvailable);

        await _productRepository.UpdateProductAsync(product);

        var productAvailabilityChangedEvent = new ProductAvailabilityChangedEvent
        {
            EventId = Guid.NewGuid().ToString("D"),
            OccuredAt = DateTimeOffset.UtcNow,
            ProductId = product.ProductId.ToString("D"),
            BusinessId = product.BusinessId,
            IsAvailable = product.IsAvailable
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = Guid.Parse(productAvailabilityChangedEvent.EventId),
            Type = nameof(ProductAvailabilityChangedEvent),
            RoutingKey = "business.productAvailabilityChanged",
            Payload = JsonSerializer.Serialize(productAvailabilityChangedEvent),
            OccurredAt = productAvailabilityChangedEvent.OccuredAt
        }, ct);

        await _productRepository.Save();

        await _logging.WriteAsync(LogType.Activity, LoggingAction.BusinessSetProductAvailability, productAvailabilityChangedEvent, ct);

        return Result<Product>.Ok(product);
    }
}
