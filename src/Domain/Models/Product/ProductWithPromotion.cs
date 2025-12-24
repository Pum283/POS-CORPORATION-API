namespace Domain;


    public class ProductWithPromotion
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public decimal Quantity { get; set; }
        public ProductGroupBasicModel? ProductGroup { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }

        // Mới thêm: mã khuyến mãi đang được áp dụng (nếu có)
        public string? AppliedPromotionCode { get; set; }
        public Guid? AppliedPromotionId { get; set; }

        public string? SKU { get; set; }
        public string? Picture { get; set; }
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public bool IsActive { get; set; }
        public decimal? VAT { get; set; }
        public DateTime CreateDate { get; set; }
    }
