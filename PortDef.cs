using MachineRepair.Grid;
using System;
using UnityEngine;

namespace MachineRepair
{
    [Serializable]
    public struct PortLocal
    {
        [Tooltip("Unique identifier for mapping connections between ports.")]
        public string id;
        public Vector2Int cell;
        public PortType port;
        public bool isInput;
    }

    [Serializable]
    public struct PortConnection
    {
        [Tooltip("Identifier of the input port this mapping starts from.")]
        public string inputId;

        [Tooltip("Identifier of the output port this input feeds. Leave empty when routing to a simulation variable instead of an output port.")]
        public string outputId;

        [Tooltip("Optional simulation variable name used when the input feeds into an internal machine connection instead of a physical output port.")]
        public string simulationVariable;

        [Tooltip("Type of port being routed (water, power, or signal).")]
        public PortType portType;
    }

    [CreateAssetMenu(menuName = "Espresso/Port Set")]
    public class PortDef : ScriptableObject
    {
        public PortLocal[] ports;

        [Tooltip("Defines how inputs connect to outputs or simulation variables for this component.")]
        public PortConnection[] connections;
    }
}
