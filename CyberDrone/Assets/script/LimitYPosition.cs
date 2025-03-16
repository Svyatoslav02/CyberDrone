using UnityEngine;

public class LimitYPosition : MonoBehaviour
{
    public int limit = 500;
    void Update()
    {
        if (transform.position.y > limit)
        {
            Vector3 newPosition = transform.position;
            newPosition.y = limit;
            transform.position = newPosition;

            // If using Rigidbody, reset velocity to avoid getting stuck
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            }
        }
    }
}
