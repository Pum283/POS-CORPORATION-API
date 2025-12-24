using Newtonsoft.Json;
using RestSharp;

namespace Application;

[RegisterService(typeof(IVietQRPaymentService))]
public class VietQRPaymentService : IVietQRPaymentService
{
    private readonly HttpClient _httpClient;
    public VietQRPaymentService()
    {
        _httpClient = new HttpClient();
    }
    public async Task<Bank> GetBanksAsync()
    {
        var response = await _httpClient.GetAsync("https://api.vietqr.io/v2/banks");
        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to fetch bank data.");

        var bankRawJson = await response.Content.ReadAsStringAsync();
        var listBankData = JsonConvert.DeserializeObject<Bank>(bankRawJson);
        if (listBankData == null)
            throw new NotFoundException("No bank data found.");

        return listBankData;
    }
    public byte[] GetVietQRImage(ApiRequest model)
    {
        var apiRequest = new ApiRequest
        {
            acqId = model.acqId,
            accountNo = model.accountNo,
            accountName = model.accountName,
            amount = model.amount,
            format = model.format,
            template = model.template
        };

        var jsonRequest = JsonConvert.SerializeObject(apiRequest);

        var client = new RestClient("https://api.vietqr.io/v2/generate");
        var request = new RestRequest();
        request.Method = Method.Post;
        request.AddHeader("Accept", "application/json");
        request.AddParameter("application/json", jsonRequest, ParameterType.RequestBody);

        var response = client.Execute(request);

        if (!response.IsSuccessful)
            throw new Exception("VietQR API call failed: " + response.ErrorMessage);

        var content = response.Content;
        var dataResult = JsonConvert.DeserializeObject<ApiResponse>(content!);

        if (dataResult?.data == null)
            throw new Exception("VietQR API response does not contain valid data.");

        var base64 = dataResult.data.qrDataUrl.Replace("data:image/png;base64,", "");

        return Convert.FromBase64String(base64);
    }
}
