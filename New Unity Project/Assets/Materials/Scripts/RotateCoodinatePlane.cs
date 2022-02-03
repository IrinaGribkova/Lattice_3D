using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TMPro;
using Unity.VisualScripting;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Application = UnityEngine.Application;
using UnityEngine.EventSystems;

public class RotateCoodinatePlane : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenFileName
    {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;
        public String filter = null;
        public String customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;
        public String file = null;
        public int maxFile = 0;
        public String fileTitle = null;
        public int maxFileTitle = 0;
        public String initialDir = null;
        public String title = null;
        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;
        public String defExt = null;
        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;
        public String templateName = null;
        public IntPtr reservedPtr = IntPtr.Zero;
        public int reservedInt = 0;
        public int flagsEx = 0;
        public OpenFileName(int FileLenth = 256, int FileTitleLenth = 64)
        {
            structSize = Marshal.SizeOf(this);
            file = new string(new char[FileLenth]);
            maxFile = file.Length;
            fileTitle = new string(new char[FileTitleLenth]);
            maxFileTitle = fileTitle.Length;
            title = String.Empty;
            flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
            title = "Заголовок окна";
            initialDir = Application.streamingAssetsPath.Replace('/', '\\');
        }
    }

    public class LocalDialog
    {
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);
    }

    class Point3D
    {
        public float x;
        public float y;
        public float z;
        private float diametr;
        public GameObject point;
        public Material color;
        public Point3D(float x1, float y1, float z1, float diametr1, Material col)
        {
            x = x1;
            y = y1;
            z = z1;
            color = col;
            diametr = diametr1;
            point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.transform.position = new Vector3(x, y, z);
            point.transform.localScale = new Vector3(diametr, diametr, diametr);
            point.GetComponent<MeshRenderer>().material = color;
        }

        public static Vector3 operator -(Point3D p1, Point3D p2)
        {
            return new Vector3(p1.x - p2.x, p1.y - p2.y, p1.y - p2.y);
        }

        public static Vector3 operator +(Point3D p1, Point3D p2)
        {
            return new Vector3(p1.x + p2.x, p1.y + p2.y, p1.y + p2.y);
        }

        public static Vector3 operator *(Point3D p1, Point3D p2)
        {
            return new Vector3(p1.x * p2.x, p1.y * p2.y, p1.y * p2.y);
        }

        public static Vector3 operator /(Point3D p1, Point3D p2)
        {
            return new Vector3(p1.x / p2.x, p1.y / p2.y, p1.y / p2.y);
        }
    }

    class Vector3D
    {
        public Vector3 first_point;
        public Vector3 second_point;
        public float thickness;
        public GameObject vector3D;
        public Material color;
        public Vector3 vector;

        public Vector3D(Vector3 p1, Vector3 p2, float thick, Material col)
        {
            first_point = p1;
            second_point = p2;
            thickness = thick;
            color = col;
            vector = new Vector3(second_point.x - first_point.x, second_point.y - first_point.y, second_point.z - first_point.z);
            vector3D = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            vector3D.transform.position = GetCenter(p1, p2);
            vector3D.GetComponent<Renderer>().material = color;
            vector3D.transform.localScale = new Vector3(thickness, Vector3.Distance(first_point, second_point) / 2, thickness);
            vector3D.transform.rotation = Quaternion.LookRotation((first_point - second_point).normalized) * Quaternion.Euler(Vector3.right * 90f);
        }

        public Vector3 GetCenter(Vector3 p1, Vector3 p2)
        {
            return new Vector3((p1.x + p2.x) / 2, (p1.y + p2.y) / 2, (p1.z + p2.z) / 2);
        }
        public float GetLength(Vector3 v1, Vector3 v2)
        {
            return Mathf.Sqrt(Mathf.Pow(v2.x - v1.x, 2) + Mathf.Pow(v2.y - v1.y, 2) + Mathf.Pow(v2.z - v1.z, 2));
        }
    }

    class Vector_n
    {
        public List<double> vector = new List<double>();

        public Vector_n(List<double> v)
        {
            vector = v;
        }

        public static Vector_n operator /(Vector_n vectorX, Vector_n vectorY)
        {
            List<double> newArr = new List<double>();
            for (var i = 0; i < vectorX.Count; i++)
            {
                newArr.Add(vectorX[i] / vectorY[i]);
            }
            return new Vector_n(newArr);
        }
        public IEnumerator<double> GetEnumerator(int n)
        {
            foreach (var el in vector)
                yield return el;
        }
        public static double operator *(Vector_n vectorX, Vector_n vectorY)
        {
            List<double> newArr = new List<double>();
            try
            {
                if (vectorX.Count != vectorY.Count)
                {
                    throw new Exception("Vectors have different sizes!");
                }
                for (var i = 0; i < vectorX.Count; i++)
                {
                    newArr.Add(vectorX[i] * vectorY[i]);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка: {e.Message}");
            }
            return newArr.Sum();
        }

        public static Vector_n operator +(Vector_n vectorX, Vector_n vectorY)
        {
            List<double> newArr = new List<double>();
            if (vectorX.Count == vectorY.Count)
            {
                for (var i = 0; i < vectorX.Count; i++)
                {
                    if (i >= vectorY.Count)
                        break;
                    newArr.Add(vectorX[i] + vectorY[i]);
                }
            }
            else throw new Exception("Vectors have different sizes!");
            return new Vector_n(newArr);
        }
        public static Vector_n operator -(Vector_n vectorX, Vector_n vectorY)
        {
            List<double> newArr = new List<double>();
            if (vectorX.vector.Count == vectorY.vector.Count)
            {
                for (var i = 0; i < vectorX.vector.Count; i++)
                {
                    newArr.Add(vectorX.vector[i] - vectorY.vector[i]);
                }
                return new Vector_n(newArr);
            }
            else throw new Exception("Vectors have different sizes!");

        }
        public static Vector_n operator *(double k, Vector_n vectorX)
        {
            List<double> newArr = new List<double>();
            for (var i = 0; i < vectorX.Count; i++)
            {
                newArr.Add(vectorX[i] * k);
            }
            return new Vector_n(newArr);
        }

        public static Vector_n operator /(Vector_n vectorX, double k)
        {
            List<double> newArr = new List<double>();
            for (var i = 0; i < vectorX.Count; i++)
            {
                newArr.Add(vectorX.vector[i] / k);
            }
            return new Vector_n(newArr);
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(vector.Select(x => x * x).Sum());
            }
        }
        public int Count
        {
            get
            {
                return vector.Count();
            }
        }
        public double this[int i]
        {
            get { return vector[i]; }
            set { vector[i] = value; }
        }
        public static Vector_n operator *(List<Vector_n> vec, Vector_n coof)
        {
            List<Vector_n> newArr = new List<Vector_n>();
            Vector_n res = new Vector_n(new List<double>());
            for (var i = 0; i < vec.Count; i++)
            {
                if (i == 0)
                    res = coof[i] * vec[i];
                else res += coof[i] * vec[i];
            }
            return res;
        }
        public override string ToString()
        {
            string str = "{";
            foreach (var v in vector)
                str += v.ToString() + ", ";
            str = str.Remove(str.Length - 1);
            str = str.Remove(str.Length - 1);
            str += "}";
            return str;
        }
        public List<double> ToList()
        {
            List<double> coordinates = new List<double>();
            foreach (var v in vector)
                coordinates.Add(v);

            return coordinates;
        }
        public double[] ToArray()
        {
            double[] coordinates = new double[vector.Count];
            for (int i = 0; i < coordinates.Count(); i++)
            {
                coordinates[i] = vector[i];
            }
            return coordinates;
        }
    }

    class Point_n
    {
        public List<double> point = new List<double>();

        public Point_n(List<double> v)
        {
            point = v;
        }
        public int Count
        {
            get
            {
                return point.Count();
            }
        }
        public override string ToString()
        {
            string str = "{";
            foreach (var p in point)
                str += p.ToString() + ", ";
            str = str.Remove(str.Length - 1);
            str = str.Remove(str.Length - 1);
            str += "}";
            return str;
        }
        public double[] ToArray()
        {
            double[] coordinates = new double[point.Count];
            for (int i = 0; i < point.Count(); i++)
            {
                coordinates[i] = point[i];
            }
            return coordinates;
        }
        public double this[int i]
        {
            get { return point[i]; }
            set { point[i] = value; }
        }

        public static Point_n operator -(Vector_n vectorX, Point_n point)
        {
            List<double> newArr = new List<double>();
            if (vectorX.Count == point.Count)
            {
                for (var i = 0; i < vectorX.Count; i++)
                {
                    newArr.Add(vectorX[i] - point[i]);
                }
                return new Point_n(newArr);
            }
            else throw new Exception("Vectors have different sizes!");
        }
        public static Point_n operator -(Point_n point, Vector_n vectorX)
        {
            List<double> newArr = new List<double>();
            if (vectorX.Count == point.Count)
            {
                for (var i = 0; i < vectorX.vector.Count; i++)
                {
                    newArr.Add(point[i] - vectorX[i]);
                }
                return new Point_n(newArr);
            }
            else throw new Exception("Vectors have different sizes!");
        }
        public static Point_n operator +(Point_n point, Vector_n vectorX)
        {
            List<double> newArr = new List<double>();
            if (vectorX.Count == point.Count)
            {
                for (var i = 0; i < vectorX.Count; i++)
                {
                    newArr.Add(point[i] + vectorX[i]);
                }
                return new Point_n(newArr);
            }
            else throw new Exception("Vectors have different sizes!");
        }
        public static double operator *(Vector_n vectorX, Point_n point)
        {
            List<double> newArr = new List<double>();
            if (vectorX.Count == point.Count)
            {
                for (var i = 0; i < vectorX.vector.Count; i++)
                {
                    newArr.Add(vectorX[i] * point[i]);
                }
                return newArr.Sum();
            }
            else throw new Exception("Vectors have different sizes!");
        }
    }

    class LLL
    {
        List<Vector_n> vectors = new List<Vector_n>();

        public LLL(List<Vector_n> v)
        {
            vectors = v;
        }

        public Dictionary<double[,], List<Vector_n>> Gram_Schmidt(List<Vector_n> v)
        {
            List<Vector_n> ortho_vectors = new List<Vector_n>();
            double[,] coef = new double[v.Count, v.Count];
            for (int i = 0; i < v.Count; i++)
            {
                for (int j = 0; j < v.Count; j++)
                {
                    coef[i, j] = 0;
                }
            }
            ortho_vectors.Add(v[0]);
            for (int i = 1; i < v.Count; i++)
            {
                Vector_n orh_vec = new Vector_n(v[i].vector);

                for (int j = 0; j < i; j++)
                {
                    Dictionary<double, Vector_n> res = new Dictionary<double, Vector_n>();
                    res = Proj_r(ortho_vectors[j], v[i]);
                    orh_vec -= res.ElementAt(0).Value;
                    coef[i, j] = res.ElementAt(0).Key;
                }
                ortho_vectors.Add(orh_vec);
            }
            return new Dictionary<double[,], List<Vector_n>>() { { coef, ortho_vectors } };
        }
        public IEnumerator<Vector_n> GetEnumerator()
        {
            foreach (var el in vectors)
                yield return el;
        }
        public Vector_n this[int i]
        {
            get { return vectors[i]; }
            set { vectors[i] = value; }
        }

        public Vector_n Sum_Vectors(Vector_n[] vctrs)
        {
            Vector_n res = vctrs[0];
            for (int i = 1; i < vctrs.Length; i++)
                res += vctrs[i];
            return res;
        }
        public List<List<Vector_n>> prob_LLL_alg(List<Vector_n> basis, bool flag)
        {
            List<List<Vector_n>> full_collecion = new List<List<Vector_n>>() { new List<Vector_n>() };// список состояний
            Dictionary<double[,], List<Vector_n>> gr_sch = new Dictionary<double[,], List<Vector_n>>();
            double[,] coef_Gram_Schmidt = new LLL(basis).Gram_Schmidt(basis).ElementAt(0).Key;
            List<Vector_n> ortho_vectors = new LLL(basis).Gram_Schmidt(basis).ElementAt(0).Value;
            int k = 1;
            double δ = 0.75;
            while (k < basis.Count)
            {          
                if (Math.Abs(coef_Gram_Schmidt[k, k - 1]) > 0.5)
                {
                    basis[k] -= Closest_integer(Math.Abs(coef_Gram_Schmidt[k, k - 1])) * basis[k - 1];
                    gr_sch = new LLL(basis).Gram_Schmidt(basis);
                    coef_Gram_Schmidt = gr_sch.ElementAt(0).Key;
                    ortho_vectors = gr_sch.ElementAt(0).Value;
                    full_collecion[0].Add(basis[0]);
                    full_collecion[0].Add(basis[1]);
                    full_collecion[0].Add(basis[2]);
                }                
                if (ortho_vectors[k].Length < (δ - Math.Pow(coef_Gram_Schmidt[k, k - 1], 2)) * ortho_vectors[k - 1].Length)
                {
                    Swap(basis[k], basis[k - 1], ref basis);
                    gr_sch = new LLL(basis).Gram_Schmidt(basis);
                    coef_Gram_Schmidt = gr_sch.ElementAt(0).Key;
                    ortho_vectors = gr_sch.ElementAt(0).Value;
                    full_collecion[0].Add(basis[0]);
                    full_collecion[0].Add(basis[1]);
                    full_collecion[0].Add(basis[2]);
                    k = Math.Max(k - 1, 1);
                }
                else
                {
                    for (int j = k - 2; j > -1; j--)
                    {
                        if (Math.Abs(coef_Gram_Schmidt[k, j]) > 0.5)
                        {
                            basis[k] -= Closest_integer(Math.Abs(coef_Gram_Schmidt[k, j])) * basis[j];
                            gr_sch = new LLL(basis).Gram_Schmidt(basis);
                            coef_Gram_Schmidt = gr_sch.ElementAt(0).Key;
                            ortho_vectors = gr_sch.ElementAt(0).Value;
                            full_collecion[0].Add(basis[0]);
                            full_collecion[0].Add(basis[1]);
                            full_collecion[0].Add(basis[2]);
                        }
                    }
                    k++;
                }
            }
            if (flag is false)
                return new List<List<Vector_n>>() { basis, ortho_vectors };
            else return full_collecion;
        }

        public string LLL_alg_str(List<Vector_n> basis)
        {
            string result = "";
            List<List<Vector_n>> full_collecion = new List<List<Vector_n>>() { new List<Vector_n>() };// список состояний
            Dictionary<double[,], List<Vector_n>> gr_sch = new Dictionary<double[,], List<Vector_n>>();
            double[,] coef_Gram_Schmidt = new LLL(basis).Gram_Schmidt(basis).ElementAt(0).Key;
            List<Vector_n> ortho_vectors = new LLL(basis).Gram_Schmidt(basis).ElementAt(0).Value;

            result += "Basis after Gram-Schmidt algorithm:" + "\n"; ;
            result += "Gram_Schmidt's coefficients:" + "\n";
            foreach (var v in coef_Gram_Schmidt)
                result += v.ToString() + "\n";
            result += "Orthogonal vectors:" + "\n";
            foreach (var v in ortho_vectors)
                result += v.ToString() + "\n";
            result += "δ = 0.75" + "\n";
            int k = 1;
            double δ = 0.75;
            while (k < basis.Count)
            {               
                if (Math.Abs(coef_Gram_Schmidt[k, k - 1]) > 0.5)
                {
                    result += "\n" + Math.Abs(coef_Gram_Schmidt[k, k - 1]).ToString() + "> 0.5" + "\n";
                    result += basis[k].ToString() + "-= " + (Closest_integer(Math.Abs(coef_Gram_Schmidt[k, k - 1])) * basis[k - 1]).ToString() + "\n";
                    basis[k] -= Closest_integer(coef_Gram_Schmidt[k, k - 1]) * basis[k - 1];
                    gr_sch = new LLL(basis).Gram_Schmidt(basis);
                    result += "Calculating new Gram_Schmidt's coefficients and orthogonal vectors" + "\n";
                    coef_Gram_Schmidt = gr_sch.ElementAt(0).Key;
                    ortho_vectors = gr_sch.ElementAt(0).Value; ;
                    for (int i = 0; i < coef_Gram_Schmidt.GetLength(0); i++)
                    {
                        for (int y = 0; y < coef_Gram_Schmidt.GetLength(0); y++)
                            result += coef_Gram_Schmidt[i, y].ToString() + " ";
                        result += "\n";
                    }
                    result += "Orthogonal vectors:" + "\n";
                    foreach (var v in ortho_vectors)
                        result += v.ToString() + "\n";
                }
                result += "\n" + k.ToString() + "<=" + (basis.Count - 1).ToString() + "\n";
                if (Math.Pow(ortho_vectors[k].Length, 2) < (δ - Math.Pow(coef_Gram_Schmidt[k, k - 1], 2)) * Math.Pow(ortho_vectors[k - 1].Length, 2))
                {
                    result += ortho_vectors[k].Length.ToString() + "<" + ((δ - Math.Pow(coef_Gram_Schmidt[k, k - 1], 2)) * ortho_vectors[k - 1].Length).ToString() + "\n";
                    result += "Swap " + basis[k].ToString() + " and " + basis[k - 1].ToString() + "\n";
                    Swap(basis[k], basis[k - 1], ref basis);
                    gr_sch = new LLL(basis).Gram_Schmidt(basis);
                    result += "Calculating new Gram_Schmidt's coefficients and orthogonal vectors" + "\n";
                    coef_Gram_Schmidt = gr_sch.ElementAt(0).Key;
                    ortho_vectors = gr_sch.ElementAt(0).Value;
                    for (int i = 0; i < coef_Gram_Schmidt.GetLength(0); i++)
                    {
                        for (int y = 0; y < coef_Gram_Schmidt.GetLength(0); y++)
                            result += coef_Gram_Schmidt[i, y].ToString() + " ";
                        result += "\n";
                    }
                    result += "Orthogonal vectors:" + "\n";
                    foreach (var v in ortho_vectors)
                        result += v.ToString() + "\n";
                    result += "k = Math.Max(" + (k - 1).ToString() + ",1) is " + (Math.Max(k - 1, 1)).ToString() + "\n";
                    k = Math.Max(k - 1, 1);
                }
                else
                {
                    for (int j = k - 2; j > -1; j--)
                    {
                        if (Math.Abs(coef_Gram_Schmidt[k, j]) > 0.5)
                        {
                            result += "\n" + Math.Abs(coef_Gram_Schmidt[k, j]).ToString() + "> 0.5" + "\n";
                            result += basis[k].ToString() + "-= " + (Closest_integer(Math.Abs(coef_Gram_Schmidt[k, j])) * basis[j]).ToString() + "\n";
                            basis[k] -= Closest_integer(coef_Gram_Schmidt[k, j]) * basis[j];
                            gr_sch = new LLL(basis).Gram_Schmidt(basis);
                            result += "Calculating new Gram_Schmidt's coefficients and orthogonal vectors" + "\n";
                            coef_Gram_Schmidt = gr_sch.ElementAt(0).Key;
                            ortho_vectors = gr_sch.ElementAt(0).Value; ;
                            for (int i = 0; i < coef_Gram_Schmidt.GetLength(0); i++)
                            {
                                for (int y = 0; y < coef_Gram_Schmidt.GetLength(0); y++)
                                    result += coef_Gram_Schmidt[i, y].ToString() + " ";
                                result += "\n";
                            }
                            result += "Orthogonal vectors:" + "\n";
                            foreach (var v in ortho_vectors)
                                result += v.ToString() + "\n";
                        }
                    }
                    result += ortho_vectors[k].Length.ToString() + ">=" + ((δ - Math.Pow(coef_Gram_Schmidt[k, k - 1], 2)) * ortho_vectors[k - 1].Length).ToString() + "\n";
                    k++;
                    result += "k = k + 1 = " + k.ToString() + "\n";
                }
            }
            result += "\n" + k.ToString() + ">" + (basis.Count - 1).ToString() + "\n";
            result += "Result Gram_Schmidt's coefficients:" + "\n";
            for (int i = 0; i < coef_Gram_Schmidt.GetLength(0); i++)
            {
                for (int j = 0; j < coef_Gram_Schmidt.GetLength(0); j++)
                    result += coef_Gram_Schmidt[i, j].ToString() + " ";
                result += "\n";
            }
            result += "Result Orthogonal vectors:" + "\n";
            foreach (var v in ortho_vectors)
                result += v.ToString() + "\n";
            result += "Result LLL-basis:" + "\n";
            foreach (var v in basis)
                result += v.ToString() + "\n";
            return result;
        }
        public List<Vector_n> prob_Babai_alg(List<Vector_n> basis, Point_n point)
        {
            LLL gram = new LLL(basis);
            var b = gram.prob_LLL_alg(basis, false); //b[0] - lll-базис, b[1] - ортонормированный базис
            Point_n x = point;// введённая точка
            double[] r = new double[point.Count];
            double m_z = 0;
            List<double> m = new List<double>();
            List<Vector_n> res = new List<Vector_n>();
            for (int i = point.Count - 1; i >= 0; i--)
            {
                r[i] = b[1][i] * x / (b[1][i] * b[1][i]);
                m_z = Closest_integer(r[i]);
                m.Add(m_z);
                if (i > 0)
                    x = x - m_z * b[0][i] - (r[i] - m_z) * b[1][i];
                else x = x - (r[i] - m_z) * b[1][i];
                res.Add(new Vector_n(new List<double>() { x[0], x[1], x[2]}));
            }
            m.Reverse();
            res.Add(b[0] * new Vector_n(m));
            return res;
        }



        public string Babai_alg_str(List<Vector_n> basis, Point_n point)
        {
            string result = "";
            LLL gram = new LLL(basis);
            var b = gram.prob_LLL_alg(basis, false);
            result += "Basis after LLL-algorithm:" + "\n"; ;
            result += "b[0] - lll-basis, b[1] - Gram_Schmidt's orthogonal vectors" + "\n";
            result += "Gram_Schmidt's orthogonal vectors:" + "\n";
            foreach (var v in b[1])
                result += v.ToString() + "\n";
            result += "lll-basis:" + "\n";
            foreach (var v in b[0])
                result += v.ToString() + "\n";
            Point_n x = point;
            result += "Point:" + x.ToString() + "\n";
            double[] r = new double[point.Count];
            double m_z = 0;
            List<double> m = new List<double>();
            for (int i = point.Count - 1; i >= 0; i--)
            {
                r[i] = b[1][i] * x / (b[1][i] * b[1][i]);
                result += "r[" + i.ToString() + "] = b[1][i] * x / (b[1]*[i]^2) =" + (b[0][i] * x / (b[0][i] * b[0][i])).ToString() + "\n";
                m_z = Closest_integer(r[i]);
                result += "m[" + i.ToString() + "] = Closest_integer(r[" + i.ToString() + "]) = " + Closest_integer(r[i]).ToString() + "\n";
                m.Add(m_z);
                if (i > 0)
                {
                    result += "i > 0" + "\n";
                    x = x - m_z * b[0][i] - (r[i] - m_z) * b[1][i];
                }
                result += "Point = point - m[" + i.ToString() + "] * b[0][" + i.ToString() + "] - (r[" + i.ToString() + "] - m[" + i.ToString() + "]) * b[1][" + i.ToString() + "]" + "\n";
            }
            m.Reverse();
            string str = "";
            foreach (var v in m)
                str += v.ToString() + ",";
            str = str.Remove(str.Length - 1);
            result += "m = {" + str + "}" + "\n";
            result += "Result point = b * m" + "\n";
            result += (b[1] * new Vector_n(m)).ToString() + "\n";
            return result;
        }


        private void Swap(Vector_n v1, Vector_n v2, ref List<Vector_n> vectors)
        {
            Vector_n v = v1;
            int ind1 = vectors.IndexOf(v1);
            int ind2 = vectors.IndexOf(v2);
            vectors[ind1] = v2;
            vectors[ind2] = v;
        }
        public Dictionary<double, Vector_n> Proj_r(Vector_n u, Vector_n v)
        {
            return new Dictionary<double, Vector_n>() { { (v * u) / (u * u), ((v * u) / (u * u)) * u } };
        }

        public double Closest_integer(double n)
        {
            if (n < 0)
            {
                if (Math.Abs(n) % 1 >= 0.5)
                    return (int)n - 1;
                else return (int)n;
            }
            else if (Math.Abs(n) % 1 >= 0.5)
                return (int)n + 1;
            else return (int)n;
        }
    }

    class Step_by_step
    {
        public List<Vector3D> old;
        public List<List<Vector3D>> new_basis;
        public int current_state;

        public Step_by_step(List<Vector3D> ol, List<List<Vector3D>> new_b)
        {
            old = ol;
            new_basis = new_b;
            current_state = 0;
        }
    }

    class Step_by_step_point
    {
        public List<Point3D> old;
        public List<Point3D> new_basis;
        public int current_state;

        public Step_by_step_point(List<Point3D> ol, List<Point3D> new_b)
        {
            old = ol;
            new_basis = new_b;
            current_state = 0;
        }
    }

    class Coordinate_plane
    {
        public GameObject plane;
        GameObject arrows;
        public Vector3D axis_X;
        public Vector3D axis_Y;
        public Vector3D axis_Z;
        public Material color;
        public float thickness;
        public float length;
        TextMeshPro X;
        TextMeshPro list_cor;

        public Coordinate_plane(GameObject pl, GameObject ar, float len, Material col, float thick, TextMeshPro x, TextMeshPro l)
        {
            length = len;
            color = col;
            thickness = thick;
            arrows = ar;
            X = x;
            list_cor = l;
            axis_X = new Vector3D(new Vector3(-length, 0, 0), new Vector3(length, 0, 0), thickness, color);
            axis_Y = new Vector3D(new Vector3(0, -length, 0), new Vector3(0, length, 0), thickness, color);
            axis_Z = new Vector3D(new Vector3(0, 0, -length), new Vector3(0, 0, length), thickness, color);
            plane = pl;

            foreach (Transform child in arrows.transform)
            {
                Destroy(child.gameObject);
            }

            new Arrow(axis_X, new GameObject(), color).arrow.transform.SetParent(arrows.transform);
            new Arrow(axis_Y, new GameObject(), color).arrow.transform.SetParent(arrows.transform);
            new Arrow(axis_Z, new GameObject(), color).arrow.transform.SetParent(arrows.transform);

            axis_X.vector3D.transform.SetParent(plane.transform);
            axis_Y.vector3D.transform.SetParent(plane.transform);
            axis_Z.vector3D.transform.SetParent(plane.transform);

            foreach (Transform child in list_cor.transform)
            {
                Destroy(child.gameObject);
            }

            for (float i = -(length - 1); i < length; i++)
            {
                for (float j = -(length - 1); j < length; j++)
                {
                    for (float k = -(length - 1); k < length; k++)
                    {
                        if (j == 0 && i == 0 || j == 0 && k == 0 || k == 0 && i == 0)
                        {
                            var po = new Point3D(i, j, k, 0.1f, color);
                            po.point.transform.SetParent(plane.transform);
                            X.transform.position = new Vector3(i + 0.1f, j + 0.1f, k + 0.1f);
                            if (j == 0 && i == 0 && k == 0)
                            {
                                X.text = k.ToString();
                            }
                            if (k!= 0)
                            {
                                X.text = k.ToString();
                            }
                            if (j != 0)
                            {
                                X.text = j.ToString();
                            }
                            if (i != 0)
                            {
                                X.text = i.ToString();
                            }                         
                            Instantiate(X, list_cor.transform).transform.position = new Vector3(i+0.1f, j + 0.1f, k + 0.1f);
                            if (k == length -1)
                            {
                                X.text = "Z";
                                Instantiate(X, list_cor.transform).transform.position = new Vector3(i + 0.1f, j + 0.1f, k + 0.8f);
                            }
                            if (j == length - 1)
                            {
                                X.text = "Y";
                                Instantiate(X, list_cor.transform).transform.position = new Vector3(i + 0.1f, j + 0.8f, k + 0.1f);
                            }
                            if (i == length - 1)
                            {
                                X.text = "X";
                                X.transform.position = new Vector3(i + 0.8f, j + 0.1f, k + 0.1f);
                            }
                        }
                    }
                }
            }
        }

        public void Increase_length(Coordinate_plane plane)
        {
            foreach (Transform child in plane.plane.transform)
            {
                Destroy(child.gameObject);
            }
            plane.length++;
            plane = new Coordinate_plane(plane.plane, arrows, plane.length, plane.color, plane.thickness, plane.X, plane.list_cor);
        }
        public void Decrease_length(Coordinate_plane plane)
        {
            if (plane.length > 1)
            {
                foreach (Transform child in plane.plane.transform)
                {
                    Destroy(child.gameObject);
                }
                plane.length--;
                plane = new Coordinate_plane(plane.plane, arrows, plane.length, plane.color, plane.thickness, plane.X, plane.list_cor);
            }
        }
    }

    class Algorithm_result
    {
        public List<string> all_answers;
        public string whole_result;
        int div;
        public int state;

        public Algorithm_result(string whole_res, int divider)
        {
            whole_result = whole_res;
            div = divider;
            all_answers = DivideOnSubstring(whole_result, div);
            state = 0;
        }

        List<string> DivideOnSubstring(string s, int stepSize)
        {
            List<string> ans = new List<string>();
            int start = 0;
            bool flag = false;
            do
            {
                ans.Add(s.Substring(start, stepSize));
                start += stepSize;
                if (stepSize > s.Length - start) flag = true;
            } while (!flag);
            ans.Add(s.Substring(start, s.Length - start));
            return ans;
        }

        public string Next_step(Algorithm_result result)
        {
            if (result.state < result.all_answers.Count - 1)
            {
                result.state++;
                return result.all_answers[result.state];
            }
            else return result.all_answers[result.state];
        }

        public string Step_back(Algorithm_result result)
        {
            if (result.state > 0)
            {
                result.state--;
                return result.all_answers[result.state];
            }
            else return result.all_answers[0];
        }
    }
    
    class Arrow
    {
        public Vector3D vector;
        public GameObject arrow;

        Mesh mesh;
        MeshRenderer meshRenderer;

        List<Vector3> vertices;
        List<int> triangles;

        public Material material;

        public float height;

        public int segments = 365;

        public Vector3 pos;
        float angle = 0.0f;
        float angleAmount = 0.0f;

        public Arrow(Vector3D vec, GameObject g, Material m)
        {
            vector = vec;
            arrow = g;
            material = m;
            pos = new Vector3(0, 0, 0);
            arrow = DrawArrow(vector, arrow, material, vector.GetLength(vector.first_point, vector.second_point) * 0.1f);
            GameObject bottom = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bottom.transform.rotation = vector.vector3D.transform.rotation;
            bottom.transform.localScale = new Vector3(vector.vector3D.transform.localScale.x * 2, 0.005f, vector.vector3D.transform.localScale.z * 2);
            bottom.GetComponent<Renderer>().material = material;
            bottom.transform.position = GetArrowPosition(vector);
            bottom.transform.SetParent(arrow.transform);
            arrow.transform.Rotate(0, 0, 180f);
            arrow.transform.SetParent(vector.vector3D.transform);
        }

        Vector3 GetArrowPosition(Vector3D vector)
        {
            float full_len = vector.GetLength(vector.first_point, vector.second_point); // длина вектора, соединяющего две точки == длина отрезка
            return vector.second_point + (vector.first_point - vector.second_point) * (full_len * 0.1f / full_len);
        }

        GameObject DrawArrow(Vector3D vector, GameObject arrow, Material material, float h)
        {
            height = h;
            float radius = vector.vector3D.transform.localScale.x;

            arrow.AddComponent<MeshFilter>();
            meshRenderer = arrow.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            mesh = new Mesh();

            arrow.GetComponent<MeshFilter>().mesh = mesh;
            vertices = new List<Vector3>();
            angleAmount = 2 * Mathf.PI / segments;

            angle = 0.0f;
            pos.y += height;
            vertices.Add(new Vector3(pos.x, pos.y, pos.z));

            pos.y -= height;
            vertices.Add(new Vector3(pos.x, pos.y, pos.z));

            for (int i = 0; i < segments; i++)
            {
                pos.x = radius * Mathf.Sin(angle);
                pos.z = radius * Mathf.Cos(angle);
                vertices.Add(new Vector3(pos.x, pos.y, pos.z));
                angle -= angleAmount;
            }
            mesh.vertices = vertices.ToArray();

            triangles = new List<int>();

            for (int i = 2; i < segments + 1; i++)
            {
                triangles.Add(0);
                triangles.Add(i + 1);
                triangles.Add(i);
            }
            triangles.Add(0);
            triangles.Add(2);
            triangles.Add(segments + 1);
            mesh.triangles = triangles.ToArray();
            arrow.transform.rotation = vector.vector3D.transform.rotation;
            vector.vector3D.transform.localScale = new Vector3(vector.vector3D.transform.localScale.x, vector.vector3D.transform.localScale.y * 0.9f, vector.vector3D.transform.localScale.z);
            arrow.transform.position = GetArrowPosition(vector);
            return arrow;
        }
    }

    class OrthonormalizedLattice
    {
        public float n;
        public GameObject OrthoLattice;
        public Material color;
        public OrthonormalizedLattice(float n1, GameObject Ortho, Material c)
        {
            n = n1;
            OrthoLattice = Ortho;
            color = c;

            foreach (Transform child in OrthoLattice.transform)
            {
                Destroy(child.gameObject);
            }

            for (float i = -n; i <= n; i++)
            {
                for (float j = -n; j <= n; j++)
                {
                    for (float k = -n; k <= n; k++)
                    {
                        if (i == n || j == n || k == n)
                        {
                            new Vector3D(new Vector3(i, j, k), new Vector3(-i, j, k), 0.03f, color).vector3D.transform.SetParent(OrthoLattice.transform);
                            new Vector3D(new Vector3(i, j, k), new Vector3(i, -j, k), 0.03f, color).vector3D.transform.SetParent(OrthoLattice.transform);
                            new Vector3D(new Vector3(i, j, k), new Vector3(i, j, -k), 0.03f, color).vector3D.transform.SetParent(OrthoLattice.transform);
                        }
                    }
                }
            }
        }
    }

    public GameObject target;
    float speed = 5;
    float coordinate_plane_length = 3f;
    float coordinate_plane_thickness = 0.03f;
    public Material red;
    public Material blue;
    public Material black;
    public Material yellow;
    public Material green;
    public Material orange;
    public Material grey;
    public Material brown;
    public Material dark_brown;

    public Material transperent;
    public GameObject coordinate_plane;
    Coordinate_plane pl;
    public GameObject arrows;
    public TextMeshPro coordinates_X;
    public TextMeshPro list_coodr;

    public GameObject container;
    Algorithm_result algorithm_Result;
    public Scrollbar scrollbar;
    public GameObject OrthLattice_gameobject;

    static List<Vector3D> vectors = new List<Vector3D>();
    static List<Point3D> point = new List<Point3D>();

    static List<Vector_n> vectors_n = new List<Vector_n>();
    static List<Point_n> point_n = new List<Point_n>();


    public Dropdown dropdown_X1;
    public Dropdown dropdown_Y1;
    public Dropdown dropdown_Z1;
    public Dropdown dropdown_X2;
    public Dropdown dropdown_Y2;
    public Dropdown dropdown_Z2;

    public Dropdown dropdown_point_X1;
    public Dropdown dropdown_point_Y1;
    public Dropdown dropdown_point_Z1;

    public Dropdown dropdown_X1_input;
    public Dropdown dropdown_Y1_input;
    public Dropdown dropdown_Z1_input;
    public Dropdown dropdown_X2_input;
    public Dropdown dropdown_Y2_input;
    public Dropdown dropdown_Z2_input;

    public Dropdown dropdown_point_X1_input;
    public Dropdown dropdown_point_Y1_input;
    public Dropdown dropdown_point_Z1_input;

    List<Dropdown> dropdowns = new List<Dropdown>() { };

    public Dropdown list_algorithms;
    public Dropdown list_algorithms_InputField;


    public GameObject warning_panel_add_vector;
    public GameObject warning_panel_execute;

    public GameObject warning_panel_execute_input;
    public GameObject warning_panel_add_vector_input;

    public GameObject incorrect_input_visual;
    public GameObject incorrect_input;

    public GameObject Lattice;
    public GameObject Points;

    public GameObject Basis;
    public GameObject First_state_point;
    public GameObject Previous_state;

    public GameObject input_point;
    public GameObject input_vectors;
    public GameObject after_play;
    public GameObject menu;
    public GameObject calculate_panel;
    public GameObject exit_panel;
    public GameObject exit_panel_input;
    public GameObject modeling_lattice;

    Step_by_step moving_vectors;
    Step_by_step_point moving_point;
    private OrthonormalizedLattice ortho_lattice;

    public Text result;

    public Slider built_or_not;
    public Toggle fast_or_not;
    public Slider visible_or_not;
    public Slider previuos_or_not;
    public Slider light_or_not;
    public Slider light_points_or_not;
    public Button execute_button;
    public Button execute_button_input;

    public GameObject Parallelepiped;

    public Text lable_steps;
    Material basis_color;
    List<string> coord = new List<string>();

    private bool on_panel;
    int UILayer;

    void Start()
    {
        pl = new Coordinate_plane(coordinate_plane, arrows, coordinate_plane_length, black, coordinate_plane_thickness, coordinates_X, list_coodr);
        Instantiate(pl.plane);
        dropdowns.Add(dropdown_X1);
        dropdowns.Add(dropdown_Y1);
        dropdowns.Add(dropdown_Z1);
        dropdowns.Add(dropdown_X2);
        dropdowns.Add(dropdown_Y2);
        dropdowns.Add(dropdown_Z2);
        dropdowns.Add(dropdown_point_X1);
        dropdowns.Add(dropdown_point_Y1);
        dropdowns.Add(dropdown_point_Z1);

        dropdowns.Add(dropdown_X1_input);
        dropdowns.Add(dropdown_Y1_input);
        dropdowns.Add(dropdown_Z1_input);
        dropdowns.Add(dropdown_X2_input);
        dropdowns.Add(dropdown_Y2_input);
        dropdowns.Add(dropdown_Z2_input);
        dropdowns.Add(dropdown_point_X1_input);
        dropdowns.Add(dropdown_point_Y1_input);
        dropdowns.Add(dropdown_point_Z1_input);

        input_point.gameObject.SetActive(false);
        after_play.gameObject.SetActive(false);
        modeling_lattice.gameObject.SetActive(false);
        calculate_panel.gameObject.SetActive(false);
        warning_panel_execute_input.gameObject.SetActive(false);
        warning_panel_add_vector_input.gameObject.SetActive(false);
        incorrect_input.gameObject.SetActive(false);
        incorrect_input_visual.gameObject.SetActive(false);
        exit_panel.gameObject.SetActive(false);
        exit_panel_input.gameObject.SetActive(false);
        FillDropdowns();
        warning_panel_add_vector.gameObject.SetActive(false);
        warning_panel_execute.gameObject.SetActive(false);
        built_or_not.interactable = false;
        built_or_not.value = 255;
        fast_or_not.isOn = false;     
        light_or_not.value = 255;
        light_points_or_not.value = 255;
        scrollbar.value = 1;
        ortho_lattice = new OrthonormalizedLattice(coordinate_plane_length, OrthLattice_gameobject, grey);
        visible_or_not.value = 255;
        previuos_or_not.value = 255;
        basis_color = transperent;
        on_panel = false;
        UILayer = LayerMask.NameToLayer("UI");
    }

    void RotateCamera()
    {
        if (on_panel is false)
        {
            if (Input.GetMouseButton(0))
            {
                transform.RotateAround(target.transform.position, new Vector3(0, 1, 0), Input.GetAxis("Mouse X") * speed);
                transform.RotateAround(target.transform.position, new Vector3(1, 0, 0), Input.GetAxis("Mouse Y") * -speed);

            }
        }
    }

    void FillDropdowns()
    {
        coord.Clear();
        for (float i = -(pl.length - 1); i < pl.length; i++)
        {
            coord.Add(i.ToString());
        }
        foreach (var drop in dropdowns)
        {
            drop.ClearOptions();
            drop.AddOptions(coord);
        }
    }

    Vector_n Convert_to_Vector_n(Vector3 v)
    {
        return new Vector_n(new List<double>() { v.x, v.y, v.z});
    }

    void DisableDropdown(Dropdown dr)
    {
        var toogles = dr.GetComponentsInChildren<Toggle>(true);
         toogles[0].interactable = false;
    
    }

    void EnableDropdown(Dropdown dr)
    {
        var toogles = dr.GetComponentsInChildren<Toggle>(true);
        toogles[0].interactable = true;

    }

    public void Choose_algorithm()
    {
        if (list_algorithms.value == 0)
        {
            input_point.gameObject.SetActive(false);
        }
        if (list_algorithms.value == 1)
        {
            input_point.gameObject.SetActive(true);
        }
    }

    public void Add_vector_to_Inputfield()
    {
        if (vectors_n.Count < 3)
        {
            Vector3 point_first = new Vector3(float.Parse(dropdown_X1_input.options[dropdown_X1.value].text), float.Parse(dropdown_Y1_input.options[dropdown_Y1.value].text), float.Parse(dropdown_Z1_input.options[dropdown_Z1.value].text));
            Vector3 point_second = new Vector3(float.Parse(dropdown_X2_input.options[dropdown_X2.value].text), float.Parse(dropdown_Y2_input.options[dropdown_Y2.value].text), float.Parse(dropdown_Z2_input.options[dropdown_Z2.value].text));
            if (point_first != point_second)
            {
                try
                {
                    if (!vectors_n.Contains(new Vector_n(new List<double>() { Convert.ToDouble(dropdown_X2_input.options[dropdown_X2_input.value].text) - Convert.ToDouble(dropdown_X1_input.options[dropdown_X1_input.value].text), Convert.ToDouble(dropdown_Y2_input.options[dropdown_Y2_input.value].text) - Convert.ToDouble(dropdown_Y1_input.options[dropdown_Y1_input.value].text), Convert.ToDouble(dropdown_Z2_input.options[dropdown_Z2_input.value].text) - Convert.ToDouble(dropdown_Z1_input.options[dropdown_Z1_input.value].text) })))
                    {
                        vectors_n.Add(new Vector_n(new List<double>() { Convert.ToDouble(dropdown_X2_input.options[dropdown_X2_input.value].text) - Convert.ToDouble(dropdown_X1_input.options[dropdown_X1_input.value].text), Convert.ToDouble(dropdown_Y2_input.options[dropdown_Y2_input.value].text) - Convert.ToDouble(dropdown_Y1_input.options[dropdown_Y1_input.value].text), Convert.ToDouble(dropdown_Z2_input.options[dropdown_Z2_input.value].text) - Convert.ToDouble(dropdown_Z1_input.options[dropdown_Z1_input.value].text) }));
                        if (vectors_n.Count == 1)
                        {
                            result.text += "Vectors:" + '\n' + vectors_n[vectors_n.Count - 1].ToString() + '\n';
                        }
                        else
                        {
                            result.GetComponent<Text>().text += vectors_n[vectors_n.Count - 1].ToString() + '\n';
                        }
                    }
                }
                catch
                {
                    incorrect_input.gameObject.SetActive(true);
                }
            }
            else
            {
                incorrect_input.gameObject.SetActive(true);
            }
        }
        else
        {
            warning_panel_add_vector_input.gameObject.SetActive(true);
        }
    }

    public void AddPoint_to_Inputfield()
    {
        if (point_n.Count < 1)
        {
            point_n.Add(new Point_n(new List<double>() { Convert.ToDouble(dropdown_point_X1_input.options[dropdown_point_X1_input.value].text), Convert.ToDouble(dropdown_point_Y1_input.options[dropdown_point_Y1_input.value].text), Convert.ToDouble(dropdown_point_Z1_input.options[dropdown_point_Z1_input.value].text) }));
            result.text += "Point:" + '\n' + point_n[0].ToString() + '\n';
        }
        else
        {
            warning_panel_add_vector_input.gameObject.SetActive(true);
        }
    }
   
    public void AddVector()
    {
        if (vectors.Count < 3)
        {
            try
            {
                Vector3 point_first = new Vector3(float.Parse(dropdown_X1.options[dropdown_X1.value].text), float.Parse(dropdown_Y1.options[dropdown_Y1.value].text), float.Parse(dropdown_Z1.options[dropdown_Z1.value].text));
                Vector3 point_second = new Vector3(float.Parse(dropdown_X2.options[dropdown_X2.value].text), float.Parse(dropdown_Y2.options[dropdown_Y2.value].text), float.Parse(dropdown_Z2.options[dropdown_Z2.value].text));
                if (point_first != point_second)
                {
                    var vec = new Vector3D(point_first, point_second, coordinate_plane_thickness + +0.02f, transperent);
                    vec = new Arrow(vec, new GameObject(), transperent).vector;
                    vec.vector3D.transform.SetParent(Basis.transform);
                    vectors.Add(vec);
                }
                else
                {
                    incorrect_input_visual.gameObject.SetActive(true);
                }
                if (vectors.Count == 3)
                {
                    built_or_not.interactable = true;
                    BuildLattice(vectors, vectors[0].first_point);
                    modeling_lattice.gameObject.SetActive(true);
                }
            }
            catch
            {
                incorrect_input_visual.gameObject.SetActive(true);
            }
        }
        else
        {
            warning_panel_add_vector.gameObject.SetActive(true);
        }
    }

    public void AddPoint()
    {
        if(point.Count <1)
        {
            var p = new Point3D(float.Parse(dropdown_point_X1.options[dropdown_point_X1.value].text), float.Parse(dropdown_point_Y1.options[dropdown_point_Y1.value].text), float.Parse(dropdown_point_Z1.options[dropdown_point_Z1.value].text), coordinate_plane_thickness + 0.06f, transperent);
            point.Add(p);
        }
    }
   
    void Clear_lattices()
    {
        foreach (Transform child in Lattice.transform)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
        foreach (Transform child in Points.transform)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
    }

    public void Light_lattice()
    {
        foreach (Transform child in Parallelepiped.transform)
        {
            Destroy(child.gameObject);
        }
       
        Vector3 B = vectors[0].second_point;
        Vector3 A = vectors[0].first_point;
        Vector3 D = vectors[1].second_point;
        Vector3 C = GetForthPoint(vectors[0].second_point, vectors[1].second_point, vectors[0].first_point);
        Vector3 A1 = vectors[2].second_point;
        Vector3 B1 = GetForthPoint(A1, B, A);
        Vector3 C1 = GetForthPoint(C, B1, B);
        Vector3 D1 = GetForthPoint(D, A1, A);
        new Vector3D(B, C, coordinate_plane_thickness + 0.01f, basis_color).vector3D.transform.SetParent(Parallelepiped.transform);
        new Vector3D(D, C, coordinate_plane_thickness + 0.01f, basis_color).vector3D.transform.SetParent(Parallelepiped.transform);
        new Vector3D(D, D1, coordinate_plane_thickness + 0.01f, basis_color).vector3D.transform.SetParent(Parallelepiped.transform);
        new Vector3D(C, C1, coordinate_plane_thickness + 0.01f, basis_color).vector3D.transform.SetParent(Parallelepiped.transform);
        new Vector3D(B, B1, coordinate_plane_thickness + 0.01f, basis_color).vector3D.transform.SetParent(Parallelepiped.transform);
        new Vector3D(B1, C1, coordinate_plane_thickness + 0.01f, basis_color).vector3D.transform.SetParent(Parallelepiped.transform);
        new Vector3D(A1, D1, coordinate_plane_thickness + 0.01f, basis_color).vector3D.transform.SetParent(Parallelepiped.transform);
        new Vector3D(A1, B1, coordinate_plane_thickness + 0.01f, basis_color).vector3D.transform.SetParent(Parallelepiped.transform);
        new Vector3D(D1, C1, coordinate_plane_thickness + 0.01f, basis_color).vector3D.transform.SetParent(Parallelepiped.transform);
    }

    void Fill_previous_state()
    {
        foreach (Transform child in Previous_state.transform)
        {
            Destroy(child.gameObject);
        }
        if (moving_vectors.current_state > 0)
        {
            Vector3 B = moving_vectors.new_basis[moving_vectors.current_state - 1][0].second_point;
            Vector3 A = moving_vectors.new_basis[moving_vectors.current_state - 1][0].first_point;
            Vector3 D = moving_vectors.new_basis[moving_vectors.current_state - 1][1].second_point;
            Vector3 C = GetForthPoint(moving_vectors.new_basis[moving_vectors.current_state - 1][0].second_point, moving_vectors.new_basis[moving_vectors.current_state - 1][1].second_point, moving_vectors.new_basis[moving_vectors.current_state - 1][0].first_point);
            Vector3 A1 = moving_vectors.new_basis[moving_vectors.current_state - 1][2].second_point;
            Vector3 B1 = GetForthPoint(A1, B, A);
            Vector3 C1 = GetForthPoint(C, B1, B);
            Vector3 D1 = GetForthPoint(D, A1, A);
            new Vector3D(moving_vectors.new_basis[moving_vectors.current_state - 1][0].first_point, moving_vectors.new_basis[moving_vectors.current_state - 1][0].second_point, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(moving_vectors.new_basis[moving_vectors.current_state - 1][1].first_point, moving_vectors.new_basis[moving_vectors.current_state - 1][1].second_point, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(moving_vectors.new_basis[moving_vectors.current_state - 1][2].first_point, moving_vectors.new_basis[moving_vectors.current_state - 1][2].second_point, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(B, C, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(D, C, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(D, D1, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(C, C1, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(B, B1, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(B1, C1, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(A1, D1, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(A1, B1, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(D1, C1, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
        }
        else
        {
            Vector3 B = vectors[0].second_point;
            Vector3 A = vectors[0].first_point;
            Vector3 D = vectors[1].second_point;
            Vector3 C = GetForthPoint(vectors[0].second_point, vectors[1].second_point, vectors[0].first_point);
            Vector3 A1 = vectors[2].second_point;
            Vector3 B1 = GetForthPoint(A1, B, A);
            Vector3 C1 = GetForthPoint(C, B1, B);
            Vector3 D1 = GetForthPoint(D, A1, A);
            new Vector3D(vectors[0].first_point, vectors[0].second_point, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(vectors[1].first_point, vectors[1].second_point, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(vectors[2].first_point, vectors[2].second_point, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(B, C, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(D, C, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(D, D1, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(C, C1, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(B, B1, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(B1, C1, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(A1, D1, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(A1, B1, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
            new Vector3D(D1, C1, coordinate_plane_thickness + 0.01f, brown).vector3D.transform.SetParent(Previous_state.transform);
        }
    }

    void BuildLattice(List<Vector3D> vectors, Vector3 sdvig)
    {
        Clear_lattices();
        float n = 0;
        if (list_algorithms.value == 1)
        {
            n = 13;
        }
        else n = 5;
        for (float i = -n; i <= n; i++)
        {
            for (float j = -n; j <= n; j++)
            {
                for (float k = -n; k <= n; k++)
                {
                    Vector3 point = i * vectors[0].vector + j * vectors[1].vector + k * vectors[2].vector + sdvig.x * vectors[0].vector + sdvig.y * vectors[1].vector + sdvig.z * vectors[2].vector;
                    new Point3D(point.x, point.y, point.z, coordinate_plane_thickness + 0.02f, dark_brown).point.transform.SetParent(Points.transform);
                    if (i == n || j == n || k == n)
                    {
                        new Vector3D(i * vectors[0].vector + j * vectors[1].vector + k * vectors[2].vector + sdvig, -i * vectors[0].vector + j * vectors[1].vector + k * vectors[2].vector + sdvig, coordinate_plane_thickness, brown).vector3D.transform.SetParent(Lattice.transform);
                        new Vector3D(i * vectors[0].vector + j * vectors[1].vector + k * vectors[2].vector + sdvig, i * vectors[0].vector - j * vectors[1].vector + k * vectors[2].vector + sdvig, coordinate_plane_thickness, brown).vector3D.transform.SetParent(Lattice.transform);
                        new Vector3D(i * vectors[0].vector + j * vectors[1].vector + k * vectors[2].vector + sdvig, i * vectors[0].vector + j * vectors[1].vector - k * vectors[2].vector + sdvig, coordinate_plane_thickness, brown).vector3D.transform.SetParent(Lattice.transform);
                    }
                }
            }
        }
    }

    Vector3 GetForthPoint(Vector3 p1, Vector3 p3, Vector3 p2)
    {
        Vector3 center = new Vector3((p1.x + p3.x) / 2, (p1.y + p3.y) / 2, (p1.z + p3.z) / 2);
        return new Vector3(center.x * 2 - p2.x, center.y * 2 - p2.y, center.z * 2 - p2.z);
    }

    public void Decrease_coordinate_plane()
    {
        if (pl.length > 3 && pl.length < 20)
        {
            pl.Decrease_length(pl);
            FillDropdowns();
            ortho_lattice = new OrthonormalizedLattice(pl.length, OrthLattice_gameobject, grey);
        }
    }

    public void Increase_coordinate_plane()
    {
        if (pl.length >= 2 && pl.length < 20)
        {
            pl.Increase_length(pl);
            FillDropdowns();
            ortho_lattice = new OrthonormalizedLattice(pl.length, OrthLattice_gameobject, grey);
        }
    }

    private void Change_color(GameObject obj, Material color)
    {
        foreach (Transform child in obj.transform)
        {
            child.gameObject.GetComponent<Renderer>().material = color;
            foreach (Transform ch in child.transform)
            {
                ch.gameObject.GetComponent<Renderer>().material = color;
            }
        }
      
    }

    private void Change_color_obj(GameObject obj, Material color)
    {
        obj.gameObject.GetComponent<Renderer>().material = color;
    }

    public void OpenLoadFile()
    {
        if (point.Count == 0 && vectors.Count == 0)
        {
            built_or_not.interactable = true;
            built_or_not.value = 255;
            OpenFileName openFileName = new OpenFileName();
            if (LocalDialog.GetOpenFileName(openFileName))
            {
                StreamReader inp_stm = new StreamReader(openFileName.file);
                while (!inp_stm.EndOfStream)
                {
                    try
                    {
                        string n = inp_stm.ReadLine();
                        if (Convert.ToInt32(n) == 3 && list_algorithms.value == 0)
                        {
                            for (int i = 0; i < Convert.ToInt32(n); i++)
                            {
                                string dig = inp_stm.ReadLine();
                                List<string> val = dig.Split(' ').ToList();
                                var vec = new Vector3D(new Vector3(0, 0, 0), new Vector3(float.Parse(val[0]), float.Parse(val[1]), float.Parse(val[2])), coordinate_plane_thickness + 0.01f, transperent);
                                vec = new Arrow(vec, new GameObject(), transperent).vector;
                                vec.vector3D.transform.SetParent(Basis.transform);
                                vectors.Add(vec);
                            }
                            BuildLattice(vectors, vectors[0].first_point);
                            modeling_lattice.gameObject.SetActive(true);
                        }
                        else if(Convert.ToInt32(n) == 4 && list_algorithms.value == 1)
                        {
                            for (int i = 0; i < Convert.ToInt32(n); i++)
                            {
                                if (i == 0)
                                {
                                    string cor = inp_stm.ReadLine();
                                    List<string> cor_point = cor.Split(' ').ToList();
                                    point.Add(new Point3D(float.Parse(cor_point[0]), float.Parse(cor_point[1]), float.Parse(cor_point[2]), coordinate_plane_thickness + 0.06f, transperent));
                                }
                                else
                                {
                                    string dig = inp_stm.ReadLine();
                                    List<string> val = dig.Split(' ').ToList();
                                    var vec = new Vector3D(new Vector3(0, 0, 0), new Vector3(float.Parse(val[0]), float.Parse(val[1]), float.Parse(val[2])), coordinate_plane_thickness + 0.01f, transperent);
                                    vec = new Arrow(vec, new GameObject(), transperent).vector;
                                    vec.vector3D.transform.SetParent(Basis.transform);
                                    vectors.Add(vec);
                                }
                            }
                            BuildLattice(vectors, vectors[0].first_point);
                            modeling_lattice.gameObject.SetActive(true);
                        }
                        else
                        {
                            incorrect_input_visual.SetActive(true);
                        }
                    }
                    catch
                    {
                        incorrect_input_visual.SetActive(true);
                    }
                }
                inp_stm.Close();
            };
        }
    }

    public void OpenLoadFile_InputField()
    {
        if (point_n.Count == 0 && vectors_n.Count == 0)
        {
            OpenFileName openFileName = new OpenFileName();
            if (LocalDialog.GetOpenFileName(openFileName))
            {
                StreamReader inp_stm = new StreamReader(openFileName.file);
                while (!inp_stm.EndOfStream)
                {
                    try
                    {
                        string n = inp_stm.ReadLine();
                        if (list_algorithms_InputField.value == 0)
                        {
                            for (int i = 0; i < Convert.ToInt32(n); i++)
                            {
                                string dig = inp_stm.ReadLine();
                                List<string> val = dig.Split(' ').ToList();
                                List<double> pc = new List<double>();
                                for (int j = 0; j < val.Count; j++)
                                {
                                    pc.Add(Convert.ToDouble(val[j]));
                                }
                                if (!vectors_n.Contains(new Vector_n(pc)))
                                {
                                    vectors_n.Add(new Vector_n(pc));
                                }
                            }
                            for (int i = 0; i < vectors_n.Count; i++)
                            {
                                if (i == 0)
                                {
                                    result.text += "Vectors:" + "\n" + vectors_n[i].ToString() + "\n";
                                }
                                else
                                {
                                    result.text += vectors_n[i].ToString() + "\n";
                                }
                            }
                        }
                        else if (list_algorithms_InputField.value == 1)
                        {
                            for (int i = 0; i < Convert.ToInt32(n); i++)
                            {
                                if (i == 0)
                                {
                                    string cor = inp_stm.ReadLine();
                                    List<string> cor_point = cor.Split(' ').ToList();
                                    List<double> pc = new List<double>();
                                    for (int j = 0; j < cor_point.Count; j++)
                                    {
                                        pc.Add(Convert.ToDouble(cor_point[j]));
                                    }
                                    point_n.Add(new Point_n(pc));
                                }
                                else
                                {
                                    string dig = inp_stm.ReadLine();
                                    List<string> val = dig.Split(' ').ToList();
                                    List<double> pc = new List<double>();
                                    for (int j = 0; j < val.Count; j++)
                                    {
                                        pc.Add(Convert.ToDouble(val[j]));
                                    }
                                    if (!vectors_n.Contains(new Vector_n(pc)))
                                    {
                                        vectors_n.Add(new Vector_n(pc));
                                    }
                                }
                            }
                            for (int i = 0; i < vectors_n.Count; i++)
                            {
                                if (i == 0)
                                {
                                    result.text += "Vectors:" + "\n" + vectors_n[i].ToString() + "\n";
                                }
                                else
                                {
                                    result.text += vectors_n[i].ToString() + "\n";
                                }

                            }
                            result.text += "Point:" + "\n" + point_n[0].ToString() + "\n";
                        }
                        scrollbar.value = 1;
                    }
                    catch
                    {
                        incorrect_input.SetActive(true);
                    }
                }
                inp_stm.Close();
            };
        }
       
    }

    public void ClearAll()
    {
        foreach (Transform child in Lattice.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in Points.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in Basis.transform)
        {
            Destroy(child.gameObject);
        }
        vectors = new List<Vector3D>();
        foreach (Point3D child in point)
        {
            Destroy(child.point);
        }
        foreach (Transform child in First_state_point.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in Previous_state.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in Parallelepiped.transform)
        {
            Destroy(child.gameObject);
        }
        point = new List<Point3D>();
        after_play.gameObject.SetActive(false);
        modeling_lattice.gameObject.SetActive(false);
        input_vectors.gameObject.SetActive(true);
        input_point.gameObject.SetActive(false);
        list_algorithms.value = 0;
        EnableDropdown(list_algorithms);
        built_or_not.interactable = false;
        built_or_not.value = 255;
        fast_or_not.isOn = false;
        visible_or_not.value = 255;
        previuos_or_not.value = 255;
        light_or_not.value = 255;
        light_points_or_not.value = 255;
        basis_color = transperent;
        execute_button.interactable = true;
    }

    public void ClearAll_for_InputField()
    {
        vectors_n.Clear();
        point_n.Clear();
        result.text = "";       
        EnableDropdown(list_algorithms_InputField);
        algorithm_Result = null;
        execute_button_input.interactable = true;
    }

    public void Back()
    {
        ClearAll();
        menu.gameObject.SetActive(true);
    }

    List<List<Vector3D>> Convert_to_Vector3D(List<List<Vector_n>> vec)
    {
        List<List<Vector3D>> vector3Ds = new List<List<Vector3D>>();
        List<Vector3D> vector = new List<Vector3D>();
        for (int i=0; i<vec[0].Count;i++)
        {
            var v = new Vector3D(new Vector3(0, 0, 0), new Vector3(Convert.ToSingle(vec[0][i][0]), Convert.ToSingle(vec[0][i][1]), Convert.ToSingle(vec[0][i][2])), coordinate_plane_thickness + 0.02f, transperent);
            v.vector3D.SetActive(false);
            vector.Add(v);   
           if((i+1) %3 == 0)
           {
                vector3Ds.Add(vector);
                vector = new List<Vector3D>();
           }
        }
        return vector3Ds;
    }

    List<Point3D> Convert_to_Point3D(List<Vector_n> vec)
    {
        List<Point3D> points = new List<Point3D>();
        for (int i = 0; i < vec.Count; i++)
        {
            var p = new Point3D(Convert.ToSingle(vec[i][0]), Convert.ToSingle(vec[i][1]), Convert.ToSingle(vec[i][2]), coordinate_plane_thickness+0.01f, green);
            points.Add(p);
            points[i].point.SetActive(false);
        }
        return points;
    }

    void LLL_play()
    {
        List<Vector_n> vectors_n = new List<Vector_n>();
        for (int i = 0; i < vectors.Count; i++)
        {
            vectors_n.Add(Convert_to_Vector_n(vectors[i].vector));
        }

        LLL gram = new LLL(vectors_n);
        var new_basis = gram.prob_LLL_alg(vectors_n, true);
       
        moving_vectors = new Step_by_step(vectors, Convert_to_Vector3D(new_basis));
        if (moving_vectors.new_basis.Count != 0)
        {
            lable_steps.text = "0/" + moving_vectors.new_basis.Count.ToString();
        }
        else
        {
            incorrect_input.gameObject.SetActive(true);
        }
    }

    void Babai_play()
    {
        List<Vector_n> vectors_n = new List<Vector_n>();
        Point_n point_n = new Point_n(new List<double>() { point[0].x, point[0].y, point[0].z} );
        for (int i = 0; i < vectors.Count; i++)
        {
            vectors_n.Add(Convert_to_Vector_n(vectors[i].vector));
        }
        LLL gram = new LLL(vectors_n);
       
        var new_basis = gram.prob_LLL_alg(vectors_n, true);

        var new_point_positions = gram.prob_Babai_alg(vectors_n, point_n);

        moving_vectors = new Step_by_step(vectors, Convert_to_Vector3D(new_basis));
        if (moving_vectors.new_basis.Count != 0)
        {
            Change_color(Basis, green);
            for (int i = 0; i < vectors.Count; i++)
            {
                bool rotating = false;
                StartCoroutine(RotateObject(vectors[i].vector3D, moving_vectors.new_basis[moving_vectors.new_basis.Count - 1][i].vector3D.transform.rotation, 2f, rotating));
                StartCoroutine(Change_size(vectors[i].vector3D, vectors[i].vector3D.transform.localScale, moving_vectors.new_basis[moving_vectors.new_basis.Count - 1][i].vector3D.transform.localScale, 2f, 5f));

                Vector3 center = vectors[i].GetCenter(moving_vectors.new_basis[moving_vectors.new_basis.Count - 1][i].first_point, moving_vectors.new_basis[moving_vectors.new_basis.Count - 1][i].second_point);
                Vector3 new_position = new Vector3(moving_vectors.new_basis[moving_vectors.new_basis.Count - 1][i].second_point.x - center.x, moving_vectors.new_basis[moving_vectors.new_basis.Count - 1][i].second_point.y - center.y, moving_vectors.new_basis[moving_vectors.new_basis.Count - 1][i].second_point.z - center.z);
                StartCoroutine(MoveOverSeconds(vectors[i].vector3D, new_position, 2f));
                vectors[i].first_point = moving_vectors.new_basis[moving_vectors.new_basis.Count - 1][i].first_point;
                vectors[i].second_point = moving_vectors.new_basis[moving_vectors.new_basis.Count - 1][i].second_point;
            }
            moving_point = new Step_by_step_point(point, Convert_to_Point3D(new_point_positions));
            lable_steps.text = moving_point.current_state.ToString() + "/" + moving_point.new_basis.Count.ToString();
            Light_lattice();
            BuildLattice(moving_vectors.new_basis[moving_vectors.new_basis.Count - 1], new Vector3());
        }
        else
        {
            incorrect_input.gameObject.SetActive(true);
        }
    }

    public void Rotate()
    {
        if (list_algorithms.value == 0)
        {
            built_or_not.value = 255;                 
            light_or_not.value = 255;
            light_points_or_not.value = 255;
            built_or_not.interactable = true;
            previuos_or_not.value = 255;
            Fill_previous_state();
            if (fast_or_not.isOn is false)// медленный режим
            {
                if (moving_vectors.current_state < moving_vectors.new_basis.Count)
                {
                    if (moving_vectors.current_state == 0)
                    {
                        Change_color(Basis, orange);
                        basis_color = orange;
                    }
                    if (moving_vectors.current_state == moving_vectors.new_basis.Count - 1)
                    {
                        Change_color(Basis, green);
                        basis_color = green;
                    }
                    for (int i = 0; i < vectors.Count; i++)
                    {
                        bool rotating = false;
                        StartCoroutine(RotateObject(vectors[i].vector3D, moving_vectors.new_basis[moving_vectors.current_state][i].vector3D.transform.rotation, 2f, rotating));
                        StartCoroutine(Change_size(vectors[i].vector3D, vectors[i].vector3D.transform.localScale, moving_vectors.new_basis[moving_vectors.current_state][i].vector3D.transform.localScale, 2f, 5f));

                        Vector3 center = vectors[i].GetCenter(moving_vectors.new_basis[moving_vectors.current_state][i].first_point, moving_vectors.new_basis[moving_vectors.current_state][i].second_point);
                        Vector3 new_position = new Vector3(moving_vectors.new_basis[moving_vectors.current_state][i].second_point.x - center.x, moving_vectors.new_basis[moving_vectors.current_state][i].second_point.y - center.y, moving_vectors.new_basis[moving_vectors.current_state][i].second_point.z - center.z);
                        StartCoroutine(MoveOverSeconds(vectors[i].vector3D, new_position, 2f));
                        vectors[i].first_point = moving_vectors.new_basis[moving_vectors.current_state][i].first_point;
                        vectors[i].second_point = moving_vectors.new_basis[moving_vectors.current_state][i].second_point;
                    }
                }
                else
                {
                    warning_panel_execute.SetActive(true);
                }
            }
            else // быстрый режим
            {
                if (moving_vectors.current_state < moving_vectors.new_basis.Count)
                {
                    if (moving_vectors.current_state == 0)
                    {
                        Change_color(Basis, orange);
                    }
                    if (moving_vectors.current_state == moving_vectors.new_basis.Count - 1)
                    {
                        Change_color(Basis, green);
                    }
                    for (int i = 0; i < vectors.Count; i++)
                    {
                        bool rotating = false;
                        StartCoroutine(RotateObject(vectors[i].vector3D, moving_vectors.new_basis[moving_vectors.current_state][i].vector3D.transform.rotation, 0.5f, rotating));
                        StartCoroutine(Change_size(vectors[i].vector3D, vectors[i].vector3D.transform.localScale, moving_vectors.new_basis[moving_vectors.current_state][i].vector3D.transform.localScale, 0.5f, 5f));

                        Vector3 center = vectors[i].GetCenter(moving_vectors.new_basis[moving_vectors.current_state][i].first_point, moving_vectors.new_basis[moving_vectors.current_state][i].second_point);
                        Vector3 new_position = new Vector3(moving_vectors.new_basis[moving_vectors.current_state][i].second_point.x - center.x, moving_vectors.new_basis[moving_vectors.current_state][i].second_point.y - center.y, moving_vectors.new_basis[moving_vectors.current_state][i].second_point.z - center.z);
                        StartCoroutine(MoveOverSeconds(vectors[i].vector3D, new_position, 0.5f));
                        vectors[i].first_point = moving_vectors.new_basis[moving_vectors.current_state][i].first_point;
                        vectors[i].second_point = moving_vectors.new_basis[moving_vectors.current_state][i].second_point;
                    }                   
                }
                else
                {
                    warning_panel_execute.SetActive(true);
                }
            }
            if (moving_vectors.current_state < moving_vectors.new_basis.Count)
            {  
                if (moving_vectors.current_state == moving_vectors.new_basis.Count - 1)
                {
                    basis_color = green;
                }
                Light_lattice();
                BuildLattice(moving_vectors.new_basis[moving_vectors.current_state], vectors[0].first_point);
                moving_vectors.current_state++;
                lable_steps.text = moving_vectors.current_state.ToString() + "/" + moving_vectors.new_basis.Count.ToString();
            }
        }
        if(list_algorithms.value == 1)
        {
            built_or_not.value = 255;
            light_or_not.value = 255;
            light_points_or_not.value = 255;
            built_or_not.interactable = true;
            previuos_or_not.value = 255;
            Fill_previous_state();         
            if (moving_point.current_state < moving_point.new_basis.Count)
            {
                if (moving_point.current_state == 0)
                {
                    new Point3D(point[0].x, point[0].y, point[0].z, coordinate_plane_thickness + 0.02f, transperent).point.transform.SetParent(First_state_point.transform);
                    Change_color_obj(point[0].point, orange);
                }
                if (moving_point.current_state == moving_point.new_basis.Count - 1)
                {
                    Change_color_obj(point[0].point, green);
                }
                if (fast_or_not.isOn is false)
                {
                    StartCoroutine(MoveOverSeconds(point[0].point, new Vector3(moving_point.new_basis[moving_point.current_state].x, moving_point.new_basis[moving_point.current_state].y, moving_point.new_basis[moving_point.current_state].z), 2f));
                    point[0].x = moving_point.new_basis[moving_point.current_state].x;
                    point[0].y = moving_point.new_basis[moving_point.current_state].y;
                    point[0].z = moving_point.new_basis[moving_point.current_state].z;
                }
                else
                {
                    StartCoroutine(MoveOverSeconds(point[0].point, new Vector3(moving_point.new_basis[moving_point.current_state].x, moving_point.new_basis[moving_point.current_state].y, moving_point.new_basis[moving_point.current_state].z), 0.5f));
                    point[0].x = moving_point.new_basis[moving_point.current_state].x;
                    point[0].y = moving_point.new_basis[moving_point.current_state].y;
                    point[0].z = moving_point.new_basis[moving_point.current_state].z;
                }
                moving_point.current_state++;
                lable_steps.text = moving_point.current_state.ToString() + "/" + moving_point.new_basis.Count.ToString();               
            }
            else
            {
                warning_panel_execute.SetActive(true);
            }
        }
    }

    IEnumerator RotateObject(GameObject gameObjectToMove, Quaternion newRot, float duration, bool rotating)
    {
        if (rotating)
        {
            yield break;
        }
        rotating = true;

        Quaternion currentRot = gameObjectToMove.transform.rotation;

        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            gameObjectToMove.transform.rotation = Quaternion.Lerp(currentRot, newRot, counter / duration);
            yield return null;
        }
        rotating = false;
    }

    IEnumerator Change_size(GameObject vec, Vector3 a, Vector3 b, float time, float speed)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;
        while(i < 1.0f)
        {
            i += Time.deltaTime * rate;
            vec.transform.localScale = Vector3.Lerp(a, b, i);
            yield return null;
        }
    }

    public void MoveObject()
    {
        if (vectors.Count == 3)
        {
            execute_button.interactable = false;
            DisableDropdown(list_algorithms);
            for (int i = 0; i < vectors.Count; i++)
            {
                Vector3 center = vectors[i].GetCenter(vectors[i].first_point, vectors[i].second_point);
                Vector3 new_position = new Vector3(vectors[i].second_point.x - center.x, vectors[i].second_point.y - center.y, vectors[i].second_point.z - center.z);
                vectors[i].first_point = new Vector3(0, 0, 0);
                vectors[i].second_point = vectors[i].vector;
                StartCoroutine(MoveOverSeconds(vectors[i].vector3D, new_position,2f));               
            }
            after_play.gameObject.SetActive(true);
            input_vectors.gameObject.SetActive(false);
            input_point.gameObject.SetActive(false);           
            if (list_algorithms.value == 0)
            {
                LLL_play();
                Light_lattice();
            }
            if (list_algorithms.value == 1)
            {
                Babai_play();
                basis_color = green;
                Light_lattice();
            }
        }
        if (vectors.Count < 3)
        {
            warning_panel_add_vector.gameObject.SetActive(true);
        }

    }

    public void Ok()
    {
        warning_panel_add_vector.gameObject.SetActive(false);
        warning_panel_execute.gameObject.SetActive(false);
        incorrect_input_visual.gameObject.SetActive(false);
        exit_panel.gameObject.SetActive(false);
    }

    public void Ok_input()
    {
        warning_panel_execute_input.gameObject.SetActive(false);
        warning_panel_add_vector_input.gameObject.SetActive(false);
        incorrect_input.gameObject.SetActive(false);
        exit_panel_input.gameObject.SetActive(false);
    }

    public void Lets_calculate()
    {
       calculate_panel.gameObject.SetActive(true);
    }

    public void Back_fromInputField()
    {
        calculate_panel.gameObject.SetActive(false);
        ClearAll_for_InputField();
    }

    public void Execute_InputField()
    {
        DisableDropdown(list_algorithms_InputField);
        if (list_algorithms_InputField.value == 0)
        {
            execute_button_input.interactable = false;
            if (vectors_n.Count> 0 && vectors_n.Count == vectors_n[0].Count)
            {
                var gram = new LLL(vectors_n);
                string s = gram.LLL_alg_str(vectors_n);
                string first_step = "";
                if (s.Length >= 16000)
                {
                    algorithm_Result = new Algorithm_result(result.text + s, 16000);
                    first_step = algorithm_Result.all_answers[0];
                    int n = first_step.Split('\n').Count();
                    RectTransform rectTransfrom = result.GetComponent<RectTransform>();
                    rectTransfrom.sizeDelta = new Vector2(rectTransfrom.rect.width, n * result.fontSize + 1000);
                }
                else
                {
                    algorithm_Result = new Algorithm_result(result.text + s, (result.text + s).Length);
                    algorithm_Result.all_answers.Remove("");
                    first_step = algorithm_Result.all_answers[0];
                    int n = first_step.Split('\n').Count();
                    RectTransform rectTransfrom = result.GetComponent<RectTransform>();
                    rectTransfrom.sizeDelta = new Vector2(rectTransfrom.rect.width, n * result.fontSize + 1000);
                }
                result.text += first_step;
                scrollbar.value = 0.96f;
            }
            else
            {
                incorrect_input.gameObject.SetActive(true);
            }
        }
        if (list_algorithms_InputField.value == 1)
        {
            if (vectors_n.Count > 0 && vectors_n.Count == vectors_n[0].Count && point_n.Count == 1)
            {
                var gram = new LLL(vectors_n);
                string first_step = "";
                string s = gram.Babai_alg_str(vectors_n, point_n[0]);
                if (s.Length >= 16000)
                {
                    algorithm_Result = new Algorithm_result(result.text + s, 16000);
                    first_step = algorithm_Result.all_answers[0];
                    int n = first_step.Split('\n').Count();
                    RectTransform rectTransfrom = result.GetComponent<RectTransform>();
                    rectTransfrom.sizeDelta = new Vector2(rectTransfrom.rect.width, n * result.fontSize + 100);
                }
                else
                {
                    algorithm_Result = new Algorithm_result(result.text + s, (result.text + s).Length);
                    algorithm_Result.all_answers.Remove("");
                    first_step = algorithm_Result.all_answers[0];
                    int n = first_step.Split('\n').Count();
                    RectTransform rectTransfrom = result.GetComponent<RectTransform>();
                    rectTransfrom.sizeDelta = new Vector2(rectTransfrom.rect.width, n * result.fontSize+100);
                }
                result.text = first_step;
                scrollbar.value = 0.96f;
            }
            else
            {
                incorrect_input.gameObject.SetActive(true);
            }
        }
    }

    public void Change_result()
    {
        if (scrollbar.value == 0)
        {
            if (result.text.Length > 0 && algorithm_Result is not null && algorithm_Result.state < algorithm_Result.all_answers.Count - 1)
            {
                string first_step = algorithm_Result.Next_step(algorithm_Result);
                int n = first_step.Split('\n').Count();
                RectTransform rectTransfrom = result.GetComponent<RectTransform>();
                rectTransfrom.sizeDelta = new Vector2(rectTransfrom.rect.width, n * result.fontSize + 100);
                result.text = first_step;
                scrollbar.value = 0.96f;
            }
        }
        if (scrollbar.value == 1)
        {
            if (result.text.Length > 0 && algorithm_Result is not null)
            {
                string first_step = algorithm_Result.Step_back(algorithm_Result);
                int n = first_step.Split('\n').Count();
                RectTransform rectTransfrom = result.GetComponent<RectTransform>();
                rectTransfrom.sizeDelta = new Vector2(rectTransfrom.rect.width, n * result.fontSize + 100);
                result.text = first_step;
                scrollbar.value = 0.96f;
            }
        }
    }

    public IEnumerator MoveOverSpeed(GameObject objectToMove, Vector3 end, float speed)
    {
        // speed should be 1 unit per second
        while (objectToMove.transform.position != end)
        {
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
    }

    public void CloseApp()
    {
        exit_panel.gameObject.SetActive(true);      
    }

    public void CloseApp_input()
    {
        exit_panel_input.gameObject.SetActive(true);
    }

    public void CloseExitPanel()
    {
        exit_panel.gameObject.SetActive(false);
    }

    public void makeTransparent()
    {
        if (visible_or_not.value < 255) 
        {
            OrthLattice_gameobject.SetActive(true);
            foreach (Transform item in OrthLattice_gameobject.transform)
            {
             
                    Color color = item.GetComponent<Renderer>().material.color;
                    color.a = 255 / 200 - visible_or_not.value / 200;
                    item.GetComponent<Renderer>().material.color = color;
            
            }
        }
        else
        {
            OrthLattice_gameobject.SetActive(false);
        }
    }

    public void Light_previos_state()
    {
        if (previuos_or_not.value < 255)
        {
             Previous_state.SetActive(true);
            foreach (Transform item in Previous_state.transform)
            {

                Color color = item.GetComponent<Renderer>().material.color;
                color.a = 255 / 200 - previuos_or_not.value / 200;
                item.GetComponent<Renderer>().material.color = color;

            }
        }
        else
        {
            Previous_state.SetActive(false);
        }
    }

    public void Light_Parallelepiped_state()
    {
        if (light_or_not.value < 255)
        {
            Parallelepiped.SetActive(true);
            foreach (Transform item in Parallelepiped.transform)
            {

                Color color = item.GetComponent<Renderer>().material.color;
                color.a = 255 / 200 - light_or_not.value / 200;
                item.GetComponent<Renderer>().material.color = color;

            }
        }
        else
        {
            Parallelepiped.SetActive(false);
        }
    }

    public void Light_Great_lattice()
    {
        if (built_or_not.value < 255)
        {
            Lattice.SetActive(true);
            foreach (Transform item in Lattice.transform)
            {
                Color color = item.GetComponent<Renderer>().material.color;
                color.a = 255 / 200 - built_or_not.value / 200;
                item.GetComponent<Renderer>().material.color = color;                
            }
        }
        else
        {
            Lattice.SetActive(false);
        }
    }

    public void Light_Points()
    {
        if (light_points_or_not.value < 255)
        {
            Points.SetActive(true);
            foreach (Transform item in Points.transform)
            {
                Color color = item.GetComponent<Renderer>().material.color;
                color.a = 255 / 200 - light_points_or_not.value / 200;
                item.GetComponent<Renderer>().material.color = color;
            }
        }
        else
        {
            Points.SetActive(false);
        }
    }

    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }

    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    void Update()
    {
        RotateCamera();
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Camera.main.fieldOfView--;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.fieldOfView++;
        }
        on_panel = IsPointerOverUIElement();
    }
}
