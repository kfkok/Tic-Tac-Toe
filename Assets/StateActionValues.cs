using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class StateActionValues : MonoBehaviour
{
    protected Dictionary<string, Dictionary<Tuple<int, int>, float>> stateActionValues;
    string fileName;
    int initialStateCount;
    bool ready;

    void Start()
    {
        stateActionValues = new Dictionary<string, Dictionary<Tuple<int, int>, float>>();
        fileName = "state action values.txt";
        ready = false;

        InitStateActionValues();
    }

    void Test()
    {
        string key = GenerateStateString(new int[3,3] { { 0, 1, 1 }, { 2, 1, 1 }, { 0, 1, 1 } });
        Dictionary<Tuple<int, int>, float> value = new Dictionary<Tuple<int, int>, float>();
        value.Add(new Tuple<int, int>(1, 1), 0.4f);
        value.Add(new Tuple<int, int>(2, 2), 0.7f);
        stateActionValues.Add(key, value);

        key = GenerateStateString(new int[3, 3] { { 0, 2, 1 }, { 2, 1, 1 }, { 0, 1, 1 } });
        value = new Dictionary<Tuple<int, int>, float>();
        value.Add(new Tuple<int, int>(1, 1), 0.4f);
        value.Add(new Tuple<int, int>(2, 2), 0.7f);
        value.Add(new Tuple<int, int>(2, 1), 0.7f);
        stateActionValues.Add(key, value);

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream("a.txt", FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, stateActionValues);
        stream.Close();
        print("Write complete");

        stream = new FileStream("a.txt", FileMode.Open, FileAccess.Read);
        Dictionary<string, Dictionary<Tuple<int, int>, float>> d2 = (Dictionary<string, Dictionary<Tuple<int, int>, float>>)formatter.Deserialize(stream);

    }

    public void Set(int[,] state, Dictionary<Tuple<int, int>, float> actionValues)
    {
        string stateString = GenerateStateString(state);

        if (stateActionValues.ContainsKey(stateString))
        {
            stateActionValues[stateString] = actionValues;
        }
        else
        {
            stateActionValues.Add(stateString, actionValues);

        }

        //print("Added key: " + stateString);
    }

    public Dictionary<Tuple<int, int>, float> Retrieve(int[,] state)
    {
        string stateString = GenerateStateString(state);

        if (stateActionValues.ContainsKey(stateString))
        {
            //print("Getting key: " + stateString);

            return stateActionValues[stateString];
        }
        else
        {
            return null;
        }
    }

    string GenerateStateString(int[,] squares)
    {
        StringBuilder sb = new StringBuilder(string.Empty);

        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 3; column++)
            {
                sb.Append(squares[row, column]);
            }
        }

        return sb.ToString();
    }

    public void SaveStateActionValues()
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, stateActionValues);
        stream.Close();

        int finalStateCount = stateActionValues.Count;
        print("Initial state count: " + initialStateCount + ", Final state count: " + finalStateCount + ", delta: " + (finalStateCount - initialStateCount));

        initialStateCount = finalStateCount;

        string filePath = System.IO.Path.GetFullPath(fileName);
        print("state action values saved to " + filePath);
    }

    void InitStateActionValues()
    {
        string filePath = System.IO.Path.GetFullPath(fileName);
        bool exist = File.Exists(filePath);
        
        if (exist)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            stateActionValues = (Dictionary<string, Dictionary<Tuple<int, int>, float>>)formatter.Deserialize(stream);
            print("Imported state action values from " + filePath);
        }
        else
        {
            print(fileName + " not found, initializing state action values");
            stateActionValues = new Dictionary<string, Dictionary<Tuple<int, int>, float>>();
        }

        initialStateCount = stateActionValues.Count;
        print("Initial state count = " + initialStateCount);

        ready = true;
    }

    /// <summary>
    /// Returns true after sucessfully initialize the state action values
    /// </summary>
    /// <returns></returns>
    public bool Ready()
    {
        return ready;
    }

    void PrintStateActionValues()
    {
        string s = "";
        foreach (var key in stateActionValues)
        {
            s = key.Key;
            var v = key.Value;
            foreach (var av in v)
            {
                s += av.Key + ": " + av.Value;
            }

            print(s);
        }
    }
}
