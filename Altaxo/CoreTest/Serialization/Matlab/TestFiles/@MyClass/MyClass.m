function obj = MyClass(x, y)
% MyClass.m
%
% Old-style (legacy) Octave class used only for generating a MAT v5 object
% (mxOBJECT_CLASS) test value.

if nargin < 1
  x = 1;
end
if nargin < 2
  y = [1; 2; 3];
end

s = struct("x", x, "y", y);
obj = class(s, "MyClass");
