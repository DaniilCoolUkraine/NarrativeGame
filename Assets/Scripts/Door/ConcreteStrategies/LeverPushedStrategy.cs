using System;
using NarrativeGame.Interactions.Extendables.Events;
using SimpleEventBus.SimpleEventBus.Runtime;
using UnityEngine;

namespace NarrativeGame.Door.ConcreteStrategies
{
    [Serializable]
    public class LeverPushedStrategy : IUnlockStrategy
    {
        [SerializeField] private int    _leverId;
        private                  Action _onUnlocked;

        public void Initialize(Action onUnlocked)
        {
            GlobalEvents.AddListener<LeverPushedEvent>(OnLeverPushed);
            _onUnlocked = onUnlocked;
        }

        public void Dispose()
        {
            GlobalEvents.RemoveListener<LeverPushedEvent>(OnLeverPushed);
            _onUnlocked = null;
        }

        private void OnLeverPushed(LeverPushedEvent ev)
        {
            if (_leverId == ev.LeverId) 
                _onUnlocked.Invoke();
        }
    }
}