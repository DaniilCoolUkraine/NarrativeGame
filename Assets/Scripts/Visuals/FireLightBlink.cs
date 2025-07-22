using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NarrativeGame.Visuals
{
    [RequireComponent(typeof(Light))]
    public class FireLightBlink : MonoBehaviour
    {
        [SerializeField, Required] private Light _sourceLight;
        [SerializeField] private AnimationCurve _loopLightIntensityCurve;
        [SerializeField] private AnimationCurve _startLightIntensityCurve;

        private float _elapsedTime;
        private float _startDuration;
        
#if UNITY_EDITOR
        private void Reset()
        {
            if (_sourceLight == null)
            {
                _sourceLight = GetComponent<Light>();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }  
#endif

        private void Awake()
        {
            _startDuration = _startLightIntensityCurve.keys[_startLightIntensityCurve.length - 1].time;
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime < _startDuration)
            {
                _sourceLight.intensity = _startLightIntensityCurve.Evaluate(_elapsedTime);
            }
            else
            {
                float loopDuration = _loopLightIntensityCurve.keys[_loopLightIntensityCurve.length - 1].time;
                float loopTime = (_elapsedTime - _startDuration) % loopDuration;
                _sourceLight.intensity = _loopLightIntensityCurve.Evaluate(loopTime);
            }
        }
    }
}