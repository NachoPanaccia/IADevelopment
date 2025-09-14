using System;
using UnityEngine;

public class ChestPressedLogic : MonoBehaviour
{
    [Serializable]
    public class Drop
    {
        public string name;
        [Min(0f)] public float weight = 1f;
    }

    [SerializeField]
    private Drop[] drops = {
        new Drop{ name="Poción", weight=20 },
        new Drop{ name="Espada", weight=5 },
        new Drop{ name="Anillo", weight=5 },
        new Drop{ name="Báculo", weight=1 },
        new Drop{ name="(Nada)", weight=69 },
    };

    public void OnChestPressed()
    {
        var d = RollWeighted();
        if (d == null) { Debug.LogWarning("[Chest] Sin drops válidos."); return; }
        Debug.Log($"[Chest] Premio: {d.name}");
    }

    private Drop RollWeighted()
    {
        float total = 0f;
        foreach (var x in drops) if (x != null && x.weight > 0f) total += x.weight;
        if (total <= 0f) return null;

        float r = UnityEngine.Random.value * total;
        foreach (var x in drops)
        {
            if (x == null || x.weight <= 0f) continue;
            if (r < x.weight) return x;
            r -= x.weight;
        }
        // fallback
        for (int i = drops.Length - 1; i >= 0; --i)
            if (drops[i] != null && drops[i].weight > 0f) return drops[i];
        return null;
    }
}
