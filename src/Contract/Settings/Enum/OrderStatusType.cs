namespace Contract;

public enum OrderStatusType
{
    /// <summary>
    /// Order is pending
    /// </summary>
    Pending = 0,
    /// <summary>
    /// Order is confirmed
    /// </summary>
    Confirmed = 1,
    /// <summary>
    /// Order is shipped
    /// </summary>
    Shipped = 2,
    /// <summary>
    /// Order is delivered
    /// </summary>
    Delivered = 3,
    /// <summary>
    /// Order is cancelled
    /// </summary>
    Cancelled = 4
}
