using UnityEngine;

public class SpecialBehaviour : MonoBehaviour
{
    [SerializeField] private int CreatineNumberOfCards;
    [SerializeField] private int proteinDamageModifier;
    [SerializeField] private int scratchStaggerRemoval;

    private Action action;
    private GameObject instinctPanel;
    
    private DeckManager deckManager;
    private Player player;
    private PlayManager playManager;

    public void Instantiate(PlayerType playerType, Action _action)
    {
        action = _action;
        action.playerState = _action.playerState;
        string playerTag = playerType == PlayerType.Player ? "Player1" : "Player2";
        player = GameObject.FindGameObjectWithTag(playerTag).GetComponent<Player>();
        deckManager = player.GetComponentInChildren<DeckManager>();
        playManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<PlayManager>();
        instinctPanel = GameObject.FindGameObjectWithTag("InstinctPanel");
        //Debug.Log("Instinct panel null? " + (instinctPanel == null));
    }

    public void Effect()
    {
        switch (action.name)
        {
            case "Creatine": Creatine(); break;
            case "Protein Shake": ProteinShake(); break;
            case "Feint": Feint(); break;
            case "Instinct": Instinct(); break;
            case "Sheer Dumb Luck": SheerDumbLuck(); break;
            case "But A Scratch": ButAScratch(); break;
        }
    }

    private void Creatine()
    {
        //Debug.Log("Creatine");
        deckManager.DrawCards(CreatineNumberOfCards);
    }

    private void ProteinShake()
    {
        //Debug.Log("Protein");
        player.damageModifier += proteinDamageModifier;
    }

    private void Feint()
    {
        //Debug.Log("Feint");
        Action feintAction = new AttackAction(6, PlayerState.Feint, "Feint", AttackType.Feint, Hand.Rear, AttackRange.Pocket, 150, 0, 0, 0, 0, action.playerType, "Play on an attack, the attack is replaced in your hand, draw a card");
        playManager.AddActionToTurn(feintAction);
    }

    private void Instinct() 
    {
        //Debug.Log("Instinct");
        // Can be played as any attack or defence card
        instinctPanel.SetActive(true);
        instinctPanel.GetComponent<InstinctPanel>().DisplayPanel();
    }

    private void SheerDumbLuck()
    {
        player.accuracyModifier += 50;
    }

    private void ButAScratch()
    {
        player.healthManager.DecreaseStagger(scratchStaggerRemoval);
    }
}
