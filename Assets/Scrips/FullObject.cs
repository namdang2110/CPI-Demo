using Scrips;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FullObject : MonoBehaviour
{
    private int pointsToDone;
    private int currentPoints;
    public GameObject ObjectContainer, Shadow;
    public ObjectPiece[] Objects;
    public SkeletonAnimation Anim;
    [SpineAnimation(dataField = "Anim")] public string AnimName;

    private static int piecesCompleted;
    
    private void Awake()
    {
        foreach (var objectPiece in Objects)
        {
            objectPiece.OnCorrectPiece += AddPoints;
        }
    }

    private void Start()
    {
        pointsToDone = Objects.Length;
    }

    public void AddPoints()
    {
        currentPoints++;
        if (currentPoints >= pointsToDone)
        {
            if (!string.IsNullOrEmpty(AnimName))
            {
                Gameplay.Instance.Unableobject();
                
                SoundManager.Instance.PlayWinSound();
                
                if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
                {
                    PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
                    PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
                    PlayerPrefs.Save();
                }

                ProgressBar progressBar = FindObjectOfType<ProgressBar>();
                if (progressBar != null && Gameplay.Instance.objectIndexer <= Gameplay.Instance.Layers.Length)
                {
                    progressBar.IncrementLayer();
                }
                
                Anim.gameObject.SetActive(true);
                Anim.AnimationState.SetAnimation(0, AnimName, false);
                Anim.AnimationState.AddAnimation(0, AnimName, false, 0).Complete += entry =>
                {
                    if (Gameplay.Instance != null)
                    {
                        Gameplay.Instance.EnableNextObject();
                    }
                };
            }
            else
            {
                if (Gameplay.Instance != null)
                {
                    Gameplay.Instance.EnableNextObject();
                }
            }
        }
    }
}
