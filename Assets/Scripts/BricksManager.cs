using System;
using System.Collections.Generic;
using UnityEngine;

public class BricksManager : MonoBehaviour
{
    #region Singleton
    private static BricksManager _instance;
    public static BricksManager Instance => _instance;
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

    private int maxRows = 14;
    private int maxCols = 13;
    public int CurrentLevel;
    public Brick brickPrefab;
    public event Action OnLevelLoaded;
    public Color[] BrickColors;
    public Sprite[] Sprites;
    public int InitialBricksCount { get; set; }
    public List<Brick> RemainingBricks { get; set; }
    public List<int[,]> LevelsData { get; set; }
    
    private void Start()
    {
        this.LevelsData = this.LoadLevelData();
        this.GenerateBricks();
        this.OnLevelLoaded?.Invoke();
    }

    void GetBlocksInformation(float blockWidth, out float screenWidth, out float screenHeight, in int columns, out float scaleMultiplier)
    {
        Camera cam = Camera.main;
        screenWidth = (cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)) - cam.ScreenToWorldPoint(new Vector3(0, 0, 0))).x;
        screenHeight = (cam.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)) - cam.ScreenToWorldPoint(new Vector3(0, 0, 0))).y;
        scaleMultiplier = screenWidth / (blockWidth * columns);
    }

    private void GenerateBricks()
    {
        float screenWidth, screenHeight, scaleMultiplier;
        float blockWidth = brickPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        float blockHeight = brickPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
        GetBlocksInformation(blockWidth, out screenWidth, out screenHeight, in maxCols, out scaleMultiplier);
        this.RemainingBricks = new List<Brick>();
        int[,] currentLevelData = this.LevelsData[this.CurrentLevel];
        for (int row = 0; row < this.maxRows; row++)
        {
            for (int col = 0; col < this.maxCols; col++)
            {
                int brickType = currentLevelData[row, col];
                if (brickType > 0)
                {
                    Brick newBrick = Instantiate(brickPrefab);
                    newBrick.transform.position = new Vector3(
                        -screenWidth / 2 + (blockWidth * scaleMultiplier * col),
                        screenHeight / 2 - (blockHeight * row),
                        0);                  
                    newBrick.transform.localScale = new Vector3(
                        newBrick.transform.localScale.x * scaleMultiplier,
                        newBrick.transform.localScale.y,
                        1);   
                    newBrick.Init(this.Sprites[brickType - 1], this.BrickColors[brickType], brickType);
                    this.RemainingBricks.Add(newBrick);
                }
            }
        }
        this.InitialBricksCount = this.RemainingBricks.Count;
    }

    private List<int[,]> LoadLevelData()
    {
        TextAsset text = Resources.Load("levels") as TextAsset;
        string[] rows = text.text.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        List<int[,]> levelsData = new List<int[,]>();
        int[,] currentLevel = new int[maxRows, maxCols];
        int currentRow = 0;
        for (int row = 0; row < rows.Length; row++)
        {
            string line = rows[row];
            if (line.IndexOf("--") == -1)
            {
                string[] bricks = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int col = 0; col < bricks.Length; col++)
                {
                    currentLevel[currentRow, col] = int.Parse(bricks[col]);
                }
                currentRow++;
            }else
            {
                currentRow = 0;
                levelsData.Add(currentLevel);
                currentLevel = new int[maxRows, maxCols];
            }
        }
        return levelsData;
    }

    public void LoadLevel(int level)
    {
        this.CurrentLevel = level;
        this.ClearRemainingBricks();
        this.GenerateBricks();
    }

    private void ClearRemainingBricks()
    {
        foreach (Brick brick in this.RemainingBricks)
        {
            Destroy(brick.gameObject);
        }
    }

    public void LoadNextLevel()
    {
        this.CurrentLevel++;
        if (this.CurrentLevel >= this.LevelsData.Count)
        {
            this.CurrentLevel = 0;
            this.LoadLevel(this.CurrentLevel);
        } 
        else
        {
            this.LoadLevel(this.CurrentLevel);
        }
    }
}