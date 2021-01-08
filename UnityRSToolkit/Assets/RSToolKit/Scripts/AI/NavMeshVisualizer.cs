// Inspired from https://blog.terresquall.com/2020/07/showing-unitys-navmesh-in-game/
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AI;

namespace RSToolkit.AI
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class NavMeshVisualizer : RSSingletonMonoBehaviour<NavMeshVisualizer>
    {
        public bool VisualizeOnStart = false;
        MeshFilter _meshFilter = null;
        public Mesh VisualizerMesh { get; private set; } = null;

        #region MonoBehaviour Funcitons
        protected override void Awake()
        {
            base.Awake();
            transform.position = Vector3.zero;
            _meshFilter = GetComponent<MeshFilter>();
        }
        // Start is called before the first frame update
        protected virtual void Start()
        {
            if (VisualizeOnStart)
            {
                Visualize();
            }
        }
        #endregion MonoBehaviour Funcitons

        // Generates the NavMesh shape and assigns it to the MeshFilter component.
        public void Visualize()
        {
            // NavMesh.CalculateTriangulation returns a NavMeshTriangulation object.
            NavMeshTriangulation meshData = NavMesh.CalculateTriangulation();

            // Create a new mesh and chuck in the NavMesh's vertex and triangle data to form the mesh.
            VisualizerMesh  = new Mesh();
            VisualizerMesh.vertices = meshData.vertices;
            VisualizerMesh.triangles = meshData.indices;

            // Assigns the newly-created mesh to the MeshFilter on the same GameObject.
            GetComponent<MeshFilter>().mesh = VisualizerMesh;
        }

        public void Clear()
        {
            GetComponent<MeshFilter>().mesh = null;
            VisualizerMesh = null;
        }

        public void Toggle()
        {
            if(!IsVisualizing)
            {
                Visualize();
            }
            else
            {
                Clear();
            }
        }

        public bool IsVisualizing
        {
            get
            {
                return VisualizerMesh != null;
            }
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(NavMeshVisualizer), true)]
    public class NavMeshVisualizerEditor : Editor
    {
        NavMeshVisualizer _targetNavMeshVisualizer;

        void OnEnable()
        {
            _targetNavMeshVisualizer = (NavMeshVisualizer)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (!_targetNavMeshVisualizer.IsVisualizing)
            {

                if (GUILayout.Button("Visualize"))
                {
                    _targetNavMeshVisualizer.Visualize();
                }
            }
            else
            {
                if (GUILayout.Button("Clear"))
                {
                    _targetNavMeshVisualizer.Clear();
                }
            }
        }
    }
#endif
}
