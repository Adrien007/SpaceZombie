//IResetEtatObserver.cs
namespace SpaceZombie.Events
{
    public interface IResetEtatObserver
    {
        void OnResetToInitaialState();
        void StartTimerState();
    }

    public interface IResetEtatNotifier
    {
        void Register(IResetEtatObserver observer);
        void Unregister(IResetEtatObserver observer);
    }
}