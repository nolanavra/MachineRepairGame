using MachineRepair.Grid;
using System;
using UnityEngine;

namespace MachineRepair
{
    [Serializable]
    public struct PortLocal
    {
        public Vector2Int cell;
        public PortType port;
        public bool isInput;
    }

    [CreateAssetMenu(menuName = "Espresso/Port Set")]
    public class PortDef : ScriptableObject
    {
        public PortLocal[] ports;
    }
}
