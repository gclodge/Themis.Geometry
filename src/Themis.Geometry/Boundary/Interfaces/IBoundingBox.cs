namespace Themis.Geometry.Boundary.Interfaces
{
    internal interface IBoundingBox
    {
        double MinX { get; }
        double MinY { get; }
        double MinZ { get; }

        double MaxX { get; }
        double MaxY { get; }
        double MaxZ { get; }

        double Area { get; }
        double Volume { get; }

        double CentroidX { get; }
        double CentroidY { get; }
        double CentroidZ { get; }

        bool Contains(double x, double y, double z);
    }
}
