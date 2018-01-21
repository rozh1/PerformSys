#region Copyright
/*
 * Copyright 2013-2018 Roman Klassen
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 */
 #endregion
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