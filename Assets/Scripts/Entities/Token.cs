using System;
using System.Collections;
using UnityEngine;

namespace Entities.Tokens
{
    public static class TokenColors
    {
        public static Color[] tokenColors =
        {
            new Color(0.25f, 0.75f, 0.85f),  // Light Blue
            new Color(0.20f, 0.35f, 0.65f),  // Blue
            new Color(0.35f, 0.25f, 0.80f),  // Purple
            new Color(0.85f, 0.35f, 0.65f),  // Pink
            new Color(0.85f, 0.25f, 0.20f),  // Red
            new Color(0.85f, 0.45f, 0.20f),  // Orange
            new Color(0.90f, 0.90f, 0.30f),  // Yellow
            new Color(0.30f, 0.70f, 0.20f),  // Green
        };
    }
    
    public class Token : MonoBehaviour
    {
        private Vector3 debugDestination;
        private SpriteRenderer _spriteRender;
        private Animator _animator;
        private ParticleSystem _destroyParticles;
        public string colorOfCurrentSquare; 

        private void Awake()
        {
            _spriteRender = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _destroyParticles = GetComponentInChildren<ParticleSystem>();
            FindCurrentSquareColor();
        }

        // Simplify the position of the token to a 2x2 grid, maintaining what colour of square that it was originally sat on.
        private void FindCurrentSquareColor()
        {
            // Bring it down to a 4x4 grid with modulo, then turn any 1.5 values into negated .5 values (1.5 to -0.5)
            float modulo_x = transform.position.x % 2f;
            float modulo_y = transform.position.y % 2f;
            if (Math.Abs(modulo_x) == 1.5f)
                modulo_x = -(modulo_x * 10 / 30);
            if (Math.Abs(modulo_y) == 1.5f)
                modulo_y = -(modulo_y * 10 / 30);

            /* The question of which colour in the 2x2 grid the token is sitting on can be answered by adding x & y
             *  [-.5 + .5][.5 + .5]    >    [0][1]
             * [-.5 + -.5][.5 + -.5]   >   [-1][0]
             */
            if (modulo_x + modulo_y == 0)
                colorOfCurrentSquare = "white";
            else
                colorOfCurrentSquare = "black";
        }

        public void ChangeColor(Color chosenColor)
        {
            _spriteRender.color = chosenColor;
        }

        // The player can only push tokens in cardinal directions. Therefore when a token is pushed, the current square color simply switches.
        public void UpdateCurrentSquareColor()
        {
            if (colorOfCurrentSquare == "black")
                colorOfCurrentSquare = "white";
            else
                colorOfCurrentSquare = "black";
            print(colorOfCurrentSquare);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(debugDestination, .5f);
        }

        public IEnumerator DestroyToken()
        {
            _destroyParticles.Play();
            _animator.SetBool("destroyed", true);
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }
    }
}