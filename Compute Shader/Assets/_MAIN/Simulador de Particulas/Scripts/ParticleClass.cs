using DG.Tweening;
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
    [Range(1, 300)] public float Speed = 100f;
    public float Acceleration;
    #endregion


    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Visual;
    }

    public void RandomMove()
    {
        var magnitude = 1000000000f;
        Vector3 randomDirection = new Vector3(Random.Range(-magnitude, magnitude), Random.Range(-magnitude, magnitude), 0).normalized;

        this.transform.DOLocalMove(randomDirection, this.Speed).SetSpeedBased().OnComplete(() =>
        {
            RandomMove();
        });
    }
}
