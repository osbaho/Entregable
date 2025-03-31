using UnityEngine;

public class PlayerDeathEffects : MonoBehaviour
{
    public ParticleSystem confettiEffect; // Asigna en el Inspector

    public void PlayDeathEffects()
    {
        if (confettiEffect != null)
        {
            // Instanciar las partículas en la posición del jugador
            ParticleSystem confetti = Instantiate(confettiEffect, transform.position, Quaternion.identity);
            confetti.Play();
            Destroy(confetti.gameObject, 2f); // Elimina la partícula después de 2 segundos
        }
    }
}
