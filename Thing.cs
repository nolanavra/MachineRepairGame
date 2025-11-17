using MachineRepair.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace MachineRepair {
    [RequireComponent(typeof(Transform))]
    public class MachineComponent : MonoBehaviour {
        [Header("References")]
        [SerializeField] public ThingDef def;
        [SerializeField] private GridManager grid;
        public FootprintMask footprint;
        public int rotation;
    }

}

