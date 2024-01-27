using UnityEngine;

[CreateAssetMenu(menuName = "CreateGameSettings", fileName = "GameSettings")]
public class GameSettings : ScriptableObject
{
    public float foodSpawnCooldown = .5f;
    public float playerSpawnCooldown = 1;
    public int winScore = 4;
    public float foodEatenThreshold = 5;
}
