# Example

```csharp
    static class Example {
        static void example() {
            Client.Init("localhost:8000", new User(..));
            
            string search = "{ 'input': 'orange' }";
            Client.Post("/ingredients/search", search)
                .Then(res => Debug.Log(res.Get("list")))
                .Catch(err => Debug.LogError(err)) // Optional
                .Send();
        }
    }
```
