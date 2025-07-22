using Sirenix.OdinInspector;
using StarterAssets;
using UnityEngine;

namespace NarrativeGame.Interactions.Core
{
    public class RaycastInteractor : MonoBehaviour, IInteractor 
    {
        [Header("Reference")]
        [SerializeField, Required] private StarterAssetsInputs _playerInput;
        [SerializeField, Required] private Camera _camera;

        [Header("Settings")] 
        [SerializeField] private float _range;
        [SerializeField] private LayerMask _interactableLayerMask;

        private bool _isBusy;
        private IInteractable _currentInteractable;

        private void Update()
        {
            if (_playerInput.Interact)
            {
                if (_isBusy)
                {
                    _currentInteractable.CancelInteract(this);
                    _currentInteractable = null;
                }
                else
                {
                    SendRaycast();
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
            _currentInteractable = interactable;

            interactable.Interact(this);
        }

        public void ResetInteract()
        {
            _isBusy = false;
        }

        private void SendRaycast()
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray aimRay = _camera.ScreenPointToRay(screenCenter);

            if (!Physics.Raycast(aimRay, out RaycastHit hit, _range, _interactableLayerMask)) 
                return;
            if (!hit.transform.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable = hit.transform.GetComponentInParent<IInteractable>();
                if (interactable == null)
                    return;
            }

            if (interactable.CanInteract(this))
                Interact(interactable);
        }

#if UNITY_EDITOR
        private void Reset()
        {
            _camera = Camera.main;
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}