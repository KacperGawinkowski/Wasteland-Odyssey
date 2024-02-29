using UnityEngine;

namespace TestyAndStareRzeczy.Fov
{
    public class FOV : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;
        Mesh mesh;
        [SerializeField] public GameObject player;
        [SerializeField] private float fov = 360f;
        [SerializeField] private int rayCount = 50;
        public float viewDistance = 10f;
        [SerializeField] private float angle = 0f;
        [SerializeField] float offset;

        [SerializeField] float height;

        private Vector3[] vertices;
        private Vector2[] uv;
        private int[] triangles;
    
        // Start is called before the first frame update
        private void Start()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        
            vertices = new Vector3[rayCount + 1 + 1];
            uv = new Vector2[vertices.Length];
            triangles = new int[rayCount * 3];
        
        }
    
        void LateUpdate()
        {
            Vector3 origin = new Vector3(player.transform.position.x, height, player.transform.position.z);

            float angle2 = angle;
            float angleIncrease = fov / rayCount;

            vertices[0] = origin;

            int vertexIndex = 1;
            int triangleIndex = 0;
            for (int i = 0; i <= rayCount; i++)
            {
                Vector3 vertex;
                RaycastHit hit;
                //Physics.Raycast(origin, GetVectorFromAngle(angle2), viewDistance, layerMask);
                Ray ray = new(origin, GetVectorFromAngle(angle2));
                Physics.Raycast(ray, out hit, viewDistance, layerMask);
                if (hit.collider == null)
                {
                    //no hit
                    vertex = origin + GetVectorFromAngle(angle2) * viewDistance;
                }
                else
                {

                    //hit object
                    //vertex = hit.point;

                    //print(hit.normal);

                    //Vector3 sus = origin - GetVectorFromAngle(angle2) * viewDistance;

                    //Vector3 suspisio = 

                    vertex = hit.point - (hit.normal * offset);
                }
                vertices[vertexIndex] = vertex;

                if (i > 0)
                {
                    triangles[triangleIndex + 0] = 0;
                    triangles[triangleIndex + 1] = vertexIndex - 1;
                    triangles[triangleIndex + 2] = vertexIndex;

                    triangleIndex += 3;
                }

                vertexIndex++;
                angle2 -= angleIncrease;
            }


            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
        }

        private Vector3 GetVectorFromAngle(float angle)
        {
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
        }
    }
}

