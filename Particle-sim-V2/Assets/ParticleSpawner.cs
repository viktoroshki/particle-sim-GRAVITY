using NUnit.Framework;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;

public class ParticleSpawner : MonoBehaviour
{
    public GameObject particleObj;
    public int spawnNumber;
    public float spawnRadius;
    public float minSpeed;
    public float maxSpeed;
    public float minMass = 1;
    public float maxMass = 1;

    List<Particle> particles = new List<Particle>();
    void Start()
    {
        //Destroy(particleObj); // We can get rid of the inital sphere we copy off of

        SpawnParticles(new Vector3(0,0,0));
    }

    void SpawnParticles(Vector3 centreOfMass)
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            // Create random starting factors for the paricles
            Vector3 randomPos = Random.insideUnitSphere * spawnRadius + centreOfMass;
            Vector3 randomVelocity = Random.onUnitSphere * Random.Range(minSpeed, maxSpeed);
            float randomMass = Random.Range(minMass, maxMass);
            float radius = Mathf.Pow(3f * randomMass / (4f * Mathf.PI), 1f / 3f); // radius dependant on the mass where density = 1 arbitry unit

            GameObject visual = Instantiate(particleObj, randomPos, Quaternion.identity);
            visual.transform.localScale = Vector3.one * radius * 2f; // scale the visual depndant on the radius

            particles.Add(new Particle(randomPos, randomVelocity, randomMass, radius, visual));
        }
    }

    public List<Particle> SpawnNewParticles(Vector3 centreOfMass)
    {
        particles = new List<Particle>();
        SpawnParticles(centreOfMass);
        return particles;
    }

    public List<Particle> GetParticles()
    {
        return particles;
    }
}

