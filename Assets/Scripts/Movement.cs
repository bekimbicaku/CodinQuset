using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Shpejtësia e lëvizjes së karakterit
    public Joystick joystick; // Referenca për joystick

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveHorizontal = joystick.Horizontal; // Merr vlerën e joystick për lëvizjen në drejtim horizontal
        float moveVertical = joystick.Vertical; // Merr vlerën e joystick për lëvizjen në drejtim vertikal

        Vector2 movement = new Vector2(moveHorizontal, moveVertical).normalized; // Krijo vektorin e lëvizjes dhe normalizoje atë

        rb.velocity = movement * moveSpeed; // Cakto shpejtësinë e karakterit bazuar në vektorin e lëvizjes dhe shpejtësinë e përcaktuar

        // Optional: rotate character towards movement direction
        // Nëse dëshironi që karakteri të ndrohet në drejtimin e lëvizjes
        if (movement != Vector2.zero)
        {
            transform.up = movement; // Ndriq karakterin në drejtimin e lëvizjes
        }
    }
}
