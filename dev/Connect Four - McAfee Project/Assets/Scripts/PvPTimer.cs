using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ConnectFour
{
    public class PvPTimer : MonoBehaviour
    {
        enum Piece //named constants
        {
            Empty = 0,
            Yellow = 1,
            Red = 2
        }

        public int numRows = 6;
        public int numColumns = 7;
        public int numPiecesToWin = 4;
        public bool allowDiagonally = true;
        public float dropTime = 4f; //f ensures its a floating point number
        public int timeLeft = 3;
        public Text countdownText;


        // Gameobjects
        public GameObject pieceRed; //red counters
        public GameObject pieceYellow; //Yellow counters
        public GameObject pieceField; //empty spaces

        public GameObject winningText;
        public GameObject losingText;
        public string playerWonText1 = "Player 1 has won!";
        public string playerWonText2 = "Player 2 has won!";
        public string drawText = "Draw!";

        public GameObject btnPlayAgain;
        bool btnPlayAgainTouching = false;
        Color btnPlayAgainOrigColor;
        Color btnPlayAgainHoverColor = new Color(255, 143, 4);

        public GameObject btnMainMenu;
        bool btnMainMenuTouching = false;
        Color btnMainMenuOrigColor;
        Color btnMainMenuHoverColor = new Color(255, 143, 4);

        GameObject gameObjectField;

        // temporary gameobject, holds the piece at mouse position until the mouse has clicked
        GameObject gameObjectTurn;

        int[,] field; //2d array

        bool isPlayer1Turn = false;
        bool isPlayer2Turn = false;
        bool whichPlayersTurn = false;
        bool isLoading = true;
        bool isDropping = false;
        bool mouseButtonPressed = false;

        bool gameOver = false;
        bool isCheckingForWinner = false;

        // Use this for initialization
        void Start()
        {
            StartCoroutine("LoseTime");
            CreateField();

            whichPlayersTurn = System.Convert.ToBoolean(Random.Range(0, 1)); //random number 1 or 0 to decide who goes first
            if (whichPlayersTurn)
            {
                isPlayer1Turn = true;
            }
            else
            {
                isPlayer2Turn = true;
            }
            btnPlayAgainOrigColor = btnPlayAgain.GetComponent<Renderer>().material.color; //accessing and assigning material colour of this button
            btnMainMenuOrigColor = btnMainMenu.GetComponent<Renderer>().material.color; //accessing and assigning material colour of this button
        }

        void CreateField()
        {
            winningText.SetActive(false); //disabling for now
            btnPlayAgain.SetActive(false); //disabling for now
            btnMainMenu.SetActive(false); //disabling for now
            losingText.SetActive(false);
            isLoading = true; //game is being played

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

            isLoading = false;
            gameOver = false;

            // center camera + texts
            Camera.main.transform.position = new Vector3(3f, -2.5f, -10f); //positioning camera

            winningText.transform.position = new Vector3(3f, -1.5f, -8f); //positioning win text

            btnPlayAgain.transform.position = new Vector3(3f, -3.5f, -8f); //positioning play again button

            btnMainMenu.transform.position = new Vector3(3f, -4.3f, -8f); //positioning main menu button

            losingText.transform.position = new Vector3(3f, -1.5f, -8f); //positioning losing text
        }


        GameObject SpawnPiece()
        {
            Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Spawns a piece at mouse position above the first row

            GameObject g = Instantiate(
                    isPlayer1Turn ? pieceYellow : pieceRed, // if isplayersturn is true then spawn yellow, else spawn red
                    new Vector3(
                    Mathf.Clamp(spawnPos.x, 0, numColumns - 1),
                    gameObjectField.transform.position.y + 1, 0), // spawn it above the first row
                    Quaternion.identity) as GameObject;

            return g;
        }

        void UpdatePlayAgainButton()
        {
            StopCoroutine("LoseTime");
            RaycastHit hit;
            //ray shooting out of the camera from where the mouse is
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit) && hit.collider.name == btnPlayAgain.name)
            {
                btnPlayAgain.GetComponent<Renderer>().material.color = btnPlayAgainHoverColor;
                //check if the left mouse has been pressed down this frame
                if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && btnPlayAgainTouching == false)
                {
                    btnPlayAgainTouching = true;

                    //CreateField();
                    SceneManager.LoadScene("PvPTimer");
                }
            }
            else
            {
                btnPlayAgain.GetComponent<Renderer>().material.color = btnPlayAgainOrigColor;
            }

            if (Input.touchCount == 0)
            {
                btnPlayAgainTouching = false;
            }
        }

        void UpdateMainMenuButton()
        {
            StopCoroutine("LoseTime");
            RaycastHit hit;
            //ray shooting out of the camera from where the mouse is
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit) && hit.collider.name == btnMainMenu.name)
            {
                btnMainMenu.GetComponent<Renderer>().material.color = btnMainMenuHoverColor;
                //check if the left mouse has been pressed down this frame
                if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && btnMainMenuTouching == false)
                {
                    btnMainMenuTouching = true;

                    //CreateField();
                    SceneManager.LoadScene("Menu");
                }
            }
            else
            {
                btnMainMenu.GetComponent<Renderer>().material.color = btnMainMenuOrigColor;
            }

            if (Input.touchCount == 0)
            {
                btnMainMenuTouching = false;
            }
        }

        // Update is called once per frame to see state of the game
        void Update()
        {
            countdownText.text = ("Time Left = " + timeLeft);

            if (timeLeft <= 0)
            {
                StopCoroutine("LoseTime");
 
                losingText.SetActive(true);
                btnPlayAgain.SetActive(true);
                btnMainMenu.SetActive(true);

                UpdatePlayAgainButton();
                UpdateMainMenuButton();

                return;
            }

            if (Time.timeScale == 0)
                return;

            if (isLoading)
                return;

            if (isCheckingForWinner)
                return;

            if (gameOver)
            {
                winningText.SetActive(true);
                btnPlayAgain.SetActive(true);
                btnMainMenu.SetActive(true);

                UpdatePlayAgainButton();
                UpdateMainMenuButton();

                return;
            }

            if (isPlayer1Turn == true && isPlayer2Turn == false || isPlayer1Turn == false && isPlayer2Turn == true)
            {
                if (gameObjectTurn == null)
                {
                    gameObjectTurn = SpawnPiece();
                }
                else
                {
                    // update the objects position
                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    gameObjectTurn.transform.position = new Vector3(
                        Mathf.Clamp(pos.x, 0, numColumns - 1),
                        gameObjectField.transform.position.y + 1, 0);

                    // click the left mouse button to drop the piece into the selected column
                    if (Input.GetMouseButtonDown(0) && !mouseButtonPressed && !isDropping)
                    {
                        mouseButtonPressed = true;

                        StartCoroutine(dropPiece(gameObjectTurn));
                    }
                    else
                    {
                        mouseButtonPressed = false;
                    }
                }
            }
        }

        IEnumerator LoseTime()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                timeLeft--;
            }
        }

        public List<int> GetPossibleMoves()
        {
            List<int> possibleMoves = new List<int>();
            for (int x = 0; x < numColumns; x++)
            {
                for (int y = numRows - 1; y >= 0; y--)
                {
                    if (field[x, y] == (int)Piece.Empty)
                    {
                        possibleMoves.Add(x);
                        break;
                    }
                }
            }
            return possibleMoves;
        }

        IEnumerator dropPiece(GameObject gObject)
        {
            isDropping = true;

            Vector3 startPosition = gObject.transform.position;
            Vector3 endPosition = new Vector3();
            timeLeft = 3;
            // round to a grid cell
            int x = Mathf.RoundToInt(startPosition.x);
            startPosition = new Vector3(x, startPosition.y, startPosition.z);

            // is there a free cell in the selected column?
            bool foundFreeCell = false;
            for (int i = numRows - 1; i >= 0; i--)
            {
                if (field[x, i] == 0)
                {
                    foundFreeCell = true;
                    field[x, i] = isPlayer1Turn ? (int)Piece.Yellow : (int)Piece.Red;
                    endPosition = new Vector3(x, i * -1, startPosition.z);

                    break;
                }
            }

            if (foundFreeCell)
            {
                // Instantiate a new Piece, disable the temporary
                GameObject g = Instantiate(gObject) as GameObject;
                gameObjectTurn.GetComponent<Renderer>().enabled = false;

                float distance = Vector3.Distance(startPosition, endPosition);

                float t = 0;
                while (t < 1)
                {
                    t += Time.deltaTime * dropTime * ((numRows - distance) + 1);

                    g.transform.position = Vector3.Lerp(startPosition, endPosition, t);
                    yield return null;
                }

                g.transform.parent = gameObjectField.transform;

                // remove the temporary gameobject
                DestroyImmediate(gameObjectTurn);

                // run coroutine to check if someone has won
                StartCoroutine(Won());

                // wait until winning check is done
                while (isCheckingForWinner)
                    yield return null;

                isPlayer1Turn = !isPlayer1Turn;
                isPlayer2Turn = !isPlayer2Turn;
            }

            isDropping = false;

            yield return 0;
        }

        IEnumerator Won()
        {
            isCheckingForWinner = true;

            for (int x = 0; x < numColumns; x++)
            {
                for (int y = 0; y < numRows; y++)
                {
                    int layermask;
                    if (isPlayer1Turn)
                    {
                        layermask = (1 << 8);
                    }
                    else
                    {
                        layermask = (1 << 9);
                    }
                    // If its Players turn ignore red as Starting piece and wise versa
                    if (field[x, y] != (isPlayer1Turn ? (int)Piece.Yellow : (int)Piece.Red))
                    {
                        continue;
                    }

                    // shoot a ray of length 'numPiecesToWin - 1' to the right to test horizontally
                    RaycastHit[] hitsHorz = Physics.RaycastAll(
                        new Vector3(x, y * -1, 0),
                        Vector3.right,
                        numPiecesToWin - 1,
                        layermask);

                    // return true (won) if enough hits
                    if (hitsHorz.Length == numPiecesToWin - 1)
                    {
                        gameOver = true;
                        break;
                    }

                    // shoot a ray up to test vertically
                    RaycastHit[] hitsVert = Physics.RaycastAll(
                        new Vector3(x, y * -1, 0),
                        Vector3.up,
                        numPiecesToWin - 1,
                        layermask);

                    if (hitsVert.Length == numPiecesToWin - 1)
                    {
                        gameOver = true;
                        break;
                    }

                    // test diagonally
                    if (allowDiagonally)
                    {
                        // calculate the length of the ray to shoot diagonally
                        float length = Vector2.Distance(new Vector2(0, 0), new Vector2(numPiecesToWin - 1, numPiecesToWin - 1));

                        RaycastHit[] hitsDiaLeft = Physics.RaycastAll(
                            new Vector3(x, y * -1, 0),
                            new Vector3(-1, 1),
                            length,
                            layermask);

                        if (hitsDiaLeft.Length == numPiecesToWin - 1)
                        {
                            gameOver = true;
                            break;
                        }

                        RaycastHit[] hitsDiaRight = Physics.RaycastAll(
                            new Vector3(x, y * -1, 0),
                            new Vector3(1, 1),
                            length,
                            layermask);

                        if (hitsDiaRight.Length == numPiecesToWin - 1)
                        {
                            gameOver = true;
                            break;
                        }
                    }

                    yield return null;
                }

                yield return null;
            }

            // if Game Over update the winning text to show who has won
            if (gameOver == true)
            {
                if (isPlayer1Turn)
                {
                    winningText.GetComponent<TextMesh>().text = playerWonText2;
                }
                else
                {
                    winningText.GetComponent<TextMesh>().text = playerWonText1;
                }


            }
            else
            {
                // check if there are any empty cells left, if not set game over and update text to show a draw
                if (!FieldContainsEmptyCell())
                {
                    gameOver = true;
                    winningText.GetComponent<TextMesh>().text = drawText;
                }
            }

            isCheckingForWinner = false;

            yield return 0;
        }

        bool FieldContainsEmptyCell() //checks to see if cells are empty
        {
            for (int x = 0; x < numColumns; x++)
            {
                for (int y = 0; y < numRows; y++)
                {
                    if (field[x, y] == (int)Piece.Empty)
                        return true;
                }
            }
            return false;
        }
    }
}
