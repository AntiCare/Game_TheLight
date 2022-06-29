using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private Animation     _animator;
    public AnimationClip _fadeOut;
    public AnimationClip _fadeIn;
    public  GameObject    buttons;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animation>();
        buttons.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void FadeOut()
    {
        gameObject.SetActive(true);
        _animator.clip = _fadeOut;
        _animator.Play();
    }
    
    public void FadeIn()
    {
        _animator.clip = _fadeIn;
        _animator.Play();
    }

    public void FadeInComplete()
    {
        gameObject.SetActive(false);
    }
    
    public void HandleYesButton()
    {
        GameManager.Instance.RestartGameFromLastSave();
    }

    public void HandleNoButton()
    {
        GameManager.Instance.MainMenu();
    }

    public void ShowButtonsAndText()
    {
    
    }
}