using Mono.Cecil;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        foreach (var player in players)
        {
            // 手札情報（Top/Middle/Bottom）をリセット
            player.ResetHand();
        }

        DealCards();
    }

    private void DealCards()
    {
        // カードを新しく生成
        List<Card> dealtCards = new List<Card>();
        for (int i = 0; i < GetDrawCardsNum(currentPhase); i++)
        {
            dealtCards.Add(deck.Draw());
        }
        
        // 配られたカードを UI の UnplacedArea に並べる
        playerHandUI.RenderHand(dealtCards);
    }

    public int GetDrawCardsNum(PlacePhase round)
    {
        return (round == PlacePhase.First) ? 5 : 3;
    }

    public PlacePhase GetCurrentPhase() { return currentPhase; }

    public void OnClickPlaceButton()
    {
        Debug.Log("End " + currentPhase);
        playerHandUI.confirmButton.interactable = false;
        if (currentPhase == PlacePhase.Fifth)
        {

        }
        else SetupNextPhase();
    }

    private void SetupNextPhase()
    {
        // 配置したカードをロック
        playerHandUI.topArea.GetComponentInChildren<DropArea>().LockCards();
        playerHandUI.middleArea.GetComponentInChildren<DropArea>().LockCards();
        playerHandUI.bottomArea.GetComponentInChildren<DropArea>().LockCards();
        playerHandUI.unplacedArea.GetComponentInChildren<DropArea>().ClearCards();

        currentPhase++;
        DealCards();
    }
}
