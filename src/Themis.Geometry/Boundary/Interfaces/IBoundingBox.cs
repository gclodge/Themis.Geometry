﻿namespace Themis.Geometry.Boundary.Interfaces
{
    public interface IBoundingBox
    {
        /// <summary>
        /// Minimum X-Coordinate of the IBoundingBox
        /// </summary>
        double MinX { get; }
        /// <summary>
        /// Minimum Y-Coordinate of the IBoundingBox
        /// </summary>
        double MinY { get; }
        /// <summary>
        /// Minimum Z-Coordinate of the IBoundingBox
        /// </summary>
        double MinZ { get; }

        /// <summary>
        /// Maximum X-Coordinate of the IBoundingBox
        /// </summary>
        double MaxX { get; }
        /// <summary>
        /// Maximum Y-Coordinate of the IBoundingBox
        /// </summary>
        double MaxY { get; }
        /// <summary>
        /// Maximum Z-Coordinate of the IBoundingBox
        /// </summary>
        double MaxZ { get; }

        /// <summary>
        /// The total Width (MaxX - MinX) of the IBoundingBox
        /// </summary>
        double Width { get; }
        /// <summary>
        /// The total Height (MaxY - MinY) of the IBoundingBox
        /// </summary>
        double Height { get; }
        /// <summary>
        /// The total Depth (MaxZ - MinZ) of the IBoundingBox
        /// </summary>
        double Depth { get; }

        /// <summary>
        /// The total Area (Width * Height) of the 2D (X & Y) projection of the IBoundingBox 
        /// </summary>
        double Area { get; }
        /// <summary>
        /// The total Volume (Width * Height * Depth) of the 3D IBoundingBox
        /// </summary>
        double Volume { get; }

        /// <summary>
        /// The X-Coordinate of the Centroid of the IBoundingBox
        /// </summary>
        double CentroidX { get; }
        /// <summary>
        /// The Y-Coordinate of the Centroid of the IBoundingBox
        /// </summary>
        double CentroidY { get; }
        /// <summary>
        /// The Z-Coordinate of the Centroid of the IBoundingBox
        /// </summary>
        double CentroidZ { get; }

        /// <summary>
        /// Checks if this 2/3D IBoundingBox contains the input 2/3D position
        /// </summary>
        /// <param name="x">Input position X</param>
        /// <param name="y">Input position Y</param>
        /// <param name="z">[Optional] Input position Z - defaults to double.NaN</param>
        /// <returns></returns>
        bool Contains(double x, double y, double z = double.NaN);
        /// <summary>
        /// Check if this 2/3D IBoundingBox intersects with the other 2/3D IBoundingBox
        /// NOTE: This will resolve the 'lowest' dimensionality (so if one is 2D, they're both assumed 2D)
        /// </summary>
        /// <param name="other">IBoundingBox to check intersection against</param>
        /// <returns></returns>
        bool Intersects(IBoundingBox other);

        /// <summary>
        /// Expand the current BoundingBox extents to include the input BoundingBox by comparing maxima/minima
        /// </summary>
        /// <param name="that">Input BoundingBox to include</param>
        /// <returns></returns>
        IBoundingBox Buffer(double buffer);
        /// <summary>
        /// Extend the bounding box by an input scalar amount
        /// </summary>
        /// <param name="buffer">Scalar amount to buffer the minima & maxima by</param>
        /// <returns></returns>
        IBoundingBox ExpandToInclude(IBoundingBox that);
    }
}
