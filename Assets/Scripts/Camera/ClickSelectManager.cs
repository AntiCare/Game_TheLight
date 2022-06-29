using UnityEngine;
using UnityEngine.EventSystems;

public class ClickSelectManager : MonoBehaviour
{
    [SerializeField] private Camera _camera;
   
    void FixedUpdate()
    {
        // CheckNCPDialog();
    }

    void CheckNCPDialog()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            RaycastHit2D hit = Physics2D.Raycast(
                GameTools.GetMousePositionWhereCameraZIs70(_camera, Input.mousePosition),
                GameTools.GetMousePositionWhereCameraZIs70(_camera, Input.mousePosition),
                1f);

            if (hit.collider != null)
            {
                hit.collider.GetComponent<Entity>().Interact();
            }
        }
    }
}