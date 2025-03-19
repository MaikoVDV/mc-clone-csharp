using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mc_clone.src.WorldData.Blocks
{
    public readonly struct BlockQuery
    {
        private readonly Block block;
        public Block Block { get { return block; } }

        private readonly BlockData data;
        public BlockData Data { get { return data; } }

        public BlockQuery(Block block, BlockData data)
        {
            this.block = block;
            this.data = data;
        }
        public T GetBlockData<T>() where T : BlockData
        {
            return this.data as T;
        }
    }
}
