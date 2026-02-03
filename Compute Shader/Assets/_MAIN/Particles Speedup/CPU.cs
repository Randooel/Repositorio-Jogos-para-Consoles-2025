using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;

public class CPU : MonoBehaviour
{
    public int particlesNumber;
    public GameObject particlePrefab;
    public List<GameObject> particles = new List<GameObject>();

    [Header("Medição de Latência")]
    private Stopwatch stopwatch = new Stopwatch();
    private double cpuTimeMs;
    public TextMeshProUGUI textMesh;

    void Start()
    {
        CreateParticles();
    }

    void Update()
    {
        MoveParticles();
    }

    void CreateParticles()
    {
        // Cria as partículas em posições aleatórias
        for(int i = 0; i < particlesNumber; i++)
        {
            Vector3 randomPos = Random.onUnitSphere * 10;
            GameObject newParticle = Instantiate(particlePrefab, randomPos, Quaternion.identity);

            particles.Add(newParticle);
        }
    }

    void MoveParticles()
    {
        stopwatch.Restart();

        for(int i = 0; i < particles.Count; i++)
        {
            Vector3 randomPos = Random.onUnitSphere * 10;

            particles[i].transform.position = randomPos;
        }

        stopwatch.Stop();
        cpuTimeMs = stopwatch.ElapsedMilliseconds;
        textMesh.text = "<b>Latência CPU</b>: " + cpuTimeMs;
    }
}
