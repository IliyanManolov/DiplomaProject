namespace HomeOwners.Domain.Abstractions;

public interface IDomainEntity
{
    public long? Id { set; get; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}
