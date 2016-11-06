using UnityEngine;
using System.Collections;

public class PDE : MonoBehaviour
{

    static double[,,] log;
    public bool isDiscrete = false;
    int count, height, width;
    float X, Y, W, H;
    public GameObject Rectangle;
    public GameObject[,] Rects;
    [Range(0, 50)]
    public int Zikoku = 0;
    private float timeleft = 5;

    // Use this for initialization
    void Start()
    {
        int i, j;
        double dt = 0.001;
        double dx = 0.01;
        double kappa = 0.0023473150395; // 鉄の熱伝達係数 m^2/s
        double lambda = kappa * dt / dx / dx;
        Debug.Log(" alpha = " + lambda);
        height = (int)(0.6 / dx) + 1; // 格子点の数 (長さ60cm)
        width = (int)(0.4 / dx) + 1; // 格子点の数 (幅40cm)
        int m = 50000; // 時間格子の数
        double[,] T = new double[height, width];
        double[,] TT = new double[height, width];
        log = new double[(int)((m / 1000d) + 1), height, width];
        // 初期条件の設定*********************
        for (i = 0; i < height; i++)
        {
            for (j = 0; j < width; j++)
            {
                if ((i - height / 2) * (i - height / 2) + (j - width / 2) * (j - width / 2) <= (0.1 / dx + 0.5)
                        * (0.1 / dx + 0.5)
                        && (i - height / 2) * (i - height / 2) + (j - width / 2) * (j - width / 2) >= (0.1 / dx - 0.5)
                                * (0.1 / dx - 0.5))
                {
                    T[i, j] = 773.0;
                }
                else
                {
                    T[i, j] = 300.0;
                }
            }
        }

        int nn = 0;
        // 初期値の格納
        for (i = 0; i < height; i++)
        {
            for (j = 0; j < width; j++)
            {
                log[nn, i, j] = T[i, j] - 273;
            }
        }
        // 内部温度の計算
        while (nn <= m)
        {
            nn++;
            for (i = 1; i < (height - 1); i++)
            {
                for (j = 1; j < (width - 1); j++)
                {
                    TT[i, j] = lambda * (T[i - 1, j] + T[i, j - 1] + T[i + 1, j] + T[i, j + 1])
                            + (1 - 2 * lambda - 2 * lambda) * T[i, j];
                }
            }
            for (i = 1; i < (height - 1); i++)
            {
                for (j = 1; j < (width - 1); j++)
                {
                    T[i, j] = TT[i, j];
                }
            }
            for (i = 0; i < height; i++)
            {// 火源はディリクレ条件とする
                for (j = 0; j < width; j++)
                {
                    if ((i - height / 2) * (i - height / 2) + (j - width / 2) * (j - width / 2) <= (0.1 / dx + 0.5)
                            * (0.1 / dx + 0.5)
                            && (i - height / 2) * (i - height / 2)
                                    + (j - width / 2) * (j - width / 2) >= (0.1 / dx - 0.5) * (0.1 / dx - 0.5))
                    {
                        T[i, j] = 773.0;
                    }
                }
            }
            for (i = 0; i < height; i++)
            {// 境界はノイマン条件とする 左右
                T[i, 0] = T[i, 1];
                T[i, width - 1] = T[i, width - 2];
            }

            for (j = 0; j < width; j++)
            {// 境界はノイマン条件とする 上下
                T[0, j] = T[1, j];
                T[height - 1, j] = T[height - 2, j];
            }

            // 計算結果の格納
            if ((nn) % 1000 == 0)
            {
                for (i = 0; i < height; i++)
                {
                    for (j = 0; j < width; j++)
                    {
                        log[(nn) / 1000, i, j] = T[i, j] - 273;
                    }
                }
            }
        }
        makeGraph();
        setColorToGraph(0);
    }

    // Update is called once per frame
    void Update()
    {
        timeleft -= Time.deltaTime;
        if (timeleft <= 0.0)
        {
            timeleft = 1.0f;
            if (Zikoku < 50) { Zikoku++; }
            if (Zikoku == 50) { Zikoku = 0; }
            setColorToGraph(Zikoku);
        }

    }

    // グラフ描画関数
    public void makeGraph()
    {
        X = 0;
        Y = 0;
        W = width;
        H = height;

        Rects = new GameObject[height, width];

        float h = (H - Y) / log.GetLength(1);
        float w = (W - X) / log.GetLength(2);

        for (int j = 0; j < log.GetLength(1); j++)
        {
            for (int k = 0; k < log.GetLength(2); k++)
            {
                Rects[j, k] = Instantiate(Rectangle);
                Rects[j, k].transform.localScale = new Vector3(w, h, 0);
                Rects[j, k].transform.position += new Vector3((float)(X + w * k), (float)(Y + h * j), 0);
            }
        }
    }

    // グラフに色を付ける関数
    public void setColorToGraph(int i)
    {
        for (int j = 0; j < log.GetLength(1); j++)
        {
            for (int k = 0; k < log.GetLength(2); k++)
            {
                Rects[j, k].GetComponent<SpriteRenderer>().color = makeColor(log[i, j, k]);
            }
        }
    }

    // 温度を色に変える関数
    public Color makeColor(double t)
    {
        if (!isDiscrete)
        {
            return Color.HSVToRGB((float)(1 - t / 500) * (5f / 9f), 1.0f, 1.0f);
        }

        if (0 < t && t <= 50)
        {
            return Color.HSVToRGB((float)(1 - 50f / 500) * (5f / 9f), 1.0f, 1.0f);
            // return new Color32(186, 0, 255, 255);
        }
        else if (50 < t && t <= 100)
        {
            return Color.HSVToRGB((float)(1 - 100f / 500) * (5f / 9f), 1.0f, 1.0f);
            // return new Color32(138, 43, 226, 255);
        }
        else if (100 < t && t <= 150)
        {
            return Color.HSVToRGB((float)(1 - 150f / 500) * (5f / 9f), 1.0f, 1.0f);
            // return Color.blue;
        }
        else if (150 < t && t <= 200)
        {
            return Color.HSVToRGB((float)(1 - 200f / 500) * (5f / 9f), 1.0f, 1.0f);
            // return Color.cyan;
        }
        else if (200 < t && t <= 250)
        {
            return Color.HSVToRGB((float)(1 - 250f / 500) * (5f / 9f), 1.0f, 1.0f);
            // return new Color32(0, 255, 0, 255);
        }
        else if (250 < t && t <= 300)
        {
            return Color.HSVToRGB((float)(1 - 300f / 500) * (5f / 9f), 1.0f, 1.0f);
            // return new Color32(124, 252, 0, 255);
        }
        else if (300 < t && t <= 350)
        {
            return Color.HSVToRGB((float)(1 - 350f / 500) * (5f / 9f), 1.0f, 1.0f);
            // return new Color32(173, 255, 47, 255);
        }
        else if (350 < t && t <= 400)
        {
            return Color.HSVToRGB((float)(1 - 400f / 500) * (5f / 9f), 1.0f, 1.0f);
            // return Color.yellow;
        }
        else if (400 < t && t <= 450)
        {
            return Color.HSVToRGB((float)(1 - 450f / 500) * (5f / 9f), 1.0f, 1.0f);
            // return new Color32(255, 165, 0, 255);
        }
        else if (450 < t && t <= 500)
        {
            return Color.HSVToRGB((float)(1 - 500f / 500) * (5f / 9f), 1.0f, 1.0f);
            // return Color.red;
        }
        else
        {
            return Color.clear;
        }
    }
}
