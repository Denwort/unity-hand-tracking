using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class TileController : NetworkBehaviour
{
    [Header("Component References")]
    public GameStateController gameController;                       // Reference to the gamecontroller
    public XRSimpleInteractable interactive;
    public MeshRenderer[] childs;
    public Text internalText;

    private Vector3 ocultar = new Vector3(10, 10, 10);
    private Vector3 mostrar = new Vector3(0, 0, 0);
    /// <summary>
    /// Called everytime we press the button, we update the state of this tile.
    /// The internal tracking for whos position (the text component) and disable the button
    /// </summary>
    public void Awake()
    {
        mostrar = transform.position;
        ResetTile();
    }

    public void UpdateTile(HoverEnterEventArgs args)
    {
        bool esHost = IsOwner;
        bool esMiTurno = esHost && gameController.GetPlayerTurn() == 'X' || !esHost && gameController.GetPlayerTurn() == 'O';

        if (esMiTurno)
        {
            UpdateTileServerRpc();
        }
        /*
        internalText.text = gameController.GetPlayersTurn() + "";
        childs[gameController.GetPlayerId()].enabled = true;
        interactive.enabled = false;
        print("--" + internalText.text + "--");
        gameController.EndTurn();
        */
        /*
        bool esJugadorHost = !IsLocalPlayer && IsServer;
        print("----------------");
        if (esJugadorHost && gameController.GetPlayerId()==0 || !esJugadorHost && gameController.GetPlayerId()==1) {
            internalText.text = gameController.GetPlayersTurn() + "";
            childs[gameController.GetPlayerId()].enabled = true;
            interactive.enabled = false;
            gameController.EndTurnServerRpc();
        }
        */
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateTileServerRpc()
    {
        internalText.text = gameController.GetPlayerTurn() + "";
        transform.position = ocultar;
        childs[gameController.GetPlayerId()].transform.position = mostrar;
        print("--" + internalText.text + "--");
        gameController.EndTurn();
        
    }

    /// <summary>
    /// Resets the tile properties
    /// - text component
    /// - buttton image
    /// </summary>
    public void ResetTile()
    {
        internalText.text = " ";
        transform.position = mostrar;
        childs[0].transform.position = ocultar;
        childs[1].transform.position = ocultar;
    }
}