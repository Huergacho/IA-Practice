using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField]private int itemsPicked;
    [SerializeField] private int maxItems;
    [SerializeField] private PlayerModel _playerModel;
    [SerializeField] private TextMeshProUGUI tmPro;
    public PlayerModel PlayerModel => _playerModel;
    private bool isPlayerAlive;
    public bool IsPlayerAlive => isPlayerAlive;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        isPlayerAlive = true;
        DontDestroyOnLoad(this);
    }
    private void Update()
    {
        UiItems();
        if(maxItems <= itemsPicked)
        {
            WinGame();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    public void SetMaxItems(int maxItemsToSet)
    {
        maxItems = maxItemsToSet;
    }
    public void AddItem()
    {
        itemsPicked++;
    }
    private void WinGame()
    {
        SceneManager.LoadScene(1);
    }
    public void PlayerIsDead()
    {
        isPlayerAlive = false;
        LostScene();
    }
    private void LostScene()
    {
        SceneManager.LoadScene(0);
    }
    public void AssignPlayer(PlayerModel player)
    {
        _playerModel = player;
    }
    private void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
    public void UiItems()
    {
        tmPro.text = itemsPicked.ToString() + " / " + maxItems.ToString();
    }
}
