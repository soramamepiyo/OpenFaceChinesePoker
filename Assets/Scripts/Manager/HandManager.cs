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
    [SerializeField] private int firstPlayerIndex;   // 0 or 1
    [SerializeField] private int currentPlayerIndex;   // 0 or 1
    [SerializeField] private HandUI playerHandUI;
    [SerializeField] private HandUI enemyHandUI;
    [SerializeField] private HandEvaluator evaluator;
    [SerializeField] private OFCScoreCalculator scoreCalculator;
    [SerializeField] private OFCCpu ofcCpu;
    [SerializeField] private GameObject resultPanel;

    public void Init(List<Player> gamePlayers, bool isEnemyFirst)
    {
        Debug.Log("Starting Open Face Chinese Poker...");

        firstPlayerIndex = isEnemyFirst ? 1 : 0;
        currentPlayerIndex = firstPlayerIndex;

        players = gamePlayers;
        deck = new Deck();
        currentPhase = PlacePhase.First;
        resultPanel.SetActive(false);
        playerHandUI.confirmButton.interactable = false;

        players.Clear();
        players.Add(new Player("Player"));
        players.Add(new Player("CPU", true));

        foreach (var player in players)
        {
            // 手札情報（Top/Middle/Bottom）をリセット
            player.ResetHand();
        }

        startPhase();
    }

    private void DealCards()
    {
        // カードを新しく生成
        List<Card> playerDealtCards = new List<Card>();
        for (int i = 0; i < GetDrawCardsNum(currentPhase); i++)
        {
            playerDealtCards.Add(deck.Draw());
        }
        // 配られたカードを UI の UnplacedArea に並べる
        playerHandUI.RenderHand(playerDealtCards);

        // 同じ処理の繰り返し(Enemy)
        List<Card> enemyDealtCards = new List<Card>();
        for (int i = 0; i < GetDrawCardsNum(currentPhase); i++)
        {
            enemyDealtCards.Add(deck.Draw());
        }
        enemyHandUI.RenderHand(enemyDealtCards);
    }

    public int GetDrawCardsNum(PlacePhase round)
    {
        return (round == PlacePhase.First) ? 5 : 3;
    }

    public PlacePhase GetCurrentPhase() { return currentPhase; }

    public void OnClickPlaceButton()
    {
        playerHandUI.confirmButton.interactable = false;
        EndPhase();
    }

    private void startPhase()
    {
        DealCards();

        if (currentPlayerIndex == 0) startPlayerTurn();
    }

    private void startPlayerTurn()
    {
        playerHandUI.confirmButton.interactable = true;
    }

    public void EndPhase()
    {
        Debug.Log("End " + currentPhase);

        // 手番交代？フェーズ終了？
        if (currentPlayerIndex == firstPlayerIndex)
        {
            // 手番交代
            if (currentPlayerIndex == 0)
            {
                currentPlayerIndex = 1;
                StartCoroutine(ofcCpu.ActionCpu(this));
            }
            else
            {
                currentPlayerIndex = 0;
                // プレイヤーを操作可能に
                startPlayerTurn();
            }
        }
        else
        {
            if (currentPhase == PlacePhase.Fifth) DispResult();
            else StartCoroutine(SetupNextPhase());
        }
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
        startPhase();
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
