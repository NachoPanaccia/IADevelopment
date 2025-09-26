using System.Collections.Generic;
using UnityEngine;

public class ChestSpawnRoulette : MonoBehaviour
{
    [Header("Prefab del cofre (con ChestController + lógica y vista)")]
    [SerializeField] private ChestController chestPrefab;

    [Header("Puntos de aparición (hijos o referencias)")]
    [SerializeField] private List<ChestSpawnPoint> puntos = new List<ChestSpawnPoint>();

    void Reset()
    {
        puntos.Clear();
        var encontrados = GetComponentsInChildren<ChestSpawnPoint>(true);
        if (encontrados != null)
        {
            puntos.AddRange(encontrados);
        }
    }

    void Awake()
    {
        if (puntos == null || puntos.Count == 0)
        {
            Debug.LogWarning("[ChestSpawnRoulette] No hay puntos de spawn configurados.");
            return;
        }

        int idxElegido = ElegirIndice();
        Vector3 destino = puntos[idxElegido].Position;

        var cofreExistente = FindFirstObjectByType<ChestController>();
        if (cofreExistente != null)
        {
            cofreExistente.transform.position = destino;
        }
        else
        {
            if (chestPrefab == null)
            {
                Debug.LogError("[ChestSpawnRoulette] No hay prefab asignado para el cofre.");
                return;
            }

            Instantiate(chestPrefab, destino, Quaternion.identity);
        }
    }

    int ElegirIndice()
    {
        float total = 0f;

        for (int i = 0; i < puntos.Count; i++)
        {
            ChestSpawnPoint p = puntos[i];
            if (p != null && p.peso > 0f)
            {
                total += p.peso;
            }
        }

        if (total <= 0f)
        {
            List<int> indicesValidos = new List<int>(puntos.Count);
            for (int i = 0; i < puntos.Count; i++)
            {
                if (puntos[i] != null)
                {
                    indicesValidos.Add(i);
                }
            }

            if (indicesValidos.Count == 0)
            {
                return 0;
            }

            int rIdx = Random.Range(0, indicesValidos.Count);
            return indicesValidos[rIdx];
        }

        float r = Random.value * total;

        for (int i = 0; i < puntos.Count; i++)
        {
            ChestSpawnPoint p = puntos[i];
            if (p == null || p.peso <= 0f)
            {
                continue;
            }

            if (r < p.peso)
            {
                return i;
            }

            r -= p.peso;
        }

        for (int i = puntos.Count - 1; i >= 0; i--)
        {
            ChestSpawnPoint p = puntos[i];
            if (p != null && p.peso > 0f)
            {
                return i;
            }
        }

        return 0;
    }
}
