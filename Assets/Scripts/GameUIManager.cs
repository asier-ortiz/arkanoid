using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    #region Singleton
    private static GameUIManager _instance;
    public static GameUIManager Instance => _instance;
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    public Text ScoreText;
    public Text LivesText;
    public Text LeaderBoardPossitionText;
    public TextMeshProUGUI NickInputText;
    public Text[] NicksText;
    public Text[] ScoresText;
    public GameObject LeftButton;
    public GameObject RightButton;
    public int Score { get; set; }

    private void Start()
    {
        Brick.OnBrickDestruction += OnBrickDestruction;
        BricksManager.Instance.OnLevelLoaded += OnLevelLoaded;
        GameManager.Instance.OnLiveLost += OnLiveLost;
        OnLiveLost(GameManager.Instance.AvailableLives);
        GameManager.Instance.OnLeaderBoardEntered += OnLeaderBoardEntered;
    }

    private void OnLeaderBoardEntered(int possition)
    {
        string place = "";
        switch (possition)
        {
            case 1:
                place = "1st" + Environment.NewLine + "Place";
                break;
            case 2:
                place = "2nd" + Environment.NewLine + "Place";
                break;
            case 3:
                place = "3rd" + Environment.NewLine + "Place";
                break;
            case 4:
                place = "4th" + Environment.NewLine + "Place";
                break;
            case 5:
                place = "5th" + Environment.NewLine + "Place";
                break;
            case 6:
                place = "6th" + Environment.NewLine + "Place";
                break;
            case 7:
                place = "7th" + Environment.NewLine + "Place";
                break;
            case 8:
                place = "8th" + Environment.NewLine + "Place";
                break;
            case 9:
                place = "9th" + Environment.NewLine + "Place";
                break;
            case 10:
                place = "10th" + Environment.NewLine + "Place";
                break;
        }
        LeaderBoardPossitionText.text = $"{place}";
    }

    private void OnLiveLost(int remainingLives)
    {
        LivesText.text = $"LIVES: {remainingLives}";
    }

    private void OnLevelLoaded()
    {
        UpdateScoreText(0);
    }

    private void UpdateScoreText(int increment)
    {
        this.Score += increment;
        string scoreString = this.Score.ToString().PadLeft(5, '0');
        ScoreText.text = $@"SCORE: {scoreString}"; 
    }

    private void OnBrickDestruction(Brick obj)
    {
        UpdateScoreText(10);  
    }

    private void OnDisable()
    {
        Brick.OnBrickDestruction -= OnBrickDestruction;
        BricksManager.Instance.OnLevelLoaded -= OnLevelLoaded;
    }

    private void Update()
    {
        if (GameManager.IsPortrait)
        {
            DisableButtons();

        } else
        {
            EnableButtons();
        }  
    }

    public void DisableButtons()
    {
        LeftButton.GetComponent<Image>().enabled = false;
        RightButton.GetComponent<Image>().enabled = false;
    }

    public void EnableButtons()
    {
        LeftButton.GetComponent<Image>().enabled = true;
        RightButton.GetComponent<Image>().enabled = true;
    }
}