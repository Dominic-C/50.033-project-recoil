using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingChild : MonoBehaviour
{
    private UnityEngine.Object explosionRef;

    // Start is called before the first frame update
    void Start()
    {
        explosionRef = Resources.Load("Explosion");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Child hit");
            KillSelf();
        }
    }


    private void KillSelf()
    {
        GameObject explosion = (GameObject)Instantiate(explosionRef);
        explosion.transform.position = new Vector3(transform.position.x, transform.position.y + transform.position.z);
        Destroy(explosion, 5.0f);
        Destroy(transform.parent.gameObject);
    }

}
