//IResetEtatObserver.cs
namespace SpaceZombie.Events
{
    public interface IResetEtatObserver
    {
        void OnResetToInitaialState();
    }

    public interface IResetEtatNotifier
    {
        void Register(IResetEtatObserver observer);
        void Unregister(IResetEtatObserver observer);
    }
}