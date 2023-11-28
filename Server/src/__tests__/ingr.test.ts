import { api } from "./setup";
import { get_authorization_header } from "./util";

// ----------------------------------------------------------------------------

describe("Test all protected routes in /ingr/ without an authorization header. \
          Each route should return a 400 error", () => {
  test("GET /ingr/search", async () => {
    let body = {
      query: "apple",
    };
    await api.get("/ingr/search").send(body).expect(400);
  });
});

describe("Test all protected routes in /ingr/ with an active login session.", () => {
  test("GET /ingr/search", async () => {
    let body = {
      query: "apple",
    };
    let header = await get_authorization_header();
    await api
      .get("/ingr/search")
      .set("authorization", header)
      .send(body)
      .expect(200);
  });
});
