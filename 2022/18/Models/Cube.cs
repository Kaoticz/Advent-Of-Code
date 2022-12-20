namespace _18.Models;

/// <summary>
/// Represents a cube.
/// </summary>
/// <param name="X">The width position at the center of the cube.</param>
/// <param name="Y">The height position at the center of the cube.</param>
/// <param name="Z">The depth position at the center of the cube.</param>
internal sealed record Cube(int X, int Y, int Z)
{
    /// <summary>
    /// Gets the amount of surfaces that are not in direct contact with another cube.
    /// </summary>
    /// <param name="cubes">A grid of cubes.</param>
    /// <returns>The amount of exposed surfaces of this cube.</returns>
    public int GetExposedSurfaces(IReadOnlyCollection<Cube> cubes)
    {
        var surfaces = 6;

        // Top
        if (cubes.Any(x => x.X == X && x.Y == Y + 1 && x.Z == Z))
            surfaces -= 1;

        // Bottom
        if (cubes.Any(x => x.X == X && x.Y == Y - 1 && x.Z == Z))
            surfaces -= 1;

        // Left
        if (cubes.Any(x => x.X == X - 1 && x.Y == Y && x.Z == Z))
            surfaces -= 1;

        // Right
        if (cubes.Any(x => x.X == X + 1 && x.Y == Y && x.Z == Z))
            surfaces -= 1;

        // Front
        if (cubes.Any(x => x.X == X && x.Y == Y && x.Z == Z + 1))
            surfaces -= 1;

        // Back
        if (cubes.Any(x => x.X == X && x.Y == Y && x.Z == Z - 1))
            surfaces -= 1;

        return surfaces;
    }
}