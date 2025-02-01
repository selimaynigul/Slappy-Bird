using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public float speed = 10f;

    private void Update()
    {
        // Move the cloud using unscaled time so it moves even when the game is paused
        transform.position += Vector3.left * speed * Time.unscaledDeltaTime;

        if (transform.position.x < -35f)
        {
            Destroy(gameObject);
        }
    }
}
