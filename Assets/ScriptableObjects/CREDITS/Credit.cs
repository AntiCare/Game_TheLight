using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Credit", menuName = "Game Credits/Credit", order = 1)]
[Serializable]
public class Credit : ScriptableObject
{
   public string creditText;
}
