import axios, { type AxiosInstance } from "axios";

export default abstract class ApiServiceBase {
  readonly instance: AxiosInstance;

  constructor(public baseUrl: string) {
    this.instance = axios.create({
      baseURL: this.baseUrl,
    });
  }

  setBearerToken(token: string) {
    this.instance.defaults.headers.post.Authorization = `Bearer ${token}`;
    this.instance.defaults.headers.patch.Authorization = `Bearer ${token}`;
    this.instance.defaults.headers.delete.Authorization = `Bearer ${token}`;
  }
}
