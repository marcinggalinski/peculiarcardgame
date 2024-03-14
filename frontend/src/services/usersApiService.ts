import type { GetUserResponse, AddUserRequest, SignInResponse } from "@/models/users/api";
import ApiError from "@/models/apiError";
import ApiServiceBase from "@/services/apiServiceBase";

export default class UsersApiService extends ApiServiceBase {
  async addUser(username: string, password: string, displayedName?: string) {
    const request: AddUserRequest = {
      displayedName,
      username,
      password,
    };
    const response = await this.instance.post<GetUserResponse>("users", request);
    if (response.status !== 201) {
      throw new ApiError(response.status, response.statusText);
    }
    return response.data;
  }

  async deleteUser(userId: number) {
    const response = await this.instance.delete(`users/${userId}`);
    if (response.status !== 200) {
      throw new ApiError(response.status, response.statusText);
    }
  }

  async getUser(userId: number) {
    const response = await this.instance.get<GetUserResponse>(`users/${userId}`);
    if (response.status !== 200) {
      throw new ApiError(response.status, response.statusText);
    }
    return response.data;
  }

  async signIn(username: string, password: string) {
    const response = await this.instance.post<SignInResponse>(
      "users/signin",
      {},
      {
        headers: {
          Authorization: `Basic ${btoa(`${username}:${password}`)}`,
        },
      }
    );
    if (response.status !== 200) {
      throw new ApiError(response.status, response.statusText);
    }
    return response.data;
  }
}
