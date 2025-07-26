using NarrativeGame.Interactions.Core.Interfaces;
using UnityEngine;

namespace NarrativeGame.Interactions.Core.Samples.Interactables
{
    public class LogInteractable : InstantInteractable
    {
        public override bool CanInteract(IInteractor interactor) => true;

        protected override void ExecuteInteraction(IInteractor interactor)
        {
            Debug.Log(transform.position);
        }
    }
}