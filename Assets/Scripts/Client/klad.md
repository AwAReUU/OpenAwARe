```csharp
Client.GetInstance().Post("list/get", body)
    .Then(res => {})
    .Catch(err => {})
    .Finish();
```