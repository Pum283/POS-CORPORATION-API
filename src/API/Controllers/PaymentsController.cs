namespace API.Controllers;

[Route("payment")]
public class PaymentsController(IVietQRPaymentService _service) : BaseApiController
{
    [HttpPost("generate-qr")]
    public IActionResult GetVietQRImage([FromBody] ApiRequest model)
    {
        var imageBytes = _service.GetVietQRImage(model);
        if (imageBytes == null || imageBytes.Length == 0)
        {
            throw new NotFoundException("QR code image not found.");
        }
        return File(imageBytes, "image/png");
    }
    [HttpGet("banks")]
    public async Task<Bank> GetBanks()
    {
        var banks = await _service.GetBanksAsync();
        return banks;
    }
}
