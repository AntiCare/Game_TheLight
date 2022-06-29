using System.Collections;
using Enums;
using Interfaces;
using StateMachine.State;
using UnityEngine;

namespace StateMachine
{
    public class StateMachine : MonoBehaviour
    {
        private ArrayList states = new ArrayList();
        private States    _currentstate;
        public  States    Currentstate => _currentstate;
        private Animator  _animator;
        private float     _horizontal;
        private float     _vertical;
        private bool      isLocked = false;

        //changeable in unity
        public float      runspeed     = 35.0f;
        public float      dashspeed    = 100.0f;
        public float      dashDistance = 35.0f;
        public float      dashCooldown = 1.5f; //this is in seconds
        public float      hurtCooldown = 2.0f; // this is in seconds
        public GameObject bowAndArrowRight;
        public GameObject bowAndArrowLeft;

        [SerializeField] private PlayerCombat playerCombat;

        [SerializeField] private AudioClip footStepSoundClip;
        public                   AudioClip hurtSoundClip;

        private AudioSource _audioSource;
        private bool        start       = false;
        private bool        newAnimator = false;
        
        public void ToggleLockInput(bool locked)
        {
            isLocked = locked;
            playerCombat.ToggleLockInput(locked);
        }

        void Start()
        {
            GameManager.OnColorChange -= ChangeAnimator;
            if (!start)
            {
                GameManager.OnColorChange += ChangeAnimator;
                start                     =  true;
            }

            _currentstate = States.idle;
            
            if (GetComponent<Animator>() == null)
            {
                return;
            }
            
            _animator = GetComponent<Animator>();

            states.AddRange(new ArrayList()
            {
                new Idle(),
                new Dash(GetComponent<Rigidbody2D>(), dashspeed, dashDistance, dashCooldown, GetComponent<Animator>()), //Dash needs to be before Movement
                new Movement(GetComponent<Rigidbody2D>(), runspeed, _animator),
                new Hurt(GetComponent<Player>(), hurtCooldown),
                new Dead(GetComponent<Player>())
            });
            
            _audioSource = GetComponent<AudioSource>();
            
            GetComponent<Player>().PlayerTakeDamage.AddListener(PlayerHurt);
        }

        void Update()
        {
            if (isLocked)
            {
                return;
            }

            _horizontal = Input.GetAxisRaw("Horizontal");
            _vertical   = Input.GetAxisRaw("Vertical");
        }

        private void FixedUpdate()
        {
            if (isLocked)
            {
                return;
            }

            if (states.Count < 1)
            {
                return;
            }

            foreach (IState state in states)
            {
                _currentstate = state.Action(_currentstate);
            }

            AnimationHandler();
        }

        void AnimationHandler()
        {
            if (isLocked)
            {
                return;
            }

            if (_currentstate != States.dashing &&
                (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Dash_blend_tree") ||
                    !_animator.GetCurrentAnimatorStateInfo(0).IsName("Color_dash_blend_tree"))) //bug fix for now
            {
                _animator.SetFloat("HorizontalSpeed", _horizontal);
                _animator.SetFloat("VerticalSpeed", _vertical);
            }

            _animator.SetBool("IsMoving", _currentstate == States.moving || _currentstate == States.dashing);
            _animator.SetBool("IsDashing", _currentstate == States.dashing);

            if (!newAnimator)
            {
                _animator.SetBool("IsDead", _currentstate == States.dead);
                _animator.SetBool("IsHurt", _currentstate == States.hurt);
            }
        }

        void ChangeAnimator()
        {
            states.Clear();
            newAnimator = true;
            Start();
            
        }

        public void FootStep()
        {
            if (_audioSource.isPlaying)
            {
                return;
            }

            _audioSource.clip = footStepSoundClip;
            _audioSource.Play();
        }

        void PlayerHurt(Player player)
        {
            if (_audioSource.isPlaying)
            {
                return;
            }

            _audioSource.clip = hurtSoundClip;
            _audioSource.Play();
        }
    }
}