        using RestSharp;



namespace BestStoreMVC.Services;
public class EmailSender(IConfiguration configuration)
{
    private readonly string apiKey = configuration["BrevoSettings:apiKey"]!;
    private readonly string SenderName = configuration["BrevoSettings:SenderName"]!;
    private readonly string SenderEmail = configuration["BrevoSettings:SenderEmail"]!;


    public async Task SendEmailAsync(string toEmail, string toName, string subject, string message) {

       

        var client = new RestClient("https://api.brevo.com/v3/smtp/email");
        var request = new RestRequest("",Method.Post);
        request.AddHeader("api-key", apiKey);
        request.AddHeader("Content-Type", "application/json");

        var body = new
        {
            sender = new { name = SenderName , email= SenderEmail },
            to = new[] { new { email = toEmail ,name =toName  } },
            Subject = subject,
            htmlContent = $"<html> <body>{message}</body></html>" 


        };

        request.AddJsonBody(body);

        RestResponse response =await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            Console.WriteLine("Brevo error: " + response.Content);
        }
    }
   
}
