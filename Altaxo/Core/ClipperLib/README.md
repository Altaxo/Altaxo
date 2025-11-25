# Comparison Clipper1 (namespace ClipperLib) vs Clipper2 (namespace Clipper2Lib)  

Clipper1 | Clipper2  
---- | ----
IntPoint | Point64  
Path = `List<IntPoint>` | Path64  
Paths = `List<List<IntPoint>>` | Paths64  
PolyNode | PolyNode64  
PolyTree | PolyTree64  
PolyNode.Contour | Path64.Polygon
Clipper | Clipper64