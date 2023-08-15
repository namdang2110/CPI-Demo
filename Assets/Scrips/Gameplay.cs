using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

namespace Scrips
{
    public class Gameplay : MonoBehaviour
    {
        //Tạo danh sách list các đối tượng 'ObjectPiece'
        [SerializeField] private List<ObjectPiece> objectPieces = new List<ObjectPiece>();
        
        //Tạo đối tương để lưu trữ đối tượng đầu tiên chưa hoàn thành trong danh sách 'objectPiece'
        private ObjectPiece _first;
        
        //Tạo một đối tương 'Gameplay' duy nhất, được sử dụng để truy cập vào lớp từ bất kì nơi nào trong mã nguồn
        private static Gameplay _instance;
        public static Gameplay Instance => _instance;

        //Các đối tượng nút Button của trò chơi
        public Button NextLevelButton, ReplayButton, HintButton, SettingButton, SoundButton, MenuButton;
        
        //Một mảng các đối tượng 'FullObject' để khai báo các Layers của màn chơi
        public FullObject[] Layers;
        
        //'objectIndexer' để theo dõi chỉ mục của đối tượng 'FullObject đang được hiển thị' 
        //'curentLevel' để theo dõi cấp độ hiện tại của trò chơi
        public int objectIndexer, currentLevel;
        
        //Danh sách các nút con trong đối tượng nút cài đặt 'Setting Button'
        public List<GameObject> SettingButtons;
        
        //Một biến đánh dấu xem cài đặt có đang mở hay không
        private bool isSettingOpen = false;

        //Một đối tượng Transform đại diện cho hình ảnh gợi ý của trò chơi
        [SerializeField] private Transform hintHand;

        [SerializeField] public Image soundImg;
        [SerializeField] public Sprite mute, notMute;
        
        public int totalLevels = 14;
        
        private bool isLastLevelCompleted = false;

        public GameObject comingSoonPrefab;

        //Phương thức 'Awake()' được gọi khi đối tượng scrips được khởi tạo
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(_instance.gameObject);
                _instance = this;
            }
            objectIndexer = 0;
            var levelName = SceneManager.GetActiveScene().name.Split(" ");
            int.TryParse(levelName[1], out currentLevel);

            NextLevelButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlayClickSound();
                SceneManager.LoadScene("Man " + (currentLevel + 1));
            });
            
            ReplayButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlayClickSound();
                ReplayButton.transform.DOLocalRotate(
                        new Vector3(0, 0, 360), 1, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutQuart)
                    .OnComplete(() =>
                {
                    SceneManager.LoadScene("Man " + currentLevel);
                });
            });
            
            SoundButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlayClickSound();
                SoundManager.Instance.ToggleSound();
            });

            SettingButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlayClickSound();
                SettingButton.transform.DOLocalRotate(
                        new Vector3(0, 0, 360), 1, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutQuart);
                ToggleSettingButtons();
            });
            
            HintButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlayClickSound();
                GetHint();
            });
            
            MenuButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlayClickSound();
                SceneManager.LoadScene("Menu Prefabs");
            });

            HideSettingButtons();

            foreach (var objectPiece in objectPieces)
            {
                objectPiece.OnCorrectPiece += FinishPiece;
            }
        }

        private void Start()
        {
            Layers[objectIndexer].gameObject.SetActive(true);
            objectIndexer++;
        }

        private void FinishPiece()
        {
            if (hintHand.gameObject.activeSelf)
            {
                hintHand.gameObject.SetActive(false);
                hintHand.transform.DOKill();
            }
            
            if (currentLevel == totalLevels && objectIndexer >= Layers.Length)
            {
                isLastLevelCompleted = true;
            }
        }
        
        
        public void EnableNextObject()
        {
            if (objectIndexer < Layers.Length)
            {
                Layers[objectIndexer].gameObject.SetActive(true);
                objectIndexer++;

                ProgressBar progressBar = FindObjectOfType<ProgressBar>();
                if (progressBar != null && objectIndexer <= Layers.Length)
                {
                    progressBar.IncrementLayer();
                }
            }
            else
            {
                if (isLastLevelCompleted)
                {
                    comingSoonPrefab.transform.DOMove(new Vector3(0, -3, 0), 1)
                        .SetEase(Ease.OutBack);
                }
                else //if (currentLevel == totalLevels)
                {
                    // Hiển thị nút Next Level để chuyển đến màn tiếp theo
                    NextLevelButton.gameObject.SetActive(true);
                }
            }
        }

        public void Unableobject()
        {
            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i].ObjectContainer.SetActive(false);
                Layers[i].Shadow.SetActive(false);
            }
        }

        private void GetHint()
        {
            if (hintHand.gameObject.activeSelf)
            {
                hintHand.transform.DOKill();
                hintHand.gameObject.SetActive(false);
            }
            
            // trong list objectPieces, tim phan tu dau tien ma co bien finish bang false
            _first = objectPieces.FirstOrDefault(_ => !_.finish);

            if (_first != null)
            {
                var startPos = _first.transform.position;
                var endPos = _first.correctForm.transform.position;

                hintHand.transform.position = startPos;
                hintHand.gameObject.SetActive(true);

                hintHand.transform.DOMove(endPos, 1.5f)
                    .SetEase(Ease.InOutCirc)
                    .SetLoops(-1, LoopType.Restart);
            }
        }
        
        private void ShowSettingButtons()
        {
            foreach (var button in SettingButtons)
            {
                button.SetActive(true);
            }
            isSettingOpen = true;
        }

        private void HideSettingButtons()
        {
            foreach (var button in SettingButtons)
            {
                button.SetActive(false);
            }
            isSettingOpen = false;
        }
        
        private void ToggleSettingButtons()
        {
            if (isSettingOpen)
            {
                HideSettingButtons();
            }
            else
            {
                ShowSettingButtons();
            }
        }
    }
}