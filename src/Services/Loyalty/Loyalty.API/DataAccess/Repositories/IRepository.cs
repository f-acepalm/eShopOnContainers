namespace Loyalty.API.DataAccess.Repositories;

public interface IRepository
{
    IUnitOfWork UnitOfWork { get; }
}
