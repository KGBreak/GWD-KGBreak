using UnityEngine;
using System.Collections.Generic;

public class DetectionIndicator : MonoBehaviour
{
    private Dictionary<Transform, RectTransform> spottingEnemies;

    [SerializeField] RectTransform indicatorPrefab; // Prefab for indicator
    [SerializeField] RectTransform canvasRect;      // Reference to the canvas rect
    [SerializeField] float edgePadding = 10f;      // Distance from screen edges

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

            // Check if enemy is in the camera view
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(enemy.position);
            bool isOnScreen = viewportPos.z > 0 &&
                              viewportPos.x > 0 && viewportPos.x < 1 &&
                              viewportPos.y > 0 && viewportPos.y < 1;

            indicator.gameObject.SetActive(!isOnScreen);

            if (!isOnScreen)
            {
                // Direction from camera to enemy
                Vector3 toEnemy = (enemy.position - mainCamera.transform.position).normalized;

                // Convert world direction to screen space direction using camera basis
                float x = Vector3.Dot(toEnemy, mainCamera.transform.right);
                float y = Vector3.Dot(toEnemy, mainCamera.transform.up);

                Vector2 direction = new Vector2(x, y).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x);

                // Position indicator near screen edge with padding
                Vector2 screenCenter = new Vector2(Screen.width, Screen.height) / 2f;
                Vector2 rawDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                float halfWidth = Screen.width / 2f - edgePadding;
                float halfHeight = Screen.height / 2f - edgePadding;

                float scale = Mathf.Min(
                    Mathf.Abs(halfWidth / rawDir.x),
                    Mathf.Abs(halfHeight / rawDir.y)
                );

                Vector2 screenPos = screenCenter + rawDir * scale;

                // Convert screen position to canvas position manually
                Vector2 canvasSize = canvasRect.sizeDelta;
                Vector2 canvasPos = new Vector2(
                    (screenPos.x / Screen.width - 0.5f) * canvasSize.x,
                    (screenPos.y / Screen.height - 0.5f) * canvasSize.y
                );

                indicator.anchoredPosition = canvasPos;

                // Rotate the indicator to point toward the enemy
                indicator.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg - 90);
            }
        }
    }

    public void AttemptAddEnemy(Transform enemy)
    {
        if (!spottingEnemies.ContainsKey(enemy))
        {
            RectTransform newIndicator = Instantiate(indicatorPrefab, canvasRect, false);
            spottingEnemies.Add(enemy, newIndicator);
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
