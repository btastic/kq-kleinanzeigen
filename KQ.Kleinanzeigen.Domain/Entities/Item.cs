using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using KQ.Kleinanzeigen.Domain.Enums;

namespace KQ.Kleinanzeigen.Domain.Entities;

public class Item
{
    public Item(
        long id,
        long sellerId,
        string title,
        string description,
        decimal price,
        string shipping,
        string zipCode,
        string city,
        string category,
        Status status,
        DateTimeOffset postedAt)
    {
        Id = id;
        SellerId = sellerId;
        Title = title;
        Description = description;
        Price = price;
        Shipping = shipping;
        ZipCode = zipCode;
        City = city;
        Category = category;
        Status = status;
        PostedAt = postedAt;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; private set; }

    public long SellerId { get; private set; }
    public virtual Seller Seller { get; private set; } = null!;

    public string Title { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public string Shipping { get; private set; }
    public string ZipCode { get; private set; }
    public string City { get; private set; }
    public string Category { get; private set; }
    public Status Status { get; private set; }
    public DateTimeOffset PostedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public virtual ICollection<ItemImage> Images { get; private set; } = [];

    public void SetTitle(string title)
    {
        if (title == Title)
        {
            return;
        }

        Title = title;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetDescription(string description)
    {
        if (description == Description)
        {
            return;
        }

        Description = description;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetPrice(decimal price)
    {
        if (price == Price)
        {
            return;
        }

        Price = price;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetStatus(Status status)
    {
        if (status == Status)
        {
            return;
        }

        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
