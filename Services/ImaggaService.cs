using System.Net.Http;
using System.Threading.Tasks;

namespace Cloud1.Services
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ImaggaService
    {
        private readonly HttpClient _httpClient;

        public ImaggaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CheckForIceCream(string imageUrl)
        {
            // שלב את ה-GateWay URL
            string gatewayUrl = "http://localhost:5122/api/imagga";

            // הכנס את הפרמטרים הדרושים לפונקציה ב-GateWay
            string requestUrl = $"{gatewayUrl}?imageUrl={imageUrl}";

            // שלב את כל הדרישות נוספות שלך, אולי כותרת Authorization וכדומה

            // שלב את ה-HTTP פעולה שלך (GET, POST, וכו')
            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

            // טפל בתשובה כמו שצריך
            if (response.IsSuccessStatusCode)
            {
                // טפל בתשובה המגיעה מ-GateWay
                // בדוק אם יש גלידה וכו'
                return response.Content.ReadAsStringAsync().Result.Contains("ice cream");
            }
            else
            {
                // טפל בשגיאה, אם יש
                // כאן תוכל לטפל בכל סוג של שגיאה שיש לך בתשובה
                return false;
            }
        }
    }
}