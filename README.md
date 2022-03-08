# Themis.Geometry
## Overview
This project encompasses the core geometric and spatial indexing functionality that allows for efficient and reliable representation, analyis, and indexing of common geospatial data types.

The concrete geometric implementations and search trees themselves are intended to provide simple interfaces for common spatial analysis tasks and do not represent a fully composed pipeline.  Instead, ***they are the fundamental building blocks that enable consumers to quickly implement spatial analysis pipelines for datasets of significant scale***.

The library itself can be broken into a number of key components as follows:
- __Themis.Geometry__
    - [Boundary](#boundary)
    - [Lines](#lines)
    - [Triangles](#triangles)
- __Themis.Geometry.Index__
    - [KdTree](#indexkdtree)
    - [QuadTree](#indexquadtree)

# Boundary
This namespace exposes the `BoundingBox` class which represents both 2D BoundingBox and 3D BoundingCube geometries within a single concrete class.  In both dimensional cases, the `BoundingBox` is defined by its local __Minima (X,Y,Z)__ and __Maxima (X,Y,Z)__.

__Note:__  When in 2D - the Minima/Maxima Z coordinates are left as double.NaN.

Further, the `BoundingBox` implementation is at the core of the spatial indexing functionality provided by the [QuadTree](#indexquadtree).
### Usage
While the current `BoundingBox` implementation only has a default constructor it also exposes a number of Factory methods as well as a Fluent interface to generate the desired resultant geometry.

Consider the following:
```csharp

// Create a 2D BoundingBox centered at (1, 1) with minima (-1.5, -1.5) and maxima (3.5, 3.5)
var box = BoundingBox.FromPoint(1.0, 1.0, 5.0);
var box2 = BoundingBox.From(-1.5, -1.5, 3.5, 3.5);
// Resulting Minima & Maxima are equal - thus the two BoundingBoxes are equal
Assert.Equal(box, box2);
Assert.True(box.Contains(1.0, 1.0)); 

// Creates a new 3D BoundingBox with the specified elevation (Z) range
var cube = BoundingBox.From(box).WithZ(0, 100.0);
// The 3D BoundingBox can still perform both 2D & 3D containment checks
Assert.True(cube.Contains(1.0, 1.0));
Assert.True(cube.Contains(1.0, 1.0, 1.0));
```
We can also easily expand a given `BoundingBox` to include any other `BoundingBox` or simply alter the local Minima / Maxima as follows:
```csharp
// Create two BoundingBoxes that overlap and then create a 3rd that include them both
var A = BoundingBox.From(-1.5, -1.5, 3.5, 3.5);
var B = BoundingBox.From(0, 0, 5.0, 5.0);
// Can check that the two BoundingBoxes intersect/overlap as well
Assert.True(A.Intersects(B));
// Lets create a new BoundingBox by expanding A to include B
var C = A.ExpandToInclude(B);
// Can now see the maxima & minima are expanded as expected
Assert.Equal(5.0, C.MaxX);
Assert.Equal(5.0, C.MaxY);
```
Beyond the above functionality - each `BoundingBox` also exposes the following fields:
- Width (MaxX - MinX)
- Height (MaxY - MinY)
- Depth (MaxZ - MinZ)
- Area (Width * Height)
- Volume (Width * Height * Depth)
- (X,Y,Z) Coordinates of the Centroid

# Lines
This namespace exposes Themis' `LineSegment` and `LineString` implementations that are intended to be used to model 2/3D linear geometries.  While both implementations will technically function within any dimension it's recommended consumers limit their dimensionality to 2/3D.

The `LineSegment` is composed of an ordered pair of vector vertices (named A & B) and encompasses both the infinite line between A->B but also the discrete `LineSegment` formed by A->B.  Given those two components the `LineSegment` is able to efficiently do the following:
- Get the station (distance along line from A->B) of any input position
- Extract a point along the `LineSegment` at any station (distance along the line)
- Extract the nearest point on the `LineSegment` to any input position
- Get the minimum distance to the `LineSegment` from any input position

The `LineString` is composed of two or more vertices as an ordered, connected, set of linear geometries.  In order to build the `LineString` we create a `LineSegment` between each vertex and the following vertex (excluding the final vertex). While in 2D this could represent the connectivity map of power poles, the 3D extension can be used to model wire geometries or other complex shapes composed of many lines.  Given this, the `LineString` exposes the following functionality:
- Extract the nearest point from all contained `LineSegment` geometries to any input position
- Calculate total 2D & 3D Length of all contained `LineSegment` geometries
## Usage
In order to instantiate a `LineSegment` we'll need to first have two [Vectors](https://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra/Vector%601.htm) that represent the starting Vertex (A) and the terminating Vertex (B).  

__Note:__ We've also included some extension methods to easily convert a given `IEnumerable<T>` into a `Vector<T>` that simplifies this.

Here's an example:

```csharp
// Need to instantiate the two vertices (0,0,0) and (5,5,5)
var A = new double[] { 0.0, 0.0, 0.0 }.ToVector();
var B = new double[] { 5.0, 5.0, 5.0 }.ToVector();
// Generate the LineSegment A->B from (0,0,0) to (5,5,5)
var line = new LineSegment(A, B);

// Get the 3D and 2D length of the LineSegment
double len = line.Length;
double len2D = line.Length2D;
Assert.NotEqual(len, len2D); // True
```
Another key example is when you need to find the nearest point (from a collection of points) to a given `LineSegment`:
```csharp
// A collection of position vectors
var points = new List<Vector<double>>() { .. };
// Since this is ascending by default, can take the 'first' element as nearest
var nearest = points.OrderyBy(p => line.DistanceToPoint(p)).First();
```
Or the inverse - given a collection of `LineSegments` find the one nearest a given point of interest:
```csharp
// A single query POI
var point = new double[] { .. }.ToVector();
// A collection of LineSegments
var segs = new List<LineSegment>() { .. };
// Get the LineSegment closest to the input POI
var nearestSeg = segs.OrderBy(s => s.DistanceToPoint(point)).First();
var nearestPoint = nearestSeg.GetClosestPoint(point);
```
# Triangles
This namespace exposes the `Triangle` class which is used to represent 2D/3D triangular geometries as defined by a set of three vector vertices.  Once created a `Triangle` exposes the following key functionality & fields:
- A collection of all edges as `LineSegments`
- A `BoundingBox` envelope of the `Triangle` geometry
- The geometry's [Normal Vector](https://mathworld.wolfram.com/NormalVector.html#:~:text=The%20normal%20vector%2C%20often%20simply,pointing%20normal%20are%20usually%20distinguished.)
- Methods to check if a given (X,Y) position is contained by the 2D projection of the `Triangle` geometry
- Methods to extract the elevation (Z) on the `Triangle` surface for a given (X,Y) position
## Usage
As mentioned above - in order to generate a `Triangle` we'll need to have a collection of vertices that define the `Triangle` geometry.  Here's an example:
```csharp
// Forming Triangle (0, 0, 0) -> (1, 0, 1) -> (0, 1, 0)
var A = new double[] { 0.0, 0.0, 0.0 }.ToVector();
var B = new double[] { 1.0, 0.0, 1.0 }.ToVector();
var C = new double[] { 0.0, 1.0, 0.0 }.ToVector();
// Generate the Triangle object
var Triangle = new Triangle(new() { A, B, C });
```
Now with the `Triangle` defined, we can check for containment of any given position and then sample its elevation on the `Triangle` surface as follows:
```csharp
// Input POI's elevation doesn't matter for containment or Z-sampling
var pos = new double[] {0.25, 0.25, double.NaN}.ToVector();
// Checking containment & extract elevation (Z)
if(Triangle.Contains(pos))
{
    double Z = Triangle.GetZ(pos); // 0.25
}
```
# Index.KdTree
This namespace exposes Themis' implementation of a [K-D Tree](https://en.wikipedia.org/wiki/K-d_tree) (a binary space partitioning tree) that can be used for fast and efficient radial & nearest neighbour searches of point geometries.  The key advantage of a K-D Tree is that it has an average complexity of O(logn) for Insert, Search, and Delete which is ideal for spatial datasets of significant scale and variable dimensionality.

A key note is that we've implemented this structure as `KdTree<TKey, TValue>` so that consumers can store any desired `TValue` type as long as they can compose an associated point geometry of form `IEnumerable<TKey>` to store within the tree.

Further to that point - each `KdTree` must be given a `ITypeMath<TKey>` that defines how we should compare the point geometries stored within the `KdTree`.  This is done so that we can support storing point geometries in both Spherical and Euclidean coordinate systems.  As of now, we current support the following `ITypeMath<TKey>` implementations:
- DoubleMath (`TypeMath<double>` - Euclidean)
- FloatMath (`TypeMath<float>` - Euclidean)
- GeographicMath (`TypeMath<float>` - Spherical)
## Construction
When constructing a new `KdTree` we always have to specify both the dimensionality but also the intended `ITypeMath` to be applied as follows:
```csharp
int dimensions = 2;
FLoatMath math = new();

var tree = new KdTree<float, string>(dimensions, typeMath); 
```
Now, the `KdTree` also allows for a few different ways of handling the case when the client attempts to insert a duplicate point geometry into the `KdTree`.  Currently we support the following behaviours:
- `AddDuplicateBehaviour.Skip`
    - Don't add or change anything, simply skip the 'new' point
- `AddDuplicateBehaviour.Error`
    - Throw a `DuplicateNodeError` when attempting to add a duplicate point
- `AddDuplicateBehaviour.Update`
    - Replace the existing `TValue` with the new `TValue`

Here's how to specify the intended behaviour for a given `KdTree`:
```csharp
var tree = new KdTree<float, string>(dimensions, typeMath, AddDuplicateBehavior.Skip);
```
With that construction effort out of the way - you can now actually fill your tree out as follows:
```csharp
internal record struct PointRecord(float[] Position, string Name);
...

var points = new PointRecord[] { .. };
// Adding points into the tree
foreach (var point in points) { tree.Add(point.Position, point.Name); }
// Removing an existing point from the tree
tree.Remove(point.Position);
// Clearing all currently stored nodes
tree.Clear();
```
## Search
So, assuming you've already got a `KdTree` built and want to actually find something that's stored within it - you've got a few options:
- Search for a specific `IEnumerable<TKey>` position and return the `TValue` stored there
- Search for a specific `TValue` and return its `IEnumerable<TKey>` position
- Search for N (configurable number) of nearest-neighbours
- Search for points around a given `IEnumerable<TKey>` position within a specified radial distance

Here's a few examples:
```csharp
// Attempt to find a value at a given position
if (tree.FindValueAt(point.Position, out value))
{
    Console.WriteLine($"Found value: {value}");
}
// Attempt to find a position for a given value
if (tree.FindValue(point.Value, out position))
{
    Console.WriteLine($"Found position: {position.ToArray()}");
}
// Perform a radial search for all points w/in 5.0 units of the given position
foreach (var point in tree.RadialSearch(point.Position, 5.0)) { .. }
// Perform a search for the three nearest neighbours to a given position
foreach (var neigh in tree.GetNearestNeighbours(point.Position, 3)) { .. }
```
## Balancing
Now, our `KdTree` is not self-balancing during construction and as such can fall victim to becoming un-balanced based on the input data.  It does, however, have a simple method that can be called to re-construct the tree in a balanced state as follows:
```csharp
// Construct the tree
var tree = new KdTree<float, string>(dimensions, typeMath); 
// Adding points into the tree
var points = new PointRecord[] { .. };
foreach (var point in points) { tree.Add(point.Position, point.Name); }
// Balance the tree
tree.Balance();
```
The call to `tree.Balance()` will edit the `KdTree` in-place and will ensure that your searches are as optimized as possible given the input dataset.
# Index.QuadTree
This namespace exposes Themis' implemetation of a [QuadTree](https://en.wikipedia.org/wiki/Quadtree) (a 2D binary space partitioning tree) that can be used for fast and efficient storage and searching of geometries that can be represented by a 2D `IBoundingBox`.  QuadTrees are particularly useful for image processing, mesh generation, and surface collision detection.

This implementation is actually technically a 'Q-Tree' as we're able to specify the maximum capacity of each `QuadTreeNode` prior to splitting into child nodes - but by default the maximum capacity is 8.

A key note is that our `QuadTree<T>` implementation allows for storage of any object as long as they can be given an `IBoundingBox` envelope when inserted into the tree.

## Construction
Unlike the `KdTree` we don't have to specify anything beyond what type is being stored within the tree and (optionally) how many items are allowed per `QuadTreeNode<T>` prior to split.

Here's an example:
```csharp
int maxItems = 16;
// Both trees are empty and store Triangles - but TreeB has a maximum node capacity of 16 
var TreeA = new QuadTree<Triangle>();
var TreeB = new QuadTree<Triangle>(maxItems);
```
It's also quite easy to add point geometries into `QuadTree<T>`'s by generating an `IBoundingBox` as follows:
```csharp
var tree = new QuadTree<Vector<double>>();
// Point geometries are supported - just need to compose an IBoundingBox envelope
var point = new double[] { .. }.ToVector();
var env = BoundingBox.FromPoint(point[0], point[1], BoundingBox.SinglePointBuffer);
// Add to tree
tree.Add(point, env);
```
Now, let's say you've got a collection if `Triangle` geometries that represent a 3D ground surface and you want to perform efficient collision detection.  Here's how you'd build the tree:
```csharp
var tree = new QuadTree<Triangle>();
// Add each Triangle using the existing IBoundingBox envelope
foreach (var triangle in Triangles) { tree.Add(triangle, triangle.Envelope); }
```
## Search / Query
The `QuadTree<T>` has more rudimentary 'search' functionality when compared to the `KdTree<TKey, TValue>` but is still quite powerful.  Consumers can query a given `QuadTree<T>` as follows:
- Get all contained elements that intersect with a given 2D (X, Y) position
- Get all contained elements that intersect with a given `IBoundingBox`

__Note:__ Any given element could be stored in a root `QuadTreeNode<T>` as well as within one or more children `QuadTreeNode<T>` of that root node.  As such, calls to `QueryDistinct()` are guaranteed to return each element only once, whereas `QueryNonDistinct()` can contain duplicates.

Building on our above example with the `QuadTree<Triangle>`, here's how we'd query the tree to find the Triangle that covers a given position:
```csharp
var tree = new QuadTree<Triangle>();
// Add each Triangle using the existing IBoundingBox envelope
foreach (var triangle in Triangles) { tree.Add(triangle, triangle.Envelope); }
// The point-of-interest (POI) to check
var poi = new double[] { .. }.ToVector();
// Get all candidate Triangles, filter to explicitly covering, then select the first (or default)
var covering = tree.QueryDistinct(poi[0], poi[1])
                   .Where(t => t.Contains(poi))
                   .FirstOrDefault();
```
As you can see - there are two steps to consider when querying the `QuadTree<T>`:
-  Getting all potential candidates
-  Filtering down to specific target

Now, let's say we instead wanted to get all Triangles within a specified 2D search distance around a given POI:
```csharp
// The 2D search distance
double searchDistance = 50.0;
// Assuming tree is built as above - compose the POI and associated AOI
var poi = new double[] { .. }.ToVector();
var queryAoi = BoundingBox.FromPoint(poi[0], poi[1], searchDistance);
// Get all elements that overlap with the given AOI
var candidates = tree.QueryDistinct(queryAoi);
```