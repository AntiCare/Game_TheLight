using UnityEngine;
using System;

public class DestroyStuff : MonoBehaviour
{
    public Animator animator;
    public String animationName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.CompareTag("Arrow"))
        {
            animator.Play(animationName);
            //animation to destroy arrow
            other.collider.GetComponent<Arrow>().gameObject.SetActive(false);


            if (transform.CompareTag("Tree"))
            {
                transform.GetComponent<BoxCollider2D>().offset = new Vector2(0.01286089f,-0.998208f );
                transform.GetComponent<BoxCollider2D>().size = new Vector2(0.3535168f,0.06692356f );
            }
            else if(transform.CompareTag("Barrel"))
            {
                transform.GetComponent<BoxCollider2D>().offset = new Vector2(-0.007999018f,-0.3227147f );
                transform.GetComponent<BoxCollider2D>().size = new Vector2(0.4725505f,0.08886263f );
            }
            else if(transform.CompareTag("Barrels"))
            {
                transform.GetComponent<BoxCollider2D>().offset = new Vector2(-0.01431596f,-0.5830975f );
                transform.GetComponent<BoxCollider2D>().size = new Vector2(1.46103f,0.1220969f );
            }
            
            
        }
    }
}
