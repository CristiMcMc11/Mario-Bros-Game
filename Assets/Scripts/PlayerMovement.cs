using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private new Camera camera;
    private new Rigidbody2D rigidbody;

    private Vector2 velocity;
    private float inputAxis;

    public float moveSpeed = 8;
    public float maxJumpHeight = 5;
    public float maxJumpTime = 1;
    public float jumpForce => (2 * maxJumpHeight) / (maxJumpTime / 2);
    public float gravity => (-2 * maxJumpHeight) / Mathf.Pow((maxJumpTime / 2), 2);

    public bool grounded { get; private set; }
    public bool jumping { get; private set; }
    public bool running => Mathf.Abs(velocity.x) > 0.25f || Mathf.Abs(inputAxis) > 0.25f;
    public bool sliding => (inputAxis > 0 && velocity.x < 0) || (inputAxis < 0 && velocity.x > 0);

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        camera = Camera.main;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HorizontalMovement();

        grounded = rigidbody.Raycast(Vector2.down);
        if (grounded)
        {
            GroundedMovement();
        }

        ApplyGravity();
    }

    private void HorizontalMovement()
    {
        inputAxis = Input.GetAxis("Horizontal");
        velocity.x = Mathf.MoveTowards(velocity.x, inputAxis * moveSpeed, moveSpeed * 2 * Time.deltaTime);

        if (rigidbody.Raycast(Vector2.right * velocity.x))
        {
            velocity.x = 0;
        }

        if (velocity.x > 0)
        {
            transform.eulerAngles = Vector3.zero;
        } else if (velocity.x < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    private void GroundedMovement()
    {
        velocity.y = Mathf.Max(velocity.y, 0);
        jumping = velocity.y > 0;

        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = jumpForce;
            jumping = true;
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = rigidbody.position;
        position += velocity * Time.fixedDeltaTime;

        Vector2 leftEdge = camera.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);

        rigidbody.MovePosition(position);
    }

    private void ApplyGravity()
    {
        bool falling = velocity.y < 0 || !Input.GetButton("Jump");
        float multiplier = falling ? 2 : 1;

        velocity.y += gravity * multiplier * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, gravity / 2);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
        {
            if (transform.DotTest(collision.transform, Vector2.up))
            {
                velocity.y = 0;
            }
        }
    }
}
