using Cysharp.Threading.Tasks;
using NarrativeGame.Door;
using NarrativeGame.Interactions.Core.Interfaces;
using NarrativeGame.Interactions.Core.Samples.Interactables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NarrativeGame.Interactions.Extendables.Interactables
{
    public class DoorInteractable : InstantInteractable
    {
        [SerializeField] private bool _isLocked;
        [SerializeField, Required] private Transform _door;

        private bool _alreadyRotated;
        private bool _isBusy;

        public void UnlockDoor()
        {
            UpdateLockState(false);
        }

        public void LockDoor()
        {
            UpdateLockState(true);
        }

        private void UpdateLockState(bool isLocked)
        {
            _isLocked = isLocked;
        }

        public override bool CanInteract(IInteractor interactor)
        {
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
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (_isLocked && GetComponent<DoorOpenCondition>() == null) 
                gameObject.AddComponent<DoorOpenCondition>();

            if (!_isLocked && TryGetComponent<DoorOpenCondition>(out var component)) 
                DestroyImmediate(component);

            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }
}