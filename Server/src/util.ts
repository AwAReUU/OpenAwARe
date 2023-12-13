import { Response } from "express";

export function assert_res(
  res: Response,
  value: boolean,
  msg?: string,
  status: number = 400,
): boolean {
  if (!value) {
    if (msg) res.status(status).send(msg);
    else res.status(status).send("Something went wrong");
  }

  return value;
}

/**
 * Check if name is formatted correctly:
 * - name should only consist of letters, numbers, dashes and underscores.
 * - name should contain at least 3 letters.
 * - name should not be longer than 20 characters.
 */
export function validName(name: string): boolean {
  let pat = new RegExp(/^[a-zA-Z-_0-9]+$/i);
  let letters = name.match(/[a-zA-Z]/);
  if (letters && letters.length < 3) return false;
  return pat.test(name) && name.length <= 20;
}

export function validEmail(email: string): boolean {
  let pat = new RegExp(/^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/i);
  return pat.test(email);
}

export function validPassword(password: string): boolean {
  let pat = new RegExp(/^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$/i);
  return pat.test(password);
}
