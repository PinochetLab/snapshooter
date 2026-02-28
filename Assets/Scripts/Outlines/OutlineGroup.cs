using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Linq;

namespace Outlines
{
    [RequireComponent(typeof(MeshRenderer))]
    public class OutlineGroup : MonoBehaviour
    {
        [SerializeField] private List<MeshRenderer> targetRenderers = new List<MeshRenderer>();
        [SerializeField] private float outlineThickness = 0.02f;
        [SerializeField] private Color outlineColor = Color.white;
        [SerializeField] private Material outlineMaterial;

        private Material currentMaterial;
        private CommandBuffer commandBuffer;
        private Camera mainCamera;
        private List<Material> originalMaterials = new List<Material>();

        private void Start()
        {
            InitializeOutline();
        }

        private void InitializeOutline()
        {
            if (outlineMaterial == null)
            {
                // Создаем материал, если не назначен
                Shader shader = Shader.Find("Custom/GroupOutline");
                if (shader != null)
                {
                    currentMaterial = new Material(shader);
                    currentMaterial.SetColor("_OutlineColor", outlineColor);
                    currentMaterial.SetFloat("_OutlineThickness", outlineThickness);
                }
            }
            else
            {
                currentMaterial = outlineMaterial;
            }

            mainCamera = Camera.main;
            SetupCommandBuffer();
        }

        private void SetupCommandBuffer()
        {
            if (targetRenderers.Count == 0) return;

            // Создаем командный буфер
            commandBuffer = new CommandBuffer();
            commandBuffer.name = "GroupOutline";

            // Получаем все mesh фильтры из группы
            var meshFilters = targetRenderers
                .Select(r => r.GetComponent<MeshFilter>())
                .Where(f => f != null && f.sharedMesh != null)
                .ToList();

            // Создаем временный буфер для глубины группы
            int groupDepthTextureID = Shader.PropertyToID("_GroupDepthTexture");
            commandBuffer.GetTemporaryRT(groupDepthTextureID, -1, -1, 24, FilterMode.Point, RenderTextureFormat.Depth);

            // Очищаем буфер
            commandBuffer.SetRenderTarget(groupDepthTextureID);
            commandBuffer.ClearRenderTarget(true, true, Color.clear);

            // Рендерим все меши группы в буфер глубины
            foreach (var meshFilter in meshFilters)
            {
                commandBuffer.DrawMesh(meshFilter.sharedMesh,
                    meshFilter.transform.localToWorldMatrix,
                    currentMaterial, 0, 0); // Используем первый проход шейдера
            }

            // Рендерим outline
            commandBuffer.SetGlobalFloat("_OutlineThickness", outlineThickness);
            commandBuffer.SetGlobalColor("_OutlineColor", outlineColor);

            foreach (var meshFilter in meshFilters)
            {
                commandBuffer.DrawMesh(meshFilter.sharedMesh,
                    meshFilter.transform.localToWorldMatrix,
                    currentMaterial, 0, 1); // Используем второй проход шейдера
            }

            // Добавляем буфер к камере
            mainCamera.AddCommandBuffer(CameraEvent.AfterDepthTexture, commandBuffer);
        }

        // Метод для обновления толщины
        public void SetOutlineThickness(float thickness)
        {
            outlineThickness = thickness;
            if (currentMaterial != null)
            {
                currentMaterial.SetFloat("_OutlineThickness", thickness);
            }
        }

        // Метод для добавления renderer'а в группу
        public void AddRenderer(MeshRenderer renderer)
        {
            if (!targetRenderers.Contains(renderer))
            {
                targetRenderers.Add(renderer);
                UpdateCommandBuffer();
            }
        }

        private void UpdateCommandBuffer()
        {
            if (commandBuffer != null)
            {
                mainCamera.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, commandBuffer);
            }

            SetupCommandBuffer();
        }

        private void OnDestroy()
        {
            if (commandBuffer != null && mainCamera != null)
            {
                mainCamera.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, commandBuffer);
            }
        }
    }
}