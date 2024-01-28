using DG.Tweening;
using UnityEngine;

public class FoodRotator : MonoBehaviour
{
    public float rotationAngle = 100;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, rotationAngle * Time.deltaTime);
    }
}
