using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static HandEvaluator;
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
    [SerializeField] private HandEvaluator evaluator;
    [SerializeField] private OFCScoreCalculator scoreCalculator;
    [SerializeField] private GameObject resultPanel;

    public void Init(List<Player> gamePlayers)
    {
        Debug.Log("Starting Open Face Chinese Poker...");

        players = gamePlayers;
        deck = new Deck();
        currentPhase = PlacePhase.First;
        resultPanel.SetActive(false);

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
        if (currentPhase == PlacePhase.Fifth) DispResult();
        else StartCoroutine(SetupNextPhase());
    }

    private IEnumerator SetupNextPhase()
    {
        // 配置したカードをロック
        playerHandUI.topArea.GetComponentInChildren<DropArea>().LockCards();
        playerHandUI.middleArea.GetComponentInChildren<DropArea>().LockCards();
        playerHandUI.bottomArea.GetComponentInChildren<DropArea>().LockCards();
        playerHandUI.unplacedArea.GetComponentInChildren<DropArea>().ClearCards();

        yield return null;  // UnplacedAreaの残ったカードの破棄のために、1フレーム待機する(Destroy()が即時廃棄しない)

        currentPhase++;
        DealCards();
    }

    private void DispResult()
    {
        resultPanel.SetActive(true);
        ResultPanelUI rp_ui = resultPanel.GetComponent<ResultPanelUI>();
       
        Dictionary<AreaType, List<Card>> placed_cards = new Dictionary<AreaType, List<Card>>();
        placed_cards.Add(AreaType.Top,    playerHandUI.topArea.GetComponentInChildren<DropArea>().GetCards());
        placed_cards.Add(AreaType.Middle, playerHandUI.middleArea.GetComponentInChildren<DropArea>().GetCards());
        placed_cards.Add(AreaType.Bottom, playerHandUI.bottomArea.GetComponentInChildren<DropArea>().GetCards());
        
        Dictionary<AreaType, EvaluationResult> result = evaluator.Evaluate(placed_cards);
        int top_point = scoreCalculator.GetAreaScore(result[AreaType.Top],    AreaType.Top);
        int mid_point = scoreCalculator.GetAreaScore(result[AreaType.Middle], AreaType.Middle);
        int btm_point = scoreCalculator.GetAreaScore(result[AreaType.Bottom], AreaType.Bottom);

        Debug.Log("T: " + top_point);
        Debug.Log("M: " + mid_point);
        Debug.Log("B: " + btm_point);

        rp_ui.SetCards(placed_cards[AreaType.Top], placed_cards[AreaType.Middle], placed_cards[AreaType.Bottom]);
        rp_ui.SetText(result[AreaType.Top], result[AreaType.Middle], result[AreaType.Bottom], 
            top_point, mid_point, btm_point);
    }
}
