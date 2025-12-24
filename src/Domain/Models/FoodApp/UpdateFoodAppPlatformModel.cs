
namespace Domain;

    public class UpdateFoodAppPlatformModel
    {
        //[Required, MaxLength(100)]
        //public string Name { get; set; } = default!;

        [MaxLength(500)]
        public string? Description { get; set; }
        public ShippingType? ShippingType { get; set; }
}

