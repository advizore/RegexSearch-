namespace RegexSearch.Core.Models
{
    /// <summary>
    /// Информация о найденной подстроке
    /// </summary>
    public class MatchResult
    {
        public int Number { get; set; }          // Порядковый номер совпадения
        public string Value { get; set; } = "";  // Найденная подстрока
        public int Position { get; set; }        // Позиция начала (с нуля)
        public int Length { get; set; }          // Длина найденной подстроки
    }
}
