///-----------------------------------------------------------------
///   Class:          GameStateController
///   Description:    Handles the current state of the game and whos turn it is
///   Author:         VueCode
///   GitHub:         https://github.com/ivuecode/
///-----------------------------------------------------------------
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class GameStateController : NetworkBehaviour
{
    [Header("TitleBar References")]
    public GameObject playerXIndicator;
    public GameObject playerOIndicator;
    public TextMeshPro textWinner;

    [Header("Misc References")]
    public GameObject endGameState;                                  // Game footer container + winner text

    [Header("Asset References")]
    public Text[] tileList;                                          // Gets a list of all the tiles in the scene

    [Header("GameState Settings")]
    public char whoPlaysFirst;                                     // Who plays first (X : 0) {NOTE! no checks are made to ensure this is either X or O}

    [Header("Private Variables")]
    private int moveCount;                                           // Internal move counter

    private NetworkVariable<char> turn = new NetworkVariable<char>('X', NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    /// <summary>
    /// Start is called on the first active frame
    /// </summary>
    private void Start()
    {
        // Set the internal tracker of whos turn is first and setup UI icon feedback for whos turn it is
        turn.Value = whoPlaysFirst;
        if (turn.Value == 'X')
        {
            playerXIndicator.transform.position = new Vector3(-0.699999988f, 1.82665396f, 1.11543298f);
            playerOIndicator.transform.position = new Vector3(10,10,10);
        }
        else
        {
            playerXIndicator.transform.position = new Vector3(10,10,10);
            playerOIndicator.transform.position = new Vector3(0.699999928f, 1.82665396f, 1.11543298f);
        }
        endGameState.transform.position = new Vector3(10, 10, 10);
    }

    /// <summary>
    /// Called at the end of every turn to check for win conditions
    /// Hardcoded all possible win conditions (8)
    /// We just take position of tiles and check the neighbours (within a row)
    /// 
    /// Tiles are numbered 0..8 from left to right, row by row, example:
    /// [0][1][2]
    /// [3][4][5]
    /// [6][7][8]
    /// </summary>
    public void EndTurn()
    {
        moveCount++;
        if (tileList[0].text[0] == turn.Value && tileList[1].text[0] == turn.Value && tileList[2].text[0] == turn.Value) GameOver(turn.Value);
        else if (tileList[3].text[0] == turn.Value && tileList[4].text[0] == turn.Value && tileList[5].text[0] == turn.Value) GameOver(turn.Value);
        else if (tileList[6].text[0] == turn.Value && tileList[7].text[0] == turn.Value && tileList[8].text[0] == turn.Value) GameOver(turn.Value);
        else if (tileList[0].text[0] == turn.Value && tileList[3].text[0] == turn.Value && tileList[6].text[0] == turn.Value) GameOver(turn.Value);
        else if (tileList[1].text[0] == turn.Value && tileList[4].text[0] == turn.Value && tileList[7].text[0] == turn.Value) GameOver(turn.Value);
        else if (tileList[2].text[0] == turn.Value && tileList[5].text[0] == turn.Value && tileList[8].text[0] == turn.Value) GameOver(turn.Value);
        else if (tileList[0].text[0] == turn.Value && tileList[4].text[0] == turn.Value && tileList[8].text[0] == turn.Value) GameOver(turn.Value);
        else if (tileList[2].text[0] == turn.Value && tileList[4].text[0] == turn.Value && tileList[6].text[0] == turn.Value) GameOver(turn.Value);
        else if (moveCount >= 9) GameOver('D');
        else
            ChangeTurn();
    }

    /// <summary>
    /// Changes the internal tracker for whos turn it is
    /// </summary>
    public void ChangeTurn()
    {
        // This is called a Ternary operator which evaluates "X" and results in "O" or "X" based on truths
        // We then just change some ui feedback like colors.
        turn.Value = (turn.Value == 'X') ? 'O' : 'X';
        if (turn.Value == 'X')
        {
            playerXIndicator.transform.position = new Vector3(-0.699999988f, 1.82665396f, 1.11543298f);
            playerOIndicator.transform.position = new Vector3(10, 10, 10);
        }
        else
        {
            playerXIndicator.transform.position = new Vector3(10, 10, 10);
            playerOIndicator.transform.position = new Vector3(0.699999928f, 1.82665396f, 1.11543298f);
        }
        print("Ahora es turno de: " + turn.Value);
    }

    /// <summary>
    /// Called when the game has found a win condition or draw
    /// </summary>
    /// <param name="winningPlayer">X O D</param>
    private void GameOver(char winningPlayer)
    {
        switch (winningPlayer)
        {
            case 'D':
                textWinner.text = "DRAW";
                break;
            case 'X':
                textWinner.text = "X WINS";
                break;
            case 'O':
                textWinner.text = "O WINS";
                break;
        }
        BlockAllTiles();
        endGameState.transform.position = new Vector3(0,1.5f,1.1f);
    }

    /// <summary>
    /// Restarts the game state
    /// </summary>
    public void RestartGame()
    {
        // Reset some gamestate properties
        moveCount = 0;
        turn.Value = whoPlaysFirst;
        endGameState.transform.position = new Vector3(10,10,10);

        // Loop though all tiles and reset them
        for (int i = 0; i < tileList.Length; i++)
        {
            tileList[i].GetComponentInParent<TileController>().ResetTile();
        }
    }

    /// <summary>
    /// Enables or disables all the buttons
    /// </summary>
    private void BlockAllTiles()
    {
        for (int i = 0; i < tileList.Length; i++)
        {
            tileList[i].GetComponentInParent<Transform>().position = new Vector3(10,10,10);
        }
    }

    /// <summary>
    /// Returns the current players turn (X / O)
    /// </summary>
    public char GetPlayerTurn()
    {
        return turn.Value;
    }

    /// <summary>
    /// Retruns the display sprite (X / 0)
    /// </summary>

    public int GetPlayerId()
    {
        if (turn.Value == 'X') return 0;
        else return 1;
    }

    /// <summary>
    /// Callback for when the P1_textfield is updated. We just update the string for Player1
    /// </summary>

    /// <summary>
    /// Callback for when the P2_textfield is updated. We just update the string for Player2
    /// </summary>
}
