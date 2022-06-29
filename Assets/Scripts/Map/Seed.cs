using UnityEngine;

public class Seed : MonoBehaviour
{
    public Animator animator;

    private AnimatorStateInfo animationInfo;

    // Start is called before the first frame update
    void Start()
    {
        // EntityEvent.OnInteraction += Grow;
    }

    // Update is called once per frame
    void Update()
    {
        if (InteractableSeed.interactedSeed)
        {
            animator.Play("seedGrowUp");
            
            animationInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (animationInfo.normalizedTime >= .99f)
            {
                GetComponent<DropLoot>().GenLoots();
                InteractableSeed.interactedSeed = false;
            }
            
        }
        
    }

    void Grow(Interactable interactable)
    {
        animator = GetComponent<Animator>();
        
        if (InteractableSeed.interactedSeed)
        {
            animator.Play("seedGrowUp");

            animationInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (animationInfo.normalizedTime >= .99f)
            {
                GetComponent<DropLoot>().GenLoots();
                InteractableSeed.interactedSeed = false;
            }
        }
    }
}