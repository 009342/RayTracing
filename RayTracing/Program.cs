using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestRenderer;
namespace RayTracing
{
    class Program
    {
        static void Main(string[] args)
        {
            Renderer renderer = new Renderer(10, 2);
            object[,] pol = { { -1.0, 0, 0, -1.0, 1.0, 0, -1.0, 0, 1.0, 0xff, 0x00, 0x00, 1.0,1.0 },
                { 1.0, 0, 0, 1.0, 0.0, 1.0, 1.0, 1.0, 0, 0x00, 0x00, 0xff, 1.0,1.0 },
            { 0.0, 0, 0, 1.0, 0.0, 0.0, 0.0, 1.0, 0, 0x00, 0xff, 0x00, 1.0,1.0 }};
            renderer.AddPolygons(pol);
            object[,] lig = { { 0.0, 0.0, 5.0, 0.0, 3.0, 5.0, 3.0, 0.0, 5.0, 0xff, 0xff, 0xff, 1.0, 1.0, 100000 } };
            renderer.AddLights(lig);
            object[,] screen = { { -1.0, -1.0, 0, -1.0, -1.0, 1.0, 1.0, -1.0, 0 },
            { -1.0, -1.0, 1.0, 1.0, -1.0, 1.0, 1.0, -1.0, 0 }};

            renderer.AddScreen(screen);
            renderer.FinishEvent += Renderer_FinishEvent;
            renderer.Render();
            Thread.Sleep(-1);
        }

        private static void Renderer_FinishEvent(List<Pixel> pixels)
        {
            using (Image image = Image.FromFile("output.jpg"))
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    g.FillRectangle(Brushes.Black, 0, 0, 400, 200);
                    foreach (var item in pixels)
                    {
                        int x = (int)(item.Point.X * 200 + 200);
                        int y = (int)(item.Point.Z * 200);
                        g.FillRectangle(new SolidBrush(Color.FromArgb(item.R, item.G, item.B)), x, y, 5, 5);
                    }
                }
                image.Save("output2.jpg");
            }


        }
    }
}

namespace TestRenderer
{
    public delegate void FinishDelegate(List<Pixel> pixels);
    class Renderer
    {
        public event FinishDelegate FinishEvent;
        private int count = 0;
        private int thread = 0;
        private int threadCount = 0;
        public Renderer(int count, int thread)
        {
            this.count = count;
            this.thread = thread;
        }
        List<Polygon> polygons = new List<Polygon>();
        List<Light> lights = new List<Light>();
        public void AddPolygons(object[,] array)
        {
            for (int x = 0; x < array.GetLength(0); x++)
            {
                Polygon polygon = new Polygon();
                Point3D p1 = new Point3D();
                p1.X = Convert.ToDouble(array[x, 0]);
                p1.Y = Convert.ToDouble(array[x, 1]);
                p1.Z = Convert.ToDouble(array[x, 2]);
                Point3D p2 = new Point3D();
                p2.X = Convert.ToDouble(array[x, 3]);
                p2.Y = Convert.ToDouble(array[x, 4]);
                p2.Z = Convert.ToDouble(array[x, 5]);
                Point3D p3 = new Point3D();
                p3.X = Convert.ToDouble(array[x, 6]);
                p3.Y = Convert.ToDouble(array[x, 7]);
                p3.Z = Convert.ToDouble(array[x, 8]);
                polygon.P1 = p1;
                polygon.P2 = p2;
                polygon.P3 = p3;
                polygon.R = Convert.ToByte(array[x, 9]);
                polygon.G = Convert.ToByte(array[x, 10]);
                polygon.B = Convert.ToByte(array[x, 11]);
                polygon.Diffusion = Convert.ToDouble(array[x, 12]);
                polygon.A = Convert.ToDouble(array[x, 13]);
                polygons.Add(polygon);
            }
        }
        public void AddScreen(object[,] array)
        {
            for (int x = 0; x < array.GetLength(0); x++)
            {
                Screen screen = new Screen();
                Point3D p1 = new Point3D();
                p1.X = Convert.ToDouble(array[x, 0]);
                p1.Y = Convert.ToDouble(array[x, 1]);
                p1.Z = Convert.ToDouble(array[x, 2]);
                Point3D p2 = new Point3D();
                p2.X = Convert.ToDouble(array[x, 3]);
                p2.Y = Convert.ToDouble(array[x, 4]);
                p2.Z = Convert.ToDouble(array[x, 5]);
                Point3D p3 = new Point3D();
                p3.X = Convert.ToDouble(array[x, 6]);
                p3.Y = Convert.ToDouble(array[x, 7]);
                p3.Z = Convert.ToDouble(array[x, 8]);
                screen.P1 = p1;
                screen.P2 = p2;
                screen.P3 = p3;
                polygons.Add(screen);
            }

        }
        public void AddLights(object[,] array)
        {
            for (int x = 0; x < array.GetLength(0); x++)
            {
                Light light = new Light();
                Point3D p1 = new Point3D();
                p1.X = Convert.ToDouble(array[x, 0]);
                p1.Y = Convert.ToDouble(array[x, 1]);
                p1.Z = Convert.ToDouble(array[x, 2]);
                Point3D p2 = new Point3D();
                p2.X = Convert.ToDouble(array[x, 3]);
                p2.Y = Convert.ToDouble(array[x, 4]);
                p2.Z = Convert.ToDouble(array[x, 5]);
                Point3D p3 = new Point3D();
                p3.X = Convert.ToDouble(array[x, 6]);
                p3.Y = Convert.ToDouble(array[x, 7]);
                p3.Z = Convert.ToDouble(array[x, 8]);
                light.P1 = p1;
                light.P2 = p2;
                light.P3 = p3;
                light.R = Convert.ToByte(array[x, 9]);
                light.G = Convert.ToByte(array[x, 10]);
                light.B = Convert.ToByte(array[x, 11]);
                light.Diffusion = Convert.ToDouble(array[x, 12]);
                light.A = Convert.ToDouble(array[x, 13]);
                light.Density = Convert.ToDouble(array[x, 14]);
                lights.Add(light);
            }
        }
        public void Render()
        {
            List<RayOfLight> rays = new List<RayOfLight>();
            foreach (var l in lights)
            {
                Point3D lightVector = Light.GetOuterProduct(l);
                double area = Point3D.Absolute(lightVector) / 2;
                Point3D normalizedVector = Point3D.Normalize(lightVector);
                int lightCount = (int)Math.Round(l.Density * area);
                for (int i = 0; i < lightCount; i++)
                {
                    Point3D pointOnLight = Polygon.GetRandomPoint(l);
                    Point3D rayVector = Point3D.RandomizedVector(normalizedVector, l.Diffusion);
                    rays.Add(new RayOfLight() { start = pointOnLight, vector = rayVector, R = l.R, G = l.G, B = l.B, A = l.A });
                }

            }
            PrepareForLoop(rays, count, thread);
        }
        public void PrepareForLoop(List<RayOfLight> rays, int count, int thread = 1)
        {
            threadCount = thread;
            if (thread == 0)
            {
                List<Pixel> pixels = new List<Pixel>();
                LoopThread(rays, pixels, count, thread);
            }
            else
            {
                List<RayOfLight>[] subRays = new List<RayOfLight>[thread];
                Thread[] threads = new Thread[thread];
                for (int j = 0; j < thread; j++)
                {
                    subRays[j] = new List<RayOfLight>();
                }
                for (int i = 0; i < rays.Count; i++)
                {
                    subRays[i % thread].Add(rays[i]);
                }
                for (int k = 0; k < thread; k++)
                {
                    Console.WriteLine(k);
                    int number = k;
                    List<Pixel> pixels = new List<Pixel>();
                    threads[k] = new Thread(() => { LoopThread(subRays[number], pixels, count, number); });
                }
                foreach (Thread t in threads)
                {
                    t.Start();
                }
            }

        }
        public void LoopThread(List<RayOfLight> rays, List<Pixel> pixels, int count, int tNumber)
        {
            {
                List<RayOfLight> temp = new List<RayOfLight>(rays);
                foreach (var ray in temp)
                {

                    Point3D rayVector = ray.vector;
                    Point3D pointOnLight = ray.start;
                    double min = double.MaxValue;
                    Polygon minP = null;
                    Point3D shortestPoint = Point3D.ZeroVector;
                    foreach (var p in polygons)
                    {
                        Point3D pVector = Polygon.GetOuterProduct(p);
                        if (Point3D.InnerProduct(rayVector, pVector) < 0)
                        {
                            Point3D polygonPoint = Polygon.CrossPoint(p, pointOnLight, rayVector);
                            if (polygonPoint == Point3D.ZeroVector)
                            {

                            }
                            else
                            {
                                if (Polygon.isPointOnPolygon(p, polygonPoint))
                                {
                                    double distance = Point3D.Absolute(polygonPoint - pointOnLight);
                                    if (distance < min)
                                    {
                                        minP = p;
                                        min = distance;
                                        shortestPoint = polygonPoint;
                                    }
                                }
                            }
                        }
                    }
                    if (shortestPoint == Point3D.ZeroVector)
                    {
                        rays.Remove(ray);
                    }
                    /*else if (minP.GetType() == typeof(Screen))
                    {
                        double strength = -Point3D.InnerProduct(rayVector, Screen.DirectionVector(minP));
                        Pixel p = new Pixel() { Point = new Point3D() { X = shortestPoint.X, Y = shortestPoint.Y, Z = shortestPoint.Z }, R = (byte)Math.Round(ray.R * strength), G = (byte)Math.Round(ray.G * strength), B = (byte)Math.Round(ray.B * strength) };
                        pixels.Add(p);
                        //Console.WriteLine("{0} {1} {2} {3} {4} {5}", shortestPoint.X, shortestPoint.Y, shortestPoint.Z, ray.R * strength, ray.G * strength, ray.B * strength);
                        rays.Remove(ray);
                    }*/
                    else
                    {
                        ray.R = (byte)Math.Round((double)ray.R * ((double)minP.R / (double)255));
                        ray.G = (byte)Math.Round((double)ray.G * ((double)minP.G / (double)255));
                        ray.B = (byte)Math.Round((double)ray.B * ((double)minP.B / (double)255));
                        ray.A *= minP.A;
                        ray.start = shortestPoint;
                        Point3D n = Polygon.DirectionVector(minP);
                        Point3D vector = ray.vector - n * Point3D.InnerProduct(ray.vector, n) * 2;
                        ray.vector = Point3D.RandomizedVector(vector, minP.Diffusion);
                        minP.rays.Add(new RayOfLight(ray));
                    }
                }

            }
            if (count == 0 || rays.Count == 0)
            {
                Console.WriteLine("{0} : Thread Terminated", tNumber);
                threadCount -= 1;
                if (threadCount <= 0)
                {
                    FinishEvent(pixels);
                }
                return;
            };
            LoopThread(rays, pixels, count - 1, tNumber);
        }
    }
    /*public class Pixel
    {
        public byte R;
        public byte G;
        public byte B;
        public Point3D Point;
    }*/
    public class Point3D
    {
        static Random random = new Random();
        public const Point3D ZeroVector = null;
        public double X;
        public double Y;
        public double Z;
        public Point3D()
        {

        }
        public Point3D(Point3D p)
        {
            this.X = p.X;
            this.Y = p.Y;
            this.Z = p.Z;
        }
        public static Point3D operator +(Point3D p1, Point3D p2)
        {
            return new Point3D() { X = p1.X + p2.X, Y = p1.Y + p2.Y, Z = p1.Z + p2.Z };
        }
        public static Point3D operator -(Point3D p1, Point3D p2)
        {
            return new Point3D() { X = p1.X - p2.X, Y = p1.Y - p2.Y, Z = p1.Z - p2.Z };
        }
        public static Point3D operator /(Point3D p1, double divider)
        {
            return new Point3D() { X = p1.X / divider, Y = p1.Y / divider, Z = p1.Z / divider };
        }
        public static Point3D operator *(Point3D p1, double multiplier)
        {
            return new Point3D() { X = p1.X * multiplier, Y = p1.Y * multiplier, Z = p1.Z * multiplier };
        }
        public static Point3D Multiply(Point3D p1, Point3D p2)
        {
            return new Point3D() { X = p1.X * p2.X, Y = p1.Y * p2.Y, Z = p1.Z * p2.Z };
        }
        public static double InnerProduct(Point3D p1, Point3D p2)
        {
            return p1.X * p2.X + p1.Y * p2.Y + p1.Z * p2.Z;
        }
        public static Point3D OuterProduct(Point3D p1, Point3D p2)
        {
            return new Point3D() { X = (p1.Y * p2.Z - p2.Y * p1.Z), Y = (p2.X * p1.Z - p1.X * p2.Z), Z = (p1.X * p2.Y - p2.X * p1.Y) };
        }
        public static double Absolute(Point3D p)
        {
            return Math.Sqrt(InnerProduct(p, p));
        }
        public static Point3D Normalize(Point3D p)
        {

            double divider = Absolute(p);
            return p / divider;
        }
        public static double RandomizedDouble(double r) //Using sin function, 0<r<1 ,if r is 0, return is always 1
        {
            r /= 2;
            double c = ((double)1 - 2 * r) / Math.PI;
            double rd = random.NextDouble();
            double div = r * Math.Sin(Math.PI / 2) + c;
            return (r * Math.Sin(rd * Math.PI) + c) / div; //return some double 0<r<1
        }
        public static Point3D RandomizedVector(double r)
        {
            return Normalize(new Point3D() { X = RandomizedDouble(r), Y = RandomizedDouble(r), Z = RandomizedDouble(r) });
        }
        public static Point3D RandomizedVector(Point3D p, double r)
        {
            Point3D rp = RandomizedVector(r);
            return Normalize(new Point3D() { X = p.X - rp.X + 0.5, Y = p.Y - rp.Y + 0.5, Z = p.Z - rp.Z + 0.5 });
        }

    }
    class RayOfLight
    {
        public Point3D vector;
        public Point3D start;
        public byte R;
        public byte G;
        public byte B;
        public double A;
        public RayOfLight()
        {

        }
        public RayOfLight(RayOfLight ray)
        {
            this.vector = new Point3D(ray.vector);
            this.start = new Point3D(ray.start);
            this.R = ray.R;
            this.G = ray.G;
            this.B = ray.B;
            this.A = ray.A;
        }
    }
    class Polygon
    {
        private static Random random = new Random();
        public List<RayOfLight> rays = new List<RayOfLight>();
        public Point3D P1;
        public Point3D P2;
        public Point3D P3;
        public byte R;
        public byte G;
        public byte B;
        public double A;
        public double Diffusion;
        //[0,1] [1,0], [0,0] 삼각형의 랜덤 좌표 r<r', r, r'-r, 1-r' ex) 0.3 0.9 => [0.3, 0.6, 0.1] 
        public static Point3D GetRandomPoint(Polygon p) //확률밀도 오류 : ㅁ 을 ㅅ 으로
        {
            /*
            Point3D vector1 = p.P2 - p.P1;
            Point3D vector2 = p.P3 - p.P2;
            double abs1 = Point3D.Absolute(vector1);
            double abs2 = Point3D.Absolute(vector2);
            double rand1 = random.NextDouble();
            double rand2 = random.NextDouble();
            vector1 = vector1 * rand1;
            vector2 = vector2 * rand1 * rand2;
            return p.P1 + vector1 + vector2;
            */
            double r1 = random.NextDouble();
            double r2 = random.NextDouble();
            if (r1 > r2)
            {
                (r1, r2) = (r2, r1);
            }
            double a = r1, b = r2 - r1, c = 1 - r2;
            return new Point3D() { X = a * p.P1.X + b * p.P2.X + c * p.P3.X, Y = a * p.P1.Y + b * p.P2.Y + c * p.P3.Y, Z = a * p.P1.Z + b * p.P2.Z + c * p.P3.Z };
        }
        public static Point3D CrossPoint(Polygon p, Point3D start, Point3D pVector) //vector must be normalized
        {
            //Polygon : ax+by+cz+d = 0, a,b,c direction form polygon d from 
            //x = a' * t +start
            Point3D dVector = DirectionVector(p);
            if (Point3D.InnerProduct(dVector, pVector) == 0) return Point3D.ZeroVector;
            double u = (Point3D.InnerProduct(dVector, p.P1 - start)) /
                Point3D.InnerProduct(dVector, pVector);
            return start + pVector * u;
        }
        public static Point3D DirectionVector(Polygon p)
        {
            return Point3D.Normalize(Point3D.OuterProduct(p.P2 - p.P1, p.P3 - p.P2));
        }
        public static Point3D GetOuterProduct(Polygon p)
        {
            return Point3D.OuterProduct(p.P2 - p.P1, p.P3 - p.P2);
        }
        public static bool isPointOnPolygon(Polygon p, Point3D point)
        {
            Point3D p1 = Point3D.OuterProduct(p.P2 - p.P1, point - p.P1);
            Point3D p2 = Point3D.OuterProduct(p.P3 - p.P2, point - p.P2);
            Point3D p3 = Point3D.OuterProduct(p.P1 - p.P3, point - p.P3);
            double a = Point3D.InnerProduct(p1, p2);
            double b = Point3D.InnerProduct(p2, p3);
            double c = Point3D.InnerProduct(p1, p3);

            if (a >= 0 && b >= 0 && c >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    class Light : Polygon
    {
        public double Density;
    }
    class Screen : Polygon
    {

    }
    public class Camera
    {
        public Point3D P1;
        public Point3D P2;
        public Point3D Focus;
    }
}

