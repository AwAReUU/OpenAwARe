import bodyParser from "body-parser";
import express, { Express, Request, Response } from "express";

import auth from "./routes/auth";
import quest from "./routes/quest";
import ingr from "./routes/ingr";

// ----------------------------------------------------------------------------

const app: Express = express();

// Setup automatic parsing to JSON
app.use(bodyParser.json());

// ----------------------------------------------------------------------------
// Routes:

// Add home route to check if server is online/offline.
app.get("/", (_req: Request, res: Response) => {
    res.send("Server online");
});

app.use("/auth", auth);
app.use("/quest", quest);
app.use("/ingr", ingr);

export default app;
