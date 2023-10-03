## How to run

1. Install dependencies:

```console
$ npm install
```

2. Start the server:

```console
$ npm run dev
```

## Routes

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
    <td ><pre>/auth/register</pre></td>
    <td ><b>POST</b></td>
    <td > <b>body (json):</b> { firstName, lastName, email, password, confirmPassword }</td>
    <td > - </td>
    <td > Register an account with an email adress</td>
  </tr>
  <tr>
    <td ><pre>/auth/login</pre></td>
    <td ><b>POST</b></td>
    <td > <b>body (json):</b> { email, password }</td>
    <td > <b>json:</b> { accessToken, refreshToken }</td>
    <td > Login with email adress and password.</td>
  </tr>
  <tr>
    <td ><pre>/auth/refreshToken</pre></td>
    <td ><b>POST</b></td>
    <td > <b>body (json):</b> { token, email }</td>
    <td > <b>json:</b> { accessToken, refreshToken }</td>
    <td > Refresh login session </td>
  </tr>
  <tr>
    <td ><pre>/auth/logout</pre></td>
    <td ><b>POST</b></td>
    <td > <b>body (json):</b> { token }</td>
    <td >-</td>
    <td > Logout </td>
  </tr>
  <tr>
    <td ><pre>/auth/check</pre></td>
    <td ><b>GET</b></td>
    <td > <b>header (key-string):</b> authorization-"{refreshToken} {accessToken}"</td>
    <td >-</td>
    <td > Check if login session is active </td>
  </tr>
  <tr>
    <td ><pre>/ingr/search</pre></td>
    <td ><b>POST</b></td>
    <td > <b>body (json):</b> { query }</td>
    <td > <b>json:</b> { rows }</td>
    <td > Get all ingredients with name containing the query string </td>
  </tr>
    <tr>
    <td ><pre>/ingr/getingredient</pre></td>
    <td ><b>POST</b></td>
    <td > <b>body (json):</b> { id, type }</td>
    <td > <b>json:</b> { rows }</td>
    <td > Get a full row of the ingredient with this id and type </td>
  </tr>
</tbody>
</table>


## Tips

- Use **Postman** for testing post requests.
