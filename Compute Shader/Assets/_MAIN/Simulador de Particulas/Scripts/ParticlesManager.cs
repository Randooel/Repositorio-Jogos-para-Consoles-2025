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
    [SerializeField, ReadOnly] private bool _moveWithCPU = true;

    #region Particles Config
    [Title("Particles Config")]
    [Space(5)]
    [PropertyOrder(0)] public Transform ParticleParent;
    [PropertyOrder(0)] public ParticleClass ParticlePrefab;

    [Space(15)]
    [PropertyOrder(0)] [SerializeField, ReadOnly] private int _currentParticles;
    [InlineButton("RefreshParticleQuantity")]
    [PropertyOrder(0)] [Range(1, 1100)] public int MaxParticles;
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
        foreach (var p in ParticlesList)
        {
            Transform t = p.transform;
            var deslocation = p.Speed * Time.deltaTime;
            t.Translate(new Vector3(deslocation, deslocation, deslocation));

            // Checa colisão contra as paredes
            if (t.position.x < p.minSpace.x || t.position.y > p.minSpace.y || t.position.z  < p.minSpace.z ||
                t.position.x > p.maxSpace.x || t.position.y > p.maxSpace.y || t.position.z  > p.maxSpace.z)
            {
                // Inverte a direção e rotaciona aleatoriamente
                t.rotation = Quaternion.LookRotation(-t.forward);
                t.Rotate(Vector3.up, Random.Range(-30f, 30f));

                // Empurra um pouco para fora da parede para evitar prender
                t.Translate(new Vector3(0, 0, 0.5f));
            }

            /*
            // WIP: Checar colisão com obstáculos estáticos
            foreach (Transform ss in ParticlesList)
            {
                Vector3 dir = t.position - ss.position;
                dir.y = 0;
                float distanceSqr = dir.sqrMagnitude;
                float combinedRadius = staticSphereRadius + r;

                // Usando sqrMagnitude para performance (compara com raio ao quadrado)
                if (distanceSqr < combinedRadius * combinedRadius)
                {
                    // Rotaciona para olhar para longe da esfera estática
                    t.rotation = Quaternion.LookRotation(dir.normalized);

                    // Reposiciona para fora da colisão
                    float overlap = combinedRadius - Mathf.Sqrt(distanceSqr);
                    t.Translate(new Vector3(0, 0, overlap));
                    break;
                }
            }
            */
        }
    }

    private void MoveWithGPU()
    {
        
    }
    #endregion
    #endregion
}
