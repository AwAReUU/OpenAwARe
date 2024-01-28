import { api } from "./setup";
import { get_authorization_header } from "./util";

describe("Test if routes are protected.", () => {
  test("POST /quest/save", async () => {
    let body = {
      questionnaire: "Dit is een voorbeeld",
    };
    await api.post("/quest/save").send(body).expect(400);
  });
});

test("POST /quest/save", async () => {
  let body = {
    questionnaire: "Dit is een voorbeeld",
  };
  let header = await get_authorization_header();
  await api
    .post("/quest/save")
    .set("authorization", header)
    .send(body)
    .expect(200);
});
