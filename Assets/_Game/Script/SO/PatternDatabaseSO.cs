using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(
    fileName = "PatternDatabase",
    menuName = "Slot/Pattern Database"
)]
public class PatternDatabaseSO : ScriptableObject
{
    public List<SlotPatternSO> patterns;
}
