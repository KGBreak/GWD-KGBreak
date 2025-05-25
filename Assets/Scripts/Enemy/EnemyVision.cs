using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class EnemyVision : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float visionRange = 10f;
    [SerializeField] private float visionAngle = 45f;
    [SerializeField] private float downwardTiltAngle = 20f;
    [SerializeField] private float proximityDetection = 5f;
    [SerializeField] private Enemy enemy;
    [SerializeField] private float detectionMeterSize = 3f;
    [SerializeField] private float deathSize = 5f;
    [SerializeField] private DetectionMeter detectionMeter;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("FMOD Guard VO")]
    [SerializeField] private EventReference guardDetectionStartEvent;
    [SerializeField] private EventReference guardChaseStartEvent;
    [SerializeField] private EventReference guardDetectionLostEvent;

    [SerializeField] private VoiceManager voiceManager;

    private float detectionMeterValue = 0f;
    private Transform player;
    private PlayerMovement playerMovement;
    private DetectionIndicator detectionIndicator;
    private float visionHeightOffset = 0.8f;

    private bool hasPlayedDetectionStartVO = false;
    private bool hasPlayedChaseStartVO = false;
    private bool hasPlayedDetectionLostVO = false;

    // Cooldown related variables
    private float cooldownTimer = 0f;
    private float cooldownDuration = 5f;  // 5 seconds cooldown

    void Start()
    {
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.transform;
        playerMovement = playerObject.GetComponent<PlayerMovement>();
        detectionIndicator = playerObject.GetComponent<DetectionIndicator>();
    }

    void Update()
    {
        if (CanSeePlayer())
        {
            detectionIndicator.AttemptAddEnemy(transform);

            // 1) Meter just went from 0→>0?
            if (detectionMeterValue <= 0f && !hasPlayedDetectionStartVO)
            {
                RuntimeManager.PlayOneShot(guardDetectionStartEvent, transform.position);
                hasPlayedDetectionStartVO = true;
                hasPlayedDetectionLostVO = false;  // allow lost-VO next time
            }

            detectionMeterValue += Time.deltaTime;

            // 2) Meter crossed chase threshold?
            if (detectionMeterValue > detectionMeterSize)
            {
                if (!hasPlayedChaseStartVO)
                {
                    RuntimeManager.PlayOneShot(guardChaseStartEvent, transform.position);
                    hasPlayedChaseStartVO = true;
                }

                cooldownTimer = cooldownDuration;
                enemy.SetInvestigateTarget(player.position);
            }

            // 3) Full detect → restart
            if (detectionMeterValue > deathSize)
            {
                if (playerMovement != null)
                {
                    playerMovement.ForceStopMovementAudio();
                }

                Debug.Log("Player detected, restarting the scene...");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

        }
        else
        {
            detectionIndicator.RemoveEnemy(transform);

            // drain the meter
            if (detectionMeterValue > 0f)
            {
                if (cooldownTimer > 0f)
                {
                    cooldownTimer -= Time.deltaTime;
                }
                else
                {
                    detectionMeterValue -= Time.deltaTime;
                }
            }
            else
            {
                if (!hasPlayedDetectionLostVO && hasPlayedChaseStartVO)
                {
                    RuntimeManager.PlayOneShot(guardDetectionLostEvent, transform.position);
                    hasPlayedDetectionLostVO = true;
                }

                detectionMeterValue = 0f;
                hasPlayedDetectionStartVO = false;
                hasPlayedChaseStartVO = false;
            }
        }

        detectionMeter.UpdateMeter(detectionMeterValue, detectionMeterSize);
    }

    private bool CanSeePlayer()
    {
        if (player == null || playerMovement.getHiding())
            return false;

        Vector3 origin = transform.position + Vector3.up * visionHeightOffset;
        if (player.position.y > origin.y) return false;

        // Proximity check
        if (Vector3.Distance(player.position, transform.position) < proximityDetection)
        {
            if (!Physics.Raycast(transform.position,
                                 (player.position - transform.position).normalized,
                                 proximityDetection,
                                 obstacleLayer))
                return true;
        }

        // Cone-of-vision check
        Vector3 dirToPlayer = (player.position - origin).normalized;
        Vector3 forwardTilt = Quaternion.Euler(-downwardTiltAngle, 0f, 0f) * transform.forward;
        float angle = Vector3.Angle(forwardTilt, dirToPlayer);

        if (angle < visionAngle * 0.5f)
        {
            float dist = Vector3.Distance(origin, player.position);
            if (dist < visionRange &&
                Physics.Raycast(origin, dirToPlayer, out RaycastHit hit, dist) &&
                hit.transform == player)
            {
                return true;
            }
        }

        return false;
    }

    void OnDrawGizmos()
    {
        Vector3 origin = transform.position + Vector3.up * visionHeightOffset;
        Vector3 forwardTilt = Quaternion.Euler(-downwardTiltAngle, 0f, 0f) * transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, visionRange);

        Vector3 fwd = forwardTilt * visionRange;
        Vector3 left = Quaternion.Euler(0, -visionAngle / 2f, 0) * fwd;
        Vector3 right = Quaternion.Euler(0, visionAngle / 2f, 0) * fwd;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origin, origin + left);
        Gizmos.DrawLine(origin, origin + right);

        Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        Gizmos.DrawMesh(CreateConeMesh(), origin, Quaternion.LookRotation(forwardTilt));

        if (Application.isPlaying && CanSeePlayer())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, player.position);
        }
    }

    private Mesh CreateConeMesh()
    {
        Mesh mesh = new Mesh();
        int segments = 20;
        float step = visionAngle / segments;
        float height = visionRange;
        float radius = height * Mathf.Tan(Mathf.Deg2Rad * visionAngle / 2f);

        Vector3[] verts = new Vector3[segments + 2];
        int[] tris = new int[segments * 3];

        verts[0] = Vector3.zero;
        for (int i = 0; i <= segments; i++)
        {
            float a = Mathf.Deg2Rad * (-visionAngle / 2f + step * i);
            verts[i + 1] = new Vector3(Mathf.Sin(a) * radius, 0f, Mathf.Cos(a) * height);
        }

        for (int i = 0; i < segments; i++)
        {
            tris[i * 3 + 0] = 0;
            tris[i * 3 + 1] = i + 1;
            tris[i * 3 + 2] = i + 2;
        }

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}
