using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class ChestPressedLogic : MonoBehaviour
{
    public static event System.Action<ChestDropDB.DropDef, Rarity> OnRewardRolled;

    public enum Rarity { Nada = 0, Normal = 1, Rara = 2, Epica = 3, Legendaria = 4 }

    [Serializable]
    public class RarityWeight
    {
        public Rarity rarity;
        [Min(0f)] public float weight = 1f;
    }

    [Header("Pesos por rareza (ruleta)")]
    [SerializeField]
    private RarityWeight[] rarityWeights = new RarityWeight[]
    {
        new RarityWeight{ rarity = Rarity.Nada,       weight = 50f },
        new RarityWeight{ rarity = Rarity.Normal,     weight = 35f },
        new RarityWeight{ rarity = Rarity.Rara,       weight = 10f },
        new RarityWeight{ rarity = Rarity.Epica,      weight = 4f  },
        new RarityWeight{ rarity = Rarity.Legendaria, weight = 1f  },
    };

    // cache para acceso rápido por rareza
    private readonly Dictionary<Rarity, List<ChestDropDB.DropDef>> _byRarity = new();

    void Awake()
    {
        BuildIndex(); // indexa el catálogo definido en código
    }

    public void OnChestPressed()
    {
        var chosenRarity = RollRarity();
        if (chosenRarity == Rarity.Nada)
        {
            Debug.Log("[Chest] No cayó nada (Rareza: Nada).");
            OnRewardRolled?.Invoke(null, Rarity.Nada);
            return;
        }

        var item = PickFromRarity(chosenRarity); // <- esta es la firma correcta
        if (item == null)
        {
            Debug.LogWarning($"[Chest] Rareza {chosenRarity} seleccionada, pero no hay ítems de esa rareza.");
            OnRewardRolled?.Invoke(null, Rarity.Nada);
            return;
        }

        Debug.Log($"[Chest] Premio: {item.name}  (Rareza: {chosenRarity})");
        OnRewardRolled?.Invoke(item, chosenRarity);
    }

    // ---------- núcleo ruleta por rareza ----------
    private Rarity RollRarity()
    {
        float total = 0f;
        for (int i = 0; i < rarityWeights.Length; i++)
        {
            var w = rarityWeights[i];
            if (w != null && w.weight > 0f) total += w.weight;
        }
        if (total <= 0f) return Rarity.Nada;

        float r = UnityEngine.Random.value * total;
        for (int i = 0; i < rarityWeights.Length; i++)
        {
            var w = rarityWeights[i];
            if (w == null || w.weight <= 0f) continue;
            if (r < w.weight) return w.rarity;
            r -= w.weight;
        }
        // fallback defensivo
        for (int i = rarityWeights.Length - 1; i >= 0; --i)
        {
            var w = rarityWeights[i];
            if (w != null && w.weight > 0f) return w.rarity;
        }
        return Rarity.Nada;
    }

    private ChestDropDB.DropDef PickFromRarity(Rarity rarity)
    {
        if (!_byRarity.TryGetValue(rarity, out var list) || list == null || list.Count == 0)
            return null;

        int idx = UnityEngine.Random.Range(0, list.Count);
        return list[idx];
    }

    private void BuildIndex()
    {
        _byRarity.Clear();
        foreach (var def in ChestDropDB.All) // catálogo definido en código (no serializado)
        {
            if (def == null) continue;
            if (!_byRarity.TryGetValue(def.rarity, out var list))
            {
                list = new List<ChestDropDB.DropDef>(16);
                _byRarity.Add(def.rarity, list);
            }
            list.Add(def);
        }
    }
}