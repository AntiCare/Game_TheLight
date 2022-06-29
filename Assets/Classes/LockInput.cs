using UnityEngine;

public class LockInput : MonoBehaviour
{
    private Player lulu;

    private void OnEnable()
    {
        if (GameManager.Instance.Player == null)
        {
            return;
        }

        lulu = GameManager.Instance.Player;
    }

    private void OnDisable()
    {
        lulu = GameManager.Instance.Player;

        if (lulu == null)
        {
            return;
        }

        lulu.GetComponent<StateMachine.StateMachine>().ToggleLockInput(false);
    }

    public void OnMouseEnter()
    {
        if (lulu == null)
        {
            lulu = GameManager.Instance.Player;
        }

        lulu.GetComponent<StateMachine.StateMachine>().ToggleLockInput(true);
    }

    public void OnMouseExit()
    {
        if (lulu == null)
        {
            lulu = GameManager.Instance.Player;
        }

        lulu.GetComponent<StateMachine.StateMachine>().ToggleLockInput(false);
    }
}