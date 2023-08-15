using System;
using Scrips;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public GameObject dotPrefab;
    public GameObject linePrefab;
    
    public Sprite grayDotSprite;
    public Sprite grayLineSprite;
    public Sprite greenDotSprite;
    public Sprite greenLineSprite;

    private int totalLayers;
    private int currentLayer;
    
    private GameObject[] dots;
    private GameObject[] lines;
        
    private Vector2[] dotPositions2 = { new Vector2(-200, 650), new Vector2(0, 650), new Vector2(200, 650)};
    private Vector2[] linePositions2 = { new Vector2(-100, 650), new Vector2(100, 650) };
    
    private Vector2[] dotPositions3 = { new Vector2(-300, 650), new Vector2(-100, 650), new Vector2(100, 650), new Vector2(300, 650)};
    private Vector2[] linePositions3 = { new Vector2(-200, 650), new Vector2(0, 650) , new Vector2(200, 650) };

    void Start()
    {
        totalLayers = Gameplay.Instance.Layers.Length;
        currentLayer = 0;

        GenerateProgressBar();
    }

    private void GenerateProgressBar()
    {
        int totalDots = totalLayers + 1;
        int totalLines = totalLayers;

        dots = new GameObject[totalDots];
        lines = new GameObject[totalLines];

        Vector2[] dotPositions;
        Vector2[] linePositions;
        
        if (totalLayers == 2)
        {
            dotPositions = dotPositions2;
            linePositions = linePositions2;
        }
        else if (totalLayers == 3)
        {
            dotPositions = dotPositions3;
            linePositions = linePositions3;
        }
        else
        {
            Debug.LogError("Unsupported number of layers: " + totalLayers);
            return;
        }
        
        for (int i = 0; i < totalDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, transform);
            dots[i].name = "Dot" + (i + 1);
            dots[i].GetComponent<Image>().sprite = grayDotSprite;
            dots[i].transform.localPosition = dotPositions[i];
        }

        for (int i = 0; i < totalLines; i++)
        {
            lines[i] = Instantiate(linePrefab, transform);
            lines[i].name = "Line" + (i + 1);
            lines[i].GetComponent<Image>().sprite = grayLineSprite;
            lines[i].transform.localPosition = linePositions[i];
        }

        UpdateProgressBar();
    }

    public void UpdateProgressBar()
    {
        for (int i = 0; i < currentLayer * 2 + 1; i++)
        {
            if (i % 2 == 0)
            {
                // Nếu i là số chẵn, thì đây là dot
                dots[i / 2].GetComponent<Image>().sprite = greenDotSprite;
            }
            else
            {
                // Nếu i là số lẻ, thì đây là line
                lines[i / 2].GetComponent<Image>().sprite = greenLineSprite;
            }
        }

        // Đặt lại sprite của các dot và line chưa hoàn thành về màu xám
        for (int i = currentLayer * 2 + 1; i < totalLayers * 2 + 1; i++)
        {
            if (i % 2 == 0)
            {
                dots[i / 2].GetComponent<Image>().sprite = grayDotSprite;
            }
            else
            {
                lines[i / 2].GetComponent<Image>().sprite = grayLineSprite;
            }
        }

        // Kiểm tra xem layer hiện tại đã hoàn thành chưa (tất cả dot và line đã được chuyển sang màu xanh)
        bool layerCompleted = true;
        for (int i = 0; i < totalLayers * 2 + 1; i++)
        {
            if (i % 2 == 0)
            {
                if (dots[i / 2].GetComponent<Image>().sprite != greenDotSprite)
                {
                    layerCompleted = false;
                    break;
                }
            }
            else
            {
                if (lines[i / 2].GetComponent<Image>().sprite != greenLineSprite)
                {
                    layerCompleted = false;
                    break;
                }
            }
        }

        // Nếu layer hiện tại đã hoàn thành và không phải là layer cuối cùng, tăng lên layer tiếp theo
        if (layerCompleted && currentLayer < totalLayers)
        {
            currentLayer++;
            IncrementLayer();
        }

        // Xử lý đổi màu cho layer cuối cùng
        if (layerCompleted && currentLayer == totalLayers)
        {
            // Đổi màu cho dot và line cuối cùng thành màu xanh
            dots[totalLayers].GetComponent<Image>().sprite = greenDotSprite;
            lines[totalLayers - 1].GetComponent<Image>().sprite = greenLineSprite;
        }
    }

    public void IncrementLayer()
    {
        if (currentLayer < totalLayers)
        {
            currentLayer++;
            UpdateProgressBar();
        }
    }
}