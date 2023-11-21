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

// 3) Test check login
test("POST /auth/check", async () => {
  let header = refreshToken + " " + accessToken;
  let ret: any = await api
    .get("/auth/check")
    .set("authorization", header)
    .expect(200);
});
