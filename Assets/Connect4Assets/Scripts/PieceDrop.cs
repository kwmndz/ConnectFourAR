using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PieceDrop : MonoBehaviour
{
    public GameObject connect4PiecePrefab;   // The prefab for the Connect 4 piece
    public float dropForce = -0.5f;             // Force to apply when the piece is dropped
    public float dropOffset = 0.5f;          // Offset to ensure the piece spawns above the board

    // Player 1's turn
    public bool p1Turn = true;
    private List<GameObject> pieces = new List<GameObject>(); // List to store the Connect 4 pieces


    // When a glow circle is pressed, drop a Connect 4 piece from its location
    public void OnCirclePressed(GameObject glowCircle, int col, GameObject board)
    {
        // Spawn a new Connect 4 piece at the circle's position slightly above the board
        Vector3 spawnPosition = glowCircle.transform.position;
        spawnPosition.y += dropOffset; // Ensure the piece spawns slightly above the board
        Quaternion spawnRotation = Quaternion.identity;
        GameObject newPiece = Instantiate(connect4PiecePrefab, spawnPosition, spawnRotation);
        newPiece.transform.rotation = Quaternion.Euler(board.transform.rotation.eulerAngles.x, board.transform.rotation.eulerAngles.y, board.transform.rotation.eulerAngles.z);

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
        rb.velocity = Vector3.down * 0.1f;

        pieces.Add(newPiece);
    }
}
