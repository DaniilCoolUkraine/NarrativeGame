using NarrativeGame.Interactions.Extendables.Interactables;
using UnityEngine;

namespace NarrativeGame.Door
{
    public class DoorOpenCondition : MonoBehaviour
    {
        [SerializeField, HideInInspector] private DoorInteractable _door;
        [SerializeReference] private IUnlockStrategy[] _unlockStrategies;

        private void OnEnable()
        {
            foreach (var strategy in _unlockStrategies)
                strategy.Initialize(_door.UnlockDoor);
        }

        private void OnDisable()
        {
            foreach (var strategy in _unlockStrategies) 
                strategy.Dispose();
        }

        #if UNITY_EDITOR

        private void Reset()
        {
            _door = GetComponent<DoorInteractable>();
            if (_door == null)
                Debug.LogError("DoorInteractable component is required on the same GameObject as DoorOpenCondition.");
            else
                UnityEditor.EditorUtility.SetDirty(this);
        }
        
        #endif
    }
}