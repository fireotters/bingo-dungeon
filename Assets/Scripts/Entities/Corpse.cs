using UnityEngine;

namespace Entities
{
    public class Corpse : MonoBehaviour
    {
        [SerializeField] private ParticleSystem pSystem;

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