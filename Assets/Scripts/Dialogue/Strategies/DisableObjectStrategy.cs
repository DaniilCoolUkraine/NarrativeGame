using System;
using UnityEngine;

namespace NarrativeGame.Dialogue.Strategies
{
    [Serializable]
    public class DisableObjectStrategy : IDialogueEndStrategy
    {
        [SerializeField] private GameObject _objectToDisable;

        public void Execute()
        {
            _objectToDisable.SetActive(false);
        }
    }
}