using UnityEngine;

namespace Entities
{
    public class Corpse : MonoBehaviour
    {
        public bool BlackPiece;
        public bool Pissed;
        [SerializeField] Animator animator;
        [SerializeField] private ParticleSystem pSystem;

        void Start()
        {
            animator.SetBool("Black", BlackPiece);
            animator.SetBool("Pissed", Pissed);
        }

        public void TriggerParticles()
        {
            pSystem.Play();
        }

        private void DestroyCorpse()
        {
            Destroy(gameObject);
        }
    }
}