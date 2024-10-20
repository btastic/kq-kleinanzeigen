using KQ.Kleinanzeigen.Domain.Enums;

namespace KQ.Kleinanzeigen.Api.Api.Responses;

public record ItemListingResponse(
    long Id, 
    string Title, 
    decimal Price, 
    string Shipping, 
    Status Status, 
    string Category, 
    DateTimeOffset PostedAt, 
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

public record ItemDetailsResponse(long Id,
    SellerDetails seller,
    string[] ImageUris,
    string Title,
    string Description,
    decimal Price,
    string Shipping,
    string ZipCode,
    string City,
    string Category,
    Status Status,
    DateTimeOffset PostedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

public record SellerDetails(long Id,
    string Name,
    Rating? Rating,
    Friendliness? Friendliness,
    Reliability? Reliability,
    DateTimeOffset ActiveSince,
    bool CommercialSeller);
