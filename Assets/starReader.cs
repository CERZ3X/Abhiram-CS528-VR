using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
public class starReader : MonoBehaviour
{
    public GameObject OStar, BStar, AStar, FStar, GStar, KStar, MStar, line;
    public Dictionary<string, GameObject> starObjects;
    private Dictionary<int, GameObject> starDictionary = new Dictionary<int, GameObject>();
    private const string CsvPath = "Assets/updatedStarData.csv";
    private static readonly CultureInfo Culture = new CultureInfo("en-US");

    void Start()
    {
        InitializeStarPrefabs();
        StartCoroutine(LoadStars());
        // Example of one such constellation. This is just a placeholder for phase 1
        DrawLineBetweenStars(61084, 60718);
        DrawLineBetweenStars(62434, 59747);
    }

    void InitializeStarPrefabs()
    {
        Debug.Log("Initialsing prefabs");
        starObjects = new Dictionary<string, GameObject>
        {
            { "O", OStar },
            { "B", BStar },
            { "A", AStar },
            { "F", FStar },
            { "G", GStar },
            { "K", KStar },
            { "M", MStar }
        };
    }
    // IEnumerator LoadCSVFile()
    // {
    //     string csvPath = "Assets/updatedStarData.csv";
    //     CultureInfo culture = new CultureInfo("en-US");
    //     int count = 0;
    //     if (!File.Exists(csvPath))
    //     {
    //         Debug.LogError($"CSV file is not found at the path: {csvPath}");
    //         yield break;
    //     }
    //     using (StreamReader reader = new StreamReader(csvPath))
    //     {
    //         while (!reader.EndOfStream)
    //         {
    //             string line = reader.ReadLine();
    //             string[] values = line.Split(',');
    //             float x0, y0, z0;
    //             float hip, dist, absMag, mag, vx, vy, vz;
    //             string spect = "";
    //             Vector3 pos = new Vector3();
    //             Vector3 vel = new Vector3();

    //             if (float.TryParse(values[2], NumberStyles.Float, culture, out x0) && float.TryParse(values[3], NumberStyles.Float, culture, out y0) && float.TryParse(values[4], NumberStyles.Float, culture, out z0))
    //             {
    //                 pos = new Vector3(x0, y0, z0);
    //                 float.TryParse(values[0], NumberStyles.Float, culture, out hip);
    //                 float.TryParse(values[1], NumberStyles.Float, culture, out dist);
    //                 float.TryParse(values[5], NumberStyles.Float, culture, out absMag);
    //                 float.TryParse(values[6], NumberStyles.Float, culture, out mag);
    //                 spect = values[10];
    //                 if (float.TryParse(values[7], NumberStyles.Float, culture, out vx) && float.TryParse(values[8], NumberStyles.Float, culture, out vy) && float.TryParse(values[9], NumberStyles.Float, culture, out vz))
    //                 {
    //                     vel = new Vector3(vx, vy, vz);
    //                 }
    //                 count++;
    //             }
    //             else { Debug.LogError("star position is not parsed"); }
    //             InitialiseObject(spect, pos, count);
    //         }
    //     }
    // }

    IEnumerator LoadStars()
    {
        Debug.Log("Loading stars from CSV...");
        yield return ParseCsvFile();
    }

    public IEnumerator ParseCsvFile()
    {
        Debug.Log("ParseCSV");
        if (!File.Exists(CsvPath))
        {
            Debug.LogError($"CSV file is not found at the path: {CsvPath}");
            yield break;
        }

        int count = 0;
        using (var reader = new StreamReader(CsvPath))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                if (values.Length < 11) // Ensure there are enough columns
                {
                    Debug.LogError("Invalid line format.");
                    continue;
                }

                if (TryParseStarData(values, out StarData starData))
                {
                    InitialiseObject(starData.Spect, starData.Position, ++count);
                }
                else
                {
                    Debug.LogError("Failed to parse star data.");
                }
            }
        }
        yield break;
    }

    private bool TryParseStarData(string[] values, out StarData starData)
    {
        Debug.Log("Trying to parse...");
        starData = new StarData();
        bool success = true;

        success &= float.TryParse(values[2], NumberStyles.Float, Culture, out starData.Position.x);
        Debug.Log($"Parse Position.x: {success} Value: '{values[2]}'");

        success &= float.TryParse(values[3], NumberStyles.Float, Culture, out starData.Position.y);
        Debug.Log($"Parse Position.y: {success} Value: '{values[3]}'");

        success &= float.TryParse(values[4], NumberStyles.Float, Culture, out starData.Position.z);
        Debug.Log($"Parse Position.z: {success} Value: '{values[4]}'");

        success &= float.TryParse(values[0], NumberStyles.Float, Culture, out starData.Hip);
        Debug.Log($"Parse Hip: {success} Value: '{values[0]}'");

        success &= float.TryParse(values[1], NumberStyles.Float, Culture, out starData.Dist);
        Debug.Log($"Parse Dist: {success} Value: '{values[1]}'");

        success &= float.TryParse(values[5], NumberStyles.Float, Culture, out starData.AbsMag);
        Debug.Log($"Parse AbsMag: {success} Value: '{values[5]}'");

        success &= float.TryParse(values[6], NumberStyles.Float, Culture, out starData.Mag);
        Debug.Log($"Parse Mag: {success} Value: '{values[6]}'");
        
        starData.Spect = values[10];

        success &= TryParseVelocity(values, out starData.Velocity);
        // The TryParseVelocity method includes its own debug logs.

        success &= !string.IsNullOrEmpty(values[10]);
        Debug.Log($"Spectral Type Non-Empty: {success} Value: '{values[10]}'");

        if (!success)
        {
            Debug.LogError("Failed parsing one or more fields.");
        }

        return success;
    }

    private bool TryParseVelocity(string[] values, out Vector3 velocity)
    {
        velocity = new Vector3();
        bool success = true;

        success &= float.TryParse(values[7], NumberStyles.Float, Culture, out velocity.x);
        Debug.Log($"Parse Velocity.x: {success} Value: '{values[7]}'");

        success &= float.TryParse(values[8], NumberStyles.Float, Culture, out velocity.y);
        Debug.Log($"Parse Velocity.y: {success} Value: '{values[8]}'");

        success &= float.TryParse(values[9], NumberStyles.Float, Culture, out velocity.z);
        Debug.Log($"Parse Velocity.z: {success} Value: '{values[9]}'");

        if (!success)
        {
            Debug.LogError("Failed parsing velocity fields.");
        }

        return success;
    }

    private struct StarData
    {
        public float Hip;
        public float Dist;
        public Vector3 Position;
        public float AbsMag;
        public float Mag;
        public string Spect;
        public Vector3 Velocity;
    }

    void InitialiseObject(string spect, Vector3 position, int id)
    {
        Debug.Log("Instantiate star");
        if (starObjects.TryGetValue(spect, out GameObject prefab))
        {
            GameObject star = Instantiate(prefab, position, Quaternion.identity);
            AddToDict(id, star);
        }
        else
        {
            Debug.LogWarning($"Prefab for spectral type '{spect}' not found.");
        }
    }

    private void AddToDict(int id, GameObject starObject)
    {
        if (!starDictionary.ContainsKey(id))
        {
            starDictionary.Add(id, starObject);
        }
        else
        {
            Debug.LogError("Duplicate star ID is detected: " + id);
        }
    }
    private GameObject GetStarById(int id)
    {
        if (starDictionary.TryGetValue(id, out GameObject starObject))
        {
            return starObject;
        }
        else
        {
            Debug.LogError("Star ID is not found: " + id);
            return null;
        }
    }
    void DrawLineBetweenStars(int id1, int id2)
    {
        GameObject star1 = GetStarById(id1);
        GameObject star2 = GetStarById(id2);
        GameObject lineObj = Instantiate(line, Vector3.zero, Quaternion.identity);
        LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
        if (star1 == null || star2 == null || lineRenderer == null) return;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, star1.transform.position);
        lineRenderer.SetPosition(1, star2.transform.position);
    }
}