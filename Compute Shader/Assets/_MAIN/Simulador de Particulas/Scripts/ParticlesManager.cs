using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using RangeAttribute = UnityEngine.RangeAttribute;
using UnityEngine.VFX;

public class ParticlesManager : MonoBehaviour
{
    #region Odin Inspector Buttons
    private bool _odinToggle;

    #region Using CPU Button
    [PropertyOrder(-10)]
    [HideIf("_odinToggle")]
    [Button(ButtonSizes.Gigantic), GUIColor(0, 1, 0)]
    private void UsingCPU()
    {
        this._odinToggle = !this._odinToggle;
        ActivateGPU();
    }
    #endregion

    #region Using GPU Button
    [ShowIf("_odinToggle")]
    [PropertyOrder(-10)]
    [Button(ButtonSizes.Gigantic), GUIColor(1, 0, 0)]
    private void UsingGPU()
    {
        this._odinToggle = !this._odinToggle;
        ActivateCPU();
    }
    #endregion

    #endregion

    #region Variables
    [SerializeField, ReadOnly] private bool _moveWithCPU;

    [PropertySpace(SpaceBefore = 10), SerializeField] const int threadGroupSize = 10;
    [PropertySpace(SpaceAfter = 10)] public ComputeShader compute;

    private float _cellSize = 2.5f;
    private Dictionary<Vector3Int, List<ParticleClass>> _grid = new Dictionary<Vector3Int, List<ParticleClass>>();

    #region Buffers Compute Shader
    private ComputeBuffer _particlesBuffer;
    private ComputeBuffer _sortedParticlesBuffer;
    private ComputeBuffer _startIndicesBuffer;
    #endregion

    #region Particles Config
    [Title("Particles Config")]
    [Space(5)]
    [PropertyOrder(0)] public Transform ParticleParent;
    [PropertyOrder(0)] public ParticleClass ParticlePrefab;

    [Space(15)]
    [PropertyOrder(0)] [SerializeField, ReadOnly] private int _currentParticles;
    [InlineButton("RefreshParticleQuantity")]
    [PropertyOrder(0)][Range(1, 1100)] public int MaxParticles;

    [Space(10)]
    public ParticleData ParticleData;
    private void RefreshParticleQuantity()
    {
        SetParticlesQuantity();
    }
    #endregion

    #region Particles List
    [Title("Particles List")]
    [Space(10)]
    [PropertyOrder(2)] public List<ParticleClass> ParticlesList = new List<ParticleClass>();
    #endregion

    #endregion

    void Start()
    {
        SetParticlesQuantity();
    }
    void FixedUpdate()
    {
        if(_moveWithCPU)
        {
            //MoveWithCPU();
            MoveWithCPUSpatial();
        }
        else
        {
            //MoveWithGPU();
            MoveWithGPUSpatial();
        }
    }

    #region Particles Quantity Related Functions
    // Iguala ParticlesList.Count a MaxParticles
    public void SetParticlesQuantity()
    {
        var difference = CheckDifference();

        // Se as tem menos partículas do que o máximo
        if (ParticlesList.Count < MaxParticles)
        {
            // Instancia e adiciona partículas a lista enquanto i for menor do que a DIFERENÇA entre ParticlesList.Count e MaxParticles
            for (int i = 0; i < difference; i++)
            {
                //Debug.LogWarning("i == " + i);

                var newParticle = Instantiate(ParticlePrefab); // Cria nova instância de partícula
                newParticle.transform.parent = ParticleParent; // Atualiza o pai dela pro ParticleParent
                ParticlesList.Add(newParticle); // Adiciona partícula a ParticlesList
            }
        }
        // Se tem menos partículas do que o máximo
        else if (ParticlesList.Count > MaxParticles)
        {
            // I é igual a quantidade de partículas e vai removendo itens da lista e os destruindo até ser <= MaxParticles
            for (int i = ParticlesList.Count - 1; i >= MaxParticles; i--)
            {
                var p = ParticlesList[i]; // Referência simplificada ao elemento atual da ParticleList
                ParticlesList.Remove(p); // Remove da lista
                Destroy(p.gameObject); // Destrói instância
            }
        }

        // Informa a quantiadade de partículas, após as operações acima
        _currentParticles = ParticlesList.Count;
        //Debug.Log("Particle Quantity = " + ParticlesList.Count);
    }

    private int CheckDifference()
    {
        var difference = MaxParticles - ParticlesList.Count;
        // Debug.Log("MaxParticles = " + MaxParticles + " || ParticlesList.Count = " + ParticlesList.Count + " || Difference = " + difference);
        return difference;
    }
    #endregion

    #region CPU vs GPU Related Functions
    private void ActivateCPU()
    {
        _moveWithCPU = true;
    }

    private void ActivateGPU()
    {
        _moveWithCPU = false;
    }

    #region Move Particles Functions
    private void MoveWithCPU()
    {
        if (ParticlesList != null)
        {
            for (int i = 0; i < ParticlesList.Count; i++)
            {
                ParticlesList[i].numPerceivedFlockmates = 0;
                ParticlesList[i].avgFlockHeading = Vector3.zero;
                ParticlesList[i].centreOfFlockmates = Vector3.zero;
                ParticlesList[i].avgAvoidanceHeading = Vector3.zero;

                for (int j = 0; j < ParticlesList.Count; j++)
                {
                    if (i != j)
                    {
                        ParticleClass neighborParticle = ParticlesList[j];
                        Vector3 distance = neighborParticle.transform.position - ParticlesList[j].transform.position;

                        if (distance.magnitude < ParticlesList[i].perceptionRadius)
                        {
                            ParticlesList[i].numPerceivedFlockmates += 1;
                            ParticlesList[i].avgFlockHeading += neighborParticle.transform.forward;
                            ParticlesList[i].centreOfFlockmates += neighborParticle.transform.position;

                            if (distance.magnitude < ParticlesList[i].avoidanceRadius)
                            {
                                ParticlesList[i].avgAvoidanceHeading -= distance / distance.magnitude;
                            }
                        }
                    }
                }
                ParticlesList[i].UpdateParticle();
            }
        }
    }

    private void MoveWithCPUSpatial()
    {
        _grid.Clear();
        _cellSize = ParticlesList[0].perceptionRadius;

        // 1. Registrar partículas na grade
        foreach (var p in ParticlesList)
        {
            Vector3Int cell = Vector3Int.FloorToInt(p.transform.position / _cellSize);
            if (!_grid.ContainsKey(cell)) _grid[cell] = new List<ParticleClass>();
            _grid[cell].Add(p);
        }

        // 2. Calcular Boids olhando apenas vizinhos
        foreach (var p in ParticlesList)
        {
            p.numPerceivedFlockmates = 0;
            p.avgFlockHeading = Vector3.zero;
            p.centreOfFlockmates = Vector3.zero;
            p.avgAvoidanceHeading = Vector3.zero;

            Vector3Int centerCell = Vector3Int.FloorToInt(p.transform.position / _cellSize);

            // Olhar células vizinhas (3x3x3 = 27 células no máximo)
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        Vector3Int neighborCell = centerCell + new Vector3Int(x, y, z);

                        if (_grid.TryGetValue(neighborCell, out List<ParticleClass> neighbors))
                        {
                            foreach (var neighbor in neighbors)
                            {
                                if (p == neighbor) continue;

                                Vector3 offset = neighbor.transform.position - p.transform.position;
                                float distSq = offset.sqrMagnitude; // sqrMagnitude é mais rápido que magnitude

                                if (distSq < p.perceptionRadius * p.perceptionRadius)
                                {
                                    p.numPerceivedFlockmates++;
                                    p.avgFlockHeading += neighbor.transform.forward;
                                    p.centreOfFlockmates += neighbor.transform.position;

                                    if (distSq < p.avoidanceRadius * p.avoidanceRadius)
                                        p.avgAvoidanceHeading -= offset / Mathf.Sqrt(distSq);
                                }
                            }
                        }
                    }
                }
            }
            p.UpdateParticle();
        }
    }

    private void MoveWithGPU()
    {
        if (ParticlesList != null && ParticlesList.Count > 0)
        {
            int numParticlesList = ParticlesList.Count;
            ParticleData[] ParticleData = new ParticleData[numParticlesList];

            for (int i = 0; i < numParticlesList; i++)
            {
                ParticleData[i].position = ParticlesList[i].transform.position;
                ParticleData[i].direction = ParticlesList[i].transform.forward;
            }

            var ParticleBuffer = new ComputeBuffer(numParticlesList, sizeof(float) * 3 * 5 + sizeof(int));
            ParticleBuffer.SetData(ParticleData);

            compute.SetBuffer(0, "Particles", ParticleBuffer);
            compute.SetInt("numParticles", numParticlesList);
            compute.SetFloat("viewRadius", ParticlesList[0].perceptionRadius);
            compute.SetFloat("avoidRadius", ParticlesList[0].avoidanceRadius);

            int threadGroups = Mathf.CeilToInt((float)numParticlesList / threadGroupSize);
            var kernel = compute.FindKernel("MoveParticle");
            compute.Dispatch(kernel, threadGroups, 1, 1);

            ParticleBuffer.GetData(ParticleData);

            for (int i = 0; i < ParticlesList.Count; i++)
            {
                ParticlesList[i].avgFlockHeading = ParticleData[i].flockHeading;
                ParticlesList[i].centreOfFlockmates = ParticleData[i].flockCentre;
                ParticlesList[i].avgAvoidanceHeading = ParticleData[i].avoidanceHeading;
                ParticlesList[i].numPerceivedFlockmates = ParticleData[i].numFlockmates;

                ParticlesList[i].UpdateParticle();
            }

            ParticleBuffer.Release();
        }
    }

    private void MoveWithGPUSpatial()
    {
        if (ParticlesList == null || ParticlesList.Count == 0) return;

        int num = ParticlesList.Count;
        float cellSize = ParticlesList[0].perceptionRadius;

        ParticleData[] rawData = new ParticleData[num];
        ParticleLookup[] lookups = new ParticleLookup[num];
        int[] startIndices = new int[num];

        for (int i = 0; i < num; i++)
        {
            startIndices[i] = -1;
            rawData[i].position = ParticlesList[i].transform.position;
            rawData[i].direction = ParticlesList[i].transform.forward;

            // Gerar Hash Espacial
            Vector3Int cellCoord = Vector3Int.FloorToInt(rawData[i].position / cellSize);
            uint hash = (uint)((cellCoord.x * 73856093) ^ (cellCoord.y * 19349663) ^ (cellCoord.z * 83492791)) % (uint)num;

            lookups[i] = new ParticleLookup { cellHash = hash, particleIndex = (uint)i };
        }

        // Ordena por hash
        System.Array.Sort(lookups, (a, b) => a.cellHash.CompareTo(b.cellHash));

        ParticleData[] sortedData = new ParticleData[num];
        for (int i = 0; i < num; i++)
        {
            sortedData[i] = rawData[lookups[i].particleIndex];

            // Registra posição inical de cada célula no grid
            uint key = lookups[i].cellHash;
            if (i == 0 || key != lookups[i - 1].cellHash)
            {
                startIndices[key] = i;
            }
        }

        // Configura os buffers
        _particlesBuffer = new ComputeBuffer(num, sizeof(float) * 3 * 5 + sizeof(int));
        _sortedParticlesBuffer = new ComputeBuffer(num, sizeof(float) * 3 * 5 + sizeof(int));
        _startIndicesBuffer = new ComputeBuffer(num, sizeof(int));

        _particlesBuffer.SetData(rawData);
        _sortedParticlesBuffer.SetData(sortedData);
        _startIndicesBuffer.SetData(startIndices);

        int kernel = compute.FindKernel("MoveParticle");
        compute.SetBuffer(kernel, "Particles", _particlesBuffer);
        compute.SetBuffer(kernel, "SortedParticles", _sortedParticlesBuffer);
        compute.SetBuffer(kernel, "StartIndices", _startIndicesBuffer);

        compute.SetInt("numParticles", num);
        compute.SetFloat("viewRadius", cellSize);
        compute.SetFloat("avoidRadius", ParticlesList[0].avoidanceRadius);
        compute.SetFloat("cellSize", cellSize);

        // Chama o kernel
        int threadGroups = Mathf.CeilToInt((float)num / threadGroupSize);
        compute.Dispatch(kernel, threadGroups, 1, 1);

        // Recupera e atualiza os dados
        _particlesBuffer.GetData(rawData);
        for (int i = 0; i < num; i++)
        {
            ParticlesList[i].avgFlockHeading = rawData[i].flockHeading;
            ParticlesList[i].centreOfFlockmates = rawData[i].flockCentre;
            ParticlesList[i].avgAvoidanceHeading = rawData[i].avoidanceHeading;
            ParticlesList[i].numPerceivedFlockmates = rawData[i].numFlockmates;
            ParticlesList[i].UpdateParticle();
        }

        // Limpa os buffers
        _particlesBuffer.Release();
        _sortedParticlesBuffer.Release();
        _startIndicesBuffer.Release();
    }
    #endregion

    #endregion
}



public struct ParticleData
{
    public Vector3 position;
    public Vector3 direction;

    public Vector3 flockHeading;
    public Vector3 flockCentre;
    public Vector3 avoidanceHeading;
    public int numFlockmates;
}

public struct ParticleLookup
{
    public uint cellHash;
    public uint particleIndex;
}