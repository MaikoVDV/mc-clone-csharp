using System.Collections.Generic;

namespace mc_clone.src.WorldData.Blocks
{
    public class BlockDataStore
    {
        public Dictionary<BlockCoordinates, BlockData> storageDict;
        public BlockDataStore()
        {
            storageDict = new();
        }

        public BlockData this[BlockCoordinates key]
        {
            get
            {
                if (storageDict.TryGetValue(key, out var data))
                {
                    return data;
                } else
                {
                    return null;
                }
            }
            set
            {
                storageDict[key] = value;
            }
        }
    }

    public abstract class BlockData { }
}
