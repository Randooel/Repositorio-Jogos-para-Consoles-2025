using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class ParticleClass : MonoBehaviour
{
    #region Variables
    [Title("Visual")]
    public Sprite Visual;

    [Title("Transform Related")]
    public Vector3 Position;
    [Range(1, 300)] public float Speed = 1;
    public float Acceleration;
    [ReadOnly] public Vector3 Direction;
    public float Delay = 30f;
    #endregion


    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Visual;
    }

    public void SetDirection()
    {
        // Gera uma direção aleatória
        float magnitude = 1f;
        Direction = new Vector3(Random.Range(-magnitude, magnitude), Random.Range(-magnitude, magnitude), 0).normalized;

        StartCoroutine(WaitToSetDirection());
    }

    #region COROUTINES
    private IEnumerator WaitToSetDirection()
    {
        yield return new WaitForSeconds(Delay);
        SetDirection();
    }
    #endregion
}
