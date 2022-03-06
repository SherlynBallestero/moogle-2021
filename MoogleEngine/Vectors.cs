using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
namespace MoogleEngine
{
    public class Vector
    {
        public double[] elements;
        public Vector(double[] elements)
        {
            if (elements == null)
                throw new ArgumentException("The input vector can't be null");
            this.elements = elements;
        }
        //properties
        public int Size
        {
            get { return this.elements.Length; }
        }
        // Methods
        public override string ToString()
        {
            StringBuilder vector = new StringBuilder("vector: ");
            for (int i = 0; i < this.Size; i++)
            {
                if (i == 0)
                    vector.AppendFormat("{0,-3}", "[");
                vector.AppendFormat("{0,-4}", this.elements[i]);

                if (i == this.Size - 1)
                    vector.AppendFormat("{0,-3}", "]");
            }
            return vector.ToString();
        }
        public bool Equals(Vector v)
        {
            for (int i = 0; i < this.Size; i++)
            {
                if (this.elements[i] != v.elements[i])
                    return false;
            }
            return true;
        }
        public Vector Suma(Vector v)
        {
            int l = Math.Max(this.Size, v.Size);
            double[] result = new double[l];
            for (int i = 0, j = 0; i < result.Length; i++)
            {
                if (this.Size > v.Size)
                {
                    if (j < v.Size)
                    {
                        result[i] = this.elements[i] + v.elements[j];
                        j++;
                    }
                    else
                    {
                        result[i] = this.elements[i];
                    }
                }
                else
                {
                    if (j < this.Size)
                    {
                        result[i] = this.elements[j] + v.elements[i];
                        j++;
                    }
                    else
                    {
                        result[i] = v.elements[i];
                    }

                }
            }
            return new Vector(result);

        }
        public Vector ProductByEscalar(double a)
        {
            Vector answer = new Vector(new double[this.Size]);
            for (int i = 0; i < this.Size; i++)
            {
                answer.elements[i] = this.elements[i] * a;
            }
            return answer;

        }
        public double Norma()
        {
            double a = 0;
            for (int i = 0; i < this.Size; i++)
            {
                a += Math.Pow(this.elements[i], 2);
            }
            return Math.Sqrt(a);
        }
        public double distance(Vector v)
        {
            Vector vector1 = v.ProductByEscalar(-1);
            vector1 = this.Suma(vector1);
            double answer = vector1.Norma();
            return answer;

        }
        public double EscalarProduct(Vector v)
        {
            double answer = 0;
            if (this.Size != v.Size)
            {
                throw new Exception("the vectors should have the same size");
            }
            for (int i = 0; i < v.Size; i++)
            {
                answer += this.elements[i] * v.elements[i];
            }
            return answer;
        }

        public double CosVector(Vector v)
        {
            return EscalarProduct(v) / (this.Norma() * v.Norma() + 1e-9);
        }

    }
}