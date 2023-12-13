import bodyParser from "body-parser";
import express, { Express, Request, Response } from "express";

import auth from "./routes/auth";
import quest from "./routes/quest";
import ingr from "./routes/ingr";

// ----------------------------------------------------------------------------

const app: Express = express();

// Setup automatic parsing to JSON
app.use(bodyParser.json());

app.use(function(error: any, req: any, res: any, next: any) {
    if (error instanceof SyntaxError) {
        res.status(400).send("SyntaxError: request is not formatted correctly.");
    } else {
        next();
    }
});

// ----------------------------------------------------------------------------
// Routes:

app.get("/", (_req: Request, res: Response) => {
    res.send("Hello world!");
});

app.use("/auth", auth);
app.use("/quest", quest);
app.use("/ingr", ingr);

export default app;
