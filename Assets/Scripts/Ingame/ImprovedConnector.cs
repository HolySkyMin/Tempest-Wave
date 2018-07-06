using UnityEngine;

namespace TempestWave.Ingame
{
    public class ImprovedConnector : MonoBehaviour
    {
        public Vector3 StartPos { get; set; }
        public Vector3 EndPos { get; set; }
        public float StartScale { get; set; }
        public float EndScale { get; set; }
        public int OwnerID { get; set; }

        private Vector3[] columns = new Vector3[4];
        private Vector2[] uvs = new Vector2[4];
        private int[] tris = new int[6] { 0, 1, 2, 1, 2, 3 };
        private Mesh thisMesh;
        public MeshRenderer MainRenderer;
        public MeshFilter filter;
        
        void Start()
        {
            thisMesh = new Mesh();
            filter.mesh = thisMesh;
            filter.mesh.Clear();

            MainRenderer.sortingLayerName = "Meshes";
        }

        void LateUpdate()
        {
            columns[0] = new Vector3(StartPos.x, StartPos.y - (StartScale / 6), StartPos.z + 1);
            columns[1] = new Vector3(StartPos.x, StartPos.y + (StartScale / 6), StartPos.z + 1);
            columns[2] = new Vector3(EndPos.x, EndPos.y - (EndScale / 6), EndPos.z + 1);
            columns[3] = new Vector3(EndPos.x, EndPos.y + (EndScale / 6), EndPos.z + 1);
            for (int i = 0; i < 4; i++) { uvs[i] = columns[i]; }

            filter.mesh.Clear();
            filter.mesh.vertices = columns;
            filter.mesh.uv = uvs;
            filter.mesh.triangles = tris;
        }
    }
}