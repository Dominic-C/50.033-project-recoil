using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    public float speed;
    private Transform player;
    private bool lockedOn;
    // Start is called before the first frame update
    public float radius;
    public int lifetime;
    private bool playerOnRight;
    private Animator animator;
    private UnityEngine.Object explosionRef;
    public AnimationClip idleAnimationClip;
    public AnimationClip attackAnimationClip;
    private SpriteRenderer spriteRenderer;
    private AudioSource deathSound;

    void Start()
    {
        lockedOn = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        explosionRef = Resources.Load("Explosion");
        spriteRenderer = GetComponent<SpriteRenderer>();
        deathSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayer(radius);
        if (lockedOn)
        {
            checkPlayerPosition();
            if (playerOnRight)
            {
                spriteRenderer.flipX = true;
                animator.Play(attackAnimationClip.name);
            }
            else
            {
                spriteRenderer.flipX = false;
                animator.Play(attackAnimationClip.name);
            }
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else
        {
            animator.Play(idleAnimationClip.name); 
        }
    }

    private void DetectPlayer(float radius)
    {
        if (Vector2.Distance(transform.position, player.transform.position) <= radius)
        {
            lockedOn = true;
            Invoke("KillSelf", lifetime);
        }
    }

    private void checkPlayerPosition()
    {
        if (player.transform.position.x < transform.position.x)
        {
            playerOnRight = false;
        }
        else
        {
            playerOnRight = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            KillSelf();
        }
    }

    private void KillSelf()
    {
        AudioSource.PlayClipAtPoint(deathSound.clip, transform.position);
        GameObject explosion = (GameObject)Instantiate(explosionRef);
        explosion.transform.position = new Vector3(transform.position.x, transform.position.y + transform.position.z);
        Destroy(explosion, 5.0f);
        Destroy(gameObject);
    }
}
