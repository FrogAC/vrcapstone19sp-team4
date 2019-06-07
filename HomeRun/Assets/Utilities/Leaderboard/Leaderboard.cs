using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] string leaderboardName = "Zombies";
    [SerializeField] int numEntries = 10;
    Entry[] leaderboardEntries;
    
    // initialize with data from playerPrefs
    void Start()
    {
        leaderboardEntries = new Entry[numEntries];
        ReadEntriesFromPlayerPrefs();
    }

    // Make sure that our current data gets added to player prefs when Destroyed or Quitting
    void onDestroy(){
        WriteEntriesToPlayerPrefs();
    }
    void OnApplicationQuit()
    {
        WriteEntriesToPlayerPrefs();
    }

    // Add a entry and sort the array.
    public void AddEntry(Entry newEntry){
        
        Entry[] allEntries = new Entry[leaderboardEntries.Length + 1];
        System.Array.Copy(leaderboardEntries, allEntries, leaderboardEntries.Length);
        allEntries[leaderboardEntries.Length] = newEntry;
        SortEntriesByScore(allEntries);
        System.Array.Copy(allEntries, leaderboardEntries, leaderboardEntries.Length);  
    }

    public Entry[] GetEntries(){
        return (Entry[])leaderboardEntries.Clone();
    }

    [ContextMenu("ClearLeaderboard")]
    public void ClearLeaderboard(){
        PlayerPrefs.DeleteKey(leaderboardName);
    }

    // Write leaderboardEntries to playerPrefs
    [ContextMenu("WriteEntriesToPlayerPrefs")]
    void WriteEntriesToPlayerPrefs(){
        string key = leaderboardName;

        string value = "";
        for(int i = 0; i < leaderboardEntries.Length; i++){
            if (leaderboardEntries[i] == null){
                continue;
            }
            
            if (i != 0) {
                value += Entry.DELIMITER;
            }
            value += leaderboardEntries[i].getValueString();
        }

        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save;
    }

    // Read an array of Entries from playerPrefs for this leaderboard
    [ContextMenu("ReadEntriesFromPlayerPrefs")]
    void ReadEntriesFromPlayerPrefs(){
        string data = PlayerPrefs.GetString(leaderboardName);
        string[] tokens = data.Split(Entry.DELIMITER);
        // Extract Entry data from tokens, up to the limit of numEntries
        for (int i = 0; i < leaderboardEntries.Length; i++) {
            // Make sure that there are enough tokens to extract the i-th entry
            if (i * Entry.NUM_FIELDS < tokens.Length){
                leaderboardEntries[i] = new Entry();
                leaderboardEntries[i].score = System.Int32.Parse(tokens[i * Entry.NUM_FIELDS + 0]);
                leaderboardEntries[i].username = tokens[i * Entry.NUM_FIELDS + 1];
            } else {
                leaderboardEntries[i] = null;
            }
        }
    }

    // Formats content for display
    string ToFormatedString(){
        string output = leaderboardName + "\n";
        output += "Score:\t UserName:\n";
        for (int i = 0; i < leaderboardEntries.Length; i++) {
            // Make sure that there are enough tokens to extract the i-th entry
            if (leaderboardEntries[i] != null){
                output += leaderboardEntries[i].score + "\t";
                output += leaderboardEntries[i].username + "\n";
                
            } else {
                leaderboardEntries[i] = null;
            }
        }
        return output;
    }

    // Sort the given entries by decreasing score
    void SortEntriesByScore(Entry[] array){
        System.Array.Sort(array, delegate(Entry e1, Entry e2) {
                    if (e1 == null && e2 == null) return 0;
                    if (e1 != null && e2 == null) return 1;
                    if (e1 == null && e2 != null) return -1;
                    return -e1.score.CompareTo(e2.score);
                  });
    }
    
    // Helps Debug Leaderboard
    [ContextMenu("Debug-AddEntry")]
    void DebugAddEntry(){
        Entry e = new Entry();
        e.score = Random.Range(0, 100);
        e.username = "debugAdded";
        AddEntry(e);
        Debug.Log("debug adding " + e.score);
    }

    [ContextMenu("Debug-PrintLeaderboard")]
    void DebugPrintLeaderboard(){
        Debug.Log(ToFormatedString());
    }

    [System.Serializable]
    public class Entry {
        public static char DELIMITER = '\t';
        public static int NUM_FIELDS = 2;
        public int score = 0;
        public string username = "no-name";

        public string getValueString(){
            string output = "";
            output += score;
            output += DELIMITER;
            output += username;
            return output;
        }
    }
}
