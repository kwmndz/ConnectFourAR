
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Threading.Tasks;

public class ARBoardPlacer : MonoBehaviour
{
    private PieceDrop pieceDroper; // Reference to the PieceDrop script
    public GameObject connect4BoardPrefab;
    public GameObject placementIndicator;  // Optional, a visual for showing the plane.

    private BoardState boardState;

    private Connect4Bot connect4Bot;

    public float holdDuration = 2.0f; // Optional, how long the user must hold to delete the board
    private float holdTimer = 0.0f; // a timer to track the hold duration

    public GameObject circlePrefab;         // The hollow glowing circle prefab
    public float yOffset = 1.0f;            // How far above the board the circles will be placed
    public float glowCircleScale = 0.2f;    // Scale of the glowing circles
    public float circleSpacing = 1.0f;      // Spacing between each circle
    public float startOffset = 0.2f;

    public GameObject[] glowCircles;

    private bool isHolding = false; // a flag to track if the user is holding

    public float boardScale = 1.0f; 

    public float indicatorScale = 1.0f; // Optional, scale of the placement indicator
    
    private ARPlaneManager planeManager; // For detecting the planes to spawn the board
    private ARRaycastManager raycastManager; // Used to detect where the user clicks on the screen
    private GameObject spawnedBoard;

    public Color indicatorColor = Color.yellow; // Optional, color of the placement indicator
    public Color boardColor = Color.blue; // Optional, color of the board
    
    // A list to store AR raycast hits
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private EndGame endGame;

    void Start()
    {
        endGame = GetComponent<EndGame>();
        boardState = GetComponent<BoardState>();
        connect4Bot = GetComponent<Connect4Bot>();
        pieceDroper = GetComponent<PieceDrop>(); // Get the PieceDrop script
        // Get the plane and raycast managers
        planeManager = GetComponent<ARPlaneManager>();
        raycastManager = GetComponent<ARRaycastManager>();

        // Ensure the placement indicator is disabled initially
        if (placementIndicator != null)
            // Initiate placementIndicator at (0,0,0) with no rotation
            placementIndicator = Instantiate(placementIndicator, new Vector3(0, 0, 0), Quaternion.identity);
            placementIndicator.SetActive(false); // '?' Checks if it is null before accessing the property

            // Set the color of the placement indicator to yellow
            Renderer indicatorRenderer = placementIndicator.GetComponent<Renderer>();
            if (indicatorRenderer != null)
            {
                indicatorRenderer.material.color = indicatorColor;
            }
    }

    void Update()
    {
        // Ensure there are detected planes in the scene
        if (planeManager.trackables.count > 0 && spawnedBoard == null)
        {
            // Cast a ray from the center of the screen to detect a plane
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

            if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon)) // Detect planes
            {
                // Get the pose (position and rotation) of the hit
                Pose hitPose = hits[0].pose;

                // move the placement indicator to the hit position
                if (placementIndicator != null)
                {
                    placementIndicator.SetActive(true);
                    placementIndicator.transform.position = hitPose.position;
                    placementIndicator.transform.rotation = hitPose.rotation;
                    placementIndicator.transform.localScale = Vector3.one * indicatorScale;

                }

                // If the user taps on the screen or clicks the mouse, place the board
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
                {
                    PlaceBoard(hitPose);
                }
            }
        }

        // Check for long press on the board to delete it
        if (spawnedBoard != null && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == spawnedBoard)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        // Start holding
                        isHolding = true;
                        holdTimer = 0f;
                    }
                    else if (touch.phase == TouchPhase.Stationary)
                    {
                        // Continue holding
                        holdTimer += Time.deltaTime;

                        if (isHolding && holdTimer > holdDuration)
                        {
                            DeleteBoard();
                        }
                    }
                }
            }

            // Reset the hold state if touch ends or moves
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Moved)
            {
                isHolding = false;
                holdTimer = 0f;
            }
        }
        if (spawnedBoard != null)
        {
            foreach (GameObject circle in glowCircles)
            {
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.gameObject == circle)
                        {
                            int index = Array.IndexOf(glowCircles, circle);
                            OnCirclePressed(index);
                        }
                    }
                }
            }
        }
        
    }

    public GameObject getSpawnedBoard()
    {
        return spawnedBoard;
    }

    void PlaceBoard(Pose placementPose)
    {
        // Instantiate the board at the selected pose
        placementPose.position.y /= 2;
        spawnedBoard = Instantiate(connect4BoardPrefab, placementPose.position, placementPose.rotation);

        // Correct the board's rotation so the feet are always downwards
        AlignBoardRotation(spawnedBoard, placementPose);

        // Scale the board to the desired size
        spawnedBoard.transform.localScale = Vector3.one * boardScale;

        // Create the glowing circles above the columns of the Connect 4 board
        CreateGlowCircles();

        // Set the color of the board to blue
        Renderer boardRenderer = spawnedBoard.GetComponent<Renderer>();
        if (boardRenderer != null)
        {
            boardRenderer.material.color = boardColor;
        }

        // Optionally disable all planes once the board is placed
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }

        // Optionally, disable the placement indicator after placing the board
        placementIndicator?.SetActive(false);
    }

    void AlignBoardRotation(GameObject board, Pose pose)
    {
        // Ensure the board has the desired rotation (-90 degrees on the X-axis)
        Quaternion boardRotation = Quaternion.Euler(-90, pose.rotation.eulerAngles.y-90, 0);
        board.transform.rotation = boardRotation;
    }

    void DeleteBoard()
    {
        // Delete the spawned board and reset variables
        Destroy(spawnedBoard);
        spawnedBoard = null;
        isHolding = false;
        holdTimer = 0f;

        // Optionally, re-enable planes so the user can place a new board
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(true);
        }

    }

    void CreateGlowCircles()
    {
        int columns = 7;  // Number of columns in the Connect 4 board
        glowCircles = new GameObject[columns];

        // Get the local width of the board (along the Y axis)
        float boardWidth = spawnedBoard.transform.localScale.y;
        float columnWidth = boardWidth / columns;
        

        for (int i = 0; i < columns; i++)
        {
            // Calculate the local position for each circle based on the column index
            Vector3 localCirclePosition = new Vector3(
                0,                                  // X position (no depth adjustment needed)
                (i*circleSpacing - columns / 2) * columnWidth+startOffset,    // Y position based on column index
                yOffset);                           // Z position (height above the board)

            // Convert the local position to world space based on the board's current rotation and position
            Vector3 worldCirclePosition = spawnedBoard.transform.TransformPoint(localCirclePosition);

            // Instantiate the circle prefab at the calculated world position
            GameObject circle = Instantiate(circlePrefab, worldCirclePosition, Quaternion.identity);

            // Ensure the circle is scaled correctly
            circle.transform.localScale = Vector3.one * glowCircleScale;

            // Optionally, rotate the circle to face upwards (depending on how you want the circles to appear)
            //circle.transform.rotation = Quaternion.LookRotation(spawnedBoard.transform.up);

            // Optionally name the circles for easier identification
            circle.name = $"GlowCircle_Column_{i + 1}";

            // Store the circle for later reference (e.g., to detect clicks)
            glowCircles[i] = circle;

        }
    }

    IEnumerator DelayMethod(float delay, int bestMove)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Call your method after the delay
        pieceDroper.OnCirclePressed(glowCircles[bestMove], bestMove, spawnedBoard);
    }
 
    void OnCirclePressed(int columnIndex)
    {
        if (boardState.currentPlayer == boardState.p2)
        {
            boardState.DropPiece(columnIndex, boardState.p2);
            pieceDroper.OnCirclePressed(glowCircles[columnIndex], columnIndex, spawnedBoard);
            if (boardState.CheckWin(true)) {
                endGame.EndGameWithWinner(true);
                return;
            }
        }
        if (boardState.currentPlayer == boardState.p1)
        { 
            int bestMove = connect4Bot.DropPieceAI(boardState.currentPlayer);

            // Call the method on the main thread
            boardState.DropPiece(bestMove, boardState.currentPlayer);  
            StartCoroutine(DelayMethod(1f, bestMove));
            if (boardState.CheckWin(false)) {
                endGame.EndGameWithWinner(false);
                return;
            }
        }
        
    }
}


