using System;
using System.Collections.Generic;
using UnityEngine;

/// - Emite OnRewardRolled(item, rarity) para que la UI y el LevelManager reaccionen.
[DisallowMultipleComponent]
public sealed class ChestPressedLogic : MonoBehaviour
{
    public static event Action<ChestDropDB.DropDef, Rarity> OnRewardRolled;

    public enum Rarity { Nada = 0, Normal = 1, Rara = 2, Epica = 3, Legendaria = 4 }

    [Serializable]
    public class RarityWeight
    {
        public Rarity rarity;
        [Min(0f)] public float weight = 1f;
    }

    [Header("Pesos por rareza")]
    [SerializeField]
    private RarityWeight[] rarityWeights =
    {
        new RarityWeight { rarity = Rarity.Nada,       weight = 50f },
        new RarityWeight { rarity = Rarity.Normal,     weight = 35f },
        new RarityWeight { rarity = Rarity.Rara,       weight = 10f },
        new RarityWeight { rarity = Rarity.Epica,      weight = 4f  },
        new RarityWeight { rarity = Rarity.Legendaria, weight = 1f  },
    };

    // Índice: por cada rareza, la lista de ítems en esa rareza. Se construye desde ChestDropDB.All
    private readonly Dictionary<Rarity, List<ChestDropDB.DropDef>> _byRarity = new();

    private void Awake()
    {
        BuildIndex(); // Preparamos el índice por rareza antes de usar la ruleta
    }

    // Llamado por ChestController cuando el cofre terminó su animación y quedó presionado.
    public void OnChestPressed()
    {
        var chosenRarity = RollRarity();

        if (chosenRarity == Rarity.Nada)
        {
            // Nada = avisamos con null y rareza Nada
            OnRewardRolled?.Invoke(null, Rarity.Nada);
            return;
        }

        // Elegimos ítem al azar dentro de la rareza elegida
        var item = PickFromRarity(chosenRarity);
        if (item == null)
        {
            // Si el catálogo no tiene nada de esa rareza, lo tratamos como qye no habia nada en el cofre
            OnRewardRolled?.Invoke(null, Rarity.Nada);
            return;
        }

        // Disparamos el evento para que la UI de recompensa se muestre
        OnRewardRolled?.Invoke(item, chosenRarity);
    }

    // Ruleta por rareza
    private Rarity RollRarity()
    {
        // Sumamos pesos
        float total = 0f;
        for (int i = 0; i < rarityWeights.Length; i++)
        {
            var w = rarityWeights[i];
            if (w != null && w.weight > 0f)
            {
                total += w.weight;
            }
        }

        // Si todos los pesos son 0, devolvemos Nada
        if (total <= 0f)
        {
            return Rarity.Nada;
        }

        float r = UnityEngine.Random.value * total;

        for (int i = 0; i < rarityWeights.Length; i++)
        {
            var w = rarityWeights[i];
            if (w == null || w.weight <= 0f)
            {
                continue;
            }

            if (r < w.weight)
            {
                return w.rarity;
            }

            r -= w.weight;
        }

        // Fallback: si por precisión no entró en ninguna, devolvemos la última con peso
        for (int i = rarityWeights.Length - 1; i >= 0; --i)
        {
            var w = rarityWeights[i];
            if (w != null && w.weight > 0f)
            {
                return w.rarity;
            }
        }
        return Rarity.Nada;
    }

    // Devuelve un ítem aleatorio dentro de la rareza.
    private ChestDropDB.DropDef PickFromRarity(Rarity rarity)
    {
        if (!_byRarity.TryGetValue(rarity, out var list) || list == null || list.Count == 0)
        {
            return null;
        }

        int idx = UnityEngine.Random.Range(0, list.Count);
        return list[idx];
    }

    // Construye el índice a partir del catálogo.
    private void BuildIndex()
    {
        _byRarity.Clear();

        foreach (var def in ChestDropDB.All)
        {
            if (def == null)
            {
                continue;
            }

            if (!_byRarity.TryGetValue(def.rarity, out var list))
            {
                list = new List<ChestDropDB.DropDef>(16);
                _byRarity.Add(def.rarity, list);
            }

            list.Add(def);
        }
    }
}
