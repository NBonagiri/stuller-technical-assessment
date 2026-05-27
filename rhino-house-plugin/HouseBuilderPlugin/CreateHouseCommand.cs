using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace HouseBuilderPlugin
{
    public class CreateHouseCommand : Command
    {
        public override string EnglishName => "CreateHouse";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Point3d basePoint;
            Result pointResult = RhinoGet.GetPoint("Pick base point", false, out basePoint);

            if (pointResult != Result.Success)
                return pointResult;

            double size = 10;
            Result sizeResult = RhinoGet.GetNumber("Enter house size", false, ref size);

            if (sizeResult != Result.Success)
                return sizeResult;

            Box house = new Box(
                Plane.WorldXY,
                new Interval(basePoint.X, basePoint.X + size),
                new Interval(basePoint.Y, basePoint.Y + size),
                new Interval(basePoint.Z, basePoint.Z + size));

            doc.Objects.AddBox(house);

            Box door = new Box(
                Plane.WorldXY,
                new Interval(basePoint.X + size * 0.4, basePoint.X + size * 0.6),
                new Interval(basePoint.Y, basePoint.Y + size * 0.1),
                new Interval(basePoint.Z, basePoint.Z + size * 0.5));

            doc.Objects.AddBox(door);

            Box chimney = new Box(
                Plane.WorldXY,
                new Interval(basePoint.X + size * 0.1, basePoint.X + size * 0.2),
                new Interval(basePoint.Y + size * 0.7, basePoint.Y + size * 0.8),
                new Interval(basePoint.Z + size, basePoint.Z + size * 1.5));

            doc.Objects.AddBox(chimney);

            Point3d p1 = new Point3d(basePoint.X, basePoint.Y, basePoint.Z + size);
            Point3d p2 = new Point3d(basePoint.X + size, basePoint.Y, basePoint.Z + size);
            Point3d p3 = new Point3d(basePoint.X + size / 2, basePoint.Y + size / 2, basePoint.Z + size * 1.5);
            Point3d p4 = new Point3d(basePoint.X, basePoint.Y + size, basePoint.Z + size);
            Point3d p5 = new Point3d(basePoint.X + size, basePoint.Y + size, basePoint.Z + size);

            Mesh roofMesh = new Mesh();

            roofMesh.Vertices.Add(p1);
            roofMesh.Vertices.Add(p2);
            roofMesh.Vertices.Add(p3);
            roofMesh.Vertices.Add(p4);
            roofMesh.Vertices.Add(p5);

            roofMesh.Faces.AddFace(0, 1, 2);
            roofMesh.Faces.AddFace(0, 2, 3);
            roofMesh.Faces.AddFace(1, 4, 2);
            roofMesh.Faces.AddFace(3, 2, 4);

            doc.Objects.AddMesh(roofMesh);

            doc.Views.Redraw();

            return Result.Success;
        }
    }
}