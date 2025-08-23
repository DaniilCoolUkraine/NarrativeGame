using System;
using NarrativeGame.Door.Events;
using SimpleEventBus.SimpleEventBus.Runtime;
using UnityEngine;

namespace NarrativeGame.Door.ConcreteStrategies
{
    [Serializable]
    public class CarryInteractableStrategy : IUnlockStrategy
    {
        [SerializeField] private int    _carryInteractableId;
        private                  Action _onUnlocked;

        public void Initialize(Action onUnlocked)
        {
            GlobalEvents.AddListener<CarryInteractableInTriggerEvent>(OnCarryInteractableInside);
            _onUnlocked = onUnlocked;
        }

        public void Dispose()
        {
            GlobalEvents.RemoveListener<CarryInteractableInTriggerEvent>(OnCarryInteractableInside);
            _onUnlocked = null;
        }

        private void OnCarryInteractableInside(CarryInteractableInTriggerEvent ev)
        {
            if (_carryInteractableId == ev.Id) 
                _onUnlocked.Invoke();
        }
    }
}