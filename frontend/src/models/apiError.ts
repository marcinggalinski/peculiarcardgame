export default class ApiError extends Error {
  constructor(
    public statusCode: number,
    message: string
  ) {
    super(message);
    this.name = "ApiError";
  }

  toString = () => `ApiError ${this.statusCode}: ${this.message}`;
}
