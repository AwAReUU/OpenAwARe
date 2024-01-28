# AwARe Server

## How to build Unity with server connection

Before you can use any server features inside the Unity app, you must connect Unity with an instance of this server.

1. Start the AwARe server on a local device or on a remote server. For instructions see "How to run".
2. Load the AwARe project in the Unity Editor.
3. Make sure the ClientSupport scene is loaded.
4. Go to the GameObject "ClientSetup".
5. Scroll down to the "ClientSetup" (script) component.
6. Fill in the server adress and port. Use "localhost" if the server is running on the same machine and you only want to test the app inside the Editor. The default port number is "8000". If the server is running on a different device than the app is, for example on mobile, please read "How to build for Android/iOS".

## How to build for Android/iOS

On Android/iOS you can't use "localhost" to connect with the server running on a desktop. Instead you have to fill in the ip adress:
1. Open a terminal on your desktop.
2. Run `$ ipconfig`
3. Copy the "IPv4" address. 
4. Fill in the adress and port number in the Unity Editor (ClientSetup). The port number is "8000", if you didn't change it.

Now you can build for Android/iOS, but make sure:
- The server/desktop is configured to listen for incoming connections on the previously obtained ip adress and port.
- The Firewalls allow incoming connections on the previously obtained ip adress and port.


## How to run

1. Navigate to directory:

```console
$ cd {repository}/Server
```

2. Install dependencies:

```console
$ npm install
```

3. Start the server:

```console
$ npm run dev
```



## Debugging

For testing purposes, you can disable the login authorization for protected routes
by setting the `VALIDATION` environment variable to "FALSE".

## Testing

Run tests with `npm test`. For code coverage use `npm run coverage`.

## Routes

### Authentication: _/auth_

<table >
<thead>
  <tr>
    <th>Route</span></th>
    <th>Type</th>
    <th>Headers / Body</th>
    <th>Returns</th>
    <th>Description</th>
  </tr>
</thead>
<tbody>
  <tr>
    <td><pre>/auth/register</pre></td>
    <td><b>POST</b></td>
    <td><b>body (json):</b> { firstName, lastName, email, password, confirmPassword }</td>
    <td> - </td>
    <td> Register an account with an email adress</td>
  </tr>
  <tr>
    <td><pre>/auth/login</pre></td>
    <td><b>POST</b></td>
    <td><b>body (json):</b> { email, password }</td>
    <td><b>json:</b> { accessToken, refreshToken }</td>
    <td> Login with email adress and password.</td>
  </tr>
  <tr>
    <td><pre>/auth/refresh</pre></td>
    <td><b>POST</b></td>
    <td><b>body (json):</b> { token, email }</td>
    <td><b>json:</b> { accessToken, refreshToken }</td>
    <td> Refresh login session </td>
  </tr>
  <tr>
    <td><pre>/auth/logout</pre></td>
    <td><b>DELETE</b></td>
    <td><b>body (json):</b> { token }</td>
    <td>-</td>
    <td> Logout </td>
  </tr>
  <tr>
    <td><pre>/auth/check</pre></td>
    <td><b>GET</b></td>
    <td><b>header (key-string):</b> authorization-"{refreshToken} {accessToken}"</td>
    <td>-</td>
    <td> Check if login session is active </td>
  </tr>
</tbody>
</table>

### Ingredients: _/ingr_

<table >
<thead>
  <tr>
    <th>Route</span></th>
    <th>Type</th>
    <th>Headers / Body</th>
    <th>Returns</th>
    <th>Description</th>
  </tr>
</thead>
<tbody>
  <tr>
    <td><pre>/ingr/search</pre></td>
    <td><b>GET</b></td>
    <td><b>body (json):</b> { query }</td>
    <td><b>json:</b> { rows }</td>
    <td> Get all ingredients with name containing the query string </td>
  </tr>
    <tr>
    <td><pre>/ingr/getIngredient</pre></td>
    <td><b>GET</b></td>
    <td><b>body (json):</b> { id }</td>
    <td><b>json:</b> { row }</td>
    <td> Get a full row of the ingredient with this ID </td>
  </tr>
  </tr>
    <tr>
    <td><pre>/ingr/getIngredientList</pre></td>
    <td><b>GET</b></td>
    <td><b>body (json):</b> { [id] }</td>
    <td><b>json:</b> { [row] }</td>
    <td> Get a full rows of the ingredients with the IDs in the list </td>
  </tr>
  </tr>
    <tr>
    <td><pre>/ingr/getRequirements</pre></td>
    <td><b>GET</b></td>
    <td><b>body (json):</b> { id }</td>
    <td><b>json:</b> { [row] }</td>
    <td> Get a list of all resources that the ingredient with the given ID requires </td>
  </tr>
  </tr>
    <tr>
    <td><pre>/ingr/getResource</pre></td>
    <td><b>GET</b></td>
    <td><b>body (json):</b> { id }</td>
    <td><b>json:</b> { row }</td>
    <td> Get a full row of the resource with this ID </td>
  </tr>
  </tr>
    <tr>
    <td><pre>/ingr/getResourceList</pre></td>
    <td><b>GET</b></td>
    <td><b>body (json):</b> { [id] }</td>
    <td><b>json:</b> { [row] }</td>
    <td> Get a full rows of the resources with the IDs in the list </td>
  </tr>
  </tr>
    <tr>
    <td><pre>/ingr/getModel</pre></td>
    <td><b>GET</b></td>
    <td><b>body (json):</b> { id }</td>
    <td><b>json:</b> { row }</td>
    <td> Get a full row of the model with this ID </td>
  </tr>
  </tr>
    <tr>
    <td><pre>/ingr/getModelList</pre></td>
    <td><b>GET</b></td>
    <td><b>body (json):</b> { [id] }</td>
    <td><b>json:</b> { [row] }</td>
    <td> Get a full rows of the models with the IDs in the list </td>
  </tr>
</tbody>
</table>

### Questionnaire: _/quest_

<table >
<thead>
  <tr>
    <th>Route</span></th>
    <th>Type</th>
    <th>Headers / Body</th>
    <th>Returns</th>
    <th>Description</th>
  </tr>
</thead>
<tbody>
  <tr>
    <td><pre>/quest/save</pre></td>
    <td><b>POST</b></td>
    <td><b>body (json):</b> { questionnaire }</td>
    <td>-</td>
    <td> Save a questionnaire on the server. <b>questionnaire</b> must be a string.  </td>
  
</tbody>
</table>

## Tips

- Use **Postman** for testing post requests manually.
