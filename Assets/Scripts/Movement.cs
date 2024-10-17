using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Shpejt�sia e l�vizjes s� karakterit
    public Joystick joystick; // Referenca p�r joystick

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveHorizontal = joystick.Horizontal; // Merr vler�n e joystick p�r l�vizjen n� drejtim horizontal
        float moveVertical = joystick.Vertical; // Merr vler�n e joystick p�r l�vizjen n� drejtim vertikal

        Vector2 movement = new Vector2(moveHorizontal, moveVertical).normalized; // Krijo vektorin e l�vizjes dhe normalizoje at�

        rb.velocity = movement * moveSpeed; // Cakto shpejt�sin� e karakterit bazuar n� vektorin e l�vizjes dhe shpejt�sin� e p�rcaktuar

        // Optional: rotate character towards movement direction
        // N�se d�shironi q� karakteri t� ndrohet n� drejtimin e l�vizjes
        if (movement != Vector2.zero)
        {
            transform.up = movement; // Ndriq karakterin n� drejtimin e l�vizjes
        }
    }
}
