using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class ChestPressedLogic : MonoBehaviour
{
    // �No cambies el orden!
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

    // cache para acceso r�pido por rareza
    private readonly Dictionary<Rarity, List<ChestDropDB.DropDef>> _byRarity = new();

    void Awake()
    {
        BuildIndex(); // indexa el cat�logo definido en c�digo
    }

    public void OnChestPressed()
    {
        var r = RollRarity();
        if (r == Rarity.Nada)
        {
            Debug.Log("[Chest] No cay� nada (Rareza: Nada).");
            return;
        }

        var item = PickFromRarity(r);
        if (item == null)
        {
            Debug.LogWarning($"[Chest] Rareza {r} seleccionada, pero no hay �tems de esa rareza en el cat�logo de c�digo.");
            return;
        }

        Debug.Log($"[Chest] Premio: {item.name}  (Rareza: {r})");
        // TODO: ac� instanciar/a�adir a inventario si quer�s
    }

    // ---------- n�cleo ruleta por rareza ----------
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
        foreach (var def in ChestDropDB.All) // cat�logo definido en c�digo (no serializado)
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