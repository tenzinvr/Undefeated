using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public Queue<PointInTime> pointsToEvaluate = new Queue<PointInTime>();
    private PointInTime[] allPoints = new PointInTime[36];
    [System.NonSerialized] public PointInTime currentPoint;
    private int pointIndex = 0;
    private Player player1;
    private Player player2;
    //private PlayerPreview playerPreview1;
    //private PlayerPreview playerPreview2;
    public PlayerType playersTurn = PlayerType.Player;
    private Queue<Action> turnActions = new Queue<Action>();

    [SerializeField] private Transform discardPile;

    [SerializeField] private PlayerStatsSO playerStats;
    [SerializeField] private PlayerStatsSO opponentStats;

    [SerializeField] private HealthManager playerHealth;
    [SerializeField] private HealthManager opponentHealth;
    [SerializeField] private GameObject critTxt;


    private TimelineManager timelineManager;
    private DeckManager deckManagerPlayer;
    private DeckManager deckManagerOpponent;
    [SerializeField] private OpponentBehaviour opponentBehaviour;

    // [System.NonSerialized] public int[] positions;

    [System.NonSerialized] public Dictionary<(string, string), Effect> attackDefenceEffects = new Dictionary<(string, string), Effect>()
    {        
        // Blocked
        { ("Jab", "Block"), Effect.Blocked },
        { ("Cross", "Block"), Effect.Blocked },
        { ("LeadHook", "Block"), Effect.Blocked },
        { ("RearHook", "Block"), Effect.Blocked },
        { ("LeadUppercut", "Block"), Effect.Blocked },
        { ("RearUppercut", "Block"), Effect.Blocked },
        // Missed
        { ("Jab", "Slip"), Effect.Missed },
        { ("Cross", "Slip"), Effect.Missed },
        { ("LeadHook", "Bob"), Effect.Missed },
        { ("RearHook", "Bob"), Effect.Missed },
        { ("LeadUppercut", "Slip"), Effect.Missed },
        { ("RearUppercut", "Slip"), Effect.Missed },
        // No defence
        { ("Jab", "None"), Effect.Hit },
        { ("Cross", "None"), Effect.Hit },
        { ("LeadHook", "None"), Effect.Hit },
        { ("RearHook", "None"), Effect.Hit },
        { ("LeadUppercut", "None"), Effect.Hit },
        { ("RearUppercut", "None"), Effect.Hit },
        // Wrong Defence 
        { ("Jab", "Bob"), Effect.Hit },
        { ("Cross", "Bob"), Effect.Hit },
        { ("LeadHook", "Slip"), Effect.Hit },
        { ("RearHook", "Slip"), Effect.Hit },
        { ("LeadUppercut", "Bob"), Effect.Hit },
        { ("RearUppercut", "Bob"), Effect.Hit },
    };

    [System.NonSerialized] public Dictionary<(string, string), bool> actionSynergy = new Dictionary<(string, string), bool>()
    {
        //Jab
        { ("Jab", "Jab"), false },
        { ("Jab", "Cross"), true },
        { ("Jab", "Lead Hook"), false },
        { ("Jab", "Rear Hook"), true },
        { ("Jab", "Lead Uppercut"), false },
        { ("Jab", "Rear Uppercut"), true },
        //Cross
        { ("Cross", "Jab"), true },
        { ("Cross", "Cross"), false },
        { ("Cross", "Lead Hook"), true },
        { ("Cross", "Rear Hook"), false },
        { ("Cross", "Lead Uppercut"), true },
        { ("Cross", "Rear Uppercut"), false },
        //Lead Hook
        { ("Lead Hook", "Jab"), false },
        { ("Lead Hook", "Cross"), true },
        { ("Lead Hook", "Lead Hook"), false },
        { ("Lead Hook", "Rear Hook"), true },
        { ("Lead Hook", "Lead Uppercut"), false },
        { ("Lead Hook", "Rear Uppercut"), true },
        //Rear Hook
        { ("Rear Hook", "Jab"), true },
        { ("Rear Hook", "Cross"), false },
        { ("Rear Hook", "Lead Hook"), true },
        { ("Rear Hook", "Rear Hook"), false },
        { ("Rear Hook", "Lead Uppercut"), true },
        { ("Rear Hook", "Rear Uppercut"), false },
        //Lead Uppercut
        { ("Lead Uppercut", "Jab"), false },
        { ("Lead Uppercut", "Cross"), true },
        { ("Lead Uppercut", "Lead Hook"), false },
        { ("Lead Uppercut", "Rear Hook"), true },
        { ("Lead Uppercut", "Lead Uppercut"), false },
        { ("Lead Uppercut", "Rear Uppercut"), true },
        //Rear Uppercut
        { ("Rear Uppercut", "Jab"), true },
        { ("Rear Uppercut", "Cross"), false },
        { ("Rear Uppercut", "Lead Hook"), true },
        { ("Rear Uppercut", "Rear Hook"), false },
        { ("Rear Uppercut", "Lead Uppercut"), true },
        { ("Rear Uppercut", "Rear Uppercut"), false },
    };

    private Dictionary<(Action[], Action), bool> attackCombos = new Dictionary<(Action[], Action), bool>()
    {

    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timelineManager = GetComponent<TimelineManager>();
        player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player>();
        player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<Player>();
        deckManagerPlayer = player1.GetComponentInChildren<DeckManager>();
        deckManagerOpponent = player2.GetComponentInChildren<DeckManager>();
        // positions = new int[20];
        // for (int i =  0; i < positions.Length; i++)
        // {
        //     positions[i] = -200 + i * 20;
        //     //Debug.Log(positions[i]);
        // }
        // player2.transform.localPosition = new Vector3(positions[14], player1.transform.localPosition.y, player1.transform.localPosition.z);

        // player1.position = 5;
        // player2.position = 15;

        //player1.SetPosition(player1.position);
    }

    public void InstantiatePoints(PointInTime[] newPoints)
    {

        foreach (PointInTime point in newPoints)
        {
            //Debug.Log(target == null);
            if (point != null)
            {
                point.index = pointIndex;
                point.time = pointIndex * 50;
                allPoints[pointIndex++] = point;
            }
        }
        currentPoint = allPoints[0];
    }

    public void IncreasePointsInTime(PointInTime[] newPoints)
    {
        //Debug.Log("increase points in time");
        int lastIndex = pointIndex;
        Array.Resize<PointInTime>(ref allPoints, allPoints.Length + newPoints.Length);
        foreach (PointInTime point in newPoints)
        {
            if (point != null)
            {
                point.index = pointIndex;
                point.time = pointIndex * 50;
                allPoints[pointIndex++] = point;
            }
        }
    }

    public PointInTime GetActionPointInTime(Action action)
    {
        int lastActionIndex = GetLastActiveIndex(action.playerType);
        Action previousAction = GetLastAction(action.playerType);
        if (!DoActionsSynergise(previousAction, action))
        {
            lastActionIndex += (int)(previousAction.returnTime / 50);
        }
        return allPoints[lastActionIndex];
    }

    private Action GetLastAction(PlayerType player)
    {
        Action action = new Action(player);
        if (player == PlayerType.Player)
        {
            for (int i = 0; i < allPoints.Length; i++)
            {
                if (allPoints[i].playerState != PlayerState.Null)
                {
                    //Debug.Log("Point at " + allPoints[i].time + " is " + allPoints[i].playerState);
                    action = allPoints[i].playerAction;
                }
            }
        }
        else
        {
            for (int i = 0; i < allPoints.Length; i++)
            {

                if (allPoints[i].opponentState != PlayerState.Null)
                {
                    //Debug.Log("Point at " + allPoints[i].time + " is " + allPoints[i].opponentState);
                    action = allPoints[i].opponentAction;
                }
            }
        }
        if (action == null) Debug.Log("Cant find last action");
        //Debug.Log("Last action for player " + player.ToString() + " is " + action.playerState);
        return action;
    }

    public int GetLastActiveIndex(PlayerType player)
    {
        int lastIndex = 0;
        if (player == PlayerType.Player)
        {
            for (int i = 0; i < allPoints.Length; i++)
            {
                //Debug.Log("Point at " + allPoints[i].time + " is " + allPoints[i].playerState);
                if (allPoints[i].playerState != PlayerState.Null && allPoints[i].playerState != PlayerState.Returning)
                {
                    lastIndex = i;
                }
            }
            //Debug.Log("Last active index for player " + playersTurn.ToString() + " = " + lastIndex + "\nState is: " + allPoints[lastIndex].playerState);
        }
        else
        {
            for (int i = 0; i < allPoints.Length; i++)
            {
                if (allPoints[i].opponentState != PlayerState.Null && allPoints[i].opponentState != PlayerState.Returning)
                {
                    lastIndex = i;
                }
            }
            //Debug.Log("Last active index for player " + playersTurn.ToString() + " = " + lastIndex + "\nState is: " + allPoints[lastIndex].playerState);
        }
        return lastIndex;
    }

    private PointInTime GetNextEventPoint(int initialIndex)
    {
        for (int i = initialIndex; i < allPoints.Length; i++)
        {
            if (allPoints[i].playerEvent) return allPoints[i];
            if (allPoints[i].opponentEvent) return allPoints[i];
        }
        return null;
    }

    public PointInTime GetNextPointInTime(PointInTime point)
    {
        if (point.index < allPoints.Length - 1) return allPoints[point.index + 1];
        return null;
    }

    public PointInTime GetPreviousPointInTime(PointInTime point)
    {
        if (point.index >= 1) return allPoints[point.index - 1];
        return null;
    }

    public Player GetCurrentPlayer()
    {
        return (playersTurn == PlayerType.Player) ? player1 : player2;
    }

    public PlayerType GetCurrentPlayerType()
    {
        return playersTurn;
    }

    public PointInTime GetNextActivePointForPlayer(PlayerType player)
    {
        if (player == PlayerType.Player)
        {
            for (int i = currentPoint.index; i < allPoints.Length; i++)
            {
                if (allPoints[i].playerEvent) return allPoints[i];
            }
        }
        else
        {
            for (int i = currentPoint.index; i < allPoints.Length; i++)
            {
                if (allPoints[i].opponentEvent) return allPoints[i];
            }
        }
        return null;
    }

    public void AddActionToSpecificPoint(Action action, int index)
    {
        AddActionToTurn(action);
        ChangeActionPointInTime(action, allPoints[action.initialIndex], allPoints[index]);
    }

    private void AddActionToPointsInTime(Action action, int initialIndex, int indicesActive, int indicesReturning)
    {
        //Debug.Log("Add action to point in time from " + initialIndex + " to " + (initialIndex + indicesActive));
        int windUpIndices = initialIndex + indicesActive;
        if (action.playerType == PlayerType.Player) { allPoints[initialIndex + 1].playerAnimationStarted = true; }
        else { allPoints[initialIndex + 1].opponentAnimationStarted = true; }
        allPoints[initialIndex].actionStart = true;
        for (int i = initialIndex + 1; i <= windUpIndices; i++)
        {
            //Debug.Log("Adding " + action.playerState + " to index " + i);
            allPoints[i].ClearPoint(action.playerType);
            allPoints[i].AddToPoint(action, action.playerType);
        }

        if (action.type == CardType.Attack)
        {
            if (action.playerType == PlayerType.Player) allPoints[windUpIndices].playerEvent = true;
            else allPoints[windUpIndices].opponentEvent = true;
            //Debug.Log("Attack event at index " + windUpIndices);
        }
        else if (action.type == CardType.Special)
        {
            //Debug.Log("Special action " + action.name);
            //allPoints[initialIndex + indicesActive].playerAttackFinished = true;
            if (action.name == "Feint")
            {
                //Debug.Log("feint added, event at " + (initialIndex + indicesActive));
                if (action.playerType == PlayerType.Player) allPoints[initialIndex + indicesActive].playerEvent = true;
                else allPoints[initialIndex + indicesActive].opponentEvent = true;
            }
        }
        else if (action.type == CardType.Breathing)
        {
            if (action.playerType == PlayerType.Player)
            {
                allPoints[initialIndex + indicesActive].playerEvent = true;
            }
            else
            {
                allPoints[initialIndex + indicesActive].opponentEvent = true;
            }
        }

        int numReturnIndices = windUpIndices + indicesReturning;
        for (int i = windUpIndices + 1; i <= numReturnIndices; i++)
        {
            //Debug.Log("Adding " + action.playerState + " return time to index " + i);
            allPoints[i].AddToPoint(action, action.playerType, PlayerState.Returning);
        }
    }

    public void ChangeActionPointInTime(Action action, PointInTime prevPoint, PointInTime newPoint)
    {
        timelineManager.MoveIcon(action, newPoint);
        RemoveActionFromPointsInTime(action.playerType, prevPoint);
        action.initialTime = allPoints[newPoint.index].time;
        action.timeOfEffect = allPoints[newPoint.index].time + action.windUpTime;
        int indicesActive = (int)(action.windUpTime / 50);
        int indicesReturning = (int)(action.returnTime / 50);
        if (newPoint.index >= GetCurrentPlayer().lastPointEvaluated)
        {
            for (int i = GetCurrentPlayer().lastPointEvaluated; i < newPoint.index; i++)
            {
                Action idleAction = new Action(GetCurrentPlayerType(), PlayerState.Idle, 0);
                AddActionToPointsInTime(idleAction, i, 1, 0);
            }
        }
        if (newPoint.index + indicesActive + indicesReturning >= allPoints.Length - 10) { timelineManager.IncreaseTimeline(); }
        action.icon.SetActive(true);
        AddActionToPointsInTime(action, newPoint.index, indicesActive, indicesReturning);
    }

    private void RemoveActionFromPointsInTime(PlayerType player, PointInTime point)
    {
        int first = point.index;
        Action nullAction = new Action(player);
        //Action action = GetCurrentPlayerType() == player ? point.playerAction : point.opponentAction;
        //List<Action> removedActions = new List<Action>();
        if (player == PlayerType.Player)
        {
            //timelineManager.CancelAction(point.playerAction);
            for (int i = first; i < allPoints.Length; i++)
            {
                //timelineManager.RemoveIcon(player, allPoints[i]);
                allPoints[i].AddToPoint(nullAction, player);
                //allPoints[i].playerAttackFinished = false;
                allPoints[i].playerEvent = false;
                allPoints[i].playerAnimationStarted = false;
            }
        }
        else
        {
            for (int i = first; i < allPoints.Length; i++)
            {
                //timelineManager.RemoveIcon(player, allPoints[i]);
                allPoints[i].AddToPoint(nullAction, player);
                //allPoints[i].opponentAttackFinished = false;
                allPoints[i].opponentEvent = false;
                allPoints[i].opponentAnimationStarted = false;
            }
        }
    }

    private void RemoveIconsFromPointsInTime(PlayerType playerType, PointInTime point)
    {
        if (playerType == PlayerType.Player)
        {
            for (int i = point.index; i < allPoints.Length; i++)
            {
                timelineManager.RemoveIcon(playerType, allPoints[i]);
            }
        }
        else
        {
            for (int i = point.index; i < allPoints.Length; i++)
            {
                timelineManager.RemoveIcon(playerType, allPoints[i]);
            }
        }
    }

    private bool DoActionsSynergise(Action firstAction, Action secondAction)
    {
        //Debug.Log("Do " +  firstAction.name + " and " + secondAction.name + " combo? ");
        if (actionSynergy.TryGetValue((firstAction.name, secondAction.name), out var combo))
        {
            //Debug.Log(combo);
            return combo;
        }
        //Debug.Log("Couldnt get attack combo value");
        return false;

    }

    // private List<Action> GetAvailableCombos()
    // {
    //     //Debug.Log("Do " +  firstAction.name + " and " + secondAction.name + " combo? ");
    //     Queue<Action> currentCombo = GetCurrentPlayer().currentCombo;
    //     List<Queue<Action>> playerCombos = GetCurrentPlayer().combos;
    //     bool doesCombo
    //     foreach (Queue<Action> combo in playerCombos)
    //     {
    //         foreach (Action action in currentCombo)
    //         {
    //             if (combo.Dequeue().name != action.name) break;
    //         }
    //     }
    //     return 
    // }

    public void AddActionToTurn(Action action)
    {
        if (action.name == "Feint")
        {
            bool prevAttackFound = false;
            Action prevAttack = new Action(action.playerType);
            foreach (Action prevAction in turnActions)
            {
                if (prevAction is AttackAction attackAction)
                {
                    //Debug.Log("Prev action is " + attackAction.name);
                    prevAttackFound = true;
                    prevAttack = attackAction;
                }
            }
            if (prevAttackFound)
            {
                Debug.Log("Adding feint to attack " + prevAttack.name + " at index " + prevAttack.initialIndex);
                timelineManager.AddFeintIcon(playersTurn, allPoints[prevAttack.initialIndex], action);
                TimelineActionIcon timelineActionIcon = prevAttack.icon.GetComponent<TimelineActionIcon>();
                if (timelineActionIcon != null) timelineActionIcon.ActionFeinted();
                RemoveActionFromPointsInTime(GetCurrentPlayerType(), allPoints[prevAttack.initialIndex]);
                AddActionToPointsInTime(action, prevAttack.initialIndex, action.windUpTime / 50, 0);
                DeckManager deckManager = GetCurrentPlayerType() == PlayerType.Player ? deckManagerPlayer : deckManagerOpponent;
                deckManager.feintedAction = prevAttack;
            }
            else { Debug.Log("No attack to feint"); }
            return;
        }
        int lastActionIndex = GetLastActiveIndex(action.playerType);
        //Debug.Log("Add action to turn, all points length = " + allPoints.Length);
        if (allPoints.Length - 10 <= lastActionIndex) timelineManager.IncreaseTimeline();
        Action previousAction = GetLastAction(action.playerType);
        if (!DoActionsSynergise(previousAction, action))
        {
            lastActionIndex += (int)(previousAction.returnTime / 50);
        }
        //Debug.Log("Adding " + action.name + " to index " + lastActionIndex);
        action.initialTime = allPoints[lastActionIndex].time;
        action.timeOfEffect = allPoints[lastActionIndex].time + action.windUpTime;
        action.initialIndex = lastActionIndex;
        int indicesActive = (int)(action.windUpTime / 50);
        int indicesReturning = (int)(action.returnTime / 50);
        //Debug.Log("Adding action, end indice = " + (lastActionIndex + indicesActive + indicesReturning) + "\nAll points length = " + allPoints.Length);
        if (lastActionIndex + indicesActive + indicesReturning >= allPoints.Length - 10) { timelineManager.IncreaseTimeline(); }
        AddToTurnList(action);
        //Debug.Log("Indices active = " + indicesActive);
        if (action.type == CardType.Breathing)
        {
            // //Debug.Log("Add breathing from " + lastActionIndex + " for " + indicesActive);
            // GetCurrentPlayer().Breathe(action.windUpTime / 50);
            timelineManager.AddBreatheIcon(playersTurn, allPoints[lastActionIndex], action);
        }
        else
        {
            action.icon = timelineManager.AddActionIcon(allPoints[lastActionIndex], action);
        }
        AddActionToPointsInTime(action, lastActionIndex, indicesActive, indicesReturning);
        //Debug.Log(allPoints[lastActionIndex].playerAction.name);
    }

    private void AddToTurnList(Action action)
    {
        //Debug.Log("Add to turn list action " + action.name);
        turnActions.Enqueue(action);
    }

    private void ClearTurnList()
    {
        //Debug.Log("Clear turn actions");
        turnActions.Clear();
    }

    public void PreviewTurn()
    {
        Player player;
        //PlayerPreview playerPreview;
        PlayerType playerType;
        if (playersTurn == PlayerType.Player)
        {
            player = player1;
            //playerPreview = playerPreview1;
            playerType = PlayerType.Player;
        }
        else
        {
            player = player2;
            //playerPreview = playerPreview2;
            playerType = PlayerType.Opponent;
        }
        //playerPreview.ClearAnimations();
        int initial = player.lastPointEvaluated;
        int final = GetLastActiveIndex(playerType);
        for (int i = initial; i <= final; i++)
        {
            if (player == player1)
            {
                if (allPoints[i].playerAnimationStarted) { }//playerPreview.AddToAnimations(allPoints[i].playerAction.name);
            }
            else
            {
                if (allPoints[i].playerAnimationStarted) { }//playerPreview.AddToAnimations(allPoints[i].opponentAction.name);
            }
        }
        //playerPreview.PreviewTurn();
    }

    public void EndTurn()
    {
        //Debug.Log(playersTurn.ToString() + " ended their turn");
        timelineManager.SetIconsStatic();
        if (turnActions.Count == 0)
        {
            //Debug.Log("Add idle");
            Action idleAction = new Action(GetCurrentPlayerType(), PlayerState.Idle, 50);
            Action previousAction = GetLastAction(GetCurrentPlayerType());

            int lastPlayerActionIndex = GetLastActiveIndex(GetCurrentPlayerType());
            PlayerType opponent = (GetCurrentPlayerType() == PlayerType.Player) ? PlayerType.Opponent : PlayerType.Player;
            int lastOpponentActionIndex = GetLastActiveIndex(opponent);

            idleAction.initialTime = allPoints[lastPlayerActionIndex].time;
            int indicesActive = lastOpponentActionIndex - lastPlayerActionIndex;
            //AddActionToPointsInTime(idleAction, lastPlayerActionIndex, indicesActive, 0);
        }
        CheckAllPoints();
        ClearTurnList();
        //GetCurrentPlayer().lastPointEvaluated = currentPoint.index;
        //Debug.Log("Player1 Initial = " + player1.lastPointEvaluated + "\n Last = " + player1.lastPointToEvaluate);
        //Debug.Log("Player2 Initial = " + player2.lastPointEvaluated + "\n Last = " + player2.lastPointToEvaluate);
        playersTurn = (playersTurn == PlayerType.Player ? PlayerType.Opponent : PlayerType.Player);
        
        if (playersTurn == PlayerType.Opponent)
        {
            opponentBehaviour.StartTurn();
        }
    }

    private void CheckAllPoints()
    {
        
        //Debug.Log(pointsToEvaluate.Count);
        Queue<PointInTime> pointsToEvaluate = GetRemaingPointsToEvaluate();
        if (pointsToEvaluate.Count > 1)
        {
            //Debug.Log(pointsToEvaluate.Count);
            StartCoroutine(PlayPointInTime(currentPoint));
        }
        //lastIndice = Math.Min(player1.lastPointToEvaluate, player2.lastPointToEvaluate);
        
        //Debug.Log("Last indice is = " + lastIndice);
        // if (player1.lastPointEvaluated < lastIndice
        //         || player2.lastPointEvaluated < lastIndice)
        // {
        //     Debug.Log("More points found");
        //     CheckAllPoints();
        // }
    }

    private Queue<PointInTime> GetRemaingPointsToEvaluate()
    {
        int initialIndice = Math.Min(player1.lastPointEvaluated, player2.lastPointEvaluated);
        for (int i = 0; i <= allPoints.Length; i++)
        {
            if (!allPoints[i].evaluated)
            {
                initialIndice = i;
                break;
            }
        }

        player1.lastPointToEvaluate = GetLastActiveIndex(PlayerType.Player);
        player2.lastPointToEvaluate = GetLastActiveIndex(PlayerType.Opponent);
        int lastIndice = Math.Min(player1.lastPointToEvaluate, player2.lastPointToEvaluate);
        //Debug.Log("Check points from " + initialIndice + " to " + lastIndice);
        return GetPointsToEvaluate(initialIndice, lastIndice);
    }

    private Queue<PointInTime> GetPointsToEvaluate(int initialIndice, int lastIndice)
    {
        //if (playersTurn == PlayerType.Player) player1.lastPointToEvaluate = GetLastActiveIndex(PlayerType.Player);
        //else player2.lastPointToEvaluate = GetLastActiveIndex(PlayerType.Opponent);
        //Debug.Log("Last common indice = " + lastIndice);
        Queue<PointInTime> pointsToEvaluate = new Queue<PointInTime>();

        for (int i = initialIndice; i <= lastIndice; i++)
        {
            pointsToEvaluate.Enqueue(allPoints[i]);
        }
        return pointsToEvaluate;
    }
    


    // private IEnumerator PlayPointsInTime(Queue<PointInTime> points)
    // {
    //     //Debug.Log("Points left = " + points.Count);

    //     if (points.Count > 0)
    //     {
    //         PointInTime _currentPoint = points.Dequeue();
    //         //Debug.Log("Point evaluated? " + _currentPoint.evaluated);
    //         //Debug.Log("Play point in time at " + currentPoint.time);
    //         EvaluatePointInTime(_currentPoint);
    //         currentPoint = _currentPoint;
    //         currentPoint.evaluated = true;
    //         player1.lastPointEvaluated = currentPoint.index;
    //         player2.lastPointEvaluated = currentPoint.index;
    //         player1.lastPointToEvaluate = GetLastActiveIndex(PlayerType.Player);
    //         player2.lastPointToEvaluate = GetLastActiveIndex(PlayerType.Opponent);
    //         points = GetRemaingPointsToEvaluate();
    //         GetNextPointInTime()
    //         if (points.Count > 0)
    //         {
    //             yield return new WaitForSeconds(0.05f);
    //             StartCoroutine(PlayPointsInTime(points));
    //             timelineManager.PlayTimeline();
    //         }
    //     }
    //     else
    //     {
    //         Debug.Log("Exiting play points in time");
    //         yield return null;
    //     }
    // }

    private IEnumerator PlayPointInTime(PointInTime point)
    {
        //Debug.Log("Points left = " + points.Count);
        int lastPointToEvaluate = Math.Min(player1.lastPointToEvaluate, player2.lastPointToEvaluate);
        if (point.index <= lastPointToEvaluate)
        {
            EvaluatePointInTime(point);
            currentPoint = point;
            currentPoint.evaluated = true;
            player1.lastPointEvaluated = currentPoint.index;
            player2.lastPointEvaluated = currentPoint.index;
            player1.lastPointToEvaluate = GetLastActiveIndex(PlayerType.Player);
            player2.lastPointToEvaluate = GetLastActiveIndex(PlayerType.Opponent);
            lastPointToEvaluate = Math.Min(player1.lastPointToEvaluate, player2.lastPointToEvaluate);
            point = GetNextPointInTime(point);
            if (point.index <= lastPointToEvaluate)
            {
                yield return new WaitForSeconds(0.05f);
                StartCoroutine(PlayPointInTime(point));
                timelineManager.PlayTimeline();
            }
        }
        else
        {
            Debug.Log("Exiting play points in time");
            yield return null;
        }
    }

    private void EvaluatePointInTime(PointInTime point)
    {
        if (point.evaluated) return;
        Action playerAction = point.playerAction;
        Action opponentAction = point.opponentAction;
        if (point.playerAnimationStarted) player1.StartAnimation(point.playerAction.name);
        if (point.opponentAnimationStarted) player2.StartAnimation(point.opponentAction.name);
        if (point.playerEvent)
        {
            //Debug.Log("Player event " + playerAction.name);
            if (playerAction.type == CardType.Attack)
            {
                if (playerAction.name == "Feint")
                {
                    Debug.Log("Evaluate feint");
                    Action feintedAction = deckManagerPlayer.feintedAction;
                    feintedAction.icon.SetActive(false);
                    deckManagerPlayer.AddCard(feintedAction);
                }
                else if (playerAction is AttackAction playerAttack)
                {
                    //Debug.Log("player attack finished");
                    Effect attackEffect = CompareActions(playerAction, opponentAction);
                    if (attackEffect != Effect.Missed) PlayerHit(PlayerType.Opponent, attackEffect, playerAttack, point);
                }
            }
            else if (playerAction.type == CardType.Breathing)
            {
                //Debug.Log("Evaluate breathe");
                player1.Breathe(playerAction.windUpTime / 50);
            }
        }
        if (point.opponentEvent)
        {
            //Debug.Log("Opponent event " + opponentAction.name);
            if (opponentAction.type == CardType.Attack)
            {
                if (opponentAction.name == "Feint")
                {
                    Debug.Log("Evaluate feint");
                    Action feintedAction = deckManagerOpponent.feintedAction;
                    feintedAction.icon.SetActive(false);
                    deckManagerOpponent.AddCard(feintedAction);
                }
                else if (opponentAction is AttackAction opponentAttack)
                {
                    //Debug.Log("opponent attack finished");
                    Effect attackEffect = CompareActions(playerAction, opponentAction);
                    if (attackEffect != Effect.Missed) PlayerHit(PlayerType.Player, attackEffect, opponentAttack, point);
                }
            }
            else if (opponentAction.type == CardType.Breathing)
            {
                //Debug.Log("Evaluate breathe");
                player2.Breathe(opponentAction.windUpTime / 50);
            }
        }
        //Debug.Log("Point " + point.index + " evaluated");
        point.evaluated = true;
    }

    private Effect CompareActions(Action playerAction, Action opponentAction)
    {
        Effect attackEffect = new Effect();
        //Debug.Log("Compare actions: " + playerAction.name + " vs " + opponentAction.name);
        if (playerAction is AttackAction playerAttack)
        {
            //Debug.Log("Damage = " + playerAction.damage);
            if (opponentAction is DefenceAction opponentDefence)
            {
                attackEffect = GetEffect(playerAttack.name, opponentDefence.name);
                return attackEffect;
            }
            else return Effect.Hit;
        }
        else if (opponentAction is AttackAction opponentAttack)
        {
            if (playerAction is DefenceAction playerDefence)
            {
                //Debug.Log(opponentAttack.attack);
                attackEffect = GetEffect(opponentAttack.name, playerDefence.name);
                return attackEffect;
            }
            else return Effect.Hit;
        }
        return attackEffect;
    }

    private Effect GetEffect(string attackName, string defenceName)
    {
        if (attackDefenceEffects.TryGetValue((attackName, defenceName), out var effect))
        {
            return effect;
        }
        return Effect.Missed;
    }

    private void PlayerHit(PlayerType playerType, Effect effect, AttackAction attackAction, PointInTime point)
    {
        Debug.Log(playerType + " hit");
        if (effect == Effect.Missed) return;
        Player playerHit = (playerType == PlayerType.Player) ? player1 : player2;
        Player attackingPlayer = (playerType == PlayerType.Player) ? player2 : player1;
        int maxDamage = attackAction.damage;
        int effectiveDamage = 0;
        if (effect == Effect.Blocked)
        {
            effectiveDamage = UnityEngine.Random.Range(1, maxDamage + 1) + attackingPlayer.GetDamageModifier();
            playerHit.Hit(effectiveDamage / 2, true);
            return;
        }
        else if (effect == Effect.Hit)
        {
            if (IsAttackCritical(attackAction.acurracy))
            {
                //Debug.Log("Critical");
                critTxt.SetActive(true);
                StartCoroutine(TurnOffCritTxt());
                effectiveDamage = maxDamage + attackingPlayer.GetDamageModifier();
                AddStun(playerType, point, effectiveDamage);
            }
            else
            {
                effectiveDamage = UnityEngine.Random.Range(1, maxDamage + 1) + attackingPlayer.GetDamageModifier();
            }
            //Debug.Log("Damage = " + effectiveDamage);
            playerHit.Hit(effectiveDamage, false);
        }
    }

    private bool IsAttackCritical(int precision)
    {
        return UnityEngine.Random.Range(1, 100) <= precision ? true : false;
    }

    public void AddStun(PlayerType player, PointInTime point, int indicesActive)
    {
        int pointIndex = point.index;
        Action stunAction = new Action(player, PlayerState.Stunned, indicesActive);
        //Debug.Log("Add " + indicesActive +  "stun to " + player);
        stunAction.initialTime = point.time;
        stunAction.windUpTime = indicesActive * 50;
        //stunAction.timeOfEffect = allPoints[lastActionIndex].time + action.windUpTime;
        //int indicesActive = (int)(stunAction.windUpTime / 50) - 1;
        //Debug.Log("Stun wind up = " + stunAction.windUpTime);
        int lastActionIndex = GetLastActiveIndex(player);
        //GetPreviousPointInTime(point);

        for (int i = pointIndex; i < allPoints.Length; i++)
        {
            if (player == PlayerType.Player)
            {
                //Debug.Log("Action icon at " + i + " null? " + (allPoints[i].playerAction.icon == null));
                if (allPoints[i].playerAction.icon != null)
                {
                    //Debug.Log("Remove icon at " + i);
                    allPoints[i].playerAction.icon.SetActive(false);
                }
            }
            else
            {
                //Debug.Log("Action icon at " + i + " null? " + (allPoints[i].opponentAction.icon == null));
                if (allPoints[i].opponentAction.icon != null)
                {
                    //Debug.Log("Remove icon at " + i);
                    allPoints[i].opponentAction.icon.SetActive(false);
                }
            }
        }
        //Debug.Log("Last action index = " + lastActionIndex + ", Point index = " + pointIndex);
        if (lastActionIndex >= pointIndex)
        {
            RemoveActionFromPointsInTime(player, point);
        }
        //CancelAction(player, point);
        //Debug.Log("Stun active from : " + pointIndex + " to " + pointIndex + indicesActive);
        AddActionToPointsInTime(stunAction, pointIndex, indicesActive, 0);
        timelineManager.AddStunIcon(player, point, stunAction);
    }

    //private void AttackHit(PlayerType player, PointInTime point, int damage)
    //{
    //    Debug.Log("attack hit");
    //    if (player == PlayerType.Player)
    //    {
    //        if (point.playerState != PlayerState.Null || point.playerState != PlayerState.Idle)
    //        {
    //            if (point.playerAction.type == CardType.Attack) { CancelAction(player, point); }
    //            else { MoveAction(point.playerAction); }
    //        }
    //    }
    //    else
    //    {
    //        if (point.opponentState != PlayerState.Null || point.opponentState != PlayerState.Idle)
    //        {
    //            if (point.playerAction.type == CardType.Attack) { CancelAction(player, point); }
    //            else { MoveAction(point.playerAction); }
    //        }
    //    }
    //}

    private IEnumerator TurnOffCritTxt()
    {
        yield return new WaitForSeconds(3);
        critTxt.SetActive(false);
    }
}
