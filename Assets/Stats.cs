using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    void Start()
    {
        prevFrames = new List<float>();
    }

    public TextMeshProUGUI text;
    public int frames;
    List<float> prevFrames;
    void Update()
    {
        prevFrames.Add(Time.unscaledDeltaTime);
        float smooth = 0;
        foreach (float frame in prevFrames)
        {
            smooth += frame;
        }
        smooth = smooth / prevFrames.Count;
        text.text = "FPS: " + (1.0f/smooth).ToString("0");
        if (prevFrames.Count > frames)
        {
            prevFrames.RemoveRange(0, prevFrames.Count-frames);
        }
    }
}
