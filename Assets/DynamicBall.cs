using UnityEngine;

public class DynamicBall : MonoBehaviour
{
    private CircleCollider2D circleCollider;
    private LineRenderer lineRenderer;
    private Rigidbody2D rb;

    public Vector2 Force;

    private void Awake()
    {
        print("Press Space to predict, release to start, press R to reset");
        circleCollider = GetComponent<CircleCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    bool running = false;
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) && !running)
        {
            SimulateTrajectory(Force);
        }

        if (Input.GetKeyUp(KeyCode.Space) && !running)
        {
            running = true;
            rb.velocity = Force;
            Debug.LogWarning("Check transform position against linerenderer points for accuracy");
            Debug.Break();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            running = false;
            rb.velocity = Vector2.zero;
            transform.position = Vector2.zero;
            lineRenderer.positionCount = 0;
        }
    }


    public void SimulateTrajectory(Vector2 force)
    {
        Vector3[] points = new Vector3[50];

        Vector2 simPosition = transform.position;

        for (int i = 0; i < points.Length; i++)
        {
            RaycastHit2D hit = Physics2D.CircleCast(simPosition, circleCollider.radius, force, force.magnitude * Time.fixedDeltaTime);
            if (hit)
            {
                print($"Predicted Hit Normal: {hit.normal.x},{hit.normal.y}");
                print($"Predicted Hit Point: {hit.point.x},{hit.point.y}");
                print($"Predicted Hit Velocity: {hit.transform.GetComponent<Rigidbody2D>().velocity - force}");
                force = Vector2.Reflect(force, hit.normal);
            }
            simPosition += (force * Time.fixedDeltaTime);

            points[i] = simPosition;

        }
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        ContactPoint2D contact = other.contacts[0];

        print($"Actual Hit Normal: {contact.normal.x},{contact.normal.y}");
        print($"Actual Hit Point: {contact.point.x},{contact.point.y}");
        print($"Actual Hit Velocity: {contact.relativeVelocity}");
    }
}