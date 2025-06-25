using UnityEngine;

public class MoveQuakes : MonoBehaviour
{
    public Transform CityPosition;
    public Vector3 LastPosition;

    public Vector3 LastVelocity = Vector3.zero;

    public float AccelerationRumble = 30f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LastPosition = CityPosition.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Velocity = CityPosition.position - LastPosition;
        float Acceleration = (Velocity - LastVelocity).magnitude;
        LastVelocity = Velocity;

        MoveCamera.SetRumble(Acceleration*AccelerationRumble);
    }
}
