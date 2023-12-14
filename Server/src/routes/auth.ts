import express, { Response, Request } from "express";
import * as bcrypt from "bcrypt";
import * as jwt from "jsonwebtoken";
import Database from "../database";

// ----------------------------------------------------------------------------

let router = express.Router();

/**
 * Route: /auth/register (POST)
 *
 * Registers a user account.
 *
 * # Input (JSON):
 * {
 *      firstName:          string,
 *      lastName:           string,
 *      email:              string,
 *      password:           string,
 *      confirmPassword:    string
 * }
 *
 * # Output:
 * The body of the response contains a message in text format for debugging purposes only.
 */
router.post("/register", async (req: any, res: any) => {
  const firstName = req.body.firstName;
  const lastName = req.body.lastName;
  let password = req.body.password;
  const confirmPassword = req.body.confirmPassword;
  const email = req.body.email;

  if (password != confirmPassword) {
    res.status(401).send("Passwords don't match");
    return;
  }

  password = await bcrypt.hash(confirmPassword, 10);

  let db = Database.getInstance().userdb();
  db.get(
    "SELECT Email FROM User WHERE Email = ?",
    [email],
    async (error: any, row: any) => {
      if (error) {
        console.error(error);
      }
      if (row) {
        // User already exists
        res
          .status(401)
          .send("Email adress is already used for a different account");
        return;
      } else {
        db.run(
          "INSERT INTO User (UserID, FirstName, LastName, Password, Email) VALUES (NULL, ?, ?, ?, ?)",
          [firstName, lastName, password, email],
        );
        res.status(201).send("Registration successful");
      }
    },
  );
});

/** Login
 *
 * Route: /auth/login (POST)
 *
 * # Input (JSON)
 * {
 *      email:      string,
 *      password:   string
 * }
 *
 * # Output (JSON):
 * {
 *      accessToken:    string,
 *      refreshToken:   string
 * }
 */
router.post("/login", async (req: any, res: any) => {
  const email: string = req.body.email;
  const password: string = req.body.password;

  let db = Database.getInstance().userdb();
  db.get(
    "SELECT Password FROM User WHERE Email = ?",
    [email],
    async (error: any, row: any) => {
      if (error) {
        console.error(error);
      }
      if (row.Password) {
        if (await bcrypt.compare(password, row.Password)) {
          // Login successful
          const accessToken = generateAccessToken(email);
          const refreshToken = generateRefreshToken(email);

          res.json({
            accessToken: accessToken,
            refreshToken: refreshToken,
          });
        } else {
          res.status(401).send("Failed to login. Wrong email/password.");
        }
      } else {
        res.status(401).send("Failed to login. Wrong email/password.");
      }
    },
  );
});

/** Refresh login session
 *
 * Route: /auth/refresh
 *
 * # Input (JSON):
 * {
 *      token: string, // The refresh token
 *      email: string
 * }
 *
 * # Output (JSON):
 * {
 *      accessToken:    string,
 *      refreshToken:   string
 * }
 */
router.post("/refresh", (req, res) => {
  if (!refreshTokens.includes(req.body.token))
    res.status(400).send("Refresh Token Invalid");

  // Remove the old token
  refreshTokens = refreshTokens.filter((t) => t != req.body.token);

  const accessToken = generateAccessToken(req.body.email);
  const refreshToken = generateRefreshToken(req.body.email);

  res.json({ accessToken: accessToken, refreshToken: refreshToken });
});

// Logout
//
// # Body
// {
//      token: string,
// }
router.delete("/logout", (req, res) => {
  // Remove the old token
  refreshTokens = refreshTokens.filter((t) => {
    return t != req.body.token;
  });

  res.status(204).send("Logged out!");
});

// Check
//
// Check if logged in.
router.get("/check", validateToken, (_req, res) => {
  res.send("Logged in");
});

/*
 * Generate an access token that expires after 15 min.
 */
function generateAccessToken(email: string): string {
  return jwt.sign({ email: email }, process.env.ACCESS_TOKEN_SECRET!, {
    expiresIn: "15m",
  });
}

/*
 * Active refreshTokens.
 */
let refreshTokens: string[] = [];

/*
 * Generate a refresh token that expires in 20 minutes.
 */
function generateRefreshToken(email: string): string {
  const refreshToken = jwt.sign(
    { email: email },
    process.env.REFRESH_TOKEN_SECRET!,
    {
      expiresIn: "20m",
    },
  );

  refreshTokens.push(refreshToken);

  return refreshToken;
}

export type ValidatedRequest = Request & { email: string };

/*
 * Checks the authorization header inside the request. If the user is logged in, it will call next().
 * Use this a middleware method for ExpressJS.
 */
export function validateToken(req: Request, res: Response, next: any) {
  if (process.env.VALIDATION == "FALSE") {
    return next();
  }

  const header = req.headers["authorization"];
  if (!header) {
    res.status(400).send("Authorization header is missing");
    return;
  }

  const token = header.split(" ")[1];

  if (token == null) {
    res.status(401).send("Missing access token");
    return;
  }

  jwt.verify(
    token,
    process.env.ACCESS_TOKEN_SECRET!,
    (err: any, email: any) => {
      if (err) {
        res.status(403).send("Access token invalid");
        return;
      }

      (req as ValidatedRequest).email = email.email;
      next();
    },
  );
}

export default router;
