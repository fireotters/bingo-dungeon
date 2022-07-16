using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Level
{
    [CreateAssetMenu(fileName = "data", menuName = "GridData", order = 0)]
    public class GridData : ScriptableObject
    {
        public List<TextMeshPro> tileNumbers;
    }
}