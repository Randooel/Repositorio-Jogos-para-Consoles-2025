using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using RangeAttribute = UnityEngine.RangeAttribute;

public class ParticlesManager : MonoBehaviour
{
    #region Variables
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


    [Title("Particles List")]
    [Space(10)]
    [PropertyOrder(2)] public List<ParticleClass> ParticlesList = new List<ParticleClass>();
    #endregion

    void Start()
    {
        SetParticlesQuantity();
    }

    // Iguala ParticlesList.Count a MaxParticles
    public void SetParticlesQuantity()
    {
        var difference = CheckDifference();

        // Se as tem menos partículas do que o máximo
        if(ParticlesList.Count < MaxParticles)
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
        else if(ParticlesList.Count > MaxParticles)
        {
            // I é igual a quantidade de partículas e vai removendo itens da lista e os destruindo até ser <= MaxParticles
            for(int i = ParticlesList.Count - 1; i >= MaxParticles; i--)
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
}
