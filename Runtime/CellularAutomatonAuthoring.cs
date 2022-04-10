using UnityEngine;
using UnityEngine.Events;

namespace ChristopherAtkinson.CellularAutomaton
{
    public class CellularAutomatonAuthoring : MonoBehaviour
    {
        [Header("Render Texture Configuration")]
        [SerializeField] private RenderTexture m_RenderTexture;

        [Header("Compute Shader Configuration")]
        [SerializeField] private ComputeShader m_ComputeShader;
        [SerializeField] private string m_KernelName;

        [Header("Unity Event Configuration")]
        [SerializeField] private UnityEvent<RenderTexture> OnAfterRenderTextureEnable;

        private void OnEnable()
        {
            RenderTexture renderTexture = new RenderTexture(m_RenderTexture);
            renderTexture.enableRandomWrite = true;

            var kernel = m_ComputeShader.FindKernel(m_KernelName);
            m_ComputeShader.SetTexture(kernel, "Result", renderTexture);
            m_ComputeShader.Dispatch(kernel, (renderTexture.width / 32) + 1, (renderTexture.height / 32) + 1, 1);

            OnAfterRenderTextureEnable?.Invoke(renderTexture);
        }

        private void OnDisable()
        {
            m_RenderTexture.Release();
        }
    }
}
