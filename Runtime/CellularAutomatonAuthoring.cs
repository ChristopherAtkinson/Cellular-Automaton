using UnityEngine;
using UnityEngine.UI;

namespace ChristopherAtkinson.CellularAutomaton
{
    public class CellularAutomatonAuthoring : MonoBehaviour
    {
        [Header("Render Texture Configuration")]
        [SerializeField] private RenderTexture m_RenderTexture;

        [Header("Compute Shader Configuration")]
        [SerializeField] private string m_KernelName = "CSMain";
        [SerializeField] private ComputeShader m_ComputeShader;

        private void OnEnable()
        {
            RenderTexture renderTexture = new RenderTexture(m_RenderTexture);
            renderTexture.enableRandomWrite = true;

            var kernel = m_ComputeShader.FindKernel(m_KernelName);
            m_ComputeShader.SetTexture(kernel, "Result", renderTexture);
            m_ComputeShader.Dispatch(kernel, (renderTexture.width / 32) + 1, (renderTexture.height / 32) + 1, 1);

            if (TryGetComponent(out RawImage rawImage))
                rawImage.texture = renderTexture;
        }

        private void OnDisable()
        {
            m_RenderTexture.Release();
        }
    }
}
