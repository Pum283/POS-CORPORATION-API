namespace Application;

public interface IVietQRPaymentService
{
    byte[] GetVietQRImage(ApiRequest model);
    Task<Bank> GetBanksAsync();
}
