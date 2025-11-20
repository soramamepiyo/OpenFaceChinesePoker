using Mono.Cecil;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public enum PlacePhase
{
    First,  // 5
    Second, // 3
    Third,  // 3
    Fourth, // 3
    Fifth   // 5
}

public class HandManager : MonoBehaviour
{
    private List<Player> players;
    private Deck deck;
    [SerializeField] private PlacePhase currentPhase;
    [SerializeField] private HandUI playerHandUI;

    public void Init(List<Player> gamePlayers)
    {
        Debug.Log("Starting Open Face Chinese Poker...");

        players = gamePlayers;
        deck = new Deck();
        currentPhase = PlacePhase.First;

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

            // カードを新しく生成
            List<Card> dealtCards = new List<Card>();
            for (int i = 0; i < GetDrawCardsNum(currentPhase); i++)
            {
                dealtCards.Add(deck.Draw());
            }
        
            // 配られたカードを UI の UnplacedArea に並べる
            playerHandUI.RenderInitialHand(dealtCards);
        }
    }

    public int GetDrawCardsNum(PlacePhase round)
    {
        return (round == PlacePhase.First) ? 5 : 3;
    }

    public PlacePhase GetCurrentPhase() { return currentPhase; }
}
