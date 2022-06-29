using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected Rigidbody2D rb2d;
    protected Vector2 startSpeed = new Vector2(-5f, -7f);
    protected float timer = 1.5f;
    
    protected bool         hasInteracted;
    public  Interactable interactable;
    
    public abstract Entity           Interact();
    public abstract Entity           StartInteraction(bool enable);
    public abstract ScriptableObject GetScriptableObject();
    public abstract void             SpecialInteraction(bool enable);
    
    
    protected void DropLoot()
    {
        TryGetComponent(out Rigidbody2D rb);
        
        rb2d = rb;
        if (rb2d == null)
        {
            return;    
        }
        
        rb2d.velocity = transform.right * startSpeed.x + transform.up * startSpeed.y;
        GetComponent<Collider2D>().enabled = true;
    }
}