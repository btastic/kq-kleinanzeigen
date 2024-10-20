using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using KQ.Kleinanzeigen.Domain.Enums;

namespace KQ.Kleinanzeigen.Domain.Entities;

public class Seller
{
    public Seller(
        long id,
        string name,
        Rating? rating,
        Friendliness? friendliness,
        Reliability? reliability,
        DateTimeOffset activeSince,
        bool commercialSeller)
    {
        Id = id;
        Name = name;
        Rating = rating;
        Friendliness = friendliness;
        Reliability = reliability;
        ActiveSince = activeSince;
        CommercialSeller = commercialSeller;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; private set; }
    public string Name { get; private set; }
    public Rating? Rating { get; private set; }
    public Friendliness? Friendliness { get; private set; }
    public Reliability? Reliability { get; private set; }
    public DateTimeOffset ActiveSince { get; private set; }
    public bool CommercialSeller { get; private set; }

    public virtual ICollection<Item> Items { get; private set; } = [];
}
