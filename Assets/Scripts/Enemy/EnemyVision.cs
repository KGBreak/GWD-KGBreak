using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] float visionRange = 10f;
    [SerializeField] float visionAngle = 45f;
    [SerializeField] EnemyMovement enemyMovement;
    [SerializeField] float detectionMeterSize;
    [SerializeField] float detectionMeterSpeed;
    [SerializeField] DetectionMeter detectionMeter;
    float dectectionMeterValue;
    Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (CanSeePlayer())
        {

            dectectionMeterValue += detectionMeterSpeed;
            Debug.Log(dectectionMeterValue);
            if (dectectionMeterValue > detectionMeterSize)
            {
                enemyMovement.SetDestination(player.position);
            }
            else if (dectectionMeterValue > detectionMeterSize*4)
            {
                Debug.Log("Player detected! YOU LOSE");
            }
        }
        else
        {
            if (dectectionMeterValue > 0) {
                dectectionMeterValue -= detectionMeterSpeed;
            }
        }
        detectionMeter.UpdateMeter(dectectionMeterValue, detectionMeterSize);
    }

    bool CanSeePlayer()
    {
        if (player == null)
        {
            return false;
        }

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer < visionAngle / 2)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < visionRange)
            {
                if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, distanceToPlayer))
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector3 forward = transform.forward * visionRange;
        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2, 0) * forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        Gizmos.color = new Color(1, 1, 0, 0.2f);
        Gizmos.DrawMesh(CreateConeMesh(), transform.position, transform.rotation);

        // Draw a line to the player if they are in view and not obstructed
        if (CanSeePlayer())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
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