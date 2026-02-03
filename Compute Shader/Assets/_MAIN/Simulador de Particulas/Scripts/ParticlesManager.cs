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
            MoveWithCPU();
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

        MoveWithGPU();
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

    private void MoveWithGPU()
    {
        if (ParticlesList != null)
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

            compute.SetBuffer(0, "ParticlesList", ParticleBuffer);
            compute.SetInt("numParticlesList", numParticlesList);
            compute.SetFloat("viewRadius", ParticlesList[0].perceptionRadius);
            compute.SetFloat("avoidRadius", ParticlesList[0].avoidanceRadius);

            int threadGroups = Mathf.CeilToInt((float)ParticlesList.Count / threadGroupSize);

            compute.Dispatch(0, threadGroups, 1, 1);

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