using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TurretSelect : MonoBehaviour {

    public int maxTurrets;  //store this in palyer stats or buildmanager or something instead

    public Text Title;

    [SerializeField]
    Shop shop;

    public Button buttonPrefab;
    public GameObject content;
    Button[] buttons;

    List<int> selectedTurrets = new List<int>();

    private void Start()
    {
        Title.text = "LOADING TURRET LIST"; //wait until shops start has been called
        Invoke("InitializeButtons", 2f);
    }

    void InitializeButtons()
    {
        Title.text = "Select Turrets (0/" + maxTurrets + ")";

        buttons = new Button[shop.buttons.Length];

        for (int i = 0; i < buttons.Length; ++i)
        {
            Button b = Instantiate(buttonPrefab, content.transform);

            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(delegate { ToggleTurretFromListItem(i); });   //this doesn't work
            Debug.Log("Adding listener: " + i);
            buttons[i] = b;
        }
    }

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
}
