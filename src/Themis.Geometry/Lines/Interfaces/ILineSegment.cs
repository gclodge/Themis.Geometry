using MathNet.Numerics.LinearAlgebra;

namespace Themis.Geometry.Lines.Interfaces
{
    public interface ILineSegment
    {
        Vector<double> A { get; }
        Vector<double> B { get; }
        Vector<double> Unit { get; }

        double Length { get; }

        double GetStation(Vector<double> v);
        double DistanceToPoint(Vector<double> v);

        Vector<double> GetClosestPoint(Vector<double> v);
        Vector<double> ExtractPoint(double station);
    }
}
