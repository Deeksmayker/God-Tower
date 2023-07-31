using static System.Runtime.InteropServices.Marshal;
using UnityEngine;

public class Stars : MonoBehaviour
{
    public int starDensity = 100;
    public int radius = 10;

    public Material starMaterial;
    public Mesh starMesh;

    private int _currentStarDensity;

    private ComputeShader initializeStarsShader;
    private ComputeBuffer starsBuffer, argsBuffer;
    
    private Bounds bounds;

    private struct StarData {
        public Vector4 position;
        public Matrix4x4 rotation;
    }

    void Start()
    {
       switch (SettingsController.GrassQualityValue)
        {
            case SettingsController.GrassQuality.Medium:
                _currentStarDensity = starDensity / 2;
                break;
            case SettingsController.GrassQuality.Low:
                _currentStarDensity = starDensity / 4;
                break;

            default:
                _currentStarDensity = starDensity;
                break;
        }

        initializeStarsShader = Resources.Load<ComputeShader>("InitializeStars");
        starsBuffer = new ComputeBuffer(_currentStarDensity, SizeOf(typeof(StarData)));
        argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);

        initializeStarsShader.SetBuffer(0, "_Stars", starsBuffer);
        initializeStarsShader.SetFloat("_Radius", radius);
        initializeStarsShader.Dispatch(0, Mathf.CeilToInt(_currentStarDensity / 256.0f), 1, 1);

        uint[] args = new uint[5] {0, 0, 0, 0, 0};
        args[0] = (uint)starMesh.GetIndexCount(0);
        args[1] = (uint)starsBuffer.count;
        args[2] = (uint)starMesh.GetIndexStart(0);
        args[3] = (uint)starMesh.GetBaseVertex(0);
        argsBuffer.SetData(args);

        bounds = new Bounds(transform.position, new Vector3(-500.0f, 200.0f, 500.0f));

        starMaterial.SetBuffer("_StarsBuffer", starsBuffer);
    }

    void Update() {
        Graphics.DrawMeshInstancedIndirect(starMesh, 0, starMaterial, bounds, argsBuffer);    
    }

    void OnDisable() {
        starsBuffer.Release();
        argsBuffer.Release();
        starsBuffer = null;
        argsBuffer = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
