using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGame
{
    public class Matrix2D
    {
        private double[,] m_matrix;

        public static Matrix2D Multiply(Matrix2D left, Matrix2D right)
        {
            Matrix2D result = new Matrix2D();
            result.m_matrix[0, 0] = 0.0;
            result.m_matrix[1, 1] = 0.0;
            result.m_matrix[2, 2] = 0.0;

            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    for(int k = 0; k < 3; k++)
                    {
                        result.m_matrix[i, j] += left.m_matrix[i, k] * right.m_matrix[k, j];
                    }
                }
            }

            return result;
        }

        public                 Matrix2D()
        {
            m_matrix = new double[3, 3];
            
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    m_matrix[i, j] = 0;
                }
                m_matrix[i, i] = 1;
            }
        }

        public ref double      GetAt(int line, int column)
        {
            return ref m_matrix[line, column];
        }

        public void            Scale(double scaleX, double scaleY)
        {
            Matrix2D scaleMatrix = new Matrix2D();
            
            scaleMatrix.m_matrix[0, 0] = scaleX;
            scaleMatrix.m_matrix[1, 1] = scaleY;
            scaleMatrix.m_matrix[2, 2] = 1.0;

            m_matrix = Multiply(this, scaleMatrix).m_matrix;
        }

        public void            Rotate(double radians)
        {
            Matrix2D rotationMatrix = new Matrix2D();

            rotationMatrix.m_matrix[0, 0] = Math.Cos(radians);
            rotationMatrix.m_matrix[0, 1] = Math.Sin(radians);
            rotationMatrix.m_matrix[1, 0] = -Math.Sin(radians);
            rotationMatrix.m_matrix[1, 1] = Math.Cos(radians);
            rotationMatrix.m_matrix[2, 2] = 1.0;

            m_matrix = Multiply(this, rotationMatrix).m_matrix;
        }

        public void            Translate(double x, double y)
        {
            Matrix2D translationMatrix = new Matrix2D();

            translationMatrix.m_matrix[0, 0] = 1.0;
            translationMatrix.m_matrix[1, 1] = 1.0;
            translationMatrix.m_matrix[2, 0] = x;
            translationMatrix.m_matrix[2, 1] = y;
            translationMatrix.m_matrix[2, 2] = 1.0;

            m_matrix = Multiply(this, translationMatrix).m_matrix;
        }

        public void            TransformPoint(ref SPoint point)
        {
            double[] otherMatrix = { point.x, point.y, 1.0 };
            double[] result = {0.0, 0.0, 0.0};

            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    result[i] += otherMatrix[j] * m_matrix[j, i];
                }
            }

            point.x = result[0];
            point.y = result[1];
        }

        public void            TransformPoints(ref SPoint[] points)
        {
            for(int i = 0; i < points.Length; i++)
                TransformPoint(ref points[i]);
        }
    }
}