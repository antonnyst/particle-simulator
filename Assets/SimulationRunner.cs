using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Atom
{
    public int type;
    public Vector2 position;
    public Vector2 velocity;
}

public class SimulationRunner : MonoBehaviour
{
    public ComputeShader computeShader;
    public bool RandomAttractions;

    public Atom[] atoms;

    public float[] typesStrength;
    public float[] typesLength;
    [Space]

    public int atomCount;
    int _atomCount;
    public float density;

    [Space,Range(0f,1f)]
    public float friction;
    [Range(0f, 10f)]
    public float timeScale;

    [HideInInspector]
    public float width;

    float timeOut = 0.1f;

    void Start()
    {
        InitSim();
    }

    void Update()
    {
        Time.timeScale = timeScale;
    }

    void FixedUpdate()
    {
        timeOut -= Time.fixedUnscaledDeltaTime;
        if (timeOut < 0f)
        {
            RunSim(Time.fixedDeltaTime);
        }
    }

    public void InitSim()
    {
        _atomCount = atomCount;

        atoms = new Atom[_atomCount];
        width = Mathf.Sqrt(_atomCount / density);

        for (int i = 0; i < atoms.Length; i++)
        {
            atoms[i] = new Atom();
            atoms[i].position = new Vector2((i / density) % width, (i / density) / width);
            atoms[i].velocity = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
            atoms[i].type = Random.Range(0, 4);

        }
        if (RandomAttractions)
        {
            for (int i = 0; i < typesStrength.Length; i++)
            {
                typesStrength[i] = Random.Range(-1f, 1f);
            }
            for (int i = 0; i < typesLength.Length; i++)
            {
                typesLength[i] = Random.Range(0f, 3f);
            }
        }

        timeOut = 0.1f;
    }

    void RunSim(float deltaTime)
    {
        int kernel = computeShader.FindKernel("Simulate");
        int atomsSize = sizeof(int) + sizeof(float) * 4;
        ComputeBuffer atomsBuffer = new ComputeBuffer(atoms.Length, atomsSize);
        atomsBuffer.SetData(atoms);

        computeShader.SetBuffer(kernel, "atoms", atomsBuffer);
        computeShader.SetFloat("time", deltaTime);
        computeShader.SetFloat("friction", 1 - friction);
        computeShader.SetFloats("typesStrength", typesStrength);
        computeShader.SetFloats("typesLength", typesLength);
        computeShader.SetInt("typeCount", 4);

        computeShader.SetFloat("width", width);
        computeShader.SetFloat("height", width);

        computeShader.Dispatch(kernel, atoms.Length/32, 1, 1);

        atomsBuffer.GetData(atoms);
        atomsBuffer.Dispose();
    }
}
