using NarrativeGame.Interactions.Core.Interfaces;
using NarrativeGame.Interactions.Core.Samples.Interactables;
using NarrativeGame.Puzzles.Core;
using NarrativeGame.Puzzles.Core.Interfaces;
using SimpleEventBus.SimpleEventBus.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NarrativeGame.Interactions.Extendables.Interactables
{
    public class LightInteractable : InstantInteractable, IPuzzle
    {
        [SerializeField, Required] private ParticleSystem _litParticles;
        [SerializeField, Required] private Light _light;

        public bool Solved { get; private set; }
        
        private bool _isLit;

        public override bool CanInteract(IInteractor interactor) => !_isLit;

        protected override void ExecuteInteraction(IInteractor interactor)
        {
            _litParticles.gameObject.SetActive(true);
            _light.gameObject.SetActive(true);
            _isLit = true;

            Solve();
        }

        public void Solve()
        {
            Solved = true;
            GlobalEvents.Publish(new PuzzlePartSolvedEvent(this));
        }
    }
}