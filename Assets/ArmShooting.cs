using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmShooting : MonoBehaviour
{
    private Player _player;

    private PlayerCombat _playerCombat;
    // Start is called before the first frame update
    void Start()
    {
        _player       = transform.parent.GetComponent<Player>();
        _playerCombat = _player.GetComponent<PlayerCombat>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnReload()
    {
        _playerCombat.OnReload();
    }
    
    public void OnArrowRelease()
    {
       _playerCombat.OnArrowRelease();
    }
}
