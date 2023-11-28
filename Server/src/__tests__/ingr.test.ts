import { api } from "./setup";
import { get_authorization_header } from "./util";

// ----------------------------------------------------------------------------

describe("Test if routes are protected.", () => {
  test("GET /ingr/search", async () => {
    let body = {
      query: "apple",
    };
    await api.get("/ingr/search").send(body).expect(400);
  });

  test("GET /ingr/getIngredient", async () => {
    let body = {
      id: 0,
    };
    await api.get("/ingr/getIngredient").send(body).expect(400);
  });

  test("GET /ingr/getIngredientList", async () => {
    let body = {
      ids: [0, 1, 2],
    };
    await api.get("/ingr/getIngredientList").send(body).expect(400);
  });

  test("GET /ingr/getRequirements", async () => {
    let body = {
      ids: 0,
    };
    await api.get("/ingr/getRequirements").send(body).expect(400);
  });

  test("GET /ingr/getResource", async () => {
    let body = {
      ids: 0,
    };
    await api.get("/ingr/getResource").send(body).expect(400);
  });

  test("GET /ingr/getResourceList", async () => {
    let body = {
      ids: [0, 1, 2],
    };
    await api.get("/ingr/getResourceList").send(body).expect(400);
  });

  test("GET /ingr/getModel", async () => {
    let body = {
      ids: 0,
    };
    await api.get("/ingr/getModel").send(body).expect(400);
  });

  test("GET /ingr/getModelList", async () => {
    let body = {
      ids: [0, 1, 2],
    };
    await api.get("/ingr/getModelList").send(body).expect(400);
  });
});

describe("Test if routes are working properly", () => {
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
