using KQ.Kleinanzeigen.Api.Api.Requests;
using KQ.Kleinanzeigen.Api.Api.Responses;
using KQ.Kleinanzeigen.Domain.Entities;
using KQ.Kleinanzeigen.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KQ.Kleinanzeigen.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private readonly ILogger<ItemsController> _logger;
    private readonly AppDbContext _appDbContext;

    public ItemsController(
        ILogger<ItemsController> logger,
        AppDbContext appDbContext)
    {
        _logger = logger;
        _appDbContext = appDbContext;
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemDetailsResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:long}")]
    public async Task<IActionResult?> Get([FromRoute] long id)
    {
        var existingItem = await _appDbContext.Items.FindAsync(id);

        if (existingItem is null)
        {
            return NotFound();
        }

        return Ok(
            new ItemDetailsResponse(
                existingItem.Id,
                new SellerDetails(
                    existingItem.Seller.Id,
                    existingItem.Seller.Name,
                    existingItem.Seller.Rating,
                    existingItem.Seller.Friendliness,
                    existingItem.Seller.Reliability,
                    existingItem.Seller.Followers,
                    existingItem.Seller.ActiveSince,
                    existingItem.Seller.CommercialSeller),
                existingItem.Images.Select(i => i.ImageUrl).ToArray(),
                existingItem.Title,
                existingItem.Description,
                existingItem.Price,
                existingItem.ShippingCost,
                existingItem.Views,
                existingItem.ZipCode,
                existingItem.City,
                existingItem.Category,
                existingItem.CreatedAt));
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ItemListingResponse>))]
    [HttpGet]
    public async Task<IActionResult?> GetListing()
    {
        var existingItems = _appDbContext.Items;

        if (await existingItems.CountAsync() == 0)
        {
            return Ok(Enumerable.Empty<ItemDetailsResponse>());
        }

        return Ok(
            existingItems.Select(p =>
                new ItemListingResponse(
                    p.Id,
                    p.Title,
                    p.Price,
                    p.ShippingCost,
                    p.Views,
                    p.Category)));
    }

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateItemRequest item)
    {
        var existingItem = await _appDbContext.Items.FindAsync(item.Id);

        if (existingItem is not null)
        {
            return Conflict();
        }

        var existingSeller = await _appDbContext.Sellers.FindAsync(item.Seller.Id);

        if (existingSeller is null)
        {
            var sellerEntity = new Seller(
                item.Seller.Id,
                item.Seller.Name,
                item.Seller.Rating,
                item.Seller.Friendliness,
                item.Seller.Reliability,
                item.Seller.Followers,
                item.Seller.ActiveSince,
                item.Seller.CommercialSeller);

            await _appDbContext.Sellers.AddAsync(sellerEntity);
            await _appDbContext.SaveChangesAsync();
        }

        var itemEntity = new Item(
            item.Id,
            item.Seller.Id,
            item.Title,
            item.Description,
            item.Price,
            item.ShippingCost,
            item.Views,
            item.ZipCode,
            item.City,
            item.Category,
            item.CreatedAt);

        await _appDbContext.Items.AddAsync(itemEntity);
        await _appDbContext.SaveChangesAsync();

        foreach (var image in item.ImageUris)
        {
            var imageEntity = new ItemImage(Guid.NewGuid(), item.Id, image);
            await _appDbContext.ItemImages.AddAsync(imageEntity);
        }

        if (await _appDbContext.SaveChangesAsync() > 0)
        {
            return Created();
        }

        return BadRequest();
    }
}
