using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEditor.Experimental.GraphView;

public class PlayManager : MonoBehaviour
{
    public Queue<PointInTime> pointsToEvaluate = new Queue<PointInTime>();
    private PointInTime[] allPoints = new PointInTime[36];
    private int pointIndex = 0;
    private Player player1;
    private Player player2;
    //private PlayerPreview playerPreview1;
    //private PlayerPreview playerPreview2;
    public PlayerType playersTurn = PlayerType.Player;

    [SerializeField] private Transform discardPile;

    private HealthManager opponentHealthManager;
    private HealthManager playerHealthManager;

    [SerializeField] private PlayerStatsSO playerStats;
    [SerializeField] private PlayerStatsSO opponentStats;

    [SerializeField] private HealthManager playerHealth;
    [SerializeField] private HealthManager opponentHealth;

    [SerializeField] private int stunDuration;

    private TimelineManager timelineManager;

    private Dictionary<(string, string), Effect> attackDefenceEffects = new Dictionary<(string, string), Effect>()
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

    private Dictionary<(AttackType, AttackType), bool> attackCombos = new Dictionary<(AttackType, AttackType), bool>()
    {
        { (AttackType.Jab, AttackType.Jab), false },
        { (AttackType.Jab, AttackType.Cross), true },
        //{ (AttackType.Jab, AttackType.) }
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timelineManager = GetComponent<TimelineManager>();
        player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player>();
        player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<Player>();
        //playerPreview1 = GameObject.FindGameObjectWithTag("PlayerPreview1").GetComponent<PlayerPreview>();
        //playerPreview2 = GameObject.FindGameObjectWithTag("PlayerPreview2").GetComponent<PlayerPreview>();
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

    //public void AddToPointsInTime(PointInTime point)
    //{
    //    //Debug.Log("Add to point with time = " + point.time);
    //    if (pointIndex >= allPoints.Length) { IncreasePointsInTime(10); AddToPointsInTime(point); }
    //    else allPoints[pointIndex++] = point;
    //}

    public PointInTime GetActionPointInTime(Action action)
    {
        //Debug.Log(GetLastActiveIndex(action.player));
        return allPoints[GetLastActiveIndex(action.player)];
    }

    public void AddActionToTurn(Action action)
    {
        int lastActionIndex = GetLastActiveIndex(action.player);
        Action previousAction = GetLastAction(action.player);
        if (DoActionsCombo(previousAction, action))
        {
            lastActionIndex -= (int)(previousAction.windUpTime / 50);
        }
        //Debug.Log("Adding " + action.name + " to index " + lastActionIndex);
        action.initialTime = allPoints[lastActionIndex].time;
        action.timeOfEffect = allPoints[lastActionIndex].time + action.windUpTime;
        int indicesActive = (int)(action.windUpTime / 50);
        int indicesReturning = (int)(action.returnTime / 50);
        //Debug.Log("Indices active = " + indicesActive);
        timelineManager.AddActionIcon(allPoints[lastActionIndex], action);
        if (lastActionIndex + indicesActive + indicesReturning >= allPoints.Length) { timelineManager.IncreaseTimeline(); }
        AddActionToPointsInTime(action, lastActionIndex, indicesActive, indicesReturning);
        //if (action.player == PlayerType.Player) 
        //{ 
        //    playerPreview1.AddToAnimations(action.name);
        //    playerPreview1.PreviewTurn();
        //}
        //else
        //{
        //    playerPreview2.AddToAnimations(action.name);
        //    playerPreview2.PreviewTurn();
        //}
        //timelineManager.PreviewAction(allPoints[lastActionIndex + indicesActive]);
    }

    private Action GetLastAction(PlayerType player)
    {
        Action action = new Action(player);
        if (player == PlayerType.Player)
        {
            for (int i = 0; i < allPoints.Length; i++)
            {
                //Debug.Log("Point at " + allPoints[i].time + " is " + allPoints[i].playerState);
                if (allPoints[i].playerState != PlayerState.Null)
                {
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
                    action = allPoints[i].opponentAction;
                }
            }
        }
        return action;
    }

    //private PointInTime GetLastAction(PlayerType player)
    //{
    //    PointInTime lastPoint = allPoints[0];
    //    bool pointFound = false;
    //    if (player == PlayerType.Player)
    //    {
    //        foreach (PointInTime point in allPoints)
    //        {
    //            //Debug.Log(point == null);
    //            if (point.playerState != PlayerState.Null)
    //            {
    //                lastPoint = point;
    //                pointFound = true;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        foreach (PointInTime point in allPoints)
    //        {
    //            if (point.opponentState != PlayerState.Null)
    //            {
    //                lastPoint = point;
    //                pointFound = true;
    //            }
    //        }
    //    }
    //    if (pointFound) return lastPoint;
    //    Debug.Log("Point not found");
    //    return lastPoint;

    //}

    private bool DoActionsCombo(Action firstAction, Action secondAction)
    {
        return false;
    }

    public int GetLastActiveIndex(PlayerType player)
    {
        int lastIndex = 0;
        if (player == PlayerType.Player)
        {
            for (int i = 0; i < allPoints.Length; i++)
            {
                //Debug.Log("Point at " + allPoints[i].time + " is " + allPoints[i].playerState);
                if (allPoints[i].playerState != PlayerState.Null)
                {
                    lastIndex = i;
                }
            }
        }
        else
        {
            for (int i = 0; i < allPoints.Length; i++)
            {
                if (allPoints[i].opponentState != PlayerState.Null)
                {
                    lastIndex = i;
                }
            }
        }
        return lastIndex;
    }

    private void AddActionToPointsInTime(Action action, int initialIndex, int indicesActive, int indicesReturning)
    {
        //Debug.Log("Add action to point in time from " + initialIndex + " to " + (initialIndex + indicesActive));
        int numWindUpIndices = initialIndex + indicesActive;

        if (action.player == PlayerType.Player) { allPoints[initialIndex].playerAnimationStarted = true; }
        else { allPoints[initialIndex].opponentAnimationStarted = true; }

        for (int i = initialIndex; i <= numWindUpIndices; i++)
        {
            //Debug.Log("Adding " + action.playerState + " to index " + i);
            allPoints[i].AddToPoint(action, action.player);
        }

        if (action.type == CardType.Attack && action.player == PlayerType.Player)
        {
            allPoints[initialIndex + indicesActive].playerAttackFinished = true;
        }
        else if (action.type == CardType.Attack && action.player == PlayerType.Opponent)
        {
            allPoints[initialIndex + indicesActive].opponentAttackFinished = true;
        }

        int numReturnIndices = numWindUpIndices + indicesReturning;
        for (int i = numWindUpIndices; i<= numReturnIndices; i++)
        {
            //Debug.Log("Adding " + action.playerState + " return time to index " + i);
            allPoints[i].AddToPoint(action, action.player);
        }
    }


    public void ChangeActionPointInTime(Action action, PointInTime newPoint, int prevTime)
    {
        //Debug.Log("Changing action " + action.name + " from " + prevTime + " to " + newTime);
        //foreach (PointInTime point in allPoints) 
        //{
        //    if (point.time == prevTime)
        //    {
        //        if (action.player == PlayerType.Player) point.playerState = PlayerState.Null;
        //        else point.opponentState = PlayerState.Null;
        //    }
        //    if (point.time == newTime)
        //    {
        //        if (action.player == PlayerType.Player) point.playerState = action.playerState;
        //        else point.opponentState = action.playerState;
        //    }
        //}
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
        //Debug.Log("End turn");
        timelineManager.SetIconsStatic();
        if (playersTurn == PlayerType.Player) player1.lastPointToEvaluate = GetLastActiveIndex(PlayerType.Player);
        else player2.lastPointToEvaluate = GetLastActiveIndex(PlayerType.Opponent);

        int initialIndice = Math.Min(player1.lastPointEvaluated, player2.lastPointEvaluated);
        int lastIndice = Math.Min(player1.lastPointToEvaluate, player2.lastPointToEvaluate);
        //Debug.Log("Last common indice = " + lastIndice);
        Queue<PointInTime> pointsToEvaluate = new Queue<PointInTime>();

        for (int i = initialIndice; i < lastIndice; i++)
        {
            pointsToEvaluate.Enqueue(allPoints[i]);
        }
        //Debug.Log(pointsToEvaluate.Count);

        if (pointsToEvaluate.Count > 0) 
        {
            StartCoroutine(PlayPointsInTime(pointsToEvaluate));
            //Debug.Log(allPoints[lastIndice].transform.position);
            if (!timelineManager.isPlaying) timelineManager.PlayTimeline(allPoints[lastIndice].transform.position);
        } 
        player1.lastPointEvaluated = lastIndice;
        player2.lastPointEvaluated = lastIndice;
        player1.lastPointToEvaluate = GetLastActiveIndex(PlayerType.Player);
        player2.lastPointToEvaluate = GetLastActiveIndex(PlayerType.Opponent);
        //Debug.Log("Player1 Initial = " + player1.lastPointEvaluated + "\n Last = " + player1.lastPointToEvaluate);
        //Debug.Log("Player2 Initial = " + player2.lastPointEvaluated + "\n Last = " + player2.lastPointToEvaluate);
        playersTurn = (playersTurn == PlayerType.Player ? PlayerType.Opponent : PlayerType.Player);
    }

    private IEnumerator PlayPointsInTime(Queue<PointInTime> points)
    {
        //Debug.Log("Points left = " + points.Count);
        if (points.Count > 0)
        {
            PointInTime currentPoint = points.Dequeue();
            //Debug.Log("Play point in time at " + currentPoint.time);
            EvaluatePointInTime(currentPoint);
            yield return new WaitForSeconds(0.05f);
            StartCoroutine(PlayPointsInTime(points));
        }
        else
        {
            Debug.Log("Exiting play points in time");
            yield return null;
        }
    }

    private void EvaluatePointInTime(PointInTime point)
    {
        Action playerAction = point.playerAction;
        Action opponentAction = point.opponentAction;
        //Debug.Log("Evalue point in time at " + point.time);
        //Debug.Log("Point attack? " + point.playerAttackInEffect);
        //Debug.Log(playerAction.damage);
        if (point.playerAnimationStarted) player1.StartAnimation(point.playerAction.name);
        if (point.opponentAnimationStarted) player2.StartAnimation(point.opponentAction.name);

        if (point.playerAttackFinished && point.opponentAttackFinished)
        {
            Debug.Log("Both hit");
            AttackAction playerAttackAction = (AttackAction)playerAction;
            AttackAction opponentAttackAction = (AttackAction)playerAction;
            EffectPlayer(PlayerType.Opponent, Effect.Hit, playerAttackAction.damage, point);
            EffectPlayer(PlayerType.Player, Effect.Hit, opponentAttackAction.damage, point);
            return;
        }

        if (playerAction is AttackAction playerAttack)
        {
            Debug.Log("Player attack");

            if (point.playerAttackFinished)
            {
                Effect attackEffect = CompareActions(playerAction, opponentAction);
                if (attackEffect != Effect.Missed) EffectPlayer(PlayerType.Opponent, attackEffect, playerAttack.damage, point);
            }
        }

        if (opponentAction is AttackAction opponentAttack)
        {
            if (point.opponentAttackFinished)
            {
                Effect attackEffect = CompareActions(playerAction, opponentAction);
                if (attackEffect != Effect.Missed) EffectPlayer(PlayerType.Player, attackEffect, opponentAttack.damage, point);
            }
        }
    }

    private Effect CompareActions(Action playerAction, Action opponentAction)
    {
        Effect attackEffect = new Effect();
        Debug.Log("Compare actions: " + playerAction.name + " vs " + opponentAction.name);
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

    private void EffectPlayer(PlayerType playerType, Effect effect, int damage, PointInTime point)
    {

        if (effect == Effect.Missed) return;
        Player player = (playerType == PlayerType.Player) ? player1 : player2;

        if (effect == Effect.Blocked)
        {
            player.Hit(damage, true);
        }
        else if (effect == Effect.Hit)
        {
            player.Hit(damage, false);
            AddStun(playerType, point);
        }
    }


    public void AddStun(PlayerType player, PointInTime point)
    {
        int pointIndex = (int)(point.time / 50);
        Action stunAction = new Action(player, PlayerState.Stunned, stunDuration);

        stunAction.initialTime = point.time;
        //stunAction.timeOfEffect = allPoints[lastActionIndex].time + action.windUpTime;
        int indicesActive = (int)(stunAction.windUpTime / 50);
        //Debug.Log("Indices active = " + indicesActive);
        if (pointIndex + indicesActive >= allPoints.Length) { timelineManager.IncreaseTimeline(); }
        AddActionToPointsInTime(stunAction, pointIndex, indicesActive, 0);

        timelineManager.AddStunIcon(player, point, stunAction);
        timelineManager.RemoveIcons();
        CancelAction(player, point);
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

    private void CancelAction(PlayerType player, PointInTime point)
    {
        Debug.Log("cancel action");
        int first = point.index;
        Action nullAction = new Action(player);
        //List<Action> removedActions = new List<Action>();

        for (int i = first; i < allPoints.Length; i++)
        {
            //if (player == PlayerType.Player) removedActions.Add(allPoints[i].playerAction);
            //else removedActions.Add(allPoints[i].opponentAction);
            allPoints[i].AddToPoint(nullAction, player);
            allPoints[i].playerAttackFinished = false;
            allPoints[i].opponentAttackFinished = false;
            allPoints[i].playerAnimationStarted = false;
            allPoints[i].opponentAnimationStarted = false;

        }
        if (player == PlayerType.Player) timelineManager.CancelAction(point.playerAction);
        else timelineManager.CancelAction(point.opponentAction);
    }

    private void MoveAction(Action action)
    {
        Debug.Log("move action");
        //CancelAction();
        //AddActionToPointsInTime();
    }
}
