using System;
using UnityEngine;

public class LocalRandom : MonoBehaviour {
    
    public static LocalRandom Instance;

    public float[] randomValues;
    public int index = 0;
    int randomLength = 100;

    void Start()
    {
        randomValues = new float[randomLength];
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Error: More than one localRandom in scene!");
            return;
        }
        Instance = this;
    }

    public int GetNextRandom(int max = 1, bool positiveOnly = true) //TODO: im sure theres a better way to do this but it works for now
    {
        if (randomValues.Length == 0) return 0;

        index++;
        if (index >= randomValues.Length) index = 0;

        if (positiveOnly)
        {
            int i = (int)Math.Round(randomValues[index] * max);
            if (i < 0) i = -i;
            return i;
        }
        else return (int)Math.Round(randomValues[index] * max);
    }

    public float GetNextRandom(float max, bool positiveOnly = true)
    {
        if (randomValues.Length == 0) return 0;

        index++;
        if (index >= randomValues.Length) index = 0;

        if (positiveOnly)
        {
            float f = randomValues[index] * max;
            if (f < 0) f = -f;
            return f;
        }
        else return randomValues[index] * max;
    }

    #region Server only (number generation)
    //Only call this when nothing is happening, otherwise the random set will be changed while something is using it. This will happen on the host, which may desync the game for clients
    void GenerateRandomValues()
    {
        for (int i = 0; i < randomValues.Length; ++i)
        {
            randomValues[i] = UnityEngine.Random.Range(-1f, 1f);
        }
    }

    public float[] GetRandomValues()
    {
        GenerateRandomValues();
        return randomValues;
    }
    #endregion
}
