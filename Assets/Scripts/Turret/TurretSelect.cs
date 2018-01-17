using UnityEngine;
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
            Debug.Log("uh oh");
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
        turretInfoDesc.text = turret.displayDesc + "\n";
        //parse stat values (beta)
        if (turret.GetStat(FetchStat.ExtraTargets) != "0" && turret.GetStat(FetchStat.ExtraTargets) != "") turretInfoDesc.text += "\n<color=green>Can hit multiple enemies</color>";
        else if (turret.GetStat(FetchStat.AOE) != "0" && turret.GetStat(FetchStat.AOE) != "") turretInfoDesc.text += "\n<color=green>Can hit multiple enemies</color>";
        if (turret.GetStat(FetchStat.DebuffType) != "")
        {
            if (turret.GetStat(FetchStat.DebuffType) != "AtkSpeed" && turret.GetStat(FetchStat.DebuffType) != "ShieldBreak")
            {
                if (turret.overrideFieldDebuffType == "") turretInfoDesc.text += "\n<color=green>Debuff Type: " + turret.GetStat(FetchStat.DebuffType) + "</color>";
                else turretInfoDesc.text += "\n<color=green>Debuff Type: " + turret.overrideFieldDebuffType + "</color>";
                turretInfoDesc.text += "\n<color=red>Ineffective vs shield</color>\n";
            }
            else
            {
                turretInfoDesc.text += "\n<color=green>Buff Type: " + turret.GetStat(FetchStat.DebuffType) + "</color>\n";
            }
        }
        if (turret.overrideFieldArmorWeak) turretInfoDesc.text += "\n<color=red>Ineffective vs armor</color>\n";
        if (turret.overrideFieldDamage == "") turretInfoDesc.text += "\nDamage: " + turret.GetStat(FetchStat.Damage);
        else turretInfoDesc.text += "\nDamage: " + turret.overrideFieldDamage;

        turretInfoDesc.text += "\nFire Rate: " + turret.GetStat(FetchStat.FireRate);

        turretInfoDesc.text += "\nRange: " + turret.GetRange(false);

        turretInfoDesc.text += "\n\n<color=yellow>Cost: " + turret.GetStat(FetchStat.Cost) + "</color>";
        if (turret.GetStat(FetchStat.ResourceCost) != "0") turretInfoDesc.text += "\n<color=yellow>Resource Cost: " + turret.GetStat(FetchStat.ResourceCost) + " " + turret.GetStat(FetchStat.ResourceType) + "</color>";
    }

    void HideTurretInfo()
    {
        turretInfoName.text = "";
        turretInfoDesc.text = "";
    }
}
