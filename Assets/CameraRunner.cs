using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRunner : MonoBehaviour
{
    public ComputeShader computeShader;
    public SimulationRunner simulationRunner;
    Atom[] atoms;
    int followAtom;

    public float cameraSize;
    public float atomSize;

    public Color[] colors;
    float[] _colors;

    public Vector2 pos;
    public float moveSpeed;
    public float scrollSpeed;

    public bool randomColors;

    void Start()
    {
        InitCamera();   
    }

    void Update()
    {
        atoms = simulationRunner.atoms;
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            pos += Vector2.ClampMagnitude(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")),1) * Time.unscaledDeltaTime * moveSpeed * cameraSize / 10f;
            followAtom = -1;
        }
        if (followAtom > -1)
        {
            pos = atoms[followAtom].position;
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            cameraSize = cameraSize + cameraSize * -Input.GetAxis("Mouse ScrollWheel") * Time.unscaledDeltaTime * scrollSpeed * 20;
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            pos = new Vector2(simulationRunner.width / 2, simulationRunner.width / 2);
            cameraSize = simulationRunner.width;
            followAtom = -1;
        }
        if (Input.GetMouseButtonDown(1))
        {
            float pixelSize = cameraSize / Screen.height;
            Vector2 mousePos = new Vector2(
                pos.x + ((Input.mousePosition.x - (Screen.width / 2f)) * pixelSize),
                pos.y + ((Input.mousePosition.y - (Screen.height / 2f)) * pixelSize)
            );

            int closest = -1;
            float dist = Mathf.Infinity;
            for (int i = 0; i < atoms.Length; i++)
            {
                float d = Vector2.Distance(mousePos, atoms[i].position);
                if (d < dist && d < 1f)
                {
                    closest = i;
                    dist = d;
                }
            }
            if (closest > -1)
            {
                followAtom = closest;
            } else
            {
                followAtom = -1;
            }
        }
    }

    public void InitCamera()
    {
        if (randomColors)
        {
            List<Color> chosen = new List<Color>();
            int colorLevels = 3;
            for (int i = 0; i < colors.Length; i++)
            {
                int r, g, b;
                do
                {
                    r = Random.Range(0, colorLevels);
                    g = Random.Range(0, colorLevels);
                    b = Random.Range(0, colorLevels);
                } while (r + g + b < 2 || chosen.Contains(new Color(r / (float)colorLevels - 1, g / (float)colorLevels - 1, b / (float)colorLevels - 1)));

                colors[i] = new Color(r / (float)(colorLevels - 1), g / (float)(colorLevels - 1), b / (float)(colorLevels - 1));
                chosen.Add(colors[i]);
            }
        }
        _colors = new float[4 * colors.Length];
        int j = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            _colors[j] = colors[i].r;
            _colors[j + 1] = colors[i].g;
            _colors[j + 2] = colors[i].b;
            _colors[j + 3] = colors[i].a;
            j += 4;
        }
        cameraSize = simulationRunner.width;
        pos = Vector2.one * cameraSize / 2;
        //moveSpeed *= cameraSize;
        followAtom = -1;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int width = source.width;
        int height = source.height;
       
        int renderKernel = computeShader.FindKernel("Render");
        int clearKernel = computeShader.FindKernel("Clear");
        
        int atomsSize = sizeof(int) + sizeof(float) * 4;
        ComputeBuffer atomsBuffer = new ComputeBuffer(atoms.Length, atomsSize);
        atomsBuffer.SetData(atoms);
        computeShader.SetBuffer(renderKernel, "Atoms", atomsBuffer);

        RenderTexture result = new RenderTexture(width, height, 24)
        {
            enableRandomWrite = true,
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Repeat,
            useMipMap = false
        };
        result.Create();

        computeShader.SetTexture(clearKernel, "Result", result);
        computeShader.Dispatch(clearKernel, Mathf.CeilToInt(width / 8), Mathf.CeilToInt(height/ 8), 1);

        computeShader.SetTexture(renderKernel, "Result", result);
        computeShader.SetVector("Position", pos);
        computeShader.SetFloat("AtomSize", atomSize);
        computeShader.SetFloat("PixelSize", cameraSize/height);
        computeShader.SetFloats("Colors", _colors);
        computeShader.SetInt("Width", width);
        computeShader.SetInt("Height", height);
        computeShader.SetFloat("SimulationWidth", simulationRunner.width);
        computeShader.SetFloat("SimulationHeight", simulationRunner.width);
        computeShader.Dispatch(renderKernel, Mathf.CeilToInt(atoms.Length/32), 1, 1);
        atomsBuffer.Dispose();
        Graphics.Blit(result, destination);
        result.Release();
    }
}
