using UnityEngine;

public class Gpu : MonoBehaviour
{
    public Particle[] particles;
    public int particleCount;
    public ComputeShader shader;

    public ComputeBuffer buffer;
    public int kernel;

    void Start()
    {
        particles = new Particle[particleCount];

        for (int i = 0; i < particleCount; i++)
        {
            particles[i].position = Random.insideUnitSphere * 5f;
            particles[i].velocity = Random.insideUnitSphere;
        }

        buffer = new ComputeBuffer(particleCount, sizeof(float) * 6);
        buffer.SetData(particles);

        kernel = shader.FindKernel("CSMain");
        shader.SetBuffer(kernel, "particles", buffer);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        shader.SetFloat("dt", dt);

        var sw = System.Diagnostics.Stopwatch.StartNew();

        shader.Dispatch(kernel, particleCount / 256, 1, 1);

        sw.Stop();

        Debug.Log($"GPU Update: {sw.ElapsedMilliseconds} ms");
    }

    void OnDestroy()
    {
        buffer.Release();
    }
}
