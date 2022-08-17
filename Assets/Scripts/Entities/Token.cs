using System;
using DG.Tweening;
using Entities.Turn_System;
using TMPro;
using Unity.VisualScripting;
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

        private void Awake()
        {
            _spriteRender = GetComponent<SpriteRenderer>();
        }

        public void ChangeColor(Color chosenColor)
        {
            _spriteRender.color = chosenColor;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(debugDestination, .5f);
        }
    }
}