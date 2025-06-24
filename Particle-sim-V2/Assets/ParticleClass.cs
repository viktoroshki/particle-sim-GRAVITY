using UnityEngine;

public class Particle // Class to hold the data and calulate velocity of a particle
{
    public Vector3 position;
    public Vector3 velocity; // In terms of direction and magnitude
    public float mass;
    public float radius;
    public GameObject visual; // This is the actual sphere that we see

    public Particle(Vector3 pos, Vector3 startVelocity, float m, float r, GameObject visualObj)
    {
        position = pos;
        velocity = startVelocity;
        mass = m;
        radius = r;
        visual = visualObj;

        // Scale the new object according to its mass
        float newRadius = Mathf.Pow(3f * mass / (4f * Mathf.PI), 1f / 3f);
        visual.transform.localScale = Vector3.one * newRadius * 2f;

    }
}
