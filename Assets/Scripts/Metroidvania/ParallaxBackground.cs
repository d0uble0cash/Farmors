using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxSpeed = 0.5f;
    public bool infiniteHorizontal = true;
    private float spriteWidth;
    private float startX;
    private Vector3 lastCameraPos;

    void Start()
    {
        startX = transform.position.x;
        lastCameraPos = cameraTransform.position;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Parallax movement
        float deltaX = cameraTransform.position.x - lastCameraPos.x;
        transform.position += new Vector3(deltaX * parallaxSpeed, 0, 0);
        lastCameraPos = cameraTransform.position;

        // Infinite tiling
        if (infiniteHorizontal)
        {
            float distanceFromStart = cameraTransform.position.x - transform.position.x;
            if (Mathf.Abs(distanceFromStart) >= spriteWidth)
            {
                float offset = distanceFromStart % spriteWidth;
                transform.position = new Vector3(
                    cameraTransform.position.x + offset,
                    transform.position.y,
                    transform.position.z
                );
            }
        }
    }
}
