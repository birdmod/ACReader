namespace Client
{
    public class Helpers
    {
        public static string StringifySeasonEnum(SeasonsEnum s)
        {
            if (s == SeasonsEnum.WINTER)
                return "Winter";
            if (s == SeasonsEnum.SPRING)
                return "Spring";
            if (s == SeasonsEnum.SUMMER)
                return "Summer";
            if (s == SeasonsEnum.FALL)
                return "Fall";
            return "Unknown season!";
        }
    }
}
