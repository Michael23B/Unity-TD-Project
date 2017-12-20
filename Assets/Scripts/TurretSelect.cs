﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TurretSelect : MonoBehaviour {

    public int maxTurrets;  //store this in palyer stats or buildmanager or something instead

    public Text Title;

    [SerializeField]
    Shop shop;

    [SerializeField]
    CanvasGroup canvasGroup;

    public Button buttonOpenWindow;

    public Button buttonPrefab;
    public GameObject content;
    Button[] buttons;

    List<int> selectedTurrets = new List<int>();
    [Space(10)]
    public Text turretInfoName;
    public Text turretInfoDesc;

    private void Start()
    {
        Title.text = "LOADING TURRET LIST"; //wait until shops start has been called
        Invoke("InitializeButtons", 2f);
        Hide();
    }

    void InitializeButtons()    //TurretSelect currently relies on blueprints index matching buttons index (button 0 accesses blueprint 0) @ line 54
    {
        Title.text = "Select Turrets (0/" + maxTurrets + ")";

        buttons = new Button[shop.buttons.Length];

        for (int i = 0; i < buttons.Length; ++i)
        {
            Button b = Instantiate(buttonPrefab, content.transform);

            int trapCard = i;   //delegate needs stored variable
            b.onClick.AddListener(delegate { ToggleTurretFromListItem(trapCard); });

            buttons[i] = b;

            buttons[i].image.overrideSprite = shop.buttons[i].image.sprite;

            //Adds hover trigger for turret info panel
            //add trigger through script. can't reference non-prefabs (info text) in the inspector
            EventTrigger trigger = buttons[i].GetComponent<EventTrigger>(); //fetch the eventtrigger component on this button
            EventTrigger.Entry entry = new EventTrigger.Entry();            //make a new entry
            entry.eventID = EventTriggerType.PointerEnter;                  //of type pointer enter
            entry.callback.AddListener((eventData) => { DisplayTurretInfo(shop.Blueprints[trapCard]); }); //run this funciton when it happens
            trigger.triggers.Add(entry);                                    //done, add it

            EventTrigger.Entry entryPointerExit = new EventTrigger.Entry();            //make a new entry
            entryPointerExit.eventID = EventTriggerType.PointerExit;                  //of type pointer enter
            entryPointerExit.callback.AddListener((eventData) => { HideTurretInfo(); }); //run this funciton when it happens
            trigger.triggers.Add(entryPointerExit);
            //TODO: add when mouse leaves clear it
        }
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        if (selectedTurrets.Count != 0) buttonOpenWindow.gameObject.SetActive(false);
    }

    public void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void UpdateShop()
    {
        shop.EnableTurrets(selectedTurrets.ToArray());
    }

    #region Turret List Handling
    void AddToTurretList(int index)
    {
        if (selectedTurrets.Count >= maxTurrets)
        {
            Debug.Log("Turret Limit Reached!");
            return;
        }
        else
        {
            selectedTurrets.Add(index);
            Title.text = "Select Turrets (" + selectedTurrets.Count + "/" + maxTurrets + ")";
            buttons[index].image.color = Color.grey;
        }

    }

    void RemoveFromTurretList(int index)
    {
        if (selectedTurrets.Count <= 0)
        {
            Debug.Log("oh oh");
            return;
        }
        else
        {
            selectedTurrets.Remove(index);
            Title.text = "Select Turrets (" + selectedTurrets.Count + "/" + maxTurrets + ")";
            buttons[index].image.color = Color.white;
        }
    }

    void ToggleTurretFromListItem(int index)
    {
        Debug.Log("Toggling index: " + index);

        if (selectedTurrets.Contains(index))
        {
            RemoveFromTurretList(index);
            return;
        }
        else
        {
            AddToTurretList(index);
        }
    }
    #endregion

    void DisplayTurretInfo(TurretBlueprint turret)
    {
        turretInfoName.text = turret.displayName;
        turretInfoDesc.text = turret.displayDesc;

        //parse stat values (beta)
        if (turret.GetStat(FetchStat.Damage) != "" && turret.GetStat(FetchStat.Damage) != "0") turretInfoDesc.text += "\n\n-Damage: " + turret.GetStat(FetchStat.Damage);   //TODO: dont check based on damge (laser turret)
        //else turretInfoDesc.text += "\n\nDebuff: " + turret.GetStat(FetchStat.DebuffType); TODO: fix debuff type check
        else
        {
            turretInfoDesc.text += "\n\n<color=green>Can Buff/Debuff</color>";
            if (turret.GetStat(FetchStat.DebuffType) != "AtkSpeed") turretInfoDesc.text += "\n<color=red>No effect vs shield</color>\n";    //TODO: dont check only atkspeed
            
        }
        turretInfoDesc.text += "\n-Fire Rate: " + turret.GetStat(FetchStat.FireRate);
        turretInfoDesc.text += "\n-Range: " + turret.GetRange(false);
        turretInfoDesc.text += "\n-Cost: " + turret.GetStat(FetchStat.Cost);
        if (turret.GetStat(FetchStat.ResourceCost) != "0") turretInfoDesc.text += "\n-Resources: " + turret.GetStat(FetchStat.ResourceCost) + " " + turret.GetStat(FetchStat.ResourceType);
    }

    void HideTurretInfo()
    {
        turretInfoName.text = "";
        turretInfoDesc.text = "";
    }
}
