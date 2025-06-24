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
    public float startingMass = 1;

    List<Particle> particles;
    void Start()
    {
        Destroy(particleObj); // We can get rid of the inital sphere we copy off of

        SpawnParticles();
    }

    public void SpawnParticles()
    {
        List<Particle> particles = new List<Particle>();
        for (int i = 0; i < spawnNumber; i++)
        {
            // Create random starting factors for the paricles
            Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
            Vector3 randomVelocity = Random.onUnitSphere * Random.Range(minSpeed, maxSpeed);

            GameObject visual = Instantiate(particleObj, randomPos, Quaternion.identity);

            particles.Add(new Particle(randomPos, randomVelocity, startingMass, 1, visual));
        }
    }

    public List<Particle> GetParticles()
    {
        return particles;
    }
}

