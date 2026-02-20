% generate_all_supported_types.m
%
% Generates a MAT-file (MAT v5 / "-mat7-binary") that exercises the value types
% supported by Altaxo's MAT v5 reader.
%
% Usage (from this directory):
%   octave --no-gui --quiet generate_all_supported_types.m
%
% Output:
%   all_supported_types.mat
%
% Notes:
% - Use "-mat7-binary" (MAT v5/v7 binary). Do NOT use "-hdf5" (MAT v7.3).
% - The object value uses Octave's legacy class objects (struct + class name).
%   This is written as an mxOBJECT_CLASS in MAT v5.
%

% Simple scalar/string
s = "Hello";
a = 42.5;

% Vector and matrix (double)
v = [1; 2; 3];
m = reshape(1:6, 2, 3);  % 2x3

% Logical scalar and logical array
b = true;
lb = logical([1 0; 0 1]);

% N-dimensional numeric array (double)
na = reshape(1:24, [2 3 4]);

% Cell array mixing types
c = { "cellstr", 7.5, [1; 2], [1 2; 3 4] };

% Struct array (1x1) with multiple field types
st = struct();
st.fstr    = "abc";
st.fscalar = 3.14;
st.fvec    = [10; 20; 30];
st.fmat    = [11 12; 13 14];
st.flog    = false;
st.flogarr = logical([1 0; 0 1]);
st.fnd     = reshape(1:24, [2 3 4]);
st.fcell   = { "x", 1 };

% Legacy object (old-style class) with fields
% Note: Octave only allows calling `class(...)` from inside a class constructor or method.
% The constructor is provided in @MyClass/MyClass.m.
obj = MyClass(1, [1; 2; 3]);

save("-mat7-binary", "all_supported_types.mat", ...
  "s", "a", "v", "m", "b", "lb", "na", "c", "st", "obj");

% Quick check
% whos("-file", "all_supported_types.mat")
