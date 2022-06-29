using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;

public class MapIcon : MonoBehaviour
{
    Transform parent;

    void Start()
    {
        var player = GameManager.Instance.Player;

        player.PlayerCharacterInteraction.AddListener(ChangeIcon);

        parent = transform.parent;

        GetComponent<SpriteRenderer>().sprite = parent.GetComponent<SpriteRenderer>().sprite;

        if (parent.TryGetComponent(out NPC npc))
        {
            GetComponent<SpriteRenderer>().sprite = npc.Undiscovered;

            transform.localScale = new Vector3(5f, 5f, 0f);

            if (player.Inventory.Characters.Contains(npc.Character))
            {
                GetComponent<SpriteRenderer>().sprite = npc.Character.CharacterSprite;
                transform.localScale                  = new Vector3(2f, 2f, 0f);
            }
        }
    }

    public void ChangeIcon(Character character)
    {
        if (parent.TryGetComponent(out NPC npc) && npc.Character.CharacterId == character.CharacterId)
        {
            GetComponent<SpriteRenderer>().sprite = character.CharacterSprite;

            transform.localScale = new Vector3(2f, 2f, 0f);
        }
    }
}