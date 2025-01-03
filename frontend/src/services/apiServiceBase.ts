import axios, { type AxiosInstance } from "axios";

export default abstract class ApiServiceBase {
  readonly instance: AxiosInstance;

  constructor(public baseUrl: string) {
    this.instance = axios.create({
      baseURL: this.baseUrl,
    });
  }

  setBearerToken(token: string) {
    this.instance.defaults.headers.common.Authorization = `Bearer ${token}`;
  }

  unsetBearerToken() {
    delete this.instance.defaults.headers.common.Authorization;
  }
}
