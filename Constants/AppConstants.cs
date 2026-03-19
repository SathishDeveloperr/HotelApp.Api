namespace HotelApp.Api.Constants;

public static class AppConstants
{
    public const string RoleAdmin = "Admin";
    public const string RoleStaff = "Staff";
    public const string RoleCustomer = "Customer";

    public const string TableAvailable = "Available";
    public const string TableOccupied = "Occupied";
    public const string TableReserved = "Reserved";

    public const string ReservationPending = "Pending";
    public const string ReservationConfirmed = "Confirmed";
    public const string ReservationCancelled = "Cancelled";

    public const string OrderPlaced = "Placed";
    public const string OrderPreparing = "Preparing";
    public const string OrderCompleted = "Completed";
    public const string OrderPaid = "Paid";

    public const string PaymentPending = "Pending";
    public const string PaymentCompleted = "Completed";
    public const string PaymentFailed = "Failed";
}
