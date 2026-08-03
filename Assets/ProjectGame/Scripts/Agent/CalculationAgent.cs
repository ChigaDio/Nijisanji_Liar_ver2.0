using System;
using UnityEngine;
using GameCore.Classes;
public static class CalculationAgent
{
    /// <summary>
    /// 好感度計算
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="targetAgent"></param>
    /// <param name="baseValue"></param>
    /// <param name="multiplier"></param>
    /// <returns></returns>
    public static float FavorabilityCalculation(Agent agent,Agent targetAgent, float baseValue, float multiplier)
    {
        //好感度の計算式（自分のステータス）
        float favorability = baseValue * multiplier;

        //自分のステータスを考慮
        favorability += agent.Character_stats.Appeal * 0.1f; //魅力の影響
        favorability += agent.Character_stats.Charisma * 0.05f; //魅力の影響

        return favorability;


    }

    /// <summary>
    /// 疑惑度計算
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="targetAgent"></param>
    /// <param name="baseValue"></param>
    /// <param name="multiplier"></param>
    /// <returns></returns>
    public static float SuspicionCalculation(Agent agent, Agent targetAgent, float baseValue, float multiplier)
    {
        //疑惑度の計算式（自分のステータス）
        float suspicion = baseValue * multiplier;

        //自分のステータスを考慮
        suspicion += agent.Character_stats.Intuition * 0.1f; //直感の影響
        suspicion += agent.Character_stats.Reasoning * 0.05f; //ロジックの影響

        return suspicion;
    }

    /// <summary>
    /// 嘘を見抜く確率の計算
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="targetAgent"></param>
    /// <param name="baseValue"></param>
    /// <param name="multiplier"></param>
    /// <returns></returns>
    public static float LieDetectionProbabilityCalculation(Agent agent, Agent targetAgent, float baseValue, float multiplier)
    {
        //自分のステータスと相手のステータスを考慮して嘘を見抜く確率を計算する
        float probability = baseValue * multiplier;

        //自分のステータスを考慮
        probability += agent.Character_stats.Intuition * 0.1f; //直感の影響
        probability += agent.Character_stats.Reasoning * 0.05f; //ロジックの影響

        //相手のステータスを考慮
        probability -= targetAgent.Character_stats.Deception * 0.1f; //演技力の影響
        probability -= targetAgent.Character_stats.Stealth * 0.05f; //ステルスの影響

        //確率が0未満にならないようにする
        probability = Mathf.Max(0, probability);

        return probability;
    }

}
