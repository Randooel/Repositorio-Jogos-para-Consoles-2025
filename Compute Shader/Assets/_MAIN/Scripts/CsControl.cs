using UnityEngine;

public class CsControl : MonoBehaviour
{
    public ComputeShader computeShader;
    public Material material;
    public RenderTexture renderTexture;

    void Start()
    {
        // Cria uma nova render texture e define width, height e depth
        renderTexture = new RenderTexture(512, 512, 0);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        // Atribui a textura ao shader.
        computeShader.SetTexture(0, "Result", renderTexture); // Todo kernel tem um index. Em casos de mais de um index, dá pra usar o método FindKernel para achar o kernel pelo nome

        material.SetTexture("_MainTex", renderTexture);

        // Usa o método Dispatch para indicar qual kernel usar para desenhar a textura e quantos grupos de threads serão usadas (dimensões da textura / tamanho de threads de cada grupo)
        computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);

        Debug.Log("Dispatch executado");
    }

    void Update()
    {
        
    }
}
