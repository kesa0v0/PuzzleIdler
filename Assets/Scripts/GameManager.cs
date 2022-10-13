using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{


    #region UI elements
    private VisualElement ui_Root;


    private static Label ui_ItemDetailHeader;
    private static Label ui_ItemDetailBody;
    private bool IsUIReady;


    private async void Configure()
    {
        ui_Root = GetComponentInChildren<UIDocument>().rootVisualElement;
        VisualElement itemDetails = ui_Root.Q<VisualElement>("ItemDetails");
        ui_ItemDetailHeader = itemDetails.Q<Label>("Header");
        ui_ItemDetailBody = itemDetails.Q<Label>("Body");

        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        
        IsUIReady = true;
    }

    #endregion
}
