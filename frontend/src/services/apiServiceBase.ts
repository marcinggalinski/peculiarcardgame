import axios, { type AxiosInstance } from "axios";

export default abstract class ApiServiceBase {
  readonly instance: AxiosInstance;

  constructor(public baseUrl: string) {
    this.instance = axios.create({
      baseURL: this.baseUrl,
    });
  }
}
