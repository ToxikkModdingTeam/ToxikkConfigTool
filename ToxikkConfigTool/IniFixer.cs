using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ToxikkServerLauncher;

namespace ToxikkConfigTool
{
  class IniFixer
  {
    private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly List<string> defaultInis = new List<string>
    {
      "CharInfo", "CustomChar", "Editor", "EditorKeyBindings", "EditorUserSettings", "Engine", "Game", "Input", "Lightmass", "MapList", "SystemSettings", "UI", "Weapon"
    };
    private readonly string configFolder;

    public IniFixer(string configFolder)
    {
      this.configFolder = configFolder;
    }

    #region FixTimestamps()
    public void FixTimestamps()
    {
      foreach (var udkIniFilePath in Directory.GetFiles(configFolder, "UDK*.ini"))
      {
        string defaultIniFilePath = Path.Combine(configFolder, "Default" + Path.GetFileName(udkIniFilePath).Substring(3));
        if (!File.Exists(defaultIniFilePath))
          continue;

        var ini = new IniFile(udkIniFilePath);
        var sec = ini.GetSection("IniVersion");
        if (sec == null)
          continue;

        List<long> timestamps = new List<long>();
        CollectDefaultIniTimestamps(defaultIniFilePath, timestamps);
        sec.RemoveAll();
        for (int i = 0; i < timestamps.Count; i++)
          sec.Add(i.ToString(), timestamps[i].ToString());
        ini.Save();
      }
    }

    private void CollectDefaultIniTimestamps(string defaultIniFilePath, List<long> timestamps)
    {
      IniFile defaultIni = new IniFile(defaultIniFilePath);
      var conf = defaultIni.GetSection("Configuration");
      var baseIni = conf?.GetString("BasedOn");
      if (!string.IsNullOrEmpty(baseIni))
      {
        var baseFile = Path.Combine(configFolder, "..", baseIni);
        if (File.Exists(baseFile))
          CollectDefaultIniTimestamps(baseFile, timestamps);
      }
      timestamps.Add(GetTimestamp(defaultIniFilePath));
    }
    #endregion

    #region GenerateUdkConfigFiles()

    public void GenerateUdkConfigFiles()
    {
      foreach (var file in defaultInis)
      {
        var ini = new IniFile();
        var defIniPath = Path.Combine(configFolder, "Default" + file + ".ini");
        GenerateIni(defIniPath, ini);
        var udkIniPath = Path.Combine(configFolder, "UDK" + file + ".ini");
        ini.Save(udkIniPath);
      }
    }

    private void GenerateIni(string defIniPath, IniFile udkIni)
    {
      var defIni = new IniFile(defIniPath);
      var configSec = defIni.GetSection("Configuration");
      var basedOn = configSec?.GetString("BasedOn");
      if (!string.IsNullOrEmpty(basedOn))
        GenerateIni(Path.Combine(configFolder, "..", basedOn), udkIni);

      foreach (var defSec in defIni.Sections)
      {
        var udkSec = udkIni.GetSection(defSec.Name, true);

        if (defSec.Name.ToLower() == "configuration")
          continue;

        foreach (var defKey in defSec.Keys)
        {
          int valIndex = 0;
          foreach (var val in defSec.GetAll(defKey))
          {
            string udkKey = defKey.Substring(1);
            switch (defKey[0])
            {
              case '!': // remove entire property
                udkSec.Remove(udkKey);
                continue;
              case '-': // remove one value of a property list
                udkSec.Remove(udkKey, val.Value);
                continue;
              case '.': // add a value allowing duplicates
                break;
              case '+': // add a value preventing duplicates
                //if (!udkSec.Keys.Contains(udkKey)) // seems like a bug in UDK that + without an existing base value will have a literal + in the name
                //  goto default;
                if (udkSec.GetAll(udkKey).Exists(v => v.Value == val.Value))
                  continue;
                break;
              default: // set a value
                udkKey = defKey;
                if (valIndex++ == 0) // when no prefix is used, reset base configuration (but allow multiple values in the current config)
                  udkSec.Remove(udkKey);
                break;
            }
            udkSec.Add(udkKey, val.Value);
          }
        }
      }

      var versionSec = udkIni.GetSection("IniVersion", true);
      versionSec.Add(versionSec.Keys.Count().ToString(), GetTimestamp(defIniPath).ToString());
    }
    #endregion

    #region Upgrade()

    public void Upgrade()
    {
      var sb = new StringBuilder();
      foreach(var coreFile in defaultInis)
        Upgrade(coreFile, sb);
      File.WriteAllText(Path.Combine(configFolder, "Patch.ini"), sb.ToString());
    }

    private void Upgrade(string coreFileName, StringBuilder flatDiff)
    {
      var configBackupDir = Path.Combine(configFolder, "..", "ConfigBackup");
      Directory.CreateDirectory(configBackupDir);

      var udkFileName = "UDK" + coreFileName + ".ini";
      var currentPatchFile = Path.Combine(configFolder, "Default" + coreFileName + ".ini");
      var prevPatchFile = Path.Combine(configBackupDir, "Default" + coreFileName + ".ini");
      var currentUdkFile = Path.Combine(configFolder, udkFileName);

      if (File.Exists(prevPatchFile))
      {
        var currentPatchIni = new IniFile();
        GenerateIni(currentPatchFile, currentPatchIni);
        var prevPatchIni = new IniFile();
        GenerateIni(prevPatchFile, prevPatchIni);
        var patchDiff = Diff(currentPatchIni, prevPatchIni, udkFileName, flatDiff);

        var currentUdkIni = new IniFile(currentUdkFile);
        var userDiff = Diff(currentUdkIni, prevPatchIni, udkFileName, flatDiff, patchDiff);

        File.Copy(currentPatchFile, prevPatchFile, true);

        Patch(currentPatchIni, patchDiff);
        Patch(currentPatchIni, userDiff);
        //currentPatchIni.Save(currentUdkFile);

        // debug output
        patchDiff.Save(Path.Combine(configFolder, coreFileName + "PDiff.ini"));
        userDiff.Save(Path.Combine(configFolder, coreFileName + "UDiff.ini"));
        currentPatchIni.Save(Path.Combine(configFolder, coreFileName + "New.ini"));
      }
      else
        File.Copy(currentPatchFile, prevPatchFile);
    }
    #endregion

    #region Diff()
    private IniFile Diff(IniFile newIni, IniFile baseIni, string file, StringBuilder flatDiff, IniFile priorityIni = null)
    {
      var diff = new IniFile();
      foreach (var newSec in newIni.Sections)
      {
        var oldSec = baseIni.GetSection(newSec.Name);
        var priSec = priorityIni?.GetSection(newSec.Name);
        foreach (var newKey in newSec.Keys)
        {
          var newVals = newSec.GetAll(newKey);
          var oldVals = oldSec?.GetAll(newKey, true);
          var priVals = priSec?.GetAll(newKey, true);

          if (oldVals != null)
          {
            foreach (var oldVal in oldVals)
            {
              if (priVals != null && priVals.Any(v => IsEqual(v.Value, oldVal.Value)))
                continue;

              if (!newVals.Any(v => IsEqual(v.Value, oldVal.Value)))
              {
                var diffSec = diff.GetSection(newSec.Name, true);
                diffSec.Add(newKey, oldVal.Value, "-=");
                flatDiff.AppendLine($"{file}\\{newSec.Name}\\{newKey}-={oldVal.Value}");
              }
            }
          }

          foreach (var newVal in newVals)
          {
            if (priVals != null && priVals.Any(v => IsEqual(v.Value, newVal.Value)))
              continue;

            if (oldVals == null || !oldVals.Any(v => IsEqual(v.Value, newVal.Value)))
            {
              var diffSec = diff.GetSection(newSec.Name, true);
              diffSec.Add(newKey, newVal.Value, "+=");
              flatDiff.AppendLine($"{file}\\{newSec.Name}\\{newKey}+={newVal.Value}");
            }
          }
        }
      }

      return diff;
    }
    #endregion

    #region Patch()
    private void Patch(IniFile baseIni, IniFile diff)
    {
      foreach (var diffSec in diff.Sections)
      {
        var baseSec = baseIni.GetSection(diffSec.Name, true);
        foreach (var diffKey in diffSec.Keys)
        {
          foreach (var diffVal in diffSec.GetAll(diffKey))
          {
            if (diffVal.Operator == "-=")
              baseSec.Remove(diffKey, diffVal.Value);
            else if (diffVal.Operator == "+=")
            {
              if (!baseSec.GetAll(diffKey).Any(v => IsEqual(v.Value, diffVal.Value)))
                baseSec.Add(diffKey, diffVal.Value);
            }
          }
        }
      }
    }
    #endregion

    #region IsEqual()
    private bool IsEqual(string a, string b)
    {
      decimal da, db;
      if (decimal.TryParse(a, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out da) && decimal.TryParse(b, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out db))
        return da - db == 0;
      return a.Replace(" ", "").Replace("%GAME%", "UDK").Trim('"') == b.Replace(" ", "").Replace("%GAME%", "UDK").Trim('"');
    }
    #endregion

    #region GetTimestamp()
    private long GetTimestamp(string file)
    {
      // emulate the bugged UE3 code
      var fltime = File.GetLastWriteTime(file);
      var futime = File.GetLastWriteTimeUtc(file);
      
      if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now))
      {
        if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(fltime))
          return (futime - epoch).Ticks / TimeSpan.TicksPerSecond;
        return (futime - epoch).Ticks/TimeSpan.TicksPerSecond + 3600;

      }
      else
      {
        if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(fltime))
          return (futime - epoch).Ticks / TimeSpan.TicksPerSecond - 3600;
        return (futime - epoch).Ticks/TimeSpan.TicksPerSecond;
      }
    }
    #endregion
  }
}
