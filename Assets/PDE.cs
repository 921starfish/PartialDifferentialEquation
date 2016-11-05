using UnityEngine;
using System.Collections;

public class PDE : MonoBehaviour
{

    static double[,,] log;
    static bool isDiscrete = true;
    int count;
    double X, Y, W, H;

    // Use this for initialization
    void Start()
    {
        int i, j;
        double dt = 0.001;
        double dx = 0.01;
        double kappa = 0.0023473150395; // 鉄の熱伝達係数 m^2/s
        double lambda = kappa * dt / dx / dx;
        Debug.Log(" alpha = " + lambda);
        int height = (int)(0.6 / dx) + 1; // 格子点の数 (長さ60cm)
        int width = (int)(0.4 / dx) + 1; // 格子点の数 (幅40cm)
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
    }

    // Update is called once per frame
    void Update()
    {

    }
}
