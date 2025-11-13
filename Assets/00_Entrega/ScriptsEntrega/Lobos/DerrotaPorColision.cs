using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Si este objeto (lobo) toca al jugador, carga la escena de derrota.

/// </summary>
public class DerrotaPorColision : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string nombreEscenaDerrota = "Pantalla_Derrota";
    [SerializeField] private string tagJugador = "Player"; 
  

   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagJugador))
        {
           
            SceneManager.LoadScene(nombreEscenaDerrota);
        }
    }
}
