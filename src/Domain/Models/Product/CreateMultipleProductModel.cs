namespace Domain;

public class CreateMultipleProductModel
{
	public List<CreateProductInfoModel> Products { get; set; } = new();
	public List<IFormFile> Pictures { get; set; } = new();
}
