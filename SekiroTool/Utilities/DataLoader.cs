using System.Globalization;
using System.IO;
using SekiroTool.Models;

namespace SekiroTool.Utilities;

public class DataLoader
{
    public static Dictionary<string, List<Warp>> GetLocations()
        {
            Dictionary<string, List<Warp>> warpDict = new Dictionary<string, List<Warp>>();
            string csvData = Resources.Warps;

            if (string.IsNullOrWhiteSpace(csvData))
                return warpDict;

            using StringReader reader = new StringReader(csvData);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(',');
                string mainAreaName = parts[0].Trim();
                string idonName = parts[1].Trim();
                int idolId = int.Parse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
                
                Warp warp = new Warp
                {
                    MainArea = mainAreaName,
                    IdolName = idonName,
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
}