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

    public int typeCount;
    int _typeCount;
    public float[] typesStrength;
    public float[] typesLength;

    float[] _typesStrength;
    float[] _typesLength;

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
        _typeCount = typeCount;

        atoms = new Atom[_atomCount];
        width = Mathf.Sqrt(_atomCount / density);

        for (int i = 0; i < atoms.Length; i++)
        {
            atoms[i] = new Atom();
            atoms[i].position = new Vector2((i / density) % width, (i / density) / width);
            atoms[i].velocity = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
            atoms[i].type = i % _typeCount;//Random.Range(0, _typeCount);

        }
        if (RandomAttractions || typesLength.Length != _typeCount  * _typeCount || typesStrength.Length != _typeCount * _typeCount)
        {
            typesStrength = new float[_typeCount * _typeCount];
            typesLength = new float[_typeCount * _typeCount];

            for (int i = 0; i < typesStrength.Length; i++)
            {
                typesStrength[i] = Random.Range(-1f, 1f);
            }
            for (int i = 0; i < typesLength.Length; i++)
            {
                typesLength[i] = Random.Range(0f, 3f);
            }
        }

        _typesLength = new float[_typeCount * _typeCount * 4];
        _typesStrength = new float[_typeCount * _typeCount * 4];

        for (int i = 0; i < _typeCount * _typeCount; i++)
        {
            _typesLength[i*4] = typesLength[i];
            _typesStrength[i*4] = typesStrength[i];
        }
        
        timeOut = 0.01f;
    }

    void RunSim(float deltaTime)
    {
        int calculateKernel = computeShader.FindKernel("CalculateVelocity");
        int applyKernel = computeShader.FindKernel("ApplyVelocity");
        
        int atomsSize = sizeof(int) + sizeof(float) * 4;

        computeShader.SetFloat("time", deltaTime);
        computeShader.SetFloat("friction", 1 - friction);
        computeShader.SetFloats("typesStrength", _typesStrength);
        computeShader.SetFloats("typesTest", typesStrength);
        computeShader.SetFloats("typesLength", _typesLength);
        computeShader.SetInt("typeCount", _typeCount);

        computeShader.SetFloat("width", width);
        computeShader.SetFloat("height", width);


        ComputeBuffer atomsBuffer = new ComputeBuffer(atoms.Length, atomsSize);
        atomsBuffer.SetData(atoms);
        computeShader.SetBuffer(calculateKernel, "atoms", atomsBuffer);
        computeShader.Dispatch(calculateKernel, Mathf.CeilToInt(atoms.Length / 32), 1, 1);
        atomsBuffer.GetData(atoms);
        atomsBuffer.Dispose();
        
        atomsBuffer = new ComputeBuffer(atoms.Length, atomsSize);
        atomsBuffer.SetData(atoms);
        computeShader.SetBuffer(applyKernel, "atoms", atomsBuffer);
        computeShader.Dispatch(applyKernel, Mathf.CeilToInt(atoms.Length / 32), 1, 1);
        atomsBuffer.GetData(atoms);
        atomsBuffer.Dispose();
    }
}
