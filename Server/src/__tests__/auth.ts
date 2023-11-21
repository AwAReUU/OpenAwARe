import supertest, { Request } from "supertest";
import dotenv from "dotenv";

import app from "../app";
import Database from "../database";

// ----------------------------------------------------------------------------

const api = supertest(app);

let refreshToken: string;
let accessToken: string;

beforeAll(() => {
  dotenv.config();
  Database.testing = true;
});

afterAll(() => {
  Database.getInstance().delete();
});

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

  //TODO: Check in table
});

// 2) Test login
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

// 3) Test refresh login
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

// 4) Test check login
test("POST /auth/check", async () => {
  let header = refreshToken + " " + accessToken;
  await api.get("/auth/check").set("authorization", header).expect(200);
});

// 5) Test logout
test("DELETE /auth/logout", async () => {
  let body = {
    token: refreshToken,
  };
  await api.delete("/auth/logout").send(body).expect(204);
});

// 6) Test login after logout
// This should work fine! Only after the accessToken is expired, loggin in should be imposible.
// Read the Security part of the Architecture Document for a detailed explenation.
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

// 7) Logout again for next test
test("DELETE /auth/logout", async () => {
  let body = {
    token: refreshToken,
  };
  await api.delete("/auth/logout").send(body).expect(204);
});

// 8) Test refreshToken after logout
// This should not work after logout.
test("POST /auth/refreshToken", async () => {
  let body = {
    email: "marvinfisher@outlook.com",
    token: refreshToken,
  };
  await api.post("/auth/refreshToken").send(body).expect(400);
});
