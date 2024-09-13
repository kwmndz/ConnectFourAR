using UnityEngine;
using UnityEngine.EventSystems;

public class PieceDrop : MonoBehaviour
{
    public GameObject connect4PiecePrefab;   // The prefab for the Connect 4 piece
    public float dropForce = 5f;             // Force to apply when the piece is dropped
    public float dropOffset = 0.5f;          // Offset to ensure the piece spawns above the board

    public bool p1Turn = true;               // Player 1's turn


    // When a glow circle is pressed, drop a Connect 4 piece from its location
    public void OnCirclePressed(GameObject glowCircle, int col, GameObject board)
    {
        // Spawn a new Connect 4 piece at the circle's position slightly above the board
        Vector3 spawnPosition = glowCircle.transform.position;
        spawnPosition.y += dropOffset; // Ensure the piece spawns slightly above the board
        Quaternion spawnRotation = Quaternion.Euler(0, 0, board.transform.rotation.eulerAngles.z);
        GameObject newPiece = Instantiate(connect4PiecePrefab, spawnPosition, spawnRotation);

        if (p1Turn)
        {
            newPiece.GetComponent<Renderer>().material.color = Color.red;
            p1Turn = false;
        }
        else
        {
            newPiece.GetComponent<Renderer>().material.color = Color.yellow;
            p1Turn = true;
        }

        // Add a Rigidbody to the piece if it doesn't already have one
        Rigidbody rb = newPiece.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = newPiece.AddComponent<Rigidbody>();
        }

        // Set Rigidbody properties to improve physics behavior
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ; // Freeze rotation and position in X and Z directions

        rb.AddForce(Vector3.down * dropForce, ForceMode.Impulse);
        
    }
}
