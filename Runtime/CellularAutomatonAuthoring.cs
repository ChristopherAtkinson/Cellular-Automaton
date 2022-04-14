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

        [Header("Simulation Rate Configuration")]
        [SerializeField] private float m_RepeatRate;
        private System.Collections.IEnumerator m_Coroutine;

        private void OnEnable()
        {
            RenderTexture renderTexture = new RenderTexture(m_RenderTexture);
            renderTexture.enableRandomWrite = true;

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

                yield return new WaitForSeconds(m_RepeatRate);
            }
        }

        private void OnDisable()
        {
            StopCoroutine(m_Coroutine);
            m_RenderTexture.Release();
        }
    }
}
