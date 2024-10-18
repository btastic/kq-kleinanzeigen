using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KQ.Kleinanzeigen.Domain.Entities;

public class Item
{
    public Item(
        long id,
        long sellerId,
        string title,
        string description,
        decimal price,
        decimal shippingCost,
        int views,
        string zipCode,
        string city,
        string category,
        DateTimeOffset createdAt)
    {
        Id = id;
        SellerId = sellerId;
        Title = title;
        Description = description;
        Price = price;
        ShippingCost = shippingCost;
        Views = views;
        ZipCode = zipCode;
        City = city;
        Category = category;
        CreatedAt = createdAt;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; private set; }

    public long SellerId { get; private set; }
    public virtual Seller Seller { get; private set; } = null!;

    public string Title { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public decimal ShippingCost { get; private set; }
    public int Views { get; private set; }
    public string ZipCode { get; private set; }
    public string City { get; private set; }
    public string Category { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public virtual ICollection<ItemImage> Images { get; private set; } = new HashSet<ItemImage>();
}
