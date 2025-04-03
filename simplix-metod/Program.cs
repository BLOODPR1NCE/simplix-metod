using System;
using System.Linq;

public class SimplexMethod
{
    public enum OptimizationType { Maximize, Minimize }

    public static void Solve(double[] objectiveFunction, double[,] constraints, OptimizationType type)
    {
        if (objectiveFunction == null || constraints == null)
        {
            throw new ArgumentNullException("Входные данные не могут быть null");
        }
        int variablesCount = objectiveFunction.Length;
        int constraintsCount = constraints.GetLength(0);


        for (int i = 0; i < constraintsCount; i++)
        {
            if (constraints.GetLength(1) != variablesCount + 1)
            {
                throw new ArgumentException("Несоответствие размеров входных данных");

            }
        }

        double[,] table = new double[constraintsCount + 1, variablesCount + constraintsCount + 1];

        for (int i = 0; i < constraintsCount; i++)
        {
            for (int j = 0; j < variablesCount; j++)
            {
                table[i, j] = constraints[i, j];
            }
            table[i, variablesCount + i] = 1;
            table[i, variablesCount + constraintsCount] = constraints[i, variablesCount];
        }

        for (int j = 0; j < variablesCount; j++)
        {
            table[constraintsCount, j] = type == OptimizationType.Maximize
                ? -objectiveFunction[j]
                : objectiveFunction[j];
        }

        while (true)
        {
            int pivotCol = -1;
            double min = 0;
            for (int j = 0; j < variablesCount + constraintsCount; j++)
            {
                if (table[constraintsCount, j] < min)
                {
                    min = table[constraintsCount, j];
                    pivotCol = j;
                }
            }

            if (pivotCol == -1) break;

            int pivotRow = -1;
            min = double.MaxValue;
            for (int i = 0; i < constraintsCount; i++)
            {
                if (table[i, pivotCol] > 0)
                {
                    double ratio = table[i, variablesCount + constraintsCount] / table[i, pivotCol];
                    if (ratio < min)
                    {
                        min = ratio;
                        pivotRow = i;
                    }
                }
            }

            if (pivotRow == -1)
            {
                Console.WriteLine("Задача не имеет ограниченного решения");
                return;
            }


            double pivotElement = table[pivotRow, pivotCol];

 
            for (int j = 0; j < variablesCount + constraintsCount + 1; j++)
            {
                table[pivotRow, j] /= pivotElement;
            }

            for (int i = 0; i < constraintsCount + 1; i++)
            {
                if (i != pivotRow)
                {
                    double factor = table[i, pivotCol];
                    for (int j = 0; j < variablesCount + constraintsCount + 1; j++)
                    {
                        table[i, j] -= table[pivotRow, j] * factor;
                    }
                }
            }
        }

        double[] solution = new double[variablesCount];

        for (int j = 0; j < variablesCount; j++)
        {
            bool isBasic = false;
            double value = 0;
            int basicRow = -1;

            for (int i = 0; i < constraintsCount; i++)
            {
                if (table[i, j] == 1)
                {
                    if (!isBasic && basicRow == -1)
                    {
                        isBasic = true;
                        basicRow = i;
                        value = table[i, variablesCount + constraintsCount];
                    }
                    else
                    {
                        isBasic = false;
                        break;
                    }
                }
                else if (table[i, j] != 0)
                {
                    isBasic = false;
                    break;
                }
            }

            solution[j] = isBasic ? value : 0;
        }

        double objectiveValue = table[constraintsCount, variablesCount + constraintsCount];
        if (type == OptimizationType.Maximize)
        {
            Console.WriteLine($"Максимальное значение целевой функции: {objectiveValue}");
        }
        else
        {
            Console.WriteLine($"Минимальное значение целевой функции: {objectiveValue}");
        }

        Console.WriteLine("Значения переменных:");
        for (int i = 0; i < variablesCount; i++)
        {
            Console.WriteLine($"x{i + 1} = {solution[i]}");
        }
    }
}

class Program
{
    static void Main()
    {
        double[] objectiveMax = { 3, 4 };
        double[,] constraintsMax = {
            { 1, 2, 4 },
            { 1, 1, 3 },
            { 2, 1, 8 }
        };

        Console.WriteLine("Решение задачи максимизации:");
        SimplexMethod.Solve(objectiveMax, constraintsMax, SimplexMethod.OptimizationType.Maximize);

        double[] objectiveMin = { 3, 4 };
        double[,] constraintsMin = {
            { 1, 2, 4 },
            { 1, 1, 3 },
            { 2, 1, 8 }
        };

        Console.WriteLine("\nРешение задачи минимизации:");
        SimplexMethod.Solve(objectiveMin, constraintsMin, SimplexMethod.OptimizationType.Minimize);
    }
}