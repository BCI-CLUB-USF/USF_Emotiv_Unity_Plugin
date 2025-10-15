using UnityEngine;

/// <summary>
/// Simple BCI Movement Tester - Works with any input system
/// Uses GUI buttons instead of keyboard input to avoid Input System conflicts
/// </summary>
public class SimpleBCITester : MonoBehaviour
{
    [Header("Test Settings")]
    public float testStrength = 0.8f;
    
    private ImprovedBCIPlayer bciPlayer;
    private Rect buttonArea = new Rect(10, 150, 200, 300);
    
    void Start()
    {
        bciPlayer = FindFirstObjectByType<ImprovedBCIPlayer>();
        
        if (bciPlayer == null)
        {
            Debug.LogError("❌ No ImprovedBCIPlayer found! Add ImprovedBCIPlayer component to test movement.");
        }
        else
        {
            Debug.Log("✅ Simple BCI Tester ready! Use GUI buttons to test commands.");
        }
    }
    
    void OnGUI()
    {
        // Background box
        GUI.Box(buttonArea, "BCI Movement Tester");
        
        // Button layout
        float buttonWidth = 80f;
        float buttonHeight = 30f;
        float startX = buttonArea.x + 10;
        float startY = buttonArea.y + 30;
        
        // Test buttons
        if (GUI.Button(new Rect(startX, startY, buttonWidth, buttonHeight), "← LEFT"))
        {
            TestCommand("left");
        }
        
        if (GUI.Button(new Rect(startX + 90, startY, buttonWidth, buttonHeight), "RIGHT →"))
        {
            TestCommand("right");
        }
        
        if (GUI.Button(new Rect(startX + 45, startY + 40, buttonWidth, buttonHeight), "↑ JUMP"))
        {
            TestCommand("lift");
        }
        
        if (GUI.Button(new Rect(startX, startY + 80, buttonWidth, buttonHeight), "↗ FORWARD"))
        {
            TestCommand("push");
        }
        
        if (GUI.Button(new Rect(startX + 90, startY + 80, buttonWidth, buttonHeight), "↙ BACK"))
        {
            TestCommand("pull");
        }
        
        // Status info
        GUI.Label(new Rect(startX, startY + 120, 180, 20), $"Test Strength: {testStrength:F1}");
        GUI.Label(new Rect(startX, startY + 140, 180, 20), bciPlayer != null ? "✅ Player Found" : "❌ No Player");
        
        // Instructions
        GUI.Label(new Rect(startX, startY + 170, 180, 40), "Click buttons to test\nBCI movement commands");
    }
    
    void TestCommand(string command)
    {
        if (bciPlayer != null)
        {
            bciPlayer.TestCommand(command, testStrength);
            Debug.Log($"🎮 GUI Test: {command.ToUpper()} with strength {testStrength}");
        }
        else
        {
            Debug.LogWarning("⚠️ No BCI Player found, cannot test commands");
        }
    }
}