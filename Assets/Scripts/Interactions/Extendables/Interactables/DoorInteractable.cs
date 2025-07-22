using Cysharp.Threading.Tasks;
using NarrativeGame.Interactions.Core;
using NarrativeGame.Interactions.Extendables.Events;
using SimpleEventBus.SimpleEventBus.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NarrativeGame.Interactions.Extendables.Interactables
{
    public class DoorInteractable : InstantInteractable
    {
        [SerializeField] private bool _isLocked;
        [SerializeField, Required] private Transform _door;

        [SerializeField, ShowIf("_isLocked")] private int _connectedLeverId;

        private bool _alreadyRotated;
        private bool _isBusy;

        private void Awake()
        {
            GlobalEvents.AddListener<LeverPushedEvent>(OnLeverPushed);
        }

        public override bool CanInteract(IInteractor interactor)
        {
            // return !(_isLocked || _alreadyRotated);
            return !(_isLocked || _isBusy);
        }

        protected override void ExecuteInteraction(IInteractor interactor)
        {
            RotateDoor().Forget();
            _alreadyRotated = !_alreadyRotated;
        }
        
        private async UniTaskVoid RotateDoor()
        {
            _isBusy = true;
            
            float duration = 1f;
            float elapsedTime = 0f;
            Quaternion startRotation = _door.rotation;
            Quaternion targetRotation = _alreadyRotated ? startRotation * Quaternion.Euler(0, 90, 0) : startRotation * Quaternion.Euler(0, -90, 0);

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                _door.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                await UniTask.Yield();
            }

            _door.rotation = targetRotation;
            _isBusy = false;
        }
        
        private void OnLeverPushed(LeverPushedEvent ev)
        {
            if (_connectedLeverId == ev.LeverId) 
                _isLocked = false;
        }
    }
}