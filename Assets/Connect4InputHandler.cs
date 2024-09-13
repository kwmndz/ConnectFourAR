using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;

public class Connect4InputHandler : MonoBehaviour
{
    public GameObject connect4Board;       // The Connect 4 board GameObject
    public GameObject circlePrefab;        // The hollow circle prefab
    public float yOffset = 1.0f;           // How far above the board the circles will be placed
    public float glowCircleScale = 0.2f;   // Scale of the glowing circles
    public float circleSpacing = 1.0f;     // Spacing between each circle
    private GameObject[] glowCircles;      // To keep track of the spawned circles
    
    void Start()
    {
        // Create the glowing circles above the columns of the Connect 4 board
        CreateGlowCircles();
    }

    void CreateGlowCircles()
    {
        // Assuming the board has 7 columns, instantiate 4 circles
        int columns = 7;
        glowCircles = new GameObject[columns];
        
        // Calculate the width of the board to position the circles above each column
        float boardWidth = connect4Board.transform.localScale.x;
        float columnWidth = boardWidth / columns;
        
        for (int i = 0; i < columns; i++)
        {
            // Calculate the position for each circle above the board
            Vector3 circlePosition = connect4Board.transform.position + new Vector3(
                (i - columns / 2) * circleSpacing,    // X position based on the column index
                yOffset,                              // Y position above the board
                0);                                   // Same Z as the board

            // Instantiate the circle prefab and position it
            GameObject circle = Instantiate(circlePrefab, circlePosition, Quaternion.identity);
            circle.transform.localScale = Vector3.one * glowCircleScale;

            // Optionally name the circles for easier identification in the editor
            circle.name = $"GlowCircle_Column_{i + 1}";

            // Store the circle in the array for future use
            glowCircles[i] = circle;

            // Add button functionality (respond to click)
            AddClickListenerToCircle(circle, i);
        }
    }

    void AddClickListenerToCircle(GameObject circle, int column)
    {
        // Add an event trigger component to detect touches
        EventTrigger trigger = circle.AddComponent<EventTrigger>();

        // Create the entry for the "Pointer Down" event
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        
        // Add the listener for pressing the circle
        entry.callback.AddListener((eventData) => OnCirclePressed(column));

        // Add the event entry to the trigger
        trigger.triggers.Add(entry);
    }

    // Method called when a circle is pressed
    void OnCirclePressed(int columnIndex)
    {
        Debug.Log($"Circle above column {columnIndex + 1} pressed!");
        // Add logic to drop a piece in the selected column
    }
}
