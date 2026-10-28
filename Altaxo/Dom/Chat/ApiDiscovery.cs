#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Altaxo.Chat
{
public sealed partial class AltaxoScriptingToolProvider
  {
    /// <summary>
    /// Reflection- and XML-documentation-based discovery of an API.
    /// Finds types by name or summary text, and describes a type with its members and their documentation.
    /// Output is compact plain text meant to be read by an LLM.
    /// </summary>
    public static class ApiDiscovery
    {
      private const BindingFlags AllMembers = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

      private static readonly ConcurrentDictionary<Assembly, Dictionary<string, XElement>> _docCache = new();
      private static readonly HashSet<string> _loadedPrefixes = new(StringComparer.OrdinalIgnoreCase);
      private static readonly object _loadLock = new();

      private static readonly Dictionary<Type, string> Keywords = new()
      {
        [typeof(void)] = "void",
        [typeof(bool)] = "bool",
        [typeof(byte)] = "byte",
        [typeof(sbyte)] = "sbyte",
        [typeof(short)] = "short",
        [typeof(ushort)] = "ushort",
        [typeof(int)] = "int",
        [typeof(uint)] = "uint",
        [typeof(long)] = "long",
        [typeof(ulong)] = "ulong",
        [typeof(float)] = "float",
        [typeof(double)] = "double",
        [typeof(decimal)] = "decimal",
        [typeof(char)] = "char",
        [typeof(string)] = "string",
        [typeof(object)] = "object",
      };

      #region Public API

      /// <summary>
      /// Finds public types whose full name or documentation summary contains all the given search terms.
      /// </summary>
      /// <param name="query">One or more terms separated by spaces; all must match (case-insensitive).</param>
      /// <param name="maxResults">Maximum number of types returned.</param>
      /// <param name="assemblyPrefix">Only assemblies whose name starts with this prefix are searched. Empty = all loaded assemblies.</param>
      public static string FindTypes(string query, int maxResults = 50, string? assemblyPrefix = "Altaxo")
      {
        if (string.IsNullOrWhiteSpace(query))
          throw new DiscoveryException("The query is empty.");
        if (maxResults < 1)
          maxResults = 1;

        var prefix = assemblyPrefix ?? string.Empty;
        var terms = query.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        var hits = new List<(int Score, Type Type, string Summary)>();

        foreach (var asm in GetAssemblies(prefix))
        {
          foreach (var t in SafeTypes(asm))
          {
            if (!(t.IsPublic || t.IsNestedPublic) || t.Name.Contains('<') || t.IsDefined(typeof(CompilerGeneratedAttribute), false))
              continue;

            var fullName = t.FullName ?? t.Name;
            string? summary = null;
            int nameHits = 0;
            bool all = true;

            foreach (var term in terms)
            {
              if (fullName.Contains(term, StringComparison.OrdinalIgnoreCase))
              {
                nameHits++;
                continue;
              }
              summary ??= Summary(t);
              if (!summary.Contains(term, StringComparison.OrdinalIgnoreCase))
              {
                all = false;
                break;
              }
            }

            if (!all)
              continue;

            var simple = StripArity(t.Name);
            int score = nameHits * 10;
            foreach (var term in terms)
            {
              if (string.Equals(simple, term, StringComparison.OrdinalIgnoreCase))
                score += 30;
              else if (simple.StartsWith(term, StringComparison.OrdinalIgnoreCase))
                score += 5;
            }
            hits.Add((score, t, summary ?? Summary(t)));
          }
        }

        if (hits.Count == 0)
          return $"No public types found for '{query}' in assemblies with prefix '{prefix}'. Try fewer or different terms" +
                 (prefix.Length > 0 ? ", or pass an empty assemblyPrefix to search all loaded assemblies." : ".");

        var ordered = hits
          .OrderByDescending(h => h.Score)
          .ThenBy(h => h.Type.FullName, StringComparer.Ordinal)
          .ToList();

        var sb = new StringBuilder();
        sb.AppendLine($"Found {ordered.Count} type(s) for '{query}'" + (ordered.Count > maxResults ? $", showing the best {maxResults}:" : ":"));
        foreach (var h in ordered.Take(maxResults))
        {
          sb.Append(Kind(h.Type)).Append(' ').Append(FmtFull(h.Type));
          sb.Append("  [").Append(h.Type.Assembly.GetName().Name).Append(']');
          if (h.Summary.Length > 0)
            sb.Append(" - ").Append(Truncate(h.Summary, 140));
          sb.AppendLine();
        }
        return sb.ToString();
      }

      /// <summary>
      /// Describes a type: kind, base types, summary and its members with signatures and documentation.
      /// </summary>
      /// <param name="typeName">Full name (preferred) or simple name of the type.</param>
      /// <param name="memberFilter">If not empty, only members whose name contains this text are listed, with full documentation.</param>
      /// <param name="includeInherited">Include members inherited from base types.</param>
      /// <param name="includeNonPublic">Include private and internal members.</param>
      /// <param name="maxMembers">Maximum number of members listed.</param>
      public static string DescribeType(string typeName, string? memberFilter = null, bool includeInherited = false, bool includeNonPublic = false, int maxMembers = 150)
      {
        var type = ResolveType(typeName);
        var filter = (memberFilter ?? string.Empty).Trim();
        bool detail = filter.Length > 0;
        if (maxMembers < 1)
          maxMembers = 1;

        var sb = new StringBuilder();
        AppendTypeHeader(sb, type, detail);

        if (type.IsEnum)
        {
          sb.AppendLine().AppendLine("Values:");
          foreach (var f in type.GetFields(BindingFlags.Public | BindingFlags.Static))
          {
            if (detail && !f.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
              continue;
            var value = Convert.ToString(f.GetRawConstantValue(), CultureInfo.InvariantCulture);
            var s = Summary(f);
            sb.Append("  ").Append(f.Name).Append(" = ").Append(value);
            if (s.Length > 0)
              sb.Append(" - ").Append(Truncate(s, 160));
            sb.AppendLine();
          }
          return sb.ToString();
        }

        if (typeof(Delegate).IsAssignableFrom(type))
          return sb.ToString(); // the Invoke signature is already in the header

        var flags = AllMembers | (includeInherited ? BindingFlags.FlattenHierarchy : BindingFlags.DeclaredOnly);

        bool Wanted(MemberInfo m)
        {
          if (IsCompilerGenerated(m) || !IsVisible(m, includeNonPublic))
            return false;
          if (includeInherited && !detail && m.DeclaringType == typeof(object))
            return false;
          return !detail || m.Name.Contains(filter, StringComparison.OrdinalIgnoreCase);
        }

        var sections = new List<(string Title, List<MemberInfo> Items)>();

        if (!type.IsInterface)
        {
          bool ctorsWanted = !detail
            || type.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)
            || filter.Equals("ctor", StringComparison.OrdinalIgnoreCase)
            || filter.Equals("new", StringComparison.OrdinalIgnoreCase);
          var ctors = ctorsWanted
            ? type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(c => !IsCompilerGenerated(c) && IsVisible(c, includeNonPublic))
                .OrderBy(c => c.GetParameters().Length).Cast<MemberInfo>().ToList()
            : new List<MemberInfo>();
          sections.Add(("Constructors", ctors));
        }

        sections.Add(("Properties", type.GetProperties(flags).Where(Wanted)
          .OrderBy(p => p.Name, StringComparer.Ordinal).ThenBy(p => p.GetIndexParameters().Length).Cast<MemberInfo>().ToList()));
        sections.Add(("Fields", type.GetFields(flags).Where(Wanted)
          .OrderBy(f => f.Name, StringComparer.Ordinal).Cast<MemberInfo>().ToList()));
        sections.Add(("Events", type.GetEvents(flags).Where(Wanted)
          .OrderBy(e => e.Name, StringComparer.Ordinal).Cast<MemberInfo>().ToList()));
        sections.Add(("Methods", type.GetMethods(flags)
          .Where(m => (!m.IsSpecialName || m.Name.StartsWith("op_", StringComparison.Ordinal)) && Wanted(m))
          .OrderBy(m => m.Name, StringComparer.Ordinal).ThenBy(m => m.GetParameters().Length).Cast<MemberInfo>().ToList()));
        sections.Add(("Nested types", type.GetNestedTypes(flags).Where(Wanted)
          .OrderBy(n => n.Name, StringComparer.Ordinal).Cast<MemberInfo>().ToList()));

        int shown = 0, omitted = 0;
        foreach (var (title, items) in sections)
        {
          if (items.Count == 0)
            continue;
          sb.AppendLine().AppendLine(title + ":");
          foreach (var mi in items)
          {
            if (shown >= maxMembers)
            {
              omitted++;
              continue;
            }
            shown++;
            AppendMember(sb, mi, type, detail);
          }
        }

        if (shown == 0)
          sb.AppendLine().AppendLine(detail ? $"No members matching '{filter}'." : "No visible members.");
        if (omitted > 0)
          sb.AppendLine().AppendLine($"... {omitted} more member(s) omitted. Use memberFilter to narrow the list.");

        return sb.ToString();
      }

      #endregion

      #region Type lookup

      /// <summary>Resolves a type by full name or, failing that, by simple name.</summary>
      public static Type ResolveType(string typeName)
      {
        if (string.IsNullOrWhiteSpace(typeName))
          throw new DiscoveryException("The type name is empty.");

        typeName = typeName.Trim();
        EnsureLoaded("Altaxo");
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).ToList();

        // 1. exact full name (nested types with '+')
        foreach (var asm in assemblies)
        {
          try
          {
            var t = asm.GetType(typeName, false, false);
            if (t is not null && (t.IsPublic || t.IsNestedPublic))
              return t;
          }
          catch { /* ignore broken assemblies */ }
        }

        // 2. simple or partial name, ignoring generic arity and case
        var wanted = Regex.Replace(typeName, "<.*>$", string.Empty).Trim();
        wanted = StripArity(wanted);

        bool Matches(Type t)
        {
          if (!(t.IsPublic || t.IsNestedPublic))
            return false;
          var full = StripArity(t.FullName ?? t.Name).Replace('+', '.');
          return string.Equals(StripArity(t.Name), wanted, StringComparison.OrdinalIgnoreCase)
              || full.EndsWith("." + wanted, StringComparison.OrdinalIgnoreCase)
              || string.Equals(full, wanted, StringComparison.OrdinalIgnoreCase);
        }

        var candidates = assemblies
          .Where(a => (a.GetName().Name ?? string.Empty).StartsWith("Altaxo", StringComparison.OrdinalIgnoreCase))
          .SelectMany(SafeTypes).Where(Matches).ToList();
        if (candidates.Count == 0)
          candidates = assemblies.SelectMany(SafeTypes).Where(Matches).ToList();

        if (candidates.Count == 1)
          return candidates[0];
        if (candidates.Count == 0)
          throw new DiscoveryException($"Type '{typeName}' not found. Use FindTypes to search by name or description.");

        var list = string.Join(", ", candidates.Take(15).Select(c => c.FullName));
        throw new DiscoveryException($"Type name '{typeName}' is ambiguous ({candidates.Count} matches). Use the full name. Candidates: {list}" + (candidates.Count > 15 ? ", ..." : string.Empty));
      }

      /// <summary>
      /// Loads all managed assemblies from the application directory whose file name starts with the prefix,
      /// so that types in not-yet-loaded assemblies can be found.
      /// </summary>
      private static void EnsureLoaded(string prefix)
      {
        if (prefix.Length == 0)
          return;

        lock (_loadLock)
        {
          if (!_loadedPrefixes.Add(prefix))
            return;

          var loaded = new HashSet<string>(
            AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetName().Name ?? string.Empty),
            StringComparer.OrdinalIgnoreCase);

          string[] files;
          try { files = Directory.GetFiles(AppContext.BaseDirectory, prefix + "*.dll"); }
          catch { return; }

          foreach (var file in files)
          {
            try
            {
              var name = AssemblyName.GetAssemblyName(file).Name ?? string.Empty;
              if (!loaded.Contains(name))
                Assembly.LoadFrom(file);
            }
            catch { /* native or otherwise unloadable dll: skip */ }
          }
        }
      }

      private static IEnumerable<Assembly> GetAssemblies(string prefix)
      {
        EnsureLoaded(prefix);
        return AppDomain.CurrentDomain.GetAssemblies().Where(a =>
          !a.IsDynamic &&
          (prefix.Length == 0 || (a.GetName().Name ?? string.Empty).StartsWith(prefix, StringComparison.OrdinalIgnoreCase)));
      }

      private static IEnumerable<Type> SafeTypes(Assembly asm)
      {
        try { return asm.GetTypes(); }
        catch (ReflectionTypeLoadException e) { return e.Types.OfType<Type>(); }
        catch { return Array.Empty<Type>(); }
      }

      #endregion

      #region XML documentation

      private static Dictionary<string, XElement> GetDocs(Assembly asm) => _docCache.GetOrAdd(asm, LoadDocs);

      private static Dictionary<string, XElement> LoadDocs(Assembly asm)
      {
        var result = new Dictionary<string, XElement>(StringComparer.Ordinal);
        string? path = null;

        try
        {
          if (!string.IsNullOrEmpty(asm.Location))
            path = Path.ChangeExtension(asm.Location, ".xml");
        }
        catch { /* ignore */ }

        if (path is null || !File.Exists(path))
        {
          var alt = Path.Combine(AppContext.BaseDirectory, (asm.GetName().Name ?? string.Empty) + ".xml");
          if (!File.Exists(alt))
            return result;
          path = alt;
        }

        try
        {
          var doc = XDocument.Load(path);
          foreach (var m in doc.Descendants("member"))
          {
            var name = (string?)m.Attribute("name");
            if (name is not null)
              result[name] = m;
          }
        }
        catch { /* corrupt file: no docs */ }

        return result;
      }

      private static XElement? Lookup(MemberInfo mi)
      {
        var id = DocId(mi);
        if (id is null)
          return null;
        var asm = (mi as Type)?.Assembly ?? mi.DeclaringType?.Assembly;
        if (asm is null)
          return null;
        return GetDocs(asm).TryGetValue(id, out var el) ? el : null;
      }

      /// <summary>Finds the documentation element of a member, following explicit and implicit inheritdoc.</summary>
      private static XElement? FindDoc(MemberInfo mi, int depth = 0)
      {
        mi = Normalize(mi);
        var el = Lookup(mi);
        bool inherit = el?.Element("inheritdoc") is not null || (el is null && mi is not Type && mi is not ConstructorInfo);
        if (!inherit || depth >= 6)
          return el;

        foreach (var b in BaseMembers(mi))
        {
          var be = FindDoc(b, depth + 1);
          if (be is not null && be.Element("inheritdoc") is null)
            return be;
        }
        return el;
      }

      private static string Summary(MemberInfo mi) => Render(FindDoc(mi)?.Element("summary"));

      private static MemberInfo Normalize(MemberInfo mi)
      {
        if (mi is Type tt)
          return tt.IsConstructedGenericType ? tt.GetGenericTypeDefinition() : tt;

        var dt = mi.DeclaringType;
        if (dt is not null && dt.IsConstructedGenericType)
        {
          var def = dt.GetGenericTypeDefinition();
          var m = def.GetMembers(AllMembers | BindingFlags.DeclaredOnly)
            .FirstOrDefault(x => x.MetadataToken == mi.MetadataToken && x.Module == mi.Module);
          if (m is not null)
            return m;
        }
        return mi;
      }

      private static IEnumerable<MemberInfo> BaseMembers(MemberInfo mi)
      {
        if (mi is Type type)
        {
          if (type.BaseType is not null && type.BaseType != typeof(object))
            yield return type.BaseType;
          foreach (var i in type.GetInterfaces())
            yield return i;
          yield break;
        }

        var dt = mi.DeclaringType;
        if (dt is null)
          yield break;

        var parents = new List<Type>();
        if (dt.BaseType is not null)
          parents.Add(dt.BaseType);
        parents.AddRange(dt.GetInterfaces());

        foreach (var p in parents)
        {
          MemberInfo[] candidates;
          try { candidates = p.GetMember(mi.Name, mi.MemberType, AllMembers); }
          catch { continue; }

          foreach (var c in candidates)
            if (SameSignature(mi, c))
              yield return c;
        }
      }

      private static bool SameSignature(MemberInfo a, MemberInfo b)
      {
        var pa = ParamTypes(a);
        var pb = ParamTypes(b);
        if (pa.Length != pb.Length)
          return false;
        for (int i = 0; i < pa.Length; i++)
          if (pa[i] != "*" && pb[i] != "*" && pa[i] != pb[i])
            return false;
        return true;
      }

      private static string[] ParamTypes(MemberInfo m) => m switch
      {
        MethodBase mb => mb.GetParameters().Select(p => Loose(p.ParameterType)).ToArray(),
        PropertyInfo p => p.GetIndexParameters().Select(x => Loose(x.ParameterType)).ToArray(),
        _ => Array.Empty<string>(),
      };

      private static string Loose(Type t)
      {
        if (t.IsByRef) return Loose(t.GetElementType()!) + "&";
        if (t.IsArray) return Loose(t.GetElementType()!) + "[]";
        if (t.IsGenericParameter) return "*";
        if (t.IsGenericType) return t.GetGenericTypeDefinition().FullName ?? t.Name;
        return t.FullName ?? t.Name;
      }

      // ---- documentation comment IDs (as written by the C# compiler) ----

      private static string? DocId(MemberInfo mi)
      {
        switch (mi)
        {
          case Type t:
            return "T:" + TypeFull(t);
          case ConstructorInfo c:
            return "M:" + TypeFull(c.DeclaringType!) + "." + (c.IsStatic ? "#cctor" : "#ctor") + ParamList(c.GetParameters());
          case MethodInfo m:
            {
              var id = "M:" + TypeFull(m.DeclaringType!) + "." + m.Name.Replace('.', '#')
                + (m.IsGenericMethodDefinition ? "``" + m.GetGenericArguments().Length : string.Empty)
                + ParamList(m.GetParameters());
              if (m.Name is "op_Implicit" or "op_Explicit")
                id += "~" + ParamTypeId(m.ReturnType);
              return id;
            }
          case PropertyInfo p:
            return "P:" + TypeFull(p.DeclaringType!) + "." + p.Name.Replace('.', '#') + ParamList(p.GetIndexParameters());
          case FieldInfo f:
            return "F:" + TypeFull(f.DeclaringType!) + "." + f.Name;
          case EventInfo e:
            return "E:" + TypeFull(e.DeclaringType!) + "." + e.Name.Replace('.', '#');
          default:
            return null;
        }
      }

      private static string TypeFull(Type t) => (t.FullName ?? t.Name).Replace('+', '.');

      private static string ParamList(ParameterInfo[] ps) =>
        ps.Length == 0 ? string.Empty : "(" + string.Join(",", ps.Select(p => ParamTypeId(p.ParameterType))) + ")";

      private static string ParamTypeId(Type t)
      {
        if (t.IsByRef) return ParamTypeId(t.GetElementType()!) + "@";
        if (t.IsPointer) return ParamTypeId(t.GetElementType()!) + "*";
        if (t.IsArray)
        {
          var e = ParamTypeId(t.GetElementType()!);
          int rank = t.GetArrayRank();
          return rank == 1 ? e + "[]" : e + "[" + string.Join(",", Enumerable.Repeat("0:", rank)) + "]";
        }
        if (t.IsGenericParameter)
          return (t.DeclaringMethod is not null ? "``" : "`") + t.GenericParameterPosition;
        if (t.IsConstructedGenericType)
        {
          var def = Regex.Replace(t.GetGenericTypeDefinition().FullName ?? t.Name, @"`\d+", string.Empty).Replace('+', '.');
          return def + "{" + string.Join(",", t.GetGenericArguments().Select(ParamTypeId)) + "}";
        }
        return (t.FullName ?? t.Name).Replace('+', '.');
      }

      // ---- rendering of documentation text ----

      private static string Render(XElement? el)
      {
        if (el is null)
          return string.Empty;
        var sb = new StringBuilder();
        foreach (var n in el.Nodes())
          AppendNode(sb, n);
        return Regex.Replace(sb.ToString(), @"\s+", " ").Trim();
      }

      private static void AppendNode(StringBuilder sb, XNode node)
      {
        switch (node)
        {
          case XText t:
            sb.Append(t.Value);
            break;
          case XElement e:
            switch (e.Name.LocalName)
            {
              case "see":
              case "seealso":
                var cref = (string?)e.Attribute("cref");
                if (cref is not null) sb.Append(ShortCref(cref));
                else sb.Append((string?)e.Attribute("langword") ?? (string?)e.Attribute("href") ?? e.Value);
                break;
              case "paramref":
              case "typeparamref":
                sb.Append((string?)e.Attribute("name"));
                break;
              case "c":
                sb.Append('`').Append(e.Value.Trim()).Append('`');
                break;
              case "para":
              case "item":
              case "br":
                sb.Append(' ');
                foreach (var c in e.Nodes()) AppendNode(sb, c);
                sb.Append(' ');
                break;
              case "inheritdoc":
                break;
              default:
                foreach (var c in e.Nodes()) AppendNode(sb, c);
                break;
            }
            break;
        }
      }

      private static string ShortCref(string cref)
      {
        var kind = cref.Length > 1 && cref[1] == ':' ? cref[0] : '\0';
        var s = kind != '\0' ? cref.Substring(2) : cref;
        int paren = s.IndexOf('(');
        if (paren >= 0)
          s = s.Substring(0, paren);
        s = s.Replace("{", "<").Replace("}", ">");
        var parts = s.Split('.');
        int keep = kind == 'T' || kind == '\0' ? 1 : 2;
        return string.Join(".", parts.Skip(Math.Max(0, parts.Length - keep)));
      }

      private static string Truncate(string s, int max) =>
        s.Length <= max ? s : s.Substring(0, max - 3).TrimEnd() + "...";

      #endregion

      #region Formatting

      private static void AppendTypeHeader(StringBuilder sb, Type type, bool detail)
      {
        var el = FindDoc(type);

        var line = new StringBuilder();
        if (type.IsDefined(typeof(FlagsAttribute), false))
          line.Append("[Flags] ");
        line.Append(Kind(type)).Append(' ').Append(FmtFull(type));

        var bases = new List<string>();
        var bt = type.BaseType;
        if (bt is not null && bt != typeof(object) && bt != typeof(ValueType) && bt != typeof(Enum) && bt != typeof(MulticastDelegate))
          bases.Add(FmtFull(bt));
        var inheritedInterfaces = bt?.GetInterfaces() ?? Array.Empty<Type>();
        bases.AddRange(type.GetInterfaces().Except(inheritedInterfaces).Select(FmtFull));
        if (bases.Count > 0)
          line.Append(" : ").Append(string.Join(", ", bases));
        sb.AppendLine(line.ToString());

        sb.AppendLine("Assembly: " + type.Assembly.GetName().Name);

        var summary = Render(el?.Element("summary"));
        if (summary.Length > 0)
          sb.AppendLine(summary);
        var remarks = Render(el?.Element("remarks"));
        if (remarks.Length > 0)
          sb.AppendLine("Remarks: " + Truncate(remarks, detail ? 1500 : 600));

        if (GetDocs(type.Assembly).Count == 0)
          sb.AppendLine("(No XML documentation file was found for this assembly; only signatures are available.)");

        if (typeof(Delegate).IsAssignableFrom(type) && type.GetMethod("Invoke") is { } invoke)
          sb.AppendLine("Signature: " + Signature(invoke).Replace("virtual ", string.Empty));
      }

      private static void AppendMember(StringBuilder sb, MemberInfo mi, Type described, bool detail)
      {
        var sig = Signature(mi);
        var from = mi.DeclaringType is not null && mi.DeclaringType != described ? $"  (from {Fmt(mi.DeclaringType)})" : string.Empty;

        if (!detail)
        {
          sb.Append("  ").Append(sig).Append(from);
          var s = Summary(mi);
          if (s.Length > 0)
            sb.Append(" - ").Append(Truncate(s, 160));
          sb.AppendLine();
          return;
        }

        sb.Append("  ").AppendLine(sig + from);
        var el = FindDoc(mi);
        if (el is null)
        {
          sb.AppendLine("      (no documentation)");
          return;
        }

        var summary = Render(el.Element("summary"));
        if (summary.Length > 0)
          sb.AppendLine("      " + summary);
        foreach (var p in el.Elements("param"))
          sb.AppendLine($"      param {(string?)p.Attribute("name")}: {Render(p)}");
        foreach (var p in el.Elements("typeparam"))
          sb.AppendLine($"      typeparam {(string?)p.Attribute("name")}: {Render(p)}");
        var returns = Render(el.Element("returns"));
        if (returns.Length > 0)
          sb.AppendLine("      returns: " + returns);
        var value = Render(el.Element("value"));
        if (value.Length > 0)
          sb.AppendLine("      value: " + value);
        var remarks = Render(el.Element("remarks"));
        if (remarks.Length > 0)
          sb.AppendLine("      remarks: " + Truncate(remarks, 800));
        var ex = el.Elements("exception").Select(x => ShortCref((string?)x.Attribute("cref") ?? "?")).Distinct().ToList();
        if (ex.Count > 0)
          sb.AppendLine("      throws: " + string.Join(", ", ex));
      }

      private static string Signature(MemberInfo mi)
      {
        switch (mi)
        {
          case ConstructorInfo c:
            return StripArity(c.DeclaringType!.Name) + "(" + Params(c.GetParameters(), false) + ")";

          case MethodInfo m:
            {
              var sb = new StringBuilder();
              bool isInterface = m.DeclaringType?.IsInterface == true;
              if (m.IsStatic) sb.Append("static ");
              else if (!isInterface && m.IsAbstract) sb.Append("abstract ");
              else if (!isInterface && m.IsVirtual && !m.IsFinal)
                sb.Append(m.GetBaseDefinition().DeclaringType != m.DeclaringType ? "override " : "virtual ");

              sb.Append(Fmt(m.ReturnType)).Append(' ').Append(m.Name);
              if (m.IsGenericMethodDefinition)
                sb.Append('<').Append(string.Join(", ", m.GetGenericArguments().Select(a => a.Name))).Append('>');
              bool ext = m.IsDefined(typeof(ExtensionAttribute), false);
              sb.Append('(').Append(Params(m.GetParameters(), ext)).Append(')');
              return sb.ToString();
            }

          case PropertyInfo p:
            {
              var getter = p.GetMethod;
              var setter = p.SetMethod;
              var any = getter ?? setter;
              var sb = new StringBuilder();
              if (any?.IsStatic == true) sb.Append("static ");
              sb.Append(Fmt(p.PropertyType)).Append(' ');
              var idx = p.GetIndexParameters();
              sb.Append(idx.Length > 0 ? "this[" + Params(idx, false) + "]" : p.Name);
              sb.Append(" { ");
              if (getter is not null) sb.Append(Accessor(getter, "get")).Append(' ');
              if (setter is not null) sb.Append(Accessor(setter, "set")).Append(' ');
              sb.Append('}');
              return sb.ToString();
            }

          case FieldInfo f:
            {
              var sb = new StringBuilder();
              if (f.IsLiteral)
              {
                sb.Append("const ").Append(Fmt(f.FieldType)).Append(' ').Append(f.Name);
                try { sb.Append(" = ").Append(DefaultText(f.GetRawConstantValue(), f.FieldType)); } catch { /* ignore */ }
                return sb.ToString();
              }
              if (f.IsStatic) sb.Append("static ");
              if (f.IsInitOnly) sb.Append("readonly ");
              sb.Append(Fmt(f.FieldType)).Append(' ').Append(f.Name);
              return sb.ToString();
            }

          case EventInfo e:
            return (e.AddMethod?.IsStatic == true ? "static " : string.Empty) + "event " + Fmt(e.EventHandlerType!) + " " + e.Name;

          case Type t:
            return Kind(t) + " " + Fmt(t);

          default:
            return mi.Name;
        }
      }

      private static string Accessor(MethodInfo m, string name)
      {
        if (m.IsPublic) return name + ";";
        if (m.IsFamily) return "protected " + name + ";";
        if (m.IsFamilyOrAssembly) return "protected internal " + name + ";";
        if (m.IsAssembly) return "internal " + name + ";";
        return "private " + name + ";";
      }

      private static string Params(ParameterInfo[] ps, bool firstIsThis)
      {
        var parts = new List<string>(ps.Length);
        for (int i = 0; i < ps.Length; i++)
        {
          var p = ps[i];
          var sb = new StringBuilder();
          if (firstIsThis && i == 0) sb.Append("this ");
          var t = p.ParameterType;
          if (t.IsByRef)
          {
            sb.Append(p.IsOut ? "out " : p.IsIn ? "in " : "ref ");
            t = t.GetElementType()!;
          }
          else if (p.IsDefined(typeof(ParamArrayAttribute), false))
          {
            sb.Append("params ");
          }
          sb.Append(Fmt(t)).Append(' ').Append(p.Name);
          try
          {
            if (p.HasDefaultValue)
              sb.Append(" = ").Append(DefaultText(p.RawDefaultValue, t));
          }
          catch { /* ignore */ }
          parts.Add(sb.ToString());
        }
        return string.Join(", ", parts);
      }

      private static string DefaultText(object? v, Type t)
      {
        if (v is null || v == DBNull.Value || v == Type.Missing)
          return t.IsValueType && Nullable.GetUnderlyingType(t) is null ? "default" : "null";
        if (v is string s) return "\"" + s + "\"";
        if (v is bool b) return b ? "true" : "false";
        if (v is char c) return "'" + c + "'";
        if (t.IsEnum) return Fmt(t) + "." + Enum.ToObject(t, v);
        return Convert.ToString(v, CultureInfo.InvariantCulture) ?? "default";
      }

      private static string Kind(Type t)
      {
        if (t.IsEnum) return "enum";
        if (t.IsInterface) return "interface";
        if (typeof(Delegate).IsAssignableFrom(t)) return "delegate";
        if (t.IsValueType) return "struct";
        if (t.IsAbstract && t.IsSealed) return "static class";
        if (t.IsAbstract) return "abstract class";
        if (t.IsSealed) return "sealed class";
        return "class";
      }

      /// <summary>Short C# type name (no namespaces).</summary>
      private static string Fmt(Type t)
      {
        if (Keywords.TryGetValue(t, out var k)) return k;
        if (t.IsByRef) return Fmt(t.GetElementType()!);
        if (t.IsPointer) return Fmt(t.GetElementType()!) + "*";
        if (t.IsArray) return Fmt(t.GetElementType()!) + "[" + new string(',', t.GetArrayRank() - 1) + "]";
        if (t.IsGenericParameter) return t.Name;

        var underlying = Nullable.GetUnderlyingType(t);
        if (underlying is not null) return Fmt(underlying) + "?";

        var name = t.Name;
        var prefix = t.IsNested && t.DeclaringType is not null ? Fmt(t.DeclaringType) + "." : string.Empty;
        int tick = name.IndexOf('`');
        if (tick >= 0)
        {
          int arity = int.Parse(name.Substring(tick + 1), CultureInfo.InvariantCulture);
          name = name.Substring(0, tick);
          var args = t.GetGenericArguments();
          name += "<" + string.Join(", ", args.Skip(Math.Max(0, args.Length - arity)).Select(Fmt)) + ">";
        }
        return prefix + name;
      }

      /// <summary>C# type name including the namespace.</summary>
      private static string FmtFull(Type t)
      {
        if (Keywords.ContainsKey(t) || t.IsGenericParameter || t.IsArray || t.IsByRef || t.IsPointer)
          return Fmt(t);
        if (Nullable.GetUnderlyingType(t) is not null)
          return Fmt(t);
        return (string.IsNullOrEmpty(t.Namespace) ? string.Empty : t.Namespace + ".") + Fmt(t);
      }

      private static string StripArity(string name) => Regex.Replace(name, @"`\d+(?=$|\+|\.)", string.Empty);

      private static bool IsCompilerGenerated(MemberInfo m) =>
        m.Name.Contains('<') || m.IsDefined(typeof(CompilerGeneratedAttribute), false);

      private static bool IsVisible(MemberInfo mi, bool includeNonPublic)
      {
        if (includeNonPublic)
          return true;

        static bool Vis(MethodBase? m) => m is not null && (m.IsPublic || m.IsFamily || m.IsFamilyOrAssembly);

        return mi switch
        {
          MethodBase mb => Vis(mb),
          FieldInfo f => f.IsPublic || f.IsFamily || f.IsFamilyOrAssembly,
          PropertyInfo p => Vis(p.GetMethod) || Vis(p.SetMethod),
          EventInfo e => Vis(e.AddMethod) || Vis(e.RemoveMethod),
          Type t => t.IsNestedPublic || t.IsNestedFamily || t.IsNestedFamORAssem,
          _ => false,
        };
      }

      #endregion


    }
  }
}
