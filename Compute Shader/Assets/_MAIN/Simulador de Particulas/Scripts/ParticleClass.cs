using Sirenix.OdinInspector;
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
    #endregion

    /*
    public void SetSpeed()
    {

    }
    */
}
