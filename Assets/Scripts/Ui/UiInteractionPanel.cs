using NarrativeGame.Interactions.Core;
using SimpleEventBus.SimpleEventBus.Runtime;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace NarrativeGame.Ui
{
    public class UiInteractionPanel : MonoBehaviour
    {
        [SerializeField, Required] private CanvasGroup _hintPanel;
        [SerializeField, Required] private TextMeshProUGUI _nameText;

        private void OnEnable()
        {
            GlobalEvents.AddListener<InteractableChangedEvent>(OnInteractableChanged);
        }

        private void OnDisable()
        {
            GlobalEvents.RemoveListener<InteractableChangedEvent>(OnInteractableChanged);
        }

        private void OnInteractableChanged(InteractableChangedEvent ev)
        {
            if (ev.Interactable == null)
            {
                _hintPanel.gameObject.SetActive(false);
                return;
            }

            if (ev.Interactor.CanInteract(ev.Interactable) && ev.Interactable.CanInteract(ev.Interactor))
            {
                _hintPanel.gameObject.SetActive(true);
                _hintPanel.alpha = 1;
            }
            else
            {
                _hintPanel.gameObject.SetActive(true);
                _hintPanel.alpha = 0.5f;
            }

            _nameText.text = ev.Interactable.InteractDescription();
        }
    }
}