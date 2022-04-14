using UnityEngine;
using UnityEngine.Events;

namespace ChristopherAtkinson.CellularAutomaton
{
    public class CellularAutomatonAuthoring : MonoBehaviour
    {
        [Header("Source Texture Configuration")]
        [SerializeField] private Texture2D m_Texture2D;

        [Header("Compute Shader Configuration")]
        [SerializeField] private ComputeShader m_ComputeShader;
        [SerializeField] private string m_KernelName;

        [Header("Unity Event Configuration")]
        [SerializeField] private UnityEvent<RenderTexture> OnAfterRenderTextureEnable;

        [Header("Simulation Rate Configuration")]
        [SerializeField, Range(0, 240)] private float m_SimulationRate;
        private System.Collections.IEnumerator m_Coroutine;

        private void OnEnable()
        {
            RenderTexture renderTexture = new RenderTexture(m_Texture2D.width, m_Texture2D.height, 0);
            renderTexture.filterMode = m_Texture2D.filterMode;
            renderTexture.enableRandomWrite = true;

            Graphics.Blit(m_Texture2D, renderTexture);

            OnAfterRenderTextureEnable?.Invoke(renderTexture);

            m_Coroutine = DispatchComputeShader(renderTexture);
            StartCoroutine(m_Coroutine);
        }

        private System.Collections.IEnumerator DispatchComputeShader(RenderTexture renderTexture)
        {
            while (true)
            {
                var kernel = m_ComputeShader.FindKernel(m_KernelName);
                m_ComputeShader.SetTexture(kernel, "Result", renderTexture);
                m_ComputeShader.Dispatch(kernel, (renderTexture.width / 8) + 1, (renderTexture.height / 8) + 1, 1);

                yield return new WaitForSeconds(1.0f / m_SimulationRate);
            }
        }

        private void OnDisable()
        {
            StopCoroutine(m_Coroutine);
        }
    }
}
