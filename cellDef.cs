using UnityEngine;
using MachineRepair;

namespace MachineRepair.Grid
{
    [System.Serializable]
    public struct cellDef
    {
        public int index;
        public CellPlaceability placeability;

        // Contents of the cell:
        public MachineComponent component;    // machine / fixture
        public WireType wire;                  // electrical
        public PlacedWire wireInstance;        // placed wire data
        public bool pipe;                  // plumbing


        // Convenience helpers
        public bool HasComponent => component != null;
        public bool HasWire => wireInstance != null || wire != WireType.None;
        public bool HasPipe => pipe;
    };
}
