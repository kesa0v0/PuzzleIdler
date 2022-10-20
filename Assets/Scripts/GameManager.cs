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

    public Indicator indicator;
    
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



    //TODO: 커스텀 자료형
    public float currentPoint = 0;
    public float maxPoint = 1000;
    public float incPointPerSec = 1;
    public float averagePointPer5Minutes = 0;
    public int currentLevel = 1;

    
    private void Update() {
        UpdateCalcPoint();
        UpdateUI();
    }

    private void UpdateCalcPoint()
    {
        var incPoint = 1.0f;
        GridManager.Instance.storedItemList.ForEach(item =>
        {
            incPoint += item.itemDefinition.point;
        });

        incPointPerSec = incPoint;

        currentPoint =  (currentPoint >= maxPoint) ? maxPoint : currentPoint + incPointPerSec * Time.deltaTime;
    }

    private float CalcPointPerFPS()
    {
        return incPointPerSec / Time.deltaTime;
    }
    

    #region UI elements

    public int preferedFPS;

    private VisualElement ui_Root;

    private static Label ui_PointLabel;
    // Increasing Point per second
    private static Label ui_IPPSLabel;
    private static Label ui_LevelLabel;

    private static VisualElement ui_PointBar;
    private static Label ui_PointBarPercent;

    // private bool IsUIReady;


    private async void Configure()
    {
        Application.targetFrameRate = preferedFPS;

        // UI
        ui_Root = GetComponentInChildren<UIDocument>().rootVisualElement;
        ui_PointLabel = ui_Root.Q<Label>("Point_Value");
        ui_IPPSLabel = ui_Root.Q<Label>("IncreasingPointPerSecValue");
        ui_LevelLabel = ui_Root.Q<Label>("Level_Value");

        ui_PointBar = ui_Root.Q<VisualElement>("Bar_Inner");
        ui_PointBarPercent = ui_Root.Q<Label>("Bar_Percent");


        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        
        // IsUIReady = true;
    }

    private void UpdateUI()
    {
        ui_PointLabel.text = currentPoint.ToString("F2") + " / " + maxPoint.ToString();
        ui_IPPSLabel.text = incPointPerSec.ToString() + " / sec";
        ui_LevelLabel.text = "Level " + currentLevel.ToString();

        // BarPercent
        float percent = currentPoint / maxPoint * 100;
        ui_PointBarPercent.text = percent.ToString("F2") + "%";
        ui_PointBar.style.width = new StyleLength(Length.Percent(percent));
    }

    #endregion
}
