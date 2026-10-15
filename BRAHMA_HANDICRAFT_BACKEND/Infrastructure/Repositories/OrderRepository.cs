using System.Text.Json;
using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Data;
using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;

namespace BRAHMA_HANDICRAFT_BACKEND.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(CreateOrderResult Result, OrderDto? Order, string? ErrorMessage)> CreateOrderAsync(
            CreateOrderRequest request,
            CancellationToken cancellationToken)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
            if (!userExists)
            {
                return (CreateOrderResult.UserNotFound, null, "User not found.");
            }

            var shippingAddress = await _context.Addresses.FirstOrDefaultAsync(
                a => a.Id == request.ShippingAddressId && a.UserId == request.UserId, cancellationToken);
            if (shippingAddress is null)
            {
                return (CreateOrderResult.ShippingAddressNotFound, null, "Shipping address not found for this user.");
            }

            var billingAddress = await _context.Addresses.FirstOrDefaultAsync(
                a => a.Id == request.BillingAddressId && a.UserId == request.UserId, cancellationToken);
            if (billingAddress is null)
            {
                return (CreateOrderResult.BillingAddressNotFound, null, "Billing address not found for this user.");
            }

            if (request.Items.Count == 0)
            {
                return (CreateOrderResult.NoItems, null, "Order must have at least one item.");
            }

            var itemNames = new Dictionary<Guid, (string ProductName, string DesignName, string SizeName)>();
            var inventoryBySizeId = new Dictionary<Guid, InventoryStock>();

            foreach (var item in request.Items)
            {
                if (!inventoryBySizeId.TryGetValue(item.SizeId, out var inventoryStock))
                {
                    inventoryStock = await _context.InventoryStocks
                        .Include(i => i.Product)
                        .Include(i => i.Design)
                        .Include(i => i.Size)
                        .FirstOrDefaultAsync(
                            i => i.SizeId == item.SizeId && i.DesignId == item.DesignId && i.ProductId == item.ProductId,
                            cancellationToken);

                    if (inventoryStock is null)
                    {
                        return (CreateOrderResult.InvalidItem, null,
                            $"The given productId/designId/sizeId combination is invalid: productId={item.ProductId}, designId={item.DesignId}, sizeId={item.SizeId}.");
                    }

                    inventoryBySizeId[item.SizeId] = inventoryStock;
                    itemNames[item.SizeId] = (inventoryStock.Product.Name, inventoryStock.Design.DesignName, inventoryStock.Size.SizeName);
                }
                else if (inventoryStock.ProductId != item.ProductId || inventoryStock.DesignId != item.DesignId)
                {
                    return (CreateOrderResult.InvalidItem, null,
                        $"The given productId/designId/sizeId combination is invalid: productId={item.ProductId}, designId={item.DesignId}, sizeId={item.SizeId}.");
                }
            }

            var requestedQuantityBySizeId = request.Items
                .GroupBy(i => i.SizeId)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));

            foreach (var (sizeId, requestedQuantity) in requestedQuantityBySizeId)
            {
                var inventoryStock = inventoryBySizeId[sizeId];

                if (inventoryStock.Quantity < requestedQuantity)
                {
                    var names = itemNames[sizeId];
                    return (CreateOrderResult.InsufficientStock, null,
                        $"Insufficient stock for '{names.ProductName} - {names.DesignName} - {names.SizeName}': requested {requestedQuantity}, available {inventoryStock.Quantity}.");
                }
            }

            foreach (var (sizeId, requestedQuantity) in requestedQuantityBySizeId)
            {
                var inventoryStock = inventoryBySizeId[sizeId];
                inventoryStock.Quantity -= requestedQuantity;
                inventoryStock.LastUpdated = DateTime.UtcNow;
            }

            var shippingSnapshot = new AddressSnapshotDto
            {
                CustomerName = shippingAddress.CustomerName,
                CustomerPhoneNumber = shippingAddress.CustomerPhoneNumber,
                AddressLine1 = shippingAddress.AddressLine1,
                City = shippingAddress.City,
                State = shippingAddress.State,
                PostalCode = shippingAddress.PostalCode,
                Country = shippingAddress.Country
            };

            var billingSnapshot = new AddressSnapshotDto
            {
                CustomerName = billingAddress.CustomerName,
                CustomerPhoneNumber = billingAddress.CustomerPhoneNumber,
                AddressLine1 = billingAddress.AddressLine1,
                City = billingAddress.City,
                State = billingAddress.State,
                PostalCode = billingAddress.PostalCode,
                Country = billingAddress.Country
            };

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                GSTNumber = request.GSTNumber,
                OrderDate = DateTime.UtcNow,
                TrackingNumber = string.Empty,
                OrderStatus = OrderStatus.Confirmed,
                ItemsSubTotal = request.ItemsSubTotal,
                IGST = request.IGST,
                CGST = request.CGST,
                SGST = request.SGST,
                TotalTaxCharges = request.TotalTaxCharges,
                GrandTotal = request.GrandTotal,
                ShippingAddressId = shippingAddress.Id,
                ShippingAddressJson = JsonSerializer.Serialize(shippingSnapshot),
                BillingAddressId = billingAddress.Id,
                BillingAddressJson = JsonSerializer.Serialize(billingSnapshot)
            };
            _context.Orders.Add(order);

            var responseItems = new List<OrderItemDto>();

            foreach (var item in request.Items)
            {
                var subTotal = item.BasePrice * item.Quantity;

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    DesignId = item.DesignId,
                    SizeId = item.SizeId,
                    Quantity = item.Quantity,
                    BasePrice = item.BasePrice,
                    SubTotal = subTotal
                };
                _context.OrderItems.Add(orderItem);

                var names = itemNames[item.SizeId];

                responseItems.Add(new OrderItemDto
                {
                    OrderItemId = orderItem.Id,
                    ProductId = item.ProductId,
                    ProductName = names.ProductName,
                    DesignId = item.DesignId,
                    DesignName = names.DesignName,
                    SizeId = item.SizeId,
                    SizeName = names.SizeName,
                    BasePrice = item.BasePrice,
                    Quantity = item.Quantity,
                    SubTotal = subTotal
                });
            }

            await _context.SaveChangesAsync(cancellationToken);

            var orderDto = new OrderDto
            {
                OrderId = order.Id,
                UserId = order.UserId,
                GSTNumber = order.GSTNumber,
                OrderDate = order.OrderDate,
                TrackingNumber = order.TrackingNumber,
                OrderStatus = order.OrderStatus,
                ItemsSubTotal = order.ItemsSubTotal,
                IGST = order.IGST,
                CGST = order.CGST,
                SGST = order.SGST,
                TotalTaxCharges = order.TotalTaxCharges,
                GrandTotal = order.GrandTotal,
                ShippingAddressId = order.ShippingAddressId,
                ShippingAddress = shippingSnapshot,
                BillingAddressId = order.BillingAddressId,
                BillingAddress = billingSnapshot,
                Items = responseItems
            };

            return (CreateOrderResult.Success, orderDto, null);
        }

        public async Task<(List<OrderDto> Items, int TotalCount)> GetOrdersByUserIdAsync(
            Guid userId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = _context.Orders.AsNoTracking().Where(o => o.UserId == userId);

            var totalCount = await query.CountAsync(cancellationToken);

            var rawOrders = await query
                .OrderByDescending(o => o.OrderDate)
                .ThenByDescending(o => o.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new
                {
                    o.Id,
                    o.UserId,
                    o.GSTNumber,
                    o.OrderDate,
                    o.TrackingNumber,
                    o.OrderStatus,
                    o.ItemsSubTotal,
                    o.IGST,
                    o.CGST,
                    o.SGST,
                    o.TotalTaxCharges,
                    o.GrandTotal,
                    o.ShippingAddressId,
                    o.ShippingAddressJson,
                    o.BillingAddressId,
                    o.BillingAddressJson,
                    Items = o.OrderItems.Select(oi => new
                    {
                        oi.Id,
                        oi.ProductId,
                        ProductName = oi.Product.Name,
                        oi.DesignId,
                        DesignName = oi.Design != null ? oi.Design.DesignName : string.Empty,
                        oi.SizeId,
                        SizeName = oi.Size != null ? oi.Size.SizeName : string.Empty,
                        oi.BasePrice,
                        oi.Quantity,
                        oi.SubTotal,
                        Image = oi.Design != null
                            ? oi.Design.ProductImages
                                .OrderByDescending(pi => pi.IsPrimary)
                                .ThenBy(pi => pi.Id)
                                .Select(pi => new ProductImageDto
                                {
                                    ImageId = pi.Id,
                                    Url = pi.ImageURL,
                                    AltText = pi.AltText,
                                    IsPrimary = pi.IsPrimary
                                })
                                .FirstOrDefault()
                            : null
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            var items = rawOrders.Select(o => new OrderDto
            {
                OrderId = o.Id,
                UserId = o.UserId,
                GSTNumber = o.GSTNumber,
                OrderDate = o.OrderDate,
                TrackingNumber = o.TrackingNumber,
                OrderStatus = o.OrderStatus,
                ItemsSubTotal = o.ItemsSubTotal,
                IGST = o.IGST,
                CGST = o.CGST,
                SGST = o.SGST,
                TotalTaxCharges = o.TotalTaxCharges,
                GrandTotal = o.GrandTotal,
                ShippingAddressId = o.ShippingAddressId,
                ShippingAddress = JsonSerializer.Deserialize<AddressSnapshotDto>(o.ShippingAddressJson) ?? new AddressSnapshotDto(),
                BillingAddressId = o.BillingAddressId,
                BillingAddress = JsonSerializer.Deserialize<AddressSnapshotDto>(o.BillingAddressJson) ?? new AddressSnapshotDto(),
                Items = o.Items.Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    DesignId = oi.DesignId ?? Guid.Empty,
                    DesignName = oi.DesignName,
                    SizeId = oi.SizeId ?? Guid.Empty,
                    SizeName = oi.SizeName,
                    BasePrice = oi.BasePrice,
                    Quantity = oi.Quantity,
                    SubTotal = oi.SubTotal,
                    Image = oi.Image
                }).ToList()
            }).ToList();

            return (items, totalCount);
        }

        private static readonly Dictionary<OrderStatus, HashSet<OrderStatus>> AllowedStatusTransitions = new()
        {
            [OrderStatus.Pending] = new HashSet<OrderStatus> { OrderStatus.Confirmed, OrderStatus.Cancelled },
            [OrderStatus.Confirmed] = new HashSet<OrderStatus> { OrderStatus.Processing, OrderStatus.Cancelled },
            [OrderStatus.Processing] = new HashSet<OrderStatus> { OrderStatus.Shipped, OrderStatus.Cancelled },
            [OrderStatus.Shipped] = new HashSet<OrderStatus> { OrderStatus.Delivered },
            [OrderStatus.Delivered] = new HashSet<OrderStatus> { OrderStatus.Refunded },
            [OrderStatus.Cancelled] = new HashSet<OrderStatus>(),
            [OrderStatus.Refunded] = new HashSet<OrderStatus>()
        };

        public async Task<(UpdateOrderStatusResult Result, Guid OrderId, OrderStatus Status, string? ErrorMessage)> UpdateOrderStatusAsync(
            Guid orderId,
            OrderStatus status,
            CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            if (order is null)
            {
                return (UpdateOrderStatusResult.OrderNotFound, orderId, status, "Order not found.");
            }

            if (!AllowedStatusTransitions[order.OrderStatus].Contains(status))
            {
                return (UpdateOrderStatusResult.InvalidTransition, orderId, order.OrderStatus,
                    $"Cannot change order status from '{order.OrderStatus}' to '{status}'.");
            }

            order.OrderStatus = status;
            await _context.SaveChangesAsync(cancellationToken);

            return (UpdateOrderStatusResult.Success, order.Id, order.OrderStatus, null);
        }

        public async Task<(List<AdminOrderDto> Items, int TotalCount)> GetAllOrdersAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = _context.Orders.AsNoTracking();

            var totalCount = await query.CountAsync(cancellationToken);

            var rawOrders = await query
                .OrderByDescending(o => o.OrderDate)
                .ThenByDescending(o => o.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new
                {
                    o.Id,
                    o.UserId,
                    UserFirstName = o.User.FirstName,
                    UserLastName = o.User.LastName,
                    UserEmailId = o.User.EmailId,
                    UserPhoneNumber = o.User.PhoneNumber,
                    o.GSTNumber,
                    o.OrderDate,
                    o.TrackingNumber,
                    o.OrderStatus,
                    o.ItemsSubTotal,
                    o.IGST,
                    o.CGST,
                    o.SGST,
                    o.TotalTaxCharges,
                    o.GrandTotal,
                    o.ShippingAddressId,
                    o.ShippingAddressJson,
                    o.BillingAddressId,
                    o.BillingAddressJson,
                    Items = o.OrderItems.Select(oi => new
                    {
                        oi.Id,
                        oi.ProductId,
                        ProductName = oi.Product.Name,
                        oi.DesignId,
                        DesignName = oi.Design != null ? oi.Design.DesignName : string.Empty,
                        oi.SizeId,
                        SizeName = oi.Size != null ? oi.Size.SizeName : string.Empty,
                        oi.BasePrice,
                        oi.Quantity,
                        oi.SubTotal,
                        Image = oi.Design != null
                            ? oi.Design.ProductImages
                                .OrderByDescending(pi => pi.IsPrimary)
                                .ThenBy(pi => pi.Id)
                                .Select(pi => new ProductImageDto
                                {
                                    ImageId = pi.Id,
                                    Url = pi.ImageURL,
                                    AltText = pi.AltText,
                                    IsPrimary = pi.IsPrimary
                                })
                                .FirstOrDefault()
                            : null
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            var items = rawOrders.Select(o => new AdminOrderDto
            {
                UserId = o.UserId,
                FullName = $"{o.UserFirstName} {o.UserLastName}",
                EmailId = o.UserEmailId,
                PhoneNumber = o.UserPhoneNumber,
                Order = new OrderDto
                {
                    OrderId = o.Id,
                    UserId = o.UserId,
                    GSTNumber = o.GSTNumber,
                    OrderDate = o.OrderDate,
                    TrackingNumber = o.TrackingNumber,
                    OrderStatus = o.OrderStatus,
                    ItemsSubTotal = o.ItemsSubTotal,
                    IGST = o.IGST,
                    CGST = o.CGST,
                    SGST = o.SGST,
                    TotalTaxCharges = o.TotalTaxCharges,
                    GrandTotal = o.GrandTotal,
                    ShippingAddressId = o.ShippingAddressId,
                    ShippingAddress = JsonSerializer.Deserialize<AddressSnapshotDto>(o.ShippingAddressJson) ?? new AddressSnapshotDto(),
                    BillingAddressId = o.BillingAddressId,
                    BillingAddress = JsonSerializer.Deserialize<AddressSnapshotDto>(o.BillingAddressJson) ?? new AddressSnapshotDto(),
                    Items = o.Items.Select(oi => new OrderItemDto
                    {
                        OrderItemId = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        DesignId = oi.DesignId ?? Guid.Empty,
                        DesignName = oi.DesignName,
                        SizeId = oi.SizeId ?? Guid.Empty,
                        SizeName = oi.SizeName,
                        BasePrice = oi.BasePrice,
                        Quantity = oi.Quantity,
                        SubTotal = oi.SubTotal,
                        Image = oi.Image
                    }).ToList()
                }
            }).ToList();

            return (items, totalCount);
        }
    }
}
