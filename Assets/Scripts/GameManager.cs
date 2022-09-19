using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    public ParticleSystem explosionParticle;


    private GroundPieceController[] allGroundPiece;
    // Start is called before the first frame update

    private void Awake()
    {
        if(singleton == null)
        {
            singleton = this;

        }
        else if(singleton != this)
        {
            Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        SetupNewLevel();
        explosionParticle.Play();

    }

    private void SetupNewLevel()
    {
        allGroundPiece = FindObjectsOfType<GroundPieceController>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {

        SetupNewLevel();
    }

    public void CheckComplete()
    {
        bool isFinished = true;

        for (int i = 0; i < allGroundPiece.Length; i++)
        {
            if (allGroundPiece[i].isColored == false)
            {
                isFinished = false;
                break;
            }
        }

        if(isFinished)
        {

            // run Next Level
            NextLevel();
        }
    }

    private void NextLevel()
    {
        if(SceneManager.GetActiveScene().buildIndex == 3)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

}
