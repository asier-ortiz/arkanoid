using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    #region Singleton
    private static MenuUIManager _instance;
    public static MenuUIManager Instance => _instance;
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

    public Text[] NicksText;
    public Text[] ScoresText;

    void Start()
    {
        GameManager.Instance.OnScoresScreenActivated += OnScoresScreenActivated;
    }

    private void OnScoresScreenActivated(List<Player> players)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(players[i].nick) 
                || string.IsNullOrEmpty(players[i].nick) 
                || players[i].nick.Trim().Length == 0 
                || players[i].nick.Trim() == "" 
                || players[i].nick == null) // No hay manera humana de hacer funcionar a esto...
            {
                NicksText[9 - i].text = "- - -";
            }else
            {
                NicksText[9 - i].text = players[i].nick;
            }
            ScoresText[9 - i].text = players[i].score.ToString();
        }
    }
}