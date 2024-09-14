using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    public GameObject textObj; // The text to display when the game ends
    public GameObject panelObj = null; // The panel to display when the game ends (optional)

    public GameObject loseTextObj; // The text to display when the game ends

    private ARBoardPlacer ARBoardPlacer;
    private Camera mainCamera;

    private GameObject endGameText;
    private GameObject endGamePanel;

    private GameObject endGameTextLose;

    public void EndGameWithWinner(bool winner)
    {
        GameObject board = ARBoardPlacer.getSpawnedBoard();
        string winnerText = winner ? "YOU WIN!" : "YOU LOSE!";

        // Calculate positions relative to the board
        Vector3 boardPosition = board.transform.position;
        Vector3 textPosition = boardPosition + new Vector3(0.75f, 0.5f, 1f); // Adjust as needed
        Vector3 panelPosition = boardPosition + new Vector3(0, 0.5f, -1.0f); // Adjust as needed

        if (winner)
        {
            endGameText.transform.position = textPosition;
            endGameText.transform.LookAt(mainCamera.transform);
            endGameText.transform.Rotate(0, 180, 0); // Rotate 180 degrees to face the camera
            endGameText.SetActive(true);

        }
        if (!winner)
        {
            endGameTextLose.transform.position = textPosition;
            endGameTextLose.transform.LookAt(mainCamera.transform);
            endGameTextLose.transform.Rotate(0, 180, 0); // Rotate 180 degrees to face the camera
            endGameTextLose.SetActive(true);
        }
        
        // Display the end game panel if it exists
        if (endGamePanel != null)
        {
            endGamePanel.transform.position = panelPosition;
            endGamePanel.transform.LookAt(mainCamera.transform);
            endGamePanel.transform.Rotate(0, 180, 0); // Rotate 180 degrees to face the camera
            endGamePanel.SetActive(true);
        }
    }

    void Start()
    {
        // Hide the end game text and panel at the start of the game
        endGameText = Instantiate(textObj, new Vector3(0, 0, 0), Quaternion.identity);
        endGameTextLose = Instantiate(loseTextObj, new Vector3(0, 0, 0), Quaternion.identity);

        ARBoardPlacer = FindObjectOfType<ARBoardPlacer>();
        mainCamera = Camera.main;

        endGameText.SetActive(false);
        endGameTextLose.SetActive(false);
        if (panelObj != null)
        {
            endGamePanel = Instantiate(panelObj, new Vector3(0, 0, 0), Quaternion.identity);
            endGamePanel.SetActive(false);
        }
    }

    void Update()
    {
        // Rotate the end game text and panel to face the camera
        if (endGameText.activeSelf)
        {
            endGameText.transform.LookAt(mainCamera.transform);
            endGameText.transform.Rotate(0, 180, 0); // Rotate 180 degrees to face the camera
        }

        if (endGameTextLose.activeSelf)
        {
            endGameTextLose.transform.LookAt(mainCamera.transform);
            endGameTextLose.transform.Rotate(0, 180, 0); // Rotate 180 degrees to face the camera
        }

        if (panelObj != null && endGamePanel.activeSelf)
        {
            endGamePanel.transform.LookAt(mainCamera.transform);
            endGamePanel.transform.Rotate(0, 180, 0); // Rotate 180 degrees to face the camera
        }
    }
}
