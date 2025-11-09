using System.Globalization;
using System.IO;
using SekiroTool.Models;

namespace SekiroTool.Utilities;

public class DataLoader
{
    public static Dictionary<string, List<Warp>> GetWarpLocations()
        {
            Dictionary<string, List<Warp>> warpDict = new Dictionary<string, List<Warp>>();
            string csvData = Resources.Warps;

            if (string.IsNullOrWhiteSpace(csvData)) return warpDict;

            using StringReader reader = new StringReader(csvData);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(',');
                string mainAreaName = parts[0].Trim();
                string name = parts[1].Trim();
                int idolId = int.Parse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
                
                Warp warp = new Warp
                {
                    MainArea = mainAreaName,
                    Name = name,
                    IdolId = idolId,
                };

                if (parts.Length > 3)
                {
                    string[] coordParts = parts[3].Split('|');

                    warp.Coords = new float[coordParts.Length];

                    for (int i = 0; i < coordParts.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(coordParts[i]))
                        {
                            warp.Coords[i] = float.Parse(coordParts[i], CultureInfo.InvariantCulture);
                        }
                    }
                    
                    warp.Angle = float.Parse(parts[4], CultureInfo.InvariantCulture);
                }

                if (!warpDict.ContainsKey(mainAreaName))
                {
                    warpDict[mainAreaName] = new List<Warp>();
                }
                
                warpDict[mainAreaName].Add(warp);
            }

            return warpDict;
        }

    public static Dictionary<uint, uint> RequestRespawnHash()
    {
        Dictionary<uint, uint> respawnHash = new Dictionary<uint, uint>();
        string csvData = Resources.RequestRespawnHashMap;

        using StringReader reader = new StringReader(csvData);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            string[] parts = line.Split(',');
            uint mapId = uint.Parse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture);
            uint idolId = uint.Parse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture);
            
            respawnHash.Add((mapId), idolId);
    
        }
        
        return respawnHash;

        

    }
        

    public static List<long> GetIdolEventIds()
    {
        List <long> idolEventIds = new List<long>();
        string idolIds = Resources.IdolEventFlags;
        
        if (string.IsNullOrWhiteSpace(idolIds)) return idolEventIds;
        
        using StringReader reader = new StringReader(idolIds);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            idolEventIds.Add(long.Parse(line, CultureInfo.InvariantCulture));
        }

        return idolEventIds;
    }

    public static List<Item> GetItemList(string listName, short itemType)
    {
        List<Item> itemList = new List<Item>();

        string? csvData = Resources.ResourceManager.GetString(listName);

        if (string.IsNullOrWhiteSpace(csvData)) return itemList;

        using StringReader reader = new StringReader(csvData);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            string[] parts = line.Split(',');
            int id = int.Parse(parts[0], CultureInfo.InvariantCulture);
            string name = parts[1].Trim();
            int stackSize = parts.Length > 2 ? int.Parse(parts[2]) : 0;

            itemList.Add(new Item(name, id, itemType, stackSize, listName));
        }

        return itemList;
    }
    
    public static List<Skill> GetSkillList()
    {
        List<Skill> skillList = new List<Skill>();

        string? csvData = Resources.Skills;

        if (string.IsNullOrWhiteSpace(csvData)) return skillList;

        using StringReader reader = new StringReader(csvData);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            string[] parts = line.Split(',');
            int id = int.Parse(parts[0], CultureInfo.InvariantCulture);
            string name = parts[1].Trim();
            skillList.Add(new Skill(name, id));
        }

        return skillList;
    }
}