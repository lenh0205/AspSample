
var _client = new HttpClient();

// GetAsync, PostAsync, PutAsync, DeleteAsync
var response = await _client.GetAsync("https://api.example.com/values");

response.EnsureSuccessStatusCode();
return await response.Content.ReadAsStringAsync();
