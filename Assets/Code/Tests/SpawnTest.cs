using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnTest : MonoBehaviour
{
    public Transform spawnPointsParent;
    private List<Transform> spawnPoints;

    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = spawnPointsParent.GetComponentsInChildren<Transform>().Skip(1).ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnAtRandomPoint();
        }
    }

    private void SpawnAtRandomPoint()
    {
        var range = spawnPoints.Count;
        var idx = Random.Range(0, range);
        var spawnPoint = spawnPoints[idx];
        player.transform.position = spawnPoint.position;
        var rotation = Quaternion.Euler(-90, spawnPoint.rotation.eulerAngles.y, 0);
        player.transform.rotation = rotation;
    }
}
