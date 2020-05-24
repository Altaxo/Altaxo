# Common

- The class Glyph have to be mirrored here. The reason is that this class is used as resource key in resources located in Generic.xaml,
and the Xaml code does not accept classes from external Dlls that are internal but visible via InternalsVisibleTo attribute.
