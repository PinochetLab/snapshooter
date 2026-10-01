using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Wires;

namespace Electricity.Wires
{
    public class Wire : MonoBehaviour
    {
        private static readonly int StartClip = Shader.PropertyToID("_StartClip");
        private static readonly int EndClip = Shader.PropertyToID("_EndClip");
        private const float CurrentSpeed = 2f;
        
        [SerializeField] private float wireRadius = 0.07f;
        [SerializeField] private float glassWidth = 0.01f;
        [SerializeField] private float roundRadius = 0.15f;
        [SerializeField] private float currentWidth = 0.02f;
        [SerializeField] private List<Vector3> smoothedPath = new ();
        [SerializeField] private float wireLength;
        [SerializeField] private MeshFilter outerMeshFilter;
        [SerializeField] private MeshFilter innerMeshFilter;
        [SerializeField] private List<LineRenderer> lineRenderers;
        [SerializeField] private Material currentMaterial;
        
        [Range(0, 1)]
        [SerializeField] private float fillRatio = 0.75f;
        
        [Header("Data")]
        [SerializeField] private MonoBehaviour sourceMb; 
        [SerializeField] private MonoBehaviour loadMb; 
        
        private List<MaterialPropertyBlock> _propertyBlocks = new ();
        private List<Current> _currents = new ();
        private bool _startOn;
        private ISource _source;
        private ILoad _load;

        private void InitLoadSource()
        {
            _source = sourceMb as ISource;
            _load = loadMb as ILoad;
            _source.Wire = this;
        }

        [ContextMenu("Generate")]
        private void Generate()
        {
            var positions = GetComponentsInChildren<WirePoint>().Select(wp => wp.GetPosition(wireRadius)).ToList();
            GeneratePath(positions);
            GenerateOuterWire();
            GenerateInnerWire();
            GenerateCurrentMaterial();
            GenerateCurrent();
        }

        private void Awake()
        {
            InitLoadSource();
            GenerateCurrentMaterial();
        }

        public void TurnOn()
        {
            _startOn = true;
            _currents.Insert(0, new Current());
        }
        
        public void TurnOff()
        {
            _startOn = false;
        }

        private void Update()
        {
            var deltaCurrent = CurrentSpeed * Time.deltaTime;

            if (_currents.Count > 0 && _startOn)
            {
                var current = _currents[0];
                if (!current.EndOn)
                {
                    current.End += deltaCurrent;
                    if (current.End >= wireLength)
                    {
                        current.End = wireLength;
                        current.EndOn = true;
                        _load.PowerUp();
                    }
                }
            }
            
            var currentsToRemove = new List<Current>();

            var startIdx = _startOn ? 1 : 0;
            
            for (var i = startIdx; i < _currents.Count; i++)
            {
                var current = _currents[i];

                if (!current.EndOn)
                {
                    current.End += deltaCurrent;
                    if (current.End >= wireLength)
                    {
                        current.End = wireLength;
                        current.EndOn = true;
                        _load.PowerUp();
                    }
                }
                
                current.Start += deltaCurrent;

                if (current.Start >= wireLength)
                {
                    currentsToRemove.Add(current);
                    _load.PowerDown();
                }
            }

            foreach (var current in currentsToRemove)
            {
                _currents.Remove(current);
            }

            _currents = _currents.Take(lineRenderers.Count).ToList();

            for (var i = 0; i < _currents.Count; i++)
            {
                lineRenderers[i].enabled = true;
                SetCurrent(i,  _currents[i].Start, _currents[i].End);
            }

            for (var i = _currents.Count; i < lineRenderers.Count; i++)
            {
                lineRenderers[i].enabled = false;
            }
        }

        private void SetCurrent(int idx, float start, float end)
        {
            _propertyBlocks[idx].SetFloat(StartClip, start);
            _propertyBlocks[idx].SetFloat(EndClip, end);
            lineRenderers[idx].SetPropertyBlock(_propertyBlocks[idx]);
        }

        private void GeneratePath(List<Vector3> points)
        {
            smoothedPath = SmoothPath(points, roundRadius);
            wireLength = 0f;
            for (var i = 0; i < smoothedPath.Count - 1; i++)
            {
                wireLength += Vector3.Distance(smoothedPath[i], smoothedPath[i + 1]);
            }
        }

        private void GenerateCurrentMaterial()
        {
            foreach (var lineRenderer in lineRenderers)
            {
                var propertyBlock = new MaterialPropertyBlock();
                lineRenderer.sharedMaterial = Instantiate(new Material(currentMaterial));
                _propertyBlocks.Add(propertyBlock);
            }
        }

        private void GenerateOuterWire()
        {
            var mesh = GenerateWireMesh(false);
            outerMeshFilter.sharedMesh = mesh;
        }
        
        private void GenerateInnerWire()
        {
            var mesh = GenerateWireMesh(true);
            innerMeshFilter.sharedMesh = mesh;
        }

        private void GenerateCurrent()
        {
            foreach (var lineRenderer in lineRenderers)
            {
                lineRenderer.SetPositions(smoothedPath.ToArray());
                lineRenderer.widthMultiplier = 1f;
                lineRenderer.startWidth = currentWidth;
                lineRenderer.endWidth = currentWidth;
            }
        }

        private Mesh GenerateWireMesh(bool innerWire)
        {
            // Вычисляем вращения для каждого сегмента
            List<Vector3> normals = CalculateNormals(smoothedPath);
            
            // Вычисляем накопленные длины для UV
            List<float> lengths = CalculatePathLengths(smoothedPath);
            float totalLength = lengths[lengths.Count - 1];

            var wireRadius = innerWire ? this.wireRadius - glassWidth : this.wireRadius;
            
            // Длина окружности сечения
            float circumference = 2 * Mathf.PI * wireRadius;

            // Создаем меш
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            int segments = 24; // Количество сегментов окружности
            float angleStep = 360f / segments;

            // Для каждого сегмента пути создаем кольцо вершин
            for (int i = 0; i < smoothedPath.Count; i++)
            {
                Vector3 currentPoint = smoothedPath[i];
                Vector3 forward, up, right;

                // Определяем направление
                if (i == 0)
                    forward = (smoothedPath[1] - smoothedPath[0]).normalized;
                else if (i == smoothedPath.Count - 1)
                    forward = (smoothedPath[i] - smoothedPath[i - 1]).normalized;
                else
                    forward = (smoothedPath[i + 1] - smoothedPath[i - 1]).normalized;

                // Используем вычисленную нормаль
                up = normals[i];
                
                // Если up и forward коллинеарны, используем альтернативный up
                if (Mathf.Abs(Vector3.Dot(up, forward)) > 0.999f)
                {
                    up = Vector3.right;
                }
                
                right = Vector3.Cross(forward, up).normalized;
                up = Vector3.Cross(right, forward).normalized;

                // Вычисляем V координату (вдоль пути) на основе реальной длины с учетом tiling
                float v = lengths[i];

                // Создаем вершины окружности
                for (int s = 0; s < segments; s++)
                {
                    float angle = s * angleStep * Mathf.Deg2Rad;
                    Vector3 offset = (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * wireRadius;
                    vertices.Add(currentPoint + offset);
                    
                    // U координата (вокруг окружности) - используем реальную длину дуги
                    float arcLength = angle * wireRadius;
                    float u = arcLength / circumference;
                    
                    uvs.Add(new Vector2(u, v));
                }
            }

            // Создаем треугольники
            for (int i = 0; i < smoothedPath.Count - 1; i++)
            {
                for (int s = 0; s < segments; s++)
                {
                    int current = i * segments + s;
                    int next = i * segments + (s + 1) % segments;
                    int currentNext = (i + 1) * segments + s;
                    int nextNext = (i + 1) * segments + (s + 1) % segments;

                    triangles.Add(current);
                    if (innerWire)
                    {
                        triangles.Add(currentNext);
                        triangles.Add(next);
                    }
                    else
                    {
                        triangles.Add(next);
                        triangles.Add(currentNext);
                    }
                    triangles.Add(next);
                    if (innerWire)
                    {
                        triangles.Add(currentNext);
                        triangles.Add(nextNext);
                    }
                    else
                    {
                        triangles.Add(nextNext);
                        triangles.Add(currentNext);
                    }
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // Применяем меш
            return mesh;
        }

        private List<float> CalculatePathLengths(List<Vector3> path)
        {
            List<float> lengths = new List<float>();
            if (path.Count == 0) return lengths;
            
            float currentLength = 0;
            lengths.Add(0);
            
            for (int i = 1; i < path.Count; i++)
            {
                currentLength += Vector3.Distance(path[i - 1], path[i]);
                lengths.Add(currentLength);
            }
            
            return lengths;
        }

        private List<Vector3> CalculateNormals(List<Vector3> path)
        {
            List<Vector3> normals = new List<Vector3>();
            
            if (path.Count == 0) return normals;
            
            // Начальная нормаль
            Vector3 firstForward = path.Count > 1 ? (path[1] - path[0]).normalized : Vector3.forward;
            Vector3 initialNormal = Vector3.up;
            
            if (Mathf.Abs(Vector3.Dot(firstForward, initialNormal)) > 0.999f)
                initialNormal = Vector3.right;
            
            normals.Add(initialNormal);
            
            // Для каждой следующей точки вычисляем нормаль, минимизируя скручивание
            for (int i = 1; i < path.Count; i++)
            {
                Vector3 prevPoint = path[i - 1];
                Vector3 currPoint = path[i];
                Vector3 prevForward = (i > 1) ? (path[i - 1] - path[i - 2]).normalized : firstForward;
                Vector3 currForward = (currPoint - prevPoint).normalized;
                
                Vector3 prevNormal = normals[i - 1];
                
                // Проецируем предыдущую нормаль на плоскость, перпендикулярную текущему направлению
                Vector3 projectedNormal = (prevNormal - Vector3.Dot(prevNormal, currForward) * currForward).normalized;
                
                // Если проекция слишком маленькая, создаем новую нормаль
                if (projectedNormal.magnitude < 0.001f)
                {
                    Vector3 alternative = Vector3.up;
                    if (Mathf.Abs(Vector3.Dot(currForward, alternative)) > 0.999f)
                        alternative = Vector3.right;
                    
                    projectedNormal = (alternative - Vector3.Dot(alternative, currForward) * currForward).normalized;
                }
                
                normals.Add(projectedNormal);
            }
            
            return normals;
        }

        private List<Vector3> SmoothPath(List<Vector3> points, float radius)
        {
            if (points.Count < 3 || radius <= 0) return points;

            List<Vector3> smoothed = new List<Vector3>();

            for (int i = 0; i < points.Count; i++)
            {
                if (i == 0 || i == points.Count - 1)
                {
                    smoothed.Add(points[i]);
                    continue;
                }

                Vector3 prev = points[i - 1];
                Vector3 curr = points[i];
                Vector3 next = points[i + 1];

                Vector3 dir1 = (curr - prev).normalized;
                Vector3 dir2 = (next - curr).normalized;
                float angle = Vector3.Angle(dir1, dir2);

                // Если угол слишком маленький, не делаем закругление
                if (angle < 5f || angle > 175f)
                {
                    smoothed.Add(curr);
                    continue;
                }

                // Вычисляем точки закругления
                float halfAngle = angle * 0.5f * Mathf.Deg2Rad;
                float maxRadius = Vector3.Distance(prev, curr) * 0.5f;
                maxRadius = Mathf.Min(maxRadius, Vector3.Distance(curr, next) * 0.5f);
                float actualRadius = Mathf.Min(radius, maxRadius);

                float offset = actualRadius / Mathf.Tan(halfAngle);

                Vector3 point1 = curr - dir1 * offset;
                Vector3 point2 = curr + dir2 * offset;

                // Добавляем точку до закругления
                smoothed.Add(point1);

                // Генерируем дугу закругления
                Vector3 center = GetArcCenter(point1, point2, actualRadius, dir1, dir2);
                Vector3 normal = Vector3.Cross(dir1, dir2).normalized;
                Vector3 startDir = (point1 - center).normalized;
                Vector3 endDir = (point2 - center).normalized;
                
                float startAngle = 0;
                float endAngle = Vector3.SignedAngle(startDir, endDir, normal) * Mathf.Deg2Rad;
                
                int arcSegments = Mathf.Max(3, Mathf.RoundToInt(angle / 10f));

                for (int j = 1; j < arcSegments; j++)
                {
                    float t = (float)j / arcSegments;
                    float arcAngle = startAngle + endAngle * t;
                    Quaternion rotation = Quaternion.AngleAxis(arcAngle * Mathf.Rad2Deg, normal);
                    Vector3 arcPoint = center + rotation * startDir * actualRadius;
                    smoothed.Add(arcPoint);
                }

                smoothed.Add(point2);
            }

            return smoothed;
        }

        private Vector3 GetArcCenter(Vector3 p1, Vector3 p2, float radius, Vector3 dir1, Vector3 dir2)
        {
            Vector3 normal = Vector3.Cross(dir1, dir2).normalized;
            Vector3 midpoint = (p1 + p2) * 0.5f;
            Vector3 toMidpoint = (p2 - p1).normalized;
            Vector3 toCenter = Vector3.Cross(normal, toMidpoint).normalized;
            
            float distance = Vector3.Distance(p1, p2);
            float height = Mathf.Sqrt(radius * radius - (distance * distance * 0.25f));
            
            return midpoint + toCenter * height;
        }
    }
}