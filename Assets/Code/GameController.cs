using UnityEngine;

public class GameController : MonoBehaviour
{
    public float foodSpawnCooldown = .5f;
    public float playerSpawnCooldown = 1f;
    public int winScore = 4;
    public float foodEatenThreshold = 5;
    public float scaleChangeWhenFoodEaten;
    
    public int firstPlayerGlobalScore;
    public int secondPlayerGlobalScore;
    
    public int firstPlayerFoodEatenCount;
    public int secondPlayerFoodEatenCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
