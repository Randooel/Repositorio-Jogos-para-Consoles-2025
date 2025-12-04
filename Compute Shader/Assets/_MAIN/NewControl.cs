using UnityEngine;

public class NewControl : MonoBehaviour
{
    [Header("Compute Shader")]
    public ComputeShader computeShader;
    // public int targetKernel;

    [Header("Texture Related")]
    [Space(10)]
    public Sprite texture;
    public Material targetMaterial;

    [Space(5)]
    public RenderTexture renderTexture;

    void Start()
    {
        // Convertendo o sprite para Texture2D e guardando em uma variável
        Texture2D inputTex = texture.texture;

        renderTexture = new RenderTexture(512, 512, 32); // Como saber a quantidade de canais? Pelo RGBA?
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        // Defino o kernel que vou usar
        var kernel = computeShader.FindKernel("CSControl");
        // Passa a textura para "sourceImage" do Compute Shader
        computeShader.SetTexture(kernel, "sourceImage", inputTex);
        // Atribui a "resultTexture" à variável "Result" do CS
        computeShader.SetTexture(kernel, "Result", renderTexture);

        // Executa o shader
        computeShader.Dispatch(kernel, 512 / 8, 512 / 8, 1); // Por que dividir o tamanho da imagem por 8 e não 32 (1 warp)? E pq 1 no final?

        targetMaterial.mainTexture = renderTexture;
    }

    void Update()
    {
        
    }
}
