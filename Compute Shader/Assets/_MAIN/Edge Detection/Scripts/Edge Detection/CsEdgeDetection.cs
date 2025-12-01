using UnityEngine;

public class CsEdgeDetection : MonoBehaviour
{
    public ComputeShader computeShader;
    public Sprite inputSprite;
    public Material outputMaterial;

    private RenderTexture renderTexture;

    void Start()
    {
        Texture2D inputTexture = inputSprite.texture;

        // Cria a RenderTexture para saída
        renderTexture = new RenderTexture(inputTexture.width, inputTexture.height, 0);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        // Encontra o kernel CSMain
        int kernel = computeShader.FindKernel("CSMain");

        // Configura compute shader
        computeShader.SetTexture(kernel, "InputTex", inputTexture);
        computeShader.SetTexture(kernel, "Result", renderTexture);

        computeShader.SetInt("width", inputTexture.width);
        computeShader.SetInt("height", inputTexture.height);

        // Executa o kernel
        computeShader.Dispatch(kernel, inputTexture.width / 8, inputTexture.height / 8, 1);

        // Mostra o resultado no material
        outputMaterial.SetTexture("_MainTex", renderTexture);
    }
}
