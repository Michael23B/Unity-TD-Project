using UnityEngine;

[System.Serializable]
public class EnemyWaveHelper : MonoBehaviour
{
    [Header("This class holds the options for each waves possible enemies")]
    [SerializeField]
    EnemyWave[] enemyGroupList = new EnemyWave[11]; //options for waves 1 - 10, with 11 being bonus waves

    public EnemyWave GenerateWave(int difficulty, int waveCount)   //generate a random wave at difficulty wave difficulty with waveCount enemygroups
    {
        int bonusCount = 0; //number of waves past 10
        int rand = 0;
        if (difficulty > 10)
        {
            bonusCount = difficulty - 10;
            difficulty = 10;
        }

        EnemyWave GeneratedWave = new EnemyWave();
        GeneratedWave.wave = new EnemyGroup[waveCount + bonusCount];

        for (int i = 0; i < GeneratedWave.wave.Length; ++i)
        {
            rand = Random.Range(0, enemyGroupList[difficulty].wave.Length);
            GeneratedWave.wave[i] = enemyGroupList[difficulty].wave[rand];
        }
        return GeneratedWave;
    }
}
