using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
namespace SP
{
    [IncludeInSettings(true)]
    public class LevelController : MonoBehaviour
    {
        public Transform[] levels;
        public enum LevelOrder { Sequential, Random, SequentialThenRandom };
        public LevelOrder order;
        public int levelIndex { get; private set; } = 0;
        public int levelNumber { get; private set; } = 1;
        void Awake()
        {
            Variables.ActiveScene.Set("levelController", this);
            levelIndex = LevelIndex;
            levelNumber = LevelNumber;
            foreach(Transform item in transform)
            {
                item.gameObject.SetActive(false);
            }
        }
        private void Start()
        {
            if (levels.Length < 1)
            {
                Debug.LogError("There are no levels in the list to load from.", this);
                return;
            }
            if (levelIndex >= levels.Length)
            {
                Debug.LogError("level index is out of ragne, a random level is selected.", this);
                levelIndex = Random.Range(0, levels.Length);
            }
            Instantiate(levels[levelIndex]).gameObject.SetActive(true);
        }

        void Update()
        {

        }

        public bool LevelComplete()
        {
            int preLevel = levelIndex;
            levelIndex++;
            levelNumber++;
            if (order == LevelOrder.Random || (order == LevelOrder.SequentialThenRandom && levelNumber > levels.Length))
            {
                do
                {
                    levelIndex = Random.Range(0, levels.Length);
                } while (levelIndex == preLevel && levels.Length > 1);
            }
            else if (levelNumber > levels.Length)
            {
                levelIndex--;
                levelNumber--;
                return false;
            }
            LevelIndex = levelIndex;
            LevelNumber = levelNumber;
            if(LevelNumber>LevelUnlocked)
                LevelUnlocked = LevelNumber;
            return true;
        }
        static public void LoadNextLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        static public void RestartGame()
        {
            LevelIndex = 0;
            LevelNumber = 1;
            LevelUnlocked = 1;
            LoadNextLevel();
        } 
        static public void SelectLevel(int levelNumber)
        {
            if (levelNumber <= LevelUnlocked) 
            {
                LevelIndex = levelNumber - 1;
                LevelNumber = levelNumber;
            }
        }
        static public int LevelIndex
        {
            get => PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "_level", 0);
            private set => PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_level", value);
        }
        static public int LevelNumber
        {
            get => PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "_level_num", LevelIndex+1);
            private set => PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_level_num", value);
        }
        static public int LevelUnlocked
        {
            get => PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "_level_unlocked", 1);
            private set => PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_level_unlocked", value);
        }
    }
}
