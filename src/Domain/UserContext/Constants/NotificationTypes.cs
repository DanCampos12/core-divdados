namespace Core.Divdados.Domain.UserContext.Constants;

public static class NotificationTypes
{
    public const string OBJECTIVE_EXPIRED = "objectiveExpired";
    public const string OBJECTIVE_COMPLETED = "objectiveCompleted";
    public const string OBJECTIVE_HALF_COMPLETED = "objectiveHalfCompleted";
    public const string OBJECTIVE_FINISHED = "objectiveFinished";
    public const string OBJECTIVE_EXPIRING_IN_FIVE_DAYS = "objectiveExpiringInFiveDays";
    public const string OBJECTIVE_EXPIRING_TOMORROW = "objectiveExpiringTomorrow";
    public const string OPERATION_FIVE_HUNDRED_OUTFLOW = "operationFiveHundredOutflow";
    public const string OPERATION_ONE_THOUSAND_OUTFLOW = "operationOneThousandOutflow";
    public const string OPERATION_TWO_THOUSAND_OUTFLOW = "operationTwoThousandOutflow";
    public const string OPERATION_THREE_THOUSAND_OUTFLOW = "operationThreeThousandOutflow";
    public const string OPERATION_FIVE_THOUSAND_OUTFLOW = "operatioFiveThousandOutflow";
    public const string EFFECT_OPERATION = "effectOperation";
    public const string CATEGORY_BEST_RESULTS = "categoryBestResults";
    public const string CATEGORY_WORST_RESULTS = "categoryWorstResults";
    public const string CATEGORY_LIMIT_EXCEEDED = "categoryLimitExceeded";
}