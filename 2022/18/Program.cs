using System.Collections.Immutable;
using _18.Models;

namespace _18;

internal sealed class Program
{
    private static readonly string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var cubes = (await File.ReadAllLinesAsync(_inputLocation))
            .Select(ParseCube)
            .ToImmutableArray();

        var result1 = cubes.Sum(x => x.GetExposedSurfaces(cubes));
        var result2 = CalculateInternalSurface(FillExteriorWithWater(cubes));

        Console.WriteLine($"First answer: {result1}");
        Console.WriteLine($"Second answer: {result2}");
    }

    /// <summary>
    /// Parses a cube from raw input data.
    /// </summary>
    /// <param name="input">The raw input data.</param>
    /// <returns>A parsed <see cref="Cube"/>.</returns>
    private static Cube ParseCube(string input)
    {
        var splitInput = input.Split(',');
        return new(int.Parse(splitInput[0]), int.Parse(splitInput[1]), int.Parse(splitInput[2]));
    }

    /// <summary>
    /// Gets the dimensions of a 3D grid of <see cref="Cube"/>s.
    /// </summary>
    /// <param name="cubes">The 3D grid of cubes.</param>
    /// <returns>A tuple with the width, height, and depth of the grid.</returns>
    private static (Vector Width, Vector Height, Vector Depth) GetGridDimensions(IReadOnlyCollection<Cube> cubes)
    {
        var x = new Vector(cubes.Min(x => x.X), cubes.Max(x => x.X));
        var y = new Vector(cubes.Min(x => x.Y), cubes.Max(x => x.Y));
        var z = new Vector(cubes.Min(x => x.Z), cubes.Max(x => x.Z));

        return (x, y, z);
    }

    /// <summary>
    /// Creates a 3D grid that is exactly 1 length bigger than <paramref name="cubes"/> in all dimensions
    /// and fills it with cubes from the exterior towards the center, without overlapping the positions
    /// already occupied in <paramref name="cubes"/>.
    /// </summary>
    /// <param name="cubes">The 3D grid of cubes.</param>
    /// <returns>
    /// A 3D grid of cubes representing a rectangular prism that does not contain cubes in
    /// the positions defined in <paramref name="cubes"/>.
    /// </returns>
    private static IReadOnlySet<Cube> FillExteriorWithWater(IReadOnlyCollection<Cube> cubes)
    {
        var (width, height, depth) = GetGridDimensions(cubes);

        // Adaptation of Dijkstra
        var waterCubes = new HashSet<Cube>();
        var candidateCubes = new Queue<Cube>();
        candidateCubes.Enqueue(new(width.Start - 1, height.Start - 1, depth.Start - 1));

        while (candidateCubes.TryDequeue(out var visited))
        {
            if (waterCubes.Contains(visited))
                continue;

            var candidates = GetNeighboringCubes(visited)
                .Where(x =>
                    x.X >= width.Start - 1 && x.X <= width.End + 1
                    && x.Y >= height.Start - 1 && x.Y <= height.End + 1
                    && x.Z >= depth.Start - 1 && x.Z <= depth.End + 1
                    && !waterCubes.Contains(x) && !cubes.Contains(x)
                );

            foreach (var candidate in candidates)
                candidateCubes.Enqueue(candidate);

            waterCubes.Add(visited);
        }

        return waterCubes;
    }

    /// <summary>
    /// Gets all neighboring cubes of a given <paramref name="cube"/>.
    /// </summary>
    /// <param name="cube">The cube to get the neighbors from.</param>
    /// <returns>The neighboring cubes of <paramref name="cube"/>.</returns>
    private static IEnumerable<Cube> GetNeighboringCubes(Cube cube)
    {
        yield return new(cube.X + 1, cube.Y, cube.Z);
        yield return new(cube.X - 1, cube.Y, cube.Z);
        yield return new(cube.X, cube.Y + 1, cube.Z);
        yield return new(cube.X, cube.Y - 1, cube.Z);
        yield return new(cube.X, cube.Y, cube.Z + 1);
        yield return new(cube.X, cube.Y, cube.Z - 1);
    }

    /// <summary>
    /// Calculates the interior surface of a 3D grid.
    /// </summary>
    /// <param name="cubesWithWater">A 3D grid in the format of a rectangular prism.</param>
    /// <remarks>The grid must be a rectangular prism in the outside.</remarks>
    /// <returns>The internal area of the grid.</returns>
    private static int CalculateInternalSurface(IReadOnlyCollection<Cube> cubesWithWater)
    {
        var (width, height, depth) = GetGridDimensions(cubesWithWater);
        var totalArea = cubesWithWater.Sum(x => x.GetExposedSurfaces(cubesWithWater));

        // Formula for the area of a rectangular prism:
        // 2 * (wd + hd + wh)
        var externalArea = 2 * ((depth.Length * width.Length) + (depth.Length * height.Length) + (width.Length * height.Length));

        return totalArea - externalArea;
    }
}