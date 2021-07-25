using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Transform[] menuItems;
    public SimulationRunner simulationRunner;
    public CameraRunner cameraRunner;

    [Space]
    public Slider speedSlider;
    public TextMeshProUGUI speedText;

    [Space]
    public Slider frictionSlider;
    public TextMeshProUGUI frictionText;

    [Space]
    public Slider atomCountSlider;
    public TextMeshProUGUI atomCountText;

    [Space]
    public Slider densitySlider;
    public TextMeshProUGUI densityText;

    [Space]
    public Toggle attractionsToggle;

    [Space]
    public Slider moveSpeedSlider;
    public TextMeshProUGUI moveSpeedText;

    [Space]
    public Slider typeCountSlider;
    public TextMeshProUGUI typeCountText;

    [Space]
    public Slider zoomSpeedSlider;
    public TextMeshProUGUI zoomSpeedText;

    [Space]
    public Toggle colorsToggle;

    [Space]
    public Toggle renderingToggle;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleMenuItem(int index)
    {
        if (menuItems[index].GetComponent<RectTransform>().rect.height == 25)
        {
            menuItems[index].GetComponent<RectTransform>().sizeDelta = new Vector2(360, menuItems[index].GetComponent<RectTransform>().GetChild(1).GetComponent<RectTransform>().sizeDelta.y + 25);
            menuItems[index].GetChild(1).gameObject.SetActive(true);
        } else
        {
            menuItems[index].GetComponent<RectTransform>().sizeDelta = new Vector2(360, 25);
            menuItems[index].GetChild(1).gameObject.SetActive(false);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void ChangeSpeed()
    {
        simulationRunner.timeScale = speedSlider.value / 10;
        speedText.text = (speedSlider.value / 10).ToString("0.0");
    }

    public void ChangeFriction()
    {
        simulationRunner.friction = frictionSlider.value / 100;
        frictionText.text = (frictionSlider.value / 100).ToString("0.00");
    }

    public void ChangeAtomCount()
    {
        simulationRunner.atomCount = (int)atomCountSlider.value * 100;
        atomCountText.text = (atomCountSlider.value * 100).ToString("0");
    }

    public void ChangeDensity()
    {
        simulationRunner.density = densitySlider.value / 10;
        densityText.text = (densitySlider.value / 10).ToString("0.0");
    }

    public void ChangeAttractions()
    {
        simulationRunner.RandomAttractions = attractionsToggle.isOn;
    }

    public void ChangeMoveSpeed()
    {
        cameraRunner.moveSpeed = moveSpeedSlider.value / 10;
        moveSpeedText.text = (moveSpeedSlider.value / 10).ToString("0.0");
    }

    public void ChangeZoomSpeed()
    {
        cameraRunner.scrollSpeed = zoomSpeedSlider.value / 10;
        zoomSpeedText.text = (zoomSpeedSlider.value / 10).ToString("0.0");
    }

    public void ChangeColors()
    {
        cameraRunner.randomColors = colorsToggle.isOn;
    }

    public void ChangeRendering()
    {
        cameraRunner.repeatingRender = renderingToggle.isOn;
    }

    public void ChangeTypeCount()
    {
        simulationRunner.typeCount = (int)typeCountSlider.value;
        typeCountText.text = typeCountSlider.value.ToString("0");
    }

    public void RestartSimulation()
    {
        simulationRunner.InitSim();
        cameraRunner.InitCamera();
    }

}
