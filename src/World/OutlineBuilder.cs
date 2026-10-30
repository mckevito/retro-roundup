using System.Collections.Generic;
using Godot;

namespace RetroRoundup.World;

/// <summary>
/// Collects black faces (triangles) and outline edges (line segments), then turns them into two meshes.
/// Faces are pulled in slightly (FaceInset) so the outlines on their edges always draw in front.
/// </summary>
public class OutlineBuilder
{
    public readonly List<Vector3> Faces = new();
    public readonly List<Vector3> Lines = new();
    public float FaceInset = 0.02f;

    public void AddLine(Vector3 a, Vector3 b)
    {
        Lines.Add(a);
        Lines.Add(b);
    }

    /// <summary>Outline through the corners in order, closed back to the first.</summary>
    public void AddLoop(params Vector3[] corners)
    {
        for (int i = 0; i < corners.Length; i++)
            AddLine(corners[i], corners[(i + 1) % corners.Length]);
    }

    public void AddQuadFace(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        Faces.AddRange(new[] { a, b, c, a, c, d });
    }

    /// <summary>A box from any two opposite corners: six black faces and twelve outline edges.</summary>
    public void AddBox(Vector3 cornerA, Vector3 cornerB)
    {
        Vector3 lo = cornerA.Min(cornerB);
        Vector3 hi = cornerA.Max(cornerB);
        Vector3 inset = new(
            Mathf.Min(FaceInset, (hi.X - lo.X) * 0.25f),
            Mathf.Min(FaceInset, (hi.Y - lo.Y) * 0.25f),
            Mathf.Min(FaceInset, (hi.Z - lo.Z) * 0.25f));
        Vector3 iLo = lo + inset;
        Vector3 iHi = hi - inset;

        for (int axis = 0; axis < 3; axis++)
        {
            for (int side = 0; side < 2; side++)
            {
                var q = new Vector3[4];
                int[][] corners = { new[] { 0, 0 }, new[] { 1, 0 }, new[] { 1, 1 }, new[] { 0, 1 } };
                for (int k = 0; k < 4; k++)
                {
                    var v = new int[3];
                    v[axis] = side;
                    v[(axis + 1) % 3] = corners[k][0];
                    v[(axis + 2) % 3] = corners[k][1];
                    q[k] = Pick(v, iLo, iHi);
                }
                AddQuadFace(q[0], q[1], q[2], q[3]);
            }
            for (int a = 0; a < 2; a++)
            {
                for (int b = 0; b < 2; b++)
                {
                    var v0 = new int[3];
                    v0[(axis + 1) % 3] = a;
                    v0[(axis + 2) % 3] = b;
                    var v1 = (int[])v0.Clone();
                    v1[axis] = 1;
                    AddLine(Pick(v0, lo, hi), Pick(v1, lo, hi));
                }
            }
        }
    }

    static Vector3 Pick(int[] v, Vector3 lo, Vector3 hi) =>
        new(v[0] == 1 ? hi.X : lo.X, v[1] == 1 ? hi.Y : lo.Y, v[2] == 1 ? hi.Z : lo.Z);

    public ArrayMesh BuildFacesMesh(Material material) => Build(Faces, Mesh.PrimitiveType.Triangles, material);

    public ArrayMesh BuildLinesMesh(Material material) => Build(Lines, Mesh.PrimitiveType.Lines, material);

    static ArrayMesh Build(List<Vector3> points, Mesh.PrimitiveType primitive, Material material)
    {
        var mesh = new ArrayMesh();
        if (points.Count == 0)
            return mesh;
        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = points.ToArray();
        mesh.AddSurfaceFromArrays(primitive, arrays);
        mesh.SurfaceSetMaterial(0, material);
        return mesh;
    }
}
