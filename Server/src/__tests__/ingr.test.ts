import { api } from "./setup";
import { get_authorization_header, isNumber, isUnsignedInteger } from "./util";

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

// We only test if the output is in the correct format for each route,
// because each route in `/ingr/` is just a simple SQL query.
function test_ingredient_search(query: string) {
  test("GET /ingr/search", async () => {
    let body = {
      query: query,
    };
    let header = await get_authorization_header();
    let ret = await api
      .get("/ingr/search")
      .set("authorization", header)
      .send(body)
      .expect(200);

    for (let ingr of ret.body) {
      expect(ingr.IngredientID).not.toBeNull();
      expect(isUnsignedInteger(ingr.IngredientID)).toBeTruthy();

      expect(ingr.PrefName).not.toBeNull();
      expect(typeof ingr.PrefName == "string");

      // Ingredient should have one or the other (GramsPerML or GramsPerPiece)
      expect(
        (ingr.GramsPerML !== null && ingr.GramsPerPiece === null) ||
          (ingr.GramsPerML === null && ingr.GramsPerPiece !== null),
      ).toBeTruthy();

      if (ingr.GramsPerML !== null) {
        expect(isNumber(ingr.GramsPerML)).toBeTruthy();
        expect(ingr.GramsPerML).toBeGreaterThan(0);
      } else {
        expect(isNumber(ingr.GramsPerPiece)).toBeTruthy();
        expect(ingr.GramsPerPiece).toBeGreaterThan(0);
      }
    }
  });
}

for (let q of [
  "apple",
  "Pear",
  "Strawberry",
  "Beef",
  "Chicken",
  "Milk",
  "Water",
  "",
  "asssgabbsf",
  "195bskas3535kj",
  "123",
]) {
  test_ingredient_search(q);
}
