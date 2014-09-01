using System;
using client.Config.Data;

namespace client
{
    /// <summary>
    ///     Манагер последовательности сценария
    /// </summary>
    internal class ScenarioStepsManager
    {
        /// <summary>
        ///     Время срабатывания шагов сценария
        /// </summary>
        private readonly DateTime[] _actionTimes;

        /// <summary>
        ///     шаги сценария
        /// </summary>
        private readonly ScenarioStep[] _scenarioSteps;

        /// <summary>
        ///     Манагер последовательности сценария
        /// </summary>
        /// <param name="scenarioSteps">шаги сценария</param>
        public ScenarioStepsManager(ScenarioStep[] scenarioSteps)
        {
            _scenarioSteps = scenarioSteps;
            _actionTimes = InitActionTimes(scenarioSteps);
        }

        /// <summary>
        ///     Инициализация времен срабатывания шагов
        /// </summary>
        /// <param name="scenarioSteps">шаги сценария</param>
        /// <returns>времена срабатвания</returns>
        private DateTime[] InitActionTimes(ScenarioStep[] scenarioSteps)
        {
            var actionTimes = new DateTime[scenarioSteps.Length];
            DateTime curTime = DateTime.UtcNow;
            for (int i = 0; i < actionTimes.Length; i++)
            {
                curTime = curTime.Add(scenarioSteps[i].Duration);
                actionTimes[i] = curTime;
            }
            return actionTimes;
        }

        /// <summary>
        ///     Ищет индекс текущего шага сценария
        /// </summary>
        /// <param name="actionTimes">времена срабатывания</param>
        /// <returns>индекс</returns>
        private int FindScenarioStepIndex(DateTime[] actionTimes)
        {
            DateTime curTime = DateTime.UtcNow;
            for (int i = 0; i < actionTimes.Length; i++)
            {
                if (curTime < actionTimes[i])
                {
                    return i;
                }
            }
            return -1; //не нашли индекс
        }

        /// <summary>
        ///     Получает текущее действие по сценарию
        /// </summary>
        public ScenarioActions GetCurrentScenarioAction()
        {
            int index = FindScenarioStepIndex(_actionTimes);
            if (index >= 0)
            {
                return _scenarioSteps[index].Action;
            }
            return ScenarioActions.Stop;
        }
    }
}