namespace AwARe.Data.Logic
{
    public class ChunkGridFactory<T>
    {
        ChunkFactory<T> chunkFactory;

        public ChunkGridFactory()
        {
            this.chunkFactory = new ChunkFactory<T>();
        }

        public ChunkGridFactory(ChunkFactory<T> chunkFactory)
        {
            this.chunkFactory = chunkFactory;
        }

        public ChunkGrid<T> Create(Point3 gridSize, Point3 chunkSize)
        {
            // Compute how many chunks are needed in each dimension/direction
            Point3 chunkGridSize = (gridSize - 1) / chunkSize + 1;

            // Create all chunks
            int cgx = chunkGridSize.x, cgy = chunkGridSize.y, cgz = chunkGridSize.z;
            Chunk<T>[,,] chunks = new Chunk<T>[cgx, cgy, cgz];
            for (int i = 0; i < cgx; i++)
                for (int j = 0; j < cgy; j++)
                    for (int k = 0; k < cgz; k++)
                        chunks[i, j, k] = chunkFactory.Create(chunkSize);

            // Create grid of chunks
            return new ChunkGrid<T>(gridSize, chunkSize, chunks);
        }
    }

    public class ChunkFactory<T>
    {
        public ChunkFactory() { }

        public Chunk<T> Create(Point3 chunkSize)
        {
            // Create the subgrid stored in chunk
            T[,,] chunkData = new T[chunkSize.x, chunkSize.y, chunkSize.z];
            // Create the chunk storing and managing the data
            return new Chunk<T>(chunkData);
        }
    }

}