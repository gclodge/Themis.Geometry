using Themis.Geometry.Lines.Interfaces;

using MathNet.Numerics.LinearAlgebra;

namespace Themis.Geometry.Lines
{
    public class LineSegment : ILineSegment, IEquatable<LineSegment>
    {
        /// <summary>
        /// Starting Vertex of the LineSegment
        /// </summary>
        public Vector<double> A { get; protected set; }
        /// <summary>
        /// Terminating Vertex of the LineSegment
        /// </summary>
        public Vector<double> B { get; protected set; }

        /// <summary>
        /// The Vector (Euclidean) magnitude of the current LineSegment formed by A->B
        /// </summary>
        public double Length => (B - A).L2Norm();

        /// <summary>
        /// Gets the Unit Vector pointing in the direction of the current LineSegment formed by A->B
        /// </summary>
        public Vector<double> Unit => (B - A) / Length;

        /// <summary>
        /// Construct a new LineSegment deep-copying input vertices
        /// </summary>
        /// <param name="a">Starting Vertex (A) of the LineSegment</param>
        /// <param name="b">Terminating Vertex (B) of the LineSegment</param>
        public LineSegment(Vector<double> a, Vector<double> b)
        {
            this.A = a.Clone();
            this.B = b.Clone();
        }

        #region ILineSegment Methods
        /// <summary>
        /// Get the projected magnitude (station) of the position vector v upon the infinite line formed by LineSegment A->B
        /// </summary>
        /// <param name="v">Input position vector</param>
        /// <returns>The scalar projected magnitude (station)</returns>
        public double GetStation(Vector<double> v)
        {
            //< Get the vector A->V, then get magnitude projected onto our Unit vector
            return (v - A).DotProduct(Unit);
        }

        /// <summary>
        /// Get the minimum distance between the input position vector 'v' and the LineSegment A->B
        /// </summary>
        /// <param name="v">Input position vector</param>
        /// <returns>The scalar distance between v and the closest point on A->B</returns>
        public double DistanceToPoint(Vector<double> v)
        {
            //< Get the closest point on the LineSegment - then return the distance from it to 'v'
            return (GetClosestPoint(v) - v).L2Norm();
        }

        /// <summary>
        /// Extract the closest position along the LineSegment A->B to the given position vector 'v'
        /// NOTE:  Extracted points are limited to the discrete LineSegment A->B (station never exceeds [0, Length])
        /// </summary>
        /// <param name="v">Input position vector</param>
        /// <returns>Position vector of closest point on LineSegment A->B</returns>
        public Vector<double> GetClosestPoint(Vector<double> v)
        {
            double station = GetStation(v);

            if (station <= 0) return A; //< If prior to Starting Vertex - just return A
            if (station >= Length) return B; //< If after the Terminating Vertex - just return B

            return ExtractPoint(station);
        }

        /// <summary>
        /// Extract a point along the LineSegment A->B at the given input station (starting at A)
        /// </summary>
        /// <param name="station">Scalar station (relative to A) of desired result position</param>
        /// <returns>Position vector of point along A->B at input scalar station (T)</returns>
        public Vector<double> ExtractPoint(double station)
        {
            return A + (station * Unit);
        }
        #endregion

        #region IEquatable
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            return Equals(obj as LineSegment);
        }

        public bool Equals(LineSegment? other)
        {
            return other != null &&
                   A.SequenceEqual(other.A) &&
                   B.SequenceEqual(other.B);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(A.ToArray(), B.ToArray());
        }
        #endregion
    }
}
