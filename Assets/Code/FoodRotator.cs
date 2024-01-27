using UnityEngine;

public class FoodRotator : MonoBehaviour
{
    public float rotationAngle = 20;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, transform.up, rotationAngle * Time.deltaTime);
    }
}
