using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class HandManager : MonoBehaviour
{
    [SerializeField] private HandUI playerHandUI;
    private Deck deck;
    private List<Player> players;

    public void Init(List<Player> gamePlayers)
    {
        Debug.Log("Starting Open Face Chinese Poker...");

        players = gamePlayers;
        deck = new Deck();

        players.Clear();
        players.Add(new Player("Player"));
        // todo: 一旦CPUはコメントアウト
        // players.Add(new Player("CPU", true));

        DealInitialCards();
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
}
