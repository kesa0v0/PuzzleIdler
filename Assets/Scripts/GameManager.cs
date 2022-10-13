using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System;

public class GameManager : MonoBehaviour
{
    // singletone
    public static GameManager Instance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Configure();
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }


    public int preferedFPS = 60;


    //TODO: 커스텀 자료형
    public float currentPoint = 0;
    public float maxPoint = 1000;
    public float incPointPerSec = 1;
    public int currentLevel = 1;

    // Start UpdateCalcPoint()
    private void Start()
    {
        UpdateCalcPoint();
    }
    
    private void Update() {
        ui_CurrencyLabel.text = currentPoint.ToString() + " / " + maxPoint.ToString();
        ui_ICPSLabel.text = incPointPerSec.ToString() + " / sec";
        ui_LevelLabel.text = "Level " + currentLevel.ToString();
    }

    public async void UpdateCalcPoint()
    {
        while (true)
        {
            await UniTask.Delay(1000/preferedFPS);
            currentPoint += CalcPointPerFPS();
        }
    }

    private float CalcPointPerFPS()
    {
        return incPointPerSec / preferedFPS;
    }
    

    #region UI elements


    private VisualElement ui_Root;

    private static Label ui_CurrencyLabel;
    // Increasing Currency per second
    private static Label ui_ICPSLabel;

    private static Label ui_LevelLabel;

    // private bool IsUIReady;


    private async void Configure()
    {
        Application.targetFrameRate = preferedFPS;

        // UI
        ui_Root = GetComponentInChildren<UIDocument>().rootVisualElement;
        ui_CurrencyLabel = ui_Root.Q<Label>("Currency_Value");
        ui_ICPSLabel = ui_Root.Q<Label>("IncreasingCurrencyPerSecValue");
        ui_LevelLabel = ui_Root.Q<Label>("Level_Value");

        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        
        // IsUIReady = true;
    }

    #endregion
}
