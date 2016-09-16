using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PhysicUtils
{

}

/// <summary>
/// 已知Unity3d的force作用刷新时间为FixedDeltaTime，再每个deltaTime中，是一个恒力，所以利用时间模拟尝试预测duration之后的物体位置
/// drag_acceleration = -dragFactor*Velocity
/// </summary>
public class RigidbodySolver
{
    

    /// <summary>
    /// Drag代表的含义，Velocity表示drag=acc/v；SqrVelocity表示drag=acc/v^2
    /// </summary>
    public DragModes DragMode { get; private set; }

    /// <summary>
    /// Solver的模式，Interation表示使用迭代模拟,与Unity3d较为符合；EquationSolution表示使用运动微分方程的解，更接近理论值
    /// </summary>
    public SolverModes SolverMode { get; private set; }

    /// <summary>
    /// 受到的恒力大小
    /// </summary>
    public Vector3 Force { get; private set; }

    /// <summary>
    /// 物体质量大小
    /// </summary>
    public float Mass { get; private set; }

    /// <summary>
    /// 阻力系数大小
    /// </summary>
    public float Drag { get; private set; }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="force"></param>
    /// <param name="mass"></param>
    /// <param name="drag"></param>
    /// <param name="dragMode">Drag代表的含义，Velocity表示drag=acc/v；SqrVelocity表示drag=acc/v^2</param>
    /// <param name="solverMode">Solver的模式，Interation表示使用迭代模拟,与Unity3d较为符合；EquationSolution表示使用运动微分方程的解，更接近理论值</param>
    public RigidbodySolver(Vector3 force, float mass, float drag, DragModes dragMode, SolverModes solverMode)
    {
        InitProperty(force, mass, drag, dragMode, solverMode);
    }

    /// <summary>
    /// 计算时间duration之后的位置与速度，初始位置设为(0,0,0)
    /// </summary>
    /// <param name="startVelocity"></param>
    /// <param name="duration"></param>
    /// <param name="fixedDeltatime"></param>
    /// <returns></returns>
    public SolverResult Solve(Vector3 startVelocity, float duration, float fixedDeltatime)
    {
        switch (DragMode)
        {
            case DragModes.Velocity:
                switch (SolverMode)
                {
                    case SolverModes.Interation:
                        return Solve_Interation_V(startVelocity, duration, duration, fixedDeltatime)[0];
                    case SolverModes.EquationSolution:
                        return Solve_EquationSulotion_V(startVelocity, duration);
                }
                break;
        }

        return new SolverResult();
    }

    /// <summary>
    /// 计算时间duration之后,美国sampleStep的位置与速度集合，初始位置设为(0,0,0)
    /// </summary>
    /// <param name="startVelocity"></param>
    /// <param name="duration"></param>
    /// <param name="sampleStep"></param>
    /// <param name="fixedDeltatime">SolverMode为Interation才有意义</param>
    /// <returns></returns>
    public List<SolverResult> Solve(Vector3 startVelocity, float duration, float sampleStep, float fixedDeltatime)
    { 
        switch (DragMode)
        {
            case DragModes.Velocity:
                switch (SolverMode)
                {
                    case SolverModes.Interation:
                        return Solve_Interation_V(startVelocity, duration, sampleStep, fixedDeltatime);
                    case SolverModes.EquationSolution:
                        return Solve_EquationSulotion_V(startVelocity, duration, sampleStep);
                }
                break;
        }

        return null;
    }

    private void InitProperty(Vector3 force, float mass, float drag, DragModes dragMode, SolverModes solverMode)
    {
        Force = force;
        Mass = mass;
        Drag = drag;
        DragMode = dragMode;
        SolverMode = solverMode;
       
    }

    private List<SolverResult> Solve_Interation_V(Vector3 startVelocity, float duration, float sampleStep, float fixedDeltatime)
    {
        Vector3 m_ForceAcceleration = Force / Mass;
        float m_SqrDrag = Drag * Drag;
        Vector3 position = Vector3.zero;
        float currentTime = 0;
        Vector3 velocity = startVelocity;
        float timeStep = 0;
        List<SolverResult> resultList = new List<SolverResult>();
        while (currentTime <= duration)
        {
            Vector3 acceleration = m_ForceAcceleration - Drag * velocity;
            position += velocity * fixedDeltatime;
            velocity += acceleration * fixedDeltatime;
          
            if(timeStep >= sampleStep)
            {
                SolverResult result = new SolverResult() { TimeStamp = currentTime, Position=position, Velocity = velocity};
                resultList.Add(result);
                timeStep = 0;
            }
            currentTime += fixedDeltatime;
            timeStep += fixedDeltatime;
        }
        if(resultList.Count == 0)
        {
            SolverResult result = new SolverResult() { TimeStamp = currentTime, Position = position, Velocity = velocity };
            resultList.Add(result);
        }
        return resultList;
    }

    private List<SolverResult> Solve_EquationSulotion_V(Vector3 startVelocity, float duration, float sampleStp)
    {
        List<SolverResult> resultList = new List<SolverResult>();
        float time = sampleStp;
        while(time<=duration)
        {
            SolverResult result = Solve_EquationSulotion_V(startVelocity, time);
            resultList.Add(result);
            time += sampleStp;
        }
        return resultList;
    }

    //运动方程为F-drag*v(t) = m*a,化为微分方程为F-drag*v=m*(dv/dt)，解得v = F/m-exp(-drag*t)*F/(m*drag)
    //位移ds = vdt,解得s = F/m*t+exp(-drag*t)*F/(m*drag*drag)
    private SolverResult Solve_EquationSulotion_V(Vector3 startVelocity, float duration)
    {
        Vector3 m_ForceAcceleration = Force / Mass;
        float m_SqrDrag = Drag * Drag;
        Vector3 speed = m_ForceAcceleration - Mathf.Exp(-Drag * duration) * m_ForceAcceleration / Drag;
        Vector3 position = m_ForceAcceleration * duration + Mathf.Exp(-Drag * duration) * m_ForceAcceleration / m_SqrDrag;
        return new SolverResult() { TimeStamp = duration, Position = position, Velocity = speed};
    }

    /// <summary>
    /// Drag代表的含义，drag=acc/v或者drag=acc/v^2
    /// </summary>
    public enum DragModes
    {
        Velocity,
        SqrVelocity
    }

    /// <summary>
    /// Solver的模式，Interation表示使用迭代模拟，EquationSolution表示使用接运动微分方程
    /// </summary>
    public enum SolverModes
    {
        Interation,
        EquationSolution
    }

    public struct SolverResult
    {
        public float TimeStamp { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
    }


}

