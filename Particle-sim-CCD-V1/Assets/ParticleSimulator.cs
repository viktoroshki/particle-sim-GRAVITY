using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ParticleSimulator : MonoBehaviour 
{
    // Constants:
    public float G = 0.5f; // Gravitational constant
    public float universeRadius = 250f;

    public ParticleSpawner spawner;
    private List<Particle> particles;

    private void Start()
    {
        particles = spawner.GetParticles(); // Fetch the particle data from the spawner script
    }

    private void Update()
    {
        updateParticleCollisions();
        updateParticleVelocities();

        // Move particles to the next position
        foreach (Particle particle in particles)
        {
            if (particle.visual == null) continue; // It was destroyed this frame — skip it

            // Update positional data
            particle.position += particle.velocity * Time.deltaTime;

            // Move it to the surface of the universe if it tries to jump outside of it
            if (particle.position.magnitude > universeRadius)
            {
                particle.position = particle.position.normalized * universeRadius;
            }

            //Update visual data
            particle.visual.transform.position = particle.position;
        }
    }

    private void updateParticleVelocities()
    {
        foreach (Particle particleA in particles)
        {
            Vector3 totalVelocity = particleA.velocity; // Still has the inertia of its previous velocity
            foreach (Particle particleB in particles)
            {
                if (particleA == particleB) { continue; } // Stop the particle comparing to itself
                else
                {
                    Vector3 vDirDueToB = (particleB.position - particleA.position).normalized;
                    float distanceAB = (particleB.position - particleA.position).magnitude;
                    float forceDueToB = ((G * particleA.mass * particleB.mass) / (distanceAB * distanceAB));

                    // F = ma  =>  a = F/m
                    float accelerationDueToB = forceDueToB / particleA.mass;

                    totalVelocity += vDirDueToB* accelerationDueToB * Time.deltaTime;
                }
            }
            particleA.velocity = totalVelocity;

            // Flip the direction the particle is going if it goes outside the "universe"
            if (particleA.position.magnitude >= universeRadius)
            {
                Vector3 n = particleA.position.normalized;      // Outward normal at the hit point
                particleA.velocity = Vector3.Reflect(particleA.velocity, n);
            }
        }
    }

    private void updateParticleCollisions()
    {
        List<(Particle, Particle, float)> toCombine = new List<(Particle, Particle, float)>();

        // First we need to see if any particles will collide with each other
        for (int i = 0; i < particles.Count; i++)
        {
            for (int j = i + 1; j < particles.Count; j++)
            {
                Particle particleA = particles[i];
                Particle particleB = particles[j];

                if (particleA == particleB) { continue; } // Stop the particle comparing to itself
                else
                {
                    // Find their velocities and positions relative to one another
                    Vector3 rVel = particleA.velocity - particleB.velocity;
                    Vector3 rPos = particleA.position - particleB.position;
                    float radiusSum = particleA.radius + particleB.radius;
                    // Solve for time t when they collide
                    // Where |p + v·t| = r1 + r2
                    // Thank you ChatGPT for solving it for me
                    // coefficients for quadratic equation:
                    float a = Vector3.Dot(rVel, rVel);
                    float b = 2 * Vector3.Dot(rPos, rVel);
                    float c = Vector3.Dot(rPos, rPos) - radiusSum * radiusSum;

                    // Solve for the discriminant (the sqrt part of the thing)
                    // Discriminant
                    float discriminant = b * b - 4 * a * c;

                    // Solve for the two values of t
                    if (discriminant >= 0)
                    {
                        float sqrtDisc = Mathf.Sqrt(discriminant);
                        float t1 = (-b - sqrtDisc) / (2 * a);
                        float t2 = (-b + sqrtDisc) / (2 * a);

                        if (0 < t1 && t1 < Time.deltaTime) // If the particles will collide between the next 2 frames
                        {
                            toCombine.Add((particleA, particleB, t1));
                            break; // Break out because we don't want A colliding with mroe than one thing or bad stuff happens
                        }
                        else if (0 < t2 && t2 < Time.deltaTime)
                        {
                            toCombine.Add((particleA, particleB, t2));
                            break; // Break out because we don't want A colliding with mroe than one thing or bad stuff happens
                        }
                    }

                }
            }
        }

        // Process the list which now needs to combine the particles
        foreach (var (particleA, particleB, t) in toCombine)
        {
            combineParticles(particleA, particleB, t);
        }
    }

    private void combineParticles(Particle particleA, Particle particleB, float t)
    {
        Vector3 futurePosA = particleA.position + particleA.velocity * t;
        Vector3 futurePosB = particleB.position + particleB.velocity * t;

        float newMass = particleA.mass + particleB.mass;
        Vector3 newVelocity = (particleA.mass * particleA.velocity + particleB.mass * particleB.velocity) / (particleA.mass + particleB.mass);
        Vector3 newPosition = (futurePosA + futurePosB) / 2f;
        GameObject newVisual = Instantiate(particleA.visual, newPosition, Quaternion.identity);

        // Scale the new object according to its mass
        float newRadius = Mathf.Pow(3f * newMass / (4f * Mathf.PI), 1f / 3f);
        newVisual.transform.localScale = Vector3.one * newRadius * 2f;

        particles.Add(new Particle(newPosition, newVelocity, newMass, newRadius, newVisual));

        // Move paticles to the position where they will then be combined
        particleA.visual.transform.position = newPosition;
        particleB.visual.transform.position = newPosition;
        // Take away the visuals of the previous particles
        Destroy(particleA.visual);
        Destroy(particleB.visual);
        // Remove old particles
        particles.Remove(particleA);
        particles.Remove(particleB);
    }
}
