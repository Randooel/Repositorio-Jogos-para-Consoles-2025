using Sirenix.OdinInspector;
using System.Collections;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class ParticleClass : MonoBehaviour
{
    #region Variables
    [Title("Visual")]
    public Sprite Visual;

    [Title("Transform Related")]
    public Vector3 Position;
    [Range(1, 300)] public float Speed = 10f;
    public float Acceleration;

    [Space(20)]
    public Vector3 minSpace = new Vector3(-1000, -1000, -1000);
    public Vector3 maxSpace = new Vector3(1000, 1000, 1000);
    #endregion


    private void Start()
    {
        
        GetComponent<SpriteRenderer>().sprite = Visual;
    }
}
