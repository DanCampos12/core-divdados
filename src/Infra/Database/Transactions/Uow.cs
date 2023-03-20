using Core.Divdados.Infra.SQL.DataContext;
using Core.Divdados.Shared.Uow;

namespace Core.Divdados.Infra.SQL.Transactions;

public class Uow : IUow
{
    private readonly UserDataContext _context;

    public Uow(UserDataContext context)
    {
        _context = context;
    }

    public void Commit()
    {
        _context.SaveChanges();
    }

    public void Rollback()
    {
        
    }
}
