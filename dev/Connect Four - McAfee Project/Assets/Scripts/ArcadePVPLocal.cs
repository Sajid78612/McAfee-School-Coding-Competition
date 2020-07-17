using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ConnectFour
{
    public class ArcadePVPLocal : MonoBehaviour
    {
        enum Piece //named constants
        {
            Empty = 0,
            Yellow = 1,
            Red = 2,
            Blackhole = -1
        }

        public int numRows = 6;
        public int numColumns = 7;
        public int numPiecesToWin;
        public int blackHoleCost = 5;
        public int extraGoCost = 4;
        public Text Player1PointsText;
        public Text Player2PointsText;

        public float dropTime = 4f; //f ensures its a floating point number

        // Gameobjects
        public GameObject pieceRed; //red counters
        public GameObject pieceYellow; //Yellow counters
        public GameObject pieceBlackhole; //blackhole counter
        public GameObject pieceField; //empty spaces

        public GameObject winningText;
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

        public GameObject btnBlackhole;
        public GameObject btnExtraGo;

        GameObject gameObjectField;

        // temporary gameobject, holds the piece at mouse position until the mouse has clicked
        GameObject gameObjectTurn;

        int[,] field; //2d array
        int[,] pointField;
        GameObject[,] counterField;

        const int MAX_POINTS = 42;
        int player1Points;
        int player2Points;

        bool isPlayer1Turn = false;
        bool isPlayer2Turn = false;
        bool isLoading = true;
        bool isDropping = false;
        bool mouseButtonPressed = false;
        int powerUsed = -1; // -1 no power, 0  extra go, 1 black hole

        bool gameOver = false;
        bool isCheckingForWinner = false;

        // Use this for initialization
        void Start()
        {
            CreateField();
            player1Points = 6;
            player2Points = 6;
            Player1PointsText.text = player1Points.ToString();
            Player2PointsText.text = player2Points.ToString();

            isPlayer2Turn = true;
            isPlayer1Turn = false;

            btnPlayAgainOrigColor = btnPlayAgain.GetComponent<Renderer>().material.color; //accessing and assigning material colour of this button
            btnMainMenuOrigColor = btnMainMenu.GetComponent<Renderer>().material.color;
        }

        void CreateField()
        {
            winningText.SetActive(false); //disabling for now
            btnPlayAgain.SetActive(false); //disabling for now
            btnMainMenu.SetActive(false); //disabling for now
            btnBlackhole.SetActive(true); // enabling power up
            btnExtraGo.SetActive(true); //enabling power up
            btnExtraGo.GetComponent<Button>().interactable = true;
            btnBlackhole.GetComponent<Button>().interactable = true;

            isLoading = true; //game is being played

            gameObjectField = GameObject.Find("Field"); // returns the active object if it exists
            if (gameObjectField != null) //if it does exist then it will clear it (Note this is done to remove any previous game being played)
            {
                DestroyImmediate(gameObjectField);
            }
            gameObjectField = new GameObject("Field"); //creates new field

            pointField = new int[numColumns, numRows];
            pointField = generatePoints(); // CREATE POINTS FIELD

            counterField = new GameObject[numColumns, numRows];
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
        }

        GameObject SpawnPiece()
        {
            Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Spawns a piece at mouse position above the first row
            GameObject g;
            if (powerUsed == 1)
            {
                g = Instantiate(
                        pieceBlackhole, // Blackhole
                        new Vector3(
                        Mathf.Clamp(spawnPos.x, 0, numColumns - 1),
                        gameObjectField.transform.position.y + 1, 0), // spawn it above the first row
                        Quaternion.identity) as GameObject;
            }
            else
            {
                g = Instantiate(
                        isPlayer1Turn ? pieceYellow : pieceRed, // if isplayersturn is true then spawn yellow, else spawn red
                        new Vector3(
                        Mathf.Clamp(spawnPos.x, 0, numColumns - 1),
                        gameObjectField.transform.position.y + 1, 0), // spawn it above the first row
                        Quaternion.identity) as GameObject;
            }
            return g;
        }

        void UpdatePlayAgainButton()
        {
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
                    SceneManager.LoadScene("ArcadePVPLocal");

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

        public void ExtraGo()
        {
            if (isPlayer1Turn && player1Points >= extraGoCost)
            {
                player1Points = player1Points - extraGoCost;               
                Player1PointsText.text = player1Points.ToString();
            }
            else if (isPlayer2Turn && player2Points >= extraGoCost)
            {
                player2Points = player2Points - extraGoCost;
                Player2PointsText.text = player2Points.ToString();
            }
            else {
                return;
            }
            Points();
            btnBlackhole.SetActive(false);
            btnExtraGo.SetActive(false);
            powerUsed = 0;
        }

        public void Blackhole()
        {
            if (isPlayer1Turn && player1Points >= blackHoleCost)
            {
                player1Points = player1Points - blackHoleCost;
                Player1PointsText.text = player1Points.ToString();
            }
            else if (isPlayer2Turn && player2Points >= blackHoleCost)
            {
                player2Points = player2Points - blackHoleCost;
                Player2PointsText.text = player2Points.ToString();
            }
            else {
                return;
            }
            Points();
            btnBlackhole.SetActive(false);
            btnExtraGo.SetActive(false);
            powerUsed = 1;
            Destroy(gameObjectTurn);

            gameObjectTurn = SpawnPiece();
        }

        // Update is called once per frame to see state of the game
        void Update()
        {
            if (Time.timeScale == 0)
                return;

            if (isLoading)
                return;

            if (isCheckingForWinner)
                return;

            if (gameOver)
            {
                btnExtraGo.GetComponent<Button>().interactable = false;
                btnBlackhole.GetComponent<Button>().interactable = false;
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
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (EventSystem.current.IsPointerOverGameObject())
                        {
                            mouseButtonPressed = true;
                            StopCoroutine(dropPiece(gameObjectTurn));
                        }
                        else if (!mouseButtonPressed && !isDropping)
                        {
                            mouseButtonPressed = true;

                            StartCoroutine(dropPiece(gameObjectTurn));
                        }
                    }
                    else
                    {
                        mouseButtonPressed = false;
                    }
                }
            }
        }

        IEnumerator dropPiece(GameObject gObject)
        {
            isDropping = true;

            int col = 0;
            int row = 0;

            Vector3 startPosition = gObject.transform.position;
            Vector3 endPosition = new Vector3();

            // round to a grid cell
            int x = Mathf.RoundToInt(startPosition.x);
            startPosition = new Vector3(x, startPosition.y, startPosition.z);

            // is there a free cell in the selected column?
            bool foundFreeCell = false;

            if (powerUsed == 1) // When black hole used we have array with references to the gameobject counters, we destroy all these objects in a column if black hole is played.
            {
                for (int i = numRows - 1; i >= 0; i--)
                {
                    if (field[x, i] == 1 || field[x,i] == 2) // Only destroy red or yellow objects.
                    {
                        field[x, i] = 0;
                        Destroy(counterField[x, i]);
                    }
                }
            }

            for (int i = numRows - 1; i >= 0; i--)
            {
                if (field[x, i] == 0)
                {
                    foundFreeCell = true;
                    if (powerUsed == 1) // if blackhole used assign field with value -1 to indicate blackhole
                    {
                        field[x, i] = -1;
                    }
                    else
                    {
                        field[x, i] = isPlayer1Turn ? (int)Piece.Yellow : (int)Piece.Red;
                        row = x;
                        col = i;
                    }
                    if (isPlayer1Turn && player1Points <= MAX_POINTS)
                    {                        
                        if (powerUsed != 1) // When blackhole is used don't decrement counters as blackhole is a seprate type of counter
                        {
                            player1Points--;
                            player1Points = player1Points + pointField[x, i];
                        }                       
                        Player1PointsText.text = player1Points.ToString();
                    }
                    else if (isPlayer2Turn && player2Points <= MAX_POINTS)
                    {                       
                        if (powerUsed != 1)
                        {
                            player2Points--;
                            player2Points = player2Points + pointField[x, i];
                        }                       
                        Player2PointsText.text = player2Points.ToString();
                    }
                    else {/* Add a message to tell the player they cannont have more than 42 counters */}
                    endPosition = new Vector3(x, i * -1, startPosition.z);
                    break;
                }
            }

            if (foundFreeCell)
            {
                // Instantiate a new Piece, disable the temporary
                GameObject g = Instantiate(gObject) as GameObject;
                gameObjectTurn.GetComponent<Renderer>().enabled = false;

                counterField[row, col] = g; // Storing reference to gameObjects into 2d array so we can delete when blackhole is used

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

                if (powerUsed != 0 && !gameOver) {
                    powerUsed = -1; // reset powerUsed state
                    Points(); // Check if you hit 0 counters
                    isPlayer1Turn = !isPlayer1Turn;
                    isPlayer2Turn = !isPlayer2Turn;
                    btnExtraGo.SetActive(true);
                    btnBlackhole.SetActive(true);
                }
                else if (!gameOver) // extra go power is being used here
                {
                    powerUsed = -1; // extra go has been used so reset
                    Points(); // Check if enough points for the next go 
                }
                else
                {
                    // GameOver is true...
                }
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

        public void Points()
        {
            isCheckingForWinner = true;
            if (player1Points <= 0 || player2Points <= 0)
            {
                gameOver = true;
           
                if (isPlayer1Turn)
                {
                    winningText.GetComponent<TextMesh>().text = playerWonText1;
                }
                else
                {
                    winningText.GetComponent<TextMesh>().text = playerWonText2;
                }
            }
            isCheckingForWinner = false;
            return;
        }

        int[,] generatePoints() // 2d array containing points gained at each position
        {
            int[,] Table = new int[7, 6] {
            {0, 0, 0, 0, 2, 2},
            {0, 0, 0, 3, 0, 2},
            {0, 0, 4, 0, 2, 2},
            {0, 5, 0, 3, 0, 2},
            {0, 0, 4, 0, 2, 2},
            {0, 0, 0, 3, 0, 2},
            {0, 0, 0, 0, 2, 2}
        };
            return Table;
        }
    }
}
