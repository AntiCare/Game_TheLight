using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public AudioSource _audioSource;

    public AudioClip audioClip;

    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator    = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.CompareTag("Arrow"))
        {
            _animator.SetTrigger("OnHit");
            PlayHurtSound();
            other.collider.GetComponent<Arrow>().gameObject.SetActive(false);
        }
    }

    void PlayHurtSound()
    {
        _audioSource.clip = audioClip;
       _audioSource.Play();
    }
}
