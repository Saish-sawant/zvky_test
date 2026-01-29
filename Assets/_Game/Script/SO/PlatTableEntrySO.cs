using UnityEngine;

[CreateAssetMenu(menuName = "Slot/Pay Table Entry")]
public class PayTableEntrySO : ScriptableObject
{
    public int symbolId;

    public float pay3;
    public float pay4;
    public float pay5;

    public float GetMultiplier(int count)
    {
        switch (count)
        {
            case 3: return pay3;
            case 4: return pay4;
            case 5: return pay5;
            default: return 0f;
        }
    }
}
