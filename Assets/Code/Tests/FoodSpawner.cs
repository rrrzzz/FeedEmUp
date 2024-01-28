using System;
using System.Collections;
using System.Collections.Generic;
using Code;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoodSpawner : MonoBehaviour
{
    public float yOffset = -0.275f;
    public List<GameObject> foodOriginals;
    public Transform spawnedFoodParent;
    public float spawnInterval = 3;
    public float spawnLimit = 20;

    private const float OrangeSize = .4f;
    private const float StrawberrySize = 0.284f;
    private int _foodListCount;
    private int _spawnedFoodCount;
    private readonly Collider[] _colliderHits = new Collider[100];
    private bool _stopSpawning;
    
    void Start()
    {
        PlayerCollisionsController.WinEvent += WinEvent;
        PlayerCollisionsController.FoodEatenEvent += FoodEatenEvent;
        _foodListCount = foodOriginals.Count;
        StartCoroutine(SpawnFood());
    }

    private void FoodEatenEvent(object sender, EventArgs e)
    {
        _spawnedFoodCount--;
    }

    private void WinEvent(object sender, EventArgs e)
    {
        _stopSpawning = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var idx = Random.Range(0, _foodListCount);
            var foodToSpawn = foodOriginals[idx];
            var size = foodToSpawn.name == "Orange" ? OrangeSize / 2 : StrawberrySize / 2;
            Vector3 randomPoint;

            if (TryGenerateRandomPointInsideCircle(out randomPoint, size))
            {
                Instantiate(foodToSpawn, randomPoint, foodToSpawn.transform.rotation, spawnedFoodParent);
            }
        }
    }

    private IEnumerator SpawnFood()
    {
        var waitFor = new WaitForSeconds(spawnInterval);
        
        while (!_stopSpawning)
        {
            if (_spawnedFoodCount <= spawnLimit)
            {
                var idx = Random.Range(0, _foodListCount);
                var foodToSpawn = foodOriginals[idx];
                var size = foodToSpawn.name == "Orange" ? OrangeSize / 2 : StrawberrySize / 2;

                Vector3 randomPoint;
                var isGenerated = TryGenerateRandomPointInsideCircle(out randomPoint, size);
                while (!isGenerated)
                {
                    isGenerated = TryGenerateRandomPointInsideCircle(out randomPoint, size);
                    yield return null;
                }
                _spawnedFoodCount++;
                Instantiate(foodToSpawn, randomPoint, foodToSpawn.transform.rotation, spawnedFoodParent);
            }
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void OnDisable()
    {
        PlayerCollisionsController.WinEvent -= WinEvent;
        PlayerCollisionsController.FoodEatenEvent -= FoodEatenEvent;
    }

    private bool TryGenerateRandomPointInsideCircle(out Vector3 pos, float objectSize)
    {
        var radius = 7;
        float angle = Random.Range(0, 2 * Mathf.PI);
        float r = radius * Mathf.Sqrt(Random.value);
        float x = r * Mathf.Cos(angle);
        float y = r * Mathf.Sin(angle);
        pos = new Vector3(x, yOffset, y);

        var size = new Vector3(objectSize, objectSize, objectSize);
        var hitCount = Physics.OverlapBoxNonAlloc(pos, size, _colliderHits);
   
        for (int i = 0; i < hitCount; i++)
        {
            var go = _colliderHits[i].gameObject;
            if (!go.CompareTag(Constants.FloorTag))
            {
                return false;
            }
        }
        return true;
    }
}
