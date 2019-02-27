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
    public GameObject CurrentRankObj;
    public GameObject TargetRankObj;
    public GameObject SkillRankObj;
    public GameObject UnitTimeObj;

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
    private int RankUpCost;
    private int TotalCost;
    private int TotalDays;
    private int TotalMinutesCostBySkill;

    private bool OnSale;
    private string[] NumofPurchase = new string[90];

    private int CurrentRank;
    private int TargetRank;
    private int SoulsUnitTime;
    private int UnitTime;
    private int[] CostForRank;
    private int[] SkillRankToSouls;

    private void Awake()
    {
        ReadString();

        CurrentLevel = PageManager.CurrentLevel;
        CurrentPage = PageManager.CurrentPage;
        LastLevel = LastPage = -1;

        OnSale = false;
        Coins = Souls = 0;

        MaxItemNumber = 9;
        for (int i=0; i< MaxItemNumber; i++)
        {
            GameObject tmpButton;
            tmpButton = Instantiate(buttonPrefab, transform.position, Quaternion.identity);
            tmpButton.transform.localScale = new Vector3(2f, 2f, 1);
            SetButtonAttribute(tmpButton, i);
        }

        CurrentRankObj.GetComponent<Dropdown>().onValueChanged.AddListener(SetCurrentRank);
        TargetRankObj.GetComponent<Dropdown>().onValueChanged.AddListener(SetTargetRank);
        SkillRankObj.GetComponent<Dropdown>().onValueChanged.AddListener(SetSkillRank);
        UnitTimeObj.GetComponent<Dropdown>().onValueChanged.AddListener(SetUnitTime);

        CurrentRank = TargetRank = RankUpCost = SoulsUnitTime = UnitTime = 0;
        CostForRank = new int[]{ 0,0,300,450,600,1200,1800,2100,2550,3750,4750,5450,6350,8350 };
        SkillRankToSouls = new int[] { 0, 3, 4, 5, 6 };

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
        // Display lefthandside buttons
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

        // Calculate total cost at the store
        TotalCost = GetTotalCost();

        // Calculate days that user should spend
        TotalDays = GetTotalDays();

        // Calculate how many minutes user should pay to use the alliance skills to earn enough souls
        TotalMinutesCostBySkill = GetTotalMinutesCostBySkill();
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

    int GetRelativePosition(int _CurrentLevel, int _CurrentPage, int _index)
    {
        int _Position = 0;
        for (int i = 0; i < _CurrentLevel; i++)
        {
            for (int j = 0; j < itemList.Level[i].Page.Count; j++)
            {
                _Position += itemList.Level[i].Page[j].Item.Count;
            }
        }

        for (int j = 0; j < _CurrentPage; j++)
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

        for (int i = 0; i < itemList.Level.Count; i++)
        {
            for (int j = 0; j < itemList.Level[i].Page.Count; j++)
            {
                for (int k = 0; k < itemList.Level[i].Page[j].Item.Count; k++)
                {
                    _purchase = int.Parse(NumofPurchase[_index]);
                    _Cost += (itemList.Level[i].Page[j].Item[k].Price * _purchase);
                    _index++;
                }
            }
        }

        if (OnSale)
            _Cost = _Cost / 10 * 7;

        // Add the cost of ranking up at last
        RankUpCost = (TargetRank > CurrentRank) ? (TargetRank - CurrentRank) : (0);
        _Cost += RankUpCost;

        return _Cost;
    }

    int GetTotalDays()
    {
        int _Days = 0;

        if (TotalCoins >= TotalCost)
        {
            _Days = 0;
        }
        else
        {
            int remain = (TotalCost - TotalCoins) % 550;
            int tmpDays = (TotalCost - TotalCoins) / 550;
            _Days = (remain == 0) ? (tmpDays) : (tmpDays + 1);
        }

        return _Days;
    }

    int GetTotalMinutesCostBySkill()
    {
        int NeedOfCoins = TotalCost - TotalCoins;
        int _Minutes = 0;

        if ( SoulsUnitTime<=0 || UnitTime <= 0 || NeedOfCoins <=0 )
        {
            return _Minutes;
        }
        else
        {
            // Find the nearest number of souls not exceed the coins ( due to the extra 10% coins reward )
            int ObjectiveCoins = (TotalCost - Coins);
            int NeedOfSouls = (int)(ObjectiveCoins/11)*1000 ;

            // add 10% extra coins after converting to souls
            NeedOfSouls += (ObjectiveCoins - ((int)(NeedOfSouls / 100) + (int)(NeedOfSouls / 10000) * 10)) * 100;
            NeedOfSouls -= Souls;

            _Minutes = NeedOfSouls / SoulsUnitTime / (60 / UnitTime);
            if( NeedOfSouls % (SoulsUnitTime*(60/UnitTime))!= 0)
                _Minutes ++;

            return _Minutes;
        }
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
        int _inputValue = CheckInputString(_obj);
        
        int index = 0;
        foreach(var tmpButton in Button_List)
        {
            if(_obj == tmpButton)
            {
                break;
            }
            index++;
        }

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
        _transform.GetComponent<InputField>().text = (OnSale) ? ("30% OFF") : ("0% OFF");
        UpdateButtonInputField();
    }

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
        StartCoroutine(ChangeMess(_obj,0));
    }

    void ButtonSetLoad(GameObject _obj)
    {
        Transform _transform = _obj.transform.GetChild(0);
        _transform.GetComponent<InputField>().text = "LOADING...";

        StartCoroutine(ChangeMess(_obj, 1));
        ReadString();
        UpdateButtonInputField();
    }

    void ButtonSetClear(GameObject _obj)
    {
        for(int i=0; i<90; i++)
        {
            NumofPurchase[i] = "0";
        }

        UpdateButtonInputField();
    }

    public IEnumerator ChangeMess(GameObject _obj, int _index)
    {
        Transform _transform = _obj.transform.GetChild(0);

        yield return new WaitForSeconds(0.5f);
        _transform.GetComponent<InputField>().text = "Finished!";
        yield return new WaitForSeconds(0.5f);

        if (_index == 0)
        {
            _transform.GetComponent<InputField>().text = "SAVE";
        }
        else if (_index == 1)
        {
            _transform.GetComponent<InputField>().text = "LOAD";
        }
    }

    void ReadString()
    {
        string path = "Assets/Resources/Record.txt";

        StreamReader reader = new StreamReader(path);
        NumofPurchase = reader.ReadToEnd().Split('\n');
        reader.Close();
    }

    public void GenerateRightHandSidePanel()
    {
        for(int i=0; i<6; i++)
        {
            GameObject tmpButton;
            tmpButton = Instantiate(buttonPrefab, transform.position, Quaternion.identity);

            Transform btnTransform = tmpButton.GetComponent<Transform>();
            btnTransform.localScale = new Vector3(2f, 2f, 1);
            btnTransform.SetParent(canvasObj.transform);
            btnTransform.localPosition = new Vector3(745, 260 - 90*i, 0);

            Transform childTransform = btnTransform.GetChild(0);

            if (i==0)
            {
                btnTransform.GetComponent<Button>().onClick.AddListener(() => ButtonSetCoins(tmpButton));
            }
            else if(i==1)
            {
                btnTransform.GetComponent<Button>().onClick.AddListener(() => ButtonSetSouls(tmpButton));
            }
            else if(i==2)
            {
                btnTransform.GetComponent<Button>().onClick.AddListener(() => ButtonSetSales(tmpButton));
                childTransform.GetComponent<InputField>().text = (OnSale)?("30% OFF"):("0% OFF");
            }
            else if(i==3)
            {
                btnTransform.GetComponent<Button>().onClick.AddListener(() => ButtonSetSave(tmpButton));
                childTransform.GetComponent<InputField>().text = "SAVE";
            }
            else if (i == 4)
            {
                btnTransform.GetComponent<Button>().onClick.AddListener(() => ButtonSetLoad(tmpButton));
                childTransform.GetComponent<InputField>().text = "LOAD";
            }
            else
            {
                btnTransform.localPosition = new Vector3(-145, 350, 0);
                btnTransform.GetComponent<Button>().onClick.AddListener(() => ButtonSetClear(tmpButton));
                childTransform.GetComponent<InputField>().text = "CLEAR";
            }

            Button_List.Add(tmpButton);
        }

        // Set Dropdown Attribute
        Transform CurDropdownTransform = CurrentRankObj.GetComponent<Transform>();
        CurDropdownTransform.localScale = new Vector3(2, 2, 0);
        CurDropdownTransform.localPosition = new Vector3(245, 350, 0);

        Transform TarDropdownTransform = TargetRankObj.GetComponent<Transform>();
        TarDropdownTransform.localScale = new Vector3(2, 2, 0);
        TarDropdownTransform.localPosition = new Vector3(245, 260, 0);

        Transform SkillDropdownTransform = SkillRankObj.GetComponent<Transform>();
        SkillDropdownTransform.localScale = new Vector3(2, 2, 0);
        SkillDropdownTransform.localPosition = new Vector3(245, 170, 0);

        Transform UnitTimeDropdownTransform = UnitTimeObj.GetComponent<Transform>();
        UnitTimeDropdownTransform.localScale = new Vector3(2, 2, 0);
        UnitTimeDropdownTransform.localPosition = new Vector3(245, 80, 0);
    }

    void SetCurrentRank(int _rank)
    {
        CurrentRank = CostForRank[_rank];
        UpdateButtonInputField();
    }

    void SetTargetRank(int _rank)
    {
        TargetRank = CostForRank[_rank];
        UpdateButtonInputField();
    }

    void SetSkillRank(int _rank)
    {
        SoulsUnitTime = SkillRankToSouls[_rank];
        UpdateButtonInputField();
    }

    void SetUnitTime(int _rank)
    {
        UnitTime = (_rank == 0) ? (-1) : (_rank + 9);
        UpdateButtonInputField();
    }

    void OnGUI()
    {
        GUIStyle _style = new GUIStyle();
        _style.normal.background = null;
        _style.normal.textColor = new Color(255, 255, 255);
        _style.fontSize = 40;

        string message = "總計持有 " + TotalCoins.ToString() + " 枚硬幣";
        GUI.Label(new Rect(830, 500, 200, 20), message, _style);

        string message2 = "需要花費 " + TotalCost.ToString() + " 枚硬幣";
        GUI.Label(new Rect(830, 600, 200, 20), message2, _style);

        string message3 = "最少需要 " + TotalDays.ToString() + " 天集滿心願";
        GUI.Label(new Rect(830, 700, 800, 20), message3, _style);

        string message4 = "每 " + UnitTime.ToString() +" 秒獲得 " + SoulsUnitTime.ToString() + " 心願，可花費 "
                        + TotalMinutesCostBySkill.ToString() + " 分掛滿";
        GUI.Label(new Rect(830, 800, 800, 20), message4, _style);
    }
}