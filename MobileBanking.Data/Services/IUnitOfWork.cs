namespace MobileBanking.Data.Services;
public interface IUnitOfWork : IDisposable
{
    void Begin();
    void Commit();
    void RollBack();

}
