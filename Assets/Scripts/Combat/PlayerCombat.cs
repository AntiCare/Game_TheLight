using System.Linq;
using Enums;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator    _animator;
    public  Animator    _animatorBowAndArrowRight;
    public  Animator    _animatorBowAndArrowLeft;
    public  GameObject  arrow;
    public  GameObject  arrowSpawn;
    public  GameObject  bowAndArrowRight;
    public  GameObject  bowAndArrowLeft;
    private bool        isLocked    = false;
    private bool        permaLock   = false;
    private bool        isAttacking = false;
    private AudioSource _audioSource;

    private StateMachine.StateMachine _stateMachine;

    [SerializeField] private AudioClip arrowSoundClip;

    void Start()
    {
        _animator     = gameObject.GetComponent<Animator>();
        _audioSource  = GetComponent<AudioSource>();
        _stateMachine = GetComponent<StateMachine.StateMachine>();
    }

    public void OnReload()
    {
        Fire();
    }

    void Fire()
    {
        Vector3 mousePosition = GameTools.GetMousePositionWhereCameraZIs70(Camera.main, Input.mousePosition);

        GameObject arrowToShoot = ObjectPool.objectPoolInstance.GetPooledObject();

        Arrow arrowToFire = arrowToShoot.GetComponent<Arrow>();

        if (arrowToShoot != null)
        {
            arrowToShoot.transform.position = arrowSpawn.transform.position;

            Vector3 arrowPDirection = (mousePosition - transform.position).normalized;

            var arrowAngle = Mathf.Atan2(arrowPDirection.y, arrowPDirection.x) * Mathf.Rad2Deg;

            arrowToShoot.transform.eulerAngles = new Vector3(0, 0, arrowAngle);

            bowAndArrowRight.transform.eulerAngles = new Vector3(0, 0, arrowAngle);
            bowAndArrowLeft.transform.eulerAngles  = new Vector3(0, 0, arrowAngle - 180f);

            arrowToShoot.SetActive(true);

            arrowToFire.Fire();

            PlaySound();
        }
    }

    void PlaySound()
    {
        _audioSource.clip = arrowSoundClip;
        _audioSource.Play();
    }

    void FixedUpdate()
    {
        ClearParameters();

        InputHandler();
    }

    public void OnArrowRelease()
    {
        if (
            bowAndArrowLeft  == null ||
            bowAndArrowRight == null
        )
        {
            return;
        }

        bowAndArrowRight.SetActive(false);
        bowAndArrowLeft.SetActive(false);
        isAttacking = false;
        ClearParameters();
    }

    public void ToggleLockInput(bool locked) => isLocked = locked;
    public void TogglePermaLock(bool locked) => permaLock = locked;

    private void InputHandler()
    {
        if (isLocked || permaLock)
        {
            return;
        }

        if (Input.GetButton("Fire1"))
        {
            var mouseDirection = (
                GameTools.GetMousePositionWhereCameraZIs70(
                    Camera.main, Input.mousePosition
                ) - transform.position).normalized;

            SetShootingDirectionBasedOnAngle(Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg);
            _animator.SetTrigger("IsAttacking");
            isAttacking = true;
        }
    }

    private void ClearParameters()
    {
        if (HasParameter("IsAttacking", _animator))
        {
            _animator.ResetTrigger("IsAttacking");
        }

        if (HasParameter("IsShootingRight", _animator))
        {
            _animator.SetBool("IsShootingRight", false);
        }

        if (HasParameter("IsShootingLeft", _animator))
        {
            _animator.SetBool("IsShootingLeft", false);
        }

        if (HasParameter("IsShootingUp", _animator))
        {
            _animator.SetBool("IsShootingUp", false);
        }

        if (HasParameter("IsShootingDown", _animator))
        {
            _animator.SetBool("IsShootingDown", false);
        }

        isAttacking = false;
    }

    public static bool HasParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }

        return false;
    }

    private void SetShootingDirectionBasedOnAngle(float angle)
    {
        if (
            bowAndArrowLeft            == null        ||
            bowAndArrowRight           == null        ||
            _stateMachine.Currentstate != States.idle ||
            isAttacking
        )
        {
            return;
        }

        if (-45f < angle && angle < 30f)
        {
            bowAndArrowRight.SetActive(true);
            _animatorBowAndArrowRight.SetBool("ShootRight", true);
            _animator.SetBool("IsShootingRight", true);
        }

        if (155f > angle && angle > 30f)
        {
            _animator.SetBool("IsShootingUp", true);
        }

        if (-135f < angle && 155f < angle)
        {
            bowAndArrowLeft.SetActive(true);
            _animatorBowAndArrowLeft.SetBool("ShootLeft", true);
            _animator.SetBool("IsShootingLeft", true);
        }

        if (-135f > angle && angle > -180f)
        {
            bowAndArrowLeft.SetActive(true);
            _animatorBowAndArrowLeft.SetBool("ShootLeft", true);
            _animator.SetBool("IsShootingLeft", true);
        }

        if (-135f < angle && angle < -45f)
        {
            _animator.SetBool("IsShootingDown", true);
        }
    }
}