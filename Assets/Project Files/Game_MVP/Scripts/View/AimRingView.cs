using System.Collections.Generic;
using UnityEngine;

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class AimRingView : MonoBehaviour
    {
        [Header("Mesh Settings")]
        [SerializeField] float width = 0.2f;
        [SerializeField] int detalisation = 50;
        [SerializeField] float stripeLength = 2f;
        [SerializeField] float gapLength = 1f;
        [SerializeField] float rotationSpeed = 30f;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Mesh mesh;
        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();

        private Transform _targetToFollow;
        private float _radius;

        public void Init(Transform target)
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            
            mesh = new Mesh { name = "AimRingMesh" };
            meshFilter.mesh = mesh;

            _targetToFollow = target;
            transform.SetParent(null); 
        }

        public void SetRadius(float radius)
        {
            _radius = Mathf.Max(radius, 1f);
            GenerateMesh();
        }

        public void Toggle(bool isActive)
        {
            if(meshRenderer) meshRenderer.enabled = isActive;
        }

        private void LateUpdate()
        {
            if (_targetToFollow == null) 
            {
                if(meshRenderer.enabled) Toggle(false);
                return;
            }

            if(!meshRenderer.enabled) Toggle(true);

            transform.position = new Vector3(_targetToFollow.position.x, 0.1f, _targetToFollow.position.z);
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        private void GenerateMesh()
        {
            mesh.Clear();
            vertices.Clear();
            triangles.Clear();

            float stepAngle = 360f / detalisation;
            float stripeAngle = 180f * stripeLength / (Mathf.PI * _radius);
            int stripeSectors = Mathf.Max(Mathf.FloorToInt(stripeAngle / stepAngle), 1);
            float gapAngle = 180f * gapLength / (Mathf.PI * _radius);
            int gapSectors = Mathf.Max(Mathf.FloorToInt(gapAngle / stepAngle), 1);

            float currentAngle = 0;

            while (currentAngle < 360f)
            {
                // Create Solid Stripe
                for (int i = 0; i < stripeSectors && currentAngle < 360f; i++)
                {
                    vertices.Add(GetPoint(_radius, Mathf.Deg2Rad * currentAngle));
                    vertices.Add(GetPoint(_radius + width, Mathf.Deg2Rad * currentAngle));
                    vertices.Add(GetPoint(_radius, Mathf.Deg2Rad * (currentAngle + stepAngle)));

                    vertices.Add(GetPoint(_radius + width, Mathf.Deg2Rad * currentAngle));
                    vertices.Add(GetPoint(_radius + width, Mathf.Deg2Rad * (currentAngle + stepAngle)));
                    vertices.Add(GetPoint(_radius, Mathf.Deg2Rad * (currentAngle + stepAngle)));

                    int trisCount = triangles.Count;
                    triangles.Add(trisCount + 2); triangles.Add(trisCount + 1); triangles.Add(trisCount);
                    triangles.Add(trisCount + 5); triangles.Add(trisCount + 4); triangles.Add(trisCount + 3);

                    currentAngle += stepAngle;
                }


                for (int i = 0; i < gapSectors && currentAngle < 360f; i++)
                {
                    currentAngle += stepAngle;
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
        }

        private Vector3 GetPoint(float r, float angle)
        {
            return new Vector3(Mathf.Cos(angle) * r, 0, Mathf.Sin(angle) * r);
        }
    }