# HttpContent

## ByteArrayContent
* -> a content represented by **a byte array**, 
* -> also serves as **a base class** for **`StringContent`** and **`FormUrlEncodedContent`**

## StringContent 
* -> a **string-based content**, by default serialized as **`text/plain`** Content-Type with UTF-8 encoding.

## FormUrlEncodedContent
* -> a content with **`name/value tuples`** serialized as **application/x-www-form-urlencoded** Content-Type

## MultipartContent 
* -> a content that **can serialize multiple different HttpContent objects** as **`multipart/* Content-Type`**
* _povides `a collection of HttpContent objects` that get serialized using the `multipart/* content type` specification._

## JsonContent 
* -> a content that serializes objects as **application/json** Content-Type with UTF-8 encoding by default

## StreamContent
* -> provides HTTP content based on a **stream**

## MultipartFormDataContent
* -> provides **a container for content encoded using 'multipart/form-data'** MIME type

```c#
using (var httpClient = new HttpClient())
{
    var formData = new MultipartFormDataContent();

    var jsonContent = new StringContent(JsonConvert.SerializeObject(accountCurrent), Encoding.UTF8, "application/json");
    formData.Add(jsonContent, "userInfo");

    foreach (string key in httpRequest.Form.Keys)
    {
        var value = httpRequest.Form[key];
        formData.Add(new StringContent(value), key);
    }

    foreach (string fileKey in files.Keys)
    {
        var file = files[fileKey];
        if (file.ContentLength > 0)
        {
            using (var fileStream = file.InputStream)
            {
                var fileContent = new StreamContent(fileStream);
                formData.Add(fileContent, "files", file.FileName);
            }
        }
    }

    var commonApi = new CommonApi();
    var resultApi = await commonApi.Call_multipart_API(url, "POST", formData, "multipart/form-data");

    return Json(resultApi);
}
```

## ReadOnlyMemoryContent
* -> System.Net.Http.ReadOnlyMemoryContent