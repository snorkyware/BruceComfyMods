﻿using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Pintervention.Extensions;
using static Pintervention.PluginConfig;
using static Pintervention.Pintervention;


namespace Pintervention.Core {
  public class SharedMapDataManager {

    public static void ReadMapData(ZDO zdo) {
      byte[] array = zdo.GetByteArray(ZDOVars.s_data, null);

      if (array == null) {
        LogWarning("No data found in zdo data field.");
        return;
      }

      if (!RevealMapOnRead.Value && !TakePinsOnRead.Value) {
        MessageLocalPlayer("All cartography table data disabled on read.");
        return;
      }

      byte[] mapData = Utils.Decompress(array);
      ZPackage zPackage = new ZPackage(mapData);
      int version = zPackage.ReadInt();

      if (TakePinsOnRead.Value) {
        WritePinsToMap(zPackage, version);
        MessageLocalPlayer("Added pins from cartography table to map.");
        return;
      }

      if (RevealMapOnRead.Value) {
        WriteExploredAreaToMap(zPackage, version);
        MessageLocalPlayer("Added revealed area from cartography table to map.");
        return;
      }
    }

    public static void WritePinsToMap(ZPackage zPackage, int version) {
      if (Minimap.instance.ReadExploredArray(zPackage, version) == null) {
        LogWarning("Failed to read explored map from ZPackage.");
        return;
      }

      List<Minimap.PinData> pinsToWrite = ReadImportedPins(zPackage, version);
      if (!pinsToWrite.Any()) {
        MessageLocalPlayer("No new pins found to copy to player map.");
        return;
      }

      Minimap.instance.m_pins.AddRange(pinsToWrite);
    }

    public static List<Minimap.PinData> ReadImportedPins(ZPackage zPackage, int version) {
      List<Minimap.PinData> importPins = new();

      if (version < 2) {
        return importPins;
      }

      int pinCount = zPackage.ReadInt();
      for (int i = 0; i < pinCount; i++) {
        Minimap.PinData newPin = ReadPin(zPackage, version);

        if (newPin == null) {
          continue;
        }

        importPins.Add(newPin);
      }

      return importPins;
    }

    static Minimap.PinData ReadPin(ZPackage zPackage, int version) {
      long pid = zPackage.ReadLong();
      string pinName = zPackage.ReadString();
      Vector3 position = zPackage.ReadVector3();
      Minimap.PinType type = (Minimap.PinType)zPackage.ReadInt();
      bool isChecked = zPackage.ReadBool();
      string author = ((version >= 3) ? zPackage.ReadString() : "");
      
      if (Minimap.instance.HavePinInRange(position, 1f)) {
        return null;
      }

      Minimap.PinData newPin = new Minimap.PinData()
          .SetOwnerID(pid)
          .SetName(pinName)
          .SetPosition(position)
          .SetType(type)
          .SetChecked(isChecked)
          .SetAuthor(author)
          .SetIcon(Minimap.instance.GetSprite(type));

      if (!string.IsNullOrEmpty(newPin.m_name)) {
        newPin.m_NamePinData = new Minimap.PinNameData(newPin);
      }

      return newPin;
    }

    public static bool WriteExploredAreaToMap(ZPackage zPackage, int version) {
      List<bool> exploredPoints = Minimap.instance.ReadExploredArray(zPackage, version);

      if (exploredPoints == null) {
        MessageLocalPlayer("No new explored area to copy to player map.");
        return false;
      }

      bool applyFog = false;

      for (int i = 0; i < Minimap.instance.m_textureSize; i++) {
        for (int j = 0; j < Minimap.instance.m_textureSize; j++) {
          int location = i * Minimap.instance.m_textureSize + j;

          if (exploredPoints[location]
              && ((Minimap.instance.m_exploredOthers[location] || Minimap.instance.m_explored[location]) != exploredPoints[location])
              && Minimap.instance.ExploreOthers(j, i)) {

            applyFog = true;
          }
        }
      }

      if (!applyFog) {
        return false;
      }

      Minimap.instance.m_fogTexture.Apply();
      return true;
    }
  }
}
