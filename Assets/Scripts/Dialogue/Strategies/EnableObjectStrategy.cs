using System;
using UnityEngine;

namespace NarrativeGame.Dialogue.Strategies
{
    [Serializable]
    public class EnableObjectStrategy : IDialogueEndStrategy
    {
        [SerializeField] private GameObject _objectToEnable;

        public void Execute()
        {
            _objectToEnable.SetActive(true);
        }
    }
}