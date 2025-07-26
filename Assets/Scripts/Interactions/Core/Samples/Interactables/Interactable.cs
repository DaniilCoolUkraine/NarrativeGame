using NarrativeGame.Interactions.Core.Interfaces;
using UnityEngine;

namespace NarrativeGame.Interactions.Core.Samples.Interactables
{
    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _name;

        public abstract bool CanInteract(IInteractor interactor);
        public abstract void Interact(IInteractor interactor);
        public abstract void CancelInteract(IInteractor interactor);

        public virtual string InteractDescription()
        {
            if (string.IsNullOrWhiteSpace(_name))
                return gameObject.name;

            return _name;
        }
    }
}