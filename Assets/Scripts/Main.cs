using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class Main : MonoBehaviour
{
    [SerializeField]
    public ItemList itemList;
    public ShopList shopList;
    public Canvas canvasObj;
    public GameObject buttonPrefab;

    private List<GameObject> Button_List = new List<GameObject>();
    private int CurrentLevel;
    private int CurrentPage;
    private int LastLevel;
    private int LastPage;
    private int item_number;
    private int MaxItemNumber;

    private int Coins;
    private int Souls;
    private int TotalCoins;
    private string s_TotalCoins;

    private int TotalCost;
    private string s_TotalCost;
    private int TotalDays;
    private string s_TotalDays;

    private bool OnSale;
    private string []NumofPurchase = new string[90];

    private void Awake()
    {
        ReadString();

        OnSale = false;
        LastLevel = LastPage = -1;
        Coins = Souls = 0;

        CurrentLevel = PageManager.CurrentLevel;
        CurrentPage = PageManager.CurrentPage;
        MaxItemNumber = 9;
        for (int i=0; i< MaxItemNumber; i++)
        {
            GameObject tmpButton;
            tmpButton = Instantiate(buttonPrefab, transform.position, Quaternion.identity);
            tmpButton.transform.localScale = new Vector3(2f, 2f, 1);
            SetButtonAttribute(tmpButton, i);
        }

        GenerateRightHandSidePanel();
    }

    public void Update()
    {
        CurrentLevel = PageManager.CurrentLevel;
        CurrentPage = PageManager.CurrentPage;
        item_number = itemList.Level[CurrentLevel].Page[CurrentPage].Item.Count;

        if ((CurrentLevel != LastLevel) || (CurrentPage != LastPage))
        {
            LastLevel = CurrentLevel;
            LastPage = CurrentPage;
            UpdateButtonInputField();
        }
    }

    void UpdateButtonInputField()
    {
        for(int i=0; i< item_number; i++)
        {
            Button_List[i].SetActive(true);

            int Position = GetRelativePosition(CurrentLevel, CurrentPage, i);
            Transform _transform = Button_List[i].transform.GetChild(0);
            _transform.GetComponent<InputField>().text = NumofPurchase[Position];
        }

        for (int i = item_number; i < MaxItemNumber; i++)
        {
            Button_List[i].SetActive(false);
        }

        // Generate Label String message
        s_TotalCoins = TotalCoins.ToString();

        TotalCost = GetTotalCost();
        if (OnSale)
        {
            TotalCost /= 10;
            TotalCost *= 7;
        }
        s_TotalCost = TotalCost.ToString();

        if (TotalCoins >= TotalCost)
        {
            TotalDays = 0;
        }
        else
        {
            int remain = (TotalCost - TotalCoins) % 550;
            int tmpDays = (TotalCost - TotalCoins) / 550;
            TotalDays = (remain==0) ? (tmpDays) : (tmpDays+1);
        }
        s_TotalDays = TotalDays.ToString();
    }

    void SetButtonAttribute(GameObject _tmpButton, int i)
    {
        _tmpButton.transform.SetParent(canvasObj.transform);
        _tmpButton.transform.localPosition = new Vector3(-145, 260 - 90 * i, 0);
        _tmpButton.transform.GetComponent<Button>().onClick.AddListener(() => ButtonOnClick(_tmpButton));
        Button_List.Add(_tmpButton);
    }

    void ButtonOnClick(GameObject _obj)
    {
        // Get InputField transform from the gameobject (button)
        int _inputValue = CheckInputString(_obj);
        
        // Find relative position
        int index = 0;
        foreach(var tmpButton in Button_List)
        {
            if(_obj == tmpButton)
            {
                break;
            }
            index++;
        }

        Transform _transform = _obj.transform.GetChild(0);
        int Position = GetRelativePosition(CurrentLevel, CurrentPage, index);
        NumofPurchase[Position] = _inputValue.ToString();

        UpdateButtonInputField();
    }

    void ButtonSetCoins(GameObject _obj)
    {
        int _inputValue = CheckInputString(_obj);
        
        Coins = _inputValue;
        TotalCoins = (Coins + (Souls / 100) + (Souls / 10000) * 10);
        UpdateButtonInputField();
    }

    void ButtonSetSouls(GameObject _obj)
    {
        int _inputValue = CheckInputString(_obj);

        Souls = _inputValue;
        TotalCoins = (Coins + (Souls / 100) + (Souls / 10000) * 10);
        UpdateButtonInputField();
    }

    void ButtonSetSales(GameObject _obj)
    {
        OnSale = !OnSale;
        Transform _transform = _obj.transform.GetChild(0);
        _transform.GetComponent<InputField>().text = (OnSale) ? ("30% ON") : ("30% OFF");
        UpdateButtonInputField();
    }

    public IEnumerator ChangeMess(GameObject _obj)
    {
        yield return new WaitForSeconds(1f);
        Transform _transform = _obj.transform.GetChild(0);
        _transform.GetComponent<InputField>().text = "Finished!";
        yield return new WaitForSeconds(1f);
        _transform.GetComponent<InputField>().text = "SAVE";
    }

    // Write String
    void ButtonSetSave(GameObject _obj)
    {
        string path = "Assets/Resources/Record.txt";
        string data = "";
        foreach (var str in NumofPurchase)
        {
            data += str;
            data += "\r\n";
        }

        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(data);
        writer.Close();

        Transform _transform = _obj.transform.GetChild(0);
        _transform.GetComponent<InputField>().text = "SAVING...";
        StartCoroutine(ChangeMess(_obj));
    }

    void ReadString()
    {
        string path = "Assets/Resources/Record.txt";

        StreamReader reader = new StreamReader(path);
        NumofPurchase = reader.ReadToEnd().Split('\n');
        reader.Close();
    }

    int CheckInputString(GameObject _obj)
    {
        Transform _transform = _obj.transform.GetChild(0);
        string _inputString = _transform.GetComponent<InputField>().text;
        int _inputValue = 0;

        for (int i = 0; i < _inputString.Length; i++)
        {
            if (!System.Char.IsDigit(_inputString[i]))
            {
                break;
            }

            if (i + 1 == _inputString.Length)
            {
                _inputValue = int.Parse(_inputString);
            }
        }
        return _inputValue;
    }

    public void GenerateRightHandSidePanel()
    {
        for(int i=0; i<4; i++)
        {
            GameObject tmpButton;
            tmpButton = Instantiate(buttonPrefab, transform.position, Quaternion.identity);
            tmpButton.transform.localScale = new Vector3(2f, 2f, 1);
            tmpButton.transform.SetParent(canvasObj.transform);
            tmpButton.transform.localPosition = new Vector3(745, 260 - 90*i, 0);

            if (i==0)
            {
                tmpButton.transform.GetComponent<Button>().onClick.AddListener(() => ButtonSetCoins(tmpButton));
            }
            else if(i==1)
            {
                tmpButton.transform.GetComponent<Button>().onClick.AddListener(() => ButtonSetSouls(tmpButton));
            }
            else if(i==2)
            {
                tmpButton.transform.GetComponent<Button>().onClick.AddListener(() => ButtonSetSales(tmpButton));
                Transform _transform = tmpButton.transform.GetChild(0);
                _transform.GetComponent<InputField>().text = (OnSale)?("30% ON"):("30% OFF");
            }
            else
            {
                tmpButton.transform.GetComponent<Button>().onClick.AddListener(() => ButtonSetSave(tmpButton));
                Transform _transform = tmpButton.transform.GetChild(0);
                _transform.GetComponent<InputField>().text = "SAVE";
            }

            Button_List.Add(tmpButton);
        }
    }

    int GetRelativePosition(int _CurrentLevel , int _CurrentPage , int _index)
    {
        int _Position = 0;
        for(int i=0; i< _CurrentLevel; i++)
        {
            for(int j=0; j< itemList.Level[i].Page.Count; j++)
            {
                _Position += itemList.Level[i].Page[j].Item.Count;
            }
        }

        for(int j=0; j< _CurrentPage; j++)
        {
            _Position += itemList.Level[_CurrentLevel].Page[j].Item.Count;
        }

        _Position += _index;
        return _Position;
    }

    int GetTotalCost()
    {
        int _Cost = 0;
        int _index = 0;
        int _purchase;

        for(int i=0; i<itemList.Level.Count; i++)
        {
            for(int j=0; j<itemList.Level[i].Page.Count; j++)
            {
                for(int k=0; k<itemList.Level[i].Page[j].Item.Count; k++)
                {
                    _purchase = int.Parse(NumofPurchase[_index]);
                    _Cost += ( itemList.Level[i].Page[j].Item[k].Price * _purchase);
                    _index++;
                }
            }
        }
        return _Cost;
    }

    void OnGUI()
    {
        GUIStyle _style = new GUIStyle();
        _style.normal.background = null;
        _style.normal.textColor = new Color(255, 255, 255);
        _style.fontSize = 40;

        string message = "總計持有 " + s_TotalCoins + " 枚硬幣";
        GUI.Label(new Rect(1100, 500, 200, 20), message, _style);

        string message2 = "需要花費 " + s_TotalCost + " 枚硬幣";
        GUI.Label(new Rect(1100, 600, 200, 20), message2, _style);

        string message3 = "最少需要 " + s_TotalDays + " 天集滿心願";
        GUI.Label(new Rect(1100, 700, 800, 20), message3, _style);
    }
}