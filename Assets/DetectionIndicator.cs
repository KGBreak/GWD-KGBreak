using UnityEngine;
using System.Collections.Generic;

public class DetectionIndicator : MonoBehaviour
{
    private Dictionary<Transform, RectTransform> spottingEnemies;
    [SerializeField] RectTransform indicatorPrefab; // Prefab for indicator
    [SerializeField] RectTransform canvasRect; // Reference to the canvas rect
    private Camera mainCamera;

    void Start()
    {
        spottingEnemies = new Dictionary<Transform, RectTransform>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (spottingEnemies.Count == 0 || !indicatorPrefab || !mainCamera || !canvasRect)
            return;

        foreach (KeyValuePair<Transform, RectTransform> pair in spottingEnemies)
        {
            Transform enemy = pair.Key;
            RectTransform indicator = pair.Value;
            
            //Check if enemy is still of screen
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(enemy.position);
            bool isOffScreen = viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1 || viewportPos.z < 0;
            if (!isOffScreen)
            {
                RemoveEnemy(enemy);
            }
            else
            {
                // Get direction from camera to enemy
                Vector3 screenPos = mainCamera.WorldToScreenPoint(enemy.position);

                // If behind camera, flip direction
                if (screenPos.z < 0)
                {
                    screenPos *= -1;
                }

                Vector2 screenCenter = new Vector2(Screen.width, Screen.height) / 2f;
                Vector2 screenBounds = screenCenter * 0.9f; // Padding from edge
                Vector2 direction = ((Vector2)screenPos - screenCenter).normalized;

                // Clamp position to screen edge with padding
                Vector2 cappedScreenPos = screenCenter + Vector2.ClampMagnitude(direction * screenBounds.magnitude, screenBounds.magnitude);

                // Convert screen space to canvas space
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, cappedScreenPos, mainCamera, out Vector2 canvasPos);
                indicator.anchoredPosition = canvasPos;

                // Rotate the indicator to point toward the enemy
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                indicator.rotation = Quaternion.Euler(0, 0, angle - 90); // Adjust to match indicator's "forward"
            }
        }
    }
    public void AttemptAddEnemy(Transform enemy)
    {
        if (!spottingEnemies.ContainsKey(enemy)) // Check if the enemy already exists in the dictionary
        {
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(enemy.position);
            bool isOffScreen = viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1 || viewportPos.z < 0;

            if (isOffScreen)
            {
                RectTransform newIndicator = Instantiate(indicatorPrefab, canvasRect);
                spottingEnemies.Add(enemy, newIndicator);
            }
        }
    }


    public void RemoveEnemy(Transform enemy)
    {
        if (spottingEnemies.TryGetValue(enemy, out RectTransform indicator))
        {
            Destroy(indicator.gameObject);
            spottingEnemies.Remove(enemy);
        }
    }
}
