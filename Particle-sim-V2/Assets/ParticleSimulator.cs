using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ParticleSimulator : MonoBehaviour 
{
    // Constants:
    public float G = 0.5f; // Gravitational constant
    public float boundaryAroundCentreOfMass = 250f;
    public float simSpeed = 1f;

    public ParticleSpawner spawner;
    private List<Particle> particles;

    private void Start()
    {
        particles = spawner.GetParticles(); // Fetch the particle data from the spawner script
        Time.timeScale = simSpeed;          // run timeScale× faster
        Time.fixedDeltaTime = 0.02f / Time.timeScale;  // keep physics stability
    }

    private void Update()
    {
        // Change any public variables:
        Time.timeScale = simSpeed;

        spawnMoreParticles();
        outsideBoundary();
        updateParticleCollisions();
        updateParticleVelocities();

        // Move particles to the next position
        foreach (Particle particle in particles)
        {
            if (particle.visual == null) continue; // It was destroyed this frame — skip it

            // Update positional data
            particle.position += particle.velocity * Time.deltaTime;

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
            /*if (particleA.position.magnitude >= universeRadius)
            {
                Vector3 n = particleA.position.normalized;      // Outward normal at the hit point
                particleA.velocity = Vector3.Reflect(particleA.velocity, n);
            }
            */
        }
    }

    private void outsideBoundary()
    {
        Vector3 com = findCentreOfMass();

        // Find all particles outside the boundary
        List<Particle> toRemove = new List<Particle>();
        foreach (Particle particle in particles)
        {
            Vector3 offset = particle.position - com;
            float dist = offset.magnitude;

            if (dist > boundaryAroundCentreOfMass)
            {
                toRemove.Add(particle);
            }
        }

        // Remove all flagged particles
        foreach (Particle particle in toRemove)
        {
            Destroy(particle.visual);
            particles.Remove(particle);
        }
    }

    private void updateParticleCollisions()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            for (int j = i + 1; j < particles.Count; j++)
            {
                Particle particleA = particles[i];
                Particle particleB = particles[j];

                if ((particleA.position - particleB.position).magnitude < (particleA.radius + particleB.radius))
                {
                    combineParticles(particleA, particleB);
                }
            }
        }
    }

    private void combineParticles(Particle particleA, Particle particleB)
    {
        float newMass = particleA.mass + particleB.mass;
        Vector3 newVelocity = (particleA.mass * particleA.velocity + particleB.mass * particleB.velocity) / (particleA.mass + particleB.mass);
        Vector3 newPosition = (particleA.radius > particleB.radius) ? particleA.position : particleB.position; // The new position will be that of the bigger particle
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

    private void spawnMoreParticles()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            particles.AddRange(spawner.SpawnNewParticles(findCentreOfMass()));
        }
    }

    public Vector3 findCentreOfMass()
    {
        Vector3 massPositionTotal = Vector3.zero;
        float massTotal = 0f;

        foreach (Particle particle in particles)
        {
            massPositionTotal += particle.mass * particle.position;
            massTotal += particle.mass;
        }

        return (massPositionTotal / massTotal);
    }
}
