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

// ----------------------------------------------------------------------------
// We only test if the output is in the correct format for each route,
// because each route in `/ingr/` is just a simple SQL query.

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
for (let id of [1, 2, 3, 9, 10, 1000, 193258]) {
  test_get_ingredient(id);
  test_get_requirements(id);
  test_get_resource(id);
  test_get_model(id);
}
for (let ids of [
  [0, 1, 2, 3, 9, 10, 1000, 193258],
  [9, 6, 90],
  [1, 2, 3, 4, 9],
]) {
  test_get_ingredient_list(ids);
  test_get_resource_list(ids);
  test_get_model_list(ids);
}

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
      test_ingredient_format(ingr);
    }
  });
}

function test_get_ingredient(id: number) {
  test("GET /ingr/getIngredient", async () => {
    let body = {
      id: id,
    };
    let header = await get_authorization_header();
    let ret = await api
      .get("/ingr/getIngredient")
      .set("authorization", header)
      .send(body)
      .expect(200);

    if (ret.body) test_ingredient_format(ret.body);
  });
}

function test_get_ingredient_list(ids: number[]) {
  test("GET /ingr/getIngredientList", async () => {
    let body = {
      ids: ids,
    };
    let header = await get_authorization_header();
    let ret = await api
      .get("/ingr/getIngredientList")
      .set("authorization", header)
      .send(body)
      .expect(200);

    for (let ingr of ret.body) test_ingredient_format(ingr);
  });
}

function test_get_requirements(id: number) {
  test("GET /ingr/getRequirements", async () => {
    let body = {
      id: id,
    };
    let header = await get_authorization_header();
    let ret = await api
      .get("/ingr/getRequirements")
      .set("authorization", header)
      .send(body)
      .expect(200);

    for (let req of ret.body) test_requirement_format(req);
  });
}

function test_get_resource(id: number) {
  test("GET /ingr/getResource", async () => {
    let body = {
      id: id,
    };
    let header = await get_authorization_header();
    let ret = await api
      .get("/ingr/getResource")
      .set("authorization", header)
      .send(body)
      .expect(200);

    if (ret.body) test_resource_format(ret.body);
  });
}

function test_get_resource_list(ids: number[]) {
  test("GET /ingr/getResourceList", async () => {
    let body = {
      ids: ids,
    };
    let header = await get_authorization_header();
    let ret = await api
      .get("/ingr/getResourceList")
      .set("authorization", header)
      .send(body)
      .expect(200);

    for (let res of ret.body) test_resource_format(res);
  });
}

function test_get_model(id: number) {
  test("GET /ingr/getModel", async () => {
    let body = {
      id: id,
    };
    let header = await get_authorization_header();
    let ret = await api
      .get("/ingr/getModel")
      .set("authorization", header)
      .send(body)
      .expect(200);

    if (ret.body) test_model_format(ret.body);
  });
}

function test_get_model_list(ids: number[]) {
  test("GET /ingr/getModelList", async () => {
    let body = {
      ids: ids,
    };
    let header = await get_authorization_header();
    let ret = await api
      .get("/ingr/getModelList")
      .set("authorization", header)
      .send(body)
      .expect(200);

    for (let model of ret.body) test_model_format(model);
  });
}

function test_ingredient_format(ingr: any) {
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

function test_requirement_format(req: any) {
  expect(req.IngredientID).not.toBeNull();
  expect(isUnsignedInteger(req.IngredientID)).toBeTruthy();

  expect(req.ResourceID).not.toBeNull();
  expect(isUnsignedInteger(req.ResourceID)).toBeTruthy();

  expect(req.ResPerIngr).not.toBeNull();
  expect(isNumber(req.ResPerIngr)).toBeTruthy();
  expect(req.ResPerIngr).toBeGreaterThan(0);

  // ResPerIngr should exactly be 1 if IngredientID == ResourceID.
  if (req.IngredientID == req.ResourceID) {
    expect(req.ResPerIngr).toBe(1);
  }
}

const RESOURCE_TYPES = ["Water", "Plant", "Animal"];
function test_resource_format(res: any) {
  expect(res.ResourceID).not.toBeNull();
  expect(isUnsignedInteger(res.ResourceID)).toBeTruthy();

  expect(res.ModelID).not.toBeNull();
  expect(isUnsignedInteger(res.ModelID)).toBeTruthy();

  if (res.GramsPerModel) {
    expect(isUnsignedInteger(res.GramsPerModel)).toBeTruthy();
    expect(res.GramsPerModel).toBeGreaterThan(0);
  }

  expect(res.Type).not.toBeNull();
  expect(RESOURCE_TYPES).toContain(res.Type);
}

const MODEL_TYPES = ["Water", "Animal", "Plant", "Shapes"];
function test_model_format(model: any) {
  expect(model.ModelID).not.toBeNull();
  expect(isUnsignedInteger(model.ModelID)).toBeTruthy();

  expect(model.Type).not.toBeNull();
  expect(MODEL_TYPES).toContain(model.Type);

  expect(model.PrefabPath).not.toBeNull();

  expect(model.RealHeight).not.toBeNull();
  expect(isNumber(model.RealHeight)).toBeTruthy();
  expect(model.RealHeight).toBeGreaterThan(0);
}
