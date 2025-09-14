using UnityEngine;

public class ChestPressedLogic : MonoBehaviour
{
    // Por ahora vacío. Lo llamamos desde ChestController cuando el cofre
    // cambia a Chest_Press y queda “cerrado”/“resuelto” para siempre.
    public void OnChestPressed()
    {
        Debug.Log("Ejecuto la logica");
    }
}
