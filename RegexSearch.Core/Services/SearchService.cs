using System.Text.RegularExpressions;
using RegexSearch.Core.Models;

namespace RegexSearch.Core.Services
{
    /// <summary>
    /// Сервис поиска: два независимых алгоритма
    /// 1. Через регулярные выражения
    /// 2. Через собственный конечный автомат
    /// </summary>
    public class SearchService
    {
        // Регулярное выражение для СНИЛС: XXX-XXX-XXX XX
        private readonly string _pattern = @"\b\d{3}-\d{3}-\d{3}\s\d{2}\b";

        /// <summary>
        /// Поиск с помощью регулярных выражений (библиотечный способ)
        /// </summary>
        public List<MatchResult> SearchWithRegex(string text)
        {
            var results = new List<MatchResult>();
            if (string.IsNullOrEmpty(text))
                return results;

            var matches = Regex.Matches(text, _pattern);

            int number = 1;
            foreach (Match match in matches)
            {
                results.Add(new MatchResult
                {
                    Number = number++,
                    Value = match.Value,
                    Position = match.Index,
                    Length = match.Length
                });
            }

            return results;
        }

        /// <summary>
        /// Поиск с помощью собственного конечного автомата
        /// </summary>
        public List<MatchResult> SearchWithAutomaton(string text)
        {
            var results = new List<MatchResult>();
            if (string.IsNullOrEmpty(text))
                return results;

            int i = 0;
            int number = 1;

            while (i < text.Length)
            {
                var state = SnilsState.S0;
                int startPos = i;
                int j = i;

                while (j < text.Length)
                {
                    char c = text[j];
                    SnilsState nextState = GetNextState(state, c);

                    if (nextState == state && state == SnilsState.S0)
                    {
                        // Остаёмся в S0 — просто двигаемся дальше
                        break;
                    }

                    if (nextState == SnilsState.S0 && state != SnilsState.S0)
                    {
                        // Ошибка распознавания — прерываем текущую попытку
                        break;
                    }

                    state = nextState;
                    j++;

                    // Достигли принимающего состояния
                    if (state == SnilsState.S14)
                    {
                        // Проверяем границы (чтобы было похоже на \b)
                        bool leftOk = (startPos == 0) || !char.IsLetterOrDigit(text[startPos - 1]);
                        bool rightOk = (j >= text.Length) || !char.IsLetterOrDigit(text[j]);

                        if (leftOk && rightOk)
                        {
                            string value = text.Substring(startPos, j - startPos);
                            results.Add(new MatchResult
                            {
                                Number = number++,
                                Value = value,
                                Position = startPos,
                                Length = j - startPos
                            });
                        }
                        break;
                    }
                }

                i++;
            }

            return results;
        }

        /// <summary>
        /// Функция переходов конечного автомата
        /// </summary>
        private SnilsState GetNextState(SnilsState current, char c)
        {
            switch (current)
            {
                case SnilsState.S0:
                    return char.IsDigit(c) ? SnilsState.S1 : SnilsState.S0;

                case SnilsState.S1:
                    return char.IsDigit(c) ? SnilsState.S2 : SnilsState.S0;

                case SnilsState.S2:
                    return char.IsDigit(c) ? SnilsState.S3 : SnilsState.S0;

                case SnilsState.S3:
                    return c == '-' ? SnilsState.S4 : SnilsState.S0;

                case SnilsState.S4:
                    return char.IsDigit(c) ? SnilsState.S5 : SnilsState.S0;

                case SnilsState.S5:
                    return char.IsDigit(c) ? SnilsState.S6 : SnilsState.S0;

                case SnilsState.S6:
                    return char.IsDigit(c) ? SnilsState.S7 : SnilsState.S0;

                case SnilsState.S7:
                    return c == '-' ? SnilsState.S8 : SnilsState.S0;

                case SnilsState.S8:
                    return char.IsDigit(c) ? SnilsState.S9 : SnilsState.S0;

                case SnilsState.S9:
                    return char.IsDigit(c) ? SnilsState.S10 : SnilsState.S0;

                case SnilsState.S10:
                    return char.IsDigit(c) ? SnilsState.S11 : SnilsState.S0;

                case SnilsState.S11:
                    return c == ' ' ? SnilsState.S12 : SnilsState.S0;

                case SnilsState.S12:
                    return char.IsDigit(c) ? SnilsState.S13 : SnilsState.S0;

                case SnilsState.S13:
                    return char.IsDigit(c) ? SnilsState.S14 : SnilsState.S0;

                case SnilsState.S14:
                    return SnilsState.S0; // уже приняли, дальше не идём

                default:
                    return SnilsState.S0;
            }
        }
    }
}
