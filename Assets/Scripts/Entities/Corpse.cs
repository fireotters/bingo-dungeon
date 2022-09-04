using UnityEngine;

namespace Entities
{
    public class Corpse : MonoBehaviour
    {
        public bool BlackPiece;
        [SerializeField] Animator animator;
        [SerializeField] private ParticleSystem pSystem;

        void Start()
        {
            animator.SetBool("Black", BlackPiece);
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