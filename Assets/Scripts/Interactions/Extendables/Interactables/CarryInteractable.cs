using NarrativeGame.Interactions.Core;
using NarrativeGame.Interactions.Extendables.Events;
using SimpleEventBus.SimpleEventBus.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NarrativeGame.Interactions.Extendables.Interactables
{
    public class CarryInteractable : Interactable
    {
        [SerializeField, Required] private Rigidbody _rigidbody;
        [SerializeField, HideInInspector] private Transform _originalParent;

        private static Vector3 positionOffset = new(0, 1, 1);

        public override bool CanInteract(IInteractor interactor)
        {
            return interactor.CanInteract(this);
        }

        public override void Interact(IInteractor interactor)
        {
            transform.SetParent(interactor.transform);
            transform.localPosition = positionOffset;
            _rigidbody.isKinematic = true;

            GlobalEvents.Publish(new PickupEvent(interactor));
        }

        public override void CancelInteract(IInteractor interactor)
        {
            transform.SetParent(_originalParent);
            _rigidbody.isKinematic = false;

            interactor.ResetInteract();
        }

#if UNITY_EDITOR
        private void Reset()
        {
            _originalParent = transform.parent;
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

    }
}