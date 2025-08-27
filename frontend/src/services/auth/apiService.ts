import type { SignInResponse, RevokeRefreshTokenRequest, RefreshAccessTokenRequest } from "@/models/users/api";
import ApiError from "@/models/apiError";
import ApiServiceBase from "@/services/apiServiceBase";

export default class ApiService extends ApiServiceBase {
  async refreshAccessToken(refreshToken: string): Promise<SignInResponse | null> {
    const request: RefreshAccessTokenRequest = { refreshToken };
    const response = await this.instance.post<SignInResponse>("auth/refresh", request);

    switch (response.status) {
      case 200:
        return response.data;
      case 401:
        return null;
      default:
        throw new ApiError(response.status, response.statusText);
    }
  }

  async revokeRefreshToken(refreshToken: string): Promise<void> {
    const request: RevokeRefreshTokenRequest = { refreshToken };
    const response = await this.instance.post("auth/revoke", request);

    switch (response.status) {
      case 200:
        return;
      default:
        throw new ApiError(response.status, response.statusText);
    }
  }

  async signIn(username: string, password: string): Promise<SignInResponse> {
    const response = await this.instance.post<SignInResponse>(
      "auth/signin",
      {},
      {
        headers: {
          Authorization: `Basic ${btoa(`${username}:${password}`)}`,
        },
      }
    );

    switch (response.status) {
      case 200:
        return response.data;
      default:
        throw new ApiError(response.status, response.statusText);
    }
  }
}
