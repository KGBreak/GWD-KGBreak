using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyVision : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] float visionRange = 10f;
    [SerializeField] float visionAngle = 45f;
    [SerializeField] float downwardTiltAngle = 20f;
    [SerializeField] float proximityDetection;
    [SerializeField] EnemyMovement enemyMovement;
    [SerializeField] float detectionMeterSize;
    [SerializeField] float deathSize;
    [SerializeField] DetectionMeter detectionMeter;
    float dectectionMeterValue;
    Transform player;
    PlayerMovement playerMovement;
    private float visionHeightOffset = 0.8f;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.transform;
        playerMovement = playerObject.GetComponent<PlayerMovement>();

    }

    void Update()
    {
        if (CanSeePlayer())
        {

            dectectionMeterValue += Time.deltaTime;
            if (dectectionMeterValue > detectionMeterSize)
            {
                enemyMovement.SetDestination(player.position);
            }
            if (dectectionMeterValue > deathSize)
            {
                Debug.Log("Player detected, restarting the scene...");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else
        {
            if (dectectionMeterValue > 0) {
                dectectionMeterValue -= Time.deltaTime;
            }
            else
            {
                dectectionMeterValue = 0;
            }
        }
        detectionMeter.UpdateMeter(dectectionMeterValue, detectionMeterSize);
    }

    bool CanSeePlayer()
    {
        if (player == null || playerMovement.getHiding())
        {
            return false;
        }

        Vector3 visionOrigin = transform.position + Vector3.up * visionHeightOffset;

        if(player.position.y > visionOrigin.y)
        {
            return false;
        }

        if (Vector3.Distance(player.position, transform.position) < proximityDetection)
        {
            return true;
        }

        Vector3 directionToPlayer = (player.position - visionOrigin).normalized;
        Vector3 visionDirection = Quaternion.Euler(-downwardTiltAngle, 0, 0) * transform.forward;
        float angleToPlayer = Vector3.Angle(visionDirection, directionToPlayer);

        if (angleToPlayer < visionAngle / 2)
        {
            float distanceToPlayer = Vector3.Distance(visionOrigin, player.position);

            if (distanceToPlayer < visionRange)
            {
                if (Physics.Raycast(visionOrigin, directionToPlayer, out RaycastHit hit, distanceToPlayer))
                {
                    if (hit.transform == player)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    void OnDrawGizmos()
    {
        Vector3 visionOrigin = transform.position + Vector3.up * visionHeightOffset;
        Vector3 visionDirection = Quaternion.Euler(-downwardTiltAngle, 0, 0) * transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(visionOrigin, visionRange);

        Vector3 forward = visionDirection * visionRange;
        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2, 0) * forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(visionOrigin, visionOrigin + leftBoundary);
        Gizmos.DrawLine(visionOrigin, visionOrigin + rightBoundary);

        Gizmos.color = new Color(1, 1, 0, 0.2f);
        Gizmos.DrawMesh(CreateConeMesh(), visionOrigin, Quaternion.LookRotation(visionDirection));

        if (CanSeePlayer())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(visionOrigin, player.position);
        }
    }

    Mesh CreateConeMesh()
    {
        Mesh mesh = new Mesh();
        int segments = 20;
        float angleStep = visionAngle / segments;
        float height = visionRange;
        float radius = height * Mathf.Tan(Mathf.Deg2Rad * visionAngle / 2);

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * (-visionAngle / 2 + angleStep * i);
            vertices[i + 1] = new Vector3(
                Mathf.Sin(angle) * radius,
                0,
                Mathf.Cos(angle) * height
            );
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}