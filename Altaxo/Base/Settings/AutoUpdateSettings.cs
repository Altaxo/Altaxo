#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Settings
{
  using Altaxo.Main.Properties;

  /// <summary>
  /// Maintains the settings for the Altaxo auto update feature.
  /// </summary>
  public class AutoUpdateSettings : Main.ICopyFrom
  {
    public static readonly PropertyKey<AutoUpdateSettings> PropertyKeyAutoUpdate = new PropertyKey<AutoUpdateSettings>("0E8C6ED2-32AF-4CE7-9FF6-1DE298DB0D2D", "AutoUpdate", PropertyLevel.Application);

    /// <summary>Gets or sets a value indicating whether to globally enable auto updates or not.</summary>
    /// <value>If <see langword="true"/>, auto updates are enabled. If <see langword="false"/>, auto updates are disabled.</value>
    public bool EnableAutoUpdates { get; set; }

    /// <summary>Gets or sets a value indicating whether to download only stable versions or to download stable and unstable versions.</summary>
    /// <value>If <see langword="false"/>, only stable versions will be downloaded and installed. If <see langword="true"/>, both stable and unstable versions will be downloaded  installed.</value>
    public bool DownloadUnstableVersion { get; set; }

    /// <summary>Gets or sets the download interval in days.</summary>
    /// <value>Time interval in days, after which Altaxo looks, whether a new version is available.</value>
    public int DownloadIntervalInDays { get; set; }

    /// <summary>Gets or sets a value indicating whether to show the download window.</summary>
    /// <value>If	<see langword="true"/>, the download window is visible (e.g. for debugging purposes). Otherwise, it is hidden.</value>
    public bool ShowDownloadWindow { get; set; }

    /// <summary>Gets or sets a value indicating whether to install a new version of Altaxo at startup of Altaxo.</summary>
    /// <value>If <see langword="true"/> and a new version is available, Altaxo asks at startup whether to install the download.</value>
    public bool InstallAtStartup { get; set; }

    /// <summary>Gets or sets a value indicating whether to install a new version of Altaxo at shutdown of Altaxo.</summary>
    /// <value>If <see langword="true"/> and a new version is available, Altaxo asks at shutdown whether to install the download.</value>
    public bool InstallAtShutdown { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user has to confirm the installation of a new version
    /// </summary>
    /// <value>
    /// If <c>true</c>, the user is asked for confirmation before installing a new version.
    /// </value>
    public bool ConfirmInstallation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show installation window. If <c>false</c>, the installation window will only be shown up if an error occurs.
    /// </summary>
    /// <value>
    /// <c>true</c> if the installation window is shown; otherwise, <c>false</c>.
    /// </value>
    public bool ShowInstallationWindow { get; set; }

    public int InstallationWindowClosingTime { get; set; }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AutoUpdateSettings), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AutoUpdateSettings)obj;
        info.AddValue("EnableAutoUpdates", s.EnableAutoUpdates);
        info.AddValue("DownloadUnstableVersion", s.DownloadUnstableVersion);
        info.AddValue("DownloadIntervalInDays", s.DownloadIntervalInDays);
        info.AddValue("ShowDownloadWindow", s.ShowDownloadWindow);

        info.AddValue("InstallAtStartup", s.InstallAtStartup);
        info.AddValue("InstallAtShutDown", s.InstallAtShutdown);
        info.AddValue("ConfirmInstallation", s.ConfirmInstallation);
        info.AddValue("ShowInstallationWindow", s.ShowInstallationWindow);
        info.AddValue("InstallationWindowClosingTime", s.InstallationWindowClosingTime);
      }

      protected virtual AutoUpdateSettings SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AutoUpdateSettings?)o ?? new AutoUpdateSettings();

        s.EnableAutoUpdates = info.GetBoolean("EnableAutoUpdates");
        s.DownloadUnstableVersion = info.GetBoolean("DownloadUnstableVersion");
        s.DownloadIntervalInDays = info.GetInt32("DownloadIntervalInDays");
        s.ShowDownloadWindow = info.GetBoolean("ShowDownloadWindow");

        s.InstallAtStartup = info.GetBoolean("InstallAtStartup");
        s.InstallAtShutdown = info.GetBoolean("InstallAtShutDown");
        s.ConfirmInstallation = info.GetBoolean("ConfirmInstallation");
        s.ShowInstallationWindow = info.GetBoolean("ShowInstallationWindow");
        s.InstallationWindowClosingTime = info.GetInt32("InstallationWindowClosingTime");
        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Serialization

    /// <summary>Initializes a new instance of the <see cref="AutoUpdateSettings"/> class with default values.</summary>
    public AutoUpdateSettings()
    {
      EnableAutoUpdates = true;
      InstallAtShutdown = true;
#if DEBUG
      DownloadUnstableVersion = true;
#endif

      ConfirmInstallation = true;
      ShowInstallationWindow = true;
      InstallationWindowClosingTime = int.MaxValue;
    }

    public AutoUpdateSettings(AutoUpdateSettings from)
    {
      CopyFrom(from);
    }

    public bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;

      var from = obj as AutoUpdateSettings;
      if (from is not null)
      {
        EnableAutoUpdates = from.EnableAutoUpdates;
        DownloadUnstableVersion = from.DownloadUnstableVersion;
        DownloadIntervalInDays = from.DownloadIntervalInDays;
        ShowDownloadWindow = from.ShowDownloadWindow;
        InstallAtStartup = from.InstallAtStartup;
        InstallAtShutdown = from.InstallAtShutdown;
        ConfirmInstallation = from.ConfirmInstallation;
        ShowInstallationWindow = from.ShowInstallationWindow;
        InstallationWindowClosingTime = from.InstallationWindowClosingTime;

        return true;
      }
      return false;
    }

    public AutoUpdateSettings Clone()
    {
      return new AutoUpdateSettings(this);
    }

    object ICloneable.Clone()
    {
      return new AutoUpdateSettings(this);
    }
  }
}
