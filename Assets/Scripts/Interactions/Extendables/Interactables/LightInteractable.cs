using NarrativeGame.Interactions.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NarrativeGame.Interactions.Extendables.Interactables
{
    public class LightInteractable : InstantInteractable
    {
        [SerializeField, Required] private ParticleSystem _litParticles;
        [SerializeField, Required] private Light _light;

        private bool _isLit;

        public override bool CanInteract(IInteractor interactor) => !_isLit;

        protected override void ExecuteInteraction(IInteractor interactor)
        {
            _litParticles.gameObject.SetActive(true);
            _light.gameObject.SetActive(true);

            _isLit = true;
        }
    }
}