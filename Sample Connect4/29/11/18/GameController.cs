using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    enum Piece //named constants
    {
        Empty = 0,
        Yellow = 1,
        Red = 2
    }

    public int numRows = 6;
    public int numColumns = 7;
    // Gameobjects
    public GameObject pieceRed; //red counters
    public GameObject pieceYellow; //yellow counters
    public GameObject pieceField; //empty spaces

    public GameObject winningText;
    public string playerWonText = "You Won!";
    public string playerLoseText = "You Lose!";
    public string drawText = "Draw!";

    public GameObject btnPlayAgain;
    bool btnPlayAgainTouching = false;
    Color btnPlayAgainOrigColor;
    Color btnPlayAgainHoverColor = new Color(255, 143, 4);

    GameObject gameObjectField;

    // temporary gameobject, holds the piece at mouse position until the mouse has clicked
    GameObject gameObjectTurn;

    int[,] field; //2d array

    bool isPlayers1Turn = true;
    bool isPlayers2Turn = true;
    bool isDropping = false;
    bool mouseButtonPressed = false;

    bool gameOver = false;
    bool isCheckingForWinner = false;

    void Start()
    {
        CreateField();

        isPlayers1Turn = System.Convert.ToBoolean(Random.Range(0, 1)); //random number 1 or 0 to decide who goes first

        btnPlayAgainOrigColor = btnPlayAgain.GetComponent<Renderer>().material.color; //accessing and assigning material colour of this button
    }

    void CreateField()
    {
        winningText.SetActive(false); //disabling for now
        btnPlayAgain.SetActive(false); //disabling for now

        isPlayers2Turn = true; //game is being played

        gameObjectField = GameObject.Find("Field"); // returns the active object if it exists
        if (gameObjectField != null) //if it does exist then it will clear it (Note this is done to remove any previous game being played)
        {
            DestroyImmediate(gameObjectField);
        }
        gameObjectField = new GameObject("Field"); //creates new field

        // create an empty field and instantiate the cells
        field = new int[numColumns, numRows]; //initialise 2d array with the dimensions of the board remeber it is 7x6
        for (int x = 0; x < numColumns; x++)
        {
            for (int y = 0; y < numRows; y++)
            {
                field[x, y] = (int)Piece.Empty; //remember the enum constants above we are filling 2d array with empty piece which is 0
                GameObject g = Instantiate(pieceField, new Vector3(x, y * -1, -1), Quaternion.identity) as GameObject; // This clones the game object pieceField. Quaternion.identity means "no rotation"
                g.transform.parent = gameObjectField.transform; // modify's the parent-relative position, scale and rotation but keep the world space position, rotation and scale the same from original
            }
        }

        isPlayers2Turn = false;
        gameOver = false;
    }
}
