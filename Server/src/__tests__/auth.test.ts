import Database from "../database";
import { api } from "./setup";

// ----------------------------------------------------------------------------

let refreshToken: string;
let accessToken: string;

// 1) Test registering an account
test("POST /auth/register", async () => {
  let body = {
    firstName: "Marvin",
    lastName: "Fisher",
    email: "marvinfisher@outlook.com",
    password: "123test",
    confirm_password: "123test",
  };
  await api.post("/auth/register").send(body).expect(201);

  // Check if user data is correct. Password will be checked later.
  Database.getInstance()
    .userdb()
    .get(
      "SELECT * FROM User WHERE Email = ?",
      [body.email],
      async (error: any, row: any) => {
        if (error) {
          console.error(error);
        }
        expect(row.FirstName).toEqual(body.firstName);
        expect(row.LastName).toEqual(body.lastName);
        expect(row.Email).toEqual(body.email);
      },
    );
});

// 2) Test login with wrong password
test("POST /auth/login", async () => {
  let body = {
    email: "marvinfisher@outlook.com",
    password: "54321test",
  };
  await api.post("/auth/login").send(body).expect(401);
});

// 3) Test login with correct password
test("POST /auth/login", async () => {
  let body = {
    email: "marvinfisher@outlook.com",
    password: "123test",
  };
  let ret: any = await api
    .post("/auth/login")
    .send(body)
    .expect(200)
    .expect("Content-Type", /application\/json/);

  refreshToken = ret.body.refreshToken;
  accessToken = ret.body.accessToken;
});

// 4) Test refresh login with incorrect token
test("POST /auth/refreshToken", async () => {
  let body = {
    email: "marvinfisher@outlook.com",
    token: "abcdef",
  };
  await api.post("/auth/refreshToken").send(body).expect(400);
});

// 5) Test refresh login with correct token
test("POST /auth/refreshToken", async () => {
  let body = {
    email: "marvinfisher@outlook.com",
    token: refreshToken,
  };
  let ret: any = await api
    .post("/auth/refreshToken")
    .send(body)
    .expect(200)
    .expect("Content-Type", /application\/json/);
  refreshToken = ret.body.refreshToken;
  accessToken = ret.body.accessToken;
});

// 6) Test check login
test("POST /auth/check", async () => {
  let header = refreshToken + " " + accessToken;
  await api.get("/auth/check").set("authorization", header).send().expect(200);
});

// 7) Test logout
test("DELETE /auth/logout", async () => {
  let body = {
    token: refreshToken,
  };
  await api.delete("/auth/logout").send(body).expect(204);
});

// 8) Test login after logout
// This should work fine! Only after the accessToken is expired, loggin in should be imposible.
// Read the Security part of the Architecture Document for a detailed explanation.
test("POST /auth/login", async () => {
  let body = {
    email: "marvinfisher@outlook.com",
    password: "123test",
  };
  await api
    .post("/auth/login")
    .send(body)
    .expect(200)
    .expect("Content-Type", /application\/json/);
});

// 9) Logout again for next test
test("DELETE /auth/logout", async () => {
  let body = {
    token: refreshToken,
  };
  await api.delete("/auth/logout").send(body).expect(204);
});

// 10) Test refreshToken after logout
// This should not work after logout.
test("POST /auth/refreshToken", async () => {
  let body = {
    email: "marvinfisher@outlook.com",
    token: refreshToken,
  };
  await api.post("/auth/refreshToken").send(body).expect(400);
});
