using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Slot/Pay Table Database")]
public class PayTableDatabaseSO : ScriptableObject
{
    public List<PayTableEntrySO> entries;

    Dictionary<int, PayTableEntrySO> lookup;

    void OnEnable()
    {
        lookup = new Dictionary<int, PayTableEntrySO>();
        foreach (var e in entries)
            lookup[e.symbolId] = e;
    }

    public float GetMultiplier(int symbolId, int count)
    {
        if (lookup.TryGetValue(symbolId, out var entry))
            return entry.GetMultiplier(count);

        return 0f;
    }
}
