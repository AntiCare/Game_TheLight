using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    private Animation _animation;

    public GameObject loadingScreenOverlay;
    public GameObject loadingScreenImage;

    public AnimationClip fadeIn;
    public AnimationClip fadeOut;

    // Start is called before the first frame update
    void Start()
    {
        _animation = GetComponent<Animation>();
    }

    public void FadeIn()
    {
        Time.timeScale  = 1.0f;
        _animation.clip = _animation.GetClip("LoadingScreenFadeIn");
        _animation.Play();
    }
    
    public void FadeOut()
    {
        _animation.clip = _animation.GetClip("LoadingScreenFadeOut");
        _animation.Play();
    }

    public void DisableLoadingScreen()
    {
        FadeOut();
        gameObject.SetActive(false);
    }

    public void ToggleLoadingScreen(bool active)
    {
        loadingScreenOverlay.SetActive(active);
        gameObject.SetActive(active);
        loadingScreenImage.SetActive(active);
    }
}