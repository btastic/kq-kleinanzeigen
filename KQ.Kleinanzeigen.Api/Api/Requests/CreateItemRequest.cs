using KQ.Kleinanzeigen.Domain.Enums;

namespace KQ.Kleinanzeigen.Api.Api.Requests;

public record CreateItemRequest(
    long Id,
    SellerRequest Seller,
    string[] ImageUris,
    string Title,
    string Description,
    decimal Price,
    decimal ShippingCost,
    int Views,
    string ZipCode,
    string City,
    string Category,
    DateTimeOffset CreatedAt);

public record SellerRequest(
    long Id,
    string Name,
    Rating? Rating,
    Friendliness? Friendliness,
    Reliability? Reliability,
    int Followers,
    DateTimeOffset ActiveSince,
    bool CommercialSeller);
