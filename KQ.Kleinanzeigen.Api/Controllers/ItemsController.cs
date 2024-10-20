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

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpDelete]
    public async Task<IActionResult?> DeleteAll()
    {
        foreach(var item in _appDbContext.Items)
        {
            _appDbContext.Remove(item);
        }

        await _appDbContext.SaveChangesAsync();

        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult?> Delete([FromRoute] long id)
    {
        var existingItem = await _appDbContext.Items.FindAsync(id);

        if (existingItem is null)
        {
            return NotFound();
        }

        _appDbContext.Remove(existingItem);

        await _appDbContext.SaveChangesAsync();

        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPatch("{id:long}")]
    public async Task<IActionResult?> Update([FromRoute] long id, UpdateItemRequest updateItemRequest)
    {
        var existingItem = await _appDbContext.Items.FindAsync(id);

        if (existingItem is null)
        {
            return NotFound();
        }

        existingItem.SetTitle(updateItemRequest.Title);
        existingItem.SetDescription(updateItemRequest.Description);
        existingItem.SetPrice(updateItemRequest.Price);
        existingItem.SetStatus(updateItemRequest.Status);

        _appDbContext.Update(existingItem);

        await _appDbContext.SaveChangesAsync();

        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemDetailsResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:long}")]
    public async Task<IActionResult?> GetById([FromRoute] long id)
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
                    existingItem.Seller.ActiveSince,
                    existingItem.Seller.CommercialSeller),
                existingItem.Images.Select(i => i.ImageUrl).ToArray(),
                existingItem.Title,
                existingItem.Description,
                existingItem.Price,
                existingItem.Shipping,
                existingItem.ZipCode,
                existingItem.City,
                existingItem.Category,
                existingItem.Status,
                existingItem.PostedAt,
                existingItem.CreatedAt,
                existingItem.UpdatedAt));
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ItemListingResponse>))]
    [HttpGet]
    public async Task<IActionResult?> GetAll([FromQuery(Name = "s")] int? sellerId)
    {
        var existingItems = _appDbContext.Items.OrderByDescending(p => p.Id).AsQueryable();

        if (sellerId is not null)
        {
            existingItems = existingItems.Where(p => p.SellerId == sellerId);
        }

        if (!await existingItems.AnyAsync())
        {
            return Ok(Enumerable.Empty<ItemDetailsResponse>());
        }

        return Ok(
            existingItems.Select(p =>
                new ItemListingResponse(
                    p.Id,
                    p.Title,
                    p.Price,
                    p.Shipping,
                    p.Status,
                    p.Category,
                    p.PostedAt,
                    p.CreatedAt,
                    p.UpdatedAt)));
    }

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateItemRequest item)
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
                item.Seller.ActiveSince,
                item.Seller.CommercialSeller);

            await _appDbContext.Sellers.AddAsync(sellerEntity);
        }

        var itemEntity = new Item(
            item.Id,
            item.Seller.Id,
            item.Title,
            item.Description,
            item.Price,
            item.Shipping,
            item.ZipCode,
            item.City,
            item.Category,
            item.Status,
            item.PostedAt);

        await _appDbContext.Items.AddAsync(itemEntity);

        foreach (var image in item.ImageUris)
        {
            var imageEntity = new ItemImage(Guid.NewGuid(), item.Id, image);
            await _appDbContext.ItemImages.AddAsync(imageEntity);
        }

        if (await _appDbContext.SaveChangesAsync() > 0)
        {
            return Created($"/items/{item.Id}", item);
        }

        return BadRequest();
    }
}
