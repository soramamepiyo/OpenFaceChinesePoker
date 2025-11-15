using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Player> players = new List<Player>();
    [SerializeField] private HandUI playerHandUI;
    private Deck deck;

    private int currentPlayerIndex = 0;
    private bool gameStarted = false;

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        Debug.Log("Starting Open Face Chinese Poker...");

        deck = new Deck();

        players.Clear();
        players.Add(new Player("Player"));
        // todo: 一旦CPUはコメントアウト
        // players.Add(new Player("CPU", true));

        DealInitialCards();
        gameStarted = true;
    }

    private void DealInitialCards()
    {
        foreach (var player in players)
        {
            // 手札情報（Top/Middle/Bottom）をリセット
            player.ResetHand();

            // 5枚のカードを新しく生成（テスト用）
            List<Card> dealtCards = new List<Card>
        {
            new Card(SuitType.Club, RankType.Two),
            new Card(SuitType.Spade, RankType.Five),
            new Card(SuitType.Heart, RankType.Ace),
            new Card(SuitType.Diamond, RankType.Jack),
            new Card(SuitType.Joker, RankType.Joker)
        };

            // 配られたカードを UI の UnplacedArea に並べる
            playerHandUI.RenderInitialHand(dealtCards);
        }
    }

    public void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        Debug.Log($"Next turn: {players[currentPlayerIndex].playerName}");
    }
}