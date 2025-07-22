using NarrativeGame.Animations.Events;
using SimpleEventBus.SimpleEventBus.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NarrativeGame.Animations
{
    public class AnimatorController : MonoBehaviour
    {
        [SerializeField, Required] private Animator _animator;
        
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private float _animationBlend;
        
        private void Awake()
        {
            AssignAnimationIDs();
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void SubscribeToEvents()
        {
            GlobalEvents.AddListener<JumpEvent>(OnJump);
            GlobalEvents.AddListener<FreeFallEvent>(OnFreeFall);
            GlobalEvents.AddListener<GroundedEvent>(OnGrounded);
            GlobalEvents.AddListener<MovementEvent>(OnMovement);
            GlobalEvents.AddListener<ResetJumpEvent>(OnResetJump);
            GlobalEvents.AddListener<ResetFreefallEvent>(OnResetFreefall);
        }

        private void UnsubscribeFromEvents()
        {
            GlobalEvents.RemoveListener<JumpEvent>(OnJump);
            GlobalEvents.RemoveListener<GroundedEvent>(OnGrounded);
            GlobalEvents.RemoveListener<MovementEvent>(OnMovement);
            GlobalEvents.RemoveListener<FreeFallEvent>(OnFreeFall);
            GlobalEvents.RemoveListener<ResetJumpEvent>(OnResetJump);
            GlobalEvents.RemoveListener<ResetFreefallEvent>(OnResetFreefall);
        }
        
        private void OnGrounded(GroundedEvent ev)
        {
            _animator.SetBool(_animIDGrounded, ev.Grounded);
            _animator.SetBool(_animIDFreeFall, !ev.Grounded);
        }

        private void OnMovement(MovementEvent ev)
        {
            _animationBlend = Mathf.Lerp(_animationBlend, ev.Speed, Time.deltaTime * ev.SpeedChangeRate);
            if (_animationBlend < 0.01f) 
                _animationBlend = 0f;

            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, ev.InputMagnitude);
        }
        
        private void OnJump(){}
        
        private void OnJump(JumpEvent ev)
        {
            _animator.SetBool(_animIDJump, true);
        }
        
        private void OnFreeFall(FreeFallEvent ev)
        {
            _animator.SetBool(_animIDFreeFall, true);
        }
        
        private void OnResetFreefall(ResetFreefallEvent ev)
        {
            _animator.SetBool(_animIDFreeFall, false);
        }

        private void OnResetJump(ResetJumpEvent ev)
        {
            _animator.SetBool(_animIDJump, false);
        }
        
#if UNITY_EDITOR
        private void Reset()
        {
            _animator = GetComponent<Animator>();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}