using System;
using UnityEngine;

[Serializable]
public abstract class SaveData
{
    public Guid    id;
    public Vector3 positionData;

    protected SaveData(float[] position)
    {
        positionData = new Vector3(1620f, -2190, 0);
    }

    protected SaveData()
    {
    }
}