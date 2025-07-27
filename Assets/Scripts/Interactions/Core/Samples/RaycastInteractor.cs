using Cysharp.Threading.Tasks;
using NarrativeGame.Interactions.Core.Interfaces;
using NarrativeGame.Interactions.Core.Samples.Events;
using SimpleEventBus.SimpleEventBus.Runtime;
using Sirenix.OdinInspector;
using StarterAssets;
using UnityEngine;

namespace NarrativeGame.Interactions.Core.Samples
{
    public class RaycastInteractor : MonoBehaviour, IInteractor, IEnablable
    {
        [Header("Reference")]
        [SerializeField, Required] private StarterAssetsInputs _playerInput;
        [SerializeField, Required] private Camera _camera;

        [Header("Settings")] 
        [SerializeField] private float _range;
        [SerializeField] private LayerMask _interactableLayerMask;

        private bool _isBusy;
        private bool _inputBlocked;

        private IInteractable _currentInteractable;
        
        private void OnEnable()
        {
            SendRaycastDelayed().Forget();
        }

        private void Update()
        {
            if (_inputBlocked)
                return;
            
            if (_playerInput.Interact)
            {
                if (_isBusy)
                {
                    _currentInteractable.CancelInteract(this);
                    _currentInteractable = null;
                }
                else
                {
                    if (_currentInteractable?.CanInteract(this) ?? false)
                        Interact(_currentInteractable);
                }

                _playerInput.ResetInteract();
            }
        }

        public bool CanInteract(IInteractable interactable)
        {
            return !_isBusy;
        }

        public void Interact(IInteractable interactable)
        {
            _isBusy = true;
            interactable.Interact(this);
        }

        public void ResetInteract()
        {
            _isBusy = false;
        }

        private async UniTaskVoid SendRaycastDelayed()
        {
            while (gameObject.activeSelf)
            {
                if (_inputBlocked)
                {
                    await UniTask.NextFrame();
                    continue;
                }

                for (int i = 0; i < 5; i++) 
                    await UniTask.NextFrame();

                SendRaycast();
            }
        }
        
        private void SendRaycast()
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray aimRay = _camera.ScreenPointToRay(screenCenter);

            IInteractable interactable = null;

            if (Physics.Raycast(aimRay, out RaycastHit hit, _range, _interactableLayerMask))
            {
                if (!hit.transform.TryGetComponent<IInteractable>(out interactable))
                    interactable = hit.transform.GetComponentInParent<IInteractable>();
            }

            if (interactable != _currentInteractable)
            {
                _currentInteractable = interactable;
                GlobalEvents.Publish(new InteractableChangedEvent(_currentInteractable, this));
            }
        }

#if UNITY_EDITOR
        private void Reset()
        {
            _camera = Camera.main;
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
        public void Enable()
        {
            _inputBlocked = false;
        }

        public void Disable()
        {
            _inputBlocked = true;
        }
    }
}