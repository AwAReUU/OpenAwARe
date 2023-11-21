import bodyParser from "body-parser";
import express, { Express, Request, Response } from "express";

import auth from "./routes/authentication";
import data from "./routes/data";
import quest from "./routes/questionaire";
import ingr from "./routes/ingredients";

// ----------------------------------------------------------------------------

const app: Express = express();
app.use(bodyParser.json());

// ----------------------------------------------------------------------------
// Routes:

app.get("/", (_req: Request, res: Response) => {
    res.send("Hello world!");
});

app.use("/auth", auth);
app.use("/data", data);
app.use("/quest", quest);
app.use("/ingr", ingr);

export default app;
