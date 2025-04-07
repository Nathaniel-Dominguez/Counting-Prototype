#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

// Custom Editor for ScorePocket component
// Provides quick setup options for different score pocket types

[CustomEditor(typeof(ScorePocket))]
public class ScorePocketEditor : Editor
{
    // Serialized Properties
    SerializedProperty pocketTypeProp;
    SerializedProperty pointValueProp;
    SerializedProperty ballTagProp;
    SerializedProperty scoreDelayProp;
    SerializedProperty returnDirectlyToPoolProp;
    SerializedProperty directReturnDelayProp;
    SerializedProperty collectionPointProp;
    // Add new properties for ball awards
    SerializedProperty awardBallsEnabledProp;
    SerializedProperty ballAwardChanceProp;
    SerializedProperty chanceMultiplierForLowScoreProp;
    SerializedProperty chanceMultiplierForMediumScoreProp;
    SerializedProperty chanceMultiplierForHighScoreProp;
    SerializedProperty chanceMultiplierForJackpotProp;

    private bool showScoreSettings = true;
    private bool showBallHandlingSettings = true;
    private bool showBallAwardSettings = true;

    private void OnEnable()
    {
        // Get all serialized properties
        pocketTypeProp = serializedObject.FindProperty("pocketType");
        pointValueProp = serializedObject.FindProperty("pointValue");
        ballTagProp = serializedObject.FindProperty("ballTag");
        scoreDelayProp = serializedObject.FindProperty("scoreDelay");
        returnDirectlyToPoolProp = serializedObject.FindProperty("returnDirectlyToPool");
        directReturnDelayProp = serializedObject.FindProperty("directReturnDelay");
        collectionPointProp = serializedObject.FindProperty("collectionPoint");
        // Initialize new properties
        awardBallsEnabledProp = serializedObject.FindProperty("awardBallsEnabled");
        ballAwardChanceProp = serializedObject.FindProperty("ballAwardChance");
        chanceMultiplierForLowScoreProp = serializedObject.FindProperty("chanceMultiplierForLowScore");
        chanceMultiplierForMediumScoreProp = serializedObject.FindProperty("chanceMultiplierForMediumScore");
        chanceMultiplierForHighScoreProp = serializedObject.FindProperty("chanceMultiplierForHighScore");
        chanceMultiplierForJackpotProp = serializedObject.FindProperty("chanceMultiplierForJackpot");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        
        // Add box around the inspector
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // Description header
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Score Pocket Config", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Pocket type property
        EditorGUILayout.PropertyField(pocketTypeProp);

        // Quick setup buttons
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Quick Setup", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Low Score (100)"))
        {
            SetupPocketType(ScorePocket.ScoreType.LowScore);
        }
        if (GUILayout.Button("Medium Score (500)"))
        {
            SetupPocketType(ScorePocket.ScoreType.MediumScore);
        }
        if (GUILayout.Button("High Score (1000)"))
        {
            SetupPocketType(ScorePocket.ScoreType.HighScore);
        }
        if (GUILayout.Button("Jackpot (5000)"))
        {
            SetupPocketType(ScorePocket.ScoreType.Jackpot);
        }
        EditorGUILayout.EndHorizontal();

        // Validation and setup checks
        CheckForColliderSetup();

        // Score Settings Foldout
        EditorGUILayout.Space();
        showScoreSettings = EditorGUILayout.Foldout(showScoreSettings, "Score Settings", true, EditorStyles.foldoutHeader);
        if (showScoreSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(pointValueProp);
            EditorGUILayout.PropertyField(ballTagProp);
            EditorGUILayout.PropertyField(scoreDelayProp);
            EditorGUI.indentLevel--;
        }

        // Ball Handling Settings Foldout
        EditorGUILayout.Space();
        showBallHandlingSettings = EditorGUILayout.Foldout(showBallHandlingSettings, "Ball Handling", true, EditorStyles.foldoutHeader);
        if (showBallHandlingSettings)
        {
            EditorGUI.indentLevel++;

            // Draw the resturn directly to pool toggle
            EditorGUILayout.PropertyField(returnDirectlyToPoolProp, new GUIContent("Return Directly To Pool", 
                "If checked, balls will be returned directly to the pool without animating to a collection point"));

            // Only show direct return delay if return directly to pool is enabled
            if (!returnDirectlyToPoolProp.boolValue)
            {
                EditorGUILayout.PropertyField(directReturnDelayProp, new GUIContent("Direct Return Delay",
                    "How long to wait before returning the ball to the pool"));

                if (collectionPointProp.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox("No collection point set. Will use GameManager's collection tray.", MessageType.Info);
                }
            }
            EditorGUI.indentLevel--;
        }

        // Ball Award Settings Foldout
        EditorGUILayout.Space();
        showBallAwardSettings = EditorGUILayout.Foldout(showBallAwardSettings, "Ball Award System", true, EditorStyles.foldoutHeader);
        if (showBallAwardSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(awardBallsEnabledProp, new GUIContent("Enable Ball Awards", "Enable/disable the ball award system for this pocket"));
            
            if (awardBallsEnabledProp.boolValue)
            {
                EditorGUILayout.PropertyField(ballAwardChanceProp, new GUIContent("Award Chance", "Base chance to award balls (0-1)"));
                
                // Display information about the fixed ball ranges
                ScorePocket.ScoreType pocketType = (ScorePocket.ScoreType)pocketTypeProp.enumValueIndex;
                string ballRangeInfo = GetBallRangeInfo(pocketType);
                EditorGUILayout.HelpBox(ballRangeInfo, MessageType.Info);
                
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Chance Multipliers", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(chanceMultiplierForLowScoreProp, new GUIContent("Low Score Multiplier", "Multiplier for low score pockets"));
                EditorGUILayout.PropertyField(chanceMultiplierForMediumScoreProp, new GUIContent("Medium Score Multiplier", "Multiplier for medium score pockets"));
                EditorGUILayout.PropertyField(chanceMultiplierForHighScoreProp, new GUIContent("High Score Multiplier", "Multiplier for high score pockets"));
                EditorGUILayout.PropertyField(chanceMultiplierForJackpotProp, new GUIContent("Jackpot Multiplier", "Multiplier for jackpot pockets"));
                
                // Show current effective chance based on pocket type
                ScorePocket scorePocket = (ScorePocket)target;
                float effectiveChance = scorePocket.GetEffectiveBallAwardChance();
                EditorGUILayout.HelpBox($"Effective Award Chance: {effectiveChance:P0}", MessageType.Info);
            }
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private void SetupPocketType(ScorePocket.ScoreType type)
    {
        pocketTypeProp.enumValueIndex = (int)type;

        // Set default values based on type
        switch (type)
        {
            case ScorePocket.ScoreType.LowScore:
                pointValueProp.intValue = 100;
                break;

            case ScorePocket.ScoreType.MediumScore:
                pointValueProp.intValue = 500;
                break;

            case ScorePocket.ScoreType.HighScore:
                pointValueProp.intValue = 1000;
                break;
            case ScorePocket.ScoreType.Jackpot:
                pointValueProp.intValue = 5000;
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void CheckForColliderSetup()
    {
        ScorePocket scorePocket = (ScorePocket)target;

        Collider col = scorePocket.GetComponent<Collider>();
        if (col == null)
        {
            EditorGUILayout.HelpBox("No collider found! Score Pocket requires a collider to function.", MessageType.Error);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Box Collider"))
            {
                BoxCollider boxCol = scorePocket.gameObject.AddComponent<BoxCollider>();
                boxCol.isTrigger = true;
                boxCol.size = new Vector3(1f, 0.1f, 1f);
            }
            if (GUILayout.Button("Add Sphere Collider"))
            {
                SphereCollider sphereCol = scorePocket.gameObject.AddComponent<SphereCollider>();
                sphereCol.isTrigger = true;
                sphereCol.radius = 0.5f;
            }
            EditorGUILayout.EndHorizontal();
        }
        else if (!col.isTrigger)
        {
            EditorGUILayout.HelpBox("Collider is not set as a trigger! This won't work correctly.", MessageType.Warning);

            if (GUILayout.Button("Fix Collider"))
            {
                col.isTrigger = true;
            }
        }
    }

    // Scene view visualization
    void OnSceneGUI()
    {
        ScorePocket scorePocket = (ScorePocket)target;
        Transform transform = scorePocket.transform;

        // Dsiplay the score value above the pocket
        Handles.color = GetScoreTypeColor(scorePocket.GetPocketType());
        Handles.Label(transform.position + Vector3.up * 0.5f,
            $"{scorePocket.GetPocketType()} ({scorePocket.GetPointValue()} pts)");

        // Draw path to collection point if set and not using direct return
        SerializedProperty directReturnProp = serializedObject.FindProperty("returnDirectlyToPool");
        SerializedProperty collectionProp = serializedObject.FindProperty("collectionPoint");


        if (!directReturnProp.boolValue && collectionProp.objectReferenceValue != null)
        {
            Transform collectionPoint = (Transform)collectionProp.objectReferenceValue;
            Handles.color = Color.green;
            Handles.DrawDottedLine(transform.position, collectionPoint.position, 4f);
            Handles.DrawWireDisc(collectionPoint.position, Vector3.forward, 0.1f);
        }

        // Display indicator for direct return
        if (directReturnProp.boolValue)
        {
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(transform.position, Vector3.forward, 0.6f);

            // Draw an arrow down to indicate disappearing
            Vector3 start = transform.position;
            Vector3 end = start - Vector3.up * 0.5f;
            Handles.DrawLine(start, end);

            // Draw arrowhead
            Vector3 right = end + new Vector3(0.1f, 0.1f, 0);
            Vector3 left = end + new Vector3(-0.1f, 0.1f, 0);
            Handles.DrawLine(end, right);
            Handles.DrawLine(end, left);

            // Add label
            Handles.Label(transform.position - Vector3.up * 0.7f, "Direct Return");
        }
    }

    private Color GetScoreTypeColor(ScorePocket.ScoreType type)
    {
        switch (type)
        {
            case ScorePocket.ScoreType.LowScore:
                return new Color(0.2f, 0.4f, 1f); // Blue
            case ScorePocket.ScoreType.MediumScore:
                return new Color(0.8f, 0.2f, 0.8f); // Purple
            case ScorePocket.ScoreType.HighScore:
                return new Color(1f, 0.9f, 0.2f); // Yellow
            case ScorePocket.ScoreType.Jackpot:
                return new Color(1f, 0.2f, 0.2f); // Red
            default:
                return Color.white;
        }
    }

    // Helper method to get the appropriate ball range info for each pocket type
    private string GetBallRangeInfo(ScorePocket.ScoreType pocketType)
    {
        switch (pocketType)
        {
            case ScorePocket.ScoreType.LowScore:
                return "Low Score Pockets award 1 ball";
            case ScorePocket.ScoreType.MediumScore:
                return "Medium Score Pockets award 2-4 balls";
            case ScorePocket.ScoreType.HighScore:
                return "High Score Pockets award 5-9 balls";
            case ScorePocket.ScoreType.Jackpot:
                return "Jackpot Pockets award 10-19 balls plus bonuses based on current score";
            default:
                return "Ball award range not defined";
        }
    }
}
#endif