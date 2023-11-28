import bodyParser from "body-parser";
import express, { Express, Request, Response } from "express";

import auth from "./routes/auth";
import quest from "./routes/quest";
import ingr from "./routes/ingr";

// ----------------------------------------------------------------------------

const app: Express = express();
app.use(bodyParser.json());

// ----------------------------------------------------------------------------
// Routes:

app.get("/", (_req: Request, res: Response) => {
  res.send("Hello world!");
});

app.use("/auth", auth);
app.use("/quest", quest);
app.use("/ingr", ingr);

export default app;
