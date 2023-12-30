import { api } from "./setup";

let authorization_header: string | null = null;

// Use this for testing protected routes. This will create a login session.
// Dont use this for testing the login session itself.
//
// For example:
// ```
// let header = await get_authorization_header();
// await api
//   .get("/auth/check")
//   .set("authorization", header)
//   .send()
//   .expect(200);
// ````
export async function get_authorization_header(): Promise<string> {
  if (authorization_header != null) {
    return authorization_header;
  }
  let account_details = {
    firstName: "Test",
    lastName: "Account",
    email: "testaccount@outlook.com",
    password: "1234test",
    confirmPassword: "1234test",
  };
  await api.post("/auth/register").send(account_details).then();

  let login_data = {
    email: "testaccount@outlook.com",
    password: "1234test",
  };
  let ret: any = await api.post("/auth/login").send(login_data).then();

  let refreshToken = ret.body.refreshToken;
  let accessToken = ret.body.accessToken;
  authorization_header = refreshToken + " " + accessToken;

  return authorization_header;
}

export function isNumber(n: any) {
  return /^-?[\d.]+(?:e-?\d+)?$/.test(n);
}

export function isUnsignedInteger(n: any) {
  return /^\d+$/.test(n);
}
