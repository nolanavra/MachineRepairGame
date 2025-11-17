using UnityEngine;

namespace MachineRepair.Grid
{
    [System.Serializable]
    public struct cellDef
    {
        public int index; 
        public CellPlaceability placeability;

        // “Contents” of the cell:
        public ComponentType component;    // machine / fixture
        public WireType wire;                  // electrical
        public bool pipe;                  // plumbing


        // Convenience helpers
        public bool HasComponent => component != ComponentType.None;
        public bool HasWire => wire != WireType.None;
        public bool HasPipe => pipe;
    };
}
