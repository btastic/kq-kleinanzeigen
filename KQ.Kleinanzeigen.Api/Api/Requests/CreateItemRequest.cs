using KQ.Kleinanzeigen.Domain.Enums;

namespace KQ.Kleinanzeigen.Api.Api.Requests;

public record UpdateItemRequest(string Title, string Description, decimal Price, Status Status);

public record CreateItemRequest(
    long Id,
    SellerRequest Seller,
    string[] ImageUris,
    string Title,
    string Description,
    decimal Price,
    string Shipping,
    string ZipCode,
    string City,
    string Category,
    Status Status,
    DateTimeOffset PostedAt);

public record SellerRequest(
    long Id,
    string Name,
    Rating? Rating,
    Friendliness? Friendliness,
    Reliability? Reliability,
    DateTimeOffset ActiveSince,
    bool CommercialSeller);
