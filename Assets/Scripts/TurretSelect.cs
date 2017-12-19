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

    void InitializeButtons()
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
            //add trigger through script. can't reference non-prefabs (info text) in the inspector
            EventTrigger trigger = buttons[i].GetComponent<EventTrigger>(); //fetch the eventtrigger component on this button
            EventTrigger.Entry entry = new EventTrigger.Entry();            //make a new entry
            entry.eventID = EventTriggerType.PointerEnter;                  //of type pointer enter
            entry.callback.AddListener((eventData) => { DisplayTurretInfo(shop.buttons[trapCard].name, "desc"); }); //run this funciton when it happens
            trigger.triggers.Add(entry);                                    //done, add it
            //TODO: add when mouse leaves clear it
            //TODO: add a display name and description to each button in shops to pass in displayturretinfo()
        }
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
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

    void DisplayTurretInfo(string name, string desc)
    {
        turretInfoName.text = name;
        turretInfoDesc.text = desc;
    }
}
