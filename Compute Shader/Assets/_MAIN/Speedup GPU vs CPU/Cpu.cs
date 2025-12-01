using UnityEngine;

public class Cpu : MonoBehaviour
{
    public int particleCount = 200000;
    public Particle[] particles;

    void Start()
    {
        particles = new Particle[particleCount];
        for (int i = 0; i < particleCount; i++)
        {
            particles[i].position = Random.insideUnitSphere * 5f;
            particles[i].velocity = Random.insideUnitSphere * 1f;
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;

        var sw = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < particleCount; i++)
        {
            particles[i].position += particles[i].velocity * dt;
        }

        sw.Stop();

        Debug.Log($"CPU Update: {sw.ElapsedMilliseconds} ms");
    }
}
