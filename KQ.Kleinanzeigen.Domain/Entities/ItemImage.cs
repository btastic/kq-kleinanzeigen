namespace KQ.Kleinanzeigen.Domain.Entities;

public class ItemImage
{
    public ItemImage(Guid id, long itemId, string imageUrl)
    {
        Id = id;
        ItemId = itemId;
        ImageUrl = imageUrl;
    }

    public Guid Id { get; private set; }

    public long ItemId { get; private set; }
    public virtual Item Item { get; private set; } = null!;

    public string ImageUrl { get; private set; }
}
