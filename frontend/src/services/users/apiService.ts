import type { AddUserRequest, GetUserResponse, UpdateUserRequest } from "@/models/users/api";
import ApiError from "@/models/apiError";
import ApiServiceBase from "@/services/apiServiceBase";

export default class ApiService extends ApiServiceBase {
  async addUser(username: string, password: string, displayedName?: string): Promise<GetUserResponse> {
    const request: AddUserRequest = {
      displayedName,
      username,
      password,
    };
    const response = await this.instance.post<GetUserResponse>("users", request);

    switch (response.status) {
      case 200:
        return response.data;
      default:
        throw new ApiError(response.status, response.statusText);
    }
  }

  async deleteUser(userId: number): Promise<void> {
    const response = await this.instance.delete(`users/${userId}`);

    switch (response.status) {
      case 200:
        return;
      default:
        throw new ApiError(response.status, response.statusText);
    }
  }

  async getUser(userId: number): Promise<GetUserResponse> {
    const response = await this.instance.get<GetUserResponse>(`users/${userId}`);

    switch (response.status) {
      case 200:
        return response.data;
      default:
        throw new ApiError(response.status, response.statusText);
    }
  }

  async updateUser(userId: number, displayedNameUpdate?: string, passwordUpdate?: string): Promise<GetUserResponse> {
    const request: UpdateUserRequest = { displayedNameUpdate, passwordUpdate };
    const response = await this.instance.patch<GetUserResponse>(`users/${userId}`, request);

    switch (response.status) {
      case 200:
        return response.data;
      default:
        throw new ApiError(response.status, response.statusText);
    }
  }
}
