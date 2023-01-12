namespace Loyalty.API.DataAccess.Entities;

public class MemberTier
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Discount { get; set; }

    public int Threshold { get; set; }
}
