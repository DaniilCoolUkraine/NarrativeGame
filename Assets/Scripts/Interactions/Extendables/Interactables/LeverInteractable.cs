using Cysharp.Threading.Tasks;
using NarrativeGame.Interactions.Core;
using NarrativeGame.Interactions.Extendables.Events;
using SimpleEventBus.SimpleEventBus.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NarrativeGame.Interactions.Extendables.Interactables
{
    public class LeverInteractable : InstantInteractable
    {
        [SerializeField, Required] private Transform _handle;
        [SerializeField] private int _leverId;

        private bool _alreadyRotated;

        public override bool CanInteract(IInteractor interactor)
        {
            return !_alreadyRotated;
        }

        public override string InteractDescription()
        {
            throw new System.NotImplementedException();
        }

        protected override void ExecuteInteraction(IInteractor interactor)
        {
            _alreadyRotated = true;
            RotateDoor().Forget();
            GlobalEvents.Publish(new LeverPushedEvent(_leverId));
        }

        private async UniTaskVoid RotateDoor()
        {
            float duration = 1f;
            float elapsedTime = 0f;
            Quaternion startRotation = _handle.rotation;
            Quaternion targetRotation = startRotation * Quaternion.Euler(90, 0, 0);

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                _handle.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                await UniTask.Yield();
            }

            _handle.rotation = targetRotation;
        }
    }
}