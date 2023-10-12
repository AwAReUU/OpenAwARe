using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFactory<Product>
{
    public Product Create();
}

public class ChunkGridFactory<Data> : IFactory<ChunkGrid<Data>>
{
    const int dim = 3;
    (int, int, int) gridSize, chunkSize;
    IFactory<Chunk<Data>> chunkFactory;

    public ChunkGridFactory((int, int, int) gridSize, (int, int, int) chunkSize)
    {
        this.gridSize = gridSize;
        this.chunkSize = chunkSize;
        this.chunkFactory = new ChunkFactory<Data>(chunkSize);
    }

    public ChunkGridFactory((int, int, int) gridSize, (int, int, int) chunkSize, IFactory<Chunk<Data>> chunkFactory)
    {
        this.gridSize = gridSize;
        this.chunkSize = chunkSize;
        this.chunkFactory = chunkFactory;
    }

    public ChunkGrid<Data> Create()
    {
        // Change representation for ease of computation
        int[] gridSizeA = new int[] { gridSize.Item1, gridSize.Item2, gridSize.Item3 };
        int[] chunkSizeA = new int[] { chunkSize.Item1, chunkSize.Item2, chunkSize.Item3 };

        // Compute how many chunks are needed in each dimension/direction
        int[] chunkGridSize = new int[dim];
        for (int i = 0; i < dim; i++) { chunkGridSize[i] = (gridSizeA[i] - 1) / chunkSizeA[i] + 1; }

        // Create all chunks
        int cgx = chunkGridSize[0], cgy = chunkGridSize[1], cgz = chunkGridSize[2];
        Chunk<Data>[,,] chunks = new Chunk<Data>[cgx, cgy, cgz];
        for (int i = 0; i < cgx; i++)
            for (int j = 0; j < cgy; j++)
                for (int k = 0; k < cgz; k++)
                    chunks[i, j, k] = chunkFactory.Create();

        // Create grid of chunks
        return new ChunkGrid<Data>(gridSizeA, chunkSizeA, chunks);
    }


}

public class ChunkFactory<Data> : IFactory<Chunk<Data>>
{
    const int dim = 3;
    (int, int, int) chunkSize;

    public ChunkFactory((int, int, int) chunkSize)
    {
        this.chunkSize = chunkSize;
    }

    public Chunk<Data> Create()
    {
        // Create the subgrid stored in chunk
        Data[,,] chunkData = new Data[chunkSize.Item1, chunkSize.Item2, chunkSize.Item3];
        // Create the chunk storing and managing the data
        return new Chunk<Data>(chunkData);
    }


}
