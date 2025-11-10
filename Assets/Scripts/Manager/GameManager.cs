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
        playerHandUI.RenderHand(players[0].hand);
    }

    public void StartGame()
    {
        Debug.Log("Starting Open Face Chinese Poker...");

        deck = new Deck();

        players.Clear();
        players.Add(new Player("Player"));
        players.Add(new Player("CPU", true));

        DealInitialCards();
        gameStarted = true;
    }

    private void DealInitialCards()
    {
        foreach (var player in players)
        {
            player.ResetHand();
            for (int i = 0; i < 5; i++)
            {
                var card = deck.Draw();
                player.hand.AddCard(card, RowType.Back); // ‚Æ‚è‚ ‚¦‚¸Back‚É‘S•”
            }
            Debug.Log($"{player.playerName} initial hand: {player.hand}");
        }
    }

    public void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        Debug.Log($"Next turn: {players[currentPlayerIndex].playerName}");
    }
}