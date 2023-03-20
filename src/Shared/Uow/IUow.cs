namespace Core.Divdados.Shared.Uow;

public interface IUow
{
    void Commit();
    void Rollback();
}
