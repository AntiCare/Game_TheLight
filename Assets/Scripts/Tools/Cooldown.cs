using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooldown
{
    private float _remainingCooldown;
    private float _defaultCooldown;
    public Cooldown(float defaultCooldown)
    {
        _defaultCooldown = defaultCooldown;
    }

    public void CooldownTick()
    {
        if (_remainingCooldown > 0)
            _remainingCooldown--;
    }

    public void changeCooldown(float cooldown)
    {
        this._defaultCooldown = cooldown;
    }

    public void resetCooldown()
    {
        _remainingCooldown = _defaultCooldown;
    }

    public bool isOnCooldown()
    {
        return _remainingCooldown > 0;
    }
}
