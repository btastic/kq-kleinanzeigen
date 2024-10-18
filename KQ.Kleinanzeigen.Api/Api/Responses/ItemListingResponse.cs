using KQ.Kleinanzeigen.Domain.Enums;

namespace KQ.Kleinanzeigen.Api.Api.Responses;

public record ItemListingResponse(long Id, string Title, decimal Price, decimal ShippingCost, int Views, string Category);

public record ItemDetailsResponse(long Id,
    SellerDetails seller,
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

public record SellerDetails(long Id,
    string Name,
    Rating? Rating,
    Friendliness? Friendliness,
    Reliability? Reliability,
    int Followers,
    DateTimeOffset ActiveSince,
    bool CommercialSeller);
