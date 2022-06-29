using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

namespace MyWorkSpace
{    
    [RequireComponent(typeof(VideoPlayer))]
    public class AnimationBackground : MonoBehaviour
    {
        private VideoPlayer videoPlayer;
        public VideoClip a;
        private RawImage rawImage;
        [SerializeField]
        [Range(0f, 1f)] public float Fadespeed=1f; 
 
        private void Awake()
        {
         
            videoPlayer = this.GetComponent<VideoPlayer>();
            rawImage = this.GetComponent<RawImage>();
        }
 
        void Start()
        {            
         
            videoPlayer.isLooping = false;
            videoPlayer.clip = a;
        }
 
 
        void Update()
        {
           
            if (videoPlayer.texture == null)
            {
                return;
            }
 
           
            rawImage.texture = videoPlayer.texture;
            VideoFade();
        }
       
        public void VideoFade()
        {
            videoPlayer.Play();
            rawImage.color = Color.Lerp(rawImage.color, Color.white,Fadespeed*Time.deltaTime);
            
        }
 
 
    }
}