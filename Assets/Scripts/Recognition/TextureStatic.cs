using UnityEngine;

namespace Recognition
{
    public static class TextureStatic
    {
        /// <summary>
        /// Рисует линию с антиалиасингом в массиве Color[]
        /// </summary>
        /// <param name="pixels">Массив пикселей (размер width * height)</param>
        /// <param name="width">Ширина текстуры</param>
        /// <param name="height">Высота текстуры</param>
        /// <param name="x0">Начало X</param>
        /// <param name="y0">Начало Y</param>
        /// <param name="x1">Конец X</param>
        /// <param name="y1">Конец Y</param>
        /// <param name="color">Цвет линии</param>
        /// <param name="thickness">Толщина линии (обычно 1.0 для стандартной линии)</param>
        public static void DrawLine(Color[] pixels, int width, int height,
            float x0, float y0, float x1, float y1, Color color, float thickness = 1.0f)
        {
            // Переставляем точки, чтобы линия была более горизонтальной
            bool steep = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);

            if (steep)
            {
                (x0, y0) = (y0, x0);
                (x1, y1) = (y1, x1);
            }

            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }

            float dx = x1 - x0;
            float dy = y1 - y0;
            float gradient = (dx == 0) ? 1 : dy / dx;

            // Обработка первой точки
            float xEnd = Mathf.Round(x0);
            float yEnd = y0 + gradient * (xEnd - x0);
            float xGap = 1 - (x0 + 0.5f - Mathf.Floor(x0 + 0.5f));

            float xpxl1 = xEnd;
            float ypxl1 = Mathf.Floor(yEnd);

            if (steep)
            {
                Plot(pixels, width, height, ypxl1, xpxl1,
                    color, (1 - (yEnd - ypxl1)) * xGap * thickness);
                Plot(pixels, width, height, ypxl1 + 1, xpxl1,
                    color, (yEnd - ypxl1) * xGap * thickness);
            }
            else
            {
                Plot(pixels, width, height, xpxl1, ypxl1,
                    color, (1 - (yEnd - ypxl1)) * xGap * thickness);
                Plot(pixels, width, height, xpxl1, ypxl1 + 1,
                    color, (yEnd - ypxl1) * xGap * thickness);
            }

            float intery = yEnd + gradient;

            // Обработка второй точки
            xEnd = Mathf.Round(x1);
            yEnd = y1 + gradient * (xEnd - x1);
            xGap = x1 + 0.5f - Mathf.Floor(x1 + 0.5f);

            float xpxl2 = xEnd;
            float ypxl2 = Mathf.Floor(yEnd);

            if (steep)
            {
                Plot(pixels, width, height, ypxl2, xpxl2,
                    color, (1 - (yEnd - ypxl2)) * xGap * thickness);
                Plot(pixels, width, height, ypxl2 + 1, xpxl2,
                    color, (yEnd - ypxl2) * xGap * thickness);
            }
            else
            {
                Plot(pixels, width, height, xpxl2, ypxl2,
                    color, (1 - (yEnd - ypxl2)) * xGap * thickness);
                Plot(pixels, width, height, xpxl2, ypxl2 + 1,
                    color, (yEnd - ypxl2) * xGap * thickness);
            }

            // Основной цикл
            for (float x = xpxl1 + 1; x <= xpxl2 - 1; x++)
            {
                if (steep)
                {
                    Plot(pixels, width, height, Mathf.Floor(intery), x,
                        color, (1 - (intery - Mathf.Floor(intery))) * thickness);
                    Plot(pixels, width, height, Mathf.Floor(intery) + 1, x,
                        color, (intery - Mathf.Floor(intery)) * thickness);
                }
                else
                {
                    Plot(pixels, width, height, x, Mathf.Floor(intery),
                        color, (1 - (intery - Mathf.Floor(intery))) * thickness);
                    Plot(pixels, width, height, x, Mathf.Floor(intery) + 1,
                        color, (intery - Mathf.Floor(intery)) * thickness);
                }

                intery += gradient;
            }
        }

        /// <summary>
        /// Рисует линию с переменной толщиной
        /// </summary>
        public static void DrawLineVariableThickness(Color[] pixels, int width, int height,
            float x0, float y0, float x1, float y1,
            Color color, float startThickness, float endThickness)
        {
            // Вычисляем длину линии
            float length = Mathf.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0));

            // Переставляем точки, чтобы линия была более горизонтальной
            bool steep = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);

            if (steep)
            {
                (x0, y0) = (y0, x0);
                (x1, y1) = (y1, x1);
            }

            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
                // Меняем местами толщины при изменении направления
                (startThickness, endThickness) = (endThickness, startThickness);
            }

            float dx = x1 - x0;
            float dy = y1 - y0;
            float gradient = (dx == 0) ? 1 : dy / dx;

            // Обработка первой точки
            float xEnd = Mathf.Round(x0);
            float yEnd = y0 + gradient * (xEnd - x0);
            float xGap = 1 - (x0 + 0.5f - Mathf.Floor(x0 + 0.5f));

            float xpxl1 = xEnd;
            float ypxl1 = Mathf.Floor(yEnd);

            float t0 = 0; // Параметр для интерполяции толщины

            if (steep)
            {
                Plot(pixels, width, height, ypxl1, xpxl1,
                    color, (1 - (yEnd - ypxl1)) * xGap * GetThickness(t0, startThickness, endThickness));
                Plot(pixels, width, height, ypxl1 + 1, xpxl1,
                    color, (yEnd - ypxl1) * xGap * GetThickness(t0, startThickness, endThickness));
            }
            else
            {
                Plot(pixels, width, height, xpxl1, ypxl1,
                    color, (1 - (yEnd - ypxl1)) * xGap * GetThickness(t0, startThickness, endThickness));
                Plot(pixels, width, height, xpxl1, ypxl1 + 1,
                    color, (yEnd - ypxl1) * xGap * GetThickness(t0, startThickness, endThickness));
            }

            float intery = yEnd + gradient;

            // Обработка второй точки
            xEnd = Mathf.Round(x1);
            yEnd = y1 + gradient * (xEnd - x1);
            xGap = x1 + 0.5f - Mathf.Floor(x1 + 0.5f);

            float xpxl2 = xEnd;
            float ypxl2 = Mathf.Floor(yEnd);

            float t1 = 1; // Параметр для интерполяции толщины

            if (steep)
            {
                Plot(pixels, width, height, ypxl2, xpxl2,
                    color, (1 - (yEnd - ypxl2)) * xGap * GetThickness(t1, startThickness, endThickness));
                Plot(pixels, width, height, ypxl2 + 1, xpxl2,
                    color, (yEnd - ypxl2) * xGap * GetThickness(t1, startThickness, endThickness));
            }
            else
            {
                Plot(pixels, width, height, xpxl2, ypxl2,
                    color, (1 - (yEnd - ypxl2)) * xGap * GetThickness(t1, startThickness, endThickness));
                Plot(pixels, width, height, xpxl2, ypxl2 + 1,
                    color, (yEnd - ypxl2) * xGap * GetThickness(t1, startThickness, endThickness));
            }

            // Основной цикл с переменной толщиной
            float step = 0;
            float totalSteps = xpxl2 - xpxl1 - 1;

            for (float x = xpxl1 + 1; x <= xpxl2 - 1; x++)
            {
                step++;
                float t = step / totalSteps; // Параметр от 0 до 1 для интерполяции толщины
                float currentThickness = GetThickness(t, startThickness, endThickness);

                if (steep)
                {
                    Plot(pixels, width, height, Mathf.Floor(intery), x,
                        color, (1 - (intery - Mathf.Floor(intery))) * currentThickness);
                    Plot(pixels, width, height, Mathf.Floor(intery) + 1, x,
                        color, (intery - Mathf.Floor(intery)) * currentThickness);
                }
                else
                {
                    Plot(pixels, width, height, x, Mathf.Floor(intery),
                        color, (1 - (intery - Mathf.Floor(intery))) * currentThickness);
                    Plot(pixels, width, height, x, Mathf.Floor(intery) + 1,
                        color, (intery - Mathf.Floor(intery)) * currentThickness);
                }

                intery += gradient;
            }
        }

        /// <summary>
        /// Быстрая версия для линий толщиной 1 (оптимизирована)
        /// </summary>
        public static void DrawLineFast(Color[] pixels, int width, int height,
            int x0, int y0, int x1, int y1, Color color)
        {
            // Целочисленный алгоритм Брезенхема для быстрых линий
            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                SetPixel(pixels, width, height, x0, y0, color);

                if (x0 == x1 && y0 == y1) break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }

                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        // Вспомогательные методы

        private static void Plot(Color[] pixels, int width, int height,
            float x, float y, Color color, float intensity)
        {
            int ix = Mathf.RoundToInt(x);
            int iy = Mathf.RoundToInt(y);

            if (ix >= 0 && ix < width && iy >= 0 && iy < height)
            {
                int index = iy * width + ix;

                // Ограничиваем интенсивность
                intensity = Mathf.Clamp01(intensity);

                // Смешиваем с существующим цветом
                pixels[index] = Color.Lerp(pixels[index], color, intensity);
            }
        }

        private static void SetPixel(Color[] pixels, int width, int height,
            int x, int y, Color color)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                pixels[y * width + x] = color;
            }
        }

        private static float GetThickness(float t, float start, float end)
        {
            return Mathf.Lerp(start, end, t);
        }

    }
}