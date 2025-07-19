using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsSO", menuName = "Scriptable Objects/PlayerStatsSO")]
public class PlayerStatsSO : ScriptableObject
{
    public int health = 100;
    public int timeModifier = 0;
    public bool isBlocking;
    public Distance distance;
    public Stance stance;
}

public enum Distance { Ranged, Mid, Pocket }

public enum Stance { Orthodox, Soutpaw }
