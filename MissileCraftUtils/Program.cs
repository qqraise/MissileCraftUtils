using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MissileCraftUtils
{
    class Program
    {
        static void Main(string[] args)
        {
            
            RigidbodySolver solver = new RigidbodySolver(new Vector3(10, 0, 50), 5, 1, RigidbodySolver.DragModes.Velocity, RigidbodySolver.SolverModes.EquationSolution);
            var result = solver.Solve(Vector3.zero, 10, 0.02f);
            Console.WriteLine(result.TimeStamp + " " + result.Position + " " + result.Velocity);
            Console.ReadKey();
        }
    }
}
