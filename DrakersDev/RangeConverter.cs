namespace DrakersDev
{
    public class RangeConverter
    {
        private readonly Boolean isInverted;
        /// <summary>
        /// 기준 최소값
        /// </summary>
        public Decimal StandardMin { get; private set; }

        /// <summary>
        /// 기준 최대값
        /// </summary>
        public Decimal StandardMax { get; private set; }

        private readonly Decimal compMin;
        /// <summary>
        /// 비교 최소값
        /// </summary>
        public Decimal ComparisonMin { get; private set; }

        private readonly Decimal compMax;
        /// <summary>
        /// 비교 최대값
        /// </summary>
        public Decimal ComparisonMax { get; private set; }

        /// <summary>
        /// 비율
        /// </summary>
        public Decimal Ratio { get; private set; }

        public RangeConverter(Decimal standardMin, Decimal standardMax, Decimal comparisonMin, Decimal comparisonMax)
        {
            Boolean invertedStandard = standardMin > standardMax;
            Boolean invertedComparison = comparisonMin > comparisonMax;
            this.isInverted = !((invertedStandard && invertedComparison) || (!invertedStandard && !invertedComparison));
            this.StandardMin = Math.Min(standardMin, standardMax);
            this.StandardMax = Math.Max(standardMin, standardMax);
            if (this.isInverted)
            {
                this.ComparisonMin = Math.Max(comparisonMin, comparisonMax);
                this.ComparisonMax = Math.Min(comparisonMin, comparisonMax);
            }
            else
            {
                this.ComparisonMin = Math.Min(comparisonMin, comparisonMax);
                this.ComparisonMax = Math.Max(comparisonMin, comparisonMax);
            }
            this.compMin = Math.Min(this.ComparisonMin, this.ComparisonMax);
            this.compMax = Math.Max(this.ComparisonMin, this.ComparisonMax);

            Decimal standardLength = invertedStandard ? this.StandardMin - this.StandardMax : this.StandardMax - this.StandardMin;
            Decimal comparisonLengh = invertedComparison ? this.ComparisonMin - this.ComparisonMax : this.ComparisonMax - this.ComparisonMin;
            this.Ratio = standardLength / comparisonLengh;
        }

        /// <summary>
        /// 비교 단위의 값을 기준 단위값으로 변환
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Decimal ConvertToStandardUnit(Decimal value)
        {
            if (this.compMin <= value && value <= this.compMax)
            {
                return this.isInverted
                    ? this.StandardMax - this.Ratio * (value - this.ComparisonMax)
                    : this.Ratio * (value - this.ComparisonMin) + this.StandardMin;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        /// <summary>
        /// 기준 단위의 값을 비교 단위값으로 변환
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Decimal ConvertToComparisionUnit(Decimal value)
        {
            return this.StandardMin <= value && value <= this.StandardMax
                ? this.isInverted
                    ? this.ComparisonMin - (value - this.StandardMin) / this.Ratio
                    : (value - this.StandardMin) / this.Ratio + this.ComparisonMin
                : throw new ArgumentOutOfRangeException(nameof(value));
        }
    }
}
