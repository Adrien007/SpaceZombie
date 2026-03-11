//ResetEtatManager.cs
using System.Collections.Generic;

namespace SpaceZombie.Events
{
    public interface IResetEtatManager
    {
        public void ResetToInitaialState();
        public void StartTimerState();
    }
    public class ResetEtatManager : IResetEtatNotifier, IResetEtatManager
    {
        private List<IResetEtatObserver> _observers = new();
        public ResetEtatManager()
        {
        }
        public void Register(IResetEtatObserver observer) => _observers.Add(observer);
        public void Unregister(IResetEtatObserver observer) => _observers.Remove(observer);

        public void ResetToInitaialState()
        {
            foreach (var observer in _observers)
            {
                observer.OnResetToInitaialState();
            }
        }

        public void StartTimerState()
        {
            foreach (var observer in _observers)
            {
                observer.StartTimerState();
            }
        }
    }
}