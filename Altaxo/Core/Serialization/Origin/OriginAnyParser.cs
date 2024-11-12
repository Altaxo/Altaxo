/*
 * OriginParser.cpp
 *  Copyright            : (C) 2008 Alex Kargovsky (kargovsky@yumr.phys.msu.su)
 *  Copyright            : (C) 2017 Stefan Gerlach (stefan.gerlach@uni.kn)
 *
 * OriginAnyParser.cpp
 *
 * Copyright 2017 Miquel Garriga <gbmiquel@gmail.com>
 * Copyright 2024 Dirk Lellinger (translation to C#, bug fixing)
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * Parser for all versions. Based mainly on Origin750Parser.cpp
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Altaxo.Serialization.Origin
{
  /// <summary>
  /// Parser for Origin .OPJ files (.OPJU files can not be read).
  /// </summary>
  public class OriginAnyParser
  {
    public List<SpreadColumn> Datasets { get; set; } = [];
    public List<SpreadSheet> SpreadSheets { get; set; } = [];
    public List<Matrix> Matrixes { get; set; } = [];
    public List<Excel> Excels { get; set; } = [];
    public List<Function> Functions { get; set; } = [];
    public List<Graph> Graphs { get; set; } = [];
    public List<Note> Notes { get; set; } = [];
    public List<(string Name, double Value)> Parameters { get; } = [];
    public ProjectNode ProjectTree { get; set; } = new();
    public string ResultsLog { get; set; }
    public int WindowsCount { get; set; }

    private int _fileVersion;
    public int FileVersion => _fileVersion;

    private int _buildVersion;
    public int BuildVersion => _buildVersion;

    public double Version => FileVersion / 100.0;

    // Process data
    private Stream _file;
    private StreamWriter _logfile;
    private System.Text.Encoding _encoding;

    private long _d_file_size;
    private long _curpos;
    private int _objectIndex;
    private int _parseError;
    private int _ispread;
    private int _imatrix;
    private int _iexcel;
    private int _igraph;
    private int _ilayer;
    private int _iaxispar;
    private byte[] _buffer16;

    public int ParseError => _parseError;

    /// <summary>
    /// Initializes a new instance of the <see cref="OriginAnyParser"/> class.
    /// </summary>
    /// <param name="originFile">The origin file.</param>
    public OriginAnyParser(FileStream originFile)
      : this(originFile, null)
    {
    }

    public OriginAnyParser(Stream originFile, StreamWriter? logFile)
    {
      this._file = originFile;
      _logfile = logFile;
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      _encoding = Encoding.GetEncoding("Windows-1252");
      _d_file_size = 0;
      _curpos = 0;
      _objectIndex = 0;
      _parseError = 0;
      _ispread = -1;
      _imatrix = -1;
      _iexcel = -1;
      _igraph = -1;
      _ilayer = -1;
      _buffer16 = new byte[16];

      Parse();
    }

    private bool Parse()
    {
      _d_file_size = _file.Length;
      _file.Seek(0, SeekOrigin.Begin);

      LogPrint($"File size: {_d_file_size}\n");

      // get file and program version, check it is a valid file
      (_fileVersion, var newFileVersion, _buildVersion, var isOpjuFile, var error) = ReadFileVersion(_file);
      if (error is not null)
      {
        _parseError = 1;
        LogPrint(error);
        return false;
      }

      if (newFileVersion == 0)
      {
        LogPrint($"Found project version {_fileVersion / 100.0:F2}");
      }
      else if (_fileVersion < 941)
      {
        LogPrint($"Found project version {newFileVersion / 10.0:F1} ({_fileVersion / 100.0:F2})");
      }

      _curpos = _file.Position;
      LogPrint($"Now at {_curpos} [0x{_curpos:X}]\n");

      // get global header
      ReadGlobalHeader();
      if (_parseError > 1)
      {
        return false;
      }

      _curpos = _file.Position;
      LogPrint($"Now at {_curpos} [0x{_curpos:X}]\n");

      // get dataset list
      uint dataset_list_size = 0;
      _objectIndex = 0; // use it to count DataSets

      LogPrint("Reading Data sets ...\n");
      while (true)
      {
        if (!ReadDataSetElement())
        {
          break;
        }

        dataset_list_size++;
      }
      if (_parseError > 1)
      {
        return false;
      }

      LogPrint($" ... done. Data sets: {dataset_list_size}\n");
      _curpos = _file.Position;
      LogPrint($"Now at {_curpos} [0x{_curpos:X}], file size {_d_file_size}\n");

      for (int i = 0; i < SpreadSheets.Count; ++i)
      {
        if (SpreadSheets[(int)i].Sheets > 1)
        {
          LogPrint($"        CONVERT SPREADSHEET \"{SpreadSheets[(int)i].Name}\" to EXCEL\n");
          ConvertSpreadToExcel(i);
          --i;
        }
      }

      // get window list
      uint window_list_size = 0;
      _objectIndex = 0; // reset it to count Windows (except Notes)

      LogPrint("Reading Windows ...\n");
      while (true)
      {
        if (!ReadWindowElement())
        {
          break;
        }

        window_list_size++;
      }
      LogPrint($" ... done. Windows: {window_list_size}\n");
      _curpos = _file.Position;
      LogPrint($"Now at {_curpos} [0x{_curpos:X}], file size {_d_file_size}\n");

      // get parameter list
      uint parameter_list_size = 0;

      LogPrint("Reading Parameters ...\n");
      while (true)
      {
        if (!ReadParameterElement())
        {
          break;
        }

        parameter_list_size++;
      }
      LogPrint($" ... done. Parameters: {parameter_list_size}\n");
      _curpos = _file.Position;
      LogPrint($"Now at {_curpos} [0x{_curpos:X}], file size {_d_file_size}\n");

      // Note windows were added between version >4.141 and 4.210,
      // i.e., with Release 5.0
      if (_curpos < _d_file_size)
      {
        // get note windows list
        uint note_list_size = 0;

        LogPrint("Reading Note windows ...\n");
        _objectIndex = 0; // reset it to count Notes
        while (true)
        {
          if (!ReadNoteElement())
          {
            break;
          }

          note_list_size++;
        }
        LogPrint($" ... done. Note windows: {note_list_size}\n");
        _curpos = _file.Position;
        LogPrint($"Now at {_curpos} [0x{_curpos:X}], file size {_d_file_size}\n");
      }

      // Project Tree was added between version >4.210 and 4.2616,
      // i.e., with Release 6.0
      if (_curpos < _d_file_size && Version >= 6)
      {
        // get project tree
        ReadProjectTree();
        _curpos = _file.Position;
        LogPrint($"Now at {_curpos} [0x{_curpos:X}], file size {_d_file_size}\n");
      }

      // Attachments were added between version >4.2673_558 and 4.2764_623,
      // i.e., with Release 7.0
      if (_curpos < _d_file_size && Version >= 7)
      {
        ReadAttachmentList();
        _curpos = _file.Position;
        LogPrint($"Now at {_curpos} [0x{_curpos:X}], file size {_d_file_size}\n");
      }

      if (_curpos >= _d_file_size)
      {
        LogPrint("Now at end of file\n");
      }

      AssignObjectsToProjectTree(ProjectTree);

      return true;
    }

    public int ReadObjectSize()
    {
      _file.ReadExactly(_buffer16, 0, sizeof(Int32) + 1);
      if (_buffer16[sizeof(Int32)] != '\n')
      {
        _curpos = _file.Position;
        LogPrint($"Wrong delimiter {_buffer16[sizeof(Int32)]} at {_curpos} [0x{_curpos:X}]");
        _parseError = 3;

        throw new InvalidDataException($"Error reading object size at position {_file.Position - 1 - sizeof(Int32)}: the newline char (0x0A) is missing at position {_file.Position - 1}");
      }
      return BitConverter.ToInt32(_buffer16, 0);
    }

    public byte[] ReadObjectAsByteArray(int size)
    {
      // read a size-byte blob of data followed by '\n'
      if (size > 0)
      {
        // get a string large enough to hold the result, initialize it to all 0's
        byte[] blob = new byte[size];
        // read data into that string
        // cannot use '>>' operator because iendianfstream truncates it at first '\0'
        _file.ReadExactly(blob, 0, (int)size);
        // read the '\n'
        var c = _file.ReadByte();
        if (c < 0)
        {
          throw new EndOfStreamException();
        }
        if (c != '\n')
        {
          _curpos = _file.Position;
          LogPrint($"Wrong delimiter {c} at {_curpos} [0x{_curpos:X}]");
          _parseError = 4;
          return [];
        }
        return blob;
      }
      return [];
    }

    public string ReadObjectAsString(int size)
    {
      char c;
      // read a size-byte blob of data followed by '\n'
      if (size > 0)
      {
        // get a string large enough to hold the result, initialize it to all 0's
        byte[] blob = new byte[size];
        // read data into that string
        // cannot use '>>' operator because iendianfstream truncates it at first '\0'
        _file.ReadExactly(blob, 0, (int)size);
        // read the '\n'
        c = (char)_file.ReadByte();
        if (c < 0)
        {
          throw new EndOfStreamException();
        }
        if (c != '\n')
        {
          _curpos = _file.Position;
          LogPrint($"Wrong delimiter {c} at {_curpos} [0x{_curpos:X}]");
          _parseError = 4;
          return string.Empty;
        }
        return _encoding.GetString(blob);
      }
      return string.Empty;
    }

    /// <summary>
    /// Reads the file version from an .opj or .opju file.
    /// </summary>
    /// <param name="file">The file stream.</param>
    /// <returns>Tuple of file version, new file version (contains the year), buildversion, whether it is an opju file, and a error string.
    /// The function is considered successful if the returned error is null.</returns>
    public static (int FileVersion, int NewFileVersion, int BuildVersion, bool IsOpjuFile, string? Error) ReadFileVersion(Stream file)
    {
      int fileVersion;
      int newFileVersion = 0;
      int buildVersion;
      bool isOpjuFile;

      file.Seek(0, SeekOrigin.Begin);
      // get file and program version, check it is a valid file
      string sFileVersion = ReadLine(file, false);

      if (sFileVersion.Length <= 5)
      {
        return (0, 0, 0, false, "Unexpectedly short version string");
      }

      if (sFileVersion.Substring(0, 4) == "CPYA")
      {
        isOpjuFile = false;
      }
      else if (sFileVersion.Substring(0, 5) == "CPYUA")
      {
        isOpjuFile = true;
      }
      else
      {
        return (0, 0, 0, false, $"File is neither an Origin .OPJ nor .OPJU file; the version string is {sFileVersion}");
      }

      if (sFileVersion[sFileVersion.Length - 1] != '#')
      {
        return (0, 0, 0, false, $"Unexpected end of version string (should end with #); the version string is {sFileVersion}");
      }

      long majorVersion = int.Parse(sFileVersion.Substring(5, 1), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
      int idx = sFileVersion.IndexOf(' ', 7);
      buildVersion = int.Parse(sFileVersion.Substring(7, idx - 7), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);

      // Translate version

      if (majorVersion == 3)
      {
        if (buildVersion < 830)
        {
          fileVersion = 350;
        }
        else
        {
          fileVersion = 410;
        }
      }
      else if (buildVersion >= 110 && buildVersion <= 141) // 4.1
      {
        fileVersion = 410;
      }
      else if (buildVersion <= 210) // 5.0
      {
        fileVersion = 500;
      }
      else if (buildVersion < 2624) // 6.0
      {
        fileVersion = 600;
      }
      else if (buildVersion < 2628) // 6.0 SR1
      {
        fileVersion = 601;
      }
      else if (buildVersion < 2635) // 6.0 SR4
      {
        fileVersion = 604;
      }
      else if (buildVersion < 2656) // 6.1
      {
        fileVersion = 610;
      }
      else if (buildVersion < 2659) // 7.0 SR0 (2656-2658)
      {
        fileVersion = 700;
      }
      else if (buildVersion < 2664) // 7.0 SR1 (2659-2663)
      {
        fileVersion = 701;
      }
      else if (buildVersion < 2672) // 7.0 SR2 (2664-2671)
      {
        fileVersion = 702;
      }
      else if (buildVersion < 2673) // 7.0 SR3 (2672-2672)
      {
        fileVersion = 703;
      }
      else if (buildVersion < 2766) // 7.0 SR4 (2673-2765)
      {
        fileVersion = 704;
      }
      else if (buildVersion < 2878) // 7.5 (2766-2877)
      {
        fileVersion = 750;
      }
      else if (buildVersion < 2881) // 8.0 SR0 (2878-2880)
      {
        fileVersion = 800;
      }
      else if (buildVersion < 2892) // 8.0 SR1,SR2,SR3 (2878-2891)
      {
        fileVersion = 801;
      }
      else if (buildVersion < 2944) // 8.0 SR4, 8.1 SR1-SR4 (2891-2943)
      {
        fileVersion = 810;
      }
      else if (buildVersion < 2947) // 8.5 SR0, SR1 (2944-2946)
      {
        fileVersion = 850;
      }
      else if (buildVersion < 2962) // 8.5.1 SR0, SR1, SR2
      {
        fileVersion = 851;
      }
      else if (buildVersion < 2980) // 8.6 SR0, SR1, SR2, SR3
      {
        fileVersion = 860;
      }
      else if (buildVersion < 3025) // 9.0 SR0, SR1, SR2
      {
        fileVersion = 900;
      }
      else if (buildVersion < 3078) // 9.1 SR0, SR1, SR2, SR3
      {
        fileVersion = 910;
      }
      else if (buildVersion < 3117) // 2015 (9.2) SR0, SR1, SR2
      {
        fileVersion = 920;
        newFileVersion = 20150;
      }
      else if (buildVersion < 3169) // 2016 (9.3.0) SR0
      {
        fileVersion = 930;
        newFileVersion = 20160;
      }
      else if (buildVersion < 3172) // 2016.1 (9.3.1), 2016.2 (9.3.2) SR1, SR2
      {
        fileVersion = 931;
        newFileVersion = 20161;
      }
      else if (buildVersion < 3225) // 2017.0 (9.4.0.220) SR0 3224
      {
        fileVersion = 940;
        newFileVersion = 20170;
      }
      else if (buildVersion < 3228) // 2017.1 (9.4.1.354), 2017.2 (9.4.2.380) SR1, SR2 3227
      {
        fileVersion = 941;
        newFileVersion = 20171;
      }
      else if (buildVersion < 3269) // 2018.0 (9.5.0.193), 2018.1 (9.5.1.195) SR0, SR1 3268
      {
        fileVersion = 950;
        newFileVersion = 20180;
      }
      else if (buildVersion < 3296) // 2018b.0 (9.5.5.409) SR0, SR1 3295
      {
        fileVersion = 955;
        newFileVersion = 20185;
      }
      else if (buildVersion < 3331) // 2019.0 (9.6.0.172) SR0 3330
      {
        fileVersion = 960;
        newFileVersion = 20190;
      }
      else if (buildVersion < 3360) // 2019b.0 (9.6.5.169) SR0 3359
      {
        fileVersion = 965;
        newFileVersion = 20195;
      }
      else
      {
        // > 2019bSR0
        fileVersion = 966;
        newFileVersion = 20196;
      }

      return (fileVersion, newFileVersion, buildVersion, isOpjuFile, null);
    }

    public void ReadGlobalHeader()
    {
      // get global header size
      int gh_size = 0, gh_endmark = 0;
      gh_size = ReadObjectSize();
      _curpos = _file.Position;
      LogPrint($"Global header size: {gh_size} [0x{gh_size:X}], starts at {_curpos} [0x{_curpos:X}],");

      // get global header data
      var gh_data = ReadObjectAsByteArray(gh_size);

      _curpos = _file.Position;
      LogPrint($" ends at {_curpos} [0x{_curpos:X}]\n");

      // when gh_size > 0x1B, a double with fileVersion/100 can be read at gh_data[0x1B:0x23]
      if (gh_size > (0x1B + sizeof(double)))
      {
        var dFileVersion = BitConverter.ToDouble(gh_data, 0x1B);
        if (dFileVersion > 8.5)
        {
          _fileVersion = (int)Math.Truncate(dFileVersion * 100.0);
        }
        else
        {
          _fileVersion = 10 * (int)Math.Truncate(dFileVersion * 10.0);
        }
        LogPrint($"Project version as read from header: {FileVersion / 100.0:F2} ({dFileVersion:F6})\n");

      }

      // now read a zero size end mark
      gh_endmark = ReadObjectSize();
      if (gh_endmark != 0)
      {
        _curpos = _file.Position;
        LogPrint($"Wrong end of list mark {gh_endmark} at {_curpos} [0x{_curpos:X}]\n");
        _parseError = 5;
        return;
      }
    }

    public bool ReadDataSetElement()
    {
      /* get info and values of a DataSet (worksheet column, matrix sheet, ...)
       * return true if a DataSet is found, otherwise return false */
      int dse_header_size = 0, dse_data_size = 0, dse_mask_size = 0;
      long dsh_start = 0, dsd_start = 0, dsm_start = 0;

      // get dataset header size
      dse_header_size = ReadObjectSize();
      if (dse_header_size == 0)
      {
        return false;
      }

      _curpos = _file.Position;
      dsh_start = _curpos;
      LogPrint($"Column: header size {dse_header_size} [0x{dse_header_size:X}], starts at {_curpos} [0x{_curpos:X}], ");
      var dse_header = ReadObjectAsByteArray(dse_header_size);

      // get known info
      var name = _encoding.GetString(dse_header, 0x58, 25).TrimEnd('\0');

      // go to end of dataset header, get data size
      _file.Seek(dsh_start + dse_header_size + 1, SeekOrigin.Begin);
      dse_data_size = ReadObjectSize();
      dsd_start = _file.Position;
      var dse_data = ReadObjectAsByteArray(dse_data_size);
      _curpos = _file.Position;
      LogPrint($"data size {dse_data_size} [0x{dse_data_size:X}], from {dsd_start} [0x{dsd_start:X}] to {_curpos} [0x{_curpos:X}],");

      // get data values
      GetColumnInfoAndData(dse_header, dse_header_size, dse_data, dse_data_size);

      // go to end of data values, get mask size (often zero)
      _file.Seek(dsd_start + dse_data_size, SeekOrigin.Begin); // dse_data_size can be zero
      if (dse_data_size > 0)
      {
        _file.Seek(1, SeekOrigin.Current);
      }

      dse_mask_size = ReadObjectSize();
      dsm_start = _file.Position;
      if (dse_mask_size > 0)
      {
        LogPrint($"\nmask size {dse_mask_size} [0x{dse_mask_size:X}], starts at {dsm_start} [0x{dsm_start:X}]");
      }

      string dse_mask = ReadObjectAsString(dse_mask_size);

      // get mask values
      if (dse_mask_size > 0)
      {
        _curpos = _file.Position;
        LogPrint($", ends at {_curpos} [0x{_curpos:X}]\n");
        // TODO: extract mask values from dse_mask
        // go to end of dataset mask
        _file.Seek(dsm_start + dse_mask_size + 1, SeekOrigin.Begin);
      }
      _curpos = _file.Position;
      LogPrint($" ends at {_curpos} [0x{_curpos:X}]: ");
      LogPrint($"{name}\n");

      return true;
    }

    public bool ReadWindowElement()
    {
      /* get general info and details of a window
       * return true if a Window is found, otherwise return false */
      int wdeHeaderSize = 0;
      long wdhStart = 0;

      // get window header size
      wdeHeaderSize = ReadObjectSize();
      if (wdeHeaderSize == 0)
      {
        return false;
      }

      _curpos = _file.Position;
      wdhStart = _curpos;
      LogPrint($"Window found: header size {wdeHeaderSize} [0x{wdeHeaderSize:X}], starts at {_curpos} [0x{_curpos:X}]: ");
      var wdeHeader = ReadObjectAsByteArray(wdeHeaderSize);

      // get known info
      var name = _encoding.GetString(wdeHeader, 2, 25).TrimEnd('\0');
      LogPrint($"{name}\n");

      // classify type of window
      _ispread = FindSpreadByName(name);
      _imatrix = FindMatrixByName(name);
      _iexcel = FindExcelByName(name);
      _igraph = -1;

      if (_ispread != -1)
      {
        LogPrint("\n  Window is a Worksheet book\n");
        GetWindowProperties(SpreadSheets[_ispread], wdeHeader, wdeHeaderSize);
      }
      else if (_imatrix != -1)
      {
        LogPrint("\n  Window is a Matrix book\n");
        GetWindowProperties(Matrixes[_imatrix], wdeHeader, wdeHeaderSize);
      }
      else if (_iexcel != -1)
      {
        LogPrint("\n  Window is an Excel book\n");
        GetWindowProperties(Excels[_iexcel], wdeHeader, wdeHeaderSize);
      }
      else
      {
        LogPrint("\n  Window is a Graph\n");
        Graphs.Add(new Graph(name));
        _igraph = Graphs.Count - 1;
        GetWindowProperties(Graphs[_igraph], wdeHeader, wdeHeaderSize);
      }

      // go to end of window header
      _file.Seek(wdhStart + wdeHeaderSize + 1, SeekOrigin.Begin);

      // get layer list
      int layerListSize = 0;

      LogPrint(" Reading Layers ...\n");
      while (true)
      {
        _ilayer = layerListSize;
        if (!ReadLayerElement())
        {
          break;
        }

        layerListSize++;
      }
      LogPrint($" ... done. Layers: {layerListSize}\n");
      _curpos = _file.Position;
      LogPrint($"window ends at {_curpos} [0x{_curpos:X}]\n");

      return true;
    }

    public bool ReadLayerElement()
    {
      /* get general info and details of a layer
       * return true if a Layer is found, otherwise return false */
      int lye_header_size = 0;
      long lyh_start = 0;

      // get layer header size
      lye_header_size = ReadObjectSize();
      if (lye_header_size == 0)
      {
        return false;
      }

      _curpos = _file.Position;
      lyh_start = _curpos;
      LogPrint($"  Layer found: header size {lye_header_size} [0x{lye_header_size:X}], starts at {_curpos} [0x{_curpos:X}]");
      var lye_header = ReadObjectAsByteArray(lye_header_size);

      // get known info
      GetLayerProperties(lye_header, lye_header_size);

      // go to end of layer header
      _file.Seek(lyh_start + lye_header_size + 1, SeekOrigin.Begin);

      // get annotation list
      int annotation_list_size = 0;

      LogPrint("   Reading Annotations ...");
      /* Some annotations can be groups of annotations. We need a recursive function for those cases */
      annotation_list_size = ReadAnnotationList();
      if (annotation_list_size > 0)
      {
        LogPrint($"   ... done. Annotations: {annotation_list_size}");
      }

      // get curve list
      uint curve_list_size = 0;

      LogPrint("   Reading Curves ...");
      while (true)
      {
        if (!ReadCurveElement())
        {
          break;
        }

        curve_list_size++;
      }
      LogPrint($"   ... done. Curves: {curve_list_size}");

      // get axisbreak list
      uint axisbreak_list_size = 0;

      LogPrint("   Reading Axis breaks ...");
      while (true)
      {
        if (!ReadAxisBreakElement())
        {
          break;
        }

        axisbreak_list_size++;
      }
      LogPrint($"   ... done. Axis breaks: {axisbreak_list_size}");

      // get x axisparameter list
      uint axispar_x_list_size = 0;

      LogPrint("   Reading x-Axis parameters ...");
      while (true)
      {
        if (!ReadAxisParameterElement(1))
        {
          break;
        }

        axispar_x_list_size++;
      }
      LogPrint($"   ... done. x-Axis parameters: {axispar_x_list_size}");

      // get y axisparameter list
      uint axispar_y_list_size = 0;

      LogPrint("   Reading y-Axis parameters ...");
      while (true)
      {
        if (!ReadAxisParameterElement(2))
        {
          break;
        }

        axispar_y_list_size++;
      }
      LogPrint($"   ... done. y-Axis parameters: {axispar_y_list_size}");

      // get z axisparameter list
      uint axispar_z_list_size = 0;

      LogPrint("   Reading z-Axis parameters ...");
      while (true)
      {
        if (!ReadAxisParameterElement(3))
        {
          break;
        }

        axispar_z_list_size++;
      }
      LogPrint($"   ... done. z-Axis parameters: {axispar_z_list_size}");

      _curpos = _file.Position;
      LogPrint($"  layer ends at {_curpos} [0x{_curpos:X}]");

      return true;
    }

    public int ReadAnnotationList()
    {
      /* Purpose of this function is to allow recursive call for groups of annotation elements. */
      int annotationListSize = 0;

      while (true)
      {
        if (!ReadAnnotationElement())
        {
          break;
        }

        annotationListSize++;
      }
      return annotationListSize;
    }

    public bool ReadAnnotationElement()
    {
      /* get general info and details of an Annotation
       * return true if an Annotation is found, otherwise return false */
      int ane_header_size = 0;
      long anh_start = 0;

      // get annotation header size
      ane_header_size = ReadObjectSize();
      if (ane_header_size == 0)
      {
        return false;
      }

      _curpos = _file.Position;
      anh_start = _curpos;
      LogPrint($"    Annotation found: header size {ane_header_size} [0x{ane_header_size:X}], starts at {_curpos} [0x{_curpos:X}]: ");
      var ane_header = ReadObjectAsByteArray(ane_header_size);

      // get known info
      var name = _encoding.GetString(ane_header, 0x46, Math.Min(41, ane_header.Length - 0x46)).TrimEnd('\0');
      LogPrint($"{name}\n");

      // go to end of annotation header
      _file.Seek(anh_start + ane_header_size + 1, SeekOrigin.Begin);

      // data of an annotation element is divided in three blocks
      // first block
      int ane_data_1_size = 0;
      long andt1_start = 0;
      ane_data_1_size = ReadObjectSize();

      andt1_start = _file.Position;
      LogPrint($"     block 1 size {ane_data_1_size} [0x{ane_data_1_size:X}] at {andt1_start} [0x{andt1_start:X}]\n");
      var andt1_data = ReadObjectAsByteArray(ane_data_1_size);

      // TODO: get known info

      // go to end of first data block
      _file.Seek(andt1_start + ane_data_1_size + 1, SeekOrigin.Begin);

      // second block
      int ane_data_2_size = 0;
      long andt2_start = 0;
      ane_data_2_size = ReadObjectSize();
      andt2_start = _file.Position;
      LogPrint($"     block 2 size {ane_data_2_size} [0x{ane_data_2_size:X}] at {andt2_start} [0x{andt2_start:X}]\n");
      byte[] andt2_data;

      // check for group of annotations
      if (((ane_data_1_size == 0x5E) || (ane_data_1_size == 0x0A)) && (ane_data_2_size == 0x04))
      {
        _curpos = _file.Position;
        LogPrint($"  Annotation group found at {_curpos} [0x{_curpos:X}] ...\n");
        int angroup_size = ReadAnnotationList();
        _curpos = _file.Position;
        if (angroup_size > 0)
        {
          LogPrint($"  ... group end at {_curpos} [0x{_curpos:X}]. Annotations: {angroup_size}\n");
        }
        andt2_data = Array.Empty<byte>();
      }
      else
      {
        andt2_data = ReadObjectAsByteArray(ane_data_2_size);
        // TODO: get known info
        // go to end of second data block
        _file.Seek(andt2_start + ane_data_2_size, SeekOrigin.Begin);
        if (ane_data_2_size > 0)
        {
          _file.Seek(1, SeekOrigin.Current);
        }
      }

      // third block
      int ane_data_3_size = 0;
      ane_data_3_size = ReadObjectSize();

      long andt3_start = _file.Position;
      if (andt3_start > 0)
      {
        LogPrint($"     block 3 size {ane_data_3_size} [0x{ane_data_3_size:X}] at {andt3_start} [0x{andt3_start:X}]\n");
      }
      var andt3_data = ReadObjectAsByteArray(ane_data_3_size);

      _curpos = _file.Position;
      LogPrint($"    annotation ends at {_curpos} [0x{_curpos:X}]\n");

      // get annotation info
      GetAnnotationProperties(ane_header, ane_header_size, andt1_data, ane_data_1_size, andt2_data, ane_data_2_size, andt3_data, ane_data_3_size);

      return true;
    }

    public bool ReadCurveElement()
    {
      /* get general info and details of a Curve
       * return true if a Curve is found, otherwise return false */
      int cve_header_size = 0, cve_data_size = 0;
      long cvh_start = 0, cvd_start = 0;

      // get curve header size
      cve_header_size = ReadObjectSize();
      if (cve_header_size == 0)
      {
        return false;
      }

      _curpos = _file.Position;
      cvh_start = _curpos;
      LogPrint($"    Curve: header size {cve_header_size} [0x{cve_header_size:X}], starts at {_curpos} [0x{_curpos:X}], ");
      var cve_header = ReadObjectAsByteArray(cve_header_size);

      // TODO: get known info from curve header
      string name = _encoding.GetString(cve_header, 0x12, 12).TrimEnd('\0');

      // go to end of header, get curve data size
      _file.Seek(cvh_start + cve_header_size + 1, SeekOrigin.Begin);
      cve_data_size = ReadObjectSize();
      cvd_start = _file.Position;
      LogPrint($"data size {cve_data_size} [0x{cve_data_size:X}], from {cvd_start} [0x{cvd_start:X}]");
      var cve_data = ReadObjectAsByteArray(cve_data_size);

      // TODO: get known info from curve data

      // go to end of data
      _file.Seek(cvd_start + cve_data_size, SeekOrigin.Begin);
      if (cve_data_size > 0)
      {
        _file.Seek(1, SeekOrigin.Current);
      }

      _curpos = _file.Position;
      LogPrint($"to {_curpos} [0x{_curpos:X}]: {name}");

      // get curve (or column) info
      GetCurveProperties(cve_header, cve_header_size, cve_data, cve_data_size);

      return true;
    }

    public bool ReadAxisBreakElement()
    {
      /* get info of Axis breaks
       * return true if an Axis break, otherwise return false */
      int abeDataSize = 0;
      long abdStart = 0;

      // get axis break data size
      abeDataSize = ReadObjectSize();
      if (abeDataSize == 0)
      {
        return false;
      }

      long curpos = _file.Position;
      abdStart = curpos;
      var abdData = ReadObjectAsByteArray(abeDataSize);

      // go to end of axis break data
      _file.Seek(abdStart + abeDataSize + 1, SeekOrigin.Begin);

      // get axis break info
      GetAxisBreakProperties(abdData, abeDataSize);

      return true;
    }

    public bool ReadAxisParameterElement(int naxis)
    {
      /* get info of Axis parameters for naxis-axis (x,y,z) = (1,2,3)
       * return true if an Axis break is found, otherwise return false */
      int apeDataSize = 0;
      long apeStart = 0;

      // get axis break data size
      apeDataSize = ReadObjectSize();
      if (apeDataSize == 0)
      {
        return false;
      }

      _curpos = _file.Position;
      apeStart = _curpos;
      var apeData = ReadObjectAsByteArray(apeDataSize);

      // go to end of axis break data
      _file.Seek(apeStart + apeDataSize + 1, SeekOrigin.Begin);

      // get axis parameter info
      GetAxisParameterProperties(apeData, apeDataSize, naxis);

      return true;
    }

    public bool ReadParameterElement()
    {
      // get parameter name
      string par_name;

      par_name = ReadLine(_file, false);
      if (string.IsNullOrEmpty(par_name))
      {
        var eof_parameters_mark = ReadObjectSize();
        if (eof_parameters_mark != 0)
        {
          LogPrint("Wrong end of parameters mark\n");
        }
        return false;
      }
      LogPrint(" {0}:", par_name);

      // get value
      _file.ReadExactly(_buffer16, 0, sizeof(double) + 1);
      var value = BitConverter.ToDouble(_buffer16, 0);
      LogPrint(" {0}\n", value);

      // read the '\n'
      var c = _buffer16[sizeof(double)];
      if (c != '\n')
      {
        _curpos = _file.Position;
        LogPrint("Wrong delimiter {0} at {1} [0x{2:X}]\n", (char)c, _curpos, _curpos);
        _parseError = 6;
        throw new InvalidDataException($"Wrong delimiter (0x{c:2X} instead of 0x0A)) at file position 0x{(_file.Position - 1):X}");
      }

      Parameters.Add((par_name, value));

      return true;
    }

    public bool ReadNoteElement()
    {
      /* get info of Note windows, including "Results Log"
       * return true if a Note window is found, otherwise return false */
      int nweHeaderSize = 0, nweLabelSize = 0, nweContentsSize = 0;
      long nwhStart = 0, nwlStart = 0, nwcStart = 0;

      // get note header size
      nweHeaderSize = ReadObjectSize();
      if (nweHeaderSize == 0)
      {
        return false;
      }

      _curpos = _file.Position;
      nwhStart = _curpos;
      LogPrint($"  Note window found: header size {nweHeaderSize} [0x{nweHeaderSize:X}], starts at {_curpos} [0x{_curpos:X}]");
      var nweHeader = ReadObjectAsByteArray(nweHeaderSize);

      // TODO: get known info from header

      // go to end of header
      _file.Seek(nwhStart + nweHeaderSize + 1, SeekOrigin.Begin);

      // get label size
      nweLabelSize = ReadObjectSize();
      nwlStart = _file.Position;
      var nweLabel = ReadObjectAsByteArray(nweLabelSize);
      LogPrint($"  label at {nwlStart} [0x{nwlStart:X}]: {nweLabel}");

      // go to end of label
      _file.Seek(nwlStart + nweLabelSize, SeekOrigin.Begin);
      if (nweLabelSize > 0)
      {
        _file.Seek(1, SeekOrigin.Current);
      }

      // get contents size
      nweContentsSize = ReadObjectSize();
      nwcStart = _file.Position;
      var nweContents = ReadObjectAsByteArray(nweContentsSize);
      if (nwcStart > 0)
      {
        LogPrint($"  contents at {nwcStart} [0x{nwcStart:X}]: \n{nweContents}\n");
      }

      // get note window info
      GetNoteProperties(nweHeader, nweHeaderSize, nweLabel, nweLabelSize, nweContents, nweContentsSize);

      return true;
    }

    public void ReadProjectTree()
    {
      int pteDepth = 0;

      // first preamble size and data (usually 4)
      int ptePre1Size = ReadObjectSize();
      string ptePre1 = ReadObjectAsString(ptePre1Size);

      // second preamble size and data (usually 16)
      int ptePre2Size = ReadObjectSize();
      string ptePre2 = ReadObjectAsString(ptePre2Size);

      // root element and children
      int rootFolder = ReadFolderTree(null, pteDepth);
      if (rootFolder > 0)
      {
        LogPrint("Number of files at root: {0}\n", rootFolder);
      }

      // epilogue (should be zero)
      int ptePostSize = ReadObjectSize();
      if (ptePostSize != 0)
      {
        LogPrint("Wrong end of project tree mark\n");
      }

      // log info on project tree
#if GENERATE_CODE_FOR_LOG
        OutputProjectTree(Console.Out);
#endif // GENERATE_CODE_FOR_LOG

      return;
    }

    protected int ReadFolderTree(ProjectNode? parent, int depth)
    {
      int fle_header_size = 0, fle_eofh_size = 0, fle_name_size = 0, fle_prop_size = 0;

      // folder header size, data, end mark
      fle_header_size = ReadObjectSize();
      var fle_header = ReadObjectAsByteArray(fle_header_size);
      fle_eofh_size = ReadObjectSize(); // (usually 0)
      if (fle_eofh_size != 0)
      {
        LogPrint("Wrong end of folder header mark");
      }

      // folder name size
      fle_name_size = ReadObjectSize();
      long curpos = _file.Position;
      string fle_name = ReadObjectAsString(fle_name_size).TrimEnd('\0');
      LogPrint($"Folder name at {curpos} [0x{curpos:X}]: {fle_name}");

      // additional properties
      fle_prop_size = ReadObjectSize();
      for (int i = 0; i < fle_prop_size; i++)
      {
        int obj_size = ReadObjectSize();
        string obj_data = ReadObjectAsString(obj_size).Trim('\0');
      }

      // get project folder properties
      var current_folder = new ProjectNode(fle_name, ProjectNodeType.Folder);
      GetProjectFolderProperties(current_folder, fle_header, fle_header_size);
      if (parent is null)
        ProjectTree = current_folder;
      else
        parent.AppendChild(current_folder);

      // file entries
      int number_of_files_size = ReadObjectSize(); // should be 4 as number_of_files is an integer
      curpos = _file.Position;
      LogPrint($"Number of files at {curpos} [0x{curpos:X}] ");
      var fle_nfiles = ReadObjectAsByteArray(number_of_files_size);



      var number_of_files = BitConverter.ToInt32(fle_nfiles, 0);
      LogPrint($"{number_of_files}");

      for (int i = 0; i < number_of_files; i++)
      {
        ReadProjectLeaf(current_folder);
      }

      // subfolder entries
      int number_of_folders_size = ReadObjectSize(); // should be 4 as number_of_subfolders is an integer
      curpos = _file.Position;
      LogPrint($"Number of subfolders at {curpos} [0x{curpos:X}] ");
      var fle_nfolders = ReadObjectAsByteArray(number_of_folders_size);


      int number_of_folders = BitConverter.ToInt32(fle_nfolders, 0);
      LogPrint($"{number_of_folders}");

      for (uint i = 0; i < number_of_folders; i++)
      {
        depth++;
        int files_in_subfolder = ReadFolderTree(current_folder, depth);
        if (files_in_subfolder > 0)
        {
          LogPrint($"Number of files in subfolder: {files_in_subfolder}");
        }
        depth--;
      }


      return number_of_files;
    }

    public void ReadProjectLeaf(ProjectNode currentFolder)
    {
      // preamble size (usually 0) and data
      int ptlPreSize = ReadObjectSize();
      var ptlPre = ReadObjectAsByteArray(ptlPreSize);

      // file data size (usually 8) and data
      int ptlDataSize = ReadObjectSize();
      long curpos = _file.Position;
      var ptlData = ReadObjectAsByteArray(ptlDataSize);
      LogPrint($"File at {curpos} [0x{curpos:X}]\n");

      // epilogue (should be zero)
      int ptlPostSize = ReadObjectSize();
      if (ptlPostSize != 0)
      {
        LogPrint("Wrong end of project leaf mark\n");
      }

      // get project node properties
      GetProjectLeafProperties(currentFolder, ptlData, ptlDataSize);
    }

    public void ReadAttachmentList()
    {
      /* Attachments are divided in two groups (which can be empty)
         first group is preceded by two integers: 4096 (0x1000) and number_of_attachments followed as usual by a '\n' mark
         second group is a series of (header, name, data) triplets without the '\n' mark.
      */

      // figure out if first group is not empty. In this case we will read integer=8 at current file position
      int att_1st_empty = 0;
      _file.ReadExactly(_buffer16, 0, 4);
      att_1st_empty = BitConverter.ToInt32(_buffer16, 0);
      _file.Seek(-4, SeekOrigin.Current);

      byte[] att_header;
      if (att_1st_empty == 8)
      {
        // first group
        int att_list1_size = 0;

        // get two integers
        att_list1_size = ReadObjectSize(); // should be 8 as we expect two integer values
        _curpos = _file.Position;
        var att_list1 = ReadObjectAsByteArray(att_list1_size);
        LogPrint($"First attachment group at {_curpos} [0x{_curpos:X}]");

        int att_mark = 0, number_of_atts = 0, iattno = 0, att_data_size = 0;
        att_mark = BitConverter.ToInt32(att_list1, 0); // should be 4096
        number_of_atts = BitConverter.ToInt32(att_list1, 0x04);
        LogPrint($" with {number_of_atts} attachments.");

        for (uint i = 0; i < number_of_atts; i++)
        {
          /* Header is a group of 7 integers followed by \n
          1st  attachment mark (4096: 0x00 0x10 0x00 0x00)
          2nd  attachment number ( <num_of_att)
          3rd  attachment size
          4th .. 7th ???
          */
          // get header
          att_header = ReadObjectAsByteArray(7 * 4);
          att_mark = BitConverter.ToInt32(att_header, 0); // should be 4096
          iattno = BitConverter.ToInt32(att_header, 0x04);
          att_data_size = BitConverter.ToInt32(att_header, 0x08);
          _curpos = _file.Position;
          LogPrint($"Attachment no {i} ({iattno}) at {_curpos} [0x{_curpos:X}], size {att_data_size}");

          // get data
          string att_data = ReadObjectAsString(att_data_size);
          // even if att_data_size is zero, we get a '\n' mark
          if (att_data_size == 0)
          {
            _file.Seek(1, SeekOrigin.Current);
          }
        }
      }
      else
      {
        LogPrint("First attachment group is empty");
      }

      /* Second group is a series of (header, name, data) triplets
         There is no number of attachments. It ends when we reach EOF. */
      _curpos = _file.Position;
      LogPrint($"Second attachment group starts at {_curpos} [0x{_curpos:X}], file size {_d_file_size}");
      /* Header is a group of 3 integers, with no '\n' at end
        1st attachment header+name size including itself
        2nd attachment type (0x59 0x04 0xCA 0x7F for excel workbooks)
        3rd size of data */

      // get header
      while (true)
      {
        // check for eof
        if ((_d_file_size == _file.Position) || (_file.Position >= _d_file_size))
        {
          break;
        }
        // cannot use ReadObjectAsString: there is no '\n' at end
        byte[] headerBytes = new byte[12];
        _file.ReadExactly(headerBytes, 0, 12);

        if (headerBytes.Length != 12)
        {
          break;
        }
        // get header size, type and data size
        int att_header_size = 0, att_type = 0, att_size = 0;
        att_header_size = BitConverter.ToInt32(headerBytes, 0);
        att_type = BitConverter.ToInt32(headerBytes, 0x04);
        att_size = BitConverter.ToInt32(headerBytes, 0x08);

        // get name and data
        int name_size = att_header_size - 3 * 4;
        string att_name = new string('\0', (int)name_size);
        byte[] nameBytes = new byte[name_size];
        _file.ReadExactly(nameBytes, 0, (int)name_size);
        att_name = Encoding.UTF8.GetString(nameBytes);
        _curpos = _file.Position;
        string att_data = new string('\0', (int)att_size);
        byte[] dataBytes = new byte[att_size];
        _file.ReadExactly(dataBytes, 0, (int)att_size);
        att_data = Encoding.UTF8.GetString(dataBytes);
        LogPrint($"attachment at {_curpos} [0x{_curpos:X}], type 0x{att_type:X}, size {att_size} [0x{att_size:X}]: {att_name}");
      }

    }

    public bool GetColumnInfoAndData(byte[] colHeader, int colHeaderSize, byte[] colData, int colDataSize)
    {

      short dataType;
      byte dataTypeU;
      byte valueSize;
      dataType = BitConverter.ToInt16(colHeader, 0x16);

      dataTypeU = colHeader[0x3F];
      if (FileVersion == 350)
      {
        valueSize = (byte)colHeader[0x36];
      }
      else
      {
        valueSize = (byte)colHeader[0x3D];
      }
      if (valueSize == 0)
      {
        LogPrint($" WARNING : found strange valuesize of {valueSize}\n");
        valueSize = 8;
      }

      string name;
      if (FileVersion == 350)
      {
        // name = colHeader.Substring(0x57, 25);
        name = _encoding.GetString(colHeader, 0x57, 25).TrimEnd('\0');
      }
      else
      {
        name = _encoding.GetString(colHeader, 0x58, 25).TrimEnd('\0');
      }
      string datasetName = name;
      int colPos = name.LastIndexOf("_");

      string columnName = string.Empty;
      if (colPos != -1)
      {
        columnName = name.Substring(colPos + 1);
        name = name.Substring(0, colPos);
      }

      LogPrint("\n  data_type 0x{0:X4}, data_type_u 0x{1:X2}, valuesize {2} [0x{3:X}], {4} [{5}]\n", dataType, dataTypeU, valueSize, valueSize, name, columnName);

      int totalRows, firstRow, lastRow;
      totalRows = BitConverter.ToInt32(colHeader, 0x19);
      firstRow = BitConverter.ToInt32(colHeader, 0x19 + 1 * sizeof(Int32));
      lastRow = BitConverter.ToInt32(colHeader, 0x19 + 2 * sizeof(Int32));
      LogPrint("  total {0}, first {1}, last {2} rows\n", totalRows, firstRow, lastRow);

      ushort signature;
      if (colHeaderSize > 0x72)
      {
        signature = BitConverter.ToUInt16(colHeader, 0x71);
      }
      else
      {
        LogPrint("  NOTE: alternative signature determination\n");
        signature = (ushort)colHeader[0x18];
      }
      LogPrint("  signature {0} [0x{1:X}], valuesize {2} size {3} ", signature, signature, valueSize, colDataSize);

      int currentCol = 1;
      int colIndex = 0;
      int currentSheet = 0;
      int spread = 0;

      if (string.IsNullOrEmpty(columnName)) // Matrix or function
      {
        if (dataType == 0x6081) // Function
        {
          Functions.Add(new Function(name, _objectIndex));
          Function f = Functions[Functions.Count - 1];
          f.Formula = _encoding.GetString(colData).TrimEnd('\0').ToLowerInvariant();

          short t = BitConverter.ToInt16(colHeader, 0x0A);
          if (t == 0x1194)
          {
            f.Type = Function.FunctionType.Polar;
          }

          f.TotalPoints = BitConverter.ToInt32(colHeader, 0x21);
          f.Begin = BitConverter.ToDouble(colHeader, 0x21 + sizeof(Int32));
          double d = BitConverter.ToDouble(colHeader, 0x21 + sizeof(Int32) + sizeof(double));
          f.End = f.Begin + d * (f.TotalPoints - 1);

          LogPrint("\n NEW FUNCTION: {0} = {1}", f.Name, f.Formula);
          LogPrint(". Range [{0:g} : {1:g}], number of points: {2}\n", f.Begin, f.End, f.TotalPoints);
        }
        else // Matrix
        {
          int mIndex = -1;
          int pos = name.IndexOf("@");
          if (pos != -1)
          {
            string sheetName = name;
            name = name.Substring(0, pos);
            mIndex = FindMatrixByName(name);
            if (mIndex != -1)
            {
              LogPrint("\n  NEW MATRIX SHEET\n");
              Matrixes[mIndex].Sheets.Add(new MatrixSheet(sheetName, _objectIndex));
            }
          }
          else
          {
            LogPrint("\n  NEW MATRIX\n");
            Matrixes.Add(new Matrix(name));
            Matrixes[Matrixes.Count - 1].Sheets.Add(new MatrixSheet(name, _objectIndex));
          }
          Datasets.Add(new SpreadColumn(name, _objectIndex));
          GetMatrixValues(colData, colDataSize, dataType, dataTypeU, valueSize, mIndex);
        }
      }
      else
      {
        currentCol = 1;
        currentSheet = 0;
        if (SpreadSheets.Count == 0 || FindSpreadByName(name) == -1)
        {
          LogPrint("\n  NEW SPREADSHEET\n");
          SpreadSheets.Add(new SpreadSheet(name));
          spread = SpreadSheets.Count - 1;
          SpreadSheets[spread].MaxRows = 0;
        }
        else
        {
          spread = FindSpreadByName(name);
          currentCol = SpreadSheets[spread].Columns.Count;
          if (currentCol == 0)
          {
            currentCol = 1;
          }

          ++currentCol;
        }
        var newSpreadColumn = new SpreadColumn(columnName, _objectIndex)
        {
          ColIndex = ++colIndex,
          DatasetName = datasetName,
          NumRows = totalRows,
          BeginRow = firstRow,
          EndRow = lastRow,
        };
        SpreadSheets[spread].Columns.Add(newSpreadColumn);

        int sheetpos = newSpreadColumn.Name.LastIndexOf('@');
        if (sheetpos >= 0)
        {
          int sheet = int.Parse(columnName.Substring(sheetpos + 1), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
          if (sheet > 1)
          {
            newSpreadColumn.Name = columnName;

            if (currentSheet != (sheet - 1))
            {
              currentSheet = sheet - 1;
            }

            newSpreadColumn.Sheet = currentSheet;
            if (SpreadSheets[spread].Sheets < sheet)
            {
              SpreadSheets[spread].Sheets = sheet;
            }
          }
        }

        LogPrint($"  data index {_objectIndex}, valuesize {valueSize}");

        var nr = colDataSize / valueSize;
        LogPrint($"n. of rows = {nr}\n\n");

        SpreadSheets[spread].MaxRows = Math.Max(SpreadSheets[spread].MaxRows, nr);
        //stmp = new StringReader(col_data);
        for (int i = 0; i < nr; ++i)
        {
          double value;
          if (valueSize <= 8) // Numeric, Time, Date, Month, Day
          {
            value = BitConverter.ToDouble(colData, i * valueSize);
            if ((i < 5) || (i > (nr - 5)))
            {
              LogPrint($"{value} ");
            }
            else if (i == 5)
            {
              LogPrint("... ");
            }
            SpreadSheets[spread].Columns[currentCol - 1].Data.Add(new Variant(value));
          }
          else if ((dataType & 0x100) == 0x100) // Text&Numeric
          {
            byte c = colData[i * valueSize];
            if (c != 1) // value
            {
              value = BitConverter.ToDouble(colData, i * valueSize + 2);
              if ((i < 5) || (i > (nr - 5)))
              {
                LogPrint($"{value} ");
              }
              else if (i == 5)
              {
                LogPrint("... ");
              }
              SpreadSheets[spread].Columns[currentCol - 1].Data.Add(new Variant(value));
            }
            else // text
            {
              string svaltmp = _encoding.GetString(colData, i * valueSize + 2, valueSize - 2).TrimEnd('\0');
              // TODO: check if this test is still needed
#if NETFRAMEWORK
              if (svaltmp.Contains("\u000E"))
#else
              if (svaltmp.Contains((char)0x0E))
#endif
              { // try find non-printable symbol - garbage test
                svaltmp = string.Empty;
                LogPrint($"Non printable symbol found, place 1 for i={i}");
              }
              if ((i < 5) || (i > (nr - 5)))
              {
                LogPrint($"\"{svaltmp}\" ");
              }
              else if (i == 5)
              {
                LogPrint("... ");
              }
              SpreadSheets[spread].Columns[currentCol - 1].Data.Add(new Variant(svaltmp));
            }
          }
          else // text
          {
            string svaltmp = _encoding.GetString(colData, i * valueSize, valueSize).TrimEnd('\0');
            // TODO: check if this test is still needed
#if NETFRAMEWORK
            if (svaltmp.Contains("\u000E"))
#else
            if (svaltmp.Contains((char)0x0E))
#endif
            { // try find non-printable symbol - garbage test
              svaltmp = string.Empty;
              LogPrint($"Non printable symbol found, place 2 for i={i}");
            }
            if ((i < 5) || (i > (nr - 5)))
            {
              Console.Write($"\"{svaltmp}\" ");
            }
            else if (i == 5)
            {
              Console.Write("... ");
            }
            SpreadSheets[spread].Columns[currentCol - 1].Data.Add(new Variant(svaltmp));
          }
        }
        LogPrint("\n\n");
        Datasets.Add(SpreadSheets[spread].Columns[^1]);
      }

      ++_objectIndex;

      return true;
    }

    public void GetMatrixValues(byte[] colData, int colDataSize, short dataType, byte dataTypeU, byte valueSize, int mIndex)
    {
      if (Matrixes.Count == 0)
      {
        return;
      }

      if (mIndex < 0)
      {
        mIndex = Matrixes.Count - 1;
      }

      var size = colDataSize / valueSize;
      bool logValues = true;

      switch (dataType)
      {
        case 0x6001: // double
          for (int i = 0; i < size; ++i)
          {
            Matrixes[mIndex].Sheets[^1].Data.Add(BitConverter.ToDouble(colData, i * sizeof(double)));
          }
          break;
        case 0x6003: // float
          for (int i = 0; i < size; ++i)
          {
            Matrixes[mIndex].Sheets[^1].Data.Add(BitConverter.ToSingle(colData, i * sizeof(float)));
          }
          break;
        case 0x6201: // complex (2xdouble)
          {
            for (int i = 0; i < size; ++i)
            {
              Matrixes[mIndex].Sheets[^1].Data.Add(BitConverter.ToDouble(colData, i * valueSize));
              Matrixes[mIndex].Sheets[^1].ImaginaryData.Add(BitConverter.ToDouble(colData, i * valueSize + sizeof(Double)));
            }
          }
          break;
        case 0x6801: // int
          if (dataTypeU == 8) // unsigned
          {
            for (int i = 0; i < size; ++i)
            {
              Matrixes[mIndex].Sheets[^1].Data.Add(BitConverter.ToUInt32(colData, i * sizeof(UInt32)));
            }
          }
          else
          {
            for (int i = 0; i < size; ++i)
            {
              Matrixes[mIndex].Sheets[^1].Data.Add(BitConverter.ToInt32(colData, i * sizeof(Int32)));
            }

          }
          break;
        case 0x6803: // short
          if (dataTypeU == 8) // unsigned
          {
            for (int i = 0; i < size; ++i)
            {
              Matrixes[mIndex].Sheets[^1].Data.Add(BitConverter.ToUInt16(colData, i * sizeof(UInt16)));
            }
          }
          else
          {
            for (int i = 0; i < size; ++i)
            {
              Matrixes[mIndex].Sheets[^1].Data.Add(BitConverter.ToInt16(colData, i * sizeof(Int16)));
            }
          }
          break;
        case 0x6821: // char
          for (int i = 0; i < size; ++i)
          {
            Matrixes[(int)mIndex].Sheets[^1].Data.Add(colData[i]);
          }
          break;
        default:
          LogPrint("UNKNOWN MATRIX DATATYPE: {0:X2} SKIP DATA", dataType);
          Matrixes.RemoveAt(Matrixes.Count - 1);
          logValues = false;
          break;
      }

      if (logValues)
      {
        LogPrint("FIRST 10 CELL VALUES: ");
        for (uint i = 0; i < 10 && i < Matrixes[(int)mIndex].Sheets[^1].Data.Count; ++i)
        {
          LogPrint("{0}\t", Matrixes[(int)mIndex].Sheets[^1].Data[(int)i]);
        }
      }

    }

    public void GetWindowProperties(Origin.Window window, byte[] wde_header, int wde_header_size)
    {
      window.ObjectID = _objectIndex;
      _objectIndex++;

      window.FrameRect.Left = BitConverter.ToInt16(wde_header, 0x1B);
      window.FrameRect.Top = BitConverter.ToInt16(wde_header, 0x1B + 1 * sizeof(Int16));
      window.FrameRect.Right = BitConverter.ToInt16(wde_header, 0x1B + 2 * sizeof(Int16));
      window.FrameRect.Bottom = BitConverter.ToInt16(wde_header, 0x1B + 3 * sizeof(Int16));

      var c = wde_header[0x32];

      if ((c & 0x01) != 0)
      {
        window.State = WindowState.Minimized;
      }
      else if ((c & 0x02) != 0)
      {
        window.State = WindowState.Maximized;
      }

      if (wde_header[0x42] != 0)
      {
        window.WindowBackgroundColorGradient = (ColorGradientDirection)(wde_header[0x42] >> 2);
        window.WindowBackgroundColorBase.ColorType = ColorType.Regular;
        window.WindowBackgroundColorBase.Regular = wde_header[0x43];
        window.WindowBackgroundColorEnd.ColorType = ColorType.Regular;
        window.WindowBackgroundColorEnd.Regular = wde_header[0x44];
      }
      else
      {
        window.WindowBackgroundColorGradient = ColorGradientDirection.NoGradient;
        window.WindowBackgroundColorBase.ColorType = ColorType.Regular;
        window.WindowBackgroundColorBase.Regular = (byte)RegularColor.White;
        window.WindowBackgroundColorEnd.ColorType = ColorType.Regular;
        window.WindowBackgroundColorEnd.Regular = (byte)RegularColor.White;
      }
      LogPrint("ColorGradient {0}, base {1}, end {2}", window.WindowBackgroundColorGradient,
          window.WindowBackgroundColorBase.Regular, window.WindowBackgroundColorEnd.Regular);

      c = wde_header[0x69];

      if ((c & 0x01) != 0)
      {
        window.Title = WindowTitle.Label;
      }
      else if ((c & 0x02) != 0)
      {
        window.Title = WindowTitle.Name;
      }
      else
      {
        window.Title = WindowTitle.Both;
      }

      window.IsHidden = ((c & 0x08) != 0);
      if (window.IsHidden)
      {
        LogPrint("            WINDOW {0} NAME : {1} is hidden", _objectIndex, window.Name);
      }
      else
      {
        LogPrint("            WINDOW {0} NAME : {1} is not hidden", _objectIndex, window.Name);
      }

      if (wde_header_size > 0x82 && FileVersion >= 600)
      {
        // only projects of version 6.0 and higher have these
        var creationDate = BitConverter.ToDouble(wde_header, 0x73);
        var modificationDate = BitConverter.ToDouble(wde_header, 0x73 + sizeof(double));
        window.CreationDate = DoubleToPosixTime(creationDate);
        window.ModificationDate = DoubleToPosixTime(modificationDate);
      }

      if ((wde_header_size > 0x9E) && (wde_header[0x42] != 0))
      {
        // get window background colors for version > 5.0
        window.WindowBackgroundColorBase = GetColor(_encoding.GetString(wde_header, 0x97, 4));
        window.WindowBackgroundColorEnd = GetColor(_encoding.GetString(wde_header, 0x9B, 4));
      }

      if (wde_header_size > 0xC3)
      {
        window.Label = _encoding.GetString(wde_header, 0xC3, wde_header.Length - 0xC3).TrimEnd('\0');
        int idx = window.Label.IndexOfAny(new char[] { '@', '$', '{' });
        if (idx > 0)
        {
          window.Label = window.Label.Substring(0, idx);
        }
        LogPrint("            WINDOW {0} LABEL: {1}", _objectIndex, window.Label);
      }

      if (_imatrix != -1) // additional properties for matrix windows
      {
        byte h = (byte)wde_header[0x29];
        Matrixes[_imatrix].ActiveSheet = h;
        if (wde_header_size > 0x86)
        {
          h = (byte)wde_header[0x87];
          Matrixes[_imatrix].Header = (h == 194) ? Matrix.HeaderViewType.XY : Matrix.HeaderViewType.ColumnRow;
        }
      }
      if (_igraph != -1) // additional properties for graph/layout windows
      {

        Graphs[_igraph].Width = BitConverter.ToInt16(wde_header, 0x23);
        Graphs[_igraph].Height = BitConverter.ToInt16(wde_header, 0x23 + sizeof(Int16));


        var co = wde_header[0x38];
        Graphs[_igraph].ConnectMissingData = ((co & 0x40) != 0);

        string templateName = _encoding.GetString(wde_header, 0x45, 20).TrimEnd('\0');
        Graphs[_igraph].TemplateName = templateName;
        if (templateName == "LAYOUT")
        {
          Graphs[_igraph].IsLayout = true;
        }
      }
    }

    public void GetLayerProperties(byte[] lye_header, int lye_header_size)
    {
      int cursor = 0;
      if (_ispread != -1) // spreadsheet
      {
        SpreadSheets[_ispread].Loose = false;
      }
      else if (_imatrix != -1) // matrix
      {
        var sheet = Matrixes[_imatrix].Sheets[_ilayer];

        short width = 8;
        width = BitConverter.ToInt16(lye_header, 0);
        if (width == 0)
        {
          width = 8;
        }

        sheet.Width = width;

        sheet.ColumnCount = BitConverter.ToInt16(lye_header, 0x2B);
        sheet.RowCount = BitConverter.ToInt16(lye_header, 0x52);

        byte view = (byte)lye_header[0x71];
        sheet.View = (view != 0x32 && view != 0x28) ? MatrixSheet.ViewType.ImageView : MatrixSheet.ViewType.DataView;

        if (lye_header_size > 0xD2)
        {
          sheet.Name = _encoding.GetString(lye_header, 0xD2, 32).TrimEnd('\0');
        }
      }
      else if (_iexcel != -1) // excel
      {
        Excels[_iexcel].Loose = false;
        if (lye_header_size > 0xD2)
        {
          Excels[_iexcel].Sheets[_ilayer].Name = _encoding.GetString(lye_header, 0xD2, 32).TrimEnd('\0');
        }
      }
      else // graph
      {
        Graphs[_igraph].Layers.Add(new GraphLayer());
        var glayer = Graphs[_igraph].Layers[_ilayer];

        glayer.XAxis.Min = BitConverter.ToDouble(lye_header, cursor); cursor += sizeof(double);
        glayer.XAxis.Max = BitConverter.ToDouble(lye_header, cursor); cursor += sizeof(double);
        glayer.XAxis.Step = BitConverter.ToDouble(lye_header, cursor); cursor += sizeof(double);

        glayer.XAxis.MajorTicks = (byte)lye_header[0x2B];

        byte g = (byte)lye_header[0x2D];
        glayer.XAxis.ZeroLine = (g & 0x80) != 0;
        glayer.XAxis.OppositeLine = (g & 0x40) != 0;

        glayer.XAxis.MinorTicks = (byte)lye_header[0x37];
        glayer.XAxis.Scale = (byte)lye_header[0x38];

        glayer.YAxis.Min = BitConverter.ToDouble(lye_header, cursor); cursor += sizeof(double);
        glayer.YAxis.Max = BitConverter.ToDouble(lye_header, cursor); cursor += sizeof(double);
        glayer.YAxis.Step = BitConverter.ToDouble(lye_header, cursor); cursor += sizeof(double);

        glayer.YAxis.MajorTicks = (byte)lye_header[0x56];

        g = (byte)lye_header[0x58];
        glayer.YAxis.ZeroLine = (g & 0x80) != 0;
        glayer.YAxis.OppositeLine = (g & 0x40) != 0;

        glayer.YAxis.MinorTicks = (byte)lye_header[0x62];
        glayer.YAxis.Scale = (byte)lye_header[0x63];

        g = (byte)lye_header[0x68];
        glayer.GridOnTop = (g & 0x04) != 0;
        glayer.ExchangedAxes = (g & 0x40) != 0;

        glayer.ClientRect.Left = BitConverter.ToInt16(lye_header, cursor); cursor += sizeof(Int16);
        glayer.ClientRect.Top = BitConverter.ToInt16(lye_header, cursor); cursor += sizeof(Int16);
        glayer.ClientRect.Right = BitConverter.ToInt16(lye_header, cursor); cursor += sizeof(Int16);
        glayer.ClientRect.Bottom = BitConverter.ToInt16(lye_header, cursor); cursor += sizeof(Int16);

        byte border = (byte)lye_header[0x89];
        glayer.BorderType = (BorderType)(border >= 0x80 ? border - 0x80 : (int)BorderType.None);

        if (lye_header_size > 0x107)
        {
          glayer.BackgroundColor = GetColor(_encoding.GetString(lye_header, 0x105, 4));
        }
      }
    }

    public Color GetColor(string strbincolor)
    {
      /* decode a color value from a 4 byte binary string */
      Color result = new Color(ColorType.Regular, RegularColor.Black);
      byte[] sbincolor = new byte[4];

      for (int i = 0; i < 4; i++)
      {
        sbincolor[i] = (byte)strbincolor[i];
      }

      switch (sbincolor[3])
      {
        case 0:
          if (sbincolor[0] < 0x64)
          {
            result.Regular = sbincolor[0];
          }
          else
          {
            switch (sbincolor[2])
            {
              case 0:
                result.ColorType = ColorType.Indexing;
                break;
              case 0x40:
                result.ColorType = ColorType.Mapping;
                break;
              case 0x80:
                result.ColorType = ColorType.RGB;
                break;
            }
            result.Column = (byte)(sbincolor[0] - 0x64);
          }
          break;
        case 1:
          result.ColorType = ColorType.Custom;
          result.Regular = sbincolor[0];
          result.Starting = sbincolor[1];
          result.Column = sbincolor[2];
          break;
        case 0x20:
          result.ColorType = ColorType.Increment;
          result.Starting = sbincolor[1];
          break;
        case 0xFF:
          if (sbincolor[0] == 0xFC)
          {
            result.ColorType = ColorType.None;
          }
          else if (sbincolor[0] == 0xF7)
          {
            result.ColorType = ColorType.Automatic;
          }
          else
          {
            result.Regular = sbincolor[0];
          }
          break;
        default:
          result.ColorType = ColorType.Regular;
          result.Regular = sbincolor[0];
          break;
      }
      return result;
    }

    public void GetAnnotationProperties(byte[] anhd, int anhdsz, byte[] andt1, int andt1sz, byte[] andt2, int andt2sz, byte[] andt3, int andt3sz)
    {
      //StringReader stmp = new StringReader(andt1);
      // (void) anhdsz; (void) andt3; (void) andt3sz; // Not needed in C#
      int cursor = 0;
      string sec_name = _encoding.GetString(anhd, 0x46, Math.Min(41, anhd.Length - 0x46)).TrimEnd('\0');
      if (_ispread != -1)
      {
        int col_index = FindColumnByName((int)_ispread, sec_name);
        if (col_index != -1) //check if it is a formula
        {
          SpreadSheets[_ispread].Columns[col_index].Command = _encoding.GetString(andt1, 0, andt1.Length);
          LogPrint($"Column: {sec_name} has formula: {SpreadSheets[_ispread].Columns[col_index].Command}");
        }
      }
      else if (_imatrix != -1)
      {
        MatrixSheet sheet = Matrixes[_imatrix].Sheets[_ilayer];

        //stmp = new StringReader(andt1);
        if (sec_name == "MV")
        {
          sheet.Command = _encoding.GetString(andt1, 0, andt1.Length).TrimEnd('\0');
        }
        else if (sec_name == "Y2" || sec_name == "X2" || sec_name == "Y1" || sec_name == "X1")
        {
          var idxCoordinate = sec_name switch
          {
            "Y2" => 0,
            "X2" => 1,
            "Y1" => 2,
            "X1" => 3,
            _ => throw new NotImplementedException()
          };

          if (andt1.Length >= sizeof(double))
          {
            sheet.Coordinates[idxCoordinate] = BitConverter.ToDouble(andt1, cursor); cursor += sizeof(Double);
          }
          else if (andt1.Length >= sizeof(float))
          {
            sheet.Coordinates[idxCoordinate] = BitConverter.ToSingle(andt1, cursor); cursor += sizeof(Single);
          }
          else if (andt1.Length >= sizeof(Int16))
          {
            sheet.Coordinates[idxCoordinate] = BitConverter.ToInt16(andt1, cursor); cursor += sizeof(Int16);
          }
          else
          {
            throw new NotImplementedException($"Annotation named {sec_name} has {andt1.Length} of data");
          }
        }
        else if (sec_name == "COLORMAP")
        {
          // Color maps for matrix annotations are similar to color maps for graph curves (3D).
          // They differ only in the start offset to the data string.
          GetColorMap(sheet.ColorMap, andt2, andt2sz);
        }
      }
      else if (_iexcel != -1)
      {
        int col_index = FindExcelColumnByName(_iexcel, _ilayer, sec_name);
        if (col_index != -1) //check if it is a formula
        {
          Excels[_iexcel].Sheets[_ilayer].Columns[col_index].Command = _encoding.GetString(andt1, 0, andt1.Length).TrimEnd('\0');
        }
      }
      else
      {
        GraphLayer glayer = Graphs[_igraph].Layers[_ilayer];
        Rect r = new Rect()
        {
          Left = BitConverter.ToInt16(anhd, 3 + 0 * sizeof(Int16)),
          Top = BitConverter.ToInt16(anhd, 3 + 1 * sizeof(Int16)),
          Right = BitConverter.ToInt16(anhd, 3 + 2 * sizeof(Int16)),
          Bottom = BitConverter.ToInt16(anhd, 3 + 3 * sizeof(Int16)),
        };

        byte attach = (byte)anhd[0x28];
        bool shown = true;
        if (attach == 0x32)
        {
          shown = false;
        }
        if (attach >= (byte)Attach.End_)
        {
          attach = (byte)Attach.Frame;
        }

        byte border = (byte)anhd[0x29];

        Color color = GetColor(_encoding.GetString(anhd, 0x33, 4));

        var andt1String = _encoding.GetString(andt1, 0, andt1.Length).TrimEnd('\0');
        if (sec_name == "PL")
        {
          glayer.YAxis.FormatAxis[0].Prefix = andt1String;
        }

        if (sec_name == "PR")
        {
          glayer.YAxis.FormatAxis[1].Prefix = andt1String;
        }

        if (sec_name == "PB")
        {
          glayer.XAxis.FormatAxis[0].Prefix = andt1String;
        }

        if (sec_name == "PT")
        {
          glayer.XAxis.FormatAxis[1].Prefix = andt1String;
        }

        if (sec_name == "SL")
        {
          glayer.YAxis.FormatAxis[0].Suffix = andt1String;
        }

        if (sec_name == "SR")
        {
          glayer.YAxis.FormatAxis[1].Suffix = andt1String;
        }

        if (sec_name == "SB")
        {
          glayer.XAxis.FormatAxis[0].Suffix = andt1String;
        }

        if (sec_name == "ST")
        {
          glayer.XAxis.FormatAxis[1].Suffix = andt1String;
        }

        if (sec_name == "OL")
        {
          glayer.YAxis.FormatAxis[0].Factor = andt1String;
        }

        if (sec_name == "OR")
        {
          glayer.YAxis.FormatAxis[1].Factor = andt1String;
        }

        if (sec_name == "OB")
        {
          glayer.XAxis.FormatAxis[0].Factor = andt1String;
        }

        if (sec_name == "OT")
        {
          glayer.XAxis.FormatAxis[1].Factor = andt1String;
        }
        if (sec_name == "X1T")
        {
          glayer.XAxis.Anchor = double.Parse(andt1String, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        }
        if (sec_name == "Y1T")
        {
          glayer.YAxis.Anchor = double.Parse(andt1String, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        }
        if (sec_name == "Z1T")
        {
          glayer.ZAxis.Anchor = double.Parse(andt1String, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        }

        byte type = andt1[0x00];
        LineVertex begin = new LineVertex();
        LineVertex end = new LineVertex();
        byte ankind = anhd[0x02];
        if (ankind == 0x22) // Line/Arrow
        {
          if (attach == (byte)Attach.Scale && andt1sz > 0x5F)
          {
            if (type == 2)
            {
              cursor = 0x20;
              begin.X = BitConverter.ToDouble(andt1, cursor); cursor += sizeof(double);
              end.X = BitConverter.ToDouble(andt1, cursor); cursor += sizeof(double);
              cursor = 0x40;
              begin.Y = BitConverter.ToDouble(andt1, cursor); cursor += sizeof(double);
              end.Y = BitConverter.ToDouble(andt1, cursor); cursor += sizeof(double);
            }
            else if (type == 4) // curved arrow
            {
              cursor = 0x20;
              begin.X = BitConverter.ToDouble(andt1, cursor); cursor += sizeof(double);
              end.X = BitConverter.ToDouble(andt1, cursor); cursor += sizeof(double);
              end.X = BitConverter.ToDouble(andt1, cursor); cursor += sizeof(double);
              end.X = BitConverter.ToDouble(andt1, cursor); cursor += sizeof(double);
              begin.Y = BitConverter.ToDouble(andt1, cursor); cursor += sizeof(double);
              end.Y = BitConverter.ToDouble(andt1, cursor); cursor += sizeof(double);
              end.Y = BitConverter.ToDouble(andt1, cursor); cursor += sizeof(double);
              end.Y = BitConverter.ToDouble(andt1, cursor); cursor += sizeof(double);
            }
          }
          else
          {
            short x1 = 0, x2 = 0, y1 = 0, y2 = 0;
            if (type == 2) // straight line/arrow
            {
              cursor = 0x01;
              x1 = BitConverter.ToInt16(andt1, cursor); cursor += sizeof(Int16);
              x2 = BitConverter.ToInt16(andt1, cursor); cursor += sizeof(Int16);
              cursor += 4; // Skip to the relevant part
              y1 = BitConverter.ToInt16(andt1, cursor); cursor += sizeof(Int16);
              y2 = BitConverter.ToInt16(andt1, cursor); cursor += sizeof(Int16);
            }
            else if (type == 4) // curved line/arrow has 4 points
            {
              cursor = 0x01; // Skip to the relevant part
              x1 = BitConverter.ToInt16(andt1, cursor); cursor += sizeof(Int16);
              cursor += 4; // Skip to the relevant part
              x2 = BitConverter.ToInt16(andt1, cursor); cursor += sizeof(Int16);
              y1 = BitConverter.ToInt16(andt1, cursor); cursor += sizeof(Int16);
              cursor += 4; // Skip to the relevant part
              y2 = BitConverter.ToInt16(andt1, cursor); cursor += sizeof(Int16);
            }

            double dx = Math.Abs(x2 - x1);
            double dy = Math.Abs(y2 - y1);
            double minx = Math.Min(x1, x2);
            double miny = Math.Min(y1, y2);

            begin.X = (x1 == x2) ? r.Left + 0.5 * r.Width() : r.Left + (x1 - minx) / dx * r.Width();
            end.X = (x1 == x2) ? r.Left + 0.5 * r.Width() : r.Left + (x2 - minx) / dx * r.Width();
            begin.Y = (y1 == y2) ? r.Top + 0.5 * r.Height() : r.Top + (y1 - miny) / dy * r.Height();
            end.Y = (y1 == y2) ? r.Top + 0.5 * r.Height() : r.Top + (y2 - miny) / dy * r.Height();
          }
          if (andt1sz > 0x011)
          {
            byte arrows = (byte)andt1[0x11];
            switch (arrows)
            {
              case 0:
                begin.ShapeType = 0;
                end.ShapeType = 0;
                break;
              case 1:
                begin.ShapeType = 1;
                end.ShapeType = 0;
                break;
              case 2:
                begin.ShapeType = 0;
                end.ShapeType = 1;
                break;
              case 3:
                begin.ShapeType = 1;
                end.ShapeType = 1;
                break;
            }
          }
          if (andt1sz > 0x77)
          {
            begin.ShapeType = (byte)andt1[0x60];
            int w = 0;
            cursor = 0x64; // Skip to the relevant part
            w = BitConverter.ToInt32(andt1, cursor); cursor += sizeof(Int32);
            begin.ShapeWidth = (double)w / 500.0;
            w = BitConverter.ToInt32(andt1, cursor); cursor += sizeof(Int32);
            begin.ShapeLength = (double)w / 500.0;

            end.ShapeType = (byte)andt1[0x6C];
            cursor = 0x70; // Skip to the relevant part
            w = BitConverter.ToInt32(andt1, cursor); cursor += sizeof(Int32);
            end.ShapeWidth = (double)w / 500.0;
            w = BitConverter.ToInt32(andt1, cursor); cursor += sizeof(Int32);
            end.ShapeLength = (double)w / 500.0;
          }
        }
        // text properties
        short rotation = 0;
        byte fontSize = 12;
        if (andt1sz > 0x04)
        {
          rotation = BitConverter.ToInt16(andt1, 0x02);
          fontSize = (byte)andt1[0x04];
        }
        byte tab = andt1.Length > 0x0A ? andt1[0x0A] : (byte)8;

        // line properties
        byte lineStyle = andt1.Length > 0x12 ? andt1[0x12] : (byte)0;
        short w1 = 0;
        if (andt1sz > 0x14)
        {
          w1 = BitConverter.ToInt16(andt1, 0x13);
        }
        double width = (double)w1 / 500.0;

        Figure figure = new Figure();
        if (andt1sz > 0x06)
        {
          w1 = BitConverter.ToInt16(andt1, 0x05);
          figure.Width = (double)w1 / 500.0;
        }
        if (andt1sz > 0x08)
        {
          figure.Style = (byte)andt1[0x08];
        }
        if (andt1sz > 0x4E)
        {
          figure.FillAreaColor = GetColor(_encoding.GetString(andt1, 0x42, 4));
          cursor = 0x46; // Simulating the behavior of istringstream
          short w2 = BitConverter.ToInt16(andt1, cursor);
          figure.FillAreaPatternWidth = (double)w2 / 500.0;
          figure.FillAreaPatternColor = GetColor(_encoding.GetString(andt1, 0x4A, 4));
          figure.FillAreaPattern = (FillPattern)andt1[0x4E];
        }
        if (andt1sz > 0x57)
        {
          byte h = andt1[0x57];
          figure.UseBorderColor = (h == 0x10);
        }

        if (sec_name == "XB")
        {
          string text = _encoding.GetString(andt2, 0, andt2.Length).TrimEnd('\0');
          glayer.XAxis.Position = GraphAxis.AxisPosition.Bottom;
          glayer.XAxis.FormatAxis[0].Label = new TextBox(text, r, color, fontSize, rotation / 10, tab, (BorderType)(border >= 0x80 ? border - 0x80 : (int)BorderType.None), (Attach)attach, shown);
        }
        else if (sec_name == "XT")
        {
          string text = _encoding.GetString(andt2, 0, andt2.Length).TrimEnd('\0');
          glayer.XAxis.Position = GraphAxis.AxisPosition.Top;
          glayer.XAxis.FormatAxis[1].Label = new TextBox(text, r, color, fontSize, rotation / 10, tab, (BorderType)(border >= 0x80 ? border - 0x80 : (int)BorderType.None), (Attach)attach, shown);
        }
        else if (sec_name == "YL")
        {
          string text = _encoding.GetString(andt2, 0, andt2.Length).TrimEnd('\0');
          glayer.YAxis.Position = GraphAxis.AxisPosition.Left;
          glayer.YAxis.FormatAxis[0].Label = new TextBox(text, r, color, fontSize, rotation / 10, tab, (BorderType)(border >= 0x80 ? border - 0x80 : (int)BorderType.None), (Attach)attach, shown);
        }
        else if (sec_name == "YR")
        {
          string text = _encoding.GetString(andt2, 0, andt2.Length).TrimEnd('\0');
          glayer.YAxis.Position = GraphAxis.AxisPosition.Right;
          glayer.YAxis.FormatAxis[1].Label = new TextBox(text, r, color, fontSize, rotation / 10, tab, (BorderType)(border >= 0x80 ? border - 0x80 : (int)BorderType.None), (Attach)attach, shown);
        }
        else if (sec_name == "ZF")
        {
          string text = _encoding.GetString(andt2, 0, andt2.Length).TrimEnd('\0');
          glayer.ZAxis.Position = GraphAxis.AxisPosition.Front;
          glayer.ZAxis.FormatAxis[0].Label = new TextBox(text, r, color, fontSize, rotation / 10, tab, (BorderType)(border >= 0x80 ? border - 0x80 : (int)BorderType.None), (Attach)attach, shown);
        }
        else if (sec_name == "ZB")
        {
          string text = _encoding.GetString(andt2, 0, andt2.Length).TrimEnd('\0');
          glayer.ZAxis.Position = GraphAxis.AxisPosition.Back;
          glayer.ZAxis.FormatAxis[1].Label = new TextBox(text, r, color, fontSize, rotation / 10, tab, (BorderType)(border >= 0x80 ? border - 0x80 : (int)BorderType.None), (Attach)attach, shown);
        }
        else if (sec_name == "3D")
        {
          cursor = 0;
          glayer.ZAxis.Min = BitConverter.ToDouble(andt2, cursor); cursor += sizeof(double);
          glayer.ZAxis.Max = BitConverter.ToDouble(andt2, cursor); cursor += sizeof(double);
          glayer.ZAxis.Step = BitConverter.ToDouble(andt2, cursor); cursor += sizeof(double);
          glayer.ZAxis.MajorTicks = andt2[0x1C];
          glayer.ZAxis.MinorTicks = andt2[0x28];
          glayer.ZAxis.Scale = andt2[0x29];

          cursor = 0x5A;
          glayer.XAngle = BitConverter.ToSingle(andt2, cursor); cursor += sizeof(Single);
          glayer.YAngle = BitConverter.ToSingle(andt2, cursor); cursor += sizeof(Single);
          glayer.ZAngle = BitConverter.ToSingle(andt2, cursor); cursor += sizeof(Single);

          cursor = 0x218;
          glayer.XLength = BitConverter.ToSingle(andt2, cursor) / 23f; cursor += sizeof(Single);
          glayer.YLength = BitConverter.ToSingle(andt2, cursor) / 23f; cursor += sizeof(Single);
          glayer.ZLength = BitConverter.ToSingle(andt2, cursor) / 23f; cursor += sizeof(Single);

          glayer.Orthographic3D = (andt2[0x240] != 0);
        }
        else if (sec_name == "Legend" || sec_name == "legend")
        {
          string text = _encoding.GetString(andt2, 0, andt2.Length).TrimEnd('\0');
          glayer.Legend = new TextBox(text, r, color, fontSize, rotation / 10, tab, (BorderType)(border >= 0x80 ? border - 0x80 : (int)BorderType.None), (Attach)attach);
        }
        else if (sec_name == "__BCO2")
        { // histogram
          cursor = 0x10;
          glayer.HistogramBin = BitConverter.ToDouble(andt2, cursor); cursor += sizeof(double);
          cursor = 0x20;
          glayer.HistogramEnd = BitConverter.ToDouble(andt2, cursor); cursor += sizeof(double);
          glayer.HistogramBegin = BitConverter.ToDouble(andt2, cursor); cursor += sizeof(double);

          glayer.Percentile.P1SymbolType = andt2[0x5E];
          glayer.Percentile.P99SymbolType = andt2[0x5F];
          glayer.Percentile.MeanSymbolType = andt2[0x60];
          glayer.Percentile.MaxSymbolType = andt2[0x61];
          glayer.Percentile.MinSymbolType = andt2[0x62];

          glayer.Percentile.Labels = andt2[0x9F];
          glayer.Percentile.WhiskersRange = andt2[0x6B];
          glayer.Percentile.BoxRange = andt2[0x6C];
          // 0x8e = 0x5E+141-93 = 142
          glayer.Percentile.WhiskersCoeff = andt2[0x8e];
          glayer.Percentile.BoxCoeff = andt2[0x8f];
          var h = andt2[0x90];
          glayer.Percentile.DiamondBox = (h == 0x82) ? true : false;
          // 0xCB = 0x5E+109 = 203
          glayer.Percentile.SymbolSize = BitConverter.ToInt16(andt2, 0xCB);
          glayer.Percentile.SymbolSize = (short)(glayer.Percentile.SymbolSize / 2 + 1);
          // 0x101 = 0x5E+163
          glayer.Percentile.SymbolColor = GetColor(_encoding.GetString(andt2, 0x101, 4));
          glayer.Percentile.SymbolFillColor = GetColor(_encoding.GetString(andt2, 0x105, 4));
        }
        else if (sec_name == "_206")
        {
          // box plot labels
        }
        else if (sec_name == "VLine")
        {
          double start = BitConverter.ToDouble(andt1, 0x0A);
          var vLineWidth = BitConverter.ToDouble(andt1, 0x1A);
          glayer.VLine = start + 0.5 * vLineWidth;
          glayer.ImageProfileTool = 2;
        }
        else if (sec_name == "HLine")
        {
          double start = BitConverter.ToDouble(andt1, 0x12);
          var hLineWidth = BitConverter.ToDouble(andt1, 0x22);
          glayer.HLine = start + 0.5 * hLineWidth;
          glayer.ImageProfileTool = 2;
        }
        else if (sec_name == "vline")
        {
          glayer.VLine = BitConverter.ToDouble(andt1, 0x20);
          glayer.ImageProfileTool = 1;
        }
        else if (sec_name == "hline")
        {
          glayer.HLine = BitConverter.ToDouble(andt1, 0x40);
          glayer.ImageProfileTool = 1;
        }
        else if (sec_name == "ZCOLORS")
        {
          glayer.IsXYY3D = true;
          if (FileVersion < 600)
          {
            ColorMap colorMap = glayer.ColorMap;
            GetZColorsMap(colorMap, andt2, andt2sz);
          }
        }
        else if (sec_name == "SPECTRUM1")
        {
          glayer.IsXYY3D = false;
          glayer.ColorScale.IsVisible = true;
          glayer.ColorScale.ReverseOrder = (andt2[24] != 0);
          glayer.ColorScale.ColorBarThickness = BitConverter.ToInt16(andt2, 0x20);
          glayer.ColorScale.LabelGap = BitConverter.ToInt16(andt2, 0x22);
          glayer.ColorScale.LabelsColor = GetColor(_encoding.GetString(andt2, 92, 4));
        }
        else if (sec_name == "&0")
        {
          glayer.IsWaterfall = true;
          string text = _encoding.GetString(andt1, 0, andt1.Length).TrimEnd('\0');
          int commaPos = text.IndexOf(",");
          throw new NotImplementedException("Please Implement the next two lines");
          //glayer.xOffset = double.Parse(stmp.ReadLine());
          //glayer.yOffset = double.Parse(stmp.ReadLine());
        }
        /* OriginNNNParser identify text, circle, rectangle and bitmap annotation by checking size of andt1:
                         text/pie text          rectangle/circle       line            bitmap
               Origin410: 22                    0xA(10)                21/24           38
               Origin500: 22                    0xA(10)                24              40
               Origin610: 22                    0xA(10)                24/96           40
               Origin700:                       0x5E(94)               120             0x28(40)
               Origin750: 0x3E(62)/78           0x5E(94)              0x78(120)        0x28(40)
               Origin850: 0x3E(62)/78           0x5E(94)              0x78(120)        0x28(40)
               An alternative is to look at anhd[0x02]:
                 (0x00 for Text, 0x21 for Circle/Rect, 0x22 for Line/Arrow, 0x23 for Polygon/Polyline)
        */
        else if ((ankind == 0x0) && (sec_name != "DelData")) // text
        {
          string text = _encoding.GetString(andt2, 0, andt2.Length).TrimEnd('\0');
          if (sec_name.StartsWith("PIE"))
          {
            glayer.PieTexts.Add(new TextBox(text, r, color, fontSize, rotation / 10, tab, (BorderType)(border >= 0x80 ? border - 0x80 : (int)BorderType.None), (Attach)attach));
          }
          else
          {
            glayer.Texts.Add(new TextBox(text, r, color, fontSize, rotation / 10, tab, (BorderType)(border >= 0x80 ? border - 0x80 : (int)BorderType.None), (Attach)attach));
          }
        }
        else if (ankind == 0x21) // rectangle & circle
        {
          switch (type) // type = andt1[0x00]
          {
            case 0:
            case 1:
              figure.FigureType = FigureType.Rectangle;
              break;
            case 2:
            case 3:
              figure.FigureType = FigureType.Circle;
              break;
          }
          figure.ClientRect = r;
          figure.Attach = (Attach)attach;
          figure.Color = color;

          glayer.Figures.Add(figure);
        }
        else if ((ankind == 0x22) && (sec_name != "sLine") && (sec_name != "sline")) // line/arrow
        {
          glayer.Lines.Add(new Line());
          Line line = glayer.Lines[^1];
          line.Color = color;
          line.ClientRect = r;
          line.Attach = (Attach)attach;
          line.Width = width;
          line.Style = lineStyle;
          line.Begin = begin;
          line.End = end;
        }
        else if (andt1sz == 40) // bitmap
        {
          if (type == 4) // type = andt1[0x00]
          {
            long filesize = andt2sz + 14;
            glayer.Bitmaps.Add(new Bitmap());
            Bitmap bitmap = glayer.Bitmaps[^1];
            bitmap.ClientRect = r;
            bitmap.Attach = (Attach)attach;
            bitmap.Size = filesize;
            bitmap.BorderType = (BorderType)(border >= 0x80 ? border - 0x80 : (int)BorderType.None);
            bitmap.Data = new byte[filesize];
            byte[] data = bitmap.Data;

            // Add Bitmap header
            Buffer.BlockCopy(_encoding.GetBytes("BM"), 0, data, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(filesize), 0, data, 2, 4);
            uint d = 0;
            Buffer.BlockCopy(BitConverter.GetBytes(d), 0, data, 6, 4);
            d = 0x36;
            Buffer.BlockCopy(BitConverter.GetBytes(d), 0, data, 10, 4);
            Buffer.BlockCopy(andt2, 0, data, 14, andt2sz);
          }
          else if (type == 6)
          {
            // TODO check if 0x5E is right (obtained from anhdsz-0x46+93-andt1sz = 111-70+93-40 = 94)
            string gname = _encoding.GetString(andt2, 0x5E, andt2.Length - 0x5E).TrimEnd('\0');
            glayer.Bitmaps.Add(new Bitmap(gname));
            Bitmap bitmap = glayer.Bitmaps[^1];
            bitmap.ClientRect = r;
            bitmap.Attach = (Attach)attach;
            bitmap.Size = 0;
            bitmap.BorderType = (BorderType)(border >= 0x80 ? border - 0x80 : (int)BorderType.None);
          }
        }
      }
    }

    public void GetCurveProperties(byte[] cvehd, int cvehdsz, byte[] cvedt, int cvedtsz)
    {
      byte h;
      short w;
      (string, string) column;

      if (_ispread != -1) // spreadsheet: curves are columns
      {
        // TODO: check that spreadsheet columns are stored in proper order
        // List<SpreadColumn> header = new List<SpreadColumn>();
        byte c = (byte)cvehd[0x11];
        string name = _encoding.GetString(cvehd, 0x12, 12).TrimEnd('\0');
        ushort width = 0;

        if (cvehdsz > 0x4B)
        {
          width = BitConverter.ToUInt16(cvehd, 0x4A);
        }

        int col_index = FindColumnByName((int)_ispread, name);
        if (col_index != -1)
        {
          SpreadSheets[_ispread].Columns[col_index].Name = name;

          SpreadColumnType type;
          switch (c)
          {
            case 3:
              type = SpreadColumnType.X;
              break;
            case 0:
              type = SpreadColumnType.Y;
              break;
            case 5:
              type = SpreadColumnType.Z;
              break;
            case 6:
              type = SpreadColumnType.XErr;
              break;
            case 2:
              type = SpreadColumnType.YErr;
              break;
            case 4:
              type = SpreadColumnType.Label;
              break;
            default:
              type = SpreadColumnType.NONE;
              break;
          }
          SpreadSheets[_ispread].Columns[col_index].ColumnType = type;

          width /= 0xA;
          if (width == 0)
          {
            width = 8;
          }

          SpreadSheets[_ispread].Columns[col_index].Width = width;
          byte c1 = (byte)cvehd[0x1E];
          byte c2 = (byte)cvehd[0x1F];
          switch (c1)
          {
            case 0x00: // Numeric - Dec1000
            case 0x09: // Text&Numeric - Dec1000
            case 0x10: // Numeric - Scientific
            case 0x19: // Text&Numeric - Scientific
            case 0x20: // Numeric - Engineering
            case 0x29: // Text&Numeric - Engineering
            case 0x30: // Numeric - Dec1,000
            case 0x39: // Text&Numeric - Dec1,000
              SpreadSheets[_ispread].Columns[col_index].ValueType = (c1 % 0x10 == 0x9) ? ValueType.TextNumeric : ValueType.Numeric;
              SpreadSheets[_ispread].Columns[col_index].ValueTypeSpecification = (byte)(c1 / 0x10);
              if (c2 >= 0x80)
              {
                SpreadSheets[_ispread].Columns[col_index].SignificantDigits = c2 - 0x80;
                SpreadSheets[_ispread].Columns[col_index].NumericDisplayType = NumericDisplayType.SignificantDigits;
              }
              else if (c2 > 0)
              {
                SpreadSheets[_ispread].Columns[col_index].DecimalPlaces = c2 - 0x03;
                SpreadSheets[_ispread].Columns[col_index].NumericDisplayType = NumericDisplayType.DecimalPlaces;
              }
              break;
            case 0x02: // Time
              SpreadSheets[_ispread].Columns[col_index].ValueType = ValueType.Time;
              SpreadSheets[_ispread].Columns[col_index].ValueTypeSpecification = (byte)(c2 - 0x80);
              break;
            case 0x03: // Date
            case 0x33:
              SpreadSheets[_ispread].Columns[col_index].ValueType = ValueType.Date;
              SpreadSheets[_ispread].Columns[col_index].ValueTypeSpecification = (byte)(c2 - 0x80);
              break;
            case 0x31: // Text
              SpreadSheets[_ispread].Columns[col_index].ValueType = ValueType.Text;
              break;
            case 0x04: // Month
            case 0x34:
              SpreadSheets[_ispread].Columns[col_index].ValueType = ValueType.Month;
              SpreadSheets[_ispread].Columns[col_index].ValueTypeSpecification = c2;
              break;
            case 0x05: // Day
            case 0x35:
              SpreadSheets[_ispread].Columns[col_index].ValueType = ValueType.Day;
              SpreadSheets[_ispread].Columns[col_index].ValueTypeSpecification = c2;
              break;
            default: // Text
              SpreadSheets[_ispread].Columns[col_index].ValueType = ValueType.Text;
              break;
          }
          if (cvedtsz > 0)
          {
            SpreadSheets[_ispread].Columns[col_index].Comment = _encoding.GetString(cvedt, 0, cvedt.Length).TrimEnd('\0');
          }
          // TODO: check that spreadsheet columns are stored in proper order
          // header.Add(spreadSheets[ispread].columns[col_index]);
        }
        // TODO: check that spreadsheet columns are stored in proper order
        // for (uint i = 0; i < header.Count; i++)
        //     spreadSheets[spread].columns[i] = header[i];
      }
      else if (_imatrix != -1)
      {
        MatrixSheet sheet = Matrixes[_imatrix].Sheets[_ilayer];
        byte c1 = (byte)cvehd[0x1E];
        byte c2 = (byte)cvehd[0x1F];

        sheet.ValueTypeSpecification = c1 / 0x10;
        if (c2 >= 0x80)
        {
          sheet.SignificantDigits = c2 - 0x80;
          sheet.NumericDisplayType = NumericDisplayType.SignificantDigits;
        }
        else if (c2 > 0)
        {
          sheet.DecimalPlaces = c2 - 0x03;
          sheet.NumericDisplayType = NumericDisplayType.DecimalPlaces;
        }

        Matrixes[_imatrix].Sheets[_ilayer] = sheet;
      }
      else if (_iexcel != -1)
      {
        byte c = (byte)cvehd[0x11];
        string name = _encoding.GetString(cvehd, 0x12, 12).TrimEnd('\0');
        short width = BitConverter.ToInt16(cvehd, 0x4A);
        short dataID = BitConverter.ToInt16(cvehd, 0x04);

        if (dataID - 1 < Datasets.Count)
        {
          int isheet = Datasets[dataID - 1].Sheet;
          int col_index = FindExcelColumnByName(_iexcel, isheet, name);
          if (col_index != -1)
          {
            SpreadColumnType type;
            switch (c)
            {
              case 3:
                type = SpreadColumnType.X;
                break;
              case 0:
                type = SpreadColumnType.Y;
                break;
              case 5:
                type = SpreadColumnType.Z;
                break;
              case 6:
                type = SpreadColumnType.XErr;
                break;
              case 2:
                type = SpreadColumnType.YErr;
                break;
              case 4:
                type = SpreadColumnType.Label;
                break;
              default:
                type = SpreadColumnType.NONE;
                break;
            }
            Excels[_iexcel].Sheets[isheet].Columns[col_index].ColumnType = type;
            width /= 0xA;
            if (width == 0)
            {
              width = 8;
            }

            Excels[_iexcel].Sheets[isheet].Columns[col_index].Width = width;

            byte c1 = (byte)cvehd[0x1E];
            byte c2 = (byte)cvehd[0x1F];
            switch (c1)
            {
              case 0x00: // Numeric - Dec1000
              case 0x09: // Text&Numeric - Dec1000
              case 0x10: // Numeric - Scientific
              case 0x19: // Text&Numeric - Scientific
              case 0x20: // Numeric - Engineering
              case 0x29: // Text&Numeric - Engineering
              case 0x30: // Numeric - Dec1,000
              case 0x39: // Text&Numeric - Dec1,000
                Excels[_iexcel].Sheets[isheet].Columns[col_index].ValueType = (c1 % 0x10 == 0x9) ? ValueType.TextNumeric : ValueType.Numeric;
                Excels[_iexcel].Sheets[isheet].Columns[col_index].ValueTypeSpecification = (byte)(c1 / 0x10);
                if (c2 >= 0x80)
                {
                  Excels[_iexcel].Sheets[isheet].Columns[col_index].SignificantDigits = c2 - 0x80;
                  Excels[_iexcel].Sheets[isheet].Columns[col_index].NumericDisplayType = NumericDisplayType.SignificantDigits;
                }
                else if (c2 > 0)
                {
                  Excels[_iexcel].Sheets[isheet].Columns[col_index].DecimalPlaces = c2 - 0x03;
                  Excels[_iexcel].Sheets[isheet].Columns[col_index].NumericDisplayType = NumericDisplayType.DecimalPlaces;
                }
                break;
              case 0x02: // Time
                Excels[_iexcel].Sheets[isheet].Columns[col_index].ValueType = ValueType.Time;
                Excels[_iexcel].Sheets[isheet].Columns[col_index].ValueTypeSpecification = (byte)(c2 - 0x80);
                break;
              case 0x03: // Date
                Excels[_iexcel].Sheets[isheet].Columns[col_index].ValueType = ValueType.Date;
                Excels[_iexcel].Sheets[isheet].Columns[col_index].ValueTypeSpecification = (byte)(c2 - 0x80);
                break;
              case 0x31: // Text
                Excels[_iexcel].Sheets[isheet].Columns[col_index].ValueType = ValueType.Text;
                break;
              case 0x04: // Month
              case 0x34:
                Excels[_iexcel].Sheets[isheet].Columns[col_index].ValueType = ValueType.Month;
                Excels[_iexcel].Sheets[isheet].Columns[col_index].ValueTypeSpecification = c2;
                break;
              case 0x05: // Day
              case 0x35:
                Excels[_iexcel].Sheets[isheet].Columns[col_index].ValueType = ValueType.Day;
                Excels[_iexcel].Sheets[isheet].Columns[col_index].ValueTypeSpecification = c2;
                break;
              default: // Text
                Excels[_iexcel].Sheets[isheet].Columns[col_index].ValueType = ValueType.Text;
                break;
            }
            if (cvedtsz > 0)
            {
              Excels[_iexcel].Sheets[isheet].Columns[col_index].Comment = _encoding.GetString(cvedt, 0, cvedt.Length).TrimEnd('\0');
            }
          }
        }
      }
      else
      {
        GraphLayer glayer = Graphs[_igraph].Layers[_ilayer];
        glayer.Curves.Add(new GraphCurve());
        GraphCurve curve = glayer.Curves[glayer.Curves.Count - 1];

        h = (byte)cvehd[0x26];
        curve.IsHidden = (h == 33);
        if (cvehd.Length > 0x4C) // at least version 4.1 does not have this
        {
          curve.PlotType = (GraphCurve.Plot)cvehd[0x4C];
        }
        if (curve.PlotType == GraphCurve.Plot.XYZContour || curve.PlotType == GraphCurve.Plot.Contour)
        {
          glayer.IsXYY3D = false;
        }

        w = BitConverter.ToInt16(cvehd, 0x04);
        column = FindDataByIndex(w - 1);
        short nColY = w;
        if (column.Item1.Length > 0)
        {
          curve.DataName = column.Item1;
          if (glayer.Is3D() || (curve.PlotType == GraphCurve.Plot.XYZContour))
          {
            curve.ZColumnName = column.Item2;
          }
          else
          {
            curve.YColumnName = column.Item2;
          }
        }

        w = BitConverter.ToInt16(cvehd, 0x23);
        column = FindDataByIndex(w - 1);
        if (column.Item1.Length > 0)
        {
          curve.XDataName = (curve.DataName != column.Item1) ? column.Item1 : string.Empty;
          if (glayer.Is3D() || (curve.PlotType == GraphCurve.Plot.XYZContour))
          {
            curve.YColumnName = column.Item2;
          }
          else if (glayer.IsXYY3D)
          {
            curve.XColumnName = column.Item2;
          }
          else
          {
            curve.XColumnName = column.Item2;
          }
        }

        if (cvehdsz > 0x4E)
        {
          w = BitConverter.ToInt16(cvehd, 0x4D);
          column = FindDataByIndex(w - 1);
          if (column.Item1.Length > 0 && (glayer.Is3D() || (curve.PlotType == GraphCurve.Plot.XYZContour)))
          {
            curve.XColumnName = column.Item2;
            if (curve.DataName != column.Item1)
            {
              // graph X and Y from different tables
            }
          }
        }

        if (glayer.Is3D() || glayer.IsXYY3D)
        {
          Graphs[_igraph].Is3D = true;
        }

        curve.LineConnect = cvehd[0x11];
        curve.LineStyle = cvehd[0x12];
        curve.BoxWidth = cvehd[0x14];

        w = BitConverter.ToInt16(cvehd, 0x15);
        curve.LineWidth = (double)w / 500.0;

        curve.SymbolShape = cvehd[0x17];
        curve.SymbolInterior = cvehd[0x18];

        w = BitConverter.ToInt16(cvehd, 0x19);
        curve.SymbolSize = (double)w / 500.0;

        h = (byte)cvehd[0x1C];
        curve.FillArea = (h == 2);
        curve.FillAreaType = cvehd[0x1E];

        //text
        if (curve.PlotType == GraphCurve.Plot.TextPlot)
        {
          curve.Text ??= new TextProperties();
          curve.Text.Rotation = (short)(BitConverter.ToInt16(cvehd, 0x13) / 10);
          curve.Text.FontSize = BitConverter.ToInt16(cvehd, 0x15);

          h = (byte)cvehd[0x19];
          switch (h)
          {
            case 26:
              curve.Text.Justify = TextProperties.TextJustify.Center;
              break;
            case 2:
              curve.Text.Justify = TextProperties.TextJustify.Right;
              break;
            default:
              curve.Text.Justify = TextProperties.TextJustify.Left;
              break;
          }

          h = (byte)cvehd[0x20];
          curve.Text.FontUnderline = ((h & 0x1) != 0);
          curve.Text.FontItalic = ((h & 0x2) != 0);
          curve.Text.FontBold = ((h & 0x8) != 0);
          curve.Text.WhiteOut = ((h & 0x20) != 0);

          sbyte offset = (sbyte)cvehd[0x37];
          curve.Text.XOffset = (short)(offset * 5);
          offset = (sbyte)cvehd[0x38];
          curve.Text.YOffset = (short)(offset * 5);
        }

        //vector
        if (curve.PlotType == GraphCurve.Plot.FlowVector || curve.PlotType == GraphCurve.Plot.Vector)
        {
          curve.Vector ??= new VectorProperties();
          curve.Vector.Multiplier = BitConverter.ToSingle(cvehd, 0x56);

          h = (byte)cvehd[0x5E];
          column = FindDataByIndex(nColY - 1 + h - 0x64);
          if (column.Item1.Length > 0)
          {
            curve.Vector.EndXColumnName = column.Item2;
          }

          h = (byte)cvehd[0x62];
          column = FindDataByIndex(nColY - 1 + h - 0x64);
          if (column.Item1.Length > 0)
          {
            curve.Vector.EndYColumnName = column.Item2;
          }

          h = (byte)cvehd[0x18];
          if (h >= 0x64)
          {
            column = FindDataByIndex(nColY - 1 + h - 0x64);
            if (column.Item1.Length > 0)
            {
              curve.Vector.AngleColumnName = column.Item2;
            }
          }
          else if (h <= 0x08)
          {
            curve.Vector.ConstAngle = 45 * h;
          }
          // TODO look here if translation is ok
          h = (byte)cvehd[0x19];
          if (h >= 0x64)
          {
            column = FindDataByIndex(nColY - 1 + h - 0x64);
            if (column.Item1.Length > 0)
            {
              curve.Vector.MagnitudeColumnName = column.Item2;
            }
          }
          else
          {
            curve.Vector.ConstMagnitude = (int)curve.SymbolSize;
          }

          curve.Vector.ArrowLength = BitConverter.ToInt16(cvehd, 0x66);
          curve.Vector.ArrowAngle = cvehd[0x68];

          h = cvehd[0x69];
          curve.Vector.ArrowClosed = 0 != (h & 0x1);

          w = BitConverter.ToInt16(cvehd, 0x70);
          curve.Vector.Width = (double)w / 500.0;

          h = cvehd[0x142];
          switch (h)
          {
            case 2:
              curve.Vector.Position = VectorProperties.VectorPosition.Midpoint;
              break;
            case 4:
              curve.Vector.Position = VectorProperties.VectorPosition.Head;
              break;
            default:
              curve.Vector.Position = VectorProperties.VectorPosition.Tail;
              break;
          }
        }
        // pie
        if (curve.PlotType == GraphCurve.Plot.Pie)
        {
          h = cvehd[0x14];
          curve.Pie ??= new PieProperties();
          curve.Pie.FormatPercentages = ((h & 0x08) != 0);
          curve.Pie.FormatValues = !curve.Pie.FormatPercentages;
          curve.Pie.PositionAssociate = ((h & 0x80) != 0);
          curve.Pie.FormatCategories = ((h & 0x20) != 0);

          h = cvehd[0x19];
          curve.Pie.Radius = (short)(100 - h);

          h = cvehd[0x1A];
          curve.Pie.Distance = h;
          curve.Pie.FormatAutomatic = true;
          curve.Pie.ViewAngle = 90;
          curve.Pie.Thickness = 33;
          curve.Pie.Rotation = 0;
          curve.Pie.HorizontalOffset = 0;

          if (cvehdsz > 0xA9)
          {
            h = cvehd[0x92];
            curve.Pie.FormatPercentages = ((h & 0x01) != 0);
            curve.Pie.FormatValues = ((h & 0x02) != 0);
            curve.Pie.PositionAssociate = ((h & 0x08) != 0);
            curve.Pie.ClockwiseRotation = ((h & 0x20) != 0);
            curve.Pie.FormatCategories = ((h & 0x80) != 0);

            curve.Pie.FormatAutomatic = (cvehd[0x93] != 0);

            curve.Pie.Distance = BitConverter.ToInt16(cvehd, 0x94);
            curve.Pie.ViewAngle = cvehd[0x96];
            curve.Pie.Thickness = cvehd[0x98];


            curve.Pie.Rotation = BitConverter.ToInt16(cvehd, 0x9A);
            curve.Pie.Displacement = BitConverter.ToInt16(cvehd, 0x9E);


            curve.Pie.Radius = BitConverter.ToInt16(cvehd, 0xA0);
            curve.Pie.HorizontalOffset = BitConverter.ToInt16(cvehd, 0xA2);

            curve.Pie.DisplacedSectionCount = BitConverter.ToInt32(cvehd, 0xA6);
          }
        }
        // surface
        if (glayer.IsXYY3D || curve.PlotType == GraphCurve.Plot.Mesh3D)
        {
          curve.Surface ??= new SurfaceProperties();
          curve.Surface.Type = cvehd[0x17];
          h = cvehd[0x1C];
          if ((h & 0x60) == 0x60)
          {
            curve.Surface.Grids = SurfaceGrids.X;
          }
          else if (0 != (h & 0x20))
          {
            curve.Surface.Grids = SurfaceGrids.Y;
          }
          else if (0 != (h & 0x40))
          {
            curve.Surface.Grids = SurfaceGrids.None;
          }
          else
          {
            curve.Surface.Grids = SurfaceGrids.XY;
          }

          curve.Surface.SideWallEnabled = ((h & 0x10) != 0);
          curve.Surface.FrontColor = GetColor(_encoding.GetString(cvehd, 0x1D, 4));

          h = cvehd[0x13];
          curve.Surface.BackColorEnabled = ((h & 0x08) != 0);
          curve.Surface.Surface.Fill = ((h & 0x10) != 0);
          curve.Surface.Surface.Contour = ((h & 0x40) != 0);
          curve.Surface.TopContour.Fill = ((h & 0x02) != 0);
          curve.Surface.TopContour.Contour = ((h & 0x04) != 0);
          curve.Surface.BottomContour.Fill = ((h & 0x80) != 0);
          curve.Surface.BottomContour.Contour = ((h & 0x01) != 0);

          if (cvehdsz > 0x165)
          {
            w = BitConverter.ToInt16(cvehd, 0x14C);
            curve.Surface.GridLineWidth = (double)w / 500.0;
            curve.Surface.GridColor = GetColor(_encoding.GetString(cvehd, 0x14E, 4));
            curve.Surface.BackColor = GetColor(_encoding.GetString(cvehd, 0x15A, 4));
            curve.Surface.XSideWallColor = GetColor(_encoding.GetString(cvehd, 0x15E, 4));
            curve.Surface.YSideWallColor = GetColor(_encoding.GetString(cvehd, 0x162, 4));
          }
          if (cvehdsz > 0xA9)
          {
            w = BitConverter.ToInt16(cvehd, 0x94);
            curve.Surface.Surface.LineWidth = (double)w / 500.0;
            curve.Surface.Surface.LineColor = GetColor(_encoding.GetString(cvehd, 0x96, 4));

            w = BitConverter.ToInt16(cvehd, 0xB4);
            curve.Surface.TopContour.LineWidth = (double)w / 500.0;
            curve.Surface.TopContour.LineColor = GetColor(_encoding.GetString(cvehd, 0xB6, 4));

            w = BitConverter.ToInt16(cvehd, 0xA4);
            curve.Surface.BottomContour.LineWidth = (double)w / 500.0;
            curve.Surface.BottomContour.LineColor = GetColor(_encoding.GetString(cvehd, 0xA6, 4));
          }
        }
        if (curve.PlotType == GraphCurve.Plot.Mesh3D || curve.PlotType == GraphCurve.Plot.Contour || curve.PlotType == GraphCurve.Plot.XYZContour)
        {
          if (curve.PlotType == GraphCurve.Plot.Contour || curve.PlotType == GraphCurve.Plot.XYZContour)
          {
            glayer.IsXYY3D = false;
          }

          ColorMap colorMap = (curve.PlotType == GraphCurve.Plot.Mesh3D ? curve.Surface.ColorMap : curve.ColorMap);
          h = cvehd[0x13];
          colorMap.IsFillEnabled = ((h & 0x82) != 0);

          if ((curve.PlotType == GraphCurve.Plot.Contour) && (cvehdsz > 0x89))
          {
            curve.Text ??= new TextProperties();
            curve.Text.FontSize = BitConverter.ToInt16(cvehd, 0x7A);

            h = cvehd[0x83];
            curve.Text.FontUnderline = ((h & 0x1) != 0);
            curve.Text.FontItalic = ((h & 0x2) != 0);
            curve.Text.FontBold = ((h & 0x8) != 0);
            curve.Text.WhiteOut = ((h & 0x20) != 0);

            curve.Text.Color = GetColor(_encoding.GetString(cvehd, 0x86, 4));
          }
          if (cvedtsz > 0x6C)
          {
            GetColorMap(colorMap, cvedt, cvedtsz);
          }
          else
          {
            colorMap = glayer.ColorMap;
          }
        }

        if (FileVersion >= 850)
        {
          curve.LineTransparency = cvehd[0x9C];
          h = cvehd[0x9D];
          curve.FillAreaWithLineTransparency = h != 0;
          curve.FillAreaTransparency = cvehd[0x11E];
        }
        else
        {
          // use sensible default values
          curve.FillAreaWithLineTransparency = false;
          curve.FillAreaTransparency = 255;
        }

        if (cvehdsz > 0x143)
        {
          curve.FillAreaColor = GetColor(_encoding.GetString(cvehd, 0xC2, 4));
          w = BitConverter.ToInt16(cvehd, 0xC6);
          curve.FillAreaPatternWidth = (double)w / 500.0;

          curve.FillAreaPatternColor = GetColor(_encoding.GetString(cvehd, 0xCA, 4));
          curve.FillAreaPattern = cvehd[0xCE];
          curve.FillAreaPatternBorderStyle = cvehd[0xCF];
          w = BitConverter.ToInt16(cvehd, 0xD0);
          curve.FillAreaPatternBorderWidth = (double)w / 500.0;
          curve.FillAreaPatternBorderColor = GetColor(_encoding.GetString(cvehd, 0xD2, 4));

          curve.FillAreaTransparency = cvehd[0x11E];

          curve.LineColor = GetColor(_encoding.GetString(cvehd, 0x16A, 4));

          if (curve.PlotType != GraphCurve.Plot.Contour && curve.Text is not null)
          {
            curve.Text.Color = curve.LineColor;
          }

          curve.SymbolFillColor = GetColor(_encoding.GetString(cvehd, 0x12E, 4));
          curve.SymbolColor = GetColor(_encoding.GetString(cvehd, 0x132, 4));
          if (curve.Vector is not null)
          {
            curve.Vector.Color = curve.SymbolColor;
          }

          h = (byte)cvehd[0x136];
          curve.SymbolThickness = (byte)(h == 255 ? 1 : h);
          curve.PointOffset = cvehd[0x137];
          // h = (byte)cvehd[0x138];
          curve.SymbolFillTransparency = cvehd[0x139];

          h = (byte)cvehd[0x143];
          curve.ConnectSymbols = ((h & 0x8) != 0);
        }
      }
    }

    public void GetAxisBreakProperties(byte[] abdata, int abdatasz)
    {
      int cursor = 0;
      if (_ispread != -1) // spreadsheet
      {
        // Implementation for spreadsheet
      }
      else if (_imatrix != -1) // matrix
      {
        // Implementation for matrix
      }
      else if (_iexcel != -1) // excel
      {
        // Implementation for excel
      }
      else // graph
      {
        GraphLayer glayer = Graphs[_igraph].Layers[_ilayer];
        byte h = (byte)abdata[0x02];
        if (h == 2)
        {
          glayer.XAxisBreak.MinorTicksBefore = glayer.XAxis.MinorTicks;
          glayer.XAxisBreak.ScaleIncrementBefore = glayer.XAxis.Step;
          glayer.XAxisBreak.Show = true;
          cursor = 0x0B; // Skip to the relevant part of the string
          glayer.XAxisBreak.From = BitConverter.ToDouble(abdata, cursor); cursor += sizeof(double);
          glayer.XAxisBreak.To = BitConverter.ToDouble(abdata, cursor); cursor += sizeof(double);
          glayer.XAxisBreak.ScaleIncrementAfter = BitConverter.ToDouble(abdata, cursor); cursor += sizeof(double);
          glayer.XAxisBreak.Position = BitConverter.ToDouble(abdata, cursor); cursor += sizeof(double);
          h = (byte)abdata[0x2B];
          glayer.XAxisBreak.Log10 = (h == 1);
          glayer.XAxisBreak.MinorTicksAfter = (byte)abdata[0x2C];
        }
        else if (h == 4)
        {
          glayer.YAxisBreak.MinorTicksBefore = glayer.YAxis.MinorTicks;
          glayer.YAxisBreak.ScaleIncrementBefore = glayer.YAxis.Step;
          glayer.YAxisBreak.Show = true;
          cursor = 0x0B; // Skip to the relevant part of the string
          glayer.YAxisBreak.From = BitConverter.ToDouble(abdata, cursor); cursor += sizeof(double);
          glayer.YAxisBreak.To = BitConverter.ToDouble(abdata, cursor); cursor += sizeof(double);
          glayer.YAxisBreak.ScaleIncrementAfter = BitConverter.ToDouble(abdata, cursor); cursor += sizeof(double);
          glayer.YAxisBreak.Position = BitConverter.ToDouble(abdata, cursor); cursor += sizeof(double);
          h = (byte)abdata[0x2B];
          glayer.YAxisBreak.Log10 = (h == 1);
          glayer.YAxisBreak.MinorTicksAfter = (byte)abdata[0x2C];
        }
      }
    }

    public void GetAxisParameterProperties(byte[] apdata, int apdatasz, int naxis)
    {
      if (_igraph != -1)
      {
        byte h = 0;
        short w = 0;

        GraphLayer glayer = Graphs[_igraph].Layers[_ilayer];
        GraphAxis axis = glayer.XAxis;

        if (naxis == 1)
        {
          axis = glayer.XAxis;
        }
        else if (naxis == 2)
        {
          axis = glayer.YAxis;
        }
        else if (naxis == 3)
        {
          axis = glayer.ZAxis;
        }

        if (_iaxispar == 0) // minor Grid
        {
          h = (byte)apdata[0x26];
          axis.MinorGrid.Hidden = (h == 0);
          axis.MinorGrid.Color = apdata[0x0F];
          axis.MinorGrid.Style = apdata[0x12];
          w = BitConverter.ToInt16(apdata, 0x15);
          axis.MinorGrid.Width = (double)w / 500.0;
        }
        else if (_iaxispar == 1) // major Grid
        {
          h = (byte)apdata[0x26];
          axis.MajorGrid.Hidden = (h == 0);
          axis.MajorGrid.Color = apdata[0x0F];
          axis.MajorGrid.Style = apdata[0x12];
          w = BitConverter.ToInt16(apdata, 0x15);
          axis.MajorGrid.Width = (double)w / 500.0;
        }
        else if (_iaxispar == 2) // tickaxis 0
        {
          h = (byte)apdata[0x26];
          axis.TickAxis[0].ShowMajorLabels = ((h & 0x40) != 0);
          axis.TickAxis[0].Color = apdata[0x0F];
          w = BitConverter.ToInt16(apdata, 0x13);
          axis.TickAxis[0].Rotation = w / 10;
          w = BitConverter.ToInt16(apdata, 0x15);
          axis.TickAxis[0].FontSize = w;
          h = (byte)apdata[0x1A];
          axis.TickAxis[0].FontBold = ((h & 0x08) != 0);
          w = BitConverter.ToInt16(apdata, 0x23);
          h = (byte)apdata[0x25];
          byte h1 = (byte)apdata[0x26];
          axis.TickAxis[0].ValueType = (ValueType)(h & 0x0F);
          (string, string) column;

          switch (axis.TickAxis[0].ValueType)
          {
            case ValueType.Numeric:
              /*switch ((h>>4)) {
          case 0x9:
            axis.tickAxis[0].valueTypeSpecification=1;
            break;
          case 0xA:
            axis.tickAxis[0].valueTypeSpecification=2;
            break;
          case 0xB:
            axis.tickAxis[0].valueTypeSpecification=3;
            break;
          default:
            axis.tickAxis[0].valueTypeSpecification=0;
        }*/
              if ((h >> 4) > 7)
              {
                axis.TickAxis[0].ValueTypeSpecification = (h >> 4) - 8;
                axis.TickAxis[0].DecimalPlaces = h1 - 0x40;
              }
              else
              {
                axis.TickAxis[0].ValueTypeSpecification = (h >> 4);
                axis.TickAxis[0].DecimalPlaces = -1;
              }
              break;
            case ValueType.Time:
            case ValueType.Date:
            case ValueType.Month:
            case ValueType.Day:
            case ValueType.ColumnHeading:
              axis.TickAxis[0].ValueTypeSpecification = h1 - 0x40;
              break;
            case ValueType.Text:
            case ValueType.TickIndexedDataset:
            case ValueType.Categorical:
              column = FindDataByIndex(w - 1);
              if (column.Item1.Length > 0)
              {
                axis.TickAxis[0].DataName = column.Item1;
                axis.TickAxis[0].ColumnName = column.Item2;
              }
              break;
            case ValueType.TextNumeric: // Numeric Decimal 1.000
              axis.TickAxis[0].ValueType = ValueType.Numeric;
              axis.TickAxis[0].ValueTypeSpecification = 0;
              break;
          }
        }
        else if (_iaxispar == 3) // formataxis 0
        {
          h = (byte)apdata[0x26];
          axis.FormatAxis[0].IsHidden = (h == 0);
          axis.FormatAxis[0].Color = apdata[0x0F];
          if (apdatasz > 0x4B)
          {
            w = BitConverter.ToInt16(apdata, 0x4A);
            axis.FormatAxis[0].MajorTickLength = (double)w / 10.0;
          }
          w = BitConverter.ToInt16(apdata, 0x15);
          axis.FormatAxis[0].Thickness = (double)w / 500.0;
          h = (byte)apdata[0x25];
          axis.FormatAxis[0].MinorTicksType = (h >> 6);
          axis.FormatAxis[0].MajorTicksType = ((h >> 4) & 3);
          axis.FormatAxis[0].AxisPosition = (h & 0x0F);
          short w1 = 0;

          switch (axis.FormatAxis[0].AxisPosition)
          {
            // TODO: check if correct
            case 1:
              w1 = BitConverter.ToInt16(apdata, 0x37);
              axis.FormatAxis[0].AxisPositionValue = (double)w1;
              break;
            case 2:
              axis.FormatAxis[0].AxisPositionValue = BitConverter.ToDouble(apdata, 0x2F);
              break;
          }
        }
        else if (_iaxispar == 4) // tickaxis 1
        {
          h = (byte)apdata[0x26];
          axis.TickAxis[1].ShowMajorLabels = ((h & 0x40) != 0);
          axis.TickAxis[1].Color = apdata[0x0F];
          w = BitConverter.ToInt16(apdata, 0x13);
          axis.TickAxis[1].Rotation = w / 10;
          w = BitConverter.ToInt16(apdata, 0x15);
          axis.TickAxis[1].FontSize = w;
          h = (byte)apdata[0x1A];
          axis.TickAxis[1].FontBold = ((h & 0x08) != 0);
          w = BitConverter.ToInt16(apdata, 0x23);
          h = (byte)apdata[0x25];
          byte h1 = (byte)apdata[0x26];
          axis.TickAxis[1].ValueType = (ValueType)(h & 0x0F);
          (string, string) column;

          switch (axis.TickAxis[1].ValueType)
          {
            case ValueType.Numeric:
              if ((h >> 4) > 7)
              {
                axis.TickAxis[1].ValueTypeSpecification = (h >> 4) - 8;
                axis.TickAxis[1].DecimalPlaces = h1 - 0x40;
              }
              else
              {
                axis.TickAxis[1].ValueTypeSpecification = (h >> 4);
                axis.TickAxis[1].DecimalPlaces = -1;
              }
              break;
            case ValueType.Time:
            case ValueType.Date:
            case ValueType.Month:
            case ValueType.Day:
            case ValueType.ColumnHeading:
              axis.TickAxis[1].ValueTypeSpecification = h1 - 0x40;
              break;
            case ValueType.Text:
            case ValueType.TickIndexedDataset:
            case ValueType.Categorical:
              column = FindDataByIndex(w - 1);
              if (column.Item1.Length > 0)
              {
                axis.TickAxis[1].DataName = column.Item1;
                axis.TickAxis[1].ColumnName = column.Item2;
              }
              break;
            case ValueType.TextNumeric: // Numeric Decimal 1.000
              axis.TickAxis[1].ValueType = ValueType.Numeric;
              axis.TickAxis[1].ValueTypeSpecification = 0;
              break;
          }
        }
        else if (_iaxispar == 5) // formataxis 1
        {
          h = (byte)apdata[0x26];
          axis.FormatAxis[1].IsHidden = (h == 0);
          axis.FormatAxis[1].Color = apdata[0x0F];
          if (apdatasz > 0x4B)
          {
            w = BitConverter.ToInt16(apdata, 0x4A);
            axis.FormatAxis[1].MajorTickLength = (double)w / 10.0;
          }
          w = BitConverter.ToInt16(apdata, 0x15);
          axis.FormatAxis[1].Thickness = (double)w / 500.0;
          h = (byte)apdata[0x25];
          axis.FormatAxis[1].MinorTicksType = (h >> 6);
          axis.FormatAxis[1].MajorTicksType = ((h >> 4) & 3);
          axis.FormatAxis[1].AxisPosition = (h & 0x0F);
          short w1 = 0;

          switch (axis.FormatAxis[1].AxisPosition)
          {
            case 1:
              w1 = BitConverter.ToInt16(apdata, 0x37);
              axis.FormatAxis[1].AxisPositionValue = (double)w1;
              break;
            case 2:
              axis.FormatAxis[1].AxisPositionValue = BitConverter.ToDouble(apdata, 0x2F);
              break;
          }
        }

        if (naxis == 1)
        {
          glayer.XAxis = axis;
        }
        else if (naxis == 2)
        {
          glayer.YAxis = axis;
        }
        else if (naxis == 3)
        {
          glayer.ZAxis = axis;
        }

        _iaxispar++;
        _iaxispar %= 6;
      }
    }

    public void GetNoteProperties(byte[] nwehd, int nwehdsz, byte[] nwelb, int nwelbsz, byte[] nwect, int nwectsz)
    {
      int cursor = 0;
      LogPrint("OriginAnyParser::GetNoteProperties()");



      // note window position and size
      Rect rect = new Rect();
      uint coord;

      coord = BitConverter.ToUInt32(nwehd, cursor); cursor += sizeof(Int32);
      rect.Left = (short)coord;
      coord = BitConverter.ToUInt32(nwehd, cursor); cursor += sizeof(Int32);
      rect.Top = (short)coord;
      coord = BitConverter.ToUInt32(nwehd, cursor); cursor += sizeof(Int32);
      rect.Right = (short)coord;
      coord = BitConverter.ToUInt32(nwehd, cursor); cursor += sizeof(Int32);
      rect.Bottom = (short)coord;

      string name = _encoding.GetString(nwelb, 0, nwelb.Length).TrimEnd('\0');

      // ResultsLog note window has left, top, right, bottom all zero.
      // All other parameters are also zero, except "name" and "text".
      if (rect.Bottom == 0 || rect.Right == 0)
      {
        ResultsLog = _encoding.GetString(nwect, 0, nwect.Length);
        return;
      }
      byte state = (byte)nwehd[0x18];

      // files from version < 6.0 have nwehdsz < 1D
      if (nwehdsz < 0x2F)
      {
        return;
      }

      double creationDate, modificationDate;
      creationDate = BitConverter.ToDouble(nwehd, 0x20);
      modificationDate = BitConverter.ToDouble(nwehd, 0x20 + sizeof(double));

      if (nwehdsz < 0x38)
      {
        return;
      }

      byte c = (byte)nwehd[0x38];

      if (nwehdsz < 0x3F)
      {
        return;
      }

      int labellen = 0;
      labellen = BitConverter.ToInt32(nwehd, 0x3C);

      Notes.Add(new Note(name));
      LogPrint($"notes: {Notes.Count}\n");
      Notes[^1].ObjectID = _objectIndex;
      _objectIndex++;

      Notes[^1].FrameRect = rect;
      Notes[^1].CreationDate = DoubleToPosixTime(creationDate);
      Notes[^1].ModificationDate = DoubleToPosixTime(modificationDate);

      if (c == 0x01)
      {
        Notes[^1].Title = WindowTitle.Label;
      }
      else if (c == 0x02)
      {
        Notes[^1].Title = WindowTitle.Name;
      }
      else
      {
        Notes[^1].Title = WindowTitle.Both;
      }

      if (state == 0x07)
      {
        Notes[^1].State = WindowState.Minimized;
      }
      else if (state == 0x0b)
      {
        Notes[^1].State = WindowState.Maximized;
      }

      Notes[^1].IsHidden = ((state & 0x40) != 0);

      if (labellen > 1)
      {
        Notes[^1].Label = _encoding.GetString(nwect, 0, (int)labellen).TrimEnd('\0');
        Notes[^1].Text = _encoding.GetString(nwect, (int)labellen, nwect.Length - labellen).TrimEnd('\0');
      }
      else
      {
        Notes[^1].Text = _encoding.GetString(nwect, 0, nwect.Length).TrimEnd('\0');
      }
    }

    public void GetColorMap(ColorMap cmap, byte[] cmapdata, int cmapdatasz)
    {
      int cmoffset = 0;
      // color maps for matrix annotations have a different offset than graph curve's colormaps
      if (_imatrix != -1)
      {
        cmoffset = 0x14;
      }
      else if (_igraph != -1)
      {
        cmoffset = 0x6C;
      }
      else
      {
        return;
      }


      var colorMapSize = BitConverter.ToInt32(cmapdata, cmoffset);

      // check we have enough data to fill the map
      var minDataSize = cmoffset + 0x114 + (colorMapSize + 2) * 0x38;
      if (minDataSize > cmapdatasz)
      {
        LogPrint("WARNING: Too few data while getting ColorMap. Needed: at least {0} bytes. Have: {1} bytes.", minDataSize, cmapdatasz);
        return;
      }

      var lvl_offset = 0;
      for (int i = 0; i < colorMapSize + 3; ++i)
      {
        lvl_offset = cmoffset + 0x114 + i * 0x38;
        ColorMapLevel level = new ColorMapLevel();

        level.FillPattern = cmapdata[lvl_offset];
        level.FillPatternColor = GetColor(_encoding.GetString(cmapdata, lvl_offset + 0x04, 4));

        var w = BitConverter.ToInt16(cmapdata, lvl_offset + 0x08);
        level.FillPatternLineWidth = (double)w / 500.0;

        level.LineStyle = cmapdata[(int)(lvl_offset + 0x10)];

        w = BitConverter.ToInt16(cmapdata, lvl_offset + 0x12);
        level.LineWidth = (double)w / 500.0;

        level.LineColor = GetColor(_encoding.GetString(cmapdata, lvl_offset + 0x14, 4));

        byte h = cmapdata[(int)(lvl_offset + 0x1A)];
        level.IsLabelVisible = (h & 0x1) != 0;
        level.IsLineVisible = (h & 0x2) == 0;

        level.FillColor = GetColor(_encoding.GetString(cmapdata, lvl_offset + 0x28, 4));

        double value = BitConverter.ToDouble(cmapdata, lvl_offset + 0x30);
        cmap.Levels.Add((value, level));
      }
    }

    public void GetZColorsMap(ColorMap colorMap, byte[] cmapdata, int cmapdatasz)
    {
      Color lowColor = new Color(); // color below
      lowColor.ColorType = ColorType.Custom;
      lowColor.Custom0 = cmapdata[0x0E];
      lowColor.Custom1 = cmapdata[0x0F];
      lowColor.Custom2 = cmapdata[0x10];
      // skip an unsigned char at 0x11

      Color highColor = new Color(); // color above
      highColor.ColorType = ColorType.Custom;
      highColor.Custom0 = cmapdata[0x12];
      highColor.Custom1 = cmapdata[0x13];
      highColor.Custom2 = cmapdata[0x14];
      // skip an unsigned char at 0x15

      short colorMapSize;
      colorMapSize = BitConverter.ToInt16(cmapdata, 0x16);

      // skip a short at 0x18-0x19

      for (int i = 0; i < 4; ++i) // low, high, middle and missing data colors
      {
        Color color = new Color();
        color.ColorType = ColorType.Custom;
        color.Custom0 = cmapdata[0x1A + 4 * i];
        color.Custom1 = cmapdata[0x1B + 4 * i];
        color.Custom2 = cmapdata[0x1C + 4 * i];
      }

      double zmin, zmax, zmissing;
      zmin = BitConverter.ToDouble(cmapdata, 0x2A);
      zmax = BitConverter.ToDouble(cmapdata, 0x2A + sizeof(double));
      zmissing = BitConverter.ToDouble(cmapdata, 0x2A + 2 * sizeof(double));

      short val;
      for (int i = 0; i < 2; ++i)
      {
        Color color = new Color();
        color.ColorType = ColorType.Custom;
        color.Custom0 = cmapdata[0x66 + 10 * i];
        color.Custom1 = cmapdata[0x67 + 10 * i];
        color.Custom2 = cmapdata[0x68 + 10 * i];
        // skip an unsigned char at 0x69+10*i
        val = BitConverter.ToInt16(cmapdata, 0x6A + 10 * i);
      }

      ColorMapLevel level = new ColorMapLevel() { FillColor = lowColor, FillPattern = 0, FillPatternColor = lowColor, FillPatternLineWidth = 1, IsLineVisible = true, LineColor = lowColor, LineStyle = 0, LineWidth = 1, IsLabelVisible = true };
      level.FillColor = lowColor;
      colorMap.Levels.Add((zmin, level));

      for (int i = 0; i < (colorMapSize + 1); ++i)
      {
        Color color = new Color();
        color.ColorType = ColorType.Custom;
        color.Custom0 = cmapdata[0x7A + 10 * i];
        color.Custom1 = cmapdata[0x7B + 10 * i];
        color.Custom2 = cmapdata[0x7C + 10 * i];
        // skip an unsigned char at 0x7D+10*i
        val = BitConverter.ToInt16(cmapdata, 0x7E + 10 * i);

        level.FillColor = color;
        colorMap.Levels.Add((val, level));
      }

      level.FillColor = highColor;
      colorMap.Levels.Add((zmax, level));

    }

    public void GetProjectLeafProperties(ProjectNode currentFolder, byte[] ptldt, int ptldtsz)
    {

      LogPrint("OriginAnyParser::GetProjectLeafProperties()\n");
      // ptldtsz is not used

      int fileType = 0, fileObjectId = 0;
      fileType = BitConverter.ToInt32(ptldt, 0);
      fileObjectId = BitConverter.ToInt32(ptldt, sizeof(int));

      LogPrint($"file_type={fileType} file_object_id={fileObjectId}\n");
      if (fileType == 0x100000) // Note window
      {
        LogPrint($"notes.size()={Notes.Count}\n");
        if (fileObjectId < Notes.Count && Notes.Count > 0)
        {
          currentFolder.AppendChild(new ProjectNode(Notes[(int)fileObjectId].Name, ProjectNodeType.Note));
        }
      }
      else // other windows
      {
        ProjectNode childNode;
        var obj = FindWindowObjectByIndex(fileObjectId);
        childNode = currentFolder.AppendChild(
          new ProjectNode(obj.Item2.Name, obj.Item1)
          {
            CreationDate = obj.Item2.CreationDate,
            ModificationDate = obj.Item2.ModificationDate,
          }
          );
      }
    }

    public void GetProjectFolderProperties(ProjectNode currentFolder, byte[] flehd, int flehdsz)
    {

      // flehdsz is not used, but we keep it for compatibility
      byte a = (byte)flehd[0x02];
      currentFolder.IsActive = (a == 1);

      double creationDate, modificationDate;
      creationDate = BitConverter.ToDouble(flehd, 0x10);
      modificationDate = BitConverter.ToDouble(flehd, 0x10 + sizeof(double));

      currentFolder.CreationDate = DoubleToPosixTime(creationDate);
      currentFolder.ModificationDate = DoubleToPosixTime(modificationDate);
    }

    public void AssignObjectsToProjectTree(ProjectNode current)
    {
      int index;
      switch (current.NodeType)
      {
        case ProjectNodeType.Folder:
          break;
        case ProjectNodeType.Note:
          index = FindNoteByName(current.Name);
          if (index >= 0)
            current.ValueAsObject = Notes[index];
          break;
        case ProjectNodeType.SpreadSheet:
          index = FindSpreadByName(current.Name);
          if (index >= 0)
            current.ValueAsObject = SpreadSheets[index];
          break;
        case ProjectNodeType.Graph3D:
        case ProjectNodeType.Graph:
          index = FindGraphByName(current.Name);
          if (index >= 0)
            current.ValueAsObject = Graphs[index];
          break;
        case ProjectNodeType.Matrix:
          index = FindMatrixByName(current.Name);
          if (index >= 0)
            current.ValueAsObject = Matrixes[index];
          break;
        case ProjectNodeType.Excel:
          index = FindExcelByName(current.Name);
          if (index >= 0)
            current.ValueAsObject = Excels[index];
          break;
        default:
          throw new NotImplementedException();
      }

      // now do the same thing for all subnodes
      foreach (var subNode in current.ChildNodes)
      {
        AssignObjectsToProjectTree(subNode);
      }
    }

    protected void OutputProjectTree(StreamWriter writer)
    {
      throw new NotImplementedException();
    }

    public static DateTime DoubleToPosixTime(double jdt)
    {
      if (jdt == 0)
        return new DateTime(1970, 1, 1);

      // 2440587.5 is julian date for the unixtime epoch
      return new DateTime(1970, 1, 1).AddDays(jdt - 2440587).AddSeconds(0.5);
    }

    #region From original file "originparser.cpp"

    public int FindSpreadByName(string name)
    {
      for (int i = 0; i < SpreadSheets.Count; i++)
      {
        if (string.Equals(SpreadSheets[i].Name, name, StringComparison.OrdinalIgnoreCase))
        {
          return i;
        }
      }
      return -1;
    }

    public int FindMatrixByName(string name)
    {
      for (int i = 0; i < Matrixes.Count; i++)
      {
        if (string.Equals(Matrixes[i].Name, name, StringComparison.CurrentCultureIgnoreCase))
        {
          return i;
        }
      }
      return -1;
    }

    public int FindFunctionByName(string name)
    {
      for (int i = 0; i < Functions.Count; i++)
      {
        if (string.Equals(Functions[i].Name, name, StringComparison.CurrentCultureIgnoreCase))
        {
          return i;
        }
      }
      return -1;
    }

    public int FindExcelByName(string name)
    {
      for (int i = 0; i < Excels.Count; i++)
      {
        if (string.Equals(Excels[i].Name, name, StringComparison.CurrentCultureIgnoreCase))
        {
          return i;
        }
      }
      return -1;
    }

    public int FindNoteByName(string name)
    {
      for (int i = 0; i < Notes.Count; i++)
      {
        if (string.Equals(Notes[i].Name, name, StringComparison.OrdinalIgnoreCase))
        {
          return i;
        }
      }
      return -1;
    }

    public int FindGraphByName(string name)
    {
      for (int i = 0; i < Graphs.Count; i++)
      {
        if (string.Equals(Graphs[i].Name, name, StringComparison.OrdinalIgnoreCase))
        {
          return i;
        }
      }
      return -1;
    }

    protected int FindSpreadColumnByName(int spread, string name)
    {
      for (int i = 0; i < SpreadSheets[spread].Columns.Count; i++)
      {
        if (SpreadSheets[spread].Columns[i].Name == name)
        {
          return i;
        }
      }
      return -1;
    }

    protected int FindExcelColumnByName(int idxExcel, int idxSheet, string name)
    {
      if (idxExcel < Excels.Count)
      {
        var excel = Excels[idxExcel];
        if (idxSheet < excel.Sheets.Count)
        {
          var sheet = excel.Sheets[idxSheet];
          for (int i = 0; i < sheet.Columns.Count; i++)
          {
            if (sheet.Columns[i].Name == name)
            {
              return i;
            }
          }
        }
      }
      return -1;
    }

    protected (string, string) FindDataByIndex(int index)
    {
      foreach (var sheet in SpreadSheets)
      {
        foreach (var column in sheet.Columns)
        {
          if (column.Index == index)
          {
            return ($"T_{sheet.Name}", column.Name);
          }
        }
      }

      foreach (var matrix in Matrixes)
      {
        foreach (var sheet in matrix.Sheets)
        {
          if (sheet.Index == index)
          {
            return ($"M_{matrix.Name}", sheet.Name);
          }
        }
      }

      foreach (var excel in Excels)
      {
        foreach (var sheet in excel.Sheets)
        {
          foreach (var column in sheet.Columns)
          {
            if (column.Index == index)
            {
              int sheetno = excel.Sheets.IndexOf(sheet) + 1;
              string sheetsuffix = $"@{sheetno}";
              if (sheetno > 1)
              {
                return ($"E_{excel.Name}{sheetsuffix}", column.Name);
              }
              else
              {
                return ($"E_{excel.Name}", column.Name);
              }
            }
          }
        }
      }

      foreach (var function in Functions)
      {
        if (function.Index == index)
        {
          return ($"F_{function.Name}", function.Name);
        }
      }

      return (string.Empty, string.Empty);
    }

    protected (ProjectNodeType, string) FindObjectByIndex(uint index)
    {
      for (int i = 0; i < SpreadSheets.Count; i++)
      {
        if (SpreadSheets[i].ObjectID == (int)index)
        {
          return (ProjectNodeType.SpreadSheet, SpreadSheets[i].Name);
        }
      }

      for (int i = 0; i < Matrixes.Count; i++)
      {
        if (Matrixes[i].ObjectID == (int)index)
        {
          return (ProjectNodeType.Matrix, Matrixes[i].Name);
        }
      }

      for (int i = 0; i < Excels.Count; i++)
      {
        if (Excels[i].ObjectID == (int)index)
        {
          return (ProjectNodeType.Excel, Excels[i].Name);
        }
      }

      for (int i = 0; i < Graphs.Count; i++)
      {
        if (Graphs[i].ObjectID == (int)index)
        {
          if (Graphs[i].Is3D)
          {
            return (ProjectNodeType.Graph3D, Graphs[i].Name);
          }
          else
          {
            return (ProjectNodeType.Graph, Graphs[i].Name);
          }
        }
      }

      return ((ProjectNodeType)0, null);
    }

    public (ProjectNodeType, Origin.Window) FindWindowObjectByIndex(int index)
    {
      for (int i = 0; i < SpreadSheets.Count; i++)
      {
        if (SpreadSheets[i].ObjectID == (int)index)
        {
          return (ProjectNodeType.SpreadSheet, (Origin.Window)SpreadSheets[i]);
        }
      }

      for (int i = 0; i < Matrixes.Count; i++)
      {
        if (Matrixes[i].ObjectID == (int)index)
        {
          return (ProjectNodeType.Matrix, (Origin.Window)Matrixes[i]);
        }
      }

      for (int i = 0; i < Excels.Count; i++)
      {
        if (Excels[i].ObjectID == (int)index)
        {
          return (ProjectNodeType.Excel, (Origin.Window)Excels[i]);
        }
      }

      for (int i = 0; i < Graphs.Count; i++)
      {
        if (Graphs[i].ObjectID == (int)index)
        {
          if (Graphs[i].Is3D)
          {
            return (ProjectNodeType.Graph3D, (Origin.Window)Graphs[i]);
          }
          else
          {
            return (ProjectNodeType.Graph, (Origin.Window)Graphs[i]);
          }
        }
      }

      return ((ProjectNodeType)0, null);
    }

    public void ConvertSpreadToExcel(int spread)
    {
      //add new Excel sheet
      Excels.Add(new Excel(SpreadSheets[spread].Name, SpreadSheets[spread].Label, (int)SpreadSheets[spread].MaxRows, SpreadSheets[spread].IsHidden, SpreadSheets[spread].Loose));

      foreach (SpreadColumn column in SpreadSheets[spread].Columns)
      {
        int index = 0;
        int pos = column.Name.LastIndexOf("@");
        if (pos != -1)
        {
          index = int.Parse(column.Name.Substring(pos + 1)) - 1;
          column.Name = column.Name.Substring(0, pos);
        }

        while (Excels[Excels.Count - 1].Sheets.Count <= index)
        {
          Excels[Excels.Count - 1].Sheets.Add(new SpreadSheet());
        }

        Excels[Excels.Count - 1].Sheets[index].Columns.Add(column);
      }

      SpreadSheets.RemoveAt(spread);
    }

    public int FindColumnByName(int spread, string name)
    {
      int columns = SpreadSheets[spread].Columns.Count;
      for (int i = 0; i < columns; i++)
      {
        string colName = SpreadSheets[spread].Columns[i].Name;
        if (colName.Length >= 11)
        {
          colName = colName.Substring(0, 11);
        }

        if (name == colName)
        {
          return i;
        }
      }
      return -1;
    }

    /// <summary>
    /// Reads a string line, and stops if the char is below 0x20.
    /// Afterwards, the stream cursor is placed on the character that has caused the stop.
    /// </summary>
    /// <param name="fs">The file stream.</param>
    /// <param name="doUnreadDelimiter">If true, the file stream is placed on the character that has cause the stop. If false, the file stream is placed
    /// after the delimiting char.</param>
    /// <returns>The line (without any newline, zeros, and other chars below 0x20.</returns>
    public static string ReadLine(Stream fs, bool doUnreadDelimiter)
    {
      var stb = new StringBuilder();
      for (; ; )
      {
        var b = fs.ReadByte();
        if (b < 0x20)
        {
          if (b < 0)
          {
            throw new System.IO.EndOfStreamException();
          }
          if (b == 0 && !doUnreadDelimiter)
          {
            b = fs.ReadByte();
            if (b != '\n')
            {
              if (b < 0)
                throw new System.IO.EndOfStreamException();
              else
                throw new InvalidDataException("After a \\0 string delimiter a 0x0A (\\n) char is expected");
            }
          }

          if (doUnreadDelimiter)
          {
            fs.Seek(-1, SeekOrigin.Current);
          }
          break;
        }
        stb.Append((char)b);
      }
      return stb.ToString();
    }

    #endregion


    [Conditional("GENERATE_CODE_FOR_LOG")]
    private void LogPrint(string message)
    {
      Console.Write(message);
      if (_logfile is not null)
        _logfile.Write(message);
    }

    [Conditional("GENERATE_CODE_FOR_LOG")]
    private void LogPrint(string message, params object[] args)
    {
      LogPrint(string.Format(message, args));
    }


  }

}
