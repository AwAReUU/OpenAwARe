import Database from "../database";
import supertest from "supertest";
import dotenv from "dotenv";
import app from "../app";

// ----------------------------------------------------------------------------

export const api = supertest(app);

// This will run before all tests for each test module.
beforeAll(() => {
  dotenv.config();
  Database.testing = true;

  // Make sure validation is enabled
  process.env.VALIDATION = "TRUE";
});

// This will run after all tests for each test module.
afterAll(() => {
  Database.getInstance().delete();
});
