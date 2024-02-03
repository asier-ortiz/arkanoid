using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Firestore;
using Firebase;
using System.Linq;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance => _instance;
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

    // https://github.com/firebase/quickstart-unity/issues/416

    private FirebaseFirestore db;
    private Dictionary<string, Player> LeaderBoard;
    public GameObject gameOverScreen;
    public GameObject victoryScreen;
    public GameObject mainScreen;
    public GameObject scoresScreen;
    public int AvailableLives = 3;
    public event Action<int> OnLiveLost;
    public event Action<int> OnLeaderBoardEntered;
    public event Action<List<Player>> OnScoresScreenActivated;
    public int LeaderBoardPossition { get; set; }
    private string LeaderBoardId { get; set; }
    public static bool IsPortrait { get; set; } = true;
    public bool isGameStarted { get; set; }
    public int Lives { get; set; }

    private void Start()
    {
        this.Lives = this.AvailableLives;
        Ball.OnBallDeath += OnBallDeath;
        Brick.OnBrickDestruction += OnBrickDestruction;
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                db = FirebaseFirestore.GetInstance(FirebaseApp.DefaultInstance);
                CollectionReference rankingRef = db.Collection("rankings");
                Query query = rankingRef.OrderBy("score").Limit(10);
                ListenerRegistration listener = query.Listen(snapshot =>
                {
                    LeaderBoard = new Dictionary<string, Player>();
                    foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
                    {
                        Dictionary<string, object> TopPlayer = documentSnapshot.ToDictionary();
                        String nick = $"{TopPlayer["nick"]}";
                        String score = $"{TopPlayer["score"]}";
                        String id = documentSnapshot.Id;
                        Player player = new Player(nick, int.Parse(score));
                        if (!LeaderBoard.ContainsKey(id))
                        {
                            LeaderBoard.Add(id, player);
                        }
                    }
                });
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                LeaderBoard = new Dictionary<string, Player>();
            }
        });   
    }

    private void Update()
    {
        if (IsPortrait)
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
        else
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }

    public void ChangeScreenOrientation()
    {
        IsPortrait = !IsPortrait;
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowMainScreen()
    {
        scoresScreen.SetActive(false);
        mainScreen.SetActive(true);
    }

    public void ShowVictoryScreen()
    {
        victoryScreen.SetActive(true);
    }

    public void ShowScoresScreen()
    {
        List<Player> players = this.LeaderBoard.Values.ToList();
        players.Sort();
        players.Reverse();
        OnScoresScreenActivated?.Invoke(players);
        mainScreen.SetActive(false);
        scoresScreen.SetActive(true);
    }

    public void StartNewGame()
    {
        mainScreen.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private bool PlayerEnteredLeaderBoard()
    {
        int Score = GameUIManager.Instance.Score;
        if (Score > 0)
        {
            this.LeaderBoardPossition = LeaderBoard.Count + 1;
            LeaderBoardId = "";
            if (LeaderBoard.Count == 0 || LeaderBoard.Count < 10)
            {
                return true;
            }
            else
            {
                bool first = true;
                foreach (KeyValuePair<string, Player> playerRecord in LeaderBoard)
                {
                    if (Score > playerRecord.Value.score)
                    {
                        this.LeaderBoardPossition--;
                        if (first)
                        {
                            LeaderBoardId = playerRecord.Key;
                            first = false;
                        }
                    }
                }
                if (LeaderBoardId.Length > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SaveLeaderBoardScore()
    {
        int Score = GameUIManager.Instance.Score;
        string nick = GameUIManager.Instance.NickInputText.text;
        if (LeaderBoardId.Length > 0)
        {
            DocumentReference docRef = db.Collection("rankings").Document(LeaderBoardId);
            docRef.UpdateAsync("nick", nick); 
            docRef.UpdateAsync("score", Score);
        }
        else
        {
            DocumentReference docRef = db.Collection("rankings").Document();
            Dictionary<string, object> TopPlayer = new Dictionary<string, object>
                {
                    { "nick", nick},
                    { "score", Score }
                };
            docRef.SetAsync(TopPlayer);
        }
        GameUIManager.Instance.NickInputText.text = "";
        SceneManager.LoadScene("Main Menu");

    }

    private void OnBrickDestruction(Brick brick)
    {
        if (BricksManager.Instance.RemainingBricks.Count <= 0)
        {
            BallsManager.Instance.ResetBalls();
            GameManager.Instance.isGameStarted = false;
            BricksManager.Instance.LoadNextLevel();
            if (BallsManager.Instance.initialBallSpeed < 350)
            {
                BallsManager.Instance.initialBallSpeed += 10;
            }
        }
    }

    private void OnBallDeath(Ball ball)
    {
        this.Lives--;
        if (this.Lives < 1)
        {
            OnLiveLost?.Invoke(this.Lives);
            if (PlayerEnteredLeaderBoard())
            {
                OnLeaderBoardEntered?.Invoke(this.LeaderBoardPossition);
                victoryScreen.SetActive(true);
                Handheld.Vibrate();
            } else
            {
                gameOverScreen.SetActive(true);
            }
        }
        else
        {
            OnLiveLost?.Invoke(this.Lives);
            BallsManager.Instance.ResetBalls();
            isGameStarted = false;
        }
    }

    private void OnDisable()
    {
        Ball.OnBallDeath -= OnBallDeath;
    }
}