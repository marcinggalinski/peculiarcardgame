export interface AddUserRequest {
  displayedName?: string;
  password: string;
  username: string;
}
export interface GetUserResponse {
  displayedName: string;
  id: number;
  username: string;
}
export interface RefreshAccessTokenRequest {
  refreshToken: string;
}
export interface RevokeRefreshTokenRequest {
  refreshToken: string;
}
export interface SignInResponse {
  accessToken: string;
  refreshToken: string;
}
export interface UpdateUserRequest {
  displayedNameUpdate?: string;
  passwordUpdate?: string;
}
